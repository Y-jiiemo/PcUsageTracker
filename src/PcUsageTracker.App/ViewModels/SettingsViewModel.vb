Imports System
Imports System.IO
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Microsoft.Win32

Namespace ViewModels

    Partial Public Class SettingsViewModel
        Inherits ObservableObject

        Private _idleThresholdMinutes As Integer
        Public Property IdleThresholdMinutes As Integer
            Get
                Return _idleThresholdMinutes
            End Get
            Set(value As Integer)
                If SetProperty(_idleThresholdMinutes, value) Then
                    SaveSettings()
                End If
            End Set
        End Property

        Private _autoStartEnabled As Boolean
        Public Property AutoStartEnabled As Boolean
            Get
                Return _autoStartEnabled
            End Get
            Set(value As Boolean)
                If SetProperty(_autoStartEnabled, value) Then
                    SetAutoStart(value)
                End If
            End Set
        End Property

        Private _statusMessage As String
        Public Property StatusMessage As String
            Get
                Return _statusMessage
            End Get
            Set(value As String)
                SetProperty(_statusMessage, value)
            End Set
        End Property

        Public Sub New()
            LoadSettings()
        End Sub

        Private Sub LoadSettings()
            Dim configPath = GetConfigPath()
            _idleThresholdMinutes = 5
            _autoStartEnabled = IsAutoStartEnabled()

            If File.Exists(configPath) Then
                Try
                    Dim lines = File.ReadAllLines(configPath)
                    For Each line In lines
                        If line.StartsWith("IdleThresholdMinutes=") Then
                            Integer.TryParse(line.Split("="c)(1), _idleThresholdMinutes)
                        End If
                    Next
                Catch
                End Try
            End If
        End Sub

        Private Sub SaveSettings()
            Try
                Dim configPath = GetConfigPath()
                Dim dir = Path.GetDirectoryName(configPath)
                If Not Directory.Exists(dir) Then
                    Directory.CreateDirectory(dir)
                End If
                File.WriteAllLines(configPath, {
                    $"IdleThresholdMinutes={IdleThresholdMinutes}"
                })
                StatusMessage = "设置已保存"
            Catch ex As Exception
                StatusMessage = $"保存失败: {ex.Message}"
            End Try
        End Sub

        Private Shared Function GetConfigPath() As String
            Dim appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Return Path.Combine(appData, "PcUsageTracker", "config.txt")
        End Function

        Private Function IsAutoStartEnabled() As Boolean
            Try
                Using key = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", False)
                    If key IsNot Nothing Then
                        Dim value = key.GetValue("PcUsageTracker")
                        Return value IsNot Nothing
                    End If
                End Using
            Catch
            End Try
            Return False
        End Function

        Private Sub SetAutoStart(enabled As Boolean)
            Try
                Using key = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
                    If key Is Nothing Then Return

                    If enabled Then
                        Dim exePath = Environment.ProcessPath
                        If exePath IsNot Nothing Then
                            key.SetValue("PcUsageTracker", exePath)
                            StatusMessage = "已设置开机自启"
                        End If
                    Else
                        key.DeleteValue("PcUsageTracker", False)
                        StatusMessage = "已取消开机自启"
                    End If
                End Using
            Catch ex As Exception
                StatusMessage = $"设置失败: {ex.Message}"
                _autoStartEnabled = Not enabled
                OnPropertyChanged(NameOf(AutoStartEnabled))
            End Try
        End Sub
    End Class

End Namespace