using Microsoft.Extensions.DependencyInjection;
using RandomNameGeneratorLibrary;
using Services;
using SimpleRpc.Serialization.Hyperion;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Client;
using NLog;


class Client
{
    private readonly RabbitDesc rabbit = new RabbitDesc();
    private readonly Random rng = new Random();

    Logger mLog = LogManager.GetCurrentClassLogger();

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

    private void Run() 
    {
        ConfigureLogging();

        while(true)
        {
            try
            {
				var sc = new ServiceCollection();
                	sc
					.AddSimpleRpcClient(
						"wolfService",
						new HttpClientTransportOptions
						{
							Url = "http://127.0.0.1:5000/wolf",
							Serializer = "HyperionMessageSerializer"
						}
					)
					.AddSimpleRpcHyperionSerializer();

                sc.AddSimpleRpcProxy<IWolfService>("wolfService");

                var sp = sc.BuildServiceProvider();
                var wolf = sp.GetService<IWolfService>();
 
                if(wolf != null)
                {
                    InitializeRabbit(wolf);
                }

                while(true)
                {
                    mLog.Info(wolf.IsRabbitAlive(rabbit));
                    while(wolf.IsRabbitAlive(rabbit))
                    {
                        rabbit.DistanceToWolf = rng.Next(1, 100);
                        wolf.UpdateRabbitDistanceToWolf(rabbit);
                        mLog.Info($"The Rabbit is {rabbit.DistanceToWolf}m away");
                        Thread.Sleep(3000);
                    }

                    mLog.Info("Rabbit has died RIP");
                    Thread.Sleep(5000);
                    InitializeRabbit(wolf);
                }
            }
            catch(Exception err)
            {
                mLog.Error("Error has occured...", err);
                Thread.Sleep(3000);
            }
        }
    }

    static void Main(string[] args)
	{
		var self = new Client();
		self.Run();
	}

    private void InitializeRabbit(IWolfService wolf)
    {
        var personGenerator = new PersonNameGenerator();

        rabbit.RabbitName = personGenerator.GenerateRandomFirstAndLastName();
        rabbit.Weight = rng.Next(0, 10);
        rabbit.isRabbitAlive = true;
        rabbit.DistanceToWolf = 1000;
        rabbit.RabbitID = wolf.EnterWolfArea(rabbit);

        mLog.Info($"{rabbit.RabbitName} ({rabbit.Weight}) the Rabbit is born! #{rabbit.RabbitID}");
    }

    
}