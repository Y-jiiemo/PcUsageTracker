Imports Microsoft.Extensions.DependencyInjection
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Services
Imports PcUsageTracker.Infrastructure.DI
Imports PcUsageTracker.App.ViewModels
Imports PcUsageTracker.Infrastructure.Data
Imports WinForms = System.Windows.Forms

Class Application

    Public Shared ServiceProvider As IServiceProvider
    Private Shared _notifyIcon As WinForms.NotifyIcon
    Private Shared _trackingService As UsageTrackingService

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)

        Dim dbPath = IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PcUsageTracker", "usage.db")

        Dim dbDir = IO.Path.GetDirectoryName(dbPath)
        If Not IO.Directory.Exists(dbDir) Then
            IO.Directory.CreateDirectory(dbDir)
        End If

        Dim services As IServiceCollection = New ServiceCollection()

        services.AddInfrastructure(dbPath)
        services.AddSingleton(Of UsageTrackingService)()

        services.AddSingleton(Of MainViewModel)()
        services.AddTransient(Of DashboardViewModel)()
        services.AddTransient(Of WeeklyViewModel)()

        ServiceProvider = services.BuildServiceProvider()

        Using scope = ServiceProvider.CreateScope()
            Dim context = scope.ServiceProvider.GetRequiredService(Of AppDbContext)()
            context.Database.EnsureCreated()
        End Using

        _trackingService = ServiceProvider.GetRequiredService(Of UsageTrackingService)()
        _trackingService.StartTrackingAsync()

        CreateTrayIcon()
    End Sub

    Private Sub CreateTrayIcon()
        _notifyIcon = New WinForms.NotifyIcon With {
            .Text = "PC Usage Tracker",
            .Visible = True
        }

        Using bmp = New System.Drawing.Bitmap(32, 32)
            Using g = System.Drawing.Graphics.FromImage(bmp)
                g.Clear(System.Drawing.Color.FromArgb(124, 77, 255))
                Using font = New System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold)
                    g.DrawString("T", font, System.Drawing.Brushes.White, 6, 2)
                End Using
            End Using
            Dim iconHandle = bmp.GetHicon()
            _notifyIcon.Icon = System.Drawing.Icon.FromHandle(iconHandle)
        End Using

        Dim contextMenu = New WinForms.ContextMenuStrip()
        contextMenu.Items.Add("显示主窗口", Nothing, Sub(s, ev) ShowMainWindow())
        contextMenu.Items.Add(New WinForms.ToolStripSeparator())
        contextMenu.Items.Add("退出", Nothing, Sub(s, ev) ExitApplication())

        _notifyIcon.ContextMenuStrip = contextMenu

        AddHandler _notifyIcon.DoubleClick, Sub(s, ev) ShowMainWindow()
    End Sub

    Private Sub ShowMainWindow()
        Dim window = Me.MainWindow
        If window Is Nothing Then
            window = New MainWindow()
            Me.MainWindow = window
        End If
        window.Show()
        window.WindowState = WindowState.Normal
        window.Activate()
    End Sub

    Private Sub ExitApplication()
        _notifyIcon?.Dispose()
        _trackingService?.StopTrackingAsync()
        _trackingService?.Dispose()
        Me.Shutdown()
    End Sub

End Class