using Networks.interfaces;
using System.Collections.Generic;
namespace Networks.parser
{
    /// <summary>
    /// 参数数据结构
    /// </summary>
    class PostData : IPostData
    {
        List<string> _commandId; //请求的接口名称列表
        string _url;  //请求地址,包括了接口及参数
        List<HttpNetResultDelegate> _resultBack;  //请求接口结果的回调委托列表

        public List<HttpNetResultDelegate> resultBack
        {
            get { return _resultBack; }
            set { _resultBack = value; }
        }

        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        public List<string> commandId
        {
            get { return _commandId; }
            set { _commandId = value; }
        }

        public override string ToString()
        {
            return string.Join(",", commandId.ToArray());
        }
    }
}
