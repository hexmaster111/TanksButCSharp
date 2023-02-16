namespace DrawingSomeTanks.TankAis;

public class BasicTestAi : ITankAi
{
    public string Name => "Basic Test AI";
    public ITankAi.TankAction Update(ITankAi.SensorData sensorData, GameField gameField, long currentTime)
    {
        return new ITankAi.TankAction();
    }
}
