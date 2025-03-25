using System;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Export.Tools;
using System.Text.RegularExpressions;

using Debug = UnityEngine.Debug;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Export
{
    /// <summary>
    /// 录制功能2
    /// </summary>
    public class Record2
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// 配置文件位置
        /// </summary>
        public string INIPATH {
            get
            {
                var dllPath = Item.GetDllPath("Export");
                var fileInfo = new FileInfo(dllPath);
                return fileInfo.DirectoryName + "\\Export.ini";
            }
        }

        /// <summary>
        /// ffmpeg.exe位置
        /// </summary>
        private string ffmpegPath;

        /// <summary>
        /// ffmpeg.exe位置
        /// </summary>
        public string FFMPEGPATH
        {
            get
            {
                return ffmpegPath;
            }
        }

        /// <summary>
        /// 视频保存位置
        /// </summary>
        private string savePath;

        /// <summary>
        /// 读取ffmpeg.exe的位置
        /// </summary>
        private Regex readPath
        {
            get
            {
                return new Regex(@"ffmpeg=([^=;]+);");
            }
        }

        /// <summary>
        /// 录制进程
        /// </summary>
        private Process recordProcess;

        /// <summary>
        /// 初始化录制模块
        /// </summary>
        public Record2():this(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\output.mkv")
        {
            
        }

        /// <summary>
        /// 初始化录制模块
        /// </summary>
        /// <param name="savePath">录制文件保存位置</param>
        public Record2(string savePath)
        {
            this.savePath = savePath;
            ffmpegPath = "";
            var file = new FileInfo(INIPATH);
            if (file.Exists)
            {
                var fr = file.OpenRead();
                var sr = new StreamReader(fr);
                var match = readPath.Match(sr.ReadToEnd());
                if (match.Length>0)
                {
                    var ffmpeg = new FileInfo(match.Groups[1].Value);
                    if (ffmpeg.Exists)
                    {
                        ffmpegPath = ffmpeg.FullName;
                    }
                }
                sr.Close();
                fr.Close();
            }
            else
            {
                file.Create();
            }
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
        }

        /// <summary>
        /// 初始化录制文件
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="ffmpegPath"></param>
        public Record2(string savePath,string ffmpegPath)
        {
            this.savePath = savePath;
            this.ffmpegPath = ffmpegPath;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
            Save();
        }

        /// <summary>
        /// 设置ffmpeg.exe的位置
        /// </summary>
        /// <param name="ffmpegPath"></param>
        public void SetFFMPEG(string ffmpegPath)
        {
            this.ffmpegPath = ffmpegPath;
            Save();
        }

        /// <summary>
        /// 设置视频保存位置
        /// </summary>
        /// <param name="path"></param>
        public void SetPATH(string path)
        {
            this.savePath = path;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void Save()
        {
            var file = new FileInfo(INIPATH);
            if (string.IsNullOrWhiteSpace(ffmpegPath)==false)
            {
                if (file.Exists == false)
                {
                    file.Create();
                }
                var sw = new StreamWriter(file.OpenWrite());
                var value = new StringBuilder();
                value.Append("ffmpeg=");
                value.Append(ffmpegPath);
                value.Append(";");
                sw.Write(value.ToString());
                sw.Close();
            }
        }

        ///// <summary>
        ///// 开始录制
        ///// </summary>
        //public void Start()
        //{
        //    if (recordProcess!=null)
        //    {
        //        Debug.LogError("录制尚未停止!");
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(ffmpegPath))
        //    {
        //        Debug.LogError("没有找到ffmpeg.exe位置!");
        //        return;
        //    }

        //    // 创建新进程启动信息
        //    ProcessStartInfo startInfo = new ProcessStartInfo
        //    {
        //        FileName = ffmpegPath,
        //        Arguments = $" -f gdigrab -i desktop {savePath}",
        //        UseShellExecute = false,
        //        CreateNoWindow = true,
        //        RedirectStandardOutput = true,
        //        RedirectStandardError = true
        //    };

        //    Debug.Log(startInfo.FileName);
        //    Debug.Log(startInfo.Arguments);

        //    // 开始录制
        //    recordProcess = new Process { StartInfo = startInfo };
        //    recordProcess.Start();
        //}

        /// <summary>
        /// 开始录制
        /// </summary>
        public void Start()
        {
            if (recordProcess!=null)
            {
                Debug.LogError("录制尚未停止!");
                return;
            }

            if (string.IsNullOrEmpty(ffmpegPath))
            {
                Debug.LogError("没有找到ffmpeg.exe位置!");
                return;
            }

            // 创建新进程启动信息
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = false,
            };

            //判断文件是否已经存在
            var saveFile = new FileInfo(savePath);
            if (saveFile.Exists)
            {
                if (MessageBox.Show($"{savePath} 文件已经存在,是否删除?", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    saveFile.Delete();
                }
                else
                {
                    return;
                }
            }

            // 开始录制
            recordProcess = new Process { StartInfo = startInfo };
            recordProcess.Start();

            // 录制命令
            var recordValue = $"{ffmpegPath} -f gdigrab -i desktop {savePath}";
            Debug.Log(recordValue);
            recordProcess.StandardInput.WriteLine(recordValue);
        }

        /// <summary>
        /// 开始录制
        /// </summary>
        public void Start(string ffpath)
        {
            if (recordProcess != null)
            {
                Debug.LogError("录制尚未停止!");
                return;
            }

            if (string.IsNullOrEmpty(ffpath))
            {
                Debug.LogError("没有找到ffmpeg.exe位置!");
                return;
            }

            // 创建新进程启动信息
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = false,
            };

            //判断文件是否已经存在
            var saveFile = new FileInfo(savePath);
            if (saveFile.Exists)
            {
                if (MessageBox.Show($"{savePath} 文件已经存在,是否删除?", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    saveFile.Delete();
                }
                else
                {
                    return;
                }
            }

            // 开始录制
            recordProcess = new Process { StartInfo = startInfo };
            recordProcess.Start();

            // 录制命令
            var recordValue = $"{ffpath} -f gdigrab -i desktop {savePath}";
            Debug.Log(recordValue);
            recordProcess.StandardInput.WriteLine(recordValue);
        }

        ///// <summary>
        ///// 停止录制进程
        ///// </summary>
        //public void Stop()
        //{
        //    if (recordProcess!=null&&!recordProcess.HasExited)
        //    {
        //        string output = recordProcess.StandardOutput.ReadToEnd(); // 读取标准输出
        //        Debug.Log(output);

        //        recordProcess.Kill();
        //        recordProcess.WaitForExit();
        //    }
        //}

        /// <summary>
        /// 停止录制进程
        /// </summary>
        public void Stop()
        {
            if (recordProcess!=null&&!recordProcess.HasExited)
            {
                //string output = recordProcess.StandardOutput.ReadToEnd(); // 读取标准输出
                //Debug.Log(output);
                //recordProcess.StandardInput.Write(Convert.ToChar(3)); //尝试发送 Ctrl+C
                recordProcess.StandardInput.Write("\x03"); //尝试发送 Ctrl+C
                recordProcess.StandardInput.WriteLine();
                recordProcess.StandardInput.WriteLine("exit");
                recordProcess.Kill();
                recordProcess.WaitForExit();
                Thread.Sleep(15 * 1000);
                KillFFMPEG();
            }
        }

        /// <summary>
        /// 关闭ffmpeg
        /// </summary>
        public void KillFFMPEG()
        {
            // 获取所有名为 "ffmpeg" 的进程
            Process[] processes = Process.GetProcessesByName("ffmpeg");
            Console.WriteLine(processes.Length);

            // 遍历进程列表并终止它们
            foreach (Process process in processes)
            {
                try
                {
                    process.Kill();
                    Console.WriteLine($"Process {process.Id} has been terminated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error terminating process {process.Id}: {ex.Message}");
                }
            }
        }
    }
}