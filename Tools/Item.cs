using Export.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static void ChoiceFile(ref string label, string tishi, Environment.SpecialFolder folder, string name)
        {
            var openDialog = new OpenFileDialog();

            openDialog.InitialDirectory = Environment.GetFolderPath(folder);
            openDialog.Filter = $"({name})|{name}";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;
            openDialog.Title = tishi;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                Log(openDialog.FileName);
                label = openDialog.FileName;
            }
        }

        /// <summary>
        /// 打印内容
        /// </summary>
        /// <param name="value"></param>
        private static void Log(string value)
        {
            UnityEngine.Debug.Log(value);
        }

        /// <summary>
        /// 选择指定文件
        /// </summary>
        /// <param name="label"></param>
        /// <param name="tishi">选择时候提示内容</param>
        /// <param name="folder">系统文件夹枚举项</param>
        /// <param name="name">限定文件</param>
        public static void ChoiceFile(ref string label, string tishi, string folder, string name)
        {
            var openDialog = new OpenFileDialog();

            openDialog.InitialDirectory = folder;
            openDialog.Filter = $"({name})|{name}";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;
            openDialog.Title = tishi;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                Log(openDialog.FileName);
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

        /// <summary>
        /// 打开网站|其他东西
        /// </summary>
        /// <param name="web">网址|地址</param>
        public static void OpenOnWindows(string web)
        {
            System.Diagnostics.Process.Start(web);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFile(string path)
        {
            var command = string.Format("explorer /select,{0}", path);
            UseCmd(command);
        }

        /// <summary>
        /// 根据INI内容创建regex对象
        /// </summary>
        /// <param name="iniValue"></param>
        /// <returns></returns>
        public static Regex CreateRegex(string iniValue)
        {
            if (iniValue == null)
            {
                return new Regex("");
            }
            var read = new Regex("^[\"']?([^\r\n]+)[\"']?$");
            var value = read.Match(iniValue);
            if (value.Success)
            {
                var regex = new Regex(value.Groups[1].ToString());
                return regex;
            }
            else
            {
                return new Regex(iniValue);
            }
        }

        /// <summary>
        /// 将regex对象转为ini内容
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static string RegexToIni(Regex regex)
        {
            var sb = new StringBuilder();
            sb.Append('"');
            sb.Append(regex.ToString());
            sb.Append('"');
            return sb.ToString();
        }

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <param name="tishi">输入框提示</param>
        /// <param name="value">输入框内已有内容</param>
        /// <param name="tishi">回调函数,在这里通过委托修改内容</param>
        /// <returns></returns>
        public static void GetInput(Action<string> callBack)
        {
            GetInput("请输入内容:", callBack);
        }

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <param name="tishi">输入框提示</param>
        /// <param name="value">输入框内已有内容</param>
        /// <param name="tishi">回调函数,在这里通过委托修改内容</param>
        /// <returns></returns>
        public static void GetInput(string tishi, Action<string> callBack)
        {
            GetInput(tishi, "", callBack);
        }

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <param name="tishi">输入框提示</param>
        /// <param name="value">输入框内已有内容</param>
        /// <param name="tishi">回调函数,在这里通过委托修改内容</param>
        /// <returns></returns>
        public static void GetInput(string tishi, string value, Action<string> callBack)
        {
            var form = GetForm("InputBox");
            if (form == null)
            {
                Task.Factory.StartNew(() =>
                {
                    System.Windows.Forms.Application.EnableVisualStyles();
                    System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                    var input = new InputBox(tishi, value);
                    System.Windows.Forms.Application.Run(input);

                    return input.value;
                }).ContinueWith(t =>
                {
                    var ret = t.Result;
                    // 输出用户输入到 Unity 控制台，或者根据需要处理用户输入
                    Log("用户输入内容: " + ret);
                    if (callBack != null && !string.IsNullOrEmpty(ret))
                    {
                        //在主线程上执行回调
                        //Dispatcher.Invoke(() => callBack(ret));
                        //不使用Dispatcher反而可以使用...
                        callBack(ret);
                    }
                });
            }
            else
            {
                form.ShowInTheCurrentScreenCenter();
                form.Activate();
                form.Focus();
            }
        }

        /// <summary>
        /// 检查窗口是否已经打开
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns></returns>
        public static Form GetForm(string formName)
        {
            foreach (Form openForm in System.Windows.Forms.Application.OpenForms)
            {
                if (openForm.Name == formName || openForm is InputBox)
                {
                    return openForm;
                }
            }
            return null;
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

        /// <summary>
        /// 将列表转为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<T>(this T[] objects, bool tf)
        {
            if (tf)
            {
                var ret = typeof(T).ToString();
                ret += "[" + objects.Length + "] { ";
                for (int i = 0; i < objects.Length; i++)
                {
                    ret += objects[i].ToString();
                    if (i < objects.Length - 1)
                    {
                        ret += ", ";
                    }
                }
                ret += " }";
                return ret;
            }
            else
            {
                return objects.ToString();
            }
        }

        /// <summary>
        /// 将列表转为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static string ToString<T>(this List<T> ts, bool tf)
        {
            if (tf == false)
            {
                return ts.ToString();
            }
            else
            {
                var ret = "List<" + typeof(T).ToString() + "> ";
                ret += "[" + ts.Count + "] { ";
                for (int i = 0; i < ts.Count; i++)
                {
                    ret += ts[i].ToString();
                    if (i < ts.Count - 1)
                    {
                        ret += ", ";
                    }
                }
                ret += " }";
                return ret;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Join<T>(this List<T> ts, string add)
        {
            var ret = new StringBuilder();
            for (int i = 0; i < ts.Count - 1; i++)
            {
                ret.Append(ts[i]);
                ret.Append(add);
            }
            ret.Append(ts[ts.Count - 1]);
            return ret.ToString();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Join<T>(this T[] ts, string add)
        {
            var ret = new StringBuilder();
            for (int i = 0; i < ts.Length - 1; i++)
            {
                ret.Append(ts[i]);
                ret.Append(add);
            }
            ret.Append(ts[ts.Length - 1]);
            return ret.ToString();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerator<T> ts, string add)
        {
            if (ts == null || !ts.MoveNext())
            {
                // 如果枚举器为null或没有元素，返回空字符串
                return string.Empty;
            }

            var ret = new StringBuilder();
            ret.Append(ts.Current); // 先添加第一个元素

            while (ts.MoveNext()) // 检查是否有更多元素
            {
                ret.Append(add); // 先添加分隔符
                ret.Append(ts.Current); // 再添加元素
            }

            return ret.ToString();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> ts, string add)
        {
            var ret = new StringBuilder();
            using (var enumerator = ts.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return string.Empty;  // 处理空集合
                }

                ret.Append(enumerator.Current);  // 添加第一个元素，避免在它之前添加分隔符

                while (enumerator.MoveNext())
                {
                    ret.Append(add);  // 在元素之间添加分隔符
                    ret.Append(enumerator.Current);
                }
            }

            return ret.ToString();
        }

        /// <summary>
        /// 对字典进行过滤
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Dictionary<Tkey, Tvalue> Filter<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Func<Tkey, Tvalue, bool> filter)
        {
            var ret = new Dictionary<Tkey, Tvalue>();
            foreach (var item in dict)
            {
                if (filter(item.Key, item.Value))
                {
                    ret.Add(item.Key, item.Value);
                }
            }
            return ret;
        }

        public static List<Tvalue> ValueList<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict)
        {
            var values = dict.Values;
            return new List<Tvalue>(values);
        }

        public static List<Tkey> KeyList<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict)
        {
            var keys = dict.Keys;
            return new List<Tkey>(keys);
        }

        /// <summary>
        /// 将列表进行转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="ts"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<R> Amplify<T, R>(this List<T> ts, Func<T, R> func)
        {
            var ret = new List<R>();

            ts.ForEach(t =>
            {
                ret.Add(func(t));
            });

            return ret;
        }

        /// <summary>
        /// 对List<T>的每个元素进行指定操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this List<T> ts, Action<T, int> action)
        {
            for (int i = 0; i < ts.Count; i++)
            {
                action.Invoke(ts[i], i);
            }
        }

        /// <summary>
        /// 反转当前这个列表顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> ReverseThis<T>(this List<T> ts)
        {
            ts.Reverse();
            return ts;
        }
    }
}
