Imports System.Text

Module Obfuscator
    Private rnd As New Random()
    Private labelMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    Function ObfuscateBatchLabels(source As String, addFake As Boolean) As String
        Dim regex As New Text.RegularExpressions.Regex("(?<=^|\s):(\w+)", Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim matches = regex.Matches(source)

        For Each match As Text.RegularExpressions.Match In matches
            Dim originalLabel As String = match.Value.Substring(1)

            If originalLabel.ToLower().StartsWith("eof") Then
                labelMap(originalLabel) = "eof"
            ElseIf Not labelMap.ContainsKey(originalLabel) Then
                labelMap(originalLabel) = GetObfuscatedName()
            End If
        Next

        Dim obfuscatedSource As String = regex.Replace(source, AddressOf ReplaceLabel)
        obfuscatedSource = ReplaceGotoAndCallReferences(obfuscatedSource)

        If addFake Then obfuscatedSource = AddFakeLabels(obfuscatedSource)

        Return obfuscatedSource
    End Function

    Function GetObfuscatedName() As String
        Return "label_0x" & Guid.NewGuid().ToString("N").Substring(0, 8)
    End Function

    Private Function ReplaceLabel(match As Text.RegularExpressions.Match) As String
        Dim labelName As String = labelMap(match.Value.Substring(1))

        Return ":" & labelName
    End Function

    Private Function ReplaceGotoAndCallReferences(source As String) As String
        Dim output As New System.Text.StringBuilder()
        Dim lines() As String = source.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

        For Each line As String In lines
            Dim gotoRegex As New Text.RegularExpressions.Regex("\bgoto\s+(\w+)\b", Text.RegularExpressions.RegexOptions.IgnoreCase)
            line = gotoRegex.Replace(line, AddressOf ReplaceGotoMatch)

            Dim callRegex As New Text.RegularExpressions.Regex("\bcall\s*\(?\s*:(\w+)\s*\)?", Text.RegularExpressions.RegexOptions.IgnoreCase)
            line = callRegex.Replace(line, AddressOf ReplaceCallMatch)

            output.AppendLine(line)
        Next

        Return output.ToString()
    End Function

    Private Function ReplaceGotoMatch(match As Text.RegularExpressions.Match) As String
        If labelMap.ContainsKey(match.Groups(1).Value) Then
            Return "goto " & ObfuscateString(labelMap(match.Groups(1).Value)).Replace("^", "^" & vbLf)
        End If
        Return match.Value
    End Function

    Private Function ReplaceCallMatch(match As Text.RegularExpressions.Match) As String
        If labelMap.ContainsKey(match.Groups(1).Value) Then
            Return "call :" & labelMap(match.Groups(1).Value)
        End If
        Return match.Value
    End Function

    Private Function AddFakeLabels(source As String) As String
        Dim output As New System.Text.StringBuilder()
        Dim lines() As String = source.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

        For Each line As String In lines
            output.AppendLine(line)
            If Not String.IsNullOrWhiteSpace(line) AndAlso Not line.StartsWith(":") Then
                Dim fakeLabelsCount As Integer = rnd.Next(2, 4)
                For i As Integer = 1 To fakeLabelsCount
                    output.AppendLine(":" & GetObfuscatedName())
                Next
            End If
        Next

        Return output.ToString()
    End Function

    Private operatorMap As New Dictionary(Of String, String)
    Public Function ObfuscateBatchCalls(source As String) As String
        Dim mutationBlock As New StringBuilder,
            mutationBlockLabel As String = GetObfuscatedName()

        mutationBlock.AppendLine("@goto " & ObfuscateString(mutationBlockLabel))

        Dim randomBytes(rnd.Next(200, 250)) As Byte
        rnd.NextBytes(randomBytes)
        mutationBlock.AppendLine(Encoding.Default.GetString(randomBytes))

        mutationBlock.AppendLine(":" & mutationBlockLabel & vbLf)

        Dim closingBracketVar As String = "_" & GetObfuscatedOperatorName()
        mutationBlock.AppendLine("@set " & ObfuscateString(closingBracketVar) & "=^)")

        source = mutationBlock.ToString() & vbCrLf & source

        Dim operators As String() = {"if", "goto", "call", "for", "start", "dir", "echo", "set", "exit", "pause", "setlocal", "endlocal", "cls", "title",
                             "assoc", "attrib", "break", "cacls", "cd", "chcp", "chdir", "choice", "clip", "color", "comp", "compact", "convert",
                             "copy", "date", "del", "diskcomp", "diskcopy", "doskey", "fc", "find", "findstr", "format", "fsutil", "ftp",
                             "getmac", "hostname", "label", "md", "mkdir", "mode", "more", "move", "net", "netstat", "nslookup", "path",
                             "ping", "popd", "pushd", "rd", "ren", "rename", "replace", "rmdir", "robocopy", "sc", "schtasks", "shutdown",
                             "sort", "subst", "systeminfo", "taskkill", "tasklist", "time", "timeout", "tree", "type", "ver", "verify",
                             "vol", "xcopy", "shift", "bcdedit", "cipher", "cleanmgr", "driverquery", "gpupdate", "ipconfig", "nlsfunc",
                             "openfiles", "pathping", "powercfg", "print", "recover", "relog", "remsvc", "sfc", "shadow", "shutdown", "sfc",
                             "tzutil", "vssadmin", "wbadmin", "wevtutil"}


        Dim operatorMap As New Dictionary(Of String, String)()

        For Each op As String In operators
            operatorMap(op) = "_" & GetObfuscatedOperatorName()
        Next

        Dim output As New System.Text.StringBuilder()
        Dim lines() As String = source.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

        For Each line As String In lines
            For Each op As String In operators
                Dim regex As New Text.RegularExpressions.Regex("^\s*@?" & op & "\b", Text.RegularExpressions.RegexOptions.IgnoreCase)
                line = regex.Replace(line, Function(m) If(m.Value.StartsWith("@"), "@", String.Empty) & "%" & operatorMap(op) & "%")
            Next

            Dim closingBracketVarPattern As String = "%" & closingBracketVar & "%"

            If line = ")" Then
                line = closingBracketVarPattern
            ElseIf line.ToLower().Replace(" ", String.Empty) = ")else(" Then
                line = ") " & ObfuscateString("else") & " ("
            End If

            output.AppendLine(line)
        Next

        Dim operatorDeclarations As New System.Text.StringBuilder()
        For Each kvp As KeyValuePair(Of String, String) In operatorMap
            If source.ToLower().Contains(kvp.Key) Then
                operatorDeclarations.AppendLine("%__" & Guid.NewGuid().ToString("N").Substring(0, 8) & "%" & Space(rnd.Next(0, 8)) & "@" & Space(rnd.Next(0, 8)) & ObfuscateString("set") & " " & ObfuscateString(kvp.Value).Replace("^", "^" & vbLf) & "=^" & vbLf & ObfuscateString(kvp.Key))
            End If
        Next

        Return operatorDeclarations.ToString() & Environment.NewLine & output.ToString()
    End Function

    Private Function ObfuscateString(input As String) As String
        Return String.Join("", input.Select(Function(c) "^" & c & "%os:~" & rnd.Next(20, 9999) & ", -" & rnd.Next(20, 9999) & "%"))
    End Function

    Private Function GetObfuscatedOperatorName() As String
        Return Guid.NewGuid().ToString("N").Substring(0, 8)
    End Function
End Module
