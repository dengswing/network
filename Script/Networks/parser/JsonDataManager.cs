using Networks.interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
namespace Networks.parser
{
    class JsonDataManager
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        public static readonly JsonDataManager Instance = new JsonDataManager();

        const string SERVER_RESPONSE_CODE = "code"; //处理结果
        const string SERVER_RESPONSE_MSG = "msg";   //内容
        const string SERVER_RESPONSE_GMT = "gmt";   //服务器时间
        const string SERVER_RESPONSE_UPDATE = "UPDATE"; //更新数据

        //错误key
        const int RESPONSE_CODE_ERROR = -999;
        
        /// <summary>
        /// 解析服务器通过 http response 传来的所有数据内容，也就是在浏览器中能看到的全部信息
        /// </summary>
        public IServerResponseData ParseJsonDataFromServer(string jsonDataFromServer)
        {
            JObject jObjectRoot = JObject.Parse(jsonDataFromServer);
            IServerResponseData serverData = ParseJsonToStructData(jObjectRoot);
            serverData.result = jsonDataFromServer;
            return serverData;
        }

        /// <summary>
        /// 表结构数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetTableStruct(object data)
        {
            List<Dictionary<string, string>> tableStructList = new List<Dictionary<string, string>>();
            JsonStructToKeyValue((JObject)data, tableStructList);
            return tableStructList;
        }

        private JsonDataManager()
        {

        }

        /// <summary>
        /// json数据包装成数据结构
        /// </summary>
        /// <param name="rootData">json数据</param>
        /// <returns></returns>
        IServerResponseData ParseJsonToStructData(JObject rootData)
        {
            IServerResponseData objServerResponseData = new ServerResponseData();

            JProperty jPropertyCode = null;
            JProperty jPropertyMsg = null;
            JProperty jPropertyUpdate = null;
            JProperty jPropertyGmt = null;
            foreach (JProperty jPropertyRootItem in rootData.Children())
            {
                if (SERVER_RESPONSE_CODE == jPropertyRootItem.Name)
                {
                    jPropertyCode = jPropertyRootItem;
                }
                else if (SERVER_RESPONSE_MSG == jPropertyRootItem.Name)
                {
                    jPropertyMsg = jPropertyRootItem;
                }
                else if (SERVER_RESPONSE_UPDATE == jPropertyRootItem.Name)
                {
                    jPropertyUpdate = jPropertyRootItem;
                }
                else if (SERVER_RESPONSE_GMT == jPropertyRootItem.Name)
                {
                    jPropertyGmt = jPropertyRootItem;
                }
            }

            if (jPropertyCode != null)
            {
                objServerResponseData.code = ParseCode(jPropertyCode);
                objServerResponseData.serverTime = long.Parse(jPropertyGmt.Value.ToString());
                if (0 == objServerResponseData.code)
                {
                    if (jPropertyMsg != null)
                    {
                        objServerResponseData.msgListData = ParseMsg(jPropertyMsg);
                    }

                    if (jPropertyUpdate != null)
                    {
                        objServerResponseData.updateListData = ParseUpdate(jPropertyUpdate);
                    }

                }
                else
                {
                    if (jPropertyMsg != null)
                    {
                        objServerResponseData.errMsg = jPropertyMsg.Value.ToString();
                    }
                }
            }

            return objServerResponseData;
        }

        /// <summary>
        /// json数据包装成键值对应
        /// </summary>
        /// <param name="tabel">表给数据</param>
        /// <param name="tableStructList">表格结构列表</param>
        void JsonStructToKeyValue(JObject tabel, List<Dictionary<string, string>> tableStructList)
        {
            foreach (JProperty jMsg in tabel.Children())
            {
                Dictionary<string, string> tableList = new Dictionary<string, string>();
                foreach (JObject jNode in jMsg.Children())
                {
                    JProperty firstChild = (JProperty)jNode.First;
                    if (IsSimpleValue(firstChild) || !IsNumberKey(firstChild))
                    { //2层结构
                        foreach (JProperty property in jNode.Children())
                        {
                            tableList.Add(property.Name, property.Value.ToString());
                        }
                    }
                    else
                    { //3层结构
                        JsonStructToKeyValue(jNode, tableStructList);
                        goto Found; //防止多生成一条数据
                    }
                }
                tableStructList.Add(tableList);
            }

        Found:
            return;
        }

        /// <summary>
        /// 解析code值
        /// </summary>
        /// <param name="jProperty"></param>
        /// <returns></returns>
        int ParseCode(JProperty jProperty)
        {
            int code = RESPONSE_CODE_ERROR;
            if (IsSimpleValue(jProperty))
            {
                code = Int32.Parse(jProperty.Value.ToString());
            }
            else
            {
                throw new Exception();
            }

            return code;
        }

        /// <summary>
        /// 解析msg值
        /// </summary>
        /// <param name="jPropertyMsgRoot"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> ParseMsg(JProperty jPropertyMsgRoot)
        {
            List<Dictionary<string, object>> msgListData = new List<Dictionary<string, object>>();

            foreach (JArray jMsgItem in jPropertyMsgRoot.Children())
            {
                foreach (JObject jMsgObjectRoot in jMsgItem)
                {
                    msgListData.Add(ParseJsonStruct(jMsgObjectRoot));
                }
            }

            return msgListData;
        }

        /// <summary>
        /// 解析update值
        /// </summary>
        /// <param name="jPropertyUpdateRoot"></param>
        /// <returns></returns>
        Dictionary<string, object> ParseUpdate(JProperty jPropertyUpdateRoot)
        {
            Dictionary<string, object> listData = null;
            foreach (JObject jPropertyObject in jPropertyUpdateRoot.Children())
            {
                listData = ParseJsonStruct(jPropertyObject);
            }

            return listData;
        }

        /// <summary>
        /// 解析json数据第一层，表名-->内容
        /// </summary>
        /// <param name="jPropertyObject"></param>
        /// <returns></returns>
        Dictionary<string, object> ParseJsonStruct(JObject jPropertyObject)
        {
            Dictionary<string, object> listData = new Dictionary<string, object>();

            foreach (JProperty jUpdateProperty in jPropertyObject.Children())
            {
                listData.Add(jUpdateProperty.Name, jUpdateProperty.Value);
            }

            return listData;
        }

        /// <summary>
        /// 判断是否简单值类型，即它的值应该不是对象，不是数组，而是 string 或 int，【如 "A" : abc】
        /// </summary>
        bool IsSimpleValue(JProperty jProperty)
        {
            bool isSimpleValue = false;

            if (jProperty.Count == 1)
            {
                JToken firstChild = jProperty.First;
                if (firstChild is JValue)
                {
                    isSimpleValue = true;
                }
            }

            return isSimpleValue;
        }

        /// <summary>
        /// 是否数字为key
        /// </summary>
        /// <param name="jprop"></param>
        /// <returns></returns>
        bool IsNumberKey(JProperty jprop)
        {
            if (jprop == null)
                return false;
            string str = jprop.Name;
            int i = 0; long k = 0;
            if (!int.TryParse(str, out i))
            {
                return long.TryParse(str, out k);
            }
            return true;
        }

    }
}
