Imports System
Imports System.Globalization
Imports System.Windows.Data

Namespace Converters

    Public Class SecondsToTimeConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value Is Nothing Then Return "0h 0m"

            Dim totalSeconds As Integer
            If Integer.TryParse(value.ToString(), totalSeconds) Then
                Dim ts = TimeSpan.FromSeconds(totalSeconds)
                Return $"{CInt(ts.TotalHours)}h {ts.Minutes}m"
            End If

            Return "0h 0m"
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class BoolToVisibilityConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim boolValue = If(value IsNot Nothing, CBool(value), False)
            Return If(boolValue, Visibility.Visible, Visibility.Collapsed)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class BoolToAccentConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim boolValue = If(value IsNot Nothing, CBool(value), False)
            Return If(boolValue, "#7C4DFF", "#2D2D44")
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace