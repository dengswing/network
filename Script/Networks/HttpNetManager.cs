using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Networks.parser;
using Networks.interfaces;
using Networks.parser;

namespace Networks
{
    /// <summary>
    /// http请求
    /// </summary>
    public class HttpNetManager : MonoBehaviour
    {
        private static GameObject gameContainer = null;
        private static HttpNetManager _Instance;
        public static HttpNetManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<HttpNetManager>();
                    if (_Instance == null && gameContainer == null)
                    {
                        gameContainer = new GameObject();
                        gameContainer.name = "HttpNetManager";
                        gameContainer.AddComponent(typeof(HttpNetManager));
                    }

                    _Instance = FindObjectOfType<HttpNetManager>();
                    if (_Instance != null)
                        DontDestroyOnLoad(_Instance.gameObject);
                }
                return _Instance;
            }
        }

        /// <summary>
        /// 服务器异常
        /// </summary>
        public HttpNetResultDelegate serverErrorResponse;

        /// <summary>
        /// 网络超时
        /// </summary>
        public NetTimerDelegate netTimeOut;

        /// <summary>
        /// 请求地址
        /// </summary>
        public string requestURL = "http://dev-soul.shinezoneapp.com/?dev=jinfeifei&*=[{0}]";

        /// <summary>
        /// 接口及参数
        /// </summary>
        public string requestParams = "[\"{0}\",[{1}]]";

        /// <summary>
        /// 等待请求时间（单位秒）
        /// </summary>
        public float waitResponseTime = 5;

        /// <summary>
        /// 最大重发次数
        /// </summary>
        public uint resetSendMax = 3;

        /// <summary>
        /// 发送request最大组合接口包
        /// </summary>
        public uint requestGroupMax = 5;

        /// <summary>
        /// 请求回调委托
        /// </summary>
        Dictionary<string, HttpNetResultDelegate> onResponseDict = new Dictionary<string, HttpNetResultDelegate>();

        //用户id
        string _userID;

        //本地时间
        DateTime localTime = DateTime.Now;

        // 游戏服务器时间（秒为单位）
        long _serverTime;

        // 队列组装数据
        QueueDataGroupManager queueDataGroup;

        //定时器
        NetTimerManager netTimer;

        //数据解析
        INetworkDataParse netParser;


        /// <summary>
        /// 用户id
        /// </summary>
        public string userID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                if (null != queueDataGroup) queueDataGroup.userID = _userID;
            }
        }

        /// <summary>
        /// 注入数据解析类,不注入默认使用框架里面的
        /// </summary>
        /// <param name="dataParse">Data parse.</param>
        public void RegisterNetworkDataParse(INetworkDataParse dataParse)
        {
            netParser = dataParse;
        }

        /// <summary>
        /// 注册委托,数据请求不管成功还是失败都会触发消息通知
        /// </summary>
        /// <param name="commandID">根据后端的接口名称注册侦听</param>
        /// <param name="callback">委托回调方法</param>
        public void RegisterResponse(string commandID, HttpNetResultDelegate callback)
        {
            if (onResponseDict.ContainsKey(commandID))
                onResponseDict[commandID] += callback;
            else
                onResponseDict[commandID] = callback;
        }

        /// <summary>
        /// 移除委托
        /// </summary>
        /// <param name="commandID">根据后端的接口名称注册侦听</param>
        /// <param name="callback">委托回调方法</param>
        public void RemoveResponse(string commandID, HttpNetResultDelegate callback)
        {
            if (onResponseDict.ContainsKey(commandID))
                onResponseDict[commandID] -= callback;
        }

        /// <summary>
        /// 打包请求
        /// </summary>
        /// <param name="commandId">后端的接口名称</param>
        /// <param name="c">需要传递的参数</param>
        public void Post(string commandId, params object[] c)
        {
            List<object> args = new List<object>(c);
            queueDataGroup.AddRequest(commandId, args);
            SendRequest();
        }

        /// <summary>
        /// 打包请求
        /// </summary>
        /// <param name="commandId">后端的接口名称</param>
        /// <param name="resultBack">结果委托,报了系统级别错误不会有回调</param>
        /// <param name="c">需要传递的参数</param>
        public void Post(string commandId, HttpNetResultDelegate resultBack, params object[] c)
        {
            List<object> args = new List<object>(c);
            queueDataGroup.AddRequest(commandId, args, resultBack);
            SendRequest();
        }

        /// <summary>
        /// 启动重新在发送请求
        /// </summary>
        public void StartResetSend()
        {
            //超时了允许重发
            if (netTimer.isTimeOut)
            {
                netTimer.Reset();
                ResetSendRequest(false);
            }
            else
            {
                Debug.LogWarning("HttpNetManager::ResetSend :Error:No timeout, don't need to resend!");
            }
        }

        /// <summary>
        /// 清除所有未请求的数据
        /// </summary>
        public void Clear()
        {
            StopCoroutine("PostAsync");
            queueDataGroup.Clear();
            netTimer.Clear();
            Debug.LogWarning("HttpNetManager::ResetSend :Error:Remove all request!");
        }

        /// <summary>
        /// 一对一请求
        /// </summary>
        /// <param name="commandId">接口名称</param>
        /// <param name="c">参数</param>
        public void PostOneToOne(string commandId, params object[] c)
        {
            List<object> args = new List<object>(c);
            SendOneToOne(commandId, args, null, null);
        }

        /// <summary>
        /// 一对一请求
        /// </summary>
        /// <param name="commandId">接口名称</param>
        /// <param name="resultBack">回调委托</param>
        /// <param name="c">参数</param>
        public void PostOneToOne(string commandId, HttpNetResultDelegate resultBack, params object[] c)
        {
            List<object> args = new List<object>(c);
            SendOneToOne(commandId, args, resultBack, null);
        }

        /// <summary>
        /// 一对一请求
        /// </summary>
        /// <param name="commandId">接口名称</param>
        /// <param name="url">请求路径</param>
        /// <param name="c">参数</param>
        public void PostOneToOne(string commandId, string url, params object[] c)
        {
            List<object> args = new List<object>(c);
            SendOneToOne(commandId, args, null, url);
        }

        /// <summary>
        /// 一对一请求
        /// </summary>
        /// <param name="commandId">接口名称</param>
        /// <param name="url">请求路径</param>
        /// <param name="resultBack">回调委托</param>
        /// <param name="c">参数</param>
        public void PostOneToOne(string commandId, string url, HttpNetResultDelegate resultBack, params object[] c)
        {
            List<object> args = new List<object>(c);
            SendOneToOne(commandId, args, resultBack, url);
        }

        /// <summary>
        /// 发送一对一请求
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="args"></param>
        /// <param name="resultBack"></param>
        /// <param name="url"></param>
        void SendOneToOne(string commandId, List<object> args, HttpNetResultDelegate resultBack, string url)
        {
            if (url == null) url = requestURL;
            string URL = queueDataGroup.OneToOnePostDataPack(commandId, args, serverTime, url);
            StartCoroutine(PostSingle(commandId, URL, resultBack));
        }

        void Awake()
        {
            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
        }

        void Update()
        {
            if (netTimer != null) netTimer.UpdateTime(); //更新定时器时间
        }

        void Init()
        {
            queueDataGroup = new QueueDataGroupManager();
            queueDataGroup.requestGroupMax = requestGroupMax;
            queueDataGroup.userID = userID;
            queueDataGroup.requestURL = requestURL;
            queueDataGroup.requestParams = requestParams;

            netTimer = new NetTimerManager();
            netTimer.resetSend = ResetSend;
            netTimer.netTimeOut = NetTimeOut;
            netTimer.waitResponseTime = waitResponseTime;
            netTimer.resetSendMax = resetSendMax;
        }

        /// <summary>
        /// 游戏服务器当前时间（秒为单位）
        /// </summary>
        long serverTime
        {
            get
            {
                TimeSpan tSpan = DateTime.Now.Subtract(localTime);
                return _serverTime + (long)tSpan.TotalSeconds;
            }
            set
            {
                _serverTime = value;
                localTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        void SendRequest()
        {
            if (!queueDataGroup.isCanSend) return; //没有请求

            IPostData postData = queueDataGroup.GroupPostDataPack(serverTime);
            if (postData == null) return;
            StartCoroutine(PostAsync(postData));
        }

        void ResetSendRequest(bool isReset = true)
        {
            IPostData postData = queueDataGroup.currentPostData;
            if (postData == null)
            {
                Debug.LogWarning("HttpNetManager::ResetSend :Error:No data can be sent!");
                return;
            }

            Debug.LogWarning("HttpNetManager::ResetSend :reset send commandId:[" + postData.ToString() + "]" + isReset);
            StartCoroutine(PostAsync(postData, isReset));
        }

        /// <summary>
        /// 重发起请求
        /// </summary>
        void ResetSend()
        {
            ResetSendRequest();
        }

        /// <summary>
        /// 超时
        /// </summary>
        void NetTimeOut()
        {
            StopCoroutine("PostAsync");
            Debug.LogWarning("HttpNetManager::NetTimeOut Error :network time out!");
            if (null != netTimeOut) netTimeOut();
        }

        /// <summary>
        /// 请求http
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerator PostAsync(IPostData data, bool isReset = false)
        {
            if (netTimer.isTimeOut) yield break;    //超时退出请求

            if (!isReset) netTimer.StartTime(); //计时开始

            string url = data.url;
            Debug.Log(">>:" + url);
            WWW www = new WWW(url);
            yield return www;

            // yield break;    //测试超时

            Debug.Log("<< [" + data.ToString() + "]:" + www.text);
            netTimer.StopTime(); //计时停止

            if (netTimer.isTimeOut) yield break;    //超时退出请求,防止超时了还回来数据

            string result = www.text;
            int res = -1;
            string errMsg = "";
            try
            {
                if (netParser == null)
                {
                    netParser = new NetworkDataParser();
                }
                res = netParser.ParseData(result, out errMsg);
                serverTime = netParser.serverTime;
            }
            catch (Exception e)
            {
                Debug.LogWarning("HttpNetManager::PostAsync Error : commandId:[" + data.ToString() + "]error:" + e.Message + " Trace [" + e.StackTrace + "]");
                TriggerResponse(data, res, result, errMsg, true);
                yield break;
            }

            TriggerResponse(data, res, result, errMsg);

            if (res == NetworkDataParser.RESPONSE_CODE_RESULT_SUCCESS && queueDataGroup.NextSendRequest)
            { //进入下一组
                SendRequest();
            }
        }

        /// <summary>
        /// 触发回调委托
        /// </summary>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <param name="result"></param>
        /// <param name="errMsg"></param>
        /// <param name="isError"></param>
        void TriggerResponse(IPostData data, int res, string result, string errMsg, bool isError = false)
        {
            int count = data.commandId.Count;
            string commandId;
            HttpNetResultDelegate resultBack;
            for (int i = 0; i < count; i += 1)
            {
                commandId = data.commandId[i];

                if (onResponseDict.ContainsKey(commandId))
                {
                    onResponseDict[commandId](commandId, res, result);
                }

                if (!isError)
                {
                    resultBack = data.resultBack[i];
                    if (resultBack != null)
                    {
                        resultBack(commandId, res, result);
                    }
                }
            }

            if (serverErrorResponse != null && res != NetworkDataParser.RESPONSE_CODE_RESULT_SUCCESS)
            {
                Debug.LogWarning("HttpNetManager::TriggerResponse Error : commandId:[" + data.ToString() + "]error:" + errMsg);
                serverErrorResponse(errMsg, res, result);
            }
        }

        /// <summary>
        /// 请求http,单一发送，不需求得到结果处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerator PostSingle(string commandId, string url, HttpNetResultDelegate resultBack)
        {
            Debug.Log("singlePost>>:" + url);
            WWW www = new WWW(url);
            yield return www;
            if (resultBack != null)
            {
                resultBack(commandId, NetworkDataParser.RESPONSE_CODE_RESULT_SUCCESS, www.text);
            }

            Debug.Log("singlePost<< " + url + ":" + www.text);
        }
    }
}