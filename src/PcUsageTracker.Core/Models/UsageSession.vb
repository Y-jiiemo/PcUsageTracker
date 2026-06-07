Imports System
Imports PcUsageTracker.Core.Enums

Namespace Models

    Public Class UsageSession
        Public Property Id As Integer
        Public Property StartTime As DateTime
        Public Property EndTime As DateTime?
        Public Property ActiveSeconds As Integer
        Public Property IdleSeconds As Integer
        Public Property Status As SessionStatus

        Public ReadOnly Property Duration As TimeSpan
            Get
                If EndTime.HasValue Then
                    Return EndTime.Value - StartTime
                End If
                Return DateTime.Now - StartTime
            End Get
        End Property

        Public ReadOnly Property EffectiveUsage As TimeSpan
            Get
                Return TimeSpan.FromSeconds(ActiveSeconds)
            End Get
        End Property
    End Class

End Namespace