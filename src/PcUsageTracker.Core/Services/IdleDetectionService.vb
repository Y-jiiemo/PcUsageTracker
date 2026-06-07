Imports System
Imports PcUsageTracker.Core.Interfaces

Namespace Services

    Public Class IdleDetectionService
        Private ReadOnly _idleThreshold As Integer
        Private ReadOnly _systemMonitor As ISystemMonitor

        Public Sub New(systemMonitor As ISystemMonitor, Optional idleThresholdSeconds As Integer = 300)
            _systemMonitor = systemMonitor
            _idleThreshold = idleThresholdSeconds
        End Sub

        Public Function IsUserIdle() As Boolean
            Dim lastInput = _systemMonitor.GetLastInputTime()
            Return (DateTime.Now - lastInput).TotalSeconds > _idleThreshold
        End Function

        Public Function GetIdleDuration() As TimeSpan
            Dim lastInput = _systemMonitor.GetLastInputTime()
            Dim idleTime = DateTime.Now - lastInput
            If idleTime.TotalSeconds > _idleThreshold Then
                Return idleTime
            End If
            Return TimeSpan.Zero
        End Function
    End Class

End Namespace