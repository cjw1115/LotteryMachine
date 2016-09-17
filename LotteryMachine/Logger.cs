using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LotteryMachine
{
    public static class Logger
    {
        private static readonly string logFileName = "log.file";
        private static StorageFile logFile;
        public static string LogFilePath
        {
            get { return logFile == null ? Windows.Storage.ApplicationData.Current.LocalFolder.Path : logFile.Path; }
        }
        static Logger()
        {
            Init();
        }
        public static async Task Init()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(logFileName);
            }
            catch(Exception ex)
            {
            }
            
            if (file == null)
            {
               file= await folder.CreateFileAsync(logFileName, CreationCollisionOption.ReplaceExisting);
            }
            logFile = file;
        }
        public static async void Log(string message)
        {
            if (logFile == null)
            {
                await Init();
            }
            using(var ras= await logFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
            {
                using(var stream = ras.AsStream())
                {
                    stream.Seek(0, SeekOrigin.End);
                    string log = $"{DateTime.Now}:{message}{Environment.NewLine}";
                    var buffer = Encoding.UTF8.GetBytes(log);
                    stream.Write(buffer, 0, buffer.Length);
                }
               
            }
        }
    }
}
