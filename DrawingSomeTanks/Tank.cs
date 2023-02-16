using System.Drawing;
using static SDL2.SDL;

namespace DrawingSomeTanks;

public class Tank
{
    public ITankAi TankAi;
    public Color Color;

    public int Health;
    public int Ammo;

    public Point Position;
    public double TankRotation;
    public double TurretRotation;
    public double TankVelocity;

    public const int TankSize = 10;


    public Tank(ITankAi tankAi, Color color, Point startingPosition)
    {
        TankAi = tankAi;
        Color = color;
        Position = startingPosition;
        Health = 5;
    }

    public void Render(IntPtr renderer)
    {
        var tankRect = new SDL_Rect
        {
            x = Position.X - (TankSize / 2),
            y = Position.Y - (TankSize / 2),
            w = TankSize,
            h = TankSize
        };
        SDL_SetRenderDrawColor(renderer, Color.R, Color.G, Color.B, Color.A);
        SDL_RenderFillRect(renderer, ref tankRect);


        var turretLine = new SDL_Point[2];
        turretLine[0] = new SDL_Point
        {
            x = Position.X,
            y = Position.Y
        };

        turretLine[1] = new SDL_Point
        {
            x = Position.X + (int)(Math.Cos(TurretRotation) * 10),
            y = Position.Y + (int)(Math.Sin(TurretRotation) * 10)
        };

        SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderDrawLines(renderer, turretLine, 2);

        var tankRotationLine = new SDL_Point[2];
        tankRotationLine[0] = turretLine[0];

        tankRotationLine[1] = new SDL_Point
        {
            x = Position.X + (int)(Math.Cos(TankRotation) * 10),
            y = Position.Y + (int)(Math.Sin(TankRotation) * 10)
        };

        SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);
        SDL_RenderDrawLines(renderer, tankRotationLine, 2);
    }

    public void Update(GameField gameField, long currentTime)
    {
        // Update the tank's AI
        TankAi.Update(new ITankAi.SensorData(), gameField, currentTime);

        // Update the tank's position
        Position = new Point(
            Position.X + (int)(Math.Cos(TankRotation) * TankVelocity),
            Position.Y + (int)(Math.Sin(TankRotation) * TankVelocity)
        );
    }
}