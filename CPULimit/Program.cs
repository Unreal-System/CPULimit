using CPULimit.Net;
using CPULimit.Log;
using CPULimit.Properties;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using Newtonsoft.Json;

namespace CPULimit
{
    static class Program
    {
        private static Mutex mutex = null;
        private static LogMain log = new LogMain(AppDomain.CurrentDomain.BaseDirectory + "//Log");

        [STAThread]
        static void Main(String[] args)
        {
            mutex = new Mutex(true, System.Diagnostics.Process.GetCurrentProcess().MachineName, out bool isRun);
            if (!isRun) { MessageBox.Show("程序已运行!\r\n请查看右下角任务栏!"); Environment.Exit(1); }

            if (args.Length != 4)
            {
                MessageBox.Show("请输入正确参数!\r\n第一个参数是最大连接数量\r\n第二个参数是缓冲区大小\r\n第三个参数是监听IP\r\n第四个参数是监听端口!\r\n点击确定退出程序...");
                Environment.Exit(1);
            }
            else
            {
                //Thread.Sleep(10000);
                ProgramMain main = new ProgramMain(args, ref log);
                Application.Run();
                log.SaveLog();
            }
        }
    }

    internal class ProgramMain
    {
        private List<ProcessItem> _ProcessList;
        private SocketManager _SocketManager = null;
        private FrmConfig _FrmConfig = null;
        private LogMain log = null;

        private NotifyIcon notifyIco = new NotifyIcon();
        private ContextMenuStrip _CMS = new ContextMenuStrip();
        private ToolStripMenuItem _TSMI1 = new ToolStripMenuItem();
        private ToolStripMenuItem _TSMI2 = new ToolStripMenuItem();

        internal ProgramMain(String[] args, ref LogMain logMain)
        {
            log = logMain;
            _ProcessList = new List<ProcessItem>();
            _SocketManager = new SocketManager(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), ref log);
            _SocketManager.ReceiveClientData += SocketManager_ReceiveClientData;
            _SocketManager.Init();
            _SocketManager.Start(new IPEndPoint(IPAddress.Parse(args[2]), Convert.ToInt32(args[3])));
            

            notifyIco.Visible = true;
            notifyIco.Icon = Resources.Icon;
            notifyIco.MouseDoubleClick += NotifyIco_MouseDoubleClick;

            _TSMI1.Text = "退出";
            _TSMI1.Click += TSMI1_Click;
            _TSMI2.Text = "管理";
            _TSMI2.Click += TSMI2_Click;

            _CMS.Items.AddRange(new ToolStripMenuItem[] { _TSMI2, _TSMI1 });
            notifyIco.ContextMenuStrip = _CMS;
        }

        private void SocketManager_ReceiveClientData(AsyncUserToken token, byte[] buff)
        {
            var msg = String.Empty;
            try
            {
                msg = Encoding.UTF8.GetString(buff);
                var item = JsonConvert.DeserializeObject<JsonItem>(msg);
                switch (item.State)
                {
                    case "Add":
                        AddItem(token, item);
                        break;
                    case "Del":
                        DelItem(token, item);
                        break;
                    case "Que":
                        QueItem(token);
                        break;
                    default:
                        _SocketManager.SendMessage(token, new byte[] { 85, 110, 107, 110, 111, 119, 83, 116, 97, 116, 101 }); // UnknowState
                        log.AddLog(LogType.Info, $"用户{token.IPAddress}发送了错误的Json信息!");
                        break;
                }
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"在执行解析内容时出错!\r\n发送用户:{token.IPAddress}\r\n发送内容:{msg}\r\n错误信息:{ex.Message}");
                _SocketManager.SendMessage(token, new byte[] { 68, 101, 115, 101, 114, 105, 97, 108, 105, 122, 101, 74, 115, 111, 110, 69, 114, 114, 111, 114 }); // DeserializeJsonError
            }
        }

        private void TSMI1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void TSMI2_Click(object sender, EventArgs e)
        {
            if (_FrmConfig == null)
                _FrmConfig = new FrmConfig(ref _ProcessList);
            _FrmConfig.Show();
        }

        private void NotifyIco_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_FrmConfig == null)
                _FrmConfig = new FrmConfig(ref _ProcessList);
            _FrmConfig.Show();
        }
        
        private void AddItem(AsyncUserToken token, JsonItem jsonItem)
        {
            var item = new ProcessItem(jsonItem.PID, jsonItem.UserName, jsonItem.Remake, jsonItem.Timer, jsonItem.Pause);
            lock (_ProcessList) _ProcessList.Add(item);
            _SocketManager.SendMessage(token, new byte[] { 65, 100, 100, 73, 116, 101, 109, 79, 75 }); // AddItemOK
        }

        private void DelItem(AsyncUserToken token, JsonItem jsonItem)
        {
            lock (_ProcessList)
            {
                ProcessItem item = null;
                Boolean tf = false;
                foreach (var i in _ProcessList)
                {
                    if (i.ProcessId == jsonItem.PID)
                    {
                        item = i; tf = true;
                    }
                    break;
                }
                if (tf == true)
                {
                    _ProcessList.Remove(item);
                    _SocketManager.SendMessage(token, new byte[] { 68, 101, 108, 73, 116, 101, 109, 79, 75 }); // DelItemOK
                }
                else
                {
                    _SocketManager.SendMessage(token, new byte[] { 78, 111, 116, 70, 111, 117, 110, 100, 68, 101, 108, 73, 116, 101, 109, 79 }); // NotFoundDelItem
                }
            }
        }

        private void QueItem(AsyncUserToken token)
        {
            lock (_ProcessList)
            {
                foreach (var i in _ProcessList)
                {
                    var json = new JsonItem() {
                        PID = i.ProcessId,
                        UserName = i.CreateUsername,
                        Remake = i.Remake,
                        Timer = i.TimerInterval,
                        Pause = i.PauseInterval
                    };
                    var msg = JsonConvert.SerializeObject(json);
                    byte[] msgb = Encoding.UTF8.GetBytes(msg);
                    _SocketManager.SendMessage(token, msgb);
                }
            } _SocketManager.SendMessage(token, new byte[] { 81, 117, 101, 114, 121, 79, 75 }); // QueryOK
        }

    }
}