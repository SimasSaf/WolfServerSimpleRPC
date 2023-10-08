using Services;
using NLog;
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
    static readonly int WOLF_MAX_WEIGHT = 30;
    static bool WOLF_IS_FULL = false;
    private Thread backgroundTaskThread;

    private Logger mLog = LogManager.GetCurrentClassLogger();

    private WolfState wolfState = new WolfState();

    Random rng = new Random();

    public WolfLogic()
    {
        backgroundTaskThread = new Thread(BackgroundTask);
        backgroundTaskThread.Start();
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

    public void UpdateRabbitDistanceToWolf(RabbitDesc rabbit)
    {
        lock(wolfState.AccessLock)
        {
                mLog.Info("Updating rabbit distance " + rabbit.DistanceToWolf);
                var rabbitNearby = wolfState.RabbitsNearby.Find(rabbitNearby => rabbitNearby.RabbitID.Equals(rabbit.RabbitID));

                if(rabbitNearby != null)
                {
                    rabbitNearby.DistanceToWolf = rabbit.DistanceToWolf;
                }
        }
    }

    public bool IsRabbitAlive(RabbitDesc rabbit)
    {
        lock(wolfState.AccessLock)
        {
            return wolfState.RabbitsNearby.Any(rabbitNearby => rabbitNearby.RabbitID == rabbit.RabbitID);
        }
    }

    private void BackgroundTask()
    {
        while(true)
        {
            lock(wolfState.AccessLock)
            {
                mLog.Info($"The wolf {wolfState.WolfWeight} is moving...");

                GenerateRandomWolfCoordinates();
                
                mLog.Info($"The Wolf is currently at [{wolfState.x},{wolfState.y}]");
                
                //This is just to not spam, let's imagine its doing calculations :D
                Thread.Sleep(1000);

  
                
                List<RabbitDesc> newRabbitsNearby = wolfState.RabbitsNearby;

                for (int i = newRabbitsNearby.Count - 1; i >= 0; i--)
                {
                    mLog.Info("Wolf is sniffing out the rabbits...");

                    var rabbit = wolfState.RabbitsNearby[i];
                    mLog.Info("Rabbit distance: " + rabbit.DistanceToWolf);

                    if (rabbit.DistanceToWolf <= 30)
                    {
                        mLog.Info("Rabbit distance: " + rabbit.DistanceToWolf);

                        EatRabbit(rabbit);
                    }
                    
                    if (wolfState.WolfWeight >= WOLF_MAX_WEIGHT)
                    {
                        WOLF_IS_FULL = true;
                        break;
                    }
                }
            };

            if (WOLF_IS_FULL)
            {
                lock(wolfState.AccessLock)
                {
                    mLog.Info("Wolf is Full");
                    wolfState.WolfWeight = 0;
                }

                WOLF_IS_FULL = false;
                
                Thread.Sleep(5000);
            }
        }
    }

    private void EatRabbit(RabbitDesc rabbit)
    {
        mLog.Info($"Eating {rabbit.RabbitName} The Rabbit");
        wolfState.WolfWeight += rabbit.Weight;
        wolfState.RabbitsNearby.Remove(rabbit);
    }

    private void GenerateRandomWolfCoordinates()
    {
        wolfState.x = rng.Next(-100, 100);
        wolfState.y = rng.Next(-100, 100);
    }
}

