namespace Networks.interfaces
{
    public interface INetworkDataParse
    {
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="netstringBuff">请求数据结构</param>
        /// <param name="serverMsg">服务器错误提示</param>
        /// <returns>是否成功</returns>
        int ParseData(string netstringBuff, out string serverMsg);

        /// <summary>
        /// 服务器时间
        /// </summary>
        long serverTime { get; }
    }
}
