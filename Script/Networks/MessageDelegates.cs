namespace Networks
{
    /// <summary>
    /// 网络时间委托
    /// </summary>
    public delegate void NetTimerDelegate();
    /// <summary>
    /// 简单时间委托
    /// </summary>
    public delegate void SimpleTimerDelegate();

    /// <summary>
    /// 队列委托
    /// </summary>
    public delegate void QueueDataGroupDelegate();

    /// <summary>
    /// http请求结果委托
    /// </summary>
    /// <param name="cmd">接口名字</param>
    /// <param name="res">0:成功,其他值:失败</param>
    /// <param name="value">请求返回的结果</param>
    public delegate void HttpNetResultDelegate(string cmd, int res, string value);

    /// <summary>
    /// 数据变更委托
    /// </summary>
    /// <param name="data">数据</param>
    public delegate void DataTableUpdateDelegate(object data);
}