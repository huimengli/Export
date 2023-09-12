using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Export.Tools;

namespace Export
{
    class RecordWindow:EditorWindow
    {
        /// <summary>
        /// 录制位置
        /// </summary>
        private static string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\output.mp4";
        /// <summary>
        /// 录制模块
        /// </summary>
        private static Record record;
        /// <summary>
        /// FFMPEG模块是否加载
        /// </summary>
        public static readonly bool FFMPEG = Item.IsAssemblyLoaded("Accord.Video.FFMPEG");

        /// <summary>
        /// 录制
        /// </summary>
        [MenuItem("我的工具/录制... %g")]
        [STAThread]
        static void Record()
        {
            Debug.Log(Path);
            CreateInstance<RecordWindow>().Show();
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

        private void OnGUI()
        {
            //开始垂直线性布局
            GUILayout.BeginVertical();

            //录制存放位置
            GUILayout.BeginHorizontal();
            GUILayout.TextArea(Path);
            if (GUILayout.Button("..."))
            {
                Item.ChoiceFolder(ref Path, "选择保存文件夹");
                Path = Path += "\\output.mp4";
            }
            GUILayout.EndHorizontal();

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
                if (record!=null)
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

            GUILayout.EndVertical();
        }
    }
}
