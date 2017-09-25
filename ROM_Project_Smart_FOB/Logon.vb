Imports System.Threading
Imports System.IO
Public Class Logon
    Sub loadData()
        Dim testmsg As Integer = 0
        If TextBox1.Text <> "" And TextBox2.Text <> "" Then
            testmsg = MsgBox("Model Name: " & Model_name.Text & "  Please Check again", 1, "Model Name Checking")
            If testmsg = 1 And Model_name.Text <> "" Then
                File.ReadAllLines("Setup\rom.ini").Skip(1).ToList.ForEach(
                    Sub(r)
                        If r.Split(",").FirstOrDefault().Trim.Contains(Model_name.Text.Trim) Then
                            valueModel = r.Split(",").Skip(2).FirstOrDefault.Trim
                        End If
                    End Sub
                    )
                If valueModel <> "" Then
                    Try
                        Process.Start(valueModel)
                    Catch ex As Exception
                        MessageBox.Show("Dường dẫn " + valueModel + " không chính xác", "information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Try
                    Me.Hide()
                    Main.Show()
                Else
                    MessageBox.Show("Đường dẫn không có", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            End If
        Else
            MsgBox(" Nhap Ten & Ma code truoc khi thao tac")
        End If
    End Sub
    Private Sub Logon_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        File.ReadAllLines("Setup\rom.ini").Skip(1).ToList().ForEach(
            Sub(r)
                Dim item As String = r.Split(",").FirstOrDefault().Trim
                Model_name.Items.Add(item)
            End Sub
            )
    End Sub

    Private Sub Model_name_SelectedValueChanged(sender As Object, e As EventArgs) Handles Model_name.SelectedValueChanged
        loadData()
    End Sub

    Private Sub Model_name_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Model_name.KeyPress
        If Asc(e.KeyChar) = 13 Then loadData()
    End Sub

    Private Sub btCancel_Click(sender As Object, e As EventArgs) Handles btCancel.Click
        TextBox1.Text = ""
        TextBox2.Text = ""
        Model_name.Text = ""
    End Sub
End Class