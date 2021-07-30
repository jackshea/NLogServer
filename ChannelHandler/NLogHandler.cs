using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace NLogServer
{
    public class NLogHandler : ChannelHandlerAdapter
    {
        private static StreamWriter sw;
        private static DateTime fileDate;
        private static string logRoot;
        private static string logFileFormat;

        public NLogHandler(IConfiguration config)
        {
            logRoot = config["logRoot"];
            logFileFormat = config["logFileFormat"];
            Directory.CreateDirectory(logRoot);

            OpenLogFileStream();
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (DateTime.Now.DayOfYear != fileDate.DayOfYear || DateTime.Now.Year != fileDate.Year)
            {
                sw.Flush();
                sw.Close();
                sw.Dispose();
                OpenLogFileStream();
            }
            var buffer = message as IByteBuffer;
            if (buffer != null)
            {
                var msg = buffer.ToString(Encoding.UTF8);
                Console.Write(msg);
                sw.Write(msg);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            sw.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }

        public static void OpenLogFileStream()
        {
            var logFile = Path.Combine(logRoot, string.Format(logFileFormat, DateTime.Now));

            if (!File.Exists(logFile))
            {
                File.Create(logFile).Dispose();
            }

            var fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            fileDate = DateTime.Now;
        }
    }
}