Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports PcUsageTracker.Core.Models
Imports PcUsageTracker.Core.Interfaces

Namespace Services

    Public Class UsageCalculator
        Implements IUsageCalculator

        Public Function CalculateDailySummary(sessions As List(Of UsageSession), targetDate As Date) As DailyUsage Implements IUsageCalculator.CalculateDailySummary
            Dim summary = New DailyUsage With {
                .[Date] = targetDate,
                .SessionCount = sessions.Count
            }

            If sessions.Any() Then
                summary.TotalActiveSeconds = CInt(sessions.Sum(Function(s) s.ActiveSeconds))
                summary.TotalIdleSeconds = CInt(sessions.Sum(Function(s) s.IdleSeconds))
                summary.FirstActiveTime = sessions.Min(Function(s) s.StartTime)
                summary.LastActiveTime = sessions.Max(Function(s) If(s.EndTime, DateTime.Now))
            End If

            Return summary
        End Function

        Public Function CalculateWeeklySummary(dailyItems As List(Of DailyUsage)) As WeeklyUsage Implements IUsageCalculator.CalculateWeeklySummary
            If Not dailyItems.Any() Then
                Return New WeeklyUsage With {
                    .WeekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday),
                    .WeekEnd = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(6),
                    .TotalActiveSeconds = 0,
                    .DailyItems = dailyItems
                }
            End If

            Return New WeeklyUsage With {
                .WeekStart = dailyItems.Min(Function(d) d.[Date]).StartOfWeek(DayOfWeek.Monday),
                .WeekEnd = dailyItems.Max(Function(d) d.[Date]).StartOfWeek(DayOfWeek.Monday).AddDays(6),
                .TotalActiveSeconds = CInt(dailyItems.Sum(Function(d) d.TotalActiveSeconds)),
                .DailyItems = dailyItems
            }
        End Function
    End Class

    Public Module DateExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Function StartOfWeek(dt As Date, weekStartDay As DayOfWeek) As Date
            Dim diff = (7 + (dt.DayOfWeek - weekStartDay)) Mod 7
            Return dt.AddDays(-1 * diff).Date
        End Function
    End Module

End Namespace