using Networks.interfaces;
using System;
using System.Collections.Generic;

namespace Networks.parser
{
    public class TableDataManager
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        public static readonly TableDataManager Instance = new TableDataManager();

        //当前请求返回的结果
        IServerResponseData _currentResponseData;
        Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

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
            Type type = null;
            if (typeDict.ContainsKey(tableName))
            {
                type = typeDict[tableName];
            }
            return type;
        }

        private TableDataManager()
        {
            RegisterBindingTableStrcut();
        }

        /// <summary>
        /// 注册绑定表格数据结构 （以下内容是sg项目组中的，其他项目组可以重新定义）
        /// </summary>
        protected virtual void RegisterBindingTableStrcut()
        {
            typeDict["ModuleProfile"] = typeof(ModuleProfile);
        }
    }
}
