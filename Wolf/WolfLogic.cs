using Services;

public class WolfState
{
    public readonly object AccessLock = new object();

    public int LastUniqueID;

    public int x;
    public int y;

    public int WolfWeight;

    public List<RabbitDesc> RabbitsNearby = new List<RabbitDesc>();
}

class WolfLogic
{
    static readonly int WOLF_IS_FULL = 30;
    private Thread backgroundTaskThread;

    private WolfState wolfState = new WolfState();

    Random rng = new Random();

    public WolfLogic()
    {
        backgroundTaskThread = new Thread(BackgroundTask);
    }

    public bool IsRabbitEaten(int rabbitID)
    {
        lock(wolfState.AccessLock)
        {
            if(wolfState.RabbitsNearby.Exists(rabbit => rabbit.RabbitID == rabbitID))
            {
                return true;
            }
            else return false;
        }
    }

    public int EnterWolfArea(RabbitDesc rabbit)
    {
        lock(wolfState.AccessLock)
        {
            wolfState.LastUniqueID += 1;

            rabbit.RabbitID = wolfState.LastUniqueID;

            wolfState.RabbitsNearby.Add(rabbit);

            return rabbit.RabbitID;
        }
    }

    private void BackgroundTask()
    {
        while(true)
        {
            lock(wolfState.AccessLock)
            {
                Console.WriteLine("The wolf is moving...");

                GenerateRandomWolfCoordinates();
                
                Console.WriteLine($"The Wolf is currently at [{wolfState.x},{wolfState.y}]");

                Thread.Sleep(1000);

                Console.WriteLine("Wolf is sniffing out the rabbits...");

                wolfState.RabbitsNearby.ForEach( rabbit => 
                {
                    if(rabbit.DistanceToWolf <= 3)
                    {
                        EatRabbit(rabbit);
                    }
                    
                    if(wolfState.WolfWeight >= WOLF_IS_FULL)
                    {
                        wolfState.WolfWeight = 0;
                        Thread.Sleep(5000); //?? Is this Okay?
                    }
                });
            };
        }
    }

    private void EatRabbit(RabbitDesc rabbit)
    {
        Console.WriteLine($"Eating {rabbit.RabbitName} The Rabbit");
        wolfState.WolfWeight += rabbit.Weight;
        
        wolfState.RabbitsNearby.Remove(rabbit);
    }

    private void GenerateRandomWolfCoordinates()
    {
        wolfState.x = rng.Next(-100, 100);
        wolfState.y = rng.Next(-100, 100);
    }
}

