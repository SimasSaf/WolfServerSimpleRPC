namespace Servers;

using System.Net;

using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Server;
using SimpleRpc.Serialization.Hyperion;

using NLog;

using Services;

public class Server
{
	Logger log = LogManager.GetCurrentClassLogger();

    private void ConfigureLogging()
	{
		var config = new NLog.Config.LoggingConfiguration();

		var console =
			new NLog.Targets.ConsoleTarget("console")
			{
				Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
			};
		config.AddTarget(console);
		config.AddRuleForAllLevels(console);

		LogManager.Configuration = config;
	}

	public static void Main(string[] args)
    {
        var self = new Server();
        self.Run(args);
    }

    private void Run(string[] args)
    {
        ConfigureLogging();

        log.Info("Starting Server...");

        StartServer(args);
    }

	private void StartServer(string[] args)
    {
		var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(opts => {
			opts.Listen(IPAddress.Loopback, 5000);
		});

        builder.Services
			.AddSimpleRpcServer(new HttpServerTransportOptions { Path = "/wolf" })
			.AddSimpleRpcHyperionSerializer();

    	builder.Services
			.AddSingleton<IWolfService>(new WolfService());

		var app = builder.Build();

		app.UseSimpleRpcServer();

		app.Run();
    }
}