Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Microsoft.Extensions.DependencyInjection

Namespace ViewModels

    Partial Public Class MainViewModel
        Inherits ObservableObject

        Private _currentView As ObservableObject
        Public Property CurrentView As ObservableObject
            Get
                Return _currentView
            End Get
            Set(value As ObservableObject)
                If SetProperty(_currentView, value) Then
                    If TypeOf value Is DashboardViewModel Then
                        DirectCast(value, DashboardViewModel).StartAutoRefresh()
                    ElseIf TypeOf value Is WeeklyViewModel Then
                        DirectCast(value, WeeklyViewModel).StartAutoRefresh()
                    ElseIf TypeOf value Is HistoryViewModel Then
                        DirectCast(value, HistoryViewModel).LoadDataAsync()
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property NavigateToDashboardCommand As IRelayCommand
        Public ReadOnly Property NavigateToWeeklyCommand As IRelayCommand
        Public ReadOnly Property NavigateToHistoryCommand As IRelayCommand
        Public ReadOnly Property NavigateToSettingsCommand As IRelayCommand

        Public Sub New()
            NavigateToDashboardCommand = New RelayCommand(AddressOf NavigateToDashboard)
            NavigateToWeeklyCommand = New RelayCommand(AddressOf NavigateToWeekly)
            NavigateToHistoryCommand = New RelayCommand(AddressOf NavigateToHistory)
            NavigateToSettingsCommand = New RelayCommand(AddressOf NavigateToSettings)

            _currentView = New DashboardViewModel()
            DirectCast(_currentView, DashboardViewModel).StartAutoRefresh()
        End Sub

        Private Sub NavigateToDashboard()
            CurrentView = New DashboardViewModel()
        End Sub

        Private Sub NavigateToWeekly()
            CurrentView = New WeeklyViewModel()
        End Sub

        Private Sub NavigateToHistory()
            CurrentView = New HistoryViewModel()
        End Sub

        Private Sub NavigateToSettings()
            CurrentView = New SettingsViewModel()
        End Sub
    End Class

End Namespace