using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Video;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Runtime.InteropServices;
using Export.Tools;
using UnityEngine;

using Screen = System.Windows.Forms.Screen;
using Graphics = System.Drawing.Graphics;
using VideoCodec = Accord.Video.FFMPEG.VideoCodec;

namespace Export
{
    /// <summary>
    /// 录制功能
    /// </summary>
    public class Record
    {
        #region Win32 API
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
        IntPtr hdc, // handle to DC
        int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        #endregion
        #region DeviceCaps常量
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;
        #endregion
        #region 属性
        /// <summary>
        /// 获取屏幕分辨率当前物理大小
        /// </summary>
        public static Size WorkingArea
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                Size size = new Size();
                size.Width = GetDeviceCaps(hdc, HORZRES);
                size.Height = GetDeviceCaps(hdc, VERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }
        /// <summary>
        /// 当前系统DPI_X 大小 一般为96
        /// </summary>
        public static int DpiX
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int DpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                ReleaseDC(IntPtr.Zero, hdc);
                return DpiX;
            }
        }
        /// <summary>
        /// 当前系统DPI_Y 大小 一般为96
        /// </summary>
        public static int DpiY
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int DpiX = GetDeviceCaps(hdc, LOGPIXELSY);
                ReleaseDC(IntPtr.Zero, hdc);
                return DpiX;
            }
        }
        /// <summary>
        /// 获取真实设置的桌面分辨率大小
        /// </summary>
        public static Size DESKTOP
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                Size size = new Size();
                size.Width = GetDeviceCaps(hdc, DESKTOPHORZRES);
                size.Height = GetDeviceCaps(hdc, DESKTOPVERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }

        /// <summary>
        /// 获取宽度缩放百分比
        /// </summary>
        public static double ScaleX
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int t = GetDeviceCaps(hdc, DESKTOPHORZRES);
                int d = GetDeviceCaps(hdc, HORZRES);
                var ScaleX = (double)GetDeviceCaps(hdc, DESKTOPHORZRES) / (double)GetDeviceCaps(hdc, HORZRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return ScaleX;
            }
        }
        /// <summary>
        /// 获取高度缩放百分比
        /// </summary>
        public static double ScaleY
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                var ScaleY = (double)GetDeviceCaps(hdc, DESKTOPVERTRES) / (double)GetDeviceCaps(hdc, VERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return ScaleY;
            }
        }
        #endregion

        private ScreenCaptureStream screenStream;
        private IDisposable videoWriter;
        private DateTime startTime;
        /// <summary>
        /// 录制文件位置
        /// </summary>
        private string recordPath;

        /// <summary>
        /// 录制文件位置
        /// </summary>
        public string RecordPath
        {
            get
            {
                return recordPath;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Record() : this(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\output.mp4")
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path">录制文件位置</param>
        public Record(string path)
        {
            this.recordPath = path;
        }

        /// <summary>
        /// 设置录制路径
        /// </summary>
        /// <param name="path"></param>
        public void SetRecordPath(string path)
        {
            this.recordPath = path;
        }

        /// <summary>
        /// 开始录制
        /// </summary>
        public void Start()
        {
            // 创建视频文件写入器
            if (RecordWindow.FFMPEG)
            {
                videoWriter = new VideoFileWriter();
                ((VideoFileWriter)videoWriter).Open(recordPath, DESKTOP.Width, DESKTOP.Height, 10, VideoCodec.MPEG4, 4 * DESKTOP.Width * DESKTOP.Height);

                // 创建屏幕捕捉流
                screenStream = new ScreenCaptureStream(Screen.PrimaryScreen.Bounds);
                screenStream.FrameInterval = 100;    //设置截屏频率(毫秒)
                screenStream.NewFrame += new NewFrameEventHandler(screenStream_NewFrame);
                //screenStream.NewFrame += new EventHandler<Accord.Audio.NewFrameEventArgs>(audioScreen_NewFrame);
                screenStream.Start();

                //计时
                startTime = DateTime.Now; 
            }
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetScreen()
        {
            Screen main = Screen.PrimaryScreen;//获取主显示屏
            var ScreenArea = DESKTOP;
            var ret = new Bitmap(ScreenArea.Width, ScreenArea.Height);
            using (Graphics g = Graphics.FromImage(ret))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(ScreenArea.Width, ScreenArea.Height));
            }
            return ret;
        }

        private void screenStream_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            // 将每一帧写入视频文件
            //videoWriter.WriteVideoFrame(eventArgs.Frame);

            if (RecordWindow.FFMPEG)
            {
                ((VideoFileWriter)videoWriter).WriteVideoFrame(GetScreen());
                GC.Collect(); 
            }
        }

        //private void audioScreen_NewFrame(object sender, Accord.Audio.NewFrameEventArgs eventArgs)
        //{
        //    videoWriter.WriteAudioFrame(eventArgs.Signal.RawData);
        //}

        /// <summary>
        /// 停止录制
        /// </summary>
        public void Stop()
        {
            if (RecordWindow.FFMPEG)
            {
                // 停止屏幕捕捉流和视频文件写入器
                screenStream.SignalToStop();
                screenStream.WaitForStop();
                ((VideoFileWriter)videoWriter).Close();

                //记时
                var value = new StringBuilder();
                value.Append("录制时长:");
                value.Append(DateTime.Now - startTime);
                Debug.Log(value);

                //在explorer打开
                Item.UseCmd("explorer /select," + recordPath); 
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        ~Record()
        {
            if (screenStream != null)
            {
                Stop();
            }
        }
    }
}
