// // Copyright (c) ParticleEnergy. All rights reserved.
// // Licensed under the Apache license. See the LICENSE file in the project root for full license information.
//
// using eV.Framework.Server.Interface;
// using eV.Framework.Server.Utils;
// using eV.Module.Cluster;
// using eV.Module.Routing.Interface;
// using eV.Module.Session;
//
// namespace eV.Framework.Server.SessionDrive;
//
// public class ClusterSessionDrive : ISessionDrive
// {
//     // private readonly ClusterSession _clusterSession;
//
//     public ClusterSessionDrive(ClusterSession clusterSession)
//     {
//         _clusterSession = clusterSession;
//     }
//
//     public bool Send(string sessionId, byte[] data)
//     {
//         Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(sessionId);
//         if (session != null)
//             return session.Send(data);
//
//         _clusterSession.Send(sessionId, data);
//         return true;
//     }
//
//     public void SendBroadcast(string selfSessionId, byte[] data)
//     {
//         SessionUtils.SendBroadcast(selfSessionId, data);
//         _clusterSession.SendBroadcast(data);
//     }
//
//     public bool Activate(ISession session)
//     {
//         if (!SessionUtils.Activate(session))
//             return false;
//         _clusterSession.Registry(session.SessionId!);
//         return true;
//     }
//
//     public bool Release(ISession session)
//     {
//         if (!SessionUtils.Release(session))
//             return false;
//         _clusterSession.Deregister(session.SessionId!);
//         return true;
//     }
// }
