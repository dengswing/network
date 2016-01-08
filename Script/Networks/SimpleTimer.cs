namespace Networks
{
	internal class SimpleTimer
	{
        /// <summary>
        /// 一轮响应
        /// </summary>
        public event SimpleTimerDelegate tick;

		/// <summary>
        /// 是否在运转
		/// </summary> 
		private bool bRunning;
		
		/// <summary>
        /// 当前秒
		/// </summary>
		private float currentTime;
		
		/// <summary>
		/// 间隔时间
		/// </summary>
		private float spacingTime;
		
		/// <summary>
		/// 简单的定时器
		/// </summary>
		/// <param name="second">间隔响应时间，单位为秒</param>
		public SimpleTimer(float second)
		{
			currentTime = 0.0f;
			spacingTime = second;
		}
		
		/// <summary>
		/// Start Timer
		/// </summary>
		public void Start()
		{
			ResetStart();
		}

        //int k;
		/// <summary>
        /// 更新时间，使用继承了MonoBehaviour的类来更新
		/// </summary>
        /// <param name="deltaTime">Time.deltaTime</param>
		public void Update(float deltaTime)
		{
			if (bRunning)
			{
				currentTime += deltaTime;


                //if (UnityEngine.Mathf.FloorToInt(currentTime) != k)
                //{
                //    k = UnityEngine.Mathf.FloorToInt(currentTime);
                //    UnityEngine.Debug.Log("||==>" + k + "|" + currentTime + "|" + spacingTime);
                //}

				if (currentTime > spacingTime)
				{
                    Stop();
					if(null !=tick) tick();
				}
			}
		}
		
		/// <summary>
		/// 停止
		/// </summary>
		public void Stop()
		{
			bRunning = false;
		}
		
		/// <summary>
		/// 暂停
		/// </summary>
		public void Pause()
		{
            Stop();
		}
		
		/// <summary>
		/// 重新开始
		/// </summary>
		public void ResetStart()
		{
            Clear();
            bRunning = true;
		}

        /// <summary>
        /// 是否在运行
        /// </summary>
        public bool isRunning
        {
            get
            {
                return bRunning;
            }
        }

		/// <summary>
		/// 间隔时间调整
		/// </summary>
        /// <param name="second">间隔响应时间，单位为秒</param>
        public void ResetSpacingTime(float second)
		{
			spacingTime = second;
		}

        /// <summary>
        /// 重置
        /// </summary>
        public void Clear() 
        {
            currentTime = 0.0f;
            Stop();
        }
	}
}