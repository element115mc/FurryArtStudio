Imports System.IO
Imports System.Runtime.InteropServices

''' <summary>
''' 基本函数
''' </summary>
Module BasicFcn
    Public ReadOnly SeparatorEqual As New String("="c, 30)
    Public ReadOnly SeparatorStar As New String("*"c, 30)
    Public ReadOnly SeparatorDash As New String("-"c, 30)
    ''' <summary>
    ''' 初始化日志记录器实例
    ''' </summary>
    Public Sub LoggerInit()
        Dim appPath As String = AppContext.BaseDirectory '程序路径
        Dim logPath As String = Path.Combine(appPath, "Logs") '日志路径
        Directory.CreateDirectory(logPath)
        Dim logFilePath As String = Path.Combine(logPath, "Latest.log") '日志文件路径
        If File.Exists(logFilePath) Then
            Dim lastLogFileDate As Date = File.GetLastWriteTime(logFilePath)
            File.Move(logFilePath, Path.Combine(logPath, $"{lastLogFileDate:yyyy-MM-dd_HH-mm-ss}.log"))
        End If '当先前的日志文件存在时, 更名
        Dim logConfig = New LoggerConfig() With {
            .LogPath = logPath,
            .MinLogLevel = LogLevel.DEBUG,
            .AutoFlush = False,
            .DateFormat = "HH:mm:ss.fff",
            .LogFormat = "{timestamp} {level} {message}",
            .Encoding = Text.Encoding.UTF8
        }
        Logger.Initialize(logConfig) '初始化日志记录器
    End Sub

    ''' <summary>
    ''' 将DateTime对象转换成64位时间戳
    ''' </summary>
    Public Function DateTimeToUnixTimestamp(time As DateTime) As Long
        Dim epoch As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        Dim utcDt As DateTime = time.ToUniversalTime()
        Dim timeSpan As TimeSpan = utcDt - epoch
        Return CLng(timeSpan.TotalSeconds)
    End Function

    ''' <summary>
    ''' 将64位时间戳转换成DateTime对象
    ''' </summary>
    Public Function UnixTimestampToDateTime(unixTimestamp As Long) As DateTime
        Dim epoch As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        Return epoch.AddSeconds(unixTimestamp).ToLocalTime()
    End Function

    ''' <summary>
    ''' 获得文件夹信息
    ''' </summary>
    ''' <param name="folderPath">文件夹路径</param>
    ''' <returns>文件数量与文件夹大小</returns>
    Public Function GetFolderInfo(ByVal folderPath As String) As (fileCount As Long, totalSize As Long, sizeString As String)
        If Not Directory.Exists(folderPath) Then
            Throw New DirectoryNotFoundException("文件夹不存在: " & folderPath)
        End If
        Dim fileCount As Long, totalSize As Long = 0
        Dim files As String() = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories) '获取所有文件及子文件夹
        fileCount = files.Length
        For Each file In files '计算总大小
            Try
                Dim fileInfo As New FileInfo(file)
                totalSize += fileInfo.Length
            Catch ex As Exception
                '忽略无法访问的文件
            End Try
        Next
        Dim sizeString As String = FormatFileSize(totalSize) '格式化大小
        Return (fileCount, totalSize, sizeString)
    End Function

    ''' <summary>
    ''' 将存储空间转换为人类可读的格式
    ''' </summary>
    ''' <param name="bytes">字节数</param>
    ''' <returns>人类可读的存储空间</returns>
    Public Function FormatFileSize(ByVal bytes As Long) As String
        Dim size As Double = bytes
        Dim units As String() = {"B", "KB", "MB", "GB", "TB"}
        Dim unitIndex As Integer = 0

        While size >= 1024 AndAlso unitIndex < units.Length - 1
            size /= 1024
            unitIndex += 1
        End While

        Return $"{size:N2}{units(unitIndex)}"
    End Function

    ''' <summary>
    ''' 将数组转换成逗号分隔的形式
    ''' </summary>
    ''' <param name="arr">要处理的数组</param>
    ''' <param name="omitAfter">(可选)要显示的元素数量</param>
    Public Function FormatArrayWithEllipsis(arr As String(), Optional omitAfter As Integer = -1) As String
        If arr Is Nothing OrElse arr.Length = 0 Then
            Return String.Empty
        End If

        If omitAfter <= 0 OrElse omitAfter >= arr.Length Then Return String.Join(", ", arr) '返回所有元素

        '获取要显示的部分
        Dim visiblePart = arr.Take(omitAfter).ToArray()
        '获取省略的部分, 用于计数
        Dim omittedCount = arr.Length - omitAfter
        '创建结果字符串
        Dim result = String.Join(", ", visiblePart)
        '添加省略号
        result &= $", ... ({omittedCount} more)"
        Return result
    End Function

    ''' <summary>
    ''' 将目录复制到剪贴板
    ''' </summary>
    ''' <param name="directoryPath">要复制的目录路径字符串</param>
    Public Sub CopyDirectoryToClipboard(directoryPath As String)
        If Not Directory.Exists(directoryPath) Then
            Throw New DirectoryNotFoundException($"目录不存在: {directoryPath}")
        End If
        Dim files As String() = Directory.GetFiles(directoryPath, "*.*",
                                                   SearchOption.AllDirectories) '创建 FileDrop 格式的数据
        Dim data As New System.Collections.Specialized.StringCollection From {
            directoryPath '添加目录本身
            } '将目录添加到列表
        Clipboard.SetFileDropList(data) '设置到剪贴板
    End Sub

    ''' <summary>
    ''' 将多个目录复制到剪贴板
    ''' </summary>
    ''' <param name="directoryPaths">要复制的目录路径字符串数组</param>
    ''' <param name="dataObject">(可选)若提供<seealso cref="DataObject"/>,则在此基础上添加数据</param>
    Public Sub CopyDirectoryToClipboard(directoryPaths As String(), Optional dataObject As DataObject = Nothing)
        '验证所有目录是否存在
        For Each dirPath As String In directoryPaths
            If Not Directory.Exists(dirPath) Then
                Throw New DirectoryNotFoundException($"目录不存在: {dirPath}")
            End If
        Next
        '创建 StringCollection 并添加所有目录
        Dim data As New System.Collections.Specialized.StringCollection()
        '添加所有目录路径
        For Each dirPath As String In directoryPaths
            data.Add(dirPath)
        Next
        If dataObject Is Nothing Then '设置到剪贴板
            Clipboard.SetFileDropList(data)
        Else
            Clipboard.SetDataObject(dataObject, True)
        End If
    End Sub

    ''' <summary>
    ''' 从文件载入图片, 并裁剪为正方形的缩略图
    ''' </summary>
    ''' <param name="filePath">文件路径</param>
    ''' <returns>裁剪好的<seealso cref="Image"/>对象</returns>
    Public Function LoadImageFromFile(filePath As String) As Image
        If String.IsNullOrEmpty(filePath) Then Return Nothing
        If Not File.Exists(filePath) Then Return Nothing
        Try
            '验证文件扩展名是否为支持的图像格式
            Dim extension As String = Path.GetExtension(filePath).ToLower()
            Dim supportedFormats As String() = {".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".ico", ".wmf", ".emf"}
            If Not supportedFormats.Contains(extension) Then Return Nothing
            '使用 FromFile 方法加载图像, 但先复制到内存以避免文件锁定
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Using memoryStream As New MemoryStream()
                    fs.CopyTo(memoryStream)
                    memoryStream.Position = 0
                    '从内存流加载图像
                    Using img As Image = Image.FromStream(memoryStream)
                        '验证图像是否有效
                        If img Is Nothing OrElse img.Width = 0 Or img.Height = 0 Then Return Nothing
                        Dim size As Integer = Math.Min(img.Width, img.Height)
                        '计算裁剪区域
                        Dim cropRect As New Rectangle(
                            (img.Width - size) \ 2,
                            (img.Height - size) \ 2,
                            size,
                            size)
                        Dim croppedImage As Bitmap = Nothing
                        Dim outputSize As Integer = 256
                        croppedImage = New Bitmap(outputSize, outputSize, img.PixelFormat)
                        croppedImage.SetResolution(img.HorizontalResolution, img.VerticalResolution) '设置图像分辨率
                        '使用Graphics对象进行裁剪
                        Using g As Graphics = Graphics.FromImage(croppedImage)
                            '设置高质量绘制选项
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
                            '绘制裁剪部分
                            g.DrawImage(img,
                                    New Rectangle(0, 0, outputSize, outputSize),
                                    cropRect,
                                    GraphicsUnit.Pixel)
                        End Using
                        '返回裁剪后图像的副本
                        Return CType(croppedImage.Clone(), Image)
                    End Using 'img 在这里释放
                End Using 'memoryStream 在这里释放
            End Using 'fs 在这里释放
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' 获得特定控件的全部子控件
    ''' </summary>
    ''' <param name="container">父控件</param>
    ''' <returns>子控件集合</returns>
    Public Function GetAllControls(container As Control) As List(Of Control)
        Dim controls As New List(Of Control)
        GetAllControlsRecursive(container, controls)
        Return controls
    End Function
    Private Sub GetAllControlsRecursive(container As Control, ByRef controlList As List(Of Control))
        For Each control As Control In container.Controls
            controlList.Add(control)
            ' 递归获取子控件
            If control.HasChildren Then
                GetAllControlsRecursive(control, controlList)
            End If
        Next
    End Sub
    ''' <summary>
    ''' 将 RGB 转换成 COLORREF 格式
    ''' </summary>
    Public Function RGBToCOLORREF(ByVal r As Byte, ByVal g As Byte, ByVal b As Byte) As Integer
        'COLORREF 是 BGR 格式: 0x00BBGGRR
        Return CInt(b) Or (CInt(g) << 8) Or (CInt(r) << 16)
    End Function
    Public Sub SetTitleBarColor(ByVal hwnd As IntPtr, ByVal r As Byte, ByVal g As Byte, ByVal b As Byte)
        Try
            Dim colorRef As Integer = RGBToCOLORREF(r, g, b)
            '设置标题栏背景色
            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.CaptionColor, colorRef, Marshal.SizeOf(Of Integer)())
            '根据背景亮度决定文字颜色
            Dim brightness As Double = (0.299 * r + 0.587 * g + 0.114 * b)
            Dim textColor As Integer = If(brightness > 128,
                                          RGBToCOLORREF(0, 0, 0),      '深色背景用白色文字
                                          RGBToCOLORREF(255, 255, 255)) '浅色背景用黑色文字
            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.TextColor, textColor, Marshal.SizeOf(Of Integer)())
            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.BorderColor, textColor, Marshal.SizeOf(Of Integer)())
        Catch ex As Exception
            MessageBox.Show("设置标题栏颜色失败: " & ex.Message)
        End Try
    End Sub
End Module
