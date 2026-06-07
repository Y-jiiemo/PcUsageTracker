Imports System.Collections.Generic
Imports PcUsageTracker.Core.Models

Namespace Interfaces

    Public Interface IUsageCalculator
        Function CalculateDailySummary(sessions As List(Of UsageSession), targetDate As Date) As DailyUsage
        Function CalculateWeeklySummary(dailyItems As List(Of DailyUsage)) As WeeklyUsage
    End Interface

End Namespace