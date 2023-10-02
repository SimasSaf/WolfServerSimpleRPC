using Microsoft.Extensions.DependencyInjection;
using RandomNameGeneratorLibrary;
using Services;
using SimpleRpc.Serialization.Hyperion;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Client;


class Client
{
    private readonly RabbitDesc rabbit = new RabbitDesc();
    private readonly Random rng = new Random();
    

    private void Run() 
    {
        var rnd = new Random();

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
 
                InitializeRabbit(wolf);

                while(true)
                {
                    while(rabbit.isRabbitAlive)
                    {
                        rabbit.DistanceToWolf = rng.Next(1, 100);
                        Console.WriteLine($"The Rabbit is {rabbit.DistanceToWolf}m away");
                        Thread.Sleep(3000);

                        if(wolf.isRabbitEaten(rabbit.RabbitID))
                        {
                            Console.WriteLine("Rabbit has died RIP");
                            Thread.Sleep(5000);
                            InitializeRabbit(wolf);
                        }
                    }
                }
            }
            catch(Exception err)
            {
                Console.WriteLine("Error has occured...", err);
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
        rabbit.RabbitID = wolf.EnterWolfArea(rabbit);
        
        Console.WriteLine($"{rabbit.RabbitName} ({rabbit.Weight}) the Rabbit is born! #{rabbit.RabbitID}");
    }
}