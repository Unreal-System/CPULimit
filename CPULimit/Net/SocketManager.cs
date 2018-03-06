using CPULimit.Log;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CPULimit.Net
{
    internal class SocketManager
    {
        #region 属性
        /// <summary>
        /// 最大连接数量
        /// </summary>
        private Int32 _MaxConnectNum;
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        private Int32 _RevBufferSize;
        /// <summary>
        /// 流管理对象?
        /// </summary>
        private BufferManager _BufferManager;
        private const int opsToAlloc = 2;//???
        /// <summary>
        /// 监听Socket
        /// </summary>
        private Socket _ListenSocket;
        /// <summary>
        /// 异步套接字事件池
        /// </summary>
        private SocketEventPool _Pool;
        private Int32 _ClientCount;
        /// <summary>
        /// 最大接收用户数量
        /// </summary>
        private Semaphore _MaxNumberAcceptedClients;
        /// <summary>
        /// 获取客户端集合
        /// </summary>
        internal List<AsyncUserToken> ClientList { get { return _Clients; } }
        /// <summary>
        /// 客户端集合
        /// </summary>
        private List<AsyncUserToken> _Clients;

        private LogMain log = null;
        #endregion

        #region 委托和事件
        /// <summary>
        /// 客户端连接数量变化时触发
        /// </summary>
        /// <param name="num">当前增加客户端的个数(用户退出时为负数,增加时为正数,一般为1)</param>
        /// <param name="token">用户的信息</param>
        internal delegate void OnClientNumberChange(Int32 num, AsyncUserToken token);
        /// <summary>
        /// 接收到客户端的数据
        /// </summary>
        /// <param name="token">客户端</param>
        /// <param name="buff">客户端数据</param>
        internal delegate void OnReceiveData(AsyncUserToken token, Byte[] buff);

        /// <summary>
        /// 客户端连接数量变化事件
        /// </summary>
        internal event OnClientNumberChange ClientNumberChange;
        /// <summary>
        /// 接收到客户端的数据事件
        /// </summary>
        internal event OnReceiveData ReceiveClientData;
        #endregion

        #region 方法

        internal SocketManager(Int32 numConnections, Int32 receiveBufferSize, ref LogMain logMain)
        {
            log = logMain;
            _ClientCount = 0; // 清理客户端连接数

            _MaxConnectNum = numConnections; // 设置最大客户端连接数
            _RevBufferSize = receiveBufferSize; // 设置缓冲区大小
            try
            {
                _BufferManager = new BufferManager(receiveBufferSize * numConnections * opsToAlloc, receiveBufferSize, ref log); // 分配缓冲区,
                                                                                                                        // 使得最大数量套接字可以有一个未完成读写同时发送到Socket
                _Pool = new SocketEventPool(numConnections); // 初始化套接字事件池
                _MaxNumberAcceptedClients = new Semaphore(numConnections, numConnections); // 限制I/O大小范围
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"因用户操作不当,此Bug已影响程序正常运行!\r\n错误信息:{ex.Message}");
            }
        }

        internal void Init()
        {
            _BufferManager.InitBuffer(); // 分配一个大字节缓冲区,所有的I/O操作都使用一个,可以防止内存碎片化(内存上讲是完整内存使用,降低资源占用)
            _Clients = new List<AsyncUserToken>(); // 分配异步Socket事件池
            SocketAsyncEventArgs readWriteEventArg; // 定义异步套接字操作
            for (int i = 0; i < _MaxConnectNum; i++)
            {
                readWriteEventArg = new SocketAsyncEventArgs(); // 初始化异步套接字事件实例
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed); // 绑定处理事件
                readWriteEventArg.UserToken = new AsyncUserToken(); // 绑定用户数据对象
                _BufferManager.SetBuffer(readWriteEventArg); // 将缓冲池中的字节缓冲区分配给异步Socket事件
                _Pool.Push(readWriteEventArg); // 将异步Socket事件添加到池中
            }
        }

        internal Boolean Start(IPEndPoint localEndPoint)
        {
            try
            {
                _Clients.Clear(); // 清空用户
                _ListenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // 初始化Socket新实例
                _ListenSocket.Bind(localEndPoint); // 绑定终结点
                _ListenSocket.Listen(_MaxConnectNum); // 设置最大客户端数量
                StartAccept(null);
                return true;
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"监听端口时出错!\r\n错误信息:{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 停止并清空服务器
        /// </summary>
        internal void Stop()
        {
            foreach (AsyncUserToken token in _Clients)
            {
                try
                {
                    token.Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    log.AddLog(LogType.Error, $"在断开与客户端的连接时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
                }
            }
            try
            {
                _ListenSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"在关闭服务器监听时出错!\r\n错误信息:{ex.Message}");
            }
            _ListenSocket.Close();
            Int32 Count = _Clients.Count;
            lock (_Clients) _Clients.Clear();
            ClientNumberChange?.Invoke(-Count, null);
        }

        /// <summary>
        /// 关闭指定客户端
        /// </summary>
        /// <param name="token"></param>
        internal void CloseClient(AsyncUserToken token)
        {
            try
            {
                token.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"在关闭指定客户端时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
        }

        /// <summary>
        /// 接收客户端连接请求
        /// </summary>
        /// <param name="args"></param>
        private void StartAccept(SocketAsyncEventArgs args)
        {
            try
            {
                if (args == null)
                {
                    args = new SocketAsyncEventArgs(); // 创建新实例并绑定事件对象
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(Args_Completed);
                }
                else { args.AcceptSocket = null; /*因为上下文对象呗重用必须清空*/}
                _MaxNumberAcceptedClients.WaitOne(); // 线程等待
                if (!_ListenSocket.AcceptAsync(args)) ProcessAccept(args);
            }
            catch (Exception ex)
            {
                AsyncUserToken token = args.UserToken as AsyncUserToken;
                log.AddLog(LogType.Error, $"在接收客户端连接请求时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
        }

        /// <summary>
        /// 发送数据到指定对象
        /// </summary>
        /// <param name="token">用户Token信息,以此来辨识用户</param>
        /// <param name="message">要发送的信息</param>
        internal void SendMessage(AsyncUserToken token, Byte[] message)
        {
            if (token == null || token.Socket == null || !token.Socket.Connected) return;
            try
            {
                Byte[] buff = new Byte[message.Length + 4]; // 简单协议发送
                Byte[] len = BitConverter.GetBytes(message.Length);
                Array.Copy(len, buff, 4);
                Array.Copy(message, 0, buff, 4, message.Length);

                SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs { UserToken = token };
                sendArgs.SetBuffer(message, 0, message.Length); // 数据放入缓冲池
                token.Socket.SendAsync(sendArgs); // 发送数据
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"发送数据到指定对象时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
        }
        #endregion

        #region 处理事件
        /// <summary>
        /// I/O事件处理
        /// </summary>
        private void IO_Completed(Object sender, SocketAsyncEventArgs args)
        {
            // 确定刚刚完成的操作类型并调用关联的处理程序
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(args);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(args);
                    break;
                default:
                    log.AddLog(LogType.Error, $"处理I/O事件时出错!\r\n错误信息:在套接字上完成的最后一个操作不是接收或发送!");
                    break;
            }
        }

        private void Args_Completed(Object sender, SocketAsyncEventArgs args)
        {
            ProcessAccept(args);
        }

        /// <summary>
        /// 线程响应
        /// </summary>
        /// <param name="args"></param>
        private void ProcessAccept(SocketAsyncEventArgs args)
        {
            try
            {
                Interlocked.Increment(ref _ClientCount);
                SocketAsyncEventArgs readEventArgs = _Pool.Pop();
                AsyncUserToken userToken = (AsyncUserToken)readEventArgs.UserToken;
                userToken.Socket = args.AcceptSocket;
                userToken.ConnectTime = DateTime.Now;
                userToken.Remote = args.AcceptSocket.RemoteEndPoint;
                userToken.IPAddress = ((IPEndPoint)args.AcceptSocket.RemoteEndPoint).Address;

                lock (_Clients) _Clients.Add(userToken); // 锁定线程添加对象

                ClientNumberChange?.Invoke(1, userToken); // 事件处理
                if (!args.AcceptSocket.ReceiveAsync(readEventArgs)) ProcessReceive(readEventArgs);
            }
            catch (Exception ex)
            {
                AsyncUserToken token = args.UserToken as AsyncUserToken;
                log.AddLog(LogType.Error, $"在线程响应时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
            if (args.SocketError == SocketError.OperationAborted) return;
            StartAccept(args);
        }

        /// <summary>
        /// 异步接收操作完成时调用此方法
        /// 如果远程主机关闭了连接，则该套接字被关闭
        /// 如果接收到数据，则数据将回传给客户端
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // 检查如果客户端关闭连接
                AsyncUserToken token = (AsyncUserToken)e.UserToken;// 获取用户信息
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)// 如果字节存在
                {
                    Byte[] data = new Byte[e.BytesTransferred];// 从套接字中获取数据
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    lock (token.Buffer)
                    {
                        token.Buffer.AddRange(data);// 锁定接收数据
                    }
                    //注意:你一定会问,这里为什么要用do-while循环? 
                    //如果当客户发送大数据流的时候,e.BytesTransferred的大小就会比客户端发送过来的要小,
                    //需要分多次接收.所以收到包的时候,先判断包头的大小.够一个完整的包再处理.
                    //如果客户短时间内发送多个小数据包时, 服务器可能会一次性把他们全收了.
                    //这样如果没有一个循环来控制,那么只会处理第一个包,
                    //剩下的包全部留在token.Buffer中了,只有等下一个数据包过来后,才会放出一个来.
                    do
                    {
                        byte[] lenBytes = token.Buffer.GetRange(0, 4).ToArray();// 获取头说明
                        int packageLen = BitConverter.ToInt32(lenBytes, 0);// 获取包长度
                        if (packageLen > token.Buffer.Count - 4)
                        {
                            break;// 长度不够时,退出循环,让程序继续接收
                        }
                        byte[] rev = token.Buffer.GetRange(4, packageLen).ToArray();// 包够长时,则提取出来,交给后面的程序去处理
                        lock (token.Buffer)
                        {
                            token.Buffer.RemoveRange(0, packageLen + 4);// 锁定线程并从数据池中移除这组数据
                        }
                        ReceiveClientData?.Invoke(token, rev); // 将数据包交给后台处理,这里你也可以新开个线程来处理.加快速度
                        //string msg = Encoding.UTF8.GetString(rev);
                        //server.lbxInfo.Items.Add(msg);// 输出信息
                        //这里API处理完后,并没有返回结果,当然结果是要返回的,却不是在这里, 这里的代码只管接收.
                        //若要返回结果,可在API处理中调用此类对象的SendMessage方法,统一打包发送.不要被微软的示例给迷惑了.
                    } while (token.Buffer.Count > 4);
                    //继续接收. 为什么要这么写,请看Socket.ReceiveAsync方法的说明
                    if (!token.Socket.ReceiveAsync(e)) this.ProcessReceive(e);
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            catch (Exception ex)
            {
                AsyncUserToken token = e.UserToken as AsyncUserToken;
                log.AddLog(LogType.Error, $"异步接收数据操作时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
        }

        /// <summary>
        /// 异步发送操作完成时调用此方法
        /// 该方法在套接字上发出另一个接收以读取任何附加信息
        /// 从客户端发送的数据
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = e.UserToken as AsyncUserToken;// 获取客户端信息
                try
                {
                    // 读取从客户端发送的下一个数据块
                    bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(e);// 如果数据不为空就发送给客户端
                    }
                }
                catch (Exception ex)
                {
                    log.AddLog(LogType.Error, $"在异步发送完成时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
                }
            }
            else { CloseClientSocket(e); }
        }

        /// <summary>
        /// 关闭客户端Socket
        /// </summary>
        /// <param name="args">异步套接字事件 对象</param>
        private void CloseClientSocket(SocketAsyncEventArgs args)
        {
            AsyncUserToken token = args.UserToken as AsyncUserToken; // 获取关联用户信息
            try
            {
                lock (_Clients) _Clients.Remove(token); // 锁定线程并移除用户对象
                ClientNumberChange?.Invoke(-1, token); // 根据对象清理掉那个用户
                try
                {
                    token.Socket.Shutdown(SocketShutdown.Send); // 关闭套接字连接
                }
                catch (Exception ex)
                {
                    log.AddLog(LogType.Error, $"在关闭客户端Socket时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
                }
                token.Socket.Close();
                Interlocked.Decrement(ref _ClientCount); // 递减计数器跟踪连接到服务器的客户端总数
                _MaxNumberAcceptedClients.Release();

                args.UserToken = new AsyncUserToken(); // 释放异步套接字事件,以便重用
                _Pool.Push(args);
            }
            catch (Exception ex)
            {
                log.AddLog(LogType.Error, $"在关闭客户端Socket2时出错!\r\n客户端地址:{token.IPAddress}\r\n错误信息:{ex.Message}");
            }
        }
        #endregion
    }
}