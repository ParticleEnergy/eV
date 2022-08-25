# eV

```C#
todo list

module
   eV.Module.PushNotification
```

```json
{
    "ProjectName": "NoteIssuingPlan",
    "Base": {
        "IsDevelopment": true,
        "ProjectAssemblyString": "NoteIssuingPlan.GameServer",
        "PublicObjectAssemblyString": "NoteIssuingPlan.PublicObject",
        "GameProfilePath": "/Users/three.zhang/Projects/NoteIssuingPlan/GameProfile/Json",
        "GameProfileMonitoringChange": true
    },
    "TcpServer": {
        "Host": "0.0.0.0",
        "Port": 1818,
        "Backlog": 32,
        "MaxConnectionCount": 128,
        "ReceiveBufferSize": 2048,
        "TcpKeepAliveTime": 60,
        "TcpKeepAliveInterval": 3,
        "TcpKeepAliveRetryCount": 3,
        "SessionMaximumIdleTime": 1800
    },
    "HttpServer": {
        "Protocol": "http",
        "Host": "127.0.0.1",
        "Port": 1616
    },
    "Redis": {
        "InstanceName": {
            "Address": [
                "10.0.0.32:6379"
            ],
            "Password": "123456",
            "Database": 0
        }
    },
    "Mongodb": {
        "DatabaseName": "mongodb://root:123456@10.0.0.32:27017/?serverSelectionTimeoutMS=5000&connectTimeoutMS=10000&authSource=admin&authMechanism=SCRAM-SHA-256&3t.uriVersion=3&3t.connection.name=localhost&3t.alwaysShowAuthDB=true&3t.alwaysShowDBFromUserRole=true"
    }
}

```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="ManagedColoredConsoleAppender" type="log4net.Appender.ManagedColoredConsoleAppender">
    <mapping>
      <level value="ERROR" />
      <foreColor value="DarkRed" />
    </mapping>
    <mapping>
      <level value="WARN" />
      <foreColor value="Yellow" />
    </mapping>
    <mapping>
      <level value="INFO" />
      <foreColor value="White" />
    </mapping>
    <mapping>
      <level value="DEBUG" />
      <foreColor value="Blue" />
    </mapping>

    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="[%level] %thread %date %logger: %message%newline" />
    </layout>
  </appender>

  <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="/Users/three.zhang/Projects/NoteIssuingPlan/NoteIssuingPlan.log" />
    <appendToFile value="true" />
    <rollingStyle value="Composite" />
    <datePattern value="\.yyyyMMdd" />
    <maxSizeRollBackups value="10" />
    <maximumFileSize value="15MB" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="[%level] %thread %date %logger: %message%newline" />
    </layout>
  </appender>

  <root>
    <level value="DEBUG" />
    <appender-ref ref="ManagedColoredConsoleAppender" />
    <appender-ref ref="RollingLogFileAppender" />
  </root>
</log4net>

```
