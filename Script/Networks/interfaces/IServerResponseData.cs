using System.Collections.Generic;
namespace Networks.interfaces
{
    public interface IServerResponseData
    {
        /// <summary>
        /// 原始返回结果
        /// </summary>
        string result { get; set; }

        /// <summary>
        /// code 的内容
        /// </summary>
        int code { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        string errMsg { get; set; }

        /// <summary>
        /// 服务器时间
        /// </summary>
        long serverTime { get; set; }

        /// <summary>
        /// msg 的内容，每个协议包的MSG是LIST中的一个对象（Json原始数据，没有解析）
        /// </summary>
        List<Dictionary<string, object>> msgListData { get; set; }

        /// <summary>
        /// update 的内容（Json原始数据，没有解析）
        /// </summary>
        Dictionary<string, object> updateListData { get; set; }

        /// <summary>
        /// 根据表结构构建的数据 （update的内容）
        /// </summary>
        Dictionary<string, object> updataListTableStruct { get; set; }

        /// <summary>
        /// 根据表结构构建的数据 （msg的内容）
        /// </summary>
        List<Dictionary<string, object>> msgListTableStruct { get; set; }
    }
}
