using System;
using System.Collections.Generic;
using System.Net.Sockets;

using CPULimit.Log;

namespace CPULimit.Net
{
    internal class BufferManager
    {

        private Int32 _NumBytes;
        private Byte[] _Buffer;
        private Stack<Int32> _FreeIndexPool;
        private Int32 _CurrentIndex;
        private Int32 _BufferSize;

        private LogMain log = null;

        internal BufferManager(Int32 totalBytes, Int32 bufferSize, ref LogMain logMain)
        {
            log = logMain;
            _NumBytes = totalBytes;
            _CurrentIndex = 0;
            _BufferSize = bufferSize;
            _FreeIndexPool = new Stack<Int32>();
        }

        internal void InitBuffer()
        {
            _Buffer = new Byte[_NumBytes];
        }

        internal Boolean SetBuffer(SocketAsyncEventArgs args)
        {
            try
            {
                if (_FreeIndexPool.Count > 0)
                {
                    args.SetBuffer(_Buffer, _FreeIndexPool.Pop(), _BufferSize);
                }
                else
                {
                    if ((_NumBytes - _BufferSize) < _CurrentIndex)
                    {
                        return false;
                    }
                    args.SetBuffer(_Buffer, _CurrentIndex, _BufferSize);
                    _CurrentIndex += _BufferSize;
                }
            }
            catch (Exception ex)
            {
                AsyncUserToken token = args.UserToken as AsyncUserToken;
                log.AddLog(LogType.Error, $"在SetBuffer时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
            return true;
        }

        /// <summary>
        /// 释放一个缓冲区
        /// </summary>
        /// <param name="args"></param>
        internal void FreeBuffer(SocketAsyncEventArgs args)
        {
            try
            {
                _FreeIndexPool.Push(args.Offset);
                args.SetBuffer(null, 0, 0);
            }
            catch (Exception ex)
            {
                AsyncUserToken token = args.UserToken as AsyncUserToken;
                log.AddLog(LogType.Error, $"在释放一个缓冲区时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
        }
    }
}