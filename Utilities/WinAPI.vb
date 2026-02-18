Imports System.Runtime.InteropServices

Public Module WinAPI

#Region "窗口与菜单"
    'GetSystemMenu 函数 - 获得系统菜单
    <DllImport("user32.dll")>
    Public Function GetSystemMenu(
        ByVal hwnd As IntPtr,
        ByVal bRevert As Boolean
    ) As IntPtr
    End Function
    'AppendMenu 函数 - 添加菜单项
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Function AppendMenu(
        ByVal hMenu As IntPtr,
        ByVal wFlags As Integer,
        ByVal wIDNewItem As Integer,
        ByVal lpNewItem As String
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    'RemoveMenu 函数 - 删除菜单项
    <DllImport("user32.dll")>
    Public Function RemoveMenu(
        ByVal hMenu As IntPtr,
        ByVal uPosition As Integer,
        ByVal uFlags As Integer
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    'CheckMenuItem 函数 - 选中/清除选中菜单项
    <DllImport("user32.dll")>
    Public Function CheckMenuItem(
        ByVal hMenu As IntPtr,
        ByVal uIDCheckItem As Integer,
        ByVal uCheck As Integer
    ) As Integer
    End Function
    'SetMenuItemBitmaps 函数 - 设置菜单位图
    <DllImport("user32.dll")>
    Public Function SetMenuItemBitmaps(
        ByVal hMenu As IntPtr,
        ByVal uPosition As Integer,
        ByVal uFlags As Integer,
        ByVal hBitmapUnchecked As IntPtr,
        ByVal hBitmapChecked As IntPtr
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    'EnableMenuItem 函数 - 使菜单在有效与无效之间切换
    <DllImport("user32.dll")>
    Public Function EnableMenuItem(
        ByVal hMenu As IntPtr,
        ByVal uIDEnableItem As Integer,
        ByVal uEnable As Integer
    ) As Integer
    End Function
    'GetMenuItemCount 函数 - 获取菜单项数量
    <DllImport("user32.dll")>
    Public Function GetMenuItemCount(
        ByVal hMenu As IntPtr
    ) As Integer
    End Function
    'InsertMenu 函数 - 插入菜单项
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Function InsertMenu(
        ByVal hMenu As IntPtr,
        ByVal nPosition As Integer,
        ByVal wFlags As Integer,
        ByVal wIDNewItem As Integer,
        <MarshalAs(UnmanagedType.LPTStr)> ByVal lpNewItem As String
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    'GetMenuItemID 函数 - 获取菜单项ID
    <DllImport("user32.dll")>
    Public Function GetMenuItemID(
        ByVal hMenu As IntPtr,
        ByVal nPos As Integer
    ) As Integer
    End Function
    'ChangeWindowMessageFilter 函数 - 修改指定窗口(UIPI)消息筛选器的用户界面特权隔离, 解除管理员模式下无法拖拽的问题
    <DllImport("user32.dll")>
    Public Function ChangeWindowMessageFilter(
        ByVal message As Integer,
        ByVal dwFlag As Integer
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    'SendMessage 函数 - 发送特定消息
    <DllImport("user32.dll")>
    Public Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function
    '消息常量
    Public Const WM_DROPFILES As Integer = &H233
    Public Const WM_COPYGLOBALDATA As Integer = &H49
    Public Const WM_COPYDATA As Integer = &H4A
    Public Const MSGFLT_ALLOW As Integer = 1
    Public Const MSGFLT_ADD As Integer = 1
    Public Const MSGFLT_REMOVE As Integer = 2
    '窗口常量
    Public Const WM_SYSCOLORCHANGE = &H15S '当系统颜色改变时, 发送此消息给所有顶级窗口
    Public Const WM_SETFOCUS = &H7S '窗体获得焦点
    Public Const WM_KILLFOCUS = &H8S '窗体失去焦点
    Public Const WM_COMMAND = &H111 '窗体选择菜单项
    Public Const WM_SYSCOMMAND = &H112 '窗体选择系统菜单项
    Public Const WM_DWMCOLORIZATIONCOLORCHANGED = &H320 '窗体主题色被更改(深色同样有效)
    '菜单常量
    Public Const MF_SEPARATOR = &H800 '分隔符
    Public Const MF_STRING = &H0 '字符串
    Public Const MF_BITMAP = &H4 '位图
    Public Const MF_GRAYED = &H1 '灰色菜单
    Public Const MF_ENABLED = &H0 '菜单可用
    Public Const MF_CHECKED = &H8 '勾选
    Public Const MF_UNCHECKED = &H0 '取消勾选
    Public Const MF_HILITE = &H80 '高亮
    Public Const MF_BYCOMMAND = &H0 '标识符
    Public Const MF_BYPOSITION = &H400 '位置
    '菜单项常量
    Public Const SC_RESTORE = &HF120 '还原
    Public Const SC_MOVE = &HF010 '移动
    Public Const SC_SIZE = &HF000 '大小
    Public Const SC_MINIMIZE = &HF020 '最小化
    Public Const SC_MAXIMIZE = &HF030 '最大化
    Public Const SC_CLOSE = &HF060 '关闭
#End Region

#Region "主题"
    'SetPreferredAppMode函数 - 设置应用程序首选主题模式
    <DllImport("uxtheme.dll", EntryPoint:="#135", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Public Function SetPreferredAppMode(ByVal PreferredAppMode As PreferredAppMode) As Long
        '修改菜单颜色
    End Function
    'SetWindowTheme函数 - 设置特定窗口的主题
    <DllImport("uxtheme.dll", EntryPoint:="#135", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Public Function SetWindowTheme(ByVal hwnd As IntPtr, ByVal pszSubAppName As String, ByVal pszSubIdList As String) As Long
        '使用主题
    End Function
    'FlushMenuThemes函数 - 刷新菜单主题
    <DllImport("uxtheme.dll", EntryPoint:="#136", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Public Function FlushMenuThemes() As Long
        '修改菜单颜色
    End Function
    Public Enum PreferredAppMode
        _default
        AllowDark
        ForceDark
        ForceLight
        Max
    End Enum
    'DwmSetWindowAttribute函数 - 设置桌面窗口管理器(DWM)属性
    <DllImport("DwmApi.dll", EntryPoint:="DwmSetWindowAttribute", SetLastError:=True)>
    Public Function DwmSetWindowAttribute(
        ByVal hwnd As IntPtr,
        ByVal attr As DwmWindowAttribute,
        ByRef attrValue As Integer,
        ByVal attrSize As Integer
    ) As Integer
    End Function
    Public Enum DwmWindowAttribute
        NCRenderingEnabled = 1
        NCRenderingPolicy
        TransitionsForceDisabled
        AllowNCPaint
        CaptionButtonBounds
        NonClientRtlLayout
        ForceIconicRepresentation
        Flip3DPolicy
        ExtendedFrameBounds
        HasIconicBitmap
        DisallowPeek
        ExcludedFromPeek
        Cloak
        Cloaked
        FreezeRepresentation
        PassiveUpdateMode
        UseHostBackdropBrush
        UseImmersiveDarkMode = 20
        WindowCornerPreference = 33
        BorderColor
        CaptionColor
        TextColor
        VisibleFrameBorderThickness
        SystemBackdropType
        Last
    End Enum
#End Region

End Module
