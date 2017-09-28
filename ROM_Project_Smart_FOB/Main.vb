Imports System.IO
Imports System.IO.Ports
Imports System.Threading
Public Class Main
#Region "khai bao bien"
    Private setcom As String       ' cai dat cong com
    Dim setlogmachine As String    ' cai dat dia chi folder log ma may o cong doan tu sinh ra
    Dim setlogforwip As String     ' cai dat dia chi luu file log cho he thong wip
    Dim setprocess As String       ' cat dat ten cong doan
    Dim setsoftware As String      ' cai dat ten soft can kich hoat
    Dim setfolder_report As String ' cai dat folder file report
    Dim stringtime As String       ' bien ghep cac gia tri thoi gian
    Dim serial As String           ' ma serial khi no dung tieu chuan
    Dim writeresult As String      ' ghi ket qua vao file log cho he thong wip
    Dim setfilenamereport As String  ' cai dat ten lien quan den file name của file report may chuc nang
    Dim filenamelog As String
    Dim filenameLogC As String 'Thiết lập ổ đĩa lưu log mới dạng txt - quyetpv
    Dim namemodel, namemodel1, namemodel2 As String
    Dim barcode_rule, barcode_rule1, barcode_rule2 As String
    Dim ngay, thang, gio, phut, giay As String
    Dim start As Boolean
    Dim settimer1, settimer2, settimer3 As Integer
    Dim npass As Integer = 0
    Dim nfail As Integer = 0
    Dim ntotal As Integer = 0
    Dim counterlog As Integer = 0
    Dim counterlog_chk As Integer = 0
    Dim labelcheck As Boolean
    'Dim j As Integer
    Dim strnamesoft, linereport As String
    Dim new_filename_report As String
    Dim countread As Integer
    Dim countlinecurrent, countlinebegin As Integer
    Dim length_label As Integer
    Dim retry As Integer
    Dim filename_passrate As String = ""
    Private mySerialPort As New SerialPort ' khai báo sử dụng cổng com
    Dim chk_bacode As Integer
    Private Property filename_sys_setting As String

    '======lien quan den process task manager
    Public Declare Sub mouse_event Lib "user32.dll" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
    Dim h As String
    Dim i, x As Boolean
#End Region

    Private Sub Control_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'Comment: dong cong com & thuc hien thao tac khi dong form chinh
        If mySerialPort.IsOpen = True Then
            mySerialPort.Write("E")     ' tat role truoc khi dong cong com
            mySerialPort.Close()        ' dong cong com
        End If
        'Try
        '    File.Delete(setlogmachine & "\" & Now.ToString("yyyy-MM-dd_bk.") & setfilenamereport)
        'Catch ex As Exception
        'End Try
        'Process.GetProcesses().ToList.ForEach(
        '    Sub(r)
        '        If r.ProcessName.Contains("FLASH") Then r.Kill()
        '    End Sub
        ')
        Application.Exit()

    End Sub
    Private Sub setup_display()
        Labelresult.BackColor = Color.Blue
        Labelresult.ForeColor = Color.White
        Labelresult.Text = "Wait"
        textserial.Enabled = True
        textserial.Text = ""
        textserial.Focus()
    End Sub

    Private Sub Control_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        loadsetting() ' load thong tin cai dat
        passpercent() ' ghi thông tin pass
        setup_display() 'Cài đặt hiển thị

        '  mySerialPort.Write("F")     '????
        For j As Integer = 1 To 100
            Datagrid.Rows.Add()
        Next
        Datagrid.Enabled = False
    End Sub
    '-------------------------------------
    Private Sub CommPortSetup()
        With mySerialPort
            .PortName = setcom
            .BaudRate = 9600
            .DataBits = 8
            .Parity = Parity.None
            .StopBits = StopBits.One
            .Handshake = Handshake.None
        End With
        Try
            mySerialPort.Open()
        Catch ex As Exception
            MsgBox(setcom & " khong ket noi duoc !")
            End
        End Try

    End Sub
    Private Sub WritePassTextfile()
        npass = 0
        nfail = 0
        ntotal = 0
        filename_passrate = "Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt"
        Dim text_passrate As New System.IO.StreamWriter(filename_passrate)
        text_passrate.WriteLine("# Total")
        text_passrate.WriteLine(ntotal)
        text_passrate.WriteLine("# Pass")
        text_passrate.WriteLine(npass)
        text_passrate.WriteLine("# NG")
        text_passrate.WriteLine(nfail)
        text_passrate.WriteLine("# PASSRATE")
        text_passrate.WriteLine(Passrate.Text)
        text_passrate.Close()
    End Sub
    ''' <summary>
    ''' Ghi lại tỉ lệ pass
    ''' </summary>
    Private Sub passpercent()

        ntotal = npass + nfail
        TextTotal.Text = ntotal
        TextPass.Text = npass
        TextNg.Text = nfail
        If ntotal <> 0 Then
            Passrate.Text = "Pass Rate: " & Format((npass / ntotal) * 100, "0.00") & "%"
        Else
            Passrate.Text = "Pass Rate: " & "0%"
        End If
        If File.Exists("Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt") = False Then ' kiem tra xem file text cua lot no nay da duoc tao hay chua
            WritePassTextfile()
        End If
    End Sub
    '===========================================
    '--------------------------------------------------------------------------------------------------------
    Private Sub loadsetting()
        If System.IO.File.Exists("Setup\setting.txt") = True Then
            setcom = ReadTextFile("Setup\setting.txt", 2) ' cai dat cong com
            setlogforwip = ReadTextFile("Setup\setting.txt", 4) ' cai dat dia folder lưu file log cho hệ thống wip
            File.ReadAllLines("Setup\rom.ini").Skip(1).ToList().ForEach(
                Sub(r)
                    If r.Split(",").FirstOrDefault.Trim.Contains(Logon.Model_name.Text) Then
                        setlogmachine = r.Split(",").Skip(3).FirstOrDefault.Trim
                    End If
                End Sub
                   )

            setprocess = ReadTextFile("Setup\setting.txt", 8) ' cai dat ten cong doan
            barcode_rule1 = Mid(ReadTextFile("Setup\setting.txt", 10), 1, 3) ' Cai dat barcode rule model 1
            namemodel1 = Mid(ReadTextFile("Setup\setting.txt", 10), 5, 12) ' cai dat name model code 1
            barcode_rule2 = Mid(ReadTextFile("Setup\setting.txt", 12), 1, 3) ' Cai dat barcode rule model 2
            namemodel2 = Mid(ReadTextFile("Setup\setting.txt", 12), 5, 12) ' cai dat name model code 2
            setsoftware = ReadTextFile("Setup\setting.txt", 14) ' cai dat ten phan mem can kich hoat
            setfilenamereport = ReadTextFile("Setup\setting.txt", 16) ' cai dat ten file log: TMP1.LOG
            settimer1 = ReadTextFile("Setup\setting.txt", 18) ' cai dat thoi gian cho timer 1
            settimer2 = ReadTextFile("Setup\setting.txt", 20) ' cai dat thoi gian cho timer 2
            settimer3 = ReadTextFile("Setup\setting.txt", 22) ' cai dat thoi gian cho timer 3
            length_label = ReadTextFile("Setup\setting.txt", 24) ' doc cai dat do dai cho phep cua barcode
            ' Txt_Retry.Text = ReadTextFile("Setup\setting.txt", 26) ' CAI DAT SO LAN RETRAY
            chk_bacode = ReadTextFile("Setup\setting.txt", 28) ' kiem tra xem co su dung barcose khong
            counterlog_chk = ReadTextFile("Setup\setting.txt", 30)
            Me.Text = setprocess
            If chk_bacode = 1 Then CommPortSetup() ' set up comport 
            ' Tạo thư mục lưu Logfile
            If Directory.Exists("C:\Logprocess") = False Then Directory.CreateDirectory("C:\Logprocess") 'Khai báo thư mục lưu report mới - quyetpv
            If Directory.Exists("Log_Report") = False Then Directory.CreateDirectory("Log_Report")
            If Directory.Exists("Log_Report\" & Now.Year) = False Then Directory.CreateDirectory("Log_Report\" & Now.Year)
            If Directory.Exists("Log_Report\" & Now.ToString("yyyy\\MM")) = False Then Directory.CreateDirectory("Log_Report\" & Now.ToString("yyyy\\MM"))
            If Directory.Exists("Log_Report\" & Now.ToString("yyyy\\MM") & "\OK") = False Then Directory.CreateDirectory("Log_Report\" & Now.ToString("yyyy\\MM") & "\OK")
            If Directory.Exists("Log_Report\" & Now.ToString("yyyy\\MM") & "\NG") = False Then Directory.CreateDirectory("Log_Report\" & Now.ToString("yyyy\\MM") & "\NG")
            If Directory.Exists("Passrate") = False Then Directory.CreateDirectory("Passrate")
            '---------------------------------------------------------
            If File.Exists("Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt") = True Then
                ntotal = ReadTextFile("Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt", 2)
                npass = ReadTextFile("Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt", 4)
                nfail = ReadTextFile("Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt", 6)
            Else
                WritePassTextfile()
            End If
        Else
            MsgBox(" File setting.txt  not found ") 'Thông báo nếu không tìm được file Seting
            End
        End If
    End Sub


    Private Sub textserial_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles textserial.KeyPress
        Dim test_model As Boolean = False
        If Asc(e.KeyChar) = 13 Then
            '========== Xu ly thong tin Barcode khi nhap vao =========================================
            textserial.Text = StrConv(textserial.Text, VbStrConv.Uppercase) ' chuyen thanh chu in hoa
            Dim kitunhandang As String = ""
            Dim ckModel As Boolean = False
            File.ReadAllLines("Setup\rom.ini").Skip(1).ToList().ForEach(
            Sub(r)
                If r.Split(",").FirstOrDefault.Trim.Contains(Logon.Model_name.Text) Then
                    ckModel = True
                    kitunhandang = r.Split(",").Skip(1).FirstOrDefault.Trim 'Tìm kí tự định dạng
                End If
            End Sub)
            If ckModel Then
                If textserial.TextLength = length_label And InStr(textserial.Text, kitunhandang, 0) <> 0 Then
                    labelcheck = True
                    serial = textserial.Text
                    barcode_rule = Mid(textserial.Text, 1, 3)   'lay ki tu quy dinh ve model tren Barcode nhap vao(barcode_rule)
                    test_model = True
                Else 'Trường hợp mã Barcode không đúng
                    labelcheck = False
                    MsgBox("Độ dài barcor khác " + length_label.ToString + " hoặc barcode không chứa kí tự " + kitunhandang)
                    Exit Sub
                End If
            End If
            If chk_bacode = 1 Then

                If labelcheck = True Then
                    mySerialPort.Write("S")
                Else
                    mySerialPort.Write("E")
                End If

            End If
            '===========================================================================================

            '============= tim xem da co file Log cua may ICT/FCT duoc tao ra chua? neu co thi se tien hanh dem so dong hien tai trong file===
            If File.Exists(setlogmachine & "\" & Now.ToString("yyyy-MM-dd.") & setfilenamereport) = True Then
                ' Tìm số dòng ban đầu trong file Log của máy
                countlinebegin = CounterlineTextFile(setlogmachine & "\" & Now.ToString("yyyy-MM-dd.") & setfilenamereport)
            End If

            If labelcheck = True Then
                For k As Integer = 1 To Datagrid.RowCount - 1
                    Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(0).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(0).Value
                    Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(1).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(1).Value
                    Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(2).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(2).Value
                    Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(3).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(3).Value
                Next
                Datagrid.Rows.Item(0).Cells(0).Value = ""        ' xoa thong tin cua dong dau trong datagrid
                Datagrid.Rows.Item(0).Cells(1).Value = ""
                Datagrid.Rows.Item(0).Cells(2).Value = ""
                Datagrid.Rows.Item(0).Cells(3).Value = ""
                Labelresult.BackColor = Color.Yellow
                Labelresult.ForeColor = Color.Red
                Labelresult.Text = "Wait"
                textserial.Enabled = False

                Timer3.Enabled = True            ' khoi tai chuong trinh quet Log
                counterlog = 0
#Region "Khoa "

                AppActivate(Findapplication(setsoftware))
                Thread.Sleep(500)
#End Region
                ' AppActivate(setprocess)
                '  SendKeys.SendWait(textserial.Text)
            End If
        End If

    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        lbtime.Text = Now
        Label4.Text = counterlog
        Label9.Text = setlogmachine & "\" & Now.ToString("yyyy-MM-dd.") & setfilenamereport
        Label10.Text = countlinecurrent
        passpercent()
    End Sub
    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        If start = True Then ' khi may da chay roi 
            'If counterlog = 5 Then
            '    start = False
            '    Timer3.Enabled = False

            '    Timer2.Enabled = False
            '    mySerialPort.Write("E")
            '    AppActivate(setprocess)

            'End If
            stringtime = Now.ToString("yyMMddHHmmss")
            ' neu file log ton tai
            If System.IO.File.Exists(setlogmachine & "\" & Now.ToString("yyyy-MM-dd.") & setfilenamereport) Then
                ' Neu ton tai file log, doc so dong trong file log do. 
                File.Copy(setlogmachine & "\" & Now.ToString("yyyy-MM-dd.") & setfilenamereport,
                       setlogmachine & "\" & Now.ToString("yyyy-MM-dd_bk.") & setfilenamereport, True)
                ' Neu ton tai file log, doc so dong trong file log do. 
                'countlinecurrent = CounterlineTextFile(setlogmachine & "\" & Now.Year & "-" & thang & "-" & ngay & "." & setfilenamereport)
                countlinecurrent = CounterlineTextFile(setlogmachine & "\" & Now.ToString("yyyy-MM-dd_bk.") & setfilenamereport)
                ' Neu so dong hien tai > so dong khi bat dau nhap barcode thi se tien hanh phan tich log

                If countlinecurrent > countlinebegin Then
                    If countlinecurrent = countlinebegin + 2 Then
                        mySerialPort.Write("E")
                        countlinebegin = countlinecurrent

                        For m As Integer = 0 To 1
                            linereport = ReadTextFile(setlogmachine & "\" & Now.ToString("yyyy-MM-dd_bk.") & setfilenamereport, countlinecurrent + m - 1)
                            counterlog = counterlog + 1
                            For k As Integer = 1 To Datagrid.RowCount - 1
                                Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(0).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(0).Value
                                Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(1).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(1).Value
                                Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(2).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(2).Value
                                Datagrid.Rows.Item(Datagrid.RowCount - k).Cells(3).Value = Datagrid.Rows.Item(Datagrid.RowCount - 1 - k).Cells(3).Value
                            Next
                            Datagrid.Rows.Item(0).Cells(0).Value = ""        ' xoa thong tin cua dong dau trong datagrid
                            Datagrid.Rows.Item(0).Cells(1).Value = ""
                            Datagrid.Rows.Item(0).Cells(2).Value = ""
                            Datagrid.Rows.Item(0).Cells(3).Value = ""
                            If InStr(linereport, "NG", 0) <> 0 Then
                                Datagrid.Rows.Item(0).Cells(0).Value = Now.ToString("dd/MM/yyyy")
                                Datagrid.Rows.Item(0).Cells(1).Value = Now.ToString("HH:mm:ss")
                                Datagrid.Rows.Item(0).Cells(2).Value = serial
                                Datagrid.Rows.Item(0).Cells(3).Value = "NG"
                                nfail = nfail + 1
                                ' Edit
                                filenamelog = "Log_Report\" & Now.ToString("yyyy\\MM\\") & "NG\" & stringtime & "_ " & serial & "_Report.csv"
                                serial = Mid(serial, 1, 10)
                                filenameLogC = "C:\Logprocess\" & stringtime & "_" & serial & "-" & (counterlog - 1) & ".txt"
                                Dim textreportforpcb As New StreamWriter(filenamelog, True)
                                textreportforpcb.WriteLine(" Serial No: " & textserial.Text & "     Operator:  " & Logon.TextBox1.Text & "    " & Logon.TextBox2.Text)
                                textreportforpcb.WriteLine(linereport) ' ghi thong tin vao file log

                                ' Ghi log ổ C - quyetpv
                                Dim textreportforpcbC As New StreamWriter(filenameLogC, True)
                                textreportforpcbC.WriteLine(Logon.Model_name.Text & "|" & serial & "-" & (counterlog - 1) & "|" & stringtime & "|F|ROM")
                                textreportforpcb.Close()
                                textreportforpcbC.Close()
                                Try
                                    File.Copy(setlogmachine & "\" & Now.ToString("yyyy-MM-dd_bk.") & setfilenamereport,
                                          setlogmachine & "\" & Now.ToString("yyyy-MM-dd.") & setfilenamereport, True)
                                Catch ex As Exception

                                End Try
                                'retry = 0
                                'countread = 0
                                If ComboBox1.Text = "Pcs" Then
                                    Labelresult.BackColor = Color.Green
                                    Labelresult.ForeColor = Color.White
                                    Labelresult.Text = "OK"

                                    counterlog = 0
                                    start = False
                                    Timer3.Enabled = False
                                    Timer2.Enabled = False

                                    AppActivate(setprocess)
                                    textserial.Enabled = True
                                    textserial.Focus()
                                    TextReceive.Text = ""
                                    textserial.Text = ""
                                Else
                                    If counterlog = counterlog_chk Then

                                        Labelresult.BackColor = Color.Green
                                        Labelresult.ForeColor = Color.White
                                        Labelresult.Text = "STOP"

                                        counterlog = 0
                                        start = False
                                        Timer3.Enabled = False
                                        Timer2.Enabled = False

                                        AppActivate(setprocess)

                                        textserial.Enabled = True
                                        textserial.Focus()
                                        TextReceive.Text = ""
                                        textserial.Text = ""
                                    End If
                                End If

                            Else
                                Datagrid.Rows.Item(0).Cells(0).Value = Now.ToString("dd/MM/yyyy")
                                Datagrid.Rows.Item(0).Cells(1).Value = Now.ToString("HH:mm:ss")
                                Datagrid.Rows.Item(0).Cells(2).Value = serial
                                Datagrid.Rows.Item(0).Cells(3).Value = "OK"

                                npass = npass + 1
                                ' Thiết lập Logfile
                                filenamelog = "Log_Report\" & Now.ToString("yyyy\\MM") & "\OK\" & stringtime & "_ " & serial & "_Report.csv"
                                serial = Mid(serial, 1, 10)
                                filenameLogC = "C:\Logprocess\" & stringtime & "_" & serial & "-" & (counterlog - 1) & ".txt"
                                Dim textreportforpcb As New System.IO.StreamWriter(filenamelog, True)
                                Dim textreportforpcbC As New System.IO.StreamWriter(filenameLogC, True)
                                textreportforpcb.WriteLine(" Serial No: " & textserial.Text & "     Operator:  " & Logon.TextBox1.Text & "    " & Logon.TextBox2.Text)
                                textreportforpcb.WriteLine(linereport) ' ghi thong tin vao file log
                                'File long mới - quyetpv
                                textreportforpcbC.WriteLine(Logon.Model_name.Text & "|" & serial & "-" & (counterlog - 1) & "|" & stringtime & "|P|ROM")
                                textreportforpcb.Close()
                                textreportforpcbC.Close()
                                'retry = 0
                                'countread = 0

                                If ComboBox1.Text = "Pcs" Then
                                    Labelresult.BackColor = Color.Green
                                    Labelresult.ForeColor = Color.White
                                    Labelresult.Text = "OK"

                                    counterlog = 0
                                    start = False
                                    Timer3.Enabled = False
                                    Timer2.Enabled = False

                                    AppActivate(setprocess)
                                    textserial.Enabled = True
                                    textserial.Focus()
                                    TextReceive.Text = ""
                                    textserial.Text = ""
                                Else
                                    If counterlog = counterlog_chk Then

                                        Labelresult.BackColor = Color.Green
                                        Labelresult.ForeColor = Color.White
                                        Labelresult.Text = "STOP"

                                        counterlog = 0
                                        start = False
                                        Timer3.Enabled = False
                                        Timer2.Enabled = False

                                        AppActivate(setprocess)

                                        textserial.Enabled = True
                                        textserial.Focus()
                                        TextReceive.Text = ""
                                        textserial.Text = ""
                                    End If
                                End If

                            End If
                            '' vao day
                        Next
                    End If
                End If

                filename_passrate = "Passrate\" & Now.ToString("yyyyMMdd") & "_Passrate.txt"
                Dim text_passrate As New StreamWriter(filename_passrate)
                text_passrate.WriteLine("# Total")
                text_passrate.WriteLine(ntotal)
                text_passrate.WriteLine("# Pass")
                text_passrate.WriteLine(npass)
                text_passrate.WriteLine("# NG")
                text_passrate.WriteLine(nfail)
                text_passrate.WriteLine("# Passrate")
                text_passrate.WriteLine(Passrate.Text)
                text_passrate.Close()
            End If
        End If
    End Sub
    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        '  mySerialPort.Write("S") ' sent ki tu S
        ' If mySerialPort.ReadExisting() = "S" Then ' may da ngung
        ' TextReceive.Text = "S"
        Timer2.Enabled = True
        ' If countread < 5 Then ' chong tran bien
        'countread = countread + 1 ' cong den khi tram den 4 lan
        '  End If
        ' If countread = 4 Then
        start = True
        ' Labelresult.BackColor = Color.Yellow
        ' Labelresult.ForeColor = Color.Red
        '  Labelresult.Text = "Busy"
        ' active phan mem cua may fct
        'SendKeys.SendWait("{ENTER}")
        'End If
        'Else
        'TextReceive.Text = ""
        'Timer2.Enabled = False
        'End If
    End Sub

    Private Sub Sent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Sent.Click
        If mySerialPort.IsOpen = False Then ' neu cong com dang dong thi mo lai
            mySerialPort.Open()
            mySerialPort.Write(TextSent.Text)
        Else
            mySerialPort.Write(TextSent.Text)
        End If
    End Sub

    Private Sub Receive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Receive.Click
        If chk_bacode = 1 Then

            If mySerialPort.IsOpen = False Then ' neu cong com dang dong thi mo lai
                CommPortSetup()
                mySerialPort.DiscardInBuffer()
                TextReceive.Text = mySerialPort.ReadExisting
            Else
                TextReceive.Text = mySerialPort.ReadExisting
            End If
        End If
    End Sub
    Private Sub Reset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Reset.Click
        setup_display()
        If chk_bacode = 1 Then mySerialPort.Write("E")
        Timer2.Enabled = False
        Timer3.Enabled = False
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If MessageBox.Show("Do you want to exit?", "Support FCT ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            If chk_bacode = 1 Then
                If mySerialPort.IsOpen = True Then
                    'mySerialPort.Write("S")
                    'quyetpham add 28/9
                    mySerialPort.WriteLine("E")
                    'end
                    mySerialPort.Close()
                End If
                End
            End If
        End If
    End Sub

    Private Sub Clear_report_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Clear_report.Click
        WritePassTextfile()
        passpercent()
        textserial.Focus()
        MessageBox.Show("Clear hoàn thành!", "Information")
    End Sub

End Class
