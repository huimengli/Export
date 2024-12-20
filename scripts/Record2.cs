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
    /// ¼�ƹ���2
    /// </summary>
    public class Record2
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// �����ļ�λ��
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
        /// ffmpeg.exeλ��
        /// </summary>
        private string ffmpegPath;

        /// <summary>
        /// ffmpeg.exeλ��
        /// </summary>
        public string FFMPEGPATH
        {
            get
            {
                return ffmpegPath;
            }
        }

        /// <summary>
        /// ��Ƶ����λ��
        /// </summary>
        private string savePath;

        /// <summary>
        /// ��ȡffmpeg.exe��λ��
        /// </summary>
        private Regex readPath
        {
            get
            {
                return new Regex(@"ffmpeg=([^=;]+);");
            }
        }

        /// <summary>
        /// ¼�ƽ���
        /// </summary>
        private Process recordProcess;

        /// <summary>
        /// ��ʼ��¼��ģ��
        /// </summary>
        public Record2():this(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\output.mkv")
        {
            
        }

        /// <summary>
        /// ��ʼ��¼��ģ��
        /// </summary>
        /// <param name="savePath">¼���ļ�����λ��</param>
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
        /// ��ʼ��¼���ļ�
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
        /// ����ffmpeg.exe��λ��
        /// </summary>
        /// <param name="ffmpegPath"></param>
        public void SetFFMPEG(string ffmpegPath)
        {
            this.ffmpegPath = ffmpegPath;
            Save();
        }

        /// <summary>
        /// ������Ƶ����λ��
        /// </summary>
        /// <param name="path"></param>
        public void SetPATH(string path)
        {
            this.savePath = path;
        }

        /// <summary>
        /// ��������
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
        ///// ��ʼ¼��
        ///// </summary>
        //public void Start()
        //{
        //    if (recordProcess!=null)
        //    {
        //        Debug.LogError("¼����δֹͣ!");
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(ffmpegPath))
        //    {
        //        Debug.LogError("û���ҵ�ffmpeg.exeλ��!");
        //        return;
        //    }

        //    // �����½���������Ϣ
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

        //    // ��ʼ¼��
        //    recordProcess = new Process { StartInfo = startInfo };
        //    recordProcess.Start();
        //}

        /// <summary>
        /// ��ʼ¼��
        /// </summary>
        public void Start()
        {
            if (recordProcess!=null)
            {
                Debug.LogError("¼����δֹͣ!");
                return;
            }

            if (string.IsNullOrEmpty(ffmpegPath))
            {
                Debug.LogError("û���ҵ�ffmpeg.exeλ��!");
                return;
            }

            // �����½���������Ϣ
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = false,
            };

            //�ж��ļ��Ƿ��Ѿ�����
            var saveFile = new FileInfo(savePath);
            if (saveFile.Exists)
            {
                if (MessageBox.Show($"{savePath} �ļ��Ѿ�����,�Ƿ�ɾ��?", "����", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    saveFile.Delete();
                }
                else
                {
                    return;
                }
            }

            // ��ʼ¼��
            recordProcess = new Process { StartInfo = startInfo };
            recordProcess.Start();

            // ¼������
            var recordValue = $"{ffmpegPath} -f gdigrab -i desktop {savePath}";
            Debug.Log(recordValue);
            recordProcess.StandardInput.WriteLine(recordValue);
        }

        /// <summary>
        /// ��ʼ¼��
        /// </summary>
        public void Start(string ffpath)
        {
            if (recordProcess != null)
            {
                Debug.LogError("¼����δֹͣ!");
                return;
            }

            if (string.IsNullOrEmpty(ffpath))
            {
                Debug.LogError("û���ҵ�ffmpeg.exeλ��!");
                return;
            }

            // �����½���������Ϣ
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = false,
            };

            //�ж��ļ��Ƿ��Ѿ�����
            var saveFile = new FileInfo(savePath);
            if (saveFile.Exists)
            {
                if (MessageBox.Show($"{savePath} �ļ��Ѿ�����,�Ƿ�ɾ��?", "����", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    saveFile.Delete();
                }
                else
                {
                    return;
                }
            }

            // ��ʼ¼��
            recordProcess = new Process { StartInfo = startInfo };
            recordProcess.Start();

            // ¼������
            var recordValue = $"{ffpath} -f gdigrab -i desktop {savePath}";
            Debug.Log(recordValue);
            recordProcess.StandardInput.WriteLine(recordValue);
        }

        ///// <summary>
        ///// ֹͣ¼�ƽ���
        ///// </summary>
        //public void Stop()
        //{
        //    if (recordProcess!=null&&!recordProcess.HasExited)
        //    {
        //        string output = recordProcess.StandardOutput.ReadToEnd(); // ��ȡ��׼���
        //        Debug.Log(output);

        //        recordProcess.Kill();
        //        recordProcess.WaitForExit();
        //    }
        //}

        /// <summary>
        /// ֹͣ¼�ƽ���
        /// </summary>
        public void Stop()
        {
            if (recordProcess!=null&&!recordProcess.HasExited)
            {
                //string output = recordProcess.StandardOutput.ReadToEnd(); // ��ȡ��׼���
                //Debug.Log(output);
                //recordProcess.StandardInput.Write(Convert.ToChar(3)); //���Է��� Ctrl+C
                recordProcess.StandardInput.Write("\x03"); //���Է��� Ctrl+C
                recordProcess.StandardInput.WriteLine();
                recordProcess.StandardInput.WriteLine("exit");
                recordProcess.Kill();
                recordProcess.WaitForExit();
                Thread.Sleep(15 * 1000);
                KillFFMPEG();
            }
        }

        /// <summary>
        /// �ر�ffmpeg
        /// </summary>
        public void KillFFMPEG()
        {
            // ��ȡ������Ϊ "ffmpeg" �Ľ���
            Process[] processes = Process.GetProcessesByName("ffmpeg");
            Console.WriteLine(processes.Length);

            // ���������б���ֹ����
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