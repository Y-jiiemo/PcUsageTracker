Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Models
Imports PcUsageTracker.Core.Enums

Namespace Data

    Public Class UsageRepository
        Implements IUsageRepository

        Private ReadOnly _context As AppDbContext

        Public Sub New(context As AppDbContext)
            _context = context
        End Sub

        Public Async Function GetTodaySessionsAsync() As Task(Of List(Of UsageSession)) Implements IUsageRepository.GetTodaySessionsAsync
            Dim today = Date.Today
            Return Await _context.UsageSessions _
                .Where(Function(s) s.StartTime.Date = today) _
                .OrderByDescending(Function(s) s.StartTime) _
                .ToListAsync()
        End Function

        Public Async Function GetSessionsByDateAsync(targetDate As Date) As Task(Of List(Of UsageSession)) Implements IUsageRepository.GetSessionsByDateAsync
            Return Await _context.UsageSessions _
                .Where(Function(s) s.StartTime.Date = targetDate) _
                .OrderBy(Function(s) s.StartTime) _
                .ToListAsync()
        End Function

        Public Async Function GetDailySummariesAsync(startDate As Date, endDate As Date) As Task(Of List(Of DailyUsage)) Implements IUsageRepository.GetDailySummariesAsync
            Return Await _context.DailySummaries _
                .Where(Function(d) d.Date >= startDate AndAlso d.Date <= endDate) _
                .OrderBy(Function(d) d.Date) _
                .ToListAsync()
        End Function

        Public Async Function GetCurrentSessionAsync() As Task(Of UsageSession) Implements IUsageRepository.GetCurrentSessionAsync
            Return Await _context.UsageSessions _
                .Where(Function(s) Not s.EndTime.HasValue) _
                .OrderByDescending(Function(s) s.StartTime) _
                .FirstOrDefaultAsync()
        End Function

        Public Async Function SaveSessionAsync(session As UsageSession) As Task Implements IUsageRepository.SaveSessionAsync
            _context.UsageSessions.Add(session)
            Await _context.SaveChangesAsync()
        End Function

        Public Async Function UpdateSessionAsync(session As UsageSession) As Task Implements IUsageRepository.UpdateSessionAsync
            _context.UsageSessions.Update(session)
            Await _context.SaveChangesAsync()
        End Function

        Public Async Function GetOrCreateDailySummaryAsync(targetDate As Date) As Task(Of DailyUsage) Implements IUsageRepository.GetOrCreateDailySummaryAsync
            Dim summary = Await _context.DailySummaries _
                .FirstOrDefaultAsync(Function(d) d.Date = targetDate)

            If summary Is Nothing Then
                summary = New DailyUsage With {.Date = targetDate}
                _context.DailySummaries.Add(summary)
                Await _context.SaveChangesAsync()
            End If

            Return summary
        End Function

        Public Async Function UpdateDailySummaryAsync(summary As DailyUsage) As Task Implements IUsageRepository.UpdateDailySummaryAsync
            _context.DailySummaries.Update(summary)
            Await _context.SaveChangesAsync()
        End Function
    End Class

End Namespace