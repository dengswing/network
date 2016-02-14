namespace Networks.interfaces
{
    public interface IDataTablePull
    {
        /// <summary>
        /// 表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        System.Type findTableTypeData(string tableName);

        /// <summary>
        /// 当前请求数据
        /// </summary>
        IServerResponseData currentResponseData { get; set; }

        /// <summary>
        /// 数据变更通知
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="data">数据</param>
        void FireNotice(string tableName, object data);       

        /// <summary>
        /// 添加表数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        void AddTableData(string tableName, object data);

        //===================================================================================
        //===================================================================================

        /// <summary>
        /// 返回表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        T GetTableData<T>(string tableName);

        /// <summary>
        /// 返回表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        System.Collections.Generic.List<T> GetTableDataList<T>(string tableName);

        /// <summary>
        /// 侦听表更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="updateBack"></param>
        void AddListenerDataTable(string tableName, DataTableUpdateDelegate updateBack);

        /// <summary>
        /// 移除表更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="updateBack"></param>
        void RemoveListenerDataTable(string tableName, DataTableUpdateDelegate updateBack);
    }
}
