Imports System
Imports System.Collections.Generic
Imports PcUsageTracker.Core.Models

Namespace Interfaces

    Public Interface IUsageRepository
        Function GetTodaySessionsAsync() As Task(Of List(Of UsageSession))
        Function GetSessionsByDateAsync(targetDate As Date) As Task(Of List(Of UsageSession))
        Function GetDailySummariesAsync(startDate As Date, endDate As Date) As Task(Of List(Of DailyUsage))
        Function GetCurrentSessionAsync() As Task(Of UsageSession)
        Function SaveSessionAsync(session As UsageSession) As Task
        Function UpdateSessionAsync(session As UsageSession) As Task
        Function GetOrCreateDailySummaryAsync(targetDate As Date) As Task(Of DailyUsage)
        Function UpdateDailySummaryAsync(summary As DailyUsage) As Task
    End Interface

End Namespace