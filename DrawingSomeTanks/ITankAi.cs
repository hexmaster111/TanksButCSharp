namespace DrawingSomeTanks;

public interface ITankAi
{
    public string Name { get; }
    public TankAction Update(SensorData sensorData, GameField gameField, long currentTime, Tank self);

    public struct TankAction
    {

        /// <summary>
        ///     Rotation of the turret in radians
        /// </summary>
        public double TurretRotation;

        /// <summary>
        ///     Rotation of the tank in radians
        /// </summary>
        public double TankRotation;

        /// <summary>
        ///     Velocity of the tank, -1 is backwards, 0 is stopped, 1 is forwards
        /// </summary>
        public double TankVelocity;

        /// <summary>
        ///     Whether or not to fire
        /// </summary>
        public bool Fire;
    }
}

public struct SensorData
{

}