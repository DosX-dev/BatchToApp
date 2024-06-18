<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainWindow
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainWindow))
        Me.operationStatusLabel = New BatchToApp.BonfireAlertBox()
        Me.btnCompile = New BatchToApp.BonfireButton()
        Me.btnSelectBat = New BatchToApp.BonfireButton()
        Me.menuTabs = New BatchToApp.BonfireTabControl()
        Me.settingsTab = New System.Windows.Forms.TabPage()
        Me.menuGroup4 = New BatchToApp.BonfireGroupBox()
        Me.obfuscationModeComboBox = New BatchToApp.BonfireCombo()
        Me.replaceCommandsCheckBox = New BatchToApp.BonfireCheckbox()
        Me.lbl4 = New BatchToApp.BonfireLabelHeader()
        Me.lblHeader2 = New BatchToApp.BonfireLabelHeader()
        Me.lblHeader1 = New BatchToApp.BonfireLabelHeader()
        Me.menuGroup3 = New BatchToApp.BonfireGroupBox()
        Me.trimUnnecessaryCharsCheckBox = New BatchToApp.BonfireCheckbox()
        Me.removeCommentsCheckBox = New BatchToApp.BonfireCheckbox()
        Me.lbl3 = New BatchToApp.BonfireLabelHeader()
        Me.menuGroup2 = New BatchToApp.BonfireGroupBox()
        Me.lbl2 = New BatchToApp.BonfireLabelHeader()
        Me.i386RadioButton = New BatchToApp.BonfireRadioButton()
        Me.AMD64RadioButton = New BatchToApp.BonfireRadioButton()
        Me.menuGroup1 = New BatchToApp.BonfireGroupBox()
        Me.lbl1 = New BatchToApp.BonfireLabelHeader()
        Me.ConsoleRadioButton = New BatchToApp.BonfireRadioButton()
        Me.HiddenRadioButton = New BatchToApp.BonfireRadioButton()
        Me.aboutTab = New System.Windows.Forms.TabPage()
        Me.lblVersion = New BatchToApp.BonfireLabelHeader()
        Me.gitLink = New System.Windows.Forms.LinkLabel()
        Me.lbl6 = New BatchToApp.BonfireLabelHeader()
        Me.lblHeader3 = New BatchToApp.BonfireLabelHeader()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.batPath = New BatchToApp.BonfireLabel()
        Me.menuTabs.SuspendLayout()
        Me.settingsTab.SuspendLayout()
        Me.menuGroup4.SuspendLayout()
        Me.menuGroup3.SuspendLayout()
        Me.menuGroup2.SuspendLayout()
        Me.menuGroup1.SuspendLayout()
        Me.aboutTab.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'operationStatusLabel
        '
        Me.operationStatusLabel.AlertStyle = BatchToApp.BonfireAlertBox.Style._Success
        Me.operationStatusLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.operationStatusLabel.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.operationStatusLabel.Location = New System.Drawing.Point(4, 286)
        Me.operationStatusLabel.Name = "operationStatusLabel"
        Me.operationStatusLabel.Size = New System.Drawing.Size(354, 45)
        Me.operationStatusLabel.TabIndex = 0
        Me.operationStatusLabel.Text = "Message text"
        Me.operationStatusLabel.Visible = False
        '
        'btnCompile
        '
        Me.btnCompile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCompile.ButtonStyle = BatchToApp.BonfireButton.Style.Blue
        Me.btnCompile.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCompile.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.btnCompile.Image = Nothing
        Me.btnCompile.Location = New System.Drawing.Point(364, 286)
        Me.btnCompile.Name = "btnCompile"
        Me.btnCompile.RoundedCorners = False
        Me.btnCompile.Size = New System.Drawing.Size(202, 45)
        Me.btnCompile.TabIndex = 0
        Me.btnCompile.Text = "Build the application"
        '
        'btnSelectBat
        '
        Me.btnSelectBat.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectBat.ButtonStyle = BatchToApp.BonfireButton.Style.Blue
        Me.btnSelectBat.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSelectBat.Font = New System.Drawing.Font("Verdana", 5.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.btnSelectBat.Image = Nothing
        Me.btnSelectBat.Location = New System.Drawing.Point(530, 9)
        Me.btnSelectBat.Name = "btnSelectBat"
        Me.btnSelectBat.RoundedCorners = False
        Me.btnSelectBat.Size = New System.Drawing.Size(34, 21)
        Me.btnSelectBat.TabIndex = 2
        Me.btnSelectBat.Text = "⚫⚫⚫"
        '
        'menuTabs
        '
        Me.menuTabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.menuTabs.Controls.Add(Me.settingsTab)
        Me.menuTabs.Controls.Add(Me.aboutTab)
        Me.menuTabs.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.menuTabs.ItemSize = New System.Drawing.Size(0, 30)
        Me.menuTabs.Location = New System.Drawing.Point(1, 49)
        Me.menuTabs.Margin = New System.Windows.Forms.Padding(6, 4, 6, 4)
        Me.menuTabs.Name = "menuTabs"
        Me.menuTabs.SelectedIndex = 0
        Me.menuTabs.Size = New System.Drawing.Size(573, 228)
        Me.menuTabs.TabIndex = 0
        '
        'settingsTab
        '
        Me.settingsTab.BackColor = System.Drawing.Color.White
        Me.settingsTab.Controls.Add(Me.menuGroup4)
        Me.settingsTab.Controls.Add(Me.lblHeader2)
        Me.settingsTab.Controls.Add(Me.lblHeader1)
        Me.settingsTab.Controls.Add(Me.menuGroup3)
        Me.settingsTab.Controls.Add(Me.menuGroup2)
        Me.settingsTab.Controls.Add(Me.menuGroup1)
        Me.settingsTab.Location = New System.Drawing.Point(4, 34)
        Me.settingsTab.Margin = New System.Windows.Forms.Padding(6, 4, 6, 4)
        Me.settingsTab.Name = "settingsTab"
        Me.settingsTab.Padding = New System.Windows.Forms.Padding(6, 4, 6, 4)
        Me.settingsTab.Size = New System.Drawing.Size(565, 190)
        Me.settingsTab.TabIndex = 0
        Me.settingsTab.Text = "Settings"
        '
        'menuGroup4
        '
        Me.menuGroup4.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.menuGroup4.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.menuGroup4.Controls.Add(Me.obfuscationModeComboBox)
        Me.menuGroup4.Controls.Add(Me.replaceCommandsCheckBox)
        Me.menuGroup4.Controls.Add(Me.lbl4)
        Me.menuGroup4.Location = New System.Drawing.Point(3, 159)
        Me.menuGroup4.Name = "menuGroup4"
        Me.menuGroup4.Size = New System.Drawing.Size(559, 30)
        Me.menuGroup4.TabIndex = 3
        Me.menuGroup4.Text = "BonfireGroupBox2"
        '
        'obfuscationModeComboBox
        '
        Me.obfuscationModeComboBox.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.obfuscationModeComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.obfuscationModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.obfuscationModeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.obfuscationModeComboBox.Font = New System.Drawing.Font("Arial", 8.0!)
        Me.obfuscationModeComboBox.FormattingEnabled = True
        Me.obfuscationModeComboBox.ItemHeight = 20
        Me.obfuscationModeComboBox.Items.AddRange(New Object() {"Do not obfuscate", "Points", "Points + Junks (unsafe)"})
        Me.obfuscationModeComboBox.Location = New System.Drawing.Point(130, 2)
        Me.obfuscationModeComboBox.Name = "obfuscationModeComboBox"
        Me.obfuscationModeComboBox.Size = New System.Drawing.Size(160, 26)
        Me.obfuscationModeComboBox.TabIndex = 1
        '
        'replaceCommandsCheckBox
        '
        Me.replaceCommandsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.replaceCommandsCheckBox.Checked = False
        Me.replaceCommandsCheckBox.Font = New System.Drawing.Font("Arial", 10.0!)
        Me.replaceCommandsCheckBox.Location = New System.Drawing.Point(305, 5)
        Me.replaceCommandsCheckBox.Name = "replaceCommandsCheckBox"
        Me.replaceCommandsCheckBox.Size = New System.Drawing.Size(227, 20)
        Me.replaceCommandsCheckBox.TabIndex = 0
        Me.replaceCommandsCheckBox.Text = "Replace commands"
        '
        'lbl4
        '
        Me.lbl4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lbl4.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lbl4.ForeColor = System.Drawing.Color.Gray
        Me.lbl4.Location = New System.Drawing.Point(30, 3)
        Me.lbl4.Name = "lbl4"
        Me.lbl4.Size = New System.Drawing.Size(94, 23)
        Me.lbl4.TabIndex = 0
        Me.lbl4.Text = "Obfuscation"
        Me.lbl4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblHeader2
        '
        Me.lblHeader2.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHeader2.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblHeader2.ForeColor = System.Drawing.Color.Gray
        Me.lblHeader2.Location = New System.Drawing.Point(10, 107)
        Me.lblHeader2.Name = "lblHeader2"
        Me.lblHeader2.Size = New System.Drawing.Size(547, 17)
        Me.lblHeader2.TabIndex = 0
        Me.lblHeader2.Text = "Script parameters"
        Me.lblHeader2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblHeader1
        '
        Me.lblHeader1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHeader1.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblHeader1.ForeColor = System.Drawing.Color.Gray
        Me.lblHeader1.Location = New System.Drawing.Point(10, 13)
        Me.lblHeader1.Name = "lblHeader1"
        Me.lblHeader1.Size = New System.Drawing.Size(547, 17)
        Me.lblHeader1.TabIndex = 0
        Me.lblHeader1.Text = "Compiler parameters"
        Me.lblHeader1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'menuGroup3
        '
        Me.menuGroup3.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.menuGroup3.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.menuGroup3.Controls.Add(Me.trimUnnecessaryCharsCheckBox)
        Me.menuGroup3.Controls.Add(Me.removeCommentsCheckBox)
        Me.menuGroup3.Controls.Add(Me.lbl3)
        Me.menuGroup3.Location = New System.Drawing.Point(3, 128)
        Me.menuGroup3.Name = "menuGroup3"
        Me.menuGroup3.Size = New System.Drawing.Size(559, 29)
        Me.menuGroup3.TabIndex = 0
        Me.menuGroup3.Text = "BonfireGroupBox2"
        '
        'trimUnnecessaryCharsCheckBox
        '
        Me.trimUnnecessaryCharsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.trimUnnecessaryCharsCheckBox.Checked = True
        Me.trimUnnecessaryCharsCheckBox.Font = New System.Drawing.Font("Arial", 10.0!)
        Me.trimUnnecessaryCharsCheckBox.Location = New System.Drawing.Point(305, 4)
        Me.trimUnnecessaryCharsCheckBox.Name = "trimUnnecessaryCharsCheckBox"
        Me.trimUnnecessaryCharsCheckBox.Size = New System.Drawing.Size(227, 20)
        Me.trimUnnecessaryCharsCheckBox.TabIndex = 0
        Me.trimUnnecessaryCharsCheckBox.Text = "Remove unnecessary characters"
        '
        'removeCommentsCheckBox
        '
        Me.removeCommentsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.removeCommentsCheckBox.Checked = True
        Me.removeCommentsCheckBox.Font = New System.Drawing.Font("Arial", 10.0!)
        Me.removeCommentsCheckBox.Location = New System.Drawing.Point(130, 4)
        Me.removeCommentsCheckBox.Name = "removeCommentsCheckBox"
        Me.removeCommentsCheckBox.Size = New System.Drawing.Size(160, 20)
        Me.removeCommentsCheckBox.TabIndex = 0
        Me.removeCommentsCheckBox.Text = "Remove comments"
        '
        'lbl3
        '
        Me.lbl3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lbl3.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lbl3.ForeColor = System.Drawing.Color.Gray
        Me.lbl3.Location = New System.Drawing.Point(30, 3)
        Me.lbl3.Name = "lbl3"
        Me.lbl3.Size = New System.Drawing.Size(94, 23)
        Me.lbl3.TabIndex = 0
        Me.lbl3.Text = "Trimming"
        Me.lbl3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'menuGroup2
        '
        Me.menuGroup2.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.menuGroup2.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.menuGroup2.Controls.Add(Me.lbl2)
        Me.menuGroup2.Controls.Add(Me.i386RadioButton)
        Me.menuGroup2.Controls.Add(Me.AMD64RadioButton)
        Me.menuGroup2.Location = New System.Drawing.Point(3, 66)
        Me.menuGroup2.Name = "menuGroup2"
        Me.menuGroup2.Size = New System.Drawing.Size(559, 29)
        Me.menuGroup2.TabIndex = 0
        Me.menuGroup2.Text = "BonfireGroupBox2"
        '
        'lbl2
        '
        Me.lbl2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lbl2.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lbl2.ForeColor = System.Drawing.Color.Gray
        Me.lbl2.Location = New System.Drawing.Point(30, 3)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Size = New System.Drawing.Size(94, 23)
        Me.lbl2.TabIndex = 0
        Me.lbl2.Text = "Bitness"
        Me.lbl2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'i386RadioButton
        '
        Me.i386RadioButton.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.i386RadioButton.Checked = True
        Me.i386RadioButton.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.i386RadioButton.Location = New System.Drawing.Point(181, 5)
        Me.i386RadioButton.Name = "i386RadioButton"
        Me.i386RadioButton.Size = New System.Drawing.Size(98, 20)
        Me.i386RadioButton.TabIndex = 0
        Me.i386RadioButton.Text = "32-bit"
        '
        'AMD64RadioButton
        '
        Me.AMD64RadioButton.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.AMD64RadioButton.Checked = False
        Me.AMD64RadioButton.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.AMD64RadioButton.Location = New System.Drawing.Point(285, 5)
        Me.AMD64RadioButton.Name = "AMD64RadioButton"
        Me.AMD64RadioButton.Size = New System.Drawing.Size(98, 20)
        Me.AMD64RadioButton.TabIndex = 0
        Me.AMD64RadioButton.Text = "64-bit"
        '
        'menuGroup1
        '
        Me.menuGroup1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.menuGroup1.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.menuGroup1.Controls.Add(Me.lbl1)
        Me.menuGroup1.Controls.Add(Me.ConsoleRadioButton)
        Me.menuGroup1.Controls.Add(Me.HiddenRadioButton)
        Me.menuGroup1.Location = New System.Drawing.Point(3, 35)
        Me.menuGroup1.Name = "menuGroup1"
        Me.menuGroup1.Size = New System.Drawing.Size(559, 29)
        Me.menuGroup1.TabIndex = 2
        Me.menuGroup1.Text = "BonfireGroupBox1"
        '
        'lbl1
        '
        Me.lbl1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lbl1.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lbl1.ForeColor = System.Drawing.Color.Gray
        Me.lbl1.Location = New System.Drawing.Point(30, 3)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Size = New System.Drawing.Size(94, 23)
        Me.lbl1.TabIndex = 0
        Me.lbl1.Text = "Mode"
        Me.lbl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ConsoleRadioButton
        '
        Me.ConsoleRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.ConsoleRadioButton.Checked = True
        Me.ConsoleRadioButton.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.ConsoleRadioButton.Location = New System.Drawing.Point(181, 5)
        Me.ConsoleRadioButton.Name = "ConsoleRadioButton"
        Me.ConsoleRadioButton.Size = New System.Drawing.Size(98, 20)
        Me.ConsoleRadioButton.TabIndex = 0
        Me.ConsoleRadioButton.Text = "Console"
        '
        'HiddenRadioButton
        '
        Me.HiddenRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.HiddenRadioButton.Checked = False
        Me.HiddenRadioButton.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.HiddenRadioButton.Location = New System.Drawing.Point(285, 5)
        Me.HiddenRadioButton.Name = "HiddenRadioButton"
        Me.HiddenRadioButton.Size = New System.Drawing.Size(98, 20)
        Me.HiddenRadioButton.TabIndex = 0
        Me.HiddenRadioButton.Text = "Hidden"
        '
        'aboutTab
        '
        Me.aboutTab.BackColor = System.Drawing.Color.White
        Me.aboutTab.Controls.Add(Me.lblVersion)
        Me.aboutTab.Controls.Add(Me.gitLink)
        Me.aboutTab.Controls.Add(Me.lbl6)
        Me.aboutTab.Controls.Add(Me.lblHeader3)
        Me.aboutTab.Controls.Add(Me.PictureBox1)
        Me.aboutTab.Location = New System.Drawing.Point(4, 34)
        Me.aboutTab.Margin = New System.Windows.Forms.Padding(6, 4, 6, 4)
        Me.aboutTab.Name = "aboutTab"
        Me.aboutTab.Padding = New System.Windows.Forms.Padding(6, 4, 6, 4)
        Me.aboutTab.Size = New System.Drawing.Size(565, 190)
        Me.aboutTab.TabIndex = 1
        Me.aboutTab.Text = "About"
        '
        'lblVersion
        '
        Me.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblVersion.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblVersion.ForeColor = System.Drawing.Color.Silver
        Me.lblVersion.Location = New System.Drawing.Point(290, 7)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(129, 27)
        Me.lblVersion.TabIndex = 0
        Me.lblVersion.Text = "v%s"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gitLink
        '
        Me.gitLink.ActiveLinkColor = System.Drawing.Color.Teal
        Me.gitLink.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.gitLink.AutoSize = True
        Me.gitLink.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.gitLink.LinkColor = System.Drawing.Color.Teal
        Me.gitLink.Location = New System.Drawing.Point(167, 95)
        Me.gitLink.Name = "gitLink"
        Me.gitLink.Size = New System.Drawing.Size(278, 17)
        Me.gitLink.TabIndex = 0
        Me.gitLink.TabStop = True
        Me.gitLink.Text = "https://github.com/DosX-dev/BatchToApp"
        '
        'lbl6
        '
        Me.lbl6.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbl6.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lbl6.ForeColor = System.Drawing.Color.Gray
        Me.lbl6.Location = New System.Drawing.Point(166, 34)
        Me.lbl6.Name = "lbl6"
        Me.lbl6.Size = New System.Drawing.Size(387, 63)
        Me.lbl6.TabIndex = 0
        Me.lbl6.Text = "An open source program that allows you to pack and obfuscate your Batch scripts (" &
    ".bat/.cmd) into a Windows applications!"
        '
        'lblHeader3
        '
        Me.lblHeader3.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblHeader3.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblHeader3.ForeColor = System.Drawing.Color.Gray
        Me.lblHeader3.Location = New System.Drawing.Point(166, 7)
        Me.lblHeader3.Name = "lblHeader3"
        Me.lblHeader3.Size = New System.Drawing.Size(130, 27)
        Me.lblHeader3.TabIndex = 0
        Me.lblHeader3.Text = "BatchToApp"
        Me.lblHeader3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.PictureBox1.Image = Global.BatchToApp.My.Resources.Resources.logo
        Me.PictureBox1.Location = New System.Drawing.Point(10, 7)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(150, 150)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'batPath
        '
        Me.batPath.AllowDrop = True
        Me.batPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.batPath.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.batPath.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.batPath.ForeColor = System.Drawing.Color.Gray
        Me.batPath.Location = New System.Drawing.Point(12, 9)
        Me.batPath.Name = "batPath"
        Me.batPath.Size = New System.Drawing.Size(512, 21)
        Me.batPath.TabIndex = 0
        Me.batPath.Text = "Select or drag&drop file"
        Me.batPath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.batPath.UseMnemonic = False
        '
        'MainWindow
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(574, 341)
        Me.Controls.Add(Me.operationStatusLabel)
        Me.Controls.Add(Me.btnCompile)
        Me.Controls.Add(Me.btnSelectBat)
        Me.Controls.Add(Me.menuTabs)
        Me.Controls.Add(Me.batPath)
        Me.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(6, 4, 6, 4)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(790, 580)
        Me.MinimumSize = New System.Drawing.Size(590, 380)
        Me.Name = "MainWindow"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "BatchToApp"
        Me.menuTabs.ResumeLayout(False)
        Me.settingsTab.ResumeLayout(False)
        Me.menuGroup4.ResumeLayout(False)
        Me.menuGroup3.ResumeLayout(False)
        Me.menuGroup2.ResumeLayout(False)
        Me.menuGroup1.ResumeLayout(False)
        Me.aboutTab.ResumeLayout(False)
        Me.aboutTab.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents menuTabs As BonfireTabControl
    Friend WithEvents settingsTab As TabPage
    Friend WithEvents aboutTab As TabPage
    Friend WithEvents batPath As BonfireLabel
    Friend WithEvents btnSelectBat As BonfireButton
    Friend WithEvents btnCompile As BonfireButton
    Friend WithEvents lbl1 As BonfireLabelHeader
    Friend WithEvents HiddenRadioButton As BonfireRadioButton
    Friend WithEvents ConsoleRadioButton As BonfireRadioButton
    Friend WithEvents menuGroup1 As BonfireGroupBox
    Friend WithEvents menuGroup2 As BonfireGroupBox
    Friend WithEvents lbl2 As BonfireLabelHeader
    Friend WithEvents i386RadioButton As BonfireRadioButton
    Friend WithEvents AMD64RadioButton As BonfireRadioButton
    Friend WithEvents lblHeader1 As BonfireLabelHeader
    Friend WithEvents lblHeader2 As BonfireLabelHeader
    Friend WithEvents menuGroup3 As BonfireGroupBox
    Friend WithEvents lbl3 As BonfireLabelHeader
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents lblHeader3 As BonfireLabelHeader
    Friend WithEvents operationStatusLabel As BonfireAlertBox
    Friend WithEvents lbl6 As BonfireLabelHeader
    Friend WithEvents gitLink As LinkLabel
    Friend WithEvents removeCommentsCheckBox As BonfireCheckbox
    Friend WithEvents trimUnnecessaryCharsCheckBox As BonfireCheckbox
    Friend WithEvents menuGroup4 As BonfireGroupBox
    Friend WithEvents lbl4 As BonfireLabelHeader
    Friend WithEvents replaceCommandsCheckBox As BonfireCheckbox
    Friend WithEvents obfuscationModeComboBox As BonfireCombo
    Friend WithEvents lblVersion As BonfireLabelHeader
End Class
