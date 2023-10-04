using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace Export.Tools
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Item
    {
        /// <summary>
        /// 选择的文件夹路径
        /// </summary>
        private static string ChousePath;

        /// <summary>
        /// SHA256签名
        /// (不适用于签名中文内容,中文加密和js上的加密不同)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = MD5CryptoServiceProvider.Create().ComputeHash(bytes);

            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5(byte[] data)
        {
            var hash = MD5CryptoServiceProvider.Create().ComputeHash(data);

            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 新建一个UID
        /// </summary>
        /// <returns></returns>
        public static string NewUUID()
        {
            return NewUUID(DateTime.Now.ToJSTime().ToString());
        }

        /// <summary>
        /// 新建一个UID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NewUUID(string input)
        {
            input = MD5(input);
            var sha = SHA256(input);
            var uuid = "uuxxxuuy-1xxx-7xxx-yxxx-xxx0xxxy";
            var temp = new StringBuilder();
            var dex = new StringBuilder();
            for (int i = 0; i < uuid.Length; i++)
            {
                var e = uuid[i];
                if (e == 'u')
                {
                    temp.Append(sha[2 * i]);
                    dex.Append(sha[2 * i]);
                }
                else if (e == 'x')
                {
                    temp.Append(input[i]);
                    dex.Append(input[i]);
                }
                else if (e == 'y')
                {
                    temp.Append(MD5(dex.ToString())[i]);
                }
                else
                {
                    temp.Append(e);
                }
            }
            uuid = temp.ToString();
            return uuid;
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="tishi">选择时候提示内容</param>
        public static void ChoiceFolder(ref string label, string tishi)
        {
            ChoiceFolder(ref label, tishi, Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="tishi">选择时候提示内容</param>
        /// <param name="path">已经存在的文件路径</param>
        public static void ChoiceFolder(ref string label, string tishi, string path)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = tishi;
            dialog.ShowNewFolderButton = true;
            if (path != String.Empty && path != null)
            {
                dialog.SelectedPath = path;
            }
            //else
            //{
            //    dialog.SelectedPath = dialog.SelectedPath + "\\Hinterland\\TheLongDark";
            //}
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    return;
                }
                //this.LoadingText = "处理中...";
                //this.LoadingDisplay = true;
                //Action<string> a = DaoRuData;
                //a.BeginInvoke(dialog.SelectedPath, asyncCallback, a);
                path = dialog.SelectedPath;
            }
            label = path;
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="tishi">选择时候提示内容</param>
        /// <param name="folder">系统文件夹枚举项</param>
        public static void ChoiceFolder(ref string label, string tishi, Environment.SpecialFolder folder)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = tishi;
            dialog.ShowNewFolderButton = true;
            //dialog.RootFolder = folder;
            dialog.SelectedPath = Environment.GetFolderPath(folder);
            var path = dialog.SelectedPath;
            if (ChousePath != String.Empty && ChousePath != null)
            {
                dialog.SelectedPath = ChousePath;
            }
            //else
            //{
            //    dialog.SelectedPath = dialog.SelectedPath + "\\Hinterland\\TheLongDark";
            //}
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    return;
                }
                //this.LoadingText = "处理中...";
                //this.LoadingDisplay = true;
                //Action<string> a = DaoRuData;
                //a.BeginInvoke(dialog.SelectedPath, asyncCallback, a);
                ChousePath = dialog.SelectedPath;
            }
            else if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                ChousePath = path;
            }
            label = ChousePath;
        }

        /// <summary>
        /// 选择指定文件
        /// </summary>
        /// <param name="label"></param>
        /// <param name="tishi">选择时候提示内容</param>
        /// <param name="folder">系统文件夹枚举项</param>
        /// <param name="name">限定文件</param>
        public static void ChoiceFile(ref string label,string tishi,Environment.SpecialFolder folder,string name)
        {
            var openDialog = new OpenFileDialog();

            openDialog.InitialDirectory = Environment.GetFolderPath(folder);
            openDialog.Filter = $"({name})|{name}";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;
            openDialog.Title = tishi;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                UnityEngine.Debug.Log(openDialog.FileName);
                label = openDialog.FileName;
            }
        }

        /// <summary>
        /// 选择指定文件
        /// </summary>
        /// <param name="label"></param>
        /// <param name="tishi">选择时候提示内容</param>
        /// <param name="folder">系统文件夹枚举项</param>
        /// <param name="name">限定文件</param>
        public static void ChoiceFile(ref string label,string tishi,string folder,string name)
        {
            var openDialog = new OpenFileDialog();

            openDialog.InitialDirectory = folder;
            openDialog.Filter = $"({name})|{name}";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;
            openDialog.Title = tishi;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                UnityEngine.Debug.Log(openDialog.FileName);
                label = openDialog.FileName;
            }
        }

        /// <summary>
        /// 使用cmd命令
        /// </summary>
        /// <param name="cmdCode"></param>
        public static void UseCmd(string cmdCode)
        {
            System.Diagnostics.Process proIP = new System.Diagnostics.Process();
            proIP.StartInfo.FileName = "cmd.exe";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;
            proIP.Start();
            proIP.StandardInput.WriteLine(cmdCode);
            proIP.StandardInput.WriteLine("exit");
            string strResult = proIP.StandardOutput.ReadToEnd();
            Console.WriteLine(strResult);
            proIP.Close();
        }

        /// <summary>
        /// 判断dll是否能用
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static bool IsAssemblyLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name == assemblyName);
        }

        /// <summary>
        /// 获取dll位置
        /// 找不到会报错
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string GetDllPath(string assemblyName)
        {
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            return assembly.Location;
        }

        ///// <summary>
        ///// 获取输入
        ///// </summary>
        ///// <returns></returns>
        //public static Task<String> GetInput()
        //{
        //    var value = "";

        //    return Task.Factory.StartNew(() =>
        //    {
        //        System.Windows.Forms.Application.EnableVisualStyles();
        //        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        //        var input = new InputBox();
        //        System.Windows.Forms.Application.Run(input);

        //        value = input.value;
        //    }).ContinueWith(t =>
        //    {
        //        // 输出用户输入到 Unity 控制台，或者根据需要处理用户输入
        //        Debug.Log("用户输入内容: " + value);
        //        return value;
        //    }, TaskScheduler.FromCurrentSynchronizationContext());// 使用主线程的上下文来更新 UI 或其他主线程操作
        //}

        ///// <summary>
        ///// 获取输入
        ///// 需要.NET 4.5及以上
        ///// </summary>
        ///// <returns></returns>
        //public static async string GetInput()
        //{
        //    var value = "";

        //    value = await Task<string>.Factory.StartNew(() =>
        //    {
        //        System.Windows.Forms.Application.EnableVisualStyles();
        //        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        //        var input = new InputBox();
        //        System.Windows.Forms.Application.Run(input);

        //        return input.value;
        //    });

        //    return value;
        //}

        ///// <summary>
        ///// 获取输入
        ///// </summary>
        ///// <returns></returns>
        //public static string GetInput()
        //{
        //    var value = "";

        //    ItemAdd.Run(() =>
        //    {
        //        System.Windows.Forms.Application.EnableVisualStyles();
        //        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        //        var input = new InputBox();
        //        System.Windows.Forms.Application.Run(input);

        //        value = input.value;
        //    });

        //    return value;
        //}

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <returns></returns>
        public static void GetInput(Action<string> callBack)
        {
            Task.Factory.StartNew(() =>
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                var input = new InputBox();
                System.Windows.Forms.Application.Run(input);

                return input.value;
            }).ContinueWith(t =>
            {
                var value = t.Result;
                // 输出用户输入到 Unity 控制台，或者根据需要处理用户输入
                Debug.Log("用户输入内容: " + value);
                if (callBack!=null&&value!=null)
                {
                    //在主线程上执行回调
                    //Dispatcher.Invoke(() => callBack(value));
                    //不使用Dispatcher反而可以使用...
                    callBack(value);
                }
            });
        }
    }

    /// <summary>
    /// 工具类追加函数
    /// </summary>
    public static class ItemAdd
    {
        /// <summary>
        /// 获取js的时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToJSTime(this DateTime time)
        {
            var ret = time.ToFileTime();

            ret -= new DateTime(1970, 1, 1, 8, 0, 0).ToFileTime();
            //ret = Math.Floor(ret / 10000);
            ret = ret / 10000;

            return ret;
        }

        /// <summary>
        /// 异步运行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task Run(Action action)
        {
            return Task.Factory.StartNew(action);
        }
    }
}
