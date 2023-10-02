using Services;

public class WolfService : IWolfService
{
    private readonly WolfLogic wolfLogic = new WolfLogic();

    public int EnterWolfArea(RabbitDesc rabbit)
    {
        return wolfLogic.EnterWolfArea(rabbit);
    }

    public bool isRabbitEaten(int rabbitID)
    {
        return wolfLogic.IsRabbitEaten(rabbitID);
    }
}