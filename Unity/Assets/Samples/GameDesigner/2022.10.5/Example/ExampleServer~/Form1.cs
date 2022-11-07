using Example1;
using Net.Event;
using Net.Share;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ExampleServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private Service server;
        private bool run;

        private void button1_Click(object sender, EventArgs e)
        {
            if (run) 
            {
                button1.Text = "启动";
                server?.Close();
                run = false;
                NDebug.RemoveDebug();
                return;
            }
            NDebug.BindDebug(new FormDebug(listBox1));
            int port = int.Parse(textBox2.Text);//设置端口
            server = new Service();//创建服务器对象
            server.OnlineLimit = 24000;//服务器最大运行2500人连接
            server.LineUp = 24000;
            server.MaxThread = Environment.ProcessorCount; //增加并发线程
            server.RTO = 50;
            server.MTU = 1300;
            server.MTPS = 2048;
            server.SetHeartTime(10,1000);
            server.OnNetworkDataTraffic += (df) => {//当统计网络性能,数据传输量
                toolStripStatusLabel1.Text = $"流出:{df.sendNumber}次/{ByteHelper.ToString(df.sendCount)} " +
                $"流入:{df.receiveNumber}次/{ByteHelper.ToString(df.receiveCount)} " +
                $"发送fps:{df.sendLoopNum} 接收fps:{df.revdLoopNum} 解析:{df.resolveNumber}次 " +
                $"总流入:{ByteHelper.ToString(df.inflowTotal)} 总流出:{ByteHelper.ToString(df.outflowTotal)}";
                label2.Text = "登录:" + server.OnlinePlayers + " 未登录:" + server.UnClientNumber;
            };
            server.AddAdapter(new Net.Adapter.SerializeAdapter2());
            server.AddAdapter(new Net.Adapter.CallSiteRpcAdapter<Client>(server));
            server.Run((ushort)port);//启动
            run = true;
            button1.Text = "关闭";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server?.Close();
            Process.GetCurrentProcess().Kill();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            var item = listBox1.SelectedItem;
            if (item == null)
                return;
            MessageBox.Show(item.ToString());
        }
    }
}
