Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices

Public Class EditDialogForm
    '存储传入的稿件对象
    Private _artwork As Artwork
    Private _libraryPath As String
    Private _filePaths As String() = Array.Empty(Of String)()

#Region "窗体相关"
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
                Icon = CreateRoundedRectangleIcon(True, My.Resources.Icons.MenuEditDark)
            Else
                bgColor = BgColorLight
                frColor = FrColorLight
                Icon = CreateRoundedRectangleIcon(False, My.Resources.Icons.MenuEditLight)
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
    Private Sub EditDialogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SystemThemeChange()
        Dim MnuHandle = GetSystemMenu(Handle, False) '获取菜单句柄
        RemoveMenu(MnuHandle, SC_RESTORE, MF_BYCOMMAND) '去除还原菜单
        RemoveMenu(MnuHandle, SC_MAXIMIZE, MF_BYCOMMAND) '去除最大化菜单
        RemoveMenu(MnuHandle, SC_SIZE, MF_BYCOMMAND) '去除大小菜单
        RemoveMenu(MnuHandle, SC_MINIMIZE, MF_BYCOMMAND) '去除最小化菜单
        InsertMenu(MnuHandle, 1, MF_BYPOSITION Or MF_SEPARATOR, 0, Nothing)
        InsertMenu(MnuHandle, 2, MF_BYPOSITION Or MF_STRING, 1, "添加图片(&A)")
        InsertMenu(MnuHandle, 3, MF_BYPOSITION Or MF_STRING, 2, "保存(&S)")
    End Sub

    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="artwork">要编辑的稿件对象</param>
    ''' <param name="libraryPath">稿件库路径</param>
    Public Sub New(artwork As Artwork, libraryPath As String)
        InitializeComponent()
        _libraryPath = libraryPath
        If artwork.ID = 0 Then '用户拖入图片添加
            Me.Text = "新建稿件"
            BtnModify.Text = "添加(&A)"
            _artwork = New Artwork With {
                        .ID = 0,
                        .UUID = Guid.NewGuid(),
                        .Title = artwork.Title,'已知
                        .Author = String.Empty,
                        .Characters = Array.Empty(Of String)(),
                        .CreateTime = artwork.CreateTime,'已知
                        .ImportTime = DateTime.Now,
                        .UpdateTime = DateTime.Now,
                        .IsDeleted = 0,
                        .Tags = Array.Empty(Of String)(),
                        .Notes = String.Empty,
                        .FilePaths = Array.Empty(Of String)()
                    }
        Else '编辑已有稿件
            Me.Text = "编辑稿件"
            BtnModify.Text = "保存(&S)"
            _artwork = artwork
        End If
        InitializeForm()
    End Sub

    ''' <summary>
    ''' 初始化窗体显示
    ''' </summary>
    Private Sub InitializeForm()
        TxtboxTitle.Text = _artwork.Title '标题
        TxtboxAuthor.Text = _artwork.Author '作者
        TxtboxCharacters.Text = If(_artwork.Characters IsNot Nothing, String.Join(" ", _artwork.Characters), "") '角色
        TxtboxTags.Text = If(_artwork.Tags IsNot Nothing, String.Join(" ", _artwork.Tags), "") '标签
        TxtboxCreateTime.Text = _artwork.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") '创作时间
        LblImportTime.Text = "导入时间: " & _artwork.ImportTime.ToString("yyyy-MM-dd HH:mm:ss") '导入时间
        LblUpdateTime.Text = "更新时间: " & _artwork.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss") '更新时间
        LblUUID.Text = _artwork.UUID.ToString() 'UUID
        TxtboxNotes.Text = _artwork.Notes '备注
        TxtboxCreateTime.ForeColor = Color.Gray '设置创建时间文本框的提示
        '加载预览图
        LoadPreviewImage()
    End Sub

    ''' <summary>
    ''' 加载预览图片
    ''' </summary>
    Private Sub LoadPreviewImage()
        Try
            '如果稿件文件夹存在, 尝试查找预览图
            Dim artworkFolder As String = Path.Combine(_libraryPath, _artwork.UUID.ToString())
            If Directory.Exists(artworkFolder) Then
                '查找预览图文件
                Dim previewFiles = Directory.GetFiles(artworkFolder, ".preview.*")
                If previewFiles.Length > 0 Then
                    '加载第一个预览图
                    Using stream As New FileStream(previewFiles(0), FileMode.Open, FileAccess.Read)
                        PreviewPicturebox.Image = Image.FromStream(stream)
                    End Using
                    PreviewPicturebox.SizeMode = PictureBoxSizeMode.Zoom
                    Return
                End If
                '如果没有预览图, 尝试加载第一个图片文件
                Dim imageFiles = Directory.GetFiles(artworkFolder, "*.*") _
                    .Where(Function(f)
                               Dim ext = Path.GetExtension(f).ToLower()
                               Return ext = ".jpg" OrElse ext = ".jpeg" OrElse
                                      ext = ".png" OrElse ext = ".bmp" OrElse
                                      ext = ".gif" OrElse ext = ".tiff"
                           End Function) _
                    .ToArray()
                If imageFiles.Length > 0 Then
                    Using stream As New FileStream(imageFiles(0), FileMode.Open, FileAccess.Read)
                        PreviewPicturebox.Image = Image.FromStream(stream)
                    End Using
                    PreviewPicturebox.SizeMode = PictureBoxSizeMode.Zoom
                End If
            End If
        Catch ex As Exception
            '加载图片失败
            PreviewPicturebox.Image = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' 获取编辑后的稿件对象
    ''' </summary>
    Public ReadOnly Property EditedArtwork As Artwork
        Get
            Return _artwork
        End Get
    End Property

    Protected Overrides Sub WndProc(ByRef m As Message) '窗体消息处理函数
        If m.Msg = WM_SYSCOMMAND Then '窗体响应菜单
            Dim hMenu = GetSystemMenu(Handle, False)
            Select Case m.WParam.ToInt32'对应菜单标号
                Case 1
                    AddPictures()
                Case 2
                    ModifyArtwork()
            End Select
        End If
        MyBase.WndProc(m) '循环监听消息
    End Sub

#End Region

#Region "其他功能"
    ''' <summary>
    ''' 创建稿件文件夹
    ''' </summary>
    Private Function CreateArtworkFolder() As Boolean
        Try
            Dim folderPath As String = Path.Combine(_libraryPath, _artwork.UUID.ToString())
            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show($"创建稿件文件夹时出错：{ex.Message}", "Furry Art Studio",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function
    Private Sub BtnModify_Click(sender As Object, e As EventArgs) Handles BtnModify.Click
        ModifyArtwork()
    End Sub
    Private Sub CreateThumb()
        Dim folderPath As String = Path.Combine(_libraryPath, _artwork.UUID.ToString())
        Dim previewPath As String = Path.Combine(folderPath, ".preview.jpg")
        If Not File.Exists(previewPath) Then '判断是否存在文件
            PreviewPicturebox.Image = LoadImageFromFile(Directory.GetFiles(folderPath)(0))
            PreviewPicturebox.SizeMode = PictureBoxSizeMode.Zoom
            PreviewPicturebox.Image.Save(previewPath, ImageFormat.Jpeg)
        End If
    End Sub
    Private Sub ModifyArtwork()
        If TxtboxTitle.Text = "" Then
            _artwork.Title = "无题"
        Else
            _artwork.Title = TxtboxTitle.Text
        End If '标题
        If TxtboxAuthor.Text = "" Then
            _artwork.Author = "无名"
        Else
            _artwork.Author = TxtboxAuthor.Text
        End If '作者
        If TxtboxCharacters.Text = "" Then
            _artwork.Characters = Array.Empty(Of String)()
        Else
            _artwork.Characters = TxtboxCharacters.Text.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        End If '角色数组
        If TxtboxTags.Text = "" Then
            _artwork.Tags = Array.Empty(Of String)()
        Else
            _artwork.Tags = TxtboxTags.Text.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        End If '标签数组
        _artwork.Notes = TxtboxNotes.Text '备注
        Dim createTime As DateTime
        If TxtboxCharacters.Text = "" Then createTime = DateTime.Parse("1970-01-01 00:00:00")
        '验证创建时间格式
        If Not DateTime.TryParse(CleanInvisibleCharacters(TxtboxCreateTime.Text), createTime) Then
            MessageBox.Show("创作时间格式不正确！请使用 yyyy-MM-dd HH:mm:ss 格式", "Furry Art Studio",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtboxCreateTime.Focus()
            Return
        End If
        '验证创建时间不能晚于当前时间
        If createTime > DateTime.Now Then
            MessageBox.Show("创作时间不能晚于当前时间！", "Furry Art Studio",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtboxCreateTime.Focus()
            Return
        End If
        _artwork.CreateTime = createTime
        _artwork.UpdateTime = Now
        _artwork.ImportTime = Now
        If Not CreateArtworkFolder() Then Return '创建/更新文件夹
        Dim folderPath As String = Path.Combine(_libraryPath, _artwork.UUID.ToString())
        If _filePaths.Length > 0 Then
            Try
                For Each filepath In _filePaths
                    Dim fileName = Path.GetFileName(filepath)
                    File.Copy(filepath, Path.Combine(folderPath, fileName))
                Next
                CreateThumb() '创建缩略图
            Catch ex As Exception

            End Try
        End If
        '设置DialogResult为OK, 关闭窗体
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
    Private Sub PreviewPicturebox_Click(sender As Object, e As EventArgs) Handles PreviewPicturebox.Click
        AddPictures()
    End Sub
    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles BtnAdd.Click
        AddPictures()
    End Sub
    Private Sub AddPictures()
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "图片文件(*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件(*.*)|*.*"
            openFileDialog.Title = "选择要添加的图片"
            openFileDialog.Multiselect = True
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Try
                    _filePaths = openFileDialog.FileNames
                    PreviewPicturebox.Image = LoadImageFromFile(openFileDialog.FileName) '设置预览图
                    PreviewPicturebox.SizeMode = PictureBoxSizeMode.Zoom
                Catch ex As Exception
                    '不处理
                End Try
            End If
        End Using
    End Sub
    ''' <summary>
    ''' 清理字符串中的不可见字符
    ''' </summary>
    Private Function CleanInvisibleCharacters(input As String) As String
        If String.IsNullOrEmpty(input) Then Return input
        '移除各种不可见字符
        Dim result As New System.Text.StringBuilder()
        For Each c As Char In input
            '判断是否是可打印字符
            If Not Char.IsControl(c) AndAlso
               c <> ChrW(&H200B) AndAlso ' 零宽空格
               c <> ChrW(&H200C) AndAlso ' 零宽非连接符
               c <> ChrW(&H200D) AndAlso ' 零宽连接符
               c <> ChrW(&H200E) AndAlso ' 左到右标记
               c <> ChrW(&H200F) AndAlso ' 右到左标记
               c <> ChrW(&H202A) AndAlso ' 从左到右嵌入
               c <> ChrW(&H202B) AndAlso ' 从右到左嵌入
               c <> ChrW(&H202C) AndAlso ' 弹出方向格式
               c <> ChrW(&H202D) AndAlso ' 从左到右覆盖
               c <> ChrW(&H202E) AndAlso ' 从右到左覆盖
               c <> ChrW(&H2060) AndAlso ' 单词连接符
               c <> ChrW(&H2066) AndAlso ' 从左到右隔离
               c <> ChrW(&H2067) AndAlso ' 从右到左隔离
               c <> ChrW(&H2068) AndAlso ' 第一强隔离
               c <> ChrW(&H2069) Then   ' 弹出隔离
                result.Append(c)
            End If
        Next
        Return result.ToString().Trim()
    End Function

#End Region

End Class