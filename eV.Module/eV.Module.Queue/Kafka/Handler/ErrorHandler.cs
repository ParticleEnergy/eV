// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.EasyLog;
namespace eV.Module.Queue.Kafka.Handler;

public static class ErrorHandler
{
    public static void ProducerErrorHandler<TKey, TValue>(IProducer<TKey, TValue> _, Error error)
    {
        Logger.Error($"Kafka Error code:{error.Code} reason: {error.Reason}");
    }

    public static void ConsumerErrorHandler<TKey, TValue>(IConsumer<TKey, TValue> _, Error error)
    {
        Logger.Error($"Kafka Error code:{error.Code} reason: {error.Reason}");
    }
}
