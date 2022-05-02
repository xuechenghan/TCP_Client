Imports System.Net          '網路相關函數
Imports System.Net.Sockets  '網路通訊物件函數
Imports System.Text         '文字編碼函數

Public Class Form1

    Dim T As Socket     '網路連線物件
    Dim User As String  '使用者名稱

    '登入伺服器
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim IP As String = TextBox1.Text    '伺服器IP
        Dim Port As Integer = TextBox2.Text '伺服器Port
        Dim EP As IPEndPoint = New IPEndPoint(IPAddress.Parse(IP), Port) '伺服器的連線端點資訊
        '建立通訊物件，參數代表可以雙向通訊的TCP連線
        T = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        User = TextBox3.Text  '使用者名稱
        Try
            T.Connect(EP) '連上伺服器的端點EP(類似撥號給電話總機)
            Send("0" + User)  '連線後隨即傳送自己的名稱給伺服器
        Catch ex As Exception
            MsgBox("無法連上伺服器！") '連線失敗時顯示訊息
            Exit Sub '連線失敗離開副程式
        End Try
        Button1.Enabled = False '讓連線按鍵失效，避免重複連線
    End Sub

    '傳送訊息給 Server (Send Message to the Server)
    Public Sub Send(Str As String)
        Dim B() As Byte = Encoding.Default.GetBytes(Str) '翻譯文字為Byte陣列
        T.Send(B, 0, B.Length, SocketFlags.None) '使用連線物件傳送資料
    End Sub

    '視窗關閉，代表離線(登出)
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (Button1.Enabled = False) Then '上線狀態
            Send("9" + User) '傳送自己的離線訊息給伺服器
            T.Close() '關閉網路通訊器
        End If
    End Sub
End Class
