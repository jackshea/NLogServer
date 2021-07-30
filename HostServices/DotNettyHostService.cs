using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLogServer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace COTDHost
{
    public class DotNettyHostService : IHostedService
    {
        private IConfiguration _configuration;
        private readonly ILogger<DotNettyHostService> _log;
        private IChannel _boundChannel;
        private IEventLoopGroup _bossGroup;
        private IEventLoopGroup _workerGroup;

        public DotNettyHostService(IConfiguration configuration, ILogger<DotNettyHostService> log)
        {
            _configuration = configuration;
            _log = log;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();
            var port = Convert.ToInt32(_configuration["port"]);
            _log.LogInformation($"Server starting...");
            _log.LogInformation($"try listen port : {port}");

            var bootstrap = new ServerBootstrap();
            bootstrap.Group(_bossGroup, _workerGroup);
            bootstrap.Channel<TcpServerSocketChannel>();

            bootstrap
                .Option(ChannelOption.SoBacklog, 100)
                .Option(ChannelOption.SoKeepalive, true)
                //.Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new NLogHandler(_configuration));
                    //pipeline.AddLast("framing-enc", new LengthFieldPrepender(ByteOrder.LittleEndian, 4, 0, false));
                    //pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, int.MaxValue, 0, 4, 0, 0, true));
                    //pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    //pipeline.AddLast("idleStateHandler", new IdleStateHandler(60, 30, 0));
                }));

            _boundChannel = await bootstrap.BindAsync(port);
            _log.LogInformation($"now listening port : {port}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _workerGroup.ShutdownGracefullyAsync();
            await _bossGroup.ShutdownGracefullyAsync();
            await _boundChannel.CloseAsync();
        }
    }
}