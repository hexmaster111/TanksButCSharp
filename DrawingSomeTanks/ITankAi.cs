namespace DrawingSomeTanks;

public interface ITankAi
{
    public string Name { get; }
    public TankAction Update(SensorData sensorData, GameField gameField, long currentTime, in Tank self);

    public struct TankAction
    {
        /// <summary>
        ///    New command for the tank after an AI Call
        /// </summary>
        /// <param name="turretRotation">rotation in rads</param>
        /// <param name="tankRotation">rotation in rads</param>
        /// <param name="tankVelocity">between -1 and 1</param>
        public TankAction(double turretRotation, double tankRotation, double tankVelocity)
        {
            TurretRotation = turretRotation;
            TankRotation = tankRotation;
            TankVelocity = tankVelocity;
        }

        public double TurretRotation;
        public double TankRotation;
        public double TankVelocity;
    }

    public struct SensorData
    {
    }
}