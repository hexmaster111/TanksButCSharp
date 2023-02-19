using System.Drawing;
using System.Numerics;
using DrawingSomeTanks;

namespace DrawingSomeTanks.TankAis;

public class DogingAi : ITankAi
{
    public string Name => "Doging AI";

    public ITankAi.TankAction Update(SensorData sensorData, GameField gameField, long currentTime, Tank self)
    {
        foreach (var projectial in gameField.Projectiles)
        {
            //The projectial has a position rotation, and a fixed speed,
            //We can calculate the position of the projectial in the future by using the formula:
            //x = x0 + v * cos(theta) * t
            //y = y0 + v * sin(theta) * t
            //Where x0 and y0 are the current position, v is the speed, theta is the rotation, and t is the time

            //If the projectial is going to hit us, we need to move out of the way
            //We can calculate the time it will take for the projectial to hit us by using the formula:
            //t = (x - x0) / (v * cos(theta))
            //Where x is the x position of the tank, x0 is the x position of the projectial, v is the speed of the
            // projectial, and theta is the rotation of the projectial
            //We can then use the formula above to calculate the position of the projectial in the future
            //We can then calculate the distance between the projectial and the tank
            //If the distance is less than the size of the tank, we need to move out of the way

            var timeToHit = (self.Position.X - projectial.Position.X) /
                (Projectile.Velocity * Math.Cos(projectial.Rotation));

            var futurePosition = new Point(
                projectial.Position.X + (int)(Math.Cos(projectial.Rotation) * Projectile.Velocity * timeToHit),
                projectial.Position.Y + (int)(Math.Sin(projectial.Rotation) * Projectile.Velocity * timeToHit)
            );

            var distanceToHit = self.Position.DistanceTo(futurePosition);

            if (distanceToHit < Tank.TankSize)
            {
                //We need to move out of the way
                //We can calculate the angle between the projectial and the tank
                //We can then calculate the angle between the tank and the point that is 90 degrees away 
                //from the projectial
                //We can then move in the direction of that point

                var angleToProjectial = Math.Atan2(projectial.Position.Y - self.Position.Y,
                                         projectial.Position.X - self.Position.X);
                var angleToMove = angleToProjectial + Math.PI / 2;

                return new ITankAi.TankAction
                {
                    TankVelocity = 1,
                    TankRotation = angleToMove,
                    TurretRotation = angleToMove
                };
            }

        }


        //We arnt getting hit by anything
        //We can just move away from any other tanks
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


        //find the closest ammo pickup
        var ammoPickup = gameField.AmmoPickups.MinBy(x => x.Position.DistanceTo(self.Position));

        if (ammoPickup != null)
        {
            var distanceToAmmo = self.Position.DistanceTo(ammoPickup.Position);
            var angleToAmmo = Math.Atan2(ammoPickup.Position.Y - self.Position.Y, ammoPickup.Position.X - self.Position.X);

            if (distanceToAmmo < Tank.TankSize * 3)
            {
                return new ITankAi.TankAction
                {
                    TankVelocity = -1,
                    TurretRotation = angleToAmmo,
                    TankRotation = angleToAmmo,
                    Fire = true
                };
            }

            return new ITankAi.TankAction
            {
                TankVelocity = 1,
                TurretRotation = angleToAmmo,
                TankRotation = angleToAmmo,
                Fire = distanceToAmmo < 150
            };
        }

        return new ITankAi.TankAction { };
    }
}