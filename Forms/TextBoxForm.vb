' Furry Art Studio - 本地稿件管理工具
' Copyright 2026 xionglongztz
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
Imports System.Runtime.InteropServices

Public Class TextBoxForm
    Public Sub New(text As String, title As String)
        InitializeComponent()
        TxtBox.Text = text
        Me.Text = title
    End Sub
    Private Sub TextBoxForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TxtBox.ReadOnly = True
        TxtBox.Dock = DockStyle.Fill
        SystemThemeChange()
    End Sub
    Private Sub SystemThemeChange()
        Using regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", True)
            Dim isDarkMode As Boolean = (regKey.GetValue("AppsUseLightTheme", "1") = 0) '判断是否为深色主题
            '颜色常量
            Dim bgColor As Color
            Dim frColor As Color
            '获取控件集合
            Dim controlList As List(Of Control) = GetAllControls(Me)
            '判断颜色
            If isDarkMode Then
                bgColor = BgColorDark
                frColor = FrColorDark
                Icon = CreateRoundedRectangleIcon(True, My.Resources.Icons.FormFileDark)
            Else
                bgColor = BgColorLight
                frColor = FrColorLight
                Icon = CreateRoundedRectangleIcon(True, My.Resources.Icons.FormFileLight)
            End If
            TxtBox.ForeColor = frColor
            TxtBox.BackColor = bgColor
            'WinAPI
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.UseImmersiveDarkMode, isDarkMode, Marshal.SizeOf(Of Integer))
            SetPreferredAppMode(PreferredAppMode.AllowDark)
            FlushMenuThemes()
        End Using
    End Sub
End Class