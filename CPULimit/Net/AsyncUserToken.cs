using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CPULimit.Net
{
    internal class AsyncUserToken : Object
    {
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        internal IPAddress IPAddress;
        /// <summary>
        /// 远程地址
        /// </summary>
        internal EndPoint Remote;
        /// <summary>
        /// 通信Socket
        /// </summary>
        internal Socket Socket;
        /// <summary>
        /// 连接时间
        /// </summary>
        internal DateTime ConnectTime;
        /// <summary>
        /// 所属用户信息
        /// </summary>
        internal UserInfoModel UserInfo;
        /// <summary>
        /// 数据缓存区
        /// </summary>
        internal List<Byte> Buffer;

        internal AsyncUserToken()
        {
            this.Buffer = new List<Byte>();
        }
    }
}