// See https://aka.ms/new-console-template for more information
using System.Net;
using Confluent.Kafka;
using eV.Module.Queue.Kafka;


//
//
// var pconfig = new ProducerConfig
// {
//     BootstrapServers = "127.0.0.1:9092",
//     ClientId = Dns.GetHostName()
// };
// using (var producer = new ProducerBuilder<string, string>(pconfig).Build())
// {
//     producer.Produce("topic_1", new Message<string, string>{Key = "test", Value="a log message" });
//     producer.Flush();
// }
//
// var cconfig = new ConsumerConfig()
// {
//     BootstrapServers = "127.0.0.1:9092",
//     GroupId = "group1",
//     EnableAutoCommit = false,
//     StatisticsIntervalMs = 5000,
//     SessionTimeoutMs = 6000,
//     AutoOffsetReset = AutoOffsetReset.Earliest,
//     EnablePartitionEof = true,
//     // PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
//
// };
//
// var consumer = new ConsumerBuilder<string, string>(cconfig).Build();
// consumer.Subscribe("topic_1");
// while (true)
// {
//     try
//     {
//         var result = consumer.Consume();
//         Console.WriteLine(result.Message.Value);
//     }
//     catch (Exception e)
//     {
//         Console.WriteLine(e);
//         throw;
//     }
// }
//

// using (var producer = new ProducerBuilder<string, string>(config).Build())
// {
//     producer.Produce("log", new Message<string, string>{Key = "test", Value="a log message" });
//     producer.Flush();
//     // await producer.ProduceAsync("weblog", new Message<string, string> {Key = "test", Value="a log message" });
// }
Dictionary<string, KeyValuePair<ProducerConfig, ConsumerConfig>> config = new();

config["test"] = KeyValuePair.Create(
    new ProducerConfig
    {
        BootstrapServers = "127.0.0.1:9092"
    },
    new ConsumerConfig
    {
        BootstrapServers = "127.0.0.1:9092",
        GroupId = "group1",
        EnableAutoCommit = true,
        StatisticsIntervalMs = 5000,
        SessionTimeoutMs = 6000,

        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnablePartitionEof = true,
    }
);

KafkaManger.Instance.Start(config);

KafkaManger.Instance.GetKafka("test")!.Produce("test_topic", "testKey", "123", Console.WriteLine);

// KafkaManger<string, object>.Instance.GetKafka("test")!.Producer.Flush();

KafkaManger.Instance.GetKafka("test")!.Consume(delegate(IConsumer<string, object> consumer)
{
    consumer.Subscribe("test_topic");
}, delegate(ConsumeResult<string, object>? result )
{
    Console.WriteLine(result.Message.Value);
    return true;
});
Console.ReadLine();
