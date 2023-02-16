namespace DrawingSomeTanks;

public interface ITankAi
{
    public string Name { get; }
    public TankAction Update(SensorData sensorData, GameField gameField, long currentTime);

    public struct TankAction
    {
    }

    public struct SensorData
    {
    }
}