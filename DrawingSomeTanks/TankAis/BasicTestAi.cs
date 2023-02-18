using System.Drawing;

namespace DrawingSomeTanks.TankAis;

public class BasicTestAi : ITankAi
{
    public string Name => "Basic Test AI";

    private const int TankGetAmmoThreshold = 1;
    private const int TankStopGettingAmmoThreshold = 5;

    public ITankAi.TankAction Update
    (
        SensorData sensorData,
        GameField gameField,
        long currentTime,
        Tank self
    )
    {
        var enemy = gameField.Tanks.MinBy(x =>
        {
            if (x == self) return double.MaxValue;
            return x.Position.DistanceTo(self.Position);
        });


        if (enemy == null || enemy == self)
            return new ITankAi.TankAction();

        var enemyPos = enemy.Position;
        var angleToEnemy = Math.Atan2(enemyPos.Y - self.Position.Y, enemyPos.X - self.Position.X);
        var distanceToEnemy = self.Position.DistanceTo(enemyPos);

        if (distanceToEnemy < Tank.TankSize * 3)
        {
            return new ITankAi.TankAction
            {
                TankVelocity = -1,
                TurretRotation = angleToEnemy,
                TankRotation = angleToEnemy,
                Fire = true
            };
        }

        return new ITankAi.TankAction
        {
            TankRotation = angleToEnemy,
            TankVelocity = 1,
            TurretRotation = angleToEnemy,
            Fire = distanceToEnemy < 150
        };
    }

    public ITankAi.TankAction GetAmmoCollectionCommand(GameField gameField, Tank self)
    {
        var myPos = self.Position;


        //Find an ammo pickup that is closest to us, and there is no other tank closer to it
        var ammoPickup = gameField.AmmoPickups.MinBy(x =>
        {
            var distanceToAmmo = x.Position.DistanceTo(myPos);
            var closestTank = gameField.Tanks.MinBy(y => y.Position.DistanceTo(x.Position));
            var distanceToClosestTank = closestTank.Position.DistanceTo(x.Position);
            if (distanceToClosestTank < distanceToAmmo) return double.MaxValue;
            return distanceToAmmo;
        });

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

}


public static class Ext
{
    public static double DistanceTo(this Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}