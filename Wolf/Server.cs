namespace Servers;

using System.Net;

using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Server;
using SimpleRpc.Serialization.Hyperion;

using Services;

public class Server
{
	public static void Main(string[] args)
    {
        var self = new Server();
        self.Run(args);
    }

    private void Run(string[] args)
    {
        Console.WriteLine("Starting Server...");

        StartServer(args);
    }

	private void StartServer(string[] args)
    {
		var builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.ConfigureKestrel(opts => {
			opts.Listen(IPAddress.Loopback, 5000);
		});

        builder.Services
			.AddSimpleRpcServer(new HttpServerTransportOptions { Path = "/assignment1" })
			.AddSimpleRpcHyperionSerializer();

    	builder.Services
			.AddSingleton<IWolfService>(new WolfService());

		var app = builder.Build();

		app.UseSimpleRpcServer();

		app.Run();
    }
}