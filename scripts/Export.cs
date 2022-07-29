using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityFBXExporter;

/// <summary>
/// 导出模块
/// </summary>
namespace Export
{
    class ExportWindow : EditorWindow
    {
        /// <summary>
        /// 当前场景的所有对象
        /// </summary>
        private GameObject[] allGameObjects;

        /// <summary>
        /// 当前滚轮
        /// </summary>
        private Vector2 nowScroll = Vector2.zero;

        /// <summary>
        /// 忽视列表
        /// </summary>
        private List<string> IgnoreList = new List<string>();

        /// <summary>
        /// 初始化
        /// </summary>
        public ExportWindow()
        {
            IgnoreList = new List<string>
        {
            "Canvas",
            "Camera",
            "EventSystem",
            "Light"
        };
        }

        /// <summary>
        /// 读取所有物体
        /// </summary>
        private void AllGameObjects()
        {
            var scene = SceneManager.GetActiveScene();
            allGameObjects = scene.GetRootGameObjects();
        }

        /// <summary>
        /// 筛选部分物体
        /// </summary>
        private void SelectGameObjects()
        {
            AllGameObjects();
            allGameObjects = allGameObjects.RemoveByName(IgnoreList);
        }

        [MenuItem("我的工具/全部导出 %#e")]
        [STAThread]
        static void ExportNowScene()
        {
            var tool = new ExportWindow();
            tool.AllGameObjects();
            tool.Export(true, true);
        }

        [MenuItem("我的工具/导出... %e")]
        [STAThread]
        static void ExportNowSceneAndWindows()
        {
            //使用官方提供的实例化窗口方法调用
            ExportWindow.CreateInstance<ExportWindow>().Show();

            ////浮动性窗口,和点击building Setting出现的窗口一样
            //ExportWindow.CreateInstance<ExportWindow>().ShowUtility();

            ////此方法需要配合OnGUI使用,否则会出现页面无法关闭的情况
            //ExportWindow.CreateInstance<ExportWindow>().ShowPopup();
        }

        static void Test()
        {

        }

        private void OnGUI()
        {
            ////开始水平线布局
            //GUILayout.BeginHorizontal();
            ////结束水平线布局
            //GUILayout.EndHorizontal();

            //开始垂直线性布局
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("全选"))
            {
                AllGameObjects();
            }

            if (GUILayout.Button("筛选"))
            {
                SelectGameObjects();
            }

            GUILayout.EndHorizontal();

            nowScroll = GUILayout.BeginScrollView(nowScroll);

            try
            {
                for (int i = 0; i < allGameObjects.Length; i++)
                {
                    try
                    {
                        GUILayout.Label(i.ToString() + " " + allGameObjects[i].name);
                    }
                    catch (Exception err)
                    {
                        Debug.Log(err);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("导出"))
            {
                Export(false, false);
            }
            if (GUILayout.Button("带贴图导出"))
            {
                Export(true, false);
            }
            if (GUILayout.Button("带纹理导出"))
            {
                Export(true, true);
            }

            GUILayout.EndHorizontal();

            //结束垂直线性布局
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 导出当前场景
        /// </summary>
        /// <param name="copyMaterial">是否拷贝贴图</param>
        /// <param name="copyMaterial">是否拷贝纹理</param>
        public void Export(bool copyMaterial, bool copyTexture)
        {
            var divPath = Directory.GetCurrentDirectory();

            var tempDiv = divPath;
            if (Directory.Exists(tempDiv + "/Assets"))
            {
                tempDiv += "/Assets/tempOut";
                if (Directory.Exists(tempDiv))
                {
                    var files = new DirectoryInfo(tempDiv).GetFiles();
                    foreach (var file in files)
                    {
                        file.Delete();
                    }
                }
                else
                {
                    Directory.CreateDirectory(tempDiv);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "找不到Assets文件夹!", "确认");
                return;
            }

            divPath = EditorUtility.SaveFilePanel("导出场景对象", divPath, allGameObjects[0].name + ".fbx", "fbx");
            var temp = divPath.Split('/', '\\');
            divPath = temp.POP().Join("/");
            Debug.Log(divPath);
            if (divPath.Length > 3)
            {
                try
                {
                    for (int i = 0; i < allGameObjects.Length; i++)
                    {
                        try
                        {
                            if (File.Exists(tempDiv + "\\" + allGameObjects[i].name) == false)
                            {
                                //FBXExporter.ExportFBX(divPath, allGameObjects[i].name, new GameObject[1] { allGameObjects[i] }, true);
                                FBXExporter.ExportGameObjToFBX(allGameObjects[i], tempDiv + "\\" + allGameObjects[i].name + ".fbx", true, true);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    //throw;
                }
                finally
                {
                    DeleteMeta(tempDiv);
                    CopyFilesTo(tempDiv, divPath);
                    Directory.Delete(tempDiv, true);
                }
            }
        }

        /// <summary>
        /// 删除所有的Meta文件
        /// </summary>
        public bool DeleteMeta(string div)
        {
            if (Directory.Exists(div))
            {
                var floder = new DirectoryInfo(div);
                var files = floder.GetFiles();

                foreach (var file in files)
                {
                    var split = file.Name.Split('.');
                    var lastName = split[split.Length - 1];
                    if (lastName == "meta")
                    {
                        file.Delete();
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将文件拷贝
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public bool CopyFilesTo(string from, string to)
        {
            if (Directory.Exists(from))
            {
                if (Directory.Exists(to) == false)
                {
                    Directory.CreateDirectory(to);
                }
                var files = new DirectoryInfo(from).GetFiles();
                foreach (var file in files)
                {
                    file.CopyTo(to + "/" + file.Name, true);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 导出窗口追加函数
    /// </summary>
    public static class ExportWindowAdd
    {
        /// <summary>
        /// 删除最后一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] POP<T>(this T[] array)
        {
            if (array.Length == 0)
            {
                return array;
            }
            else
            {
                return array.ToList<T>().MyRemoveAt(array.Length - 1).ToArray();
            }
        }

        public static List<R> MyRemoveAt<R>(this List<R> list, int index)
        {
            list.RemoveAt(index);
            return list;
        }

        public static string Join(this string[] array, string add)
        {
            if (array.Length == 0)
            {
                return "";
            }

            var ret = new StringBuilder();

            for (int i = 0; i < array.Length - 1; i++)
            {
                ret.Append(array[i]);
                ret.Append(add);
            }

            ret.Append(array[array.Length - 1]);

            return ret.ToString();
        }

        /// <summary>
        /// 移除带有名称的对象
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static GameObject[] RemoveByName(this GameObject[] gameObjects, string[] names)
        {
            var ret = new List<GameObject>();
            var temp = new List<GameObject>();

            foreach (var x in gameObjects)
            {
                foreach (var y in names)
                {
                    if (new Regex(y).IsMatch(x.name))
                    {
                        temp.Add(x);
                    }
                }
            }

            foreach (var x in gameObjects)
            {
                if (temp.IndexOf(x) < 0)
                {
                    ret.Add(x);
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// 移除带有名称的对象
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="names"></param>
        /// <returns></returns>

        public static GameObject[] RemoveByName(this GameObject[] gameObjects, List<string> names)
        {
            return gameObjects.RemoveByName(names.ToArray());
        }
    }

}