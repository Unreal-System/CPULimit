using System;
using System.Threading;

namespace CPULimit
{
    /// <summary>
    /// 负责处理执行挂起恢复操作的类,将其存储为List<T>可操作多个.
    /// </summary>
    internal class ProcessItem : IDisposable 
    {
        private Timer _Timer = null;

        /// <summary>
        /// 运行状态
        /// </summary>
        private Boolean _RunState = false;

        /// <summary>
        /// 保留的进程PID
        /// </summary>
        internal Int32 ProcessId { get { return _ProcessId; } }
        private Int32 _ProcessId;

        /// <summary>
        /// 计时器间隔
        /// </summary>
        internal Int32 TimerInterval { get { return _TimerInterval; } }
        private Int32 _TimerInterval;

        /// <summary>
        /// 暂停进程后间隔
        /// </summary>
        internal Int32 PauseInterval { get { return _PauseInterval; } }
        private Int32 _PauseInterval;

        /// <summary>
        /// 创建者用户名
        /// </summary>
        internal String CreateUsername { get { return _CreateUsername; } }
        private String _CreateUsername;

        /// <summary>
        /// 备注信息
        /// </summary>
        internal String Remake { get { return _Remake; } }
        private String _Remake;

        /// <summary>
        /// 创建新实例
        /// </summary>
        /// <param name="processId">进程pid</param>
        /// <param name="remark">备注信息</param>
        /// <param name="timerInterval_ms">暂停时间</param>
        /// <param name="pauseInterval_ms">恢复时间</param>
        internal ProcessItem(Int32 processId, String createUsername, String remark, Int32 timerInterval_ms, Int32 pauseInterval_ms)
        {
            _TimerInterval = timerInterval_ms;
            _PauseInterval = pauseInterval_ms;
            _ProcessId = processId;
            _CreateUsername = createUsername;
            _Remake = remark;
            _Timer = new Timer(new TimerCallback(TimerCallback), null, 0, timerInterval_ms);
        }

        private void TimerCallback(object obj)
        {
            if (_RunState == false)
            {
                ProcessManager.SuspendProc(_ProcessId); // 这地方执行你的挂起进程函数 
                while (_Timer.Change(5, _PauseInterval) != true) { } // 5ms为当更改完成后距离启动计时器剩余5ms,这段时间用于更改运行状态(_runState)
                _RunState = true;
            }
            else
            {
                ProcessManager.ResumeProc(_ProcessId); // 这地方执行你的恢复挂起的函数
                while (_Timer.Change(5, _TimerInterval) != true) { } // 同上,使用while只是为了确保能够成功更新计时器
                _RunState = false;
            }
        }

        public override String ToString()
        {
            return $"用户:{_CreateUsername}|进程:{_ProcessId}|备注:{_Remake}";
        }

        public void Dispose()
        {
            while (_Timer.Change(5, 1000000) != true) ;
            ProcessManager.ResumeProc(_ProcessId);
            _Timer.Dispose();
            _Timer = null;
            GC.Collect(0);
        }
    }
}