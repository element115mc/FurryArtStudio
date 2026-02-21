Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading

Public Class ViewForm
    '稿件
    Private _currentArtwork As Artwork '当前稿件
    Private _allArtworks As List(Of Artwork) '全部稿件列表
    '文件
    Private _currentFileIndex As Integer = 0 '当前文件索引
    Private _currentArtworkIndex As Integer = -1 '当前稿件索引
    '异步
    Private _isProcessing As Boolean = False '正在处理信号量
    Private _loadingLock As New Object() '锁对象
    Private _loadingTask As Task '异步加载任务
    Private _cancellationTokenSource As CancellationTokenSource '任务取消令牌
    '事件
    Private _mainForm As Form '保存主窗口引用
    '支持的文件扩展名
    Private ReadOnly _imageExtensions As String() = {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".ico", ".webp"}
#Region "窗体相关"
    ''' <summary>
    ''' 构造函数 - 接收当前稿件和所有稿件列表
    ''' </summary>
    Public Sub New(currentArtwork As Artwork, allArtworks As List(Of Artwork))
        InitializeComponent()
        _currentArtwork = currentArtwork
        _allArtworks = allArtworks
        _mainForm = MainForm
        '查找当前稿件在所有稿件列表中的索引
        If _allArtworks IsNot Nothing Then
            _currentArtworkIndex = _allArtworks.FindIndex(Function(a) a.ID = currentArtwork.ID)
        End If
        If TypeOf _mainForm Is MainForm Then
            AddHandler DirectCast(_mainForm, MainForm).LibraryClosed, AddressOf OnLibraryClosed
        End If
    End Sub

    ''' <summary>
    ''' 窗体加载事件
    ''' </summary>
    Private Sub ViewForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        Me.Text = "图片浏览器"
        PictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom
        PictureBoxMain.Dock = DockStyle.Fill
        SystemThemeChange()
        '加载当前稿件的第一张图片
        LoadCurrentArtworkFirstImage()
    End Sub

    ''' <summary>
    ''' 窗体关闭时释放资源
    ''' </summary>
    Private Sub ViewForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _mainForm IsNot Nothing AndAlso TypeOf _mainForm Is MainForm Then
            RemoveHandler DirectCast(_mainForm, MainForm).LibraryClosed, AddressOf OnLibraryClosed
        End If
        If PictureBoxMain.Image IsNot Nothing Then
            PictureBoxMain.Image.Dispose()
            PictureBoxMain.Image = Nothing
        End If
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
                Icon = CreateRoundedRectangleIcon(True, My.Resources.Icons.FormImageDark)
            Else
                bgColor = BgColorLight
                frColor = FrColorLight
                Icon = CreateRoundedRectangleIcon(False, My.Resources.Icons.FormImageLight)
            End If
            For Each control In controlList
                control.ForeColor = frColor
                control.BackColor = bgColor
            Next
            ForeColor = frColor
            BackColor = bgColor
            'WinAPI
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.UseImmersiveDarkMode, isDarkMode, Marshal.SizeOf(Of Integer))
            SetPreferredAppMode(PreferredAppMode.AllowDark)
            FlushMenuThemes()
        End Using
    End Sub
#End Region

#Region "辅助函数"
    ''' <summary>
    ''' 从文件路径数组中过滤出图片文件
    ''' </summary>
    ''' <param name="filePaths">文件夹</param>
    ''' <returns>图片文件路径</returns>
    Private Function GetImageFiles(filePaths As String()) As List(Of String)
        Dim result As New List(Of String)
        If filePaths Is Nothing Then Return result '没有文件
        For Each p In filePaths
            '跳过预览图文件
            If Path.GetFileName(p).ToLower() = ".preview.jpg" Then
                Continue For
            End If
            Dim ext As String = Path.GetExtension(p).ToLower()
            If _imageExtensions.Contains(ext) Then
                result.Add(p)
            End If
        Next
        '按文件名排序
        result.Sort()
        Return result
    End Function

    ''' <summary>
    ''' 加载当前稿件的第一个有效图片
    ''' </summary>
    Private Sub LoadCurrentArtworkFirstImage()
        If _currentArtwork Is Nothing OrElse _currentArtwork.FilePaths Is Nothing Then
            MessageBox.Show("当前稿件目录为空", "Furry Art Studio", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
            Return
        End If
        Dim imageFiles As List(Of String) = GetImageFiles(_currentArtwork.FilePaths)
        If imageFiles.Count = 0 Then
            MessageBox.Show("当前稿件没有支持的图片格式文件", "Furry Art Studio", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
            Return
        End If
        _currentFileIndex = 0
        LoadImageAsync(imageFiles(_currentFileIndex))
    End Sub

    ''' <summary>
    ''' 获取当前稿件的所有图片文件
    ''' </summary>
    ''' <returns>图片文件路径</returns>
    Private Function GetCurrentArtworkImages() As List(Of String)
        If _currentArtwork Is Nothing OrElse _currentArtwork.FilePaths Is Nothing Then
            Return New List(Of String)
        End If
        Return GetImageFiles(_currentArtwork.FilePaths)
    End Function

    ''' <summary>
    ''' 更新窗口标题
    ''' </summary>
    ''' <param name="currentFilePath">当前文件路径</param>
    Private Sub UpdateWindowTitle(Optional currentFilePath As String = Nothing)
        If _currentArtwork Is Nothing Then '没有文件
            Me.Text = "图片浏览器"
            Return
        End If
        Dim title As String = _currentArtwork.Title
        If String.IsNullOrWhiteSpace(title) Then '没有标题
            title = "无标题"
        End If
        Dim imageFiles As List(Of String) = GetCurrentArtworkImages()
        Dim totalImages As Integer = imageFiles.Count
        If totalImages = 0 Then
            Me.Text = $"{title} - 图片浏览器 [0/0]"
        Else
            Dim fileName As String = ""
            If currentFilePath IsNot Nothing Then
                fileName = Path.GetFileName(currentFilePath)
            End If
            '显示格式: 标题 - [当前文件索引/总文件数] 文件名 - 图片浏览器 (当前稿件索引/总稿件数)
            If _currentArtworkIndex >= 0 AndAlso _allArtworks IsNot Nothing Then
                Me.Text = $"{title} - [{_currentFileIndex + 1}/{totalImages}] {fileName} - 图片浏览器 ({_currentArtworkIndex + 1}/{_allArtworks.Count})"
            Else
                Me.Text = $"{title} - [{_currentFileIndex + 1}/{totalImages}] {fileName} - 图片浏览器"
            End If
        End If
    End Sub

    ''' <summary>
    ''' 显示稿件信息
    ''' </summary>
    Private Sub ShowArtworkInfo()
        If _currentArtwork Is Nothing Then Return
        Dim info As String = $"标题: {_currentArtwork.Title}{vbCrLf}" &
                             $"作者: {_currentArtwork.Author}{vbCrLf}" &
                             $"UUID: {_currentArtwork.UUID}{vbCrLf}" &
                             $"角色: {FormatArrayWithEllipsis(_currentArtwork.Characters)}{vbCrLf}" &
                             $"标签: {FormatArrayWithEllipsis(_currentArtwork.Tags)}{vbCrLf}" &
                             $"创作时间: {_currentArtwork.CreateTime:yyyy-MM-dd HH:mm:ss}{vbCrLf}" &
                             $"入库时间: {_currentArtwork.ImportTime:yyyy-MM-dd HH:mm:ss}{vbCrLf}" &
                             $"更新时间: {_currentArtwork.UpdateTime:yyyy-MM-dd HH:mm:ss}{vbCrLf}" &
                             $"备注: {_currentArtwork.Notes}{vbCrLf}"
        MessageBox.Show(info, "稿件信息", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>
    ''' 当库关闭时, 本窗口也将关闭
    ''' </summary>
    Private Sub OnLibraryClosed(sender As Object, e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() Me.Close())
        Else
            Me.Close()
        End If
    End Sub

    ''' <summary>
    ''' 异步加载图片
    ''' </summary>
    ''' <param name="filePath">图片路径</param>
    Private Async Sub LoadImageAsync(filePath As String)
        Try
            SyncLock _loadingLock
                If _isProcessing Then Return  '防止重复进入
                _isProcessing = True
            End SyncLock
            '取消之前的加载任务
            _cancellationTokenSource?.Cancel()
            _cancellationTokenSource = New CancellationTokenSource()
            '显示加载提示
            PictureBoxMain.Image = Nothing
            Me.Text = "加载中... " & Path.GetFileName(filePath)
            Me.Cursor = Cursors.WaitCursor
            '异步加载图片
            Dim image = Await Task.Run(Function() LoadImageWithResize(filePath, 1920, 1080, _cancellationTokenSource.Token),
                                       _cancellationTokenSource.Token)
            '检查是否被取消
            If _cancellationTokenSource.Token.IsCancellationRequested Then
                image?.Dispose()
                Return
            End If
            '更新UI
            If image IsNot Nothing Then
                '释放旧图片
                If PictureBoxMain.Image IsNot Nothing Then
                    Dim oldImage = PictureBoxMain.Image
                    PictureBoxMain.Image = Nothing
                    oldImage.Dispose()
                End If
                PictureBoxMain.Image = image
                UpdateWindowTitle(filePath)
            End If
        Catch ex As OperationCanceledException
            '忽略取消事件
        Catch ex As Exception
            MessageBox.Show($"加载图片时出错: {ex.Message}", "Furry Art Studio",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            '无论如何都要释放加载状态
            SyncLock _loadingLock
                _isProcessing = False
            End SyncLock
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 在后台线程中加载并调整图片大小
    ''' </summary>
    ''' <param name="filePath">文件路径</param>
    ''' <param name="maxWidth">最大宽度</param>
    ''' <param name="maxHeight">最大高度</param>
    ''' <param name="cancellationToken">取消令牌</param>
    ''' <returns>新的图片</returns>
    Private Function LoadImageWithResize(filePath As String, maxWidth As Integer,
                                         maxHeight As Integer, cancellationToken As CancellationToken) As Image
        If Not File.Exists(filePath) Then Return Nothing
        Using fs As New FileStream(filePath, FileMode.Open,
                                             FileAccess.Read,
                                             FileShare.Read, 4096, True)
            cancellationToken.ThrowIfCancellationRequested()
            '使用Image.FromStream避免文件锁定
            Using original As Image = Image.FromStream(fs)
                cancellationToken.ThrowIfCancellationRequested()
                '如果图片小于最大尺寸，直接返回副本
                If original.Width <= maxWidth AndAlso original.Height <= maxHeight Then
                    Return New Bitmap(original)
                End If
                '计算缩放尺寸
                Dim ratio As Double = Math.Min(maxWidth / original.Width, maxHeight / original.Height)
                Dim newWidth As Integer = CInt(original.Width * ratio)
                Dim newHeight As Integer = CInt(original.Height * ratio)
                '创建缩放的图片
                Dim resized As New Bitmap(newWidth, newHeight)
                Using g As Graphics = Graphics.FromImage(resized)
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.DrawImage(original, 0, 0, newWidth, newHeight)
                End Using
                cancellationToken.ThrowIfCancellationRequested()
                Return resized
            End Using
        End Using
    End Function

#End Region

#Region "图片导航"
    ''' <summary>
    ''' 导航到下一张/下一个稿件
    ''' </summary>
    Private Sub NavigateNext()
        Dim currentImageFiles As List(Of String) = GetCurrentArtworkImages()
        '如果当前稿件还有下一张图片
        If _currentFileIndex < currentImageFiles.Count - 1 Then
            _currentFileIndex += 1
            LoadImageAsync(currentImageFiles(_currentFileIndex))
            Return
        End If
        '当前稿件没有下一张图片, 尝试切换到下一个稿件
        If _allArtworks IsNot Nothing AndAlso _currentArtworkIndex < _allArtworks.Count - 1 Then
            '找到下一个有图片的稿件
            For i As Integer = _currentArtworkIndex + 1 To _allArtworks.Count - 1
                Dim nextArtwork As Artwork = _allArtworks(i)
                Dim nextImageFiles As List(Of String) = GetImageFiles(nextArtwork.FilePaths)
                If nextImageFiles.Count > 0 Then
                    '切换到下一个稿件的第一个图片
                    _currentArtworkIndex = i
                    _currentArtwork = nextArtwork
                    _currentFileIndex = 0
                    LoadImageAsync(nextImageFiles(0))
                    Return
                End If
            Next
        End If
        MessageBox.Show("已经是最后一张图片了", "Furry Art Studio", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>
    ''' 导航到上一张/上一个稿件
    ''' </summary>
    Private Sub NavigatePrevious()
        Dim currentImageFiles As List(Of String) = GetCurrentArtworkImages()
        '如果当前稿件还有上一张图片
        If _currentFileIndex > 0 Then
            _currentFileIndex -= 1
            LoadImageAsync(currentImageFiles(_currentFileIndex))
            Return
        End If
        '当前稿件没有上一张图片, 尝试切换到上一个稿件
        If _allArtworks IsNot Nothing AndAlso _currentArtworkIndex > 0 Then
            '找到上一个有图片的稿件
            For i As Integer = _currentArtworkIndex - 1 To 0 Step -1
                Dim prevArtwork As Artwork = _allArtworks(i)
                Dim prevImageFiles As List(Of String) = GetImageFiles(prevArtwork.FilePaths)
                If prevImageFiles.Count > 0 Then
                    '切换到上一个稿件的最后一张图片
                    _currentArtworkIndex = i
                    _currentArtwork = prevArtwork
                    _currentFileIndex = prevImageFiles.Count - 1
                    LoadImageAsync(prevImageFiles(_currentFileIndex))
                    Return
                End If
            Next
        End If
        MessageBox.Show("已经是第一张图片了", "Furry Art Studio", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>
    ''' 导航到第一张稿件
    ''' </summary>
    Private Sub NavigateToFirstArtwork()
        If _allArtworks Is Nothing Then Return

        For i As Integer = 0 To _allArtworks.Count - 1
            Dim imageFiles As List(Of String) = GetImageFiles(_allArtworks(i).FilePaths)
            If imageFiles.Count > 0 Then
                _currentArtworkIndex = i
                _currentArtwork = _allArtworks(i)
                _currentFileIndex = 0
                LoadImageAsync(imageFiles(0))
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' 导航到最后一张稿件
    ''' </summary>
    Private Sub NavigateToLastArtwork()
        If _allArtworks Is Nothing Then Return

        For i As Integer = _allArtworks.Count - 1 To 0 Step -1
            Dim imageFiles As List(Of String) = GetImageFiles(_allArtworks(i).FilePaths)
            If imageFiles.Count > 0 Then
                _currentArtworkIndex = i
                _currentArtwork = _allArtworks(i)
                _currentFileIndex = 0
                LoadImageAsync(imageFiles(0))
                Exit For
            End If
        Next
    End Sub
    ''' <summary>
    ''' 导航到上一个稿件
    ''' </summary>
    Private Sub NavigatePreviousArtwork()
        If _allArtworks Is Nothing OrElse _allArtworks.Count = 0 Then Return
        '找到上一个有图片的稿件
        For i As Integer = _currentArtworkIndex - 1 To 0 Step -1
            Dim prevArtwork As Artwork = _allArtworks(i)
            Dim prevImageFiles As List(Of String) = GetImageFiles(prevArtwork.FilePaths)

            If prevImageFiles.Count > 0 Then
                _currentArtworkIndex = i
                _currentArtwork = prevArtwork
                _currentFileIndex = 0
                LoadImageAsync(prevImageFiles(0))
                Return
            End If
        Next
        MessageBox.Show("已经是第一个有图片的稿件了", "Furry Art Studio", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
    ''' <summary>
    ''' 导航到下一个稿件
    ''' </summary>
    Private Sub NavigateNextArtwork()
        If _allArtworks Is Nothing OrElse _allArtworks.Count = 0 Then Return
        '找到下一个有图片的稿件
        For i As Integer = _currentArtworkIndex + 1 To _allArtworks.Count - 1
            Dim nextArtwork As Artwork = _allArtworks(i)
            Dim nextImageFiles As List(Of String) = GetImageFiles(nextArtwork.FilePaths)

            If nextImageFiles.Count > 0 Then
                _currentArtworkIndex = i
                _currentArtwork = nextArtwork
                _currentFileIndex = 0
                LoadImageAsync(nextImageFiles(0))
                Return
            End If
        Next
        MessageBox.Show("已经是最后一个有图片的稿件了", "Furry Art Studio", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
#End Region

#Region "按键处理"
    '窗体键盘事件处理 - 使用窗体事件确保响应
    Private Sub ViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If _isProcessing Then '当正在加载图片时, 取消处理按键响应
            e.Handled = True
            Return
        End If
        '处理组合键
        If e.Control Then
            Select Case e.KeyCode
                Case Keys.PageUp '上一个稿件
                    NavigatePreviousArtwork()
                    e.Handled = True
                    Return
                Case Keys.PageDown '下一个稿件
                    NavigateNextArtwork()
                    e.Handled = True
                    Return
            End Select
        End If
        '处理单键
        Select Case e.KeyCode
            Case Keys.Left, Keys.P, Keys.PageUp, Keys.Up, Keys.Oemcomma, Keys.A, Keys.W '上一张
                NavigatePrevious()
                e.Handled = True
            Case Keys.Right, Keys.N, Keys.PageDown, Keys.Down, Keys.OemPeriod, Keys.S, Keys.D,
                Keys.Space, Keys.Enter '下一张
                NavigateNext()
                e.Handled = True
            Case Keys.Home '第一张
                NavigateToFirstArtwork()
                e.Handled = True
            Case Keys.End '最后一张
                NavigateToLastArtwork()
                e.Handled = True
            Case Keys.Escape '退出
                Me.Close()
                e.Handled = True
            Case Keys.F11 '全屏切换
                ToggleFullScreen()
                e.Handled = True
            Case Keys.I '显示信息
                ShowArtworkInfo()
                e.Handled = True
            Case Keys.Insert '老板键
        End Select
    End Sub
#End Region

#Region "全屏处理"
    Private Sub PictureBoxMain_DoubleClick(sender As Object, e As EventArgs) Handles PictureBoxMain.DoubleClick
        ToggleFullScreen()
    End Sub
    '切换全屏模式
    Private Sub ToggleFullScreen()
        '全屏
    End Sub
#End Region

End Class