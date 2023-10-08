using Services;
using NLog;
using Microsoft.Extensions.DependencyInjection;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Client;
using SimpleRpc.Serialization.Hyperion;
class Client
{
    private readonly WaterDesc water = new WaterDesc();

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

                InitializeWater(wolf);

                while(true)
                {
                    while(wolf.IsWaterAlive(water))
                    {
                        mLog.Info("~~~~~~~~~~~~~~~~~");
                        //Checks every 0.5s
                        Thread.Sleep(500);
                    }

                    mLog.Info("The water is empty");
                    Thread.Sleep(5000);
                    InitializeWater(wolf);
                }

            }
            catch (Exception err)
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

    private void InitializeWater(IWolfService wolf)
    {
        water.Volume = rng.Next(0, 10);
        water.x = rng.Next(-50, 50);
        water.y = rng.Next(-50, 50);
        water.WaterID = wolf.SpawnWaterNearWolf(water);
    }
}