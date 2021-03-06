﻿namespace Networks
{
    internal class NetTimerManager
    {
        /// <summary>
        /// 等待请求时间（单位秒）
        /// </summary>
        public float waitResponseTime = 5;

        /// <summary>
        /// 最大重发次数
        /// </summary>
        public uint resetSendMax = 3;

        /// <summary>
        /// 重新发送请求、网络超时
        /// </summary>
        public NetTimerDelegate resetSend;

        /// <summary>
        /// 网络超时
        /// </summary>
        public NetTimerDelegate netTimeOut;

        /// <summary>
        /// 当前次数
        /// </summary>
        uint _currentCount;

        /// <summary>
        /// 定时器
        /// </summary>
        SimpleTimer simpleTimer;

        //是否超时
        bool _isTimeOut;

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool isTimeOut
        {
            get { return _isTimeOut; }
        }

        public NetTimerManager()
        {
            simpleTimer = new SimpleTimer(waitResponseTime);
            simpleTimer.tick += TickHandler;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public void UpdateTime()
        {
            if (simpleTimer.isRunning) simpleTimer.Update(UnityEngine.Time.deltaTime);
        }

        /// <summary>
        /// 开始运行时间
        /// </summary>
        public void StartTime()
        {
            Running();
        }

        /// <summary>
        /// 停止运行时间
        /// </summary>
        public void StopTime()
        {
            simpleTimer.Stop();
        }

		/// <summary>
		/// 当前发送次数
		/// </summary>
		/// <value>The current count.</value>
        public uint currentCount
        {
            get { return _currentCount; }
        }

        /// <summary>
        /// 超时了，允许在次重发
        /// </summary>
        public void Reset() 
        {
            _isTimeOut = false;
            _currentCount = 1;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Clear() 
        {
            simpleTimer.Clear();
            _currentCount = 0;
        }

        void Running()
        {
            simpleTimer.Start();
        }

        void TickHandler()
        {
            _currentCount += 1;
            if (currentCount <= resetSendMax)
            {
                if (null != resetSend) resetSend();
                Running();
            }
            else
            {
                _isTimeOut = true;
                StopTime();
                if (null != netTimeOut) netTimeOut();
            }
        }
    }
}