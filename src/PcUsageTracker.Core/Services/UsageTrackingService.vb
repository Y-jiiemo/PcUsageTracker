Imports System
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
        Private _currentSession As UsageSession
        Private _timer As Timer
        Private _activeAccumulator As Integer
        Private _idleAccumulator As Integer
        Private _disposed As Boolean

        Public Sub New(repository As IUsageRepository, systemMonitor As ISystemMonitor)
            _repository = repository
            _systemMonitor = systemMonitor
            _idleDetector = New IdleDetectionService(systemMonitor)
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

            _timer = New Timer(AddressOf OnTimerTick, Nothing, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))
        End Function

        Public Async Function StopTrackingAsync() As Task
            _timer?.Dispose()
            _timer = Nothing

            If _currentSession IsNot Nothing Then
                _currentSession.EndTime = DateTime.Now
                _currentSession.ActiveSeconds = _activeAccumulator
                _currentSession.IdleSeconds = _idleAccumulator
                Await _repository.UpdateSessionAsync(_currentSession)
            End If
        End Function

        Private Sub OnTimerTick(state As Object)
            Try
                If _currentSession.Status = SessionStatus.Sleep Then Return

                Dim isIdle = _idleDetector.IsUserIdle()

                If isIdle Then
                    _idleAccumulator += 5
                    _currentSession.Status = SessionStatus.Idle
                Else
                    _activeAccumulator += 5
                    _currentSession.Status = SessionStatus.Active
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Tracking error: {ex.Message}")
            End Try
        End Sub

        Private Async Sub OnSessionStateChanged(sender As Object, status As SessionStatus)
            If status = SessionStatus.Sleep OrElse status = SessionStatus.Locked Then
                Await StopTrackingAsync()
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                _timer?.Dispose()
                _disposed = True
            End If
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace