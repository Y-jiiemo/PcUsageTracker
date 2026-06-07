Imports System
Imports PcUsageTracker.Core.Enums

Namespace Interfaces

    Public Interface ISystemMonitor
        Event SessionStateChanged As EventHandler(Of SessionStatus)
        Function GetLastInputTime() As DateTime
        Function GetSystemUptime() As TimeSpan
        Function GetCurrentSessionStatus() As SessionStatus
        Sub StartMonitoring()
        Sub StopMonitoring()
    End Interface

End Namespace