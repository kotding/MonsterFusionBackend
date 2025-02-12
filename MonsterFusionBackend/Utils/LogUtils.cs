using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace MonsterFusionBackend.Utils
{
    internal class LogUtils
    {
        static bool isConfiged = false;
        static void ConfigureLoger()
        {
            isConfiged = true;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"MonsterFusionLog.Text");
            Log.Logger = new LoggerConfiguration().WriteTo.File( path,LogEventLevel.Information).CreateLogger();
        }
        public static void LogI(string message)
        {
            if (!isConfiged) ConfigureLoger();
            Log.Information(message);
        }
    }
}
