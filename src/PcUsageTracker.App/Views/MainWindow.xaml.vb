Imports PcUsageTracker.App.ViewModels
Imports Microsoft.Extensions.DependencyInjection

Class MainWindow

    Public Sub New()
        InitializeComponent()

        Dim vm = Application.ServiceProvider.GetRequiredService(Of MainViewModel)()
        DataContext = vm
    End Sub

    Private Sub TitleBar_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        If e.ClickCount = 2 Then
            ToggleMaximize()
        Else
            DragMove()
        End If
    End Sub

    Private Sub Minimize_Click(sender As Object, e As RoutedEventArgs)
        WindowState = WindowState.Minimized
    End Sub

    Private Sub Maximize_Click(sender As Object, e As RoutedEventArgs)
        ToggleMaximize()
    End Sub

    Private Sub Close_Click(sender As Object, e As RoutedEventArgs)
        Hide()
    End Sub

    Private Sub ToggleMaximize()
        If WindowState = WindowState.Maximized Then
            WindowState = WindowState.Normal
        Else
            WindowState = WindowState.Maximized
        End If
    End Sub

    Protected Overrides Sub OnStateChanged(e As EventArgs)
        MyBase.OnStateChanged(e)
        If WindowState = WindowState.Maximized Then
            MaximizeButton.Content = "🗗"
        Else
            MaximizeButton.Content = "□"
        End If
    End Sub

    Protected Overrides Sub OnClosing(e As ComponentModel.CancelEventArgs)
        MyBase.OnClosing(e)
        e.Cancel = True
        Hide()
    End Sub
End Class