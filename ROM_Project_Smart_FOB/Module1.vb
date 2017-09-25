Imports System
Imports System.IO

Module Module1
    Public model As String
    Public strname As String = ""
    Public displayModel As String = ""
    Public valueModel As String = ""
    Dim h As String
    Function WriteTextFile(ByVal filePath As String, ByVal textline As String)               'ghi tiep du lieu vao file
        If System.IO.File.Exists(filePath) = True Then
            Dim w As StreamWriter
            w = File.AppendText(filePath)
            w.WriteLine(textline)
            w.Flush()
            w.Close()
        Else
            MsgBox("File Does Not Exist")
            End
        End If
        Return 0
    End Function
    Function ReadTextFile(ByVal filePath As String, ByVal lineNumber As Integer) As String   ' nhap duong dan file va dong can doc
        Using file As New StreamReader(filePath)
            Dim line As String
            ' doc nhung Line trong text file khong can truy nhap'
            For i As Integer = 1 To lineNumber - 1
                If file.ReadLine() Is Nothing Then
                    line = " "
                End If
            Next
            'doc Line trong text file can truy nhap
            line = file.ReadLine()
            ' Succeded!
            Return line
            file.Close()
        End Using
    End Function

    Function CounterlineTextFile(ByVal File_Path As String) As String    ' nhap duong dan file va dong can doc
        Dim counterLine As String = 0
        If System.IO.File.Exists(File_Path) = True Then                              ' xac nhan duong dan ton tai hay khong
            Dim objReader As New System.IO.StreamReader(File_Path)                   ' mo file theo duong dan
            While (objReader.ReadLine <> "")
                counterLine = counterLine + 1                                        ' doc theo tung dong file text
            End While
            objReader.Close()                                                        ' dong file text da mo
        Else
            MsgBox(File_Path & " Not found")
            End
        End If
        Return counterLine
    End Function

    Function Findapplication(ByVal namesoft As String) As String
        Dim p As Process
        Dim i As String
        For Each p In Process.GetProcesses
            h = p.MainWindowTitle.ToString() ' lay tung title cua tung process
            If h.Length <> 0 Then ' chi kiem tra title cua process khac ki tu trang
                i = InStr(h, namesoft, 0) ' kiem tra trong file name ( title ) cua process co chua ki tu setsoftware hay khong
                If i = True Then ' neu co ten co nghia la dung
                    strname = h ' lay ten phan mem bo vao bien strnamesoft de kich hoat
                End If
            End If
        Next
        Return strname


    End Function
    Function findwords(ByVal filepath As String, ByVal words As String) As String
        Dim line As String
        Dim count As Integer = 0
        Using file As New StreamReader(filepath)
            line = file.ReadToEnd
            file.Close()
        End Using
        For i As Integer = 1 To line.Length Step 2
            If InStr(Mid(line, i, words.Length + 1), words, 0) <> 0 Then
                count = count + 1
            End If
        Next
        Return count
    End Function

End Module
