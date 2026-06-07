Imports System
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports Microsoft.Extensions.DependencyInjection
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Services
Imports PcUsageTracker.Infrastructure.Data

Namespace ViewModels

    Partial Public Class DashboardViewModel
        Inherits ObservableObject

        Private _refreshTimer As DispatcherTimer
        Private _isRefreshing As Boolean

        Private _todayActiveTime As String = "0h 0m"
        Public Property TodayActiveTime As String
            Get
                Return _todayActiveTime
            End Get
            Set(value As String)
                SetProperty(_todayActiveTime, value)
            End Set
        End Property

        Private _todayIdleTime As String = "0h 0m"
        Public Property TodayIdleTime As String
            Get
                Return _todayIdleTime
            End Get
            Set(value As String)
                SetProperty(_todayIdleTime, value)
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

        Private _efficiency As String = "0%"
        Public Property Efficiency As String
            Get
                Return _efficiency
            End Get
            Set(value As String)
                SetProperty(_efficiency, value)
            End Set
        End Property

        Private _systemUptime As String = "0h 0m"
        Public Property SystemUptime As String
            Get
                Return _systemUptime
            End Get
            Set(value As String)
                SetProperty(_systemUptime, value)
            End Set
        End Property

        Public Sub New()
            _refreshTimer = New DispatcherTimer With {
                .Interval = TimeSpan.FromSeconds(10)
            }
            AddHandler _refreshTimer.Tick, AddressOf OnRefreshTick
        End Sub

        Public Sub StartAutoRefresh()
#Disable Warning BC42358
            LoadDataAsync()
#Enable Warning BC42358
            If Not _refreshTimer.IsEnabled Then
                _refreshTimer.Start()
            End If
        End Sub

        Public Sub StopAutoRefresh()
            _refreshTimer.Stop()
        End Sub

        Private Sub OnRefreshTick(sender As Object, e As EventArgs)
#Disable Warning BC42358
            LoadDataAsync()
#Enable Warning BC42358
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
                        Dim monitor = scope.ServiceProvider.GetRequiredService(Of ISystemMonitor)()

                        Dim sessions = repository.GetTodaySessionsAsync().GetAwaiter().GetResult()
                        Dim summary = calculator.CalculateDailySummary(sessions, Date.Today)

                        Dim activeTime = FormatTime(summary.TotalActiveTime)
                        Dim idleTime = FormatTime(summary.TotalIdleTime)
                        Dim count = summary.SessionCount
                        Dim eff = FormatEfficiency(summary)
                        Dim uptime = FormatTime(monitor.GetSystemUptime())

                        System.Windows.Application.Current.Dispatcher.Invoke(Sub()
                            TodayActiveTime = activeTime
                            TodayIdleTime = idleTime
                            SessionCount = count
                            Efficiency = eff
                            SystemUptime = uptime
                        End Sub)
                    End Using
                End Sub)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}")
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

        Private Shared Function FormatEfficiency(summary As Core.Models.DailyUsage) As String
            Return $"{summary.Efficiency}%"
        End Function
    End Class

End Namespace