using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanXingService_WMS.Utils.ThreadUtils
{
    public class MyTask
    {
        /// <summary>
        /// 取消任务令牌
        /// </summary>
        CancellationTokenSource cts = null;
        public Task task = null;
        /// <summary>
        /// 运行方法
        /// </summary>
        Action _beforeAction;

        /// <summary>
        /// 运行方法
        /// </summary>
        Action _runAction;
        /// <summary>
        /// 关闭方法
        /// </summary>
        Action _closeAction;

        bool _isAlwaysOn = false;

        int _waitTime = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runAction">执行方法</param>
        /// <param name="waitSecond">等待秒数，可以为小数，秒为单位</param>
        /// <param name="isAlwaysOn">是否循环执行</param>
        /// <param name="runAction">循环体执行前执行的方法</param>
        /// <param name="closeAction">关闭线程后继续执行的方法</param>
        public MyTask(Action runAction, decimal waitSecond, bool isAlwaysOn = false,
            Action beforeAction=null, Action closeAction =null)
        {
            _beforeAction = beforeAction;
            _runAction = runAction;
            _closeAction = closeAction;
            _waitTime = (int)(waitSecond*1000);
            _isAlwaysOn = isAlwaysOn;
        }

        public MyTask StartTask()
        {
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            task = Task.Run(() => {

                if(_beforeAction!=null)
                    _beforeAction();

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    try
                    {
                        _runAction();
                    }
                    catch(Exception ex)
                    {
                        Logger.Default.Process(new Log(LevelType.Error
                            , "执行失败\r\n"+ex.ToString()));
                    }
                    if (!_isAlwaysOn)
                        break;

                    Thread.Sleep(_waitTime);
                }
            }, token);
            return this;
        }

        public void CloseTask()
        {
            if(_closeAction!=null)
                _closeAction();
            if (cts != null)
                cts.Cancel();
        }
    }
}
