Imports System
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows.Threading
Imports Microsoft.Extensions.DependencyInjection
Imports CommunityToolkit.Mvvm.ComponentModel
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Services
Imports PcUsageTracker.Core.Models

Namespace ViewModels

    Partial Public Class HistoryViewModel
        Inherits ObservableObject

        Private _historyItems As ObservableCollection(Of HistoryItem)
        Public Property HistoryItems As ObservableCollection(Of HistoryItem)
            Get
                Return _historyItems
            End Get
            Set(value As ObservableCollection(Of HistoryItem))
                SetProperty(_historyItems, value)
            End Set
        End Property

        Private _selectedDate As Date
        Public Property SelectedDate As Date
            Get
                Return _selectedDate
            End Get
            Set(value As Date)
                If SetProperty(_selectedDate, value) Then
                    LoadDetailAsync()
                End If
            End Set
        End Property

        Private _detailItem As HistoryItem
        Public Property DetailItem As HistoryItem
            Get
                Return _detailItem
            End Get
            Set(value As HistoryItem)
                SetProperty(_detailItem, value)
            End Set
        End Property

        Private _isLoading As Boolean
        Public Property IsLoading As Boolean
            Get
                Return _isLoading
            End Get
            Set(value As Boolean)
                SetProperty(_isLoading, value)
            End Set
        End Property

        Public Sub New()
            _historyItems = New ObservableCollection(Of HistoryItem)()
            _selectedDate = Date.Today
        End Sub

        Public Async Sub LoadDataAsync()
            If IsLoading Then Return
            IsLoading = True

            Try
                Dim provider = Application.ServiceProvider
                If provider Is Nothing Then Return

                Await System.Threading.Tasks.Task.Run(Sub()
                    Using scope = provider.CreateScope()
                        Dim repository = scope.ServiceProvider.GetRequiredService(Of IUsageRepository)()
                        Dim calculator = scope.ServiceProvider.GetRequiredService(Of UsageCalculator)()

                        Dim startDate = Date.Today.AddDays(-30)
                        Dim summaries = repository.GetDailySummariesAsync(startDate, Date.Today).GetAwaiter().GetResult()

                        Dim items = summaries.Select(Function(s)
                            Return New HistoryItem With {
                                .Date = s.Date,
                                .DateDisplay = s.Date.ToString("yyyy-MM-dd"),
                                .DayOfWeek = s.Date.ToString("ddd"),
                                .ActiveTime = FormatTime(s.TotalActiveTime),
                                .IdleTime = FormatTime(s.TotalIdleTime),
                                .SessionCount = s.SessionCount,
                                .Efficiency = $"{s.Efficiency}%",
                                .TotalActiveSeconds = s.TotalActiveSeconds
                            }
                        End Function).OrderByDescending(Function(i) i.Date).ToList()

                        System.Windows.Application.Current.Dispatcher.Invoke(Sub()
                            HistoryItems.Clear()
                            For Each item In items
                                HistoryItems.Add(item)
                            Next
                            If items.Count > 0 Then
                                SelectedDate = items(0).Date
                            End If
                        End Sub)
                    End Using
                End Sub)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"History load error: {ex.Message}")
            Finally
                IsLoading = False
            End Try
        End Sub

        Private Async Sub LoadDetailAsync()
            Try
                Dim provider = Application.ServiceProvider
                If provider Is Nothing Then Return

                Await System.Threading.Tasks.Task.Run(Sub()
                    Using scope = provider.CreateScope()
                        Dim repository = scope.ServiceProvider.GetRequiredService(Of IUsageRepository)()
                        Dim calculator = scope.ServiceProvider.GetRequiredService(Of UsageCalculator)()

                        Dim sessions = repository.GetSessionsByDateAsync(SelectedDate).GetAwaiter().GetResult()
                        Dim summary = calculator.CalculateDailySummary(sessions, SelectedDate)

                        Dim item = New HistoryItem With {
                            .Date = SelectedDate,
                            .DateDisplay = SelectedDate.ToString("yyyy-MM-dd"),
                            .DayOfWeek = SelectedDate.ToString("ddd"),
                            .ActiveTime = FormatTime(summary.TotalActiveTime),
                            .IdleTime = FormatTime(summary.TotalIdleTime),
                            .SessionCount = summary.SessionCount,
                            .Efficiency = $"{summary.Efficiency}%",
                            .TotalActiveSeconds = summary.TotalActiveSeconds
                        }

                        System.Windows.Application.Current.Dispatcher.Invoke(Sub()
                            DetailItem = item
                        End Sub)
                    End Using
                End Sub)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Detail load error: {ex.Message}")
            End Try
        End Sub

        Private Shared Function FormatTime(ts As TimeSpan) As String
            If ts.TotalHours >= 1 Then
                Return $"{CInt(ts.TotalHours)}h {ts.Minutes}m"
            End If
            Return $"{ts.Minutes}m {ts.Seconds}s"
        End Function
    End Class

    Public Class HistoryItem
        Inherits ObservableObject

        Private _date As Date
        Public Property [Date] As Date
            Get
                Return _date
            End Get
            Set(value As Date)
                SetProperty(_date, value)
            End Set
        End Property

        Private _dateDisplay As String
        Public Property DateDisplay As String
            Get
                Return _dateDisplay
            End Get
            Set(value As String)
                SetProperty(_dateDisplay, value)
            End Set
        End Property

        Private _dayOfWeek As String
        Public Property DayOfWeek As String
            Get
                Return _dayOfWeek
            End Get
            Set(value As String)
                SetProperty(_dayOfWeek, value)
            End Set
        End Property

        Private _activeTime As String
        Public Property ActiveTime As String
            Get
                Return _activeTime
            End Get
            Set(value As String)
                SetProperty(_activeTime, value)
            End Set
        End Property

        Private _idleTime As String
        Public Property IdleTime As String
            Get
                Return _idleTime
            End Get
            Set(value As String)
                SetProperty(_idleTime, value)
            End Set
        End Property

        Private _sessionCount As Integer
        Public Property SessionCount As Integer
            Get
                Return _sessionCount
            End Get
            Set(value As Integer)
                SetProperty(_sessionCount, value)
            End Set
        End Property

        Private _efficiency As String
        Public Property Efficiency As String
            Get
                Return _efficiency
            End Get
            Set(value As String)
                SetProperty(_efficiency, value)
            End Set
        End Property

        Private _totalActiveSeconds As Integer
        Public Property TotalActiveSeconds As Integer
            Get
                Return _totalActiveSeconds
            End Get
            Set(value As Integer)
                SetProperty(_totalActiveSeconds, value)
            End Set
        End Property
    End Class

End Namespace