// Zero

using eV.Framework.Server;

await ApplicationBuilder
    .CreateTcpBuilder(args)
    .Build()
    .RunAsync();
