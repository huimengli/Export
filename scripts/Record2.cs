using System;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Export.Tools;
using System.Text.RegularExpressions;

using Debug = UnityEngine.Debug;
using System.Text;

namespace Export
{
    /// <summary>
    /// ¼�ƹ���2
    /// </summary>
    public class Record2
    {
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
        public Record2():this(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\output.mp4")
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

        /// <summary>
        /// ��ʼ¼��
        /// </summary>
        public void Start()
        {
            if (recordProcess!=null)
            {
                Debug.LogError("¼����δֹͣ!");
            }

            if (string.IsNullOrEmpty(ffmpegPath))
            {
                Debug.LogError("û���ҵ�ffmpeg.exeλ��!");
            }

            // �����½���������Ϣ
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-f gdigrab -i desktop {savePath}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // ��ʼ¼��
            recordProcess = new Process { StartInfo = startInfo };
            recordProcess.Start();
        }

        /// <summary>
        /// ֹͣ¼�ƽ���
        /// </summary>
        public void Stop()
        {
            if (recordProcess!=null&&!recordProcess.HasExited)
            {
                recordProcess.Kill();
                recordProcess.WaitForExit();
            }
        }
    }
}