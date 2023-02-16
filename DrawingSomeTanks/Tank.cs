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
    public const int MaxSpeed = 2;
    public const int TankPointerLineLength = 10;


    public Tank(ITankAi tankAi, Color color, Point startingPosition)
    {
        TankAi = tankAi;
        Color = color;
        Position = startingPosition;
        Health = 5;

        _lastX = Position.X;
        _lastY = Position.Y;
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
            x = Position.X + (int)(Math.Cos(TurretRotation) * TankPointerLineLength),
            y = Position.Y + (int)(Math.Sin(TurretRotation) * TankPointerLineLength)
        };

        SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderDrawLines(renderer, turretLine, 2);

        var tankRotationLine = new SDL_Point[2];
        tankRotationLine[0] = turretLine[0];

        tankRotationLine[1] = new SDL_Point
        {
            x = Position.X + (int)(Math.Cos(TankRotation) * TankPointerLineLength),
            y = Position.Y + (int)(Math.Sin(TankRotation) * TankPointerLineLength)
        };

        SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);
        SDL_RenderDrawLines(renderer, tankRotationLine, 2);
    }

    private double _lastX;
    private double _lastY;

    public void Update(GameField gameField, long currentTime)
    {
        // Update the tank's AI
        var tankAction = TankAi.Update(new ITankAi.SensorData(), gameField, currentTime, this);

        TankRotation = tankAction.TankRotation;
        TurretRotation = tankAction.TurretRotation;


        var requestedVelocityPercent = tankAction.TankVelocity;
        requestedVelocityPercent = Math.Clamp(requestedVelocityPercent, -1, 1);
        TankVelocity = requestedVelocityPercent * MaxSpeed;

        var newX = _lastX + (Math.Cos(TankRotation) * TankVelocity);
        var newY = _lastY + (Math.Sin(TankRotation) * TankVelocity);

        // Console.WriteLine($"Tank {Color} is at {Position.X:0000.00}, {Position.Y:0000.00}");

        newX = Math.Clamp(newX, 0, gameField.Width);
        newY = Math.Clamp(newY, 0, gameField.Height);

        _lastX = newX;
        _lastY = newY;

        // Update the tank's position
        Position = new Point(
            (int)newX,
            (int)newY
        );
    }
}