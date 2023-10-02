namespace Services;

public class RabbitDesc
{
	public int RabbitID { get; set; }
	public string RabbitName { get; set; }
	public int Weight { get; set; }

	public int DistanceToWolf { get; set; }

	public bool isRabbitAlive { get; set; }
}

public interface IWolfService
{
	int EnterWolfArea(RabbitDesc rabbit);

	bool isRabbitEaten(int rabbitID);
}