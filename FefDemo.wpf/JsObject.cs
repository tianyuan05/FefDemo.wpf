using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FefDemo.wpf
{
    public class JsObject
    {
        public event EventHandler<int> ExcuteChanged = delegate { };

        public event EventHandler<string> Publisher = delegate { };

        //Js调用有参有返回值的方法
        [DebuggerHidden]
        public int Div(int a, int b)
        {
            Console.WriteLine($"{a}++{b}");
            ExcuteChanged(this, a + b);
            return a + b;
        }


        public string Url()
        {
            return Address ?? @"http://www.baidu.com";
        }

        public string Address { get; set; }

        /// <summary>
        /// 有参无返回值
        /// </summary>
        /// <param name="message"></param>
        [DebuggerHidden]
        public void Write(string message)
        {
            Console.WriteLine($"this is .net {message}");
            JsResult<string> jsResul = JsonConvert.DeserializeObject<JsResult<string>>(message);
            Address = jsResul.Message;
            Console.WriteLine(jsResul.ToString());
            //Publisher(this, message.ToString()); //事件发布
        }

        //Js调用此方法,传递一个回调方法(Js中的方法),返回结果
        [DebuggerHidden]
        public void CallBack(IJavascriptCallback javascriptCallback)
        {
            //const int taskDelay = 1500; //没有看到是否一定要延迟

            Task.Run(async () =>
            {
                //await Task.Delay(taskDelay); //没有看到是否一定要延迟

                using (javascriptCallback)
                {
                    //NOTE: Classes are not supported, simple structs are
                    var respone = await javascriptCallback.ExecuteAsync(3, 5);

                    //string strResponse = JsonConvert.SerializeObject(respone);
                    //Response<int> responseJson = JsonConvert.DeserializeObject<Response<int>>(strResponse);
                    //Console.WriteLine(responseJson);

                    Console.WriteLine($"回调返回值：{respone.Result}  --- {respone.Message} --- {respone.Success}");
                }
            });
        }
    }

    public class JsResult<T>
    {
        public string Type { get; set; }

        public T Message { get; set; }

        public bool Result { get; set; }

        public override string ToString()
        {
            return $"{Type} - {Message} - {Result}";
        }
    }

    public class Response<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Result { get; set; }

        public override string ToString()
        {
            return $"{Success} - - {Message} - - {Result}";
        }

    }
}
