<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditDialogForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TxtboxTitle = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtboxAuthor = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PreviewPicturebox = New System.Windows.Forms.PictureBox()
        Me.BtnModify = New System.Windows.Forms.Button()
        Me.LblUUID = New System.Windows.Forms.Label()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.TxtboxCharacters = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtboxTags = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblImportTime = New System.Windows.Forms.Label()
        Me.LblUpdateTime = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtboxCreateTime = New System.Windows.Forms.TextBox()
        Me.TxtboxNotes = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtnAdd = New System.Windows.Forms.Button()
        CType(Me.PreviewPicturebox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TxtboxTitle
        '
        Me.TxtboxTitle.Location = New System.Drawing.Point(63, 12)
        Me.TxtboxTitle.Name = "TxtboxTitle"
        Me.TxtboxTitle.Size = New System.Drawing.Size(258, 25)
        Me.TxtboxTitle.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 15)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "标题:"
        '
        'TxtboxAuthor
        '
        Me.TxtboxAuthor.Location = New System.Drawing.Point(63, 45)
        Me.TxtboxAuthor.Name = "TxtboxAuthor"
        Me.TxtboxAuthor.Size = New System.Drawing.Size(258, 25)
        Me.TxtboxAuthor.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 15)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "作者:"
        '
        'PreviewPicturebox
        '
        Me.PreviewPicturebox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PreviewPicturebox.Location = New System.Drawing.Point(330, 12)
        Me.PreviewPicturebox.Name = "PreviewPicturebox"
        Me.PreviewPicturebox.Size = New System.Drawing.Size(240, 240)
        Me.PreviewPicturebox.TabIndex = 5
        Me.PreviewPicturebox.TabStop = False
        '
        'BtnModify
        '
        Me.BtnModify.Location = New System.Drawing.Point(330, 258)
        Me.BtnModify.Name = "BtnModify"
        Me.BtnModify.Size = New System.Drawing.Size(109, 49)
        Me.BtnModify.TabIndex = 8
        Me.BtnModify.Text = "完成(&O)"
        Me.BtnModify.UseVisualStyleBackColor = True
        '
        'LblUUID
        '
        Me.LblUUID.AutoSize = True
        Me.LblUUID.Location = New System.Drawing.Point(12, 288)
        Me.LblUUID.Name = "LblUUID"
        Me.LblUUID.Size = New System.Drawing.Size(15, 15)
        Me.LblUUID.TabIndex = 7
        Me.LblUUID.Text = " "
        '
        'BtnCancel
        '
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(461, 258)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(109, 49)
        Me.BtnCancel.TabIndex = 9
        Me.BtnCancel.Text = "取消(&C)"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'TxtboxCharacters
        '
        Me.TxtboxCharacters.Location = New System.Drawing.Point(63, 78)
        Me.TxtboxCharacters.Name = "TxtboxCharacters"
        Me.TxtboxCharacters.Size = New System.Drawing.Size(258, 25)
        Me.TxtboxCharacters.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 81)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(45, 15)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "角色:"
        '
        'TxtboxTags
        '
        Me.TxtboxTags.Location = New System.Drawing.Point(63, 111)
        Me.TxtboxTags.Name = "TxtboxTags"
        Me.TxtboxTags.Size = New System.Drawing.Size(258, 25)
        Me.TxtboxTags.TabIndex = 4
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 114)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 15)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "标签:"
        '
        'LblImportTime
        '
        Me.LblImportTime.AutoSize = True
        Me.LblImportTime.Location = New System.Drawing.Point(12, 270)
        Me.LblImportTime.Name = "LblImportTime"
        Me.LblImportTime.Size = New System.Drawing.Size(83, 15)
        Me.LblImportTime.TabIndex = 13
        Me.LblImportTime.Text = "导入时间: "
        '
        'LblUpdateTime
        '
        Me.LblUpdateTime.AutoSize = True
        Me.LblUpdateTime.Location = New System.Drawing.Point(12, 249)
        Me.LblUpdateTime.Name = "LblUpdateTime"
        Me.LblUpdateTime.Size = New System.Drawing.Size(83, 15)
        Me.LblUpdateTime.TabIndex = 14
        Me.LblUpdateTime.Text = "更新时间: "
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 224)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(75, 15)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "创作时间:"
        '
        'TxtboxCreateTime
        '
        Me.TxtboxCreateTime.Location = New System.Drawing.Point(93, 221)
        Me.TxtboxCreateTime.Name = "TxtboxCreateTime"
        Me.TxtboxCreateTime.Size = New System.Drawing.Size(228, 25)
        Me.TxtboxCreateTime.TabIndex = 7
        '
        'TxtboxNotes
        '
        Me.TxtboxNotes.Location = New System.Drawing.Point(63, 144)
        Me.TxtboxNotes.Multiline = True
        Me.TxtboxNotes.Name = "TxtboxNotes"
        Me.TxtboxNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtboxNotes.Size = New System.Drawing.Size(228, 71)
        Me.TxtboxNotes.TabIndex = 5
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 147)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(45, 15)
        Me.Label6.TabIndex = 18
        Me.Label6.Text = "备注:"
        '
        'BtnAdd
        '
        Me.BtnAdd.Location = New System.Drawing.Point(297, 144)
        Me.BtnAdd.Name = "BtnAdd"
        Me.BtnAdd.Size = New System.Drawing.Size(27, 71)
        Me.BtnAdd.TabIndex = 6
        Me.BtnAdd.Text = "+"
        Me.BtnAdd.UseVisualStyleBackColor = True
        '
        'EditDialogForm
        '
        Me.AcceptButton = Me.BtnModify
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(582, 313)
        Me.Controls.Add(Me.BtnAdd)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TxtboxNotes)
        Me.Controls.Add(Me.TxtboxCreateTime)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.LblUpdateTime)
        Me.Controls.Add(Me.LblImportTime)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TxtboxTags)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtboxCharacters)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.LblUUID)
        Me.Controls.Add(Me.BtnModify)
        Me.Controls.Add(Me.PreviewPicturebox)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtboxAuthor)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TxtboxTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EditDialogForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "EditDialogForm"
        CType(Me.PreviewPicturebox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TxtboxTitle As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtboxAuthor As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents PreviewPicturebox As PictureBox
    Friend WithEvents BtnModify As Button
    Friend WithEvents LblUUID As Label
    Friend WithEvents BtnCancel As Button
    Friend WithEvents TxtboxCharacters As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtboxTags As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents LblImportTime As Label
    Friend WithEvents LblUpdateTime As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtboxCreateTime As TextBox
    Friend WithEvents TxtboxNotes As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents BtnAdd As Button
End Class
