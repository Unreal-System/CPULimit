using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace CPULimit
{
    internal partial class FrmConfig : Form
    {

        private List<ProcessItem> _ProcessList;

        internal FrmConfig(ref List<ProcessItem> list)
        {
            _ProcessList = list;
            InitializeComponent();
            BtnFlush_Click(null, null);
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                var item = new ProcessItem(Convert.ToInt32(CboProcess.SelectedValue), TboUsername.Text, TboRemark.Text, Convert.ToInt32(NUDTimerInterval.Value), Convert.ToInt32(NUDPauseInterval.Value));
                _ProcessList.Add(item);
                LboList.Items.Add($"用户:{TboUsername.Text}|进程:{CboProcess.SelectedValue}|备注:{TboRemark.Text}");
                BtnFlush_Click(null, null);
            }
            catch { }
            //item = new ProcessItem(Convert.ToInt32(CboProcess.SelectedValue), Convert.ToInt32(NUDTimerInterval.Value) , Convert.ToInt32(NUDPauseInterval.Value));
            //MessageBox.Show($"{CboProcess.SelectedValue}, {NUDTimerInterval.Value} , {NUDPauseInterval.Value} is Ok!");
            //LboList.DataBindings.Add("Text", _ProcessList, "Key", false, DataSourceUpdateMode.OnPropertyChanged);
            //LboList.DataBindings.Add("List" , new KeyValuePair<>);
            //_ProcessList.Add(new KeyValuePair<ProcessItem, String>(new ProcessItem(Convert.ToInt32(CboProcess.SelectedValue), Convert.ToInt32(NUDTimerInterval.Value), Convert.ToInt32(NUDPauseInterval.Value)), TboName.Text));
            //((new ProcessItem(Convert.ToInt32(CboProcess.SelectedValue), Convert.ToInt32(NUDTimerInterval.Value), Convert.ToInt32(NUDPauseInterval.Value))), TboName.Text);
            //bindingSource.Add(new ProcessItem(Convert.ToInt32(CboProcess.SelectedValue), Convert.ToInt32(NUDTimerInterval.Value), Convert.ToInt32(NUDPauseInterval.Value), TboName.Text));
            //TboName.Text = "";
            //CboProcess.SelectedValue = "";
            //NUDPauseInterval.Value = 1000;
            //NUDTimerInterval.Value = 1000;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (LboList.SelectedItem != null)
                {
                    _ProcessList[LboList.SelectedIndex].Dispose();
                    _ProcessList.RemoveAt(LboList.SelectedIndex);
                }
                BtnFlush_Click(null, null);
            }
            catch { }
        }

        private void LboList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (LboList.SelectedItem != null)
                {
                    var item = _ProcessList[LboList.SelectedIndex];
                    TboUsername.Text = item.CreateUsername;
                    TboRemark.Text = item.Remake;
                    CboProcess.Text = Process.GetProcessById(item.ProcessId).ProcessName;
                    NUDTimerInterval.Value = item.TimerInterval;
                    NUDPauseInterval.Value = item.PauseInterval;
                }
                BtnFlush_Click(null, null);
            }
            catch { }
        }

        private void CboProcess_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var bS = new BindingSource { DataSource = Process.GetProcesses() };
                CboProcess.DataSource = bS;
                CboProcess.DisplayMember = "ProcessName";
                CboProcess.ValueMember = "Id";
                BtnFlush_Click(null, null);
            }
            catch { }
        }

        private void BtnFlush_Click(object sender, EventArgs e)
        {
            try
            {
                LboList.Items.Clear();
                foreach (var i in _ProcessList)
                {
                    var msg = i.ToString();
                    LboList.Items.Add(msg);
                }
            }
            catch { }
        }


    }
}