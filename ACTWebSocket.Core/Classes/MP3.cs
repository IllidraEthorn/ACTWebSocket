﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ACTWebSocket_Plugin.Classes
{
    public class MP3 : IDisposable
    {
        private string cmd, myHandle;
        private bool isOpen;
        private long ErrorNo = 0, length;

        public MP3(string path)
        {
            myHandle = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (File.Exists(path))
                MP3Open(path);
            else
                return;

            Thread trd = new Thread(new ThreadStart(autoDispose));
            trd.IsBackground = true;
            trd.Start();
        }

        private void autoDispose()
        {
            Thread.Sleep((int)(length) + 1000);
            MP3Close();
            Dispose();
        }

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public void MP3Close()
        {
            cmd = "close " + myHandle;
            mciSendString(cmd, null, 0, IntPtr.Zero);
            isOpen = false;
        }

        public void MP3Open(string sFileName)
        {
            cmd = $"open \"{sFileName}\" type mpegvideo alias " + myHandle;
            if ((ErrorNo = mciSendString(cmd, null, 0, IntPtr.Zero)) != 0) return;

            cmd = $"set {myHandle} time format milliseconds";
            if ((ErrorNo = mciSendString(cmd, null, 0, IntPtr.Zero)) != 0) return;

            cmd = $"set {myHandle} seek exactly on";
            if ((ErrorNo = mciSendString(cmd, null, 0, IntPtr.Zero)) != 0) return;

            StringBuilder str = new StringBuilder(128);
            cmd = $"status {myHandle} length";
            if ((ErrorNo = mciSendString(cmd, str, 128, IntPtr.Zero)) != 0) return;
            length = Convert.ToInt64(str.ToString());

            isOpen = true;
        }

        public void MP3Play(bool loop)
        {
            if (isOpen)
            {
                cmd = "play " + myHandle;
                if (loop)
                    cmd += " REPEAT";
                mciSendString(cmd, null, 0, IntPtr.Zero);
            }
        }
        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
