using Networks.interfaces;
using System;
using System.Collections.Generic;

namespace Networks.parser
{
    /// <summary>
    /// 表结构数据管理
    /// </summary>
    public class TableDataManager : IDataTablePull
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        public static readonly TableDataManager Instance = new TableDataManager();

        //当前请求返回的结果
        IServerResponseData _currentResponseData;
        Dictionary<string, DataTableUpdateDelegate> dataTableListener = new Dictionary<string, DataTableUpdateDelegate>(); //数据表变更委托
        Dictionary<string, object> dataTableAll = new Dictionary<string, object>(); //所有的数据表数据
        AbsTableDataStruct _tableDataStruct;  //表结构数据

        /// <summary>
        /// 设置表结构
        /// </summary>
        public AbsTableDataStruct tableDataStruct
        {
            set
            { 
                _tableDataStruct = value;
                _tableDataStruct.RegisterBindingTableStrcut();
            }
        }

        /// <summary>
        /// 当前请求返回的结果
        /// </summary>
        public IServerResponseData currentResponseData
        {
            get { return _currentResponseData; }
            set { _currentResponseData = value; }
        }

        /// <summary>
        /// 获取对应的类
        /// </summary>
        /// <param name="tableName">数据库表名称</param>
        /// <returns></returns>
        public Type findTableTypeData(string tableName)
        {
            if (_tableDataStruct == null) return null;
            return _tableDataStruct.findTableTypeData(tableName);
        }

        /// <summary>
        /// 增加表格数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        public void AddTableData(string tableName, object data)
        {
            dataTableAll[tableName] = data;
            if (data == null) dataTableAll.Remove(tableName);
        }

        /// <summary>
        /// 返回数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public T GetTableData<T>(string tableName)
        {
            T data = default(T);
            if (dataTableAll.ContainsKey(tableName))
            {
                object obj;
                if (dataTableAll.TryGetValue(tableName, out obj))
                {
                    if (obj is T) data = (T)obj;
                }
            }

            return data;
        }

        /// <summary>
        /// 返回数据表（列表形式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<T> GetTableDataList<T>(string tableName)
        {
            List<T> data = default(List<T>);
            if (dataTableAll.ContainsKey(tableName))
            {
                object obj;
                List<object> listObj;
                if (dataTableAll.TryGetValue(tableName, out obj))
                {
                    if (obj is List<object>)
                    {
                        data = new List<T>();
                        listObj = (List<object>)obj;
                        listObj.ForEach(x => data.Add((T)x));
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        public void FireNotice(string tableName, object data)
        {
            if (dataTableListener.ContainsKey(tableName))
                dataTableListener[tableName](data);
        }

        /// <summary>
        /// 增加数据表委托
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="updateBack"></param>
        public void AddListenerDataTable(string tableName, DataTableUpdateDelegate updateBack)
        {
            if (dataTableListener.ContainsKey(tableName))
            {
                dataTableListener[tableName] -= updateBack;
                dataTableListener[tableName] += updateBack;
            }
            else
            {
                dataTableListener[tableName] = updateBack;
            }
        }

        /// <summary>
        /// 移除数据表委托
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="updateBack"></param>
        public void RemoveListenerDataTable(string tableName, DataTableUpdateDelegate updateBack)
        {
            if (dataTableListener.ContainsKey(tableName))
                dataTableListener[tableName] -= updateBack;
        }
    }
}
