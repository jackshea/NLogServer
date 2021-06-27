# NLogServer
NLog 的服务端，可从使用NLog的客户端收集日志。  
配置客户端NLog的NetworkTarget使用。

> 客户端参考：  
> https://github.com/jackshea/NLogClient  

Target配置参考:
```
<target xsi:type="Network" address="tcp://192.168.1.201:10086" name="network" newLine="true" onOverflow="Split" layout="${date}|${level:uppercase=true}|${message} ${exception}"/>
```
  
需要安装 dotnet, 版本为 .net 5  
默认端口：10086  

# 使用方法
以下方法凭选一种：
## 1.使用dotnet run
克隆代码库后，切换到所在目录执行dotnet run 即可。
## 2.使用dotnet NLogServer.dll
1. 使用dotnet publish 发布  
> dotnet publish -c Release -o ~/clog_server  

2. 执行 dll  
> dotnet ~/clog_server/NLogServer.dll
## 3. 使用Docker(推荐)
此方法需要安装docker, 无需安装dotnet 环境  
1. 启动
> docker run --name nlog_server -dp 10086:10086 jackshea/nlog_server
2. 查看日志
> docker logs -f nlog_server