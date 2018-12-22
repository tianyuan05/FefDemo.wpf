using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FefDemo.wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CefSharp.CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharp.CefSharpSettings.WcfEnabled = true;
            Loaded += OnLoad;
            //this.Browser.RegisterAsyncJsObject("boundAsync", new JsObject(), new BindingOptions { CamelCaseJavascriptNames = false });
            JsObject jsObject = new JsObject();
            jsObject.Publisher += (sender, e) =>
            {
                Console.WriteLine(e);
            };
            this.Browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            {
                Console.WriteLine(e.ObjectName);
                if (!e.ObjectRepository.HasBoundObjects)
                {
                    e.ObjectRepository.Register("boundAsync", jsObject, isAsync: true, options: new BindingOptions
                    {
                        /*
                         * 坑：所有名称需要区分大小写 完全一致：
                         */
                        CamelCaseJavascriptNames = true
                    });
                }
            };

            this.Browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            {
                Console.WriteLine($"{e.ObjectName}+");
            };

            this.Browser.JavascriptObjectRepository.ObjectsBoundInJavascript += (sender, e) =>
            {
                Console.WriteLine(e.ObjectNames.ToString());
            };
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            this.Browser.Address = "https://localhost:5001";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
             * .net call JavaScript method :
             *      在.net中直接写Js method；
             *
             */

            string script = string.Format("document.body.style.background = '{0}'", "Red");
            this.Browser.GetMainFrame().ExecuteJavaScriptAsync(script);
        }

        // call js method by method-name
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<Person> persons = new List<Person> {
                new Person{ Name="张三",Age=18},
                new Person{Name="李四",Age=20}
            };
            string jsonString = JsonConvert.SerializeObject(new { type = "token", data = persons });
            Console.WriteLine(jsonString);
            if (Browser.CanExecuteJavascriptInMainFrame)
                this.Browser.GetMainFrame().ExecuteJavaScriptAsync("Test1('2')");
        }

        class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

        }
    }
}
