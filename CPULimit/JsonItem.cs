using System;
using Newtonsoft.Json;

namespace CPULimit
{

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 操作状态
        /// </summary>
        public String State { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 进程ID
        /// </summary>
        public Int32 PID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remake { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 暂停间隔时间
        /// </summary>
        public Int32 Timer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 暂停恢复时间
        /// </summary>
        public Int32 Pause { get; set; }
    }
}