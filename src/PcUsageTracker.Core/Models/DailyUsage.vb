Imports System

Namespace Models

    Public Class DailyUsage
        Public Property Id As Integer
        Public Property [Date] As Date
        Public Property TotalActiveSeconds As Integer
        Public Property TotalIdleSeconds As Integer
        Public Property SessionCount As Integer
        Public Property FirstActiveTime As DateTime?
        Public Property LastActiveTime As DateTime?

        Public ReadOnly Property TotalActiveTime As TimeSpan
            Get
                Return TimeSpan.FromSeconds(TotalActiveSeconds)
            End Get
        End Property

        Public ReadOnly Property TotalIdleTime As TimeSpan
            Get
                Return TimeSpan.FromSeconds(TotalIdleSeconds)
            End Get
        End Property

        Public ReadOnly Property Efficiency As Double
            Get
                Dim total = TotalActiveSeconds + TotalIdleSeconds
                If total = 0 Then Return 0
                Return Math.Round(TotalActiveSeconds / total * 100, 1)
            End Get
        End Property
    End Class

End Namespace