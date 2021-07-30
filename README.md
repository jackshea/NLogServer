# NLogServer
NLog 的服务端，可从使用NLog的客户端收集日志。  
配合客户端NLog的NetworkTarget使用。

> 客户端参考：  
> https://github.com/jackshea/NLogClient  

Target配置参考:
```
<target xsi:type="Network" address="tcp://192.168.1.201:10086" name="network" newLine="true" onOverflow="Split" layout="${date}|${level:uppercase=true}|${message} ${exception}"/>
```
  
需要安装 dotnet, 版本为 .net 5  ，默认端口：10086 。

# 使用方法
以下方法任选一种：

## 1.使用dotnet run
克隆代码库后，切换到项目所在目录执行dotnet run 即可。

## 2.使用dotnet NLogServer.dll
1. 使用dotnet publish 发布  
> dotnet publish -c Release -o ~/nlog_server  

2. 执行 dll  
> dotnet ~/nlog_server/NLogServer.dll

## 3. 使用Docker
此方法需要安装docker, 无需安装dotnet 环境  
1. 启动
> docker run --name nlog_server -dp 10086:10086 jackshea/nlog_server
2. 查看日志
> docker logs -f nlog_server

## 4.使用docker-compose(推荐)
docker-compose.yml 示例文件
```docker-compose
version: '3'
services: 
    nlog_server:
        container_name: nlog_server
        image: jackshea/nlog_server
        ports: 
            - 10086:10086
        volumes: 
            - /var/log/nlog_server:/app/logs
            - /etc/localtime:/etc/localtime
        restart: always

```
启动
> docker-compose up -d

关闭
> docker-compose down