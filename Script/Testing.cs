using System;
using UnityEngine;
using Networks;
using Networks.parser;



class Testing : MonoBehaviour
{
    public StatusType statusType;

    HttpNetManager httpNetwork;
    void Start()
    {
        httpNetwork = HttpNetManager.Instance;
        httpNetwork.userID = "77"; //用户id
        httpNetwork.statusType = statusType;

        httpNetwork.RegisterResponse("game.login", ResponseHandler);  //单个接口的侦听
		httpNetwork.serverErrorResponse = ServerErrorHandler;
        httpNetwork.netTimeOut = NetTimeOutHandler;  //网络超时

        httpNetwork.Post("game.reset", ResponseHandler);
        httpNetwork.Post("game.login", ResponseHandler);  //单一侦听,报了系统级别错误不会有回调
        httpNetwork.Post("game.login", ResponseHandler);
        httpNetwork.Post("cityOrder.list", ResponseHandler);
        httpNetwork.Post("game.login", ResponseHandler);

        //测试单一的请求
      //  httpNetwork.PostOneToOne("game.login");
      //  httpNetwork.PostOneToOne("game.login", PostOneToOneHandler);
       // httpNetwork.PostOneToOne("game.login", "Http://test.com/&*={0}");
       // httpNetwork.PostOneToOne("game.login", "Http://test.com/&*={0}", PostOneToOneHandler);
    }



    void ResponseHandler(string cmd, int res, string value)
    {
        Debug.Log("Testing:ResponseHandler:response:" + cmd + "|" + res);
		//Debug.Log(TableDataManager.Instance.currentResponseData);
    }

    void ServerErrorHandler(string cmd, int res, string value)
    {
        Debug.Log("Testing:ServerErrorHandler:response:" + cmd + "|" + res);
    }

    void NetTimeOutHandler() 
    {
        Debug.Log("Testing:NetTimeOutHandler: request timeOut!");
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            httpNetwork.Clear(true); //重置
        }
    }

}
