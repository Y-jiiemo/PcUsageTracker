Imports System
Imports System.Runtime.InteropServices

Namespace System

    Public Module Win32Api

        <StructLayout(LayoutKind.Sequential)>
        Public Structure LastInputInfo
            Public cbSize As UInteger
            Public dwTime As UInteger
        End Structure

        <DllImport("user32.dll")>
        Public Function GetLastInputInfo(ByRef plii As LastInputInfo) As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Public Function GetTickCount64() As ULong
        End Function

        Public Function GetIdleSeconds() As Integer
            Dim lastInput = New LastInputInfo With {.cbSize = CUInt(Marshal.SizeOf(GetType(LastInputInfo)))}
            If GetLastInputInfo(lastInput) Then
                Return CInt((GetTickCount64() - lastInput.dwTime) \ 1000)
            End If
            Return 0
        End Function

        Public Function GetSystemUptimeSeconds() As ULong
            Return GetTickCount64() \ 1000
        End Function
    End Module

End Namespace