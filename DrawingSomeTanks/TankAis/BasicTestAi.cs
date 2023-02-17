using System.Drawing;

namespace DrawingSomeTanks.TankAis;

public class BasicTestAi : ITankAi
{
    public string Name => "Basic Test AI";

    private const int TankGetAmmoThreshold = 5;


    public ITankAi.TankAction Update
    (
        SensorData sensorData,
        GameField gameField,
        long currentTime,
        Tank self
    )
    {

        if (self.Ammo < TankGetAmmoThreshold)
        {
            var myPos = self.Position;
            var ammoPickup = gameField.AmmoPickups.OrderBy(x
                             => x.Position.DistanceTo(myPos)).FirstOrDefault();
            if (ammoPickup == null) return new ITankAi.TankAction();
            var ammoPos = ammoPickup.Position;
            var angleToAmmo = Math.Atan2(ammoPos.Y - myPos.Y, ammoPos.X - myPos.X);

            return new ITankAi.TankAction
            {
                TankRotation = angleToAmmo,
                TankVelocity = 1,
                Fire = false
            };
        }

        var enemy = gameField.Tanks.FirstOrDefault(x => x != self);
        if (enemy == null) return new ITankAi.TankAction();
        var enemyPos = enemy.Position;
        var angleToEnemy = Math.Atan2(enemyPos.Y - self.Position.Y, enemyPos.X - self.Position.X);
        //if we are inside an enemy, move away
        if (self.Position.DistanceTo(enemyPos) < Tank.TankSize * 2)
        {
            angleToEnemy += Math.PI;
        }
        return new ITankAi.TankAction
        {
            TankRotation = angleToEnemy,
            TankVelocity = 1,
            TurretRotation = angleToEnemy,
            Fire = true
        };

    }
}

public static class Ext
{
    public static double DistanceTo(this Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}