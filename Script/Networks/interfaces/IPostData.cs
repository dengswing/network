using System.Collections.Generic;
namespace Networks.interfaces
{
    interface IPostData
    {
        /// <summary>
		/// 请求的接口名称列表
		/// </summary>
		/// <value>The command identifier.</value>
        List<string> commandId { get; set; }

		/// <summary>
		/// 请求地址,包括了接口及参数
		/// </summary>
		/// <value>The URL.</value>
        string url { get; set; }

		/// <summary>
		/// 请求接口结果的回调委托列表
		/// </summary>
		/// <value>The result back.</value>
        List<HttpNetResultDelegate> resultBack { get; set; }
    }
}
