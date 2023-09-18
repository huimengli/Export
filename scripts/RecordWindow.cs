#define Record

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Export.Tools;
using System.Threading;

namespace Export
{
    class RecordWindow:EditorWindow
    {
        /// <summary>
        /// 录制位置
        /// </summary>
        private static string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\output.mkv";
#if !Record
        /// <summary>
        /// 录制模块
        /// </summary>
        private static Record record; 
#endif
#if !Record2
        /// <summary>
        /// 录制模块
        /// </summary>
        private static Record2 record2; 
#endif
        /// <summary>
        /// Accord.Video.FFMPEG模块是否加载
        /// </summary>
        public static readonly bool FFMPEG = Item.IsAssemblyLoaded("Accord.Video.FFMPEG");
        /// <summary>
        /// ffmpeg.exe模块是否加载
        /// </summary>
        //[Obsolete]
        public static readonly bool FFMPEGEXE = true;
        /// <summary>
        /// ffmpeg.exe模块位置
        /// </summary>
        public static string FFMPEGEXEPATH = "";
        /// <summary>
        /// 页面
        /// </summary>
        public static RecordWindow window;

        /// <summary>
        /// 录制
        /// </summary>
        [MenuItem("我的工具/录制... %g")]
        [STAThread]
        static void Record()
        {
            Debug.Log(Path);
            if (window==null)
            {
                window = CreateInstance<RecordWindow>();
            }
            window.Show();
        }

        /// <summary>
        /// 判断是否能录制
        /// </summary>
        /// <returns></returns>
        [MenuItem("我的工具/录制... %g",true)]
        [STAThread]
        static bool CanRecord()
        {
            return FFMPEG;
        }
        
        /// <summary>
        /// 录制
        /// </summary>
        [MenuItem("我的工具/录制2... %g")]
        [STAThread]
        static void Record2()
        {
            Debug.Log(Path);
            record2 = new Record2(Path);
            FFMPEGEXEPATH = record2.FFMPEGPATH;
            if (window == null)
            {
                window = CreateInstance<RecordWindow>();
            }
            window.Show();
        }

        /// <summary>
        /// 判断是否能录制
        /// </summary>
        /// <returns></returns>
        [MenuItem("我的工具/录制2... %g",true)]
        [STAThread]
        static bool CanRecord2()
        {
            return FFMPEGEXE;
        }

        private void OnGUI()
        {
            //开始垂直线性布局
            GUILayout.BeginVertical();

            //录制存放位置
            GUILayout.Label("录制文件保存位置:");
            GUILayout.BeginHorizontal();
            GUILayout.TextArea(Path);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                Item.ChoiceFolder(ref Path, "选择保存文件夹");
                Path = Path += "\\output.mkv";
            }
            GUILayout.EndHorizontal();

            #region 用Record.cs(Accord.Video.FFMPEG模块)录制
#if !Record
            //录制按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("开始录制"))
            {
                if (FFMPEG)
                {
                    record = new Record(Path);
                    record.Start();
                }
                else
                {
                    Debug.LogError("Accord.Video.FFMPEG.dll未加载或者无法使用!");
                }
            }
            if (GUILayout.Button("结束录制"))
            {
                if (record != null)
                {
                    record.Stop();
                    record = null;
                }
                else
                {
                    Debug.LogAssertion("录制尚未开启");
                }
            }
            GUILayout.EndHorizontal(); 
#endif
            #endregion

            #region 用Record2.cs(ffmpeg.exe)录制
#if !Record2
            //ffmpeg.exe位置
            GUILayout.Label("ffmpeg.exe存放位置:");
            GUILayout.BeginHorizontal();
            GUILayout.TextArea(FFMPEGEXEPATH);
            if (GUILayout.Button("...",GUILayout.Width(30)))
            {
                //Item.ChoiceFolder(ref FFMPEGEXEPATH, "选择ffmpeg.exe所在的位置");
                if (string.IsNullOrEmpty(FFMPEGEXEPATH))
                {
                    Item.ChoiceFile(ref FFMPEGEXEPATH, "选择ffmpeg.exe所在的位置", FFMPEGEXEPATH, "ffmpeg.exe");
                }
                else
                {
                    Item.ChoiceFile(ref FFMPEGEXEPATH, "选择ffmpeg.exe所在的位置", Environment.SpecialFolder.MyDocuments, "ffmpeg.exe");
                }
            }
            GUILayout.EndHorizontal();
            //录制按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("开始录制"))
            {
                if (string.IsNullOrEmpty(FFMPEGEXEPATH))
                {
                    Debug.LogAssertion("ffmpeg.exe未定位!");
                }
                else if (string.IsNullOrEmpty(Path))
                {
                    Debug.LogAssertion("录制位置未定位!");
                }
                else if (record2==null)
                {
                    record2 = new Record2(Path, FFMPEGEXEPATH);
                    record2.Start();
                    Debug.Log("开始录制");
                }
                else
                {
                    record2.SetPATH(Path);
                    record2.Start();
                    Debug.Log("开始录制");
                }
            }
            if (GUILayout.Button("结束录制"))
            {
                if (record2!=null)
                {
                    record2.Stop();
                    record2 = null;
                    Debug.Log("录制完成,等待文件写入");
                    //hread.Sleep(10 * 1000);
                    Item.UseCmd($"explorer /select,{Path}");
                    window.Close();
                }
                else
                {
                    Debug.LogAssertion("录制尚未开启");
                }
            }
            GUILayout.EndHorizontal();
#endif
            #endregion

            GUILayout.EndVertical();
        }
    }
}
