Imports System
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows.Threading
Imports Microsoft.Extensions.DependencyInjection
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Services
Imports PcUsageTracker.Core.Models

Namespace ViewModels

    Partial Public Class WeeklyViewModel
        Inherits ObservableObject

        Private _refreshTimer As DispatcherTimer
        Private _isRefreshing As Boolean

        Private _weeklyTotal As String = "0h 0m"
        Public Property WeeklyTotal As String
            Get
                Return _weeklyTotal
            End Get
            Set(value As String)
                SetProperty(_weeklyTotal, value)
            End Set
        End Property

        Private _dailyAverage As String = "0h 0m"
        Public Property DailyAverage As String
            Get
                Return _dailyAverage
            End Get
            Set(value As String)
                SetProperty(_dailyAverage, value)
            End Set
        End Property

        Private _dailyItems As ObservableCollection(Of DailyUsageItem)
        Public Property DailyItems As ObservableCollection(Of DailyUsageItem)
            Get
                Return _dailyItems
            End Get
            Set(value As ObservableCollection(Of DailyUsageItem))
                SetProperty(_dailyItems, value)
            End Set
        End Property

        Public ReadOnly Property ExportCommand As IRelayCommand

        Public Sub New()
            _dailyItems = New ObservableCollection(Of DailyUsageItem)()
            _refreshTimer = New DispatcherTimer With {
                .Interval = TimeSpan.FromSeconds(30)
            }
            AddHandler _refreshTimer.Tick, AddressOf OnRefreshTick
            ExportCommand = New RelayCommand(AddressOf ExportToCsv)
        End Sub

        Public Sub StartAutoRefresh()
            LoadDataAsync()
            If Not _refreshTimer.IsEnabled Then
                _refreshTimer.Start()
            End If
        End Sub

        Public Sub StopAutoRefresh()
            _refreshTimer.Stop()
        End Sub

        Private Async Sub OnRefreshTick(sender As Object, e As EventArgs)
            Await LoadDataAsync()
        End Sub

        Public Async Function LoadDataAsync() As Task
            If _isRefreshing Then Return
            _isRefreshing = True

            Try
                Dim provider = Application.ServiceProvider
                If provider Is Nothing Then Return

                Await System.Threading.Tasks.Task.Run(Sub()
                    Using scope = provider.CreateScope()
                        Dim repository = scope.ServiceProvider.GetRequiredService(Of IUsageRepository)()
                        Dim calculator = scope.ServiceProvider.GetRequiredService(Of UsageCalculator)()

                        Dim weekStart = Date.Today.StartOfWeek(DayOfWeek.Monday)
                        Dim weekEnd = weekStart.AddDays(6)

                        Dim summaries = repository.GetDailySummariesAsync(weekStart, weekEnd).GetAwaiter().GetResult()
                        Dim weeklyUsage = calculator.CalculateWeeklySummary(summaries)

                        Dim total = FormatTime(weeklyUsage.TotalActiveTime)
                        Dim avg = FormatTime(weeklyUsage.DailyAverage)

                        Dim items = New List(Of DailyUsageItem)
                        For i = 0 To 6
                            Dim currentDate = weekStart.AddDays(i)
                            Dim daySummary = summaries.FirstOrDefault(Function(s) s.Date = currentDate)

                            items.Add(New DailyUsageItem With {
                                .DayName = currentDate.ToString("ddd"),
                                .DayLabel = currentDate.ToString("MM/dd"),
                                .IsToday = currentDate = Date.Today,
                                .ActiveSeconds = If(daySummary IsNot Nothing, daySummary.TotalActiveSeconds, 0),
                                .ActiveTime = If(daySummary IsNot Nothing, FormatTime(daySummary.TotalActiveTime), "0h 0m")
                            })
                        Next

                        System.Windows.Application.Current.Dispatcher.Invoke(Sub()
                            WeeklyTotal = total
                            DailyAverage = avg
                            DailyItems.Clear()
                            For Each item In items
                                DailyItems.Add(item)
                            Next
                        End Sub)
                    End Using
                End Sub)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Weekly load error: {ex.Message}")
            Finally
                _isRefreshing = False
            End Try
        End Function

        Private Shared Function FormatTime(ts As TimeSpan) As String
            If ts.TotalHours >= 1 Then
                Return $"{CInt(ts.TotalHours)}h {ts.Minutes}m"
            End If
            Return $"{ts.Minutes}m {ts.Seconds}s"
        End Function

        Private Sub ExportToCsv()
            Try
                Dim dialog = New Microsoft.Win32.SaveFileDialog With {
                    .Filter = "CSV 文件 (*.csv)|*.csv",
                    .DefaultExt = ".csv",
                    .FileName = $"PcUsageTracker_Weekly_{Date.Today:yyyyMMdd}.csv"
                }

                If dialog.ShowDialog() = True Then
                    Dim sb = New StringBuilder()
                    sb.AppendLine("日期,星期,活跃时间,活跃秒数")

                    For Each item In DailyItems
                        sb.AppendLine($"{item.DayLabel},{item.DayName},{item.ActiveTime},{item.ActiveSeconds}")
                    Next

                    sb.AppendLine()
                    sb.AppendLine($"本周总计,,{WeeklyTotal},")
                    sb.AppendLine($"日均使用,,{DailyAverage},")

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8)
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Export error: {ex.Message}")
            End Try
        End Sub
    End Class

    Public Class DailyUsageItem
        Inherits ObservableObject

        Private _dayName As String
        Public Property DayName As String
            Get
                Return _dayName
            End Get
            Set(value As String)
                SetProperty(_dayName, value)
            End Set
        End Property

        Private _dayLabel As String
        Public Property DayLabel As String
            Get
                Return _dayLabel
            End Get
            Set(value As String)
                SetProperty(_dayLabel, value)
            End Set
        End Property

        Private _isToday As Boolean
        Public Property IsToday As Boolean
            Get
                Return _isToday
            End Get
            Set(value As Boolean)
                SetProperty(_isToday, value)
            End Set
        End Property

        Private _activeSeconds As Integer
        Public Property ActiveSeconds As Integer
            Get
                Return _activeSeconds
            End Get
            Set(value As Integer)
                SetProperty(_activeSeconds, value)
                OnPropertyChanged(NameOf(BarHeight))
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

        Public ReadOnly Property BarHeight As Double
            Get
                Dim maxSeconds = 28800
                Return Math.Max(4, Math.Min(ActiveSeconds, maxSeconds) / maxSeconds * 150)
            End Get
        End Property
    End Class

End Namespace