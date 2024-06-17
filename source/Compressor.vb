Imports System.IO
Imports System.IO.Compression

Module Compressor
    Function Compress(data As Byte()) As Byte()
        Using ms As New MemoryStream()
            Using deflateStream As New DeflateStream(ms, CompressionMode.Compress)
                deflateStream.Write(data, 0, data.Length)
            End Using
            Return ms.ToArray()
        End Using
    End Function
End Module
