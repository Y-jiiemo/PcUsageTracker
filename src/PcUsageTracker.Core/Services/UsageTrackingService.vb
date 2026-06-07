Imports System
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks
Imports PcUsageTracker.Core.Enums
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Models

Namespace Services

    Public Class UsageTrackingService
        Implements IDisposable

        Private ReadOnly _repository As IUsageRepository
        Private ReadOnly _systemMonitor As ISystemMonitor
        Private ReadOnly _idleDetector As IdleDetectionService
        Private ReadOnly _calculator As UsageCalculator
        Private _currentSession As UsageSession
        Private _tickTimer As Timer
        Private _saveTimer As Timer
        Private _stopwatch As Stopwatch
        Private _lastTickTime As DateTime
        Private _activeAccumulator As Double
        Private _idleAccumulator As Double
        Private _isSaving As Boolean
        Private _disposed As Boolean

        Public Sub New(repository As IUsageRepository,
                       systemMonitor As ISystemMonitor,
                       idleDetector As IdleDetectionService,
                       calculator As UsageCalculator)
            _repository = repository
            _systemMonitor = systemMonitor
            _idleDetector = idleDetector
            _calculator = calculator
        End Sub

        Public Async Function StartTrackingAsync() As Task
            _currentSession = Await _repository.GetCurrentSessionAsync()

            If _currentSession Is Nothing Then
                _currentSession = New UsageSession With {
                    .StartTime = DateTime.Now,
                    .Status = SessionStatus.Active
                }
                Await _repository.SaveSessionAsync(_currentSession)
            End If

            _activeAccumulator = _currentSession.ActiveSeconds
            _idleAccumulator = _currentSession.IdleSeconds
            _stopwatch = Stopwatch.StartNew()
            _lastTickTime = DateTime.Now

            _tickTimer = New Timer(AddressOf OnTick, Nothing, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))
            _saveTimer = New Timer(AddressOf OnSave, Nothing, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
        End Function

        Public Async Function StopTrackingAsync() As Task
            _tickTimer?.Dispose()
            _tickTimer = Nothing
            _saveTimer?.Dispose()
            _saveTimer = Nothing
            _stopwatch?.Stop()

            Await SaveSessionAsync()
        End Function

        Private Sub OnTick(state As Object)
            Try
                Dim now = DateTime.Now
                Dim elapsed = (now - _lastTickTime).TotalSeconds
                If elapsed <= 0 Then elapsed = 5
                _lastTickTime = now

                Dim isIdle = _idleDetector.IsUserIdle()

                If isIdle Then
                    _idleAccumulator += elapsed
                    _currentSession.Status = SessionStatus.Idle
                Else
                    _activeAccumulator += elapsed
                    _currentSession.Status = SessionStatus.Active
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Tracking tick error: {ex.Message}")
            End Try
        End Sub

        Private Sub OnSave(state As Object)
#Disable Warning BC42358
            SaveSessionAsync()
#Enable Warning BC42358
        End Sub

        Private Async Function SaveSessionAsync() As Task
            If _isSaving OrElse _currentSession Is Nothing Then Return
            _isSaving = True

            Try
                _currentSession.ActiveSeconds = CInt(_activeAccumulator)
                _currentSession.IdleSeconds = CInt(_idleAccumulator)
                Await _repository.UpdateSessionAsync(_currentSession)

                Await UpdateDailySummaryAsync()
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}")
            Finally
                _isSaving = False
            End Try
        End Function

        Private Async Function UpdateDailySummaryAsync() As Task
            Try
                Dim today = Date.Today
                Dim sessions = Await _repository.GetTodaySessionsAsync()
                Dim summary = Await _repository.GetOrCreateDailySummaryAsync(today)

                summary.SessionCount = sessions.Count
                summary.TotalActiveSeconds = CInt(sessions.Sum(Function(s) s.ActiveSeconds))
                summary.TotalIdleSeconds = CInt(sessions.Sum(Function(s) s.IdleSeconds))

                If sessions.Any() Then
                    summary.FirstActiveTime = sessions.Min(Function(s) s.StartTime)
                    summary.LastActiveTime = sessions.Max(Function(s) If(s.EndTime, DateTime.Now))
                End If

                Await _repository.UpdateDailySummaryAsync(summary)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Daily summary error: {ex.Message}")
            End Try
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                _tickTimer?.Dispose()
                _saveTimer?.Dispose()
                _stopwatch?.Stop()
                _disposed = True
            End If
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace