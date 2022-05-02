using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;         //匯入網路通訊協定相關函數
using System.Net.Sockets; //匯入網路插座功能函數
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket T;    //通訊物件
        Thread Th;
        string User; //使用者
        //登入伺服器 
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            string IP = textBox1.Text;                                 //伺服器IP
            int Port = int.Parse(textBox2.Text);                       //伺服器Port
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port); //伺服器的連線端點資訊
            //建立可以雙向通訊的TCP連線
            T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            User = textBox3.Text;                                     //使用者名稱 
            try
            {
                T.Connect(EP);                        //連上伺服器的端點EP(類似撥號給電話總機)

                Th = new Thread(Listen);
                Th.IsBackground = true;
                Th.Start();

                textBox4.Text = "無法連接伺服器" + "\r\n";
                Send("0" + User);                     //連線後隨即傳送自己的名稱給伺服器
            }
            catch (Exception)
            {
                MessageBox.Show("無法連上伺服器！");  //連線失敗時顯示訊息
                return;
            }
            button1.Enabled = false;                  //讓連線按鍵失效，避免重複連線 
            button2.Enabled = true;
        }
        //傳送訊息給 Server (Send Message to the Server)
        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);//翻譯字串Str為Byte陣列B
            T.Send(B, 0, B.Length, SocketFlags.None); //使用連線物件傳送資料
        }
        //關閉視窗代表離線登出 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button1.Enabled == false)
            {
                Send("9" + User); //傳送自己的離線訊息給伺服器
                T.Close();        //關閉網路通訊器T
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Listen()
        {
            EndPoint ServerEP = (EndPoint)T.RemoteEndPoint;
            byte[] B = new byte[1023];
            int inLen = 0;
            string Msg;  //接收到的完整訊息
            string St;   //指令碼
            string Str;  //訊息內容

            while(true)
            {
                try
                {
                    inLen = T.ReceiveFrom(B, ref ServerEP);
                }
                catch(Exception)
                {
                    T.Close();
                    listBox1_onlineList.Items.Clear();
                    MessageBox.Show("伺服器斷線了!!");
                    button1.Enabled = true;
                    Th.Abort();
                }
                Msg = Encoding.Default.GetString(B, 0, inLen);
            St = Msg.Substring(0, 1);
            Str = Msg.Substring(1);

            switch(St)
            {
                    case "L":
                    listBox1_onlineList.Items.Clear();
                    string[] M = Str.Split(',');
                    foreach(string user in M)
                    {
                        listBox1_onlineList.Items.Add(user);
                    }
                    break;
                    case "1":  //接收到廣播訊息
                    textBox4.Text += "(公開)" + Str + "\r\n";
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.ScrollToCaret();
                    break;
                    case "2":  //接收到私密訊息
                        textBox4.Text += "(公開)" + Str + "\r\n";
                        textBox1.SelectionStart = textBox1.Text.Length;
                        textBox1.ScrollToCaret();
                        break;
                }
            
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "") return;

            if (listBox1_onlineList.SelectedIndex < 0)
            {
                Send("1" + User + "公告:" + textBox5.Text);
            }
            else
            {
                Send("2" + "來自" + User + ":" + textBox5.Text + "|" + listBox1_onlineList.SelectedIndex);
                textBox5.Text += "告訴" + listBox1_onlineList.SelectedIndex + ":" + textBox5.Text + "\r\n";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1_onlineList.ClearSelected();
        }

        private void Logout()
        {
            if (button1.Enabled == false)
            {
                Send("9" + User); //傳送自己的離線訊息給伺服器
                T.Close();        //關閉網路通訊器T
            }
        }
    }
}
