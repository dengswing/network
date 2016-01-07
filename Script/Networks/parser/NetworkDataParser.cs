using Networks.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Networks.parser
{
    /// <summary>
    /// 数据解析
    /// </summary>
    public class NetworkDataParser : INetworkDataParse
    {
        /// <summary>
        /// 返回的code结构是成功的
        /// </summary>
        public const int RESPONSE_CODE_RESULT_SUCCESS = 0;

        long _serverTime; //服务器时间

        /// <summary>
        /// 服务器时间
        /// </summary>
        public long serverTime
        {
            get { return _serverTime; }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="netstringBuff"></param>
        /// <param name="serverMsg"></param>
        /// <returns></returns>
        public int ParseData(string netstringBuff, out string serverMsg)
        {
            IServerResponseData objServerResponseData = JsonDataManager.Instance.ParseJsonDataFromServer(netstringBuff);
            TableDataManager.Instance.currentResponseData = objServerResponseData;

            serverMsg = objServerResponseData.errMsg;
            _serverTime = objServerResponseData.serverTime;

            if (objServerResponseData.code == RESPONSE_CODE_RESULT_SUCCESS) AllDataChangeStruct(objServerResponseData);
            return objServerResponseData.code;
        }

        /// <summary>
        /// 所有数据转换成结构
        /// </summary>
        /// <param name="data"></param>
        void AllDataChangeStruct(IServerResponseData data)
        {
            //基础内容
            data.msgListTableStruct = new List<Dictionary<string, object>>();
            Dictionary<string, object> allTableData;
            foreach (var i in data.msgListData)
            {
                allTableData = TableChangeStruct(i);
                data.msgListTableStruct.Add(allTableData);
            }

            data.updataListTableStruct = TableChangeStruct(data.updateListData); //更新的内容
        }

        /// <summary>
        /// 根据数据库表格来转换结构
        /// </summary>
        /// <param name="jsonData">所有表格数据</param>
        /// <returns></returns>
        Dictionary<string, object> TableChangeStruct(Dictionary<string, object> jsonData)
        {
            Dictionary<string, object> allTableData = new Dictionary<string, object>(); //表结构 string表名、object表数据[按记录存]   
            foreach (var i in jsonData)
            {
                object tableStruct = TableStructConstructor(i.Key, i.Value);
                allTableData.Add(i.Key, tableStruct);
            }

            return allTableData;
        }

        /// <summary>
        /// 映射数据结构
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <param name="value">数据</param>
        /// <returns></returns>
        object TableStructConstructor(string tableName, object value)
        {
            Type table = TableDataManager.Instance.findTableTypeData(tableName);
            if (table == null) return StringChangeValue(value.ToString());  //数据结构不存在，直接返回值
            return MatchingTableStruct(tableName, value, table);
        }

        /// <summary>
        /// 表结构匹配，支持多条
        /// </summary>
        /// <param name="tableName">表格名称</param>
        /// <param name="value">数据</param>
        /// <param name="table">类结构体</param>
        /// <returns></returns>
        object MatchingTableStruct(string tableName, object value, Type classStruct)
        {
            if (value == "") return StringChangeValue(value.ToString()); //直接返回值

            List<Dictionary<string, string>> tableStruct = JsonDataManager.Instance.GetTableStruct(value); //转换json数据

            List<object> tableListData = new List<object>();

            foreach (var list in tableStruct)
            { //每条记录赋值
                object tableData = classStruct.GetConstructor(new Type[0]).Invoke(null);
                if (tableData == null)
                {//转换失败
                    UnityEngine.Debug.Log(tableName + " no exist!");
                    return null;
                }

                foreach (var data in list)
                { //每个字段赋值
                    SetFieldValue(tableData, data.Key, data.Value);
                }

                tableListData.Add(tableData);
            }

            if (tableListData.Count == 1)
                return tableListData[0]; //1条数据直接返回内容
            else
                return tableListData;
        }

        /// <summary>
        /// 设置表中的对象值
        /// </summary>
        /// <param name="classInstance">类</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="stringValue">内容</param>
        void SetFieldValue(object classInstance, string fieldName, string stringValue)
        {
            Type inst_type = classInstance.GetType();
            FieldInfo fieldInfo = inst_type.GetField(fieldName);

            Type underlying_type = Nullable.GetUnderlyingType(inst_type);
            Type value_type = underlying_type ?? inst_type;

            Type fieldType = null;
            if (null != fieldInfo)
            {
                fieldType = fieldInfo.FieldType;
                object obj = ConvertFromStr(stringValue.Trim(), fieldType);
                try
                {
                    fieldInfo.SetValue(classInstance, obj);
                }
                catch (Exception ex)
                { //类结构的属性和表字段不匹配了
                    UnityEngine.Debug.Log(value_type.FullName + ":" + fieldName + "=" + stringValue + " Error:" + ex.Message);
                }

                System.Reflection.MethodInfo mInfo = inst_type.GetMethod("ParseField");
                if (mInfo != null)
                {//触发ParseField方法来处理特殊字段。因为目前类和表只支持键值
                    mInfo.Invoke(classInstance, new object[] { fieldName, stringValue });
                }
            }
            else
            {
                UnityEngine.Debug.Log("fieldName:" + fieldName + " " + classInstance.GetType().ToString());
            }
        }

        /// <summary>
        /// 根据类型转换相应的数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        object ConvertFromStr(string value, Type valueType)
        {
            Type underlying_type = Nullable.GetUnderlyingType(valueType);
            Type value_type = underlying_type ?? valueType;
            if (value_type == typeof(string))
            {
                return value;
            }
            else if (value_type == typeof(bool))
            {
                bool n_bool = false;
                bool.TryParse(value, out n_bool);
                return n_bool;
            }
            else
            {
                return StringChangeValue(value);
            }
        }

        /// <summary>
        /// 把字符串转换成实际的类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        object StringChangeValue(string value, Type valueType = null)
        {
            if (value.IndexOf("{") >= 0 || value.IndexOf("[") >= 0)
            {
                object retObj = null;
                try
                {
                    if (null == valueType)
                        retObj = JsonConvert.DeserializeObject(value);
                    else
                        retObj = JsonConvert.DeserializeObject(value, valueType);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
                return retObj;
            }

            if (value.IndexOf('.') != -1 ||
                value.IndexOf('e') != -1 ||
                value.IndexOf('E') != -1)
            {

                double n_double;
                if (Double.TryParse(value, out n_double))
                {
                    return n_double;
                }
            }

            int n_int32;
            if (Int32.TryParse(value, out n_int32))
            {
                return n_int32;
            }

            long n_int64;
            if (Int64.TryParse(value, out n_int64))
            {
                return n_int64;
            }

            ulong n_uint64;
            if (UInt64.TryParse(value, out n_uint64))
            {
                return n_uint64;
            }

            bool n_bool;
            if (Boolean.TryParse(value, out n_bool))
            {
                return n_bool;
            }

            return null;
        }

    }
}