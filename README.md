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
