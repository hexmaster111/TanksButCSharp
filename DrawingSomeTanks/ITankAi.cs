namespace DrawingSomeTanks;

public interface ITankAi
{
    public string Name { get; }
    public TankAction Update(SensorData sensorData, GameField gameField, long currentTime, Tank self);

    public struct TankAction
    {
        /// <summary>
        ///    New command for the tank after an AI Call
        /// </summary>
        /// <param name="turretRotation">rotation in rads</param>
        /// <param name="tankRotation">rotation in rads</param>
        /// <param name="tankVelocity">between -1 and 1</param>
        public double TurretRotation;
        public double TankRotation;
        public double TankVelocity;
        public bool Fire;
    }
}

public struct SensorData
{

}