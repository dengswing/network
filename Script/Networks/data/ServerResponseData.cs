using Networks.interfaces;
using System.Collections.Generic;
namespace Networks.parser
{
    /// <summary>
    /// 每次请求返回的数据
    /// </summary>
    class ServerResponseData : IServerResponseData
    {
        string _result; //原始返回结果
        int _code; //请求是否成功
        string _errMsg; //错误提示
        long _serverTime; //服务器时间
        List<Dictionary<string, object>> _msgListData;  //msg 的内容，每个协议包的MSG是LIST中的一个对象（Json原始数据，没有解析）
        Dictionary<string, object> _updateListData; //update 的内容（Json原始数据，没有解析）
        Dictionary<string, object> _updataListTableStruct; //根据表结构构建的数据 （update的内容）
        List<Dictionary<string, object>> _msgListTableStruct; //根据表结构构建的数据 （msg的内容）

        public string result
        {
            get { return _result; }
            set { _result = value; }
        }

        public int code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string errMsg
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }

        public long serverTime
        {
            get { return _serverTime; }
            set { _serverTime = value; }
        }

        public List<Dictionary<string, object>> msgListData
        {
            get { return _msgListData; }
            set { _msgListData = value; }
        }

        public Dictionary<string, object> updateListData
        {
            get { return _updateListData; }
            set { _updateListData = value; }
        }

        public Dictionary<string, object> updataListTableStruct
        {
            get { return _updataListTableStruct; }
            set { _updataListTableStruct = value; }
        }

        public List<Dictionary<string, object>> msgListTableStruct
        {
            get { return _msgListTableStruct; }
            set { _msgListTableStruct = value; }
        }
    }
}
