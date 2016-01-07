using System;
using UnityEngine;
using Networks;
using Networks.parser;

class Testing : MonoBehaviour
{

    HttpNetManager httpNetwork;
    void Start()
    {
        httpNetwork = HttpNetManager.Instance;
        httpNetwork.requestURL = "http://dev-soul.shinezoneapp.com/?&*=[{0}]"; //请求地址
        httpNetwork.requestParams = "[\"{0}\",[{1}]]"; //参数组装
        httpNetwork.userID = "77"; //用户id
        httpNetwork.waitResponseTime = 3;
        httpNetwork.resetSendMax = 2;

        httpNetwork.RegisterResponse("game.login", ResponseHandler);  //单个接口的侦听
		httpNetwork.serverErrorResponse = ServerErrorHandler;
        httpNetwork.netTimeOut = NetTimeOutHandler;  //网络超时

		httpNetwork.Post("game.login", ResponseHandler);  //单一侦听,报了系统级别错误不会有回调
		httpNetwork.Post("game.login", ResponseHandler);
        httpNetwork.Post("game.reset", ResponseHandler);
        httpNetwork.Post("cityOrder.list", ResponseHandler);
        httpNetwork.Post("game.login", ResponseHandler);


        httpNetwork.PostOneToOne("game.login");
        httpNetwork.PostOneToOne("game.login", PostOneToOneHandler);
        httpNetwork.PostOneToOne("game.login", "Http://test.com/&*={0}");
        httpNetwork.PostOneToOne("game.login", "Http://test.com/&*={0}", PostOneToOneHandler);
    }

    void ResponseHandler(string cmd, int res, string value)
    {
        //Debug.Log("Testing:ResponseHandler:response:" + cmd + "|" + res);

		//Debug.Log(TableDataManager.Instance.currentResponseData);
    }

    void ServerErrorHandler(string cmd, int res, string value)
    {
       // Debug.Log("Testing:ServerErrorHandler:response:" + cmd + "|" + res);
    }

    void NetTimeOutHandler() 
    {
       // Debug.Log("Testing:NetTimeOutHandler: request timeOut!");
    }

    void PostOneToOneHandler(string cmd, int res, string value)
    {
        Debug.Log("Testing:PostOneToOneHandler:response:" + cmd + "|" + res);
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            httpNetwork.StartResetSend(); //超时了，重新在发一轮
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            httpNetwork.Clear(); //重置
        }
    }

}
