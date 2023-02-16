namespace DrawingSomeTanks.TankAis;

public class BasicTestAi : ITankAi
{
    public string Name => "Basic Test AI";

    double _turretRotation = 0;
    private double counter = 0;
    
    public ITankAi.TankAction Update(ITankAi.SensorData sensorData, GameField gameField, long currentTime, in Tank self)
    {
        counter += .05;
        _turretRotation = Math.Sin(counter);

        return new ITankAi.TankAction(0, _turretRotation, .1);
    }
}