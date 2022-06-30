// See https://aka.ms/new-console-template for more information

using eV.Framework.Server;
using eV.Module.EasyLog;
using eV.ServerExample;
new Application(args).SetProfile(GameProfile.Instance).SetServerOnConnected(delegate { Logger.Info("SetServerOnConnected"); }).SetServerSessionOnActivate(delegate { Logger.Info("SetServerSessionOnActivate"); }).SetServerOnConnected(delegate { Logger.Info("SetServerOnConnected"); }).Run();

//
// {
//     "ProjectName": "eV.ServerExample",
//     "Base": {
//         "HandlerNamespace": "eV.ServerExample",
//         "PublicObjectNamespace": "eV.PublicObject",
//         "GameProfilePath": "/Users/three.zhang/Projects/CSharp/eV/eV.Example/eV.ServerExample/Profile",
//         "GameProfileMonitoringChange": true
//     },
//     "Server": {
//         "Host": "0.0.0.0",
//         "Port": 8888,
//         "Backlog": 2,
//         "MaxConnectionCount": 2,
//         "ReceiveBufferSize": 2,
//         "TcpKeepAliveTime": 60,
//         "TcpKeepAliveInterval": 3,
//         "TcpKeepAliveRetryCount": 3,
//         "SessionMaximumIdleTime": 1800
//     },
//     "Cluster": {
//         "ConsumeSendPipelineNumber": 1,
//         "ConsumeSendGroupPipelineNumber": 1,
//         "ConsumeSendBroadcastPipelineNumber": 1,
//         "Redis": {
//             "Address": [
//             "127.0.0.1:6379"
//                 ],
//             "Password": "123456",
//             "Database": 0,
//             "Keepalive": 60
//         },
//         "Kafka": {
//             "Address": "127.0.0.1:9092"
//         }
//     },
//     "Redis": {
//         "eV": {
//             "Address": [
//             "127.0.0.1:6379"
//                 ],
//             "Password": "123456",
//             "Database": 0,
//             "Keepalive": 60
//         }
//     },
//     "Mongodb": {
//         "eV": "mongodb://root:123456@localhost:27017/?serverSelectionTimeoutMS=5000&connectTimeoutMS=10000&authSource=admin&authMechanism=SCRAM-SHA-256&3t.uriVersion=3&3t.connection.name=localhost&3t.alwaysShowAuthDB=true&3t.alwaysShowDBFromUserRole=true"
//     },
//     "Kafka": {
//         "eV": {
//             "Address": "127.0.0.1:9092"
//         }
//     }
// }
