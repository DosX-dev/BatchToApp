Imports System.CodeDom.Compiler
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.CSharp

Public Class MainWindow

    Private fileSpecified As Boolean = False

    Private Sub btnCompile_Click(sender As Object, e As EventArgs) Handles btnCompile.Click
        Dim batFilePath As String = batPath.Text.Trim()

        If Not fileSpecified Then
            ShowStatus(False, "No Batch file selected!")
            Return
        End If

        If Not File.Exists(batFilePath) Then
            ShowStatus(False, "Batch file not found!")
            Return
        End If

        If Not ConsoleRadioButton.Checked AndAlso Not HiddenRadioButton.Checked Then
            ShowStatus(False, "Select an application type!")
            Return
        End If

        If Not i386RadioButton.Checked AndAlso Not AMD64RadioButton.Checked Then
            ShowStatus(False, "Select an architecture!")
            Return
        End If

        Dim saveFileDialog As New SaveFileDialog With {
            .Filter = "Application (*.exe)|*.exe|Application (*.com)|*.com",
            .Title = "Choose a location to save the compiled .exe file",
            .FileName = IO.Path.GetFileNameWithoutExtension(batFilePath) & ".exe"
        }

        saveFileDialog.OverwritePrompt = False

        ' Show the file selection dialog
        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            Dim exeSavePath As String = saveFileDialog.FileName

            If Not String.IsNullOrWhiteSpace(exeSavePath) Then
                Dim architecture As String = If(i386RadioButton.Checked, "x86", "x64")

                CompileCSharpCode(My.Resources.stub, exeSavePath, architecture, HiddenRadioButton.Checked, batFilePath)
            End If
        End If
        FixBugWithComboBoxFocus()
    End Sub

    Private Sub ShowStatus(isSuccessful As Boolean, labelText As String)
        operationStatusLabel.Text = labelText

        If Not isSuccessful Then
            operationStatusLabel.AlertStyle = BonfireAlertBox.Style._Error
        Else
            operationStatusLabel.AlertStyle = BonfireAlertBox.Style._Success
        End If

        operationStatusLabel.Visible = True
    End Sub

    Private Sub CompileCSharpCode(csharpCode As String, exeSavePath As String, architecture As String, hideWindow As Boolean, batFilePath As String)
        Try
            FixBugWithComboBoxFocus()

            ' Create a C# code provider
            Dim provider As New CSharpCodeProvider()

            ' /* {IS_HIDDEN} */
            csharpCode = csharpCode.Replace("/* {IS_HIDDEN} */", If(hideWindow, "true", "false"))

            ' /* {APP_ID} */
            csharpCode = csharpCode.Replace("/* {APP_ID} */", """" & Guid.NewGuid().ToString("N").Substring(0, 8) & """")

            ' /* {FS_PROTECTION} */
            csharpCode = csharpCode.Replace("/* {FS_PROTECTION} */", If(fsProtectionCheckBox.Checked, "FileAttributes.System", "0x00"))

            ' Compilation parameters
            Dim compileParams As New CompilerParameters With {
                .GenerateExecutable = True,
                .OutputAssembly = exeSavePath
            }

            Dim tmpResCompressedFile As String = Path.Combine(Path.GetTempPath(), "embeddedBatchScript")

            Dim batchSource As String = IO.File.ReadAllText(batFilePath)

            batchSource = My.Resources.headers.Replace(":: {SOURCE}", batchSource)

            If removeCommentsCheckBox.Checked Then
                batchSource = RemoveBatchComments(batchSource)
            End If

            If trimUnnecessaryCharsCheckBox.Checked Then
                batchSource = TrimBatchUnnecessaryCharacters(batchSource)
            End If

            Select Case obfuscationModeComboBox.SelectedIndex
                Case 0
                Case 1
                    batchSource = ObfuscateBatchLabels(batchSource, False)
                Case 2
                    batchSource = ObfuscateBatchLabels(batchSource, True)
            End Select

            If replaceCommandsCheckBox.Checked Then
                batchSource = ObfuscateBatchCalls(batchSource)
            End If

            File.WriteAllBytes(tmpResCompressedFile, Compress(Encoding.GetEncoding(866).GetBytes(batchSource)))

            compileParams.EmbeddedResources.Add(tmpResCompressedFile)

            ' Do not save temporary files after compilation
            compileParams.TempFiles.KeepFiles = False

            ' Add reference to System.dll
            compileParams.ReferencedAssemblies.Add("System.dll")

            ' Set the compiler option to specify the architecture
            compileParams.CompilerOptions = If(architecture = "x86", "/platform:x86", "/platform:x64")

            ' Set the target type to Windows if the window should be hidden
            If hideWindow Then
                compileParams.CompilerOptions &= " /target:winexe"
            End If

            If provider.CompileAssemblyFromSource(compileParams, csharpCode).Errors.Count > 0 Then
                Dim err As CompilerError = provider.CompileAssemblyFromSource(compileParams, csharpCode).Errors(0)
                ' show error in message box
                MessageBox.Show(err.ErrorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ShowStatus(False, "An error occurred!")
            Else
                ShowStatus(True, "File saved as " & Path.GetFileName(exeSavePath))
            End If

            IO.File.Delete(tmpResCompressedFile)
        Catch ex As Exception
            ShowStatus(False, ex.Message)
        End Try
    End Sub

    Private Sub btnSelectBat_batPath_Click(sender As Object, e As EventArgs) Handles btnSelectBat.Click, batPath.Click
        Dim openFileDialog As New OpenFileDialog With {
            .Filter = "Batch Files (*.bat; *.cmd)|*.bat;*.cmd|All Files (*.*)|*.*",
            .Title = "Choose a .bat or .cmd file to compile"
        }

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            OpenFile(openFileDialog.FileName)
        End If

        FixBugWithComboBoxFocus()
    End Sub


    Const gitUrl = "https://github.com/DosX-dev/BatchToApp"
    Private Sub gitLink_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles gitLink.LinkClicked
        Try
            Process.Start(gitUrl)
        Catch ex As Exception
            Process.Start("explorer.exe", gitUrl)
        End Try
    End Sub

    Private Sub batPath_DragDrop(sender As Object, e As DragEventArgs) Handles batPath.DragEnter, Me.DragEnter
        Try
            OpenFile(e.Data.GetData(DataFormats.FileDrop)(0))
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MainWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If File.Exists(Command()) Then
            OpenFile(Command())
        End If

        lblVersion.Text = lblVersion.Text.Replace("%s", GetTrimmedVersion())
        helpRichText.Rtf = My.Resources.help
    End Sub

    Function GetTrimmedVersion()
        Dim Out = Application.ProductVersion
        While Out.EndsWith(".0")
            Out = Out.Substring(0, Out.Length - 2)
        End While
        Return Out
    End Function

    Sub OpenFile(filePath As String)
        batPath.TextAlign = ContentAlignment.MiddleLeft
        batPath.Text = filePath
        fileSpecified = True
    End Sub

    Private Sub FixBugWithComboBoxFocus() Handles Me.Activated, Me.Deactivate, Me.GotFocus
        btnCompile.Focus()
    End Sub

    Private Sub helpRichText_Checks() Handles helpRichText.SelectionChanged, helpRichText.GotFocus
        btnCompile.Focus()
    End Sub

    Private Sub helpRichText_Checks(sender As Object, e As EventArgs) Handles helpRichText.SelectionChanged, helpRichText.GotFocus

    End Sub
End Class
