Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports PcUsageTracker.Core.Enums
Imports PcUsageTracker.Core.Interfaces

Namespace System

    Public Class SystemMonitor
        Implements ISystemMonitor
        Implements IDisposable

        Private _timer As Timer
        Private _currentStatus As SessionStatus
        Private _disposed As Boolean

        Public Event SessionStateChanged As EventHandler(Of SessionStatus) Implements ISystemMonitor.SessionStateChanged

        Public Function GetLastInputTime() As DateTime Implements ISystemMonitor.GetLastInputTime
            Dim idleSec = Win32Api.GetIdleSeconds()
            Return DateTime.Now.AddSeconds(-idleSec)
        End Function

        Public Function GetSystemUptime() As TimeSpan Implements ISystemMonitor.GetSystemUptime
            Return TimeSpan.FromSeconds(Win32Api.GetSystemUptimeSeconds())
        End Function

        Public Function GetCurrentSessionStatus() As SessionStatus Implements ISystemMonitor.GetCurrentSessionStatus
            Return _currentStatus
        End Function

        Public Sub StartMonitoring() Implements ISystemMonitor.StartMonitoring
            _currentStatus = SessionStatus.Active
            _timer = New Timer(AddressOf CheckStatus, Nothing, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
        End Sub

        Public Sub StopMonitoring() Implements ISystemMonitor.StopMonitoring
            _timer?.Dispose()
            _timer = Nothing
        End Sub

        Private Sub CheckStatus(state As Object)
            Dim idleSec = Win32Api.GetIdleSeconds()

            Dim newStatus As SessionStatus
            If idleSec > 600 Then
                newStatus = SessionStatus.Idle
            Else
                newStatus = SessionStatus.Active
            End If

            If newStatus <> _currentStatus Then
                _currentStatus = newStatus
                RaiseEvent SessionStateChanged(Me, _currentStatus)
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