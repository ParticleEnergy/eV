// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using eV.Routing.Attributes;
using eV.Routing.Interface;
using log4net;
namespace eV.Routing
{
    public static class Dispatch
    {
        private static readonly Dictionary<string, Route> s_receiveHandlers = new();
        private static readonly Dictionary<Type, string> s_sendMessages = new();
        private static readonly ILog s_logger = LogManager.GetLogger(DefaultSetting.LoggerName);
        private static bool s_registered;

        public static void Register(string nsName)
        {
            if (s_registered)
                return;
            s_registered = true;

            Type[] allTypes = Assembly.Load(nsName).GetExportedTypes();

            foreach (Type type in allTypes)
            {
                object[] receiveMessageHandlerAttributes = type.GetCustomAttributes(typeof(ReceiveMessageHandlerAttribute), true);
                object[] sendMessageAttributes = type.GetCustomAttributes(typeof(SendMessageAttribute), true);

                if (receiveMessageHandlerAttributes.Length > 0)
                {
                    var handler = Activator.CreateInstance(type) as IHandler;
                    if (handler == null)
                        continue;

                    var contentTypes = type.BaseType?.GenericTypeArguments;
                    if (contentTypes is not { Length: > 0 })
                        continue;

                    s_receiveHandlers.Add(contentTypes[0].Name, new Route(handler, contentTypes[0]));
                    s_logger.Info($"ReceiveMessageHandler [{type.FullName}] registration succeeded");
                }
                else if (sendMessageAttributes.Length > 0)
                {
                    s_sendMessages.Add(type, type.Name);
                    s_logger.Info($"SendMessage [{type.Name}] registration succeeded");
                }
            }
        }
        public static bool Dispense(ISession session, IPacket packet)
        {
            if (packet.GetName().Equals("") || packet.GetContent().Length == 0)
                return false;

            try
            {
                IRoute? route = GetRoute(packet.GetName());
                if (route == null)
                {
                    s_logger.Error($"The receiver corresponding to packet [{packet.GetName()}] is not found");
                    return false;
                }
                object content = Serializer.Deserialize(packet.GetContent(), route.ContentType);
                route.Handler.Run(session, content);
                s_logger.Info($"Message [{packet.GetName()}] handle access");
                return true;
            }
            catch (Exception e)
            {
                s_logger.Error(e.Message, e);
                return false;
            }
        }
        private static IRoute? GetRoute(string name)
        {
            if (!s_receiveHandlers.TryGetValue(name, out Route? result))
                s_logger.Error($"Message [{name}] handler not found!");

            return result;
        }

        public static string GetSendMessageName(Type type)
        {
            if (!s_sendMessages.TryGetValue(type, out string? result))
                s_logger.Error($"Send Message [{type.Name}] not found!");
            return result ?? "";
        }
    }
}
