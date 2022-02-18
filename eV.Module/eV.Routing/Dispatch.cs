// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Reflection;
using eV.EasyLog;
using eV.Routing.Attributes;
using eV.Routing.Interface;
namespace eV.Routing;

public static class Dispatch
{
    private static readonly Dictionary<string, Route> s_receiveHandlers = new();
    private static readonly Dictionary<Type, string> s_sendMessages = new();
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
                if (Activator.CreateInstance(type) is not IHandler handler)
                    continue;

                Type[]? contentTypes = type.BaseType?.GenericTypeArguments;
                if (contentTypes is not { Length: > 0 })
                    continue;

                s_receiveHandlers[contentTypes[0].Name] = new Route(handler, contentTypes[0]);
                Logger.Info($"ReceiveMessageHandler [{type.FullName}] registration succeeded");
            }
            else if (sendMessageAttributes.Length > 0)
            {
                s_sendMessages[type] = type.Name;
                Logger.Info($"SendMessage [{type.FullName}] registration succeeded");
            }
        }
    }
    public static void Dispense(ISession session, IPacket packet)
    {
        if (packet.GetName().Equals("") || packet.GetContent().Length == 0)
            return;

        try
        {
            IRoute? route = GetRoute(packet.GetName());
            if (route == null)
            {
                Logger.Error($"The receiver corresponding to packet [{packet.GetName()}] is not found");
                return;
            }
            object content = Serializer.Deserialize(packet.GetContent(), route.ContentType);
            route.Handler.Run(session, content);
            Logger.Info($"Message [{packet.GetName()}] handle access");
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    private static IRoute? GetRoute(string name)
    {
        if (!s_receiveHandlers.TryGetValue(name, out Route? result))
            Logger.Error($"Message [{name}] handler not found!");

        return result;
    }

    public static string GetSendMessageName(Type type)
    {
        if (!s_sendMessages.TryGetValue(type, out string? result))
            Logger.Error($"Send Message [{type.Name}] not found!");
        return result ?? "";
    }

    public static void AddCustomHandler(Type handlerType, Type contentType)
    {
        if (Activator.CreateInstance(handlerType) is not IHandler handler)
            return;
        s_receiveHandlers[contentType.Name] = new Route(handler, contentType);
    }
}
