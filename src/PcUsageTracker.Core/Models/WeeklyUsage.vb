Imports System
Imports System.Collections.Generic

Namespace Models

    Public Class WeeklyUsage
        Public Property WeekStart As Date
        Public Property WeekEnd As Date
        Public Property TotalActiveSeconds As Integer
        Public Property DailyItems As List(Of DailyUsage)

        Public ReadOnly Property TotalActiveTime As TimeSpan
            Get
                Return TimeSpan.FromSeconds(TotalActiveSeconds)
            End Get
        End Property

        Public ReadOnly Property DailyAverage As TimeSpan
            Get
                If DailyItems Is Nothing OrElse DailyItems.Count = 0 Then
                    Return TimeSpan.Zero
                End If
                Return TimeSpan.FromSeconds(TotalActiveSeconds / DailyItems.Count)
            End Get
        End Property
    End Class

End Namespace