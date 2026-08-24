Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Module Program
    Private failures As New List(Of String)()

    Sub Main()
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        RunTest(NameOf(OnlyDefinitionsAndRealReferencesAreMutated), AddressOf OnlyDefinitionsAndRealReferencesAreMutated)
        RunTest(NameOf(ConditionalReferencesAreMutated), AddressOf ConditionalReferencesAreMutated)
        RunTest(NameOf(ComplexAndEofPrefixedLabelsAreMutated), AddressOf ComplexAndEofPrefixedLabelsAreMutated)
        RunTest(NameOf(StateAndLineEndingsAreIsolated), AddressOf StateAndLineEndingsAreIsolated)
        RunTest(NameOf(DynamicTargetsRemainUsable), AddressOf DynamicTargetsRemainUsable)
        RunTest(NameOf(EnvironmentChangesDoNotCorruptTargets), AddressOf EnvironmentChangesDoNotCorruptTargets)
        RunTest(NameOf(NoiseFormattingIsPolymorphicAndValid), AddressOf NoiseFormattingIsPolymorphicAndValid)
        RunTest(NameOf(BinaryJunkIsPreserved), AddressOf BinaryJunkIsPreserved)
        RunTest(NameOf(V2KeepsContinuationsAndBlocksIntact), AddressOf V2KeepsContinuationsAndBlocksIntact)
        RunTest(NameOf(AllObfuscationCombinationsPreserveBehavior), AddressOf AllObfuscationCombinationsPreserveBehavior)

        If failures.Count > 0 Then
            Console.Error.WriteLine(String.Join(Environment.NewLine, failures))
            Environment.ExitCode = 1
        Else
            Console.WriteLine("All obfuscator regression tests passed.")
        End If
    End Sub

    Private Sub RunTest(name As String, test As Action)
        Try
            test()
            Console.WriteLine("PASS " & name)
        Catch ex As Exception
            failures.Add("FAIL " & name & ": " & ex.Message)
        End Try
    End Sub

    Private Sub OnlyDefinitionsAndRealReferencesAreMutated()
        Dim source As String = Lines(
            "@echo off",
            "echo goto worker",
            "echo :worker",
            "set ""text=goto worker""",
            "if ""x""==""x"" echo goto worker",
            "goto worker",
            ":worker",
            "echo reached")

        Dim result As String = ObfuscateBatchLabels(source, False)

        AssertContains(result, "echo goto worker")
        AssertContains(result, "echo :worker")
        AssertContains(result, "set ""text=goto worker""")
        AssertContains(result, "if ""x""==""x"" echo goto worker")
        AssertNotMatches(result, "(?im)^\s*:worker\s*$")
        AssertMatches(result, "(?im)^\s*:label_0x[0-9a-f]{8}\s*$")
    End Sub

    Private Sub ConditionalReferencesAreMutated()
        Dim ifSource As String = Lines(
            "@echo off",
            "if /i ""x""==""X"" goto if-target",
            "echo FAIL",
            ":if-target",
            "echo if-ok")
        Dim forSource As String = Lines(
            "@echo off",
            "for %%G in (one) do goto for-target",
            "echo FAIL",
            ":for-target",
            "echo for-ok")

        AssertEqual(RunBatch(ifSource), RunBatch(ObfuscateBatchLabels(ifSource, False)),
                    "IF GOTO behavior changed")
        AssertEqual(RunBatch(forSource), RunBatch(ObfuscateBatchLabels(forSource, False)),
                    "FOR GOTO behavior changed")
    End Sub

    Private Sub ComplexAndEofPrefixedLabelsAreMutated()
        Dim source As String = Lines(
            "@echo off",
            "call :worker-name argument",
            "goto target with spaces",
            ":worker-name",
            "exit /b",
            ":target with spaces",
            "goto eofHandler",
            ":eofHandler",
            "goto :eof")

        Dim result As String = ObfuscateBatchLabels(source, False)

        AssertNotContains(result, "call :worker-name")
        AssertNotMatches(result, "(?im)^\s*:worker-name\s*$")
        AssertNotMatches(result, "(?im)^\s*:target with spaces\s*$")
        AssertNotMatches(result, "(?im)^\s*:eofHandler\s*$")
        AssertContains(result, "goto :eof")
    End Sub

    Private Sub DynamicTargetsRemainUsable()
        Dim source As String = Lines(
            "@echo off",
            "set ""suffix=one""",
            "goto dynamic-%suffix%",
            ":dynamic-one",
            "echo dynamic",
            "goto static-target",
            ":static-target",
            "echo static")

        Dim result As String = ObfuscateBatchLabels(source, False)

        AssertMatches(result, "(?im)^\s*:dynamic-one\s*$")
        AssertNotMatches(result, "(?im)^\s*:static-target\s*$")
        AssertEqual(RunBatch(source), RunBatch(result), "dynamic GOTO behavior changed")
    End Sub

    Private Sub StateAndLineEndingsAreIsolated()
        ObfuscateBatchLabels(":first" & vbCrLf & "echo first" & vbCrLf, False)

        Dim unrelatedSource As String = "@echo off" & vbCrLf & "goto first" & vbCrLf
        AssertEqual(unrelatedSource, ObfuscateBatchLabels(unrelatedSource, False),
                    "a label map leaked from a previous obfuscation")

        Dim lfSource As String = "@echo off" & vbLf & "goto target" & vbLf & ":target" & vbLf & "echo lf-ok" & vbLf
        Dim lfResult As String = ObfuscateBatchLabels(lfSource, True)
        If lfResult.IndexOf(ControlChars.Cr) >= 0 Then Throw New InvalidOperationException("LF line endings were changed")
        AssertEqual(RunBatch(lfSource), RunBatch(lfResult), "LF-only source behavior changed")
    End Sub

    Private Sub V2KeepsContinuationsAndBlocksIntact()
        Dim source As String = Lines(
            "@echo off",
            "if ""x""==""x"" (",
            "  echo joined^",
            "-line",
            "  goto after-block",
            ")",
            ":after-block",
            "echo done")

        Dim result As String = ObfuscateBatchLabels(source, True)
        Dim physicalLines As String() = Regex.Split(result, "\r\n|\n|\r")

        For i As Integer = 0 To physicalLines.Length - 2
            If EndsWithOddCaret(physicalLines(i)) AndAlso
               Regex.IsMatch(physicalLines(i + 1), "^\s*:label_0x", RegexOptions.IgnoreCase) Then
                Throw New InvalidOperationException("a fake label was inserted into a caret continuation")
            End If
        Next

        Dim blockStart As Integer = Array.FindIndex(physicalLines, Function(line) line.Contains("if ""x""==""x"" ("))
        Dim blockEnd As Integer = Array.FindIndex(physicalLines, blockStart + 1, Function(line) line.Trim() = ")")
        If blockStart < 0 OrElse blockEnd < 0 Then Throw New InvalidOperationException("test block was not preserved")

        For i As Integer = blockStart + 1 To blockEnd - 1
            If Regex.IsMatch(physicalLines(i), "^\s*:label_0x", RegexOptions.IgnoreCase) Then
                Throw New InvalidOperationException("a fake label was inserted inside a parenthesized block")
            End If
        Next

        AssertMatches(result, "(?im)^\s*:label_0x[0-9a-f]{8}\s*$")
        AssertEqual(RunBatch(source), RunBatch(result), "v2 changed block/continuation behavior")
    End Sub

    Private Sub EnvironmentChangesDoNotCorruptTargets()
        Dim source As String = Lines(
            "@echo off",
            "set ""os=""",
            "set ""comspec=""",
            "set ""systemroot=""",
            "set ""windir=""",
            "set ""path=""",
            "set ""pathext=""",
            "set ""temp=""",
            "set ""tmp=""",
            "set ""username=""",
            "set ""computername=""",
            "goto target",
            ":target",
            "echo reached")

        Dim expected As String = RunBatch(source)
        AssertEqual(expected, RunBatch(ObfuscateBatchLabels(source, False)),
                    "environment changes corrupted an obfuscated target")
        AssertEqual(expected, RunBatch(ObfuscateBatchCalls(ObfuscateBatchLabels(source, True))),
                    "environment changes corrupted the combined obfuscation")
    End Sub

    Private Sub NoiseFormattingIsPolymorphicAndValid()
        Dim syntaxProbe As String = Lines(
            "@echo off",
            "echo A%os:~81898,-90584%B",
            "echo C%oS:~81898, -90584%D",
            "echo E%OS:~81898,   -90584%F")
        AssertEqual("AB" & vbLf & "CD" & vbLf & "EF", RunBatch(syntaxProbe),
                    "cmd.exe rejected a casing/spacing variant")

        Dim source As String = Lines("@echo off", "goto target", ":target", "echo reached")
        Dim samples As New StringBuilder()
        For i As Integer = 1 To 12
            samples.Append(ObfuscateBatchLabels(source, False))
        Next

        Dim expressions As MatchCollection = Regex.Matches(
            samples.ToString(),
            "%(?<variable>os):~\d+,(?<spaces> {0,3})-\d+%",
            RegexOptions.IgnoreCase)
        If expressions.Count < 100 Then Throw New InvalidOperationException("too few polymorphic noise expressions were generated")

        Dim variableForms As New HashSet(Of String)(StringComparer.Ordinal)
        Dim spacingForms As New HashSet(Of Integer)()
        For Each expression As Match In expressions
            variableForms.Add(expression.Groups("variable").Value)
            spacingForms.Add(expression.Groups("spaces").Length)
        Next

        If variableForms.Count < 2 Then Throw New InvalidOperationException("noise variable casing was not varied")
        If spacingForms.Count < 2 Then Throw New InvalidOperationException("comma spacing was not varied")
    End Sub

    Private Sub BinaryJunkIsPreserved()
        Dim hasBinaryControl As Boolean = False
        Dim hasExtendedByte As Boolean = False

        For sample As Integer = 1 To 16
            Dim result As String = ObfuscateBatchCalls(Lines("@echo off", "echo reached"))

            For Each character As Char In result
                Dim codePoint As Integer = AscW(character)
                If codePoint = &H0 OrElse codePoint = &H1A Then
                    Throw New InvalidOperationException("binary junk contains a cmd.exe terminator")
                End If
                If codePoint < &H20 AndAlso character <> ControlChars.Cr AndAlso
                   character <> ControlChars.Lf AndAlso character <> ControlChars.Tab Then hasBinaryControl = True
                If codePoint > &H7F Then hasExtendedByte = True
            Next

            AssertMatches(result, "(?im)^@%_[0-9a-f]{8}% /b (?:[1-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\s*$")

            Try
                AssertEqual("reached", RunBatch(result), "binary junk changed script behavior")
            Catch ex As Exception
                Dim gotoMatch As Match = Regex.Match(result, "(?im)^@%_[0-9a-f]{8}% [^\r\n]*(?:\r\n|\n|\r)")
                Dim sentinelMatch As Match = Regex.Match(result, "(?im)^@%_[0-9a-f]{8}% /b (?:[1-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\s*$")
                Dim binaryText As String = result.Substring(gotoMatch.Index + gotoMatch.Length,
                                                            sentinelMatch.Index - gotoMatch.Index - gotoMatch.Length)
                Dim binaryHex As String = BitConverter.ToString(Encoding.GetEncoding(866).GetBytes(binaryText))
                Throw New InvalidOperationException(ex.Message & " BLOB=" & binaryHex)
            End Try
        Next

        If Not hasBinaryControl Then Throw New InvalidOperationException("binary control bytes were removed from junk")
        If Not hasExtendedByte Then Throw New InvalidOperationException("extended binary bytes were removed from junk")
    End Sub

    Private Sub AllObfuscationCombinationsPreserveBehavior()
        Dim source As String = Lines(
            "@echo off",
            "setlocal",
            "echo begin",
            "echo goto worker-name",
            "echo :worker-name",
            "goto :main-label",
            "echo FAIL-1",
            ":worker-name",
            "echo worker-%~1",
            "exit /b",
            ":main-label",
            "call :worker-name ok",
            "if ""x""==""x"" (",
            "  echo block",
            "  echo joined^",
            "-line",
            ")",
            "goto eofHandler",
            "echo FAIL-2",
            ":eofHandler",
            "echo done",
            "goto target with spaces",
            "echo FAIL-3",
            ":target with spaces",
            "echo spaced",
            "goto :eof")

        Dim expected As String = RunBatch(source)
        Dim variants As String() = {
            ObfuscateBatchLabels(source, False),
            ObfuscateBatchLabels(source, True),
            ObfuscateBatchCalls(ObfuscateBatchLabels(source, False)),
            ObfuscateBatchCalls(ObfuscateBatchLabels(source, True))
        }

        For i As Integer = 0 To variants.Length - 1
            AssertEqual(expected, RunBatch(variants(i)), "behavior changed for obfuscation variant " & (i + 1).ToString())
        Next
    End Sub

    Private Function RunBatch(source As String) As String
        Dim tempPath As String = Path.Combine(Path.GetTempPath(), "BatchToApp_" & Guid.NewGuid().ToString("N") & ".cmd")
        File.WriteAllText(tempPath, source, Encoding.GetEncoding(866))

        Try
            Dim startInfo As New ProcessStartInfo With {
                .FileName = Environment.GetEnvironmentVariable("ComSpec"),
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True,
                .WorkingDirectory = Path.GetDirectoryName(tempPath)
            }
            startInfo.ArgumentList.Add("/d")
            startInfo.ArgumentList.Add("/q")
            startInfo.ArgumentList.Add("/c")
            startInfo.ArgumentList.Add(tempPath)

            Using process As Process = Process.Start(startInfo)
                If process Is Nothing Then Throw New InvalidOperationException("cmd.exe did not start")
                Dim standardOutput As String = process.StandardOutput.ReadToEnd()
                Dim standardError As String = process.StandardError.ReadToEnd()
                If Not process.WaitForExit(10000) Then
                    process.Kill(True)
                    Throw New TimeoutException("batch script timed out")
                End If
                If process.ExitCode <> 0 Then
                    Throw New InvalidOperationException("cmd.exe exited with " & process.ExitCode.ToString() & ": " & standardError.Trim())
                End If
                Return NormalizeOutput(standardOutput)
            End Using
        Finally
            File.Delete(tempPath)
        End Try
    End Function

    Private Function Lines(ParamArray values As String()) As String
        Return String.Join(vbCrLf, values) & vbCrLf
    End Function

    Private Function NormalizeOutput(value As String) As String
        Return value.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).TrimEnd(ControlChars.Lf)
    End Function

    Private Function EndsWithOddCaret(value As String) As Boolean
        Dim count As Integer = 0
        For i As Integer = value.Length - 1 To 0 Step -1
            If value(i) <> "^"c Then Exit For
            count += 1
        Next
        Return count Mod 2 = 1
    End Function

    Private Sub AssertContains(value As String, expected As String)
        If value.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0 Then
            Throw New InvalidOperationException("expected text was not found: " & expected)
        End If
    End Sub

    Private Sub AssertNotContains(value As String, unexpected As String)
        If value.IndexOf(unexpected, StringComparison.OrdinalIgnoreCase) >= 0 Then
            Throw New InvalidOperationException("unexpected text was found: " & unexpected)
        End If
    End Sub

    Private Sub AssertMatches(value As String, pattern As String)
        If Not Regex.IsMatch(value, pattern) Then Throw New InvalidOperationException("pattern did not match: " & pattern)
    End Sub

    Private Sub AssertNotMatches(value As String, pattern As String)
        If Regex.IsMatch(value, pattern) Then Throw New InvalidOperationException("unexpected pattern matched: " & pattern)
    End Sub

    Private Sub AssertEqual(expected As String, actual As String, message As String)
        If Not String.Equals(expected, actual, StringComparison.Ordinal) Then
            Throw New InvalidOperationException(message & Environment.NewLine &
                                                "EXPECTED:" & Environment.NewLine & expected & Environment.NewLine &
                                                "ACTUAL:" & Environment.NewLine & actual)
        End If
    End Sub
End Module
