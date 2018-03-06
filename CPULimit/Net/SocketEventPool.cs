using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CPULimit.Net
{
    /// <summary>
    /// Socket事件池
    /// </summary>
    internal class SocketEventPool
    {
        /// <summary>
        /// 异步套接字事件池对象
        /// </summary>
        private Stack<SocketAsyncEventArgs> _Pool;

        /// <summary>
        /// 初始化异步套接字事件池
        /// </summary>
        /// <param name="capacity">池的最大大小</param>
        internal SocketEventPool(Int32 capacity)
        {
            _Pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// 将异步套接字事件添加入事件池
        /// </summary>
        /// <param name="item">异步套接字事件</param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items Error."); }
            lock (_Pool) _Pool.Push(item);
        }

        /// <summary>
        /// 删除最顶部的一个异步套接字事件并返回
        /// </summary>
        /// <returns>返回异步套接字事件</returns>
        internal SocketAsyncEventArgs Pop()
        {
            lock (_Pool) return _Pool.Pop();
        }

        /// <summary>
        /// 池中异步Socket实例的数量
        /// </summary>
        internal Int32 Count { get { return _Pool.Count; } }

        /// <summary>
        /// 清空套接字事件池
        /// </summary>
        internal void Clear() { _Pool.Clear(); }
    }
}