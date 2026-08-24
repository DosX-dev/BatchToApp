Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

Module Obfuscator
    Private ReadOnly rnd As New Random()
    Private ReadOnly labelDefinitionRegex As New Regex("^(?<prefix>[ \t]*:[ \t]*)(?<label>.*?)(?<suffix>[ \t]*)$", RegexOptions.Compiled)
    Private ReadOnly referenceCommandRegex As New Regex("(?<![A-Za-z0-9_])(?<command>goto|call)(?![A-Za-z0-9_])", RegexOptions.IgnoreCase Or RegexOptions.Compiled)

    Private Class BatchLine
        Public Text As String
        Public LineEnding As String

        Public Sub New(text As String, lineEnding As String)
            Me.Text = text
            Me.LineEnding = lineEnding
        End Sub
    End Class

    Private Class LabelReference
        Public Command As String
        Public Target As String
        Public TargetStart As Integer
        Public TargetLength As Integer
    End Class

    Function ObfuscateBatchLabels(source As String, addFake As Boolean) As String
        If String.IsNullOrEmpty(source) Then Return source

        Dim lines As List(Of BatchLine) = SplitSourceLines(source)
        Dim labels As New List(Of String)()
        Dim knownLabels As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each line As BatchLine In lines
            Dim labelName As String = Nothing
            If TryGetLabelDefinition(line.Text, labelName) AndAlso knownLabels.Add(labelName) Then
                labels.Add(labelName)
            End If
        Next

        If labels.Count = 0 Then
            Return If(addFake, AddFakeLabels(source), source)
        End If

        ' A computed GOTO/CALL cannot be safely renamed without knowing its value at
        ' run time. Keep only the labels that the expression can address; all other
        ' labels are still mutated normally.
        Dim protectedLabels As HashSet(Of String) = FindDynamicallyReferencedLabels(lines, labels)
        Dim usedNames As New HashSet(Of String)(labels, StringComparer.OrdinalIgnoreCase)
        Dim labelMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        For Each labelName As String In labels
            If Not labelName.Equals("eof", StringComparison.OrdinalIgnoreCase) AndAlso
               Not protectedLabels.Contains(labelName) Then
                labelMap(labelName) = GetUniqueObfuscatedName(usedNames)
            End If
        Next

        Dim noiseVariable As String = GetNoiseVariableName(source)
        Dim newLine As String = GetPreferredNewLine(lines)

        For Each line As BatchLine In lines
            Dim labelName As String = Nothing
            If TryGetLabelDefinition(line.Text, labelName) AndAlso labelMap.ContainsKey(labelName) Then
                Dim definitionMatch As Match = labelDefinitionRegex.Match(line.Text)
                line.Text = definitionMatch.Groups("prefix").Value & labelMap(labelName) & definitionMatch.Groups("suffix").Value
            ElseIf Not IsLabelLine(line.Text) Then
                line.Text = RewriteLabelReferences(line.Text, labelMap, noiseVariable, newLine)
            End If
        Next

        Dim obfuscatedSource As String = JoinSourceLines(lines)
        Return If(addFake, AddFakeLabels(obfuscatedSource), obfuscatedSource)
    End Function

    Function GetObfuscatedName() As String
        Return "label_0x" & Guid.NewGuid().ToString("N").Substring(0, 8)
    End Function

    Private Function GetUniqueObfuscatedName(usedNames As HashSet(Of String)) As String
        Dim candidate As String
        Do
            candidate = GetObfuscatedName()
        Loop While Not usedNames.Add(candidate)
        Return candidate
    End Function

    Private Function FindDynamicallyReferencedLabels(lines As IEnumerable(Of BatchLine), labels As IEnumerable(Of String)) As HashSet(Of String)
        Dim result As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each line As BatchLine In lines
            For Each reference As LabelReference In FindLabelReferences(line.Text)
                Dim target As String = DecodeCaretEscapes(reference.Target)
                If IsDynamicTarget(target) Then
                    For Each labelName As String In labels
                        If DynamicTargetCanAddress(target, labelName) Then result.Add(labelName)
                    Next
                End If
            Next
        Next

        Return result
    End Function

    Private Function RewriteLabelReferences(line As String,
                                            labelMap As Dictionary(Of String, String),
                                            noiseVariable As String,
                                            newLine As String) As String
        Dim references As List(Of LabelReference) = FindLabelReferences(line)

        For i As Integer = references.Count - 1 To 0 Step -1
            Dim reference As LabelReference = references(i)
            Dim decodedTarget As String = DecodeCaretEscapes(reference.Target)
            Dim replacement As String = Nothing

            If labelMap.TryGetValue(decodedTarget, replacement) Then
                If reference.Command.Equals("goto", StringComparison.OrdinalIgnoreCase) Then
                    replacement = ObfuscateString(replacement, noiseVariable).Replace("^", "^" & newLine)
                End If

                line = line.Remove(reference.TargetStart, reference.TargetLength).
                            Insert(reference.TargetStart, replacement)
            End If
        Next

        Return line
    End Function

    Private Function FindLabelReferences(line As String) As List(Of LabelReference)
        Dim result As New List(Of LabelReference)()

        For Each commandMatch As Match In referenceCommandRegex.Matches(line)
            If IsInsideDoubleQuotes(line, commandMatch.Index) OrElse
               Not IsExecutableCommandAt(line, commandMatch.Index) Then Continue For

            Dim reference As LabelReference = ParseLabelReference(line, commandMatch)
            If reference IsNot Nothing AndAlso reference.TargetLength > 0 Then result.Add(reference)
        Next

        Return result
    End Function

    Private Function ParseLabelReference(line As String, commandMatch As Match) As LabelReference
        Dim command As String = commandMatch.Groups("command").Value
        Dim position As Integer = SkipSpaces(line, commandMatch.Index + commandMatch.Length)

        If command.Equals("call", StringComparison.OrdinalIgnoreCase) Then
            ' Retain compatibility with the previously accepted CALL (:label form.
            If position < line.Length AndAlso line(position) = "("c Then position = SkipSpaces(line, position + 1)
            If position >= line.Length OrElse line(position) <> ":"c Then Return Nothing
            position = SkipSpaces(line, position + 1)

            Dim targetEnd As Integer = position
            While targetEnd < line.Length AndAlso
                  Not Char.IsWhiteSpace(line(targetEnd)) AndAlso
                  Not IsReferenceSeparator(line(targetEnd))
                If line(targetEnd) = "^"c AndAlso targetEnd + 1 < line.Length Then targetEnd += 1
                targetEnd += 1
            End While

            Return New LabelReference With {
                .Command = command,
                .Target = line.Substring(position, targetEnd - position),
                .TargetStart = position,
                .TargetLength = targetEnd - position
            }
        End If

        If position < line.Length AndAlso line(position) = ":"c Then position = SkipSpaces(line, position + 1)

        Dim gotoEnd As Integer = position
        While gotoEnd < line.Length AndAlso Not IsReferenceSeparator(line(gotoEnd))
            If line(gotoEnd) = "^"c AndAlso gotoEnd + 1 < line.Length Then gotoEnd += 1
            gotoEnd += 1
        End While

        While gotoEnd > position AndAlso Char.IsWhiteSpace(line(gotoEnd - 1))
            gotoEnd -= 1
        End While

        Return New LabelReference With {
            .Command = command,
            .Target = line.Substring(position, gotoEnd - position),
            .TargetStart = position,
            .TargetLength = gotoEnd - position
        }
    End Function

    Private Function IsReferenceSeparator(value As Char) As Boolean
        Return value = "&"c OrElse value = "|"c OrElse value = "<"c OrElse value = ">"c OrElse
               value = "("c OrElse value = ")"c OrElse value = ";"c OrElse value = "="c
    End Function

    Private Function IsExecutableCommandAt(line As String, commandIndex As Integer) As Boolean
        Dim segmentStart As Integer = FindCommandSegmentStart(line, commandIndex)
        Return IsExecutableCommandAt(line, segmentStart, commandIndex)
    End Function

    Private Function IsExecutableCommandAt(line As String, segmentStart As Integer, commandIndex As Integer) As Boolean
        Dim position As Integer = SkipCommandPrefix(line, segmentStart, commandIndex)
        If position = commandIndex Then Return True
        If position > commandIndex Then Return False

        Dim tokenEnd As Integer = position
        Dim token As String = ReadBatchToken(line, tokenEnd, commandIndex)
        If token.Length = 0 Then Return False

        If token.Equals("if", StringComparison.OrdinalIgnoreCase) Then
            Dim actionStart As Integer = FindIfActionStart(line, tokenEnd, commandIndex)
            Return actionStart >= 0 AndAlso IsExecutableCommandAt(line, actionStart, commandIndex)
        End If

        If token.Equals("for", StringComparison.OrdinalIgnoreCase) Then
            Dim actionStart As Integer = FindForActionStart(line, tokenEnd, commandIndex)
            Return actionStart >= 0 AndAlso IsExecutableCommandAt(line, actionStart, commandIndex)
        End If

        If token.Equals("else", StringComparison.OrdinalIgnoreCase) Then
            Return IsExecutableCommandAt(line, tokenEnd, commandIndex)
        End If

        Return False
    End Function

    Private Function FindCommandSegmentStart(line As String, commandIndex As Integer) As Integer
        Dim inQuotes As Boolean = False
        Dim segmentStart As Integer = 0
        Dim i As Integer = 0

        While i < commandIndex
            Dim current As Char = line(i)
            If current = "^"c AndAlso i + 1 < commandIndex Then
                i += 2
                Continue While
            End If

            If current = """"c Then
                inQuotes = Not inQuotes
            ElseIf Not inQuotes AndAlso (current = "&"c OrElse current = "|"c) Then
                segmentStart = i + 1
            End If
            i += 1
        End While

        Return segmentStart
    End Function

    Private Function SkipCommandPrefix(line As String, start As Integer, limit As Integer) As Integer
        Dim position As Integer = start
        While position < limit
            Dim current As Char = line(position)
            If Char.IsWhiteSpace(current) OrElse current = "@"c OrElse current = "("c OrElse current = ")"c Then
                position += 1
            Else
                Exit While
            End If
        End While
        Return position
    End Function

    Private Function FindIfActionStart(line As String, start As Integer, limit As Integer) As Integer
        Dim position As Integer = SkipSpaces(line, start)
        Dim tokenEnd As Integer
        Dim token As String

        Do
            tokenEnd = position
            token = ReadBatchToken(line, tokenEnd, limit)
            If token.Equals("/i", StringComparison.OrdinalIgnoreCase) OrElse
               token.Equals("not", StringComparison.OrdinalIgnoreCase) Then
                position = SkipSpaces(line, tokenEnd)
            Else
                Exit Do
            End If
        Loop

        If token.Length = 0 Then Return -1
        position = tokenEnd

        If token.Equals("errorlevel", StringComparison.OrdinalIgnoreCase) OrElse
           token.Equals("cmdextversion", StringComparison.OrdinalIgnoreCase) OrElse
           token.Equals("defined", StringComparison.OrdinalIgnoreCase) OrElse
           token.Equals("exist", StringComparison.OrdinalIgnoreCase) Then
            tokenEnd = SkipSpaces(line, position)
            If ReadBatchToken(line, tokenEnd, limit).Length = 0 Then Return -1
            Return SkipSpaces(line, tokenEnd)
        End If

        If token.IndexOf("==", StringComparison.Ordinal) >= 0 Then Return SkipSpaces(line, position)

        tokenEnd = SkipSpaces(line, position)
        Dim comparison As String = ReadBatchToken(line, tokenEnd, limit)
        If comparison.Length = 0 Then Return -1

        If comparison.StartsWith("==", StringComparison.Ordinal) AndAlso comparison.Length > 2 Then
            Return SkipSpaces(line, tokenEnd)
        End If

        If comparison.Equals("==", StringComparison.Ordinal) OrElse IsIfComparisonOperator(comparison) Then
            tokenEnd = SkipSpaces(line, tokenEnd)
            If ReadBatchToken(line, tokenEnd, limit).Length = 0 Then Return -1
            Return SkipSpaces(line, tokenEnd)
        End If

        Return -1
    End Function

    Private Function IsIfComparisonOperator(value As String) As Boolean
        Select Case value.ToLowerInvariant()
            Case "equ", "neq", "lss", "leq", "gtr", "geq"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Function FindForActionStart(line As String, start As Integer, limit As Integer) As Integer
        Dim inQuotes As Boolean = False
        Dim parenthesesDepth As Integer = 0
        Dim i As Integer = start

        While i < limit
            Dim current As Char = line(i)
            If current = "^"c AndAlso i + 1 < limit Then
                i += 2
                Continue While
            End If

            If current = """"c Then
                inQuotes = Not inQuotes
            ElseIf Not inQuotes Then
                If current = "("c Then
                    parenthesesDepth += 1
                ElseIf current = ")"c AndAlso parenthesesDepth > 0 Then
                    parenthesesDepth -= 1
                ElseIf parenthesesDepth = 0 AndAlso IsWordAt(line, i, "do", limit) Then
                    Return SkipSpaces(line, i + 2)
                End If
            End If
            i += 1
        End While

        Return -1
    End Function

    Private Function IsWordAt(line As String, position As Integer, word As String, limit As Integer) As Boolean
        If position + word.Length > limit OrElse
           Not line.Substring(position, word.Length).Equals(word, StringComparison.OrdinalIgnoreCase) Then Return False

        Dim beforeIsWord As Boolean = position > 0 AndAlso (Char.IsLetterOrDigit(line(position - 1)) OrElse line(position - 1) = "_"c)
        Dim afterPosition As Integer = position + word.Length
        Dim afterIsWord As Boolean = afterPosition < line.Length AndAlso
                                      (Char.IsLetterOrDigit(line(afterPosition)) OrElse line(afterPosition) = "_"c)
        Return Not beforeIsWord AndAlso Not afterIsWord
    End Function

    Private Function ReadBatchToken(line As String, ByRef position As Integer, limit As Integer) As String
        position = SkipSpaces(line, position)
        Dim start As Integer = position
        Dim inQuotes As Boolean = False

        While position < limit
            Dim current As Char = line(position)
            If current = "^"c AndAlso position + 1 < limit Then
                position += 2
                Continue While
            End If

            If current = """"c Then
                inQuotes = Not inQuotes
            ElseIf Not inQuotes AndAlso Char.IsWhiteSpace(current) Then
                Exit While
            End If
            position += 1
        End While

        Return line.Substring(start, position - start)
    End Function

    Private Function SkipSpaces(value As String, position As Integer) As Integer
        While position < value.Length AndAlso (value(position) = " "c OrElse value(position) = ControlChars.Tab)
            position += 1
        End While
        Return position
    End Function

    Private Function IsInsideDoubleQuotes(value As String, position As Integer) As Boolean
        Dim inQuotes As Boolean = False
        Dim i As Integer = 0

        While i < position
            If value(i) = "^"c AndAlso i + 1 < position Then
                i += 2
                Continue While
            End If
            If value(i) = """"c Then inQuotes = Not inQuotes
            i += 1
        End While

        Return inQuotes
    End Function

    Private Function DecodeCaretEscapes(value As String) As String
        Dim result As New StringBuilder(value.Length)
        Dim i As Integer = 0

        While i < value.Length
            If value(i) = "^"c AndAlso i + 1 < value.Length Then i += 1
            result.Append(value(i))
            i += 1
        End While

        Return result.ToString()
    End Function

    Private Function IsDynamicTarget(target As String) As Boolean
        Return target.IndexOf("%"c) >= 0 OrElse target.IndexOf("!"c) >= 0
    End Function

    Private Function DynamicTargetCanAddress(target As String, labelName As String) As Boolean
        Dim firstExpansion As Integer = target.IndexOfAny(New Char() {"%"c, "!"c})
        Dim lastExpansion As Integer = target.LastIndexOfAny(New Char() {"%"c, "!"c})
        If firstExpansion < 0 Then Return target.Equals(labelName, StringComparison.OrdinalIgnoreCase)

        Dim prefix As String = target.Substring(0, firstExpansion)
        Dim suffix As String = If(lastExpansion + 1 < target.Length, target.Substring(lastExpansion + 1), String.Empty)
        Return labelName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) AndAlso
               labelName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) AndAlso
               labelName.Length >= prefix.Length + suffix.Length
    End Function

    Private Function TryGetLabelDefinition(line As String, ByRef labelName As String) As Boolean
        Dim definitionMatch As Match = labelDefinitionRegex.Match(line)
        If Not definitionMatch.Success Then Return False

        Dim candidate As String = definitionMatch.Groups("label").Value.Trim()
        If candidate.Length = 0 OrElse candidate.StartsWith(":", StringComparison.Ordinal) Then Return False

        labelName = candidate
        Return True
    End Function

    Private Function IsLabelLine(line As String) As Boolean
        Dim ignored As String = Nothing
        Return TryGetLabelDefinition(line, ignored) OrElse line.TrimStart().StartsWith("::", StringComparison.Ordinal)
    End Function

    Private Function AddFakeLabels(source As String) As String
        If String.IsNullOrEmpty(source) Then Return source

        Dim lines As List(Of BatchLine) = SplitSourceLines(source)
        Dim newLine As String = GetPreferredNewLine(lines)
        Dim usedNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each line As BatchLine In lines
            Dim labelName As String = Nothing
            If TryGetLabelDefinition(line.Text, labelName) Then usedNames.Add(labelName)
        Next

        Dim output As New StringBuilder(source.Length)
        Dim prefix As New StringBuilder()
        Dim pendingFakeLabels As Integer = 0
        Dim parenthesesDepth As Integer = 0
        Dim inQuotes As Boolean = False

        For Each line As BatchLine In lines
            output.Append(line.Text).Append(line.LineEnding)

            If Not String.IsNullOrWhiteSpace(line.Text) AndAlso Not IsLabelLine(line.Text) Then
                pendingFakeLabels += NextRandom(2, 4)
            End If

            Dim continues As Boolean = EndsWithUnescapedCaret(line.Text)
            UpdateParenthesesDepth(line.Text, parenthesesDepth, inQuotes)

            If Not continues Then inQuotes = False
            If pendingFakeLabels > 0 AndAlso Not continues AndAlso parenthesesDepth = 0 Then
                AppendFakeLabels(output, pendingFakeLabels, usedNames, newLine)
                pendingFakeLabels = 0
            End If
        Next

        ' An unmatched literal parenthesis can make the conservative depth tracker
        ' postpone insertion even though cmd treats it as data (for example ECHO ().
        ' Put such labels before the script, which is always a command boundary.
        If pendingFakeLabels > 0 Then AppendFakeLabels(prefix, pendingFakeLabels, usedNames, newLine)
        Return prefix.ToString() & output.ToString()
    End Function

    Private Sub AppendFakeLabels(output As StringBuilder,
                                 count As Integer,
                                 usedNames As HashSet(Of String),
                                 newLine As String)
        If output.Length > 0 AndAlso output(output.Length - 1) <> ControlChars.Cr AndAlso
           output(output.Length - 1) <> ControlChars.Lf Then output.Append(newLine)

        For i As Integer = 1 To count
            output.Append(":"c).Append(GetUniqueObfuscatedName(usedNames)).Append(newLine)
        Next
    End Sub

    Private Function EndsWithUnescapedCaret(line As String) As Boolean
        If line.Length = 0 OrElse line(line.Length - 1) <> "^"c Then Return False

        Dim caretCount As Integer = 0
        Dim position As Integer = line.Length - 1
        While position >= 0 AndAlso line(position) = "^"c
            caretCount += 1
            position -= 1
        End While
        Return caretCount Mod 2 = 1
    End Function

    Private Sub UpdateParenthesesDepth(line As String, ByRef depth As Integer, ByRef inQuotes As Boolean)
        Dim i As Integer = 0
        While i < line.Length
            Dim current As Char = line(i)
            If current = "^"c Then
                i += 2
                Continue While
            End If

            If current = """"c Then
                inQuotes = Not inQuotes
            ElseIf Not inQuotes Then
                If current = "("c Then
                    depth += 1
                ElseIf current = ")"c AndAlso depth > 0 Then
                    depth -= 1
                End If
            End If
            i += 1
        End While
    End Sub

    Public Function ObfuscateBatchCalls(source As String) As String
        If String.IsNullOrEmpty(source) Then Return source

        Dim originalLines As List(Of BatchLine) = SplitSourceLines(source)
        Dim newLine As String = GetPreferredNewLine(originalLines)
        Dim noiseVariable As String = GetNoiseVariableName(source)
        Dim declarationGuardVariable As String = GetUnusedVariableName(source)
        Dim mutationBlockLabel As String = GetObfuscatedName()
        Dim mutationBlock As New StringBuilder()

        mutationBlock.Append("@goto ").Append(ObfuscateString(mutationBlockLabel, noiseVariable)).Append(newLine)
        mutationBlock.Append(GetRandomBinaryJunk(NextRandom(201, 252))).Append(newLine)
        mutationBlock.Append("@exit /b ").Append(NextRandom(1, 256)).Append(newLine)
        mutationBlock.Append(":"c).Append(mutationBlockLabel).Append(newLine).Append(newLine)

        Dim closingBracketVar As String = "_" & GetObfuscatedOperatorName()
        mutationBlock.Append("@set ").Append(ObfuscateString(closingBracketVar, noiseVariable)).Append("=^)").Append(newLine)

        source = mutationBlock.ToString() & newLine & source

        Dim operators As String() = {"if", "goto", "call", "for", "start", "dir", "echo", "set", "exit", "pause", "setlocal", "endlocal", "cls", "title",
                             "assoc", "attrib", "break", "cacls", "cd", "chcp", "chdir", "choice", "clip", "color", "comp", "compact", "convert",
                             "copy", "date", "del", "diskcomp", "diskcopy", "doskey", "fc", "find", "findstr", "format", "fsutil", "ftp",
                             "getmac", "hostname", "label", "md", "mkdir", "mode", "more", "move", "net", "netstat", "nslookup", "path",
                             "ping", "popd", "pushd", "rd", "ren", "rename", "replace", "rmdir", "robocopy", "sc", "schtasks", "shutdown",
                             "sort", "subst", "systeminfo", "taskkill", "tasklist", "time", "timeout", "tree", "type", "ver", "verify",
                             "vol", "xcopy", "shift", "bcdedit", "cipher", "cleanmgr", "driverquery", "gpupdate", "ipconfig", "nlsfunc",
                             "openfiles", "pathping", "powercfg", "print", "recover", "relog", "remsvc", "sfc", "shadow", "tzutil", "vssadmin", "wbadmin", "wevtutil",
                             "mklink", "gpresult", "ftype", "bootcfg", "chkdsk", "chkntfs", "cmd", "erase", "help", "icacls", "prompt", "rem", "deltree", "graftabl",
                             "at", "screen", "free", "history", "choice", "beep", "alias", "delay", "timer"}

        Dim operatorMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each op As String In operators
            If Not operatorMap.ContainsKey(op) Then operatorMap(op) = "_" & GetObfuscatedOperatorName()
        Next

        Dim operatorPattern As String = String.Join("|", operatorMap.Keys.OrderByDescending(Function(value) value.Length).Select(Function(value) Regex.Escape(value)))
        Dim operatorRegex As New Regex("^(?<indent>[ \t]*)(?<at>@?)(?<command>" & operatorPattern & ")\b", RegexOptions.IgnoreCase)
        Dim output As New StringBuilder()

        For Each line As BatchLine In SplitSourceLines(source)
            line.Text = operatorRegex.Replace(line.Text,
                Function(match)
                    Return match.Groups("indent").Value & match.Groups("at").Value &
                           "%" & operatorMap(match.Groups("command").Value) & "%"
                End Function,
                1)

            Dim closingBracketVarPattern As String = "%" & closingBracketVar & "%"
            Dim trimmed As String = line.Text.Trim()
            If trimmed = ")" Then
                Dim indentationLength As Integer = line.Text.Length - line.Text.TrimStart().Length
                line.Text = line.Text.Substring(0, indentationLength) & closingBracketVarPattern
            ElseIf Regex.IsMatch(trimmed, "^\)\s*else\s*\($", RegexOptions.IgnoreCase) Then
                line.Text = ") " & ObfuscateString("else", noiseVariable) & " ("
            End If

            output.Append(line.Text).Append(line.LineEnding)
        Next

        Dim operatorDeclarations As New StringBuilder()
        For Each kvp As KeyValuePair(Of String, String) In operatorMap
            If source.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0 Then
                operatorDeclarations.Append("%").Append(declarationGuardVariable).Append("%")
                operatorDeclarations.Append(Space(NextRandom(0, 8))).Append("@").Append(Space(NextRandom(0, 8)))
                operatorDeclarations.Append(ObfuscateString("set", noiseVariable)).Append(" ")
                operatorDeclarations.Append(ObfuscateString(kvp.Value, noiseVariable).Replace("^", "^" & newLine))
                operatorDeclarations.Append("=^").Append(newLine)
                operatorDeclarations.Append(ObfuscateString(kvp.Key, noiseVariable)).Append(newLine)
            End If
        Next

        Return operatorDeclarations.ToString() & newLine & output.ToString()
    End Function

    Private Function ObfuscateString(input As String, noiseVariable As String) As String
        Return String.Join("", input.Select(
            Function(c)
                Dim variedVariableName As String = RandomizeCase(noiseVariable)
                Dim commaWhitespace As String = New String(" "c, NextRandom(0, 4))
                Return "^" & c & "%" & variedVariableName & ":~" & NextRandom(40000, 99999) &
                       "," & commaWhitespace & "-" & NextRandom(40000, 99999) & "%"
            End Function))
    End Function

    Private Function RandomizeCase(value As String) As String
        Dim result As New StringBuilder(value.Length)

        For Each character As Char In value
            If Char.IsLetter(character) AndAlso NextRandom(0, 2) = 0 Then
                result.Append(Char.ToUpperInvariant(character))
            Else
                result.Append(Char.ToLowerInvariant(character))
            End If
        Next

        Return result.ToString()
    End Function

    Private Function GetNoiseVariableName(source As String) As String
        Dim candidates As String() = {"os", "comspec", "systemroot", "windir", "path", "pathext", "temp", "tmp", "username", "computername"}

        For Each candidate As String In candidates
            Dim assignmentPattern As String = "\bset\s+(?:(?:/a|/p)\s+)?[""]?\s*" & Regex.Escape(candidate) & "\s*="
            If Not Regex.IsMatch(source, assignmentPattern, RegexOptions.IgnoreCase) Then Return candidate
        Next

        ' CMDCMDLINE is a dynamic cmd.exe variable when command extensions are on
        ' (the default and also a requirement for CALL :label).
        Return "cmdcmdline"
    End Function

    Private Function GetUnusedVariableName(source As String) As String
        Dim candidate As String
        Do
            candidate = "_" & Guid.NewGuid().ToString("N")
        Loop While source.IndexOf(candidate, StringComparison.OrdinalIgnoreCase) >= 0
        Return candidate
    End Function

    Private Function GetRandomBinaryJunk(length As Integer) As String
        Dim randomBytes(length - 1) As Byte
        SyncLock rnd
            rnd.NextBytes(randomBytes)
        End SyncLock

        ' NUL terminates parts of cmd.exe's label scanner and 0x1A is DOS EOF; either
        ' can hide the mutation block target. Every other byte, including the other
        ' controls and all high bytes, stays random so analyzers still see binary data.
        For i As Integer = 0 To randomBytes.Length - 1
            While randomBytes(i) = &H0 OrElse randomBytes(i) = &H1A
                randomBytes(i) = CByte(NextRandom(0, 256))
            End While
        Next

        ' MainWindow serializes the final script as CP866. Decoding with the same
        ' code page here makes all generated bytes round-trip without replacement.
        Return Encoding.GetEncoding(866).GetString(randomBytes)
    End Function

    Private Function NextRandom(minValue As Integer, maxValue As Integer) As Integer
        SyncLock rnd
            Return rnd.Next(minValue, maxValue)
        End SyncLock
    End Function

    Private Function GetObfuscatedOperatorName() As String
        Return Guid.NewGuid().ToString("N").Substring(0, 8)
    End Function

    Private Function SplitSourceLines(source As String) As List(Of BatchLine)
        Dim result As New List(Of BatchLine)()
        If source.Length = 0 Then Return result

        Dim position As Integer = 0
        While position < source.Length
            Dim lineStart As Integer = position
            While position < source.Length AndAlso source(position) <> ControlChars.Cr AndAlso source(position) <> ControlChars.Lf
                position += 1
            End While

            Dim text As String = source.Substring(lineStart, position - lineStart)
            Dim lineEnding As String = String.Empty
            If position < source.Length Then
                If source(position) = ControlChars.Cr AndAlso position + 1 < source.Length AndAlso source(position + 1) = ControlChars.Lf Then
                    lineEnding = vbCrLf
                    position += 2
                Else
                    lineEnding = source(position).ToString()
                    position += 1
                End If
            End If
            result.Add(New BatchLine(text, lineEnding))
        End While

        Return result
    End Function

    Private Function JoinSourceLines(lines As IEnumerable(Of BatchLine)) As String
        Dim result As New StringBuilder()
        For Each line As BatchLine In lines
            result.Append(line.Text).Append(line.LineEnding)
        Next
        Return result.ToString()
    End Function

    Private Function GetPreferredNewLine(lines As IEnumerable(Of BatchLine)) As String
        For Each line As BatchLine In lines
            If line.LineEnding.Length > 0 Then Return line.LineEnding
        Next
        Return Environment.NewLine
    End Function
End Module
