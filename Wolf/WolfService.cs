using Services;

public class WolfService : IWolfService
{
    private readonly WolfLogic wolfLogic = new WolfLogic();

    public int EnterWolfArea(RabbitDesc rabbit)
    {
        return wolfLogic.EnterWolfArea(rabbit);
    }

    public int SpawnWaterNearWolf(WaterDesc water)
    {
        return wolfLogic.SpawnWaterNearWolf(water);
    }

    public void UpdateRabbitDistanceToWolf(RabbitDesc rabbit)
    {
        wolfLogic.UpdateRabbitDistanceToWolf(rabbit);
    }

    public bool IsRabbitAlive(RabbitDesc rabbit)
    {
        return wolfLogic.IsRabbitAlive(rabbit);
    }

    public bool IsWaterAlive(WaterDesc water)
    {
        return wolfLogic.IsWaterAlive(water);
    }
}