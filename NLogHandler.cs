using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using NLog;

namespace NLogServer
{
    public class NLogHandler : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var Logger = LogManager.GetLogger(context.Name);
            var buffer = message as IByteBuffer;
            if (buffer != null)
            {
                var rawString = buffer.ToString(Encoding.UTF8);
                var logMessage = JsonConvert.DeserializeObject<LogMessage>(rawString);
                Logger.Log(logMessage.LogLevel, logMessage.Message);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }

    public class LogMessage
    {
        public LogLevel LogLevel;
        public string Message;
    }
}