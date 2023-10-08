using Services;

public class WolfService : IWolfService
{
    private readonly WolfLogic wolfLogic = new WolfLogic();

    public int EnterWolfArea(RabbitDesc rabbit)
    {
        return wolfLogic.EnterWolfArea(rabbit);
    }


    public void UpdateRabbitDistanceToWolf(RabbitDesc rabbit)
    {
        wolfLogic.UpdateRabbitDistanceToWolf(rabbit);
    }

    public bool IsRabbitAlive(RabbitDesc rabbit)
    {
        return wolfLogic.IsRabbitAlive(rabbit);
    }
}