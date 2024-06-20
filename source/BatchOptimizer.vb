Imports System.Text.RegularExpressions

Module BatchOptimizer
    Function RemoveBatchComments(source As String) As String
        source = Regex.Replace(source, "^\s*::.*$", "", RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        source = Regex.Replace(source, "^\s*REM\s+.*$", "", RegexOptions.IgnoreCase Or RegexOptions.Multiline)

        Return source
    End Function

    Function TrimBatchUnnecessaryCharacters(source As String) As String
        source = Regex.Replace(source, "^\s+", "", RegexOptions.Multiline)
        source = Regex.Replace(source, "\r?\n\r?\n+", "\r\n", RegexOptions.Multiline)

        Return source
    End Function
End Module
