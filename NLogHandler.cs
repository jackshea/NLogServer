﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.IO;
using System.Text;

namespace NLogServer
{
    public class NLogHandler : ChannelHandlerAdapter
    {
        private static StreamWriter sw;

        static NLogHandler()
        {
            Console.WriteLine("Constructor");
            Directory.CreateDirectory(Program.logRoot);
            var logFile = Path.Combine(Program.logRoot, string.Format(Program.logFileFormat, DateTime.Now));
            if (!File.Exists(logFile))
            {
                File.Create(logFile);
            }
            var fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            sw = new StreamWriter(fs);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
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
    }
}