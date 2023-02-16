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
    public const int TankShotCooldownMs = 500;


    public Tank(ITankAi tankAi, Color color, Point startingPosition)
    {
        TankAi = tankAi;
        Color = color;
        Position = startingPosition;
        Health = 5;
        Ammo = 4;

        _lastX = Position.X;
        _lastY = Position.Y;
    }

    private void DrawPointer(double rotation, int length, IntPtr renderer,
    System.Drawing.Color color)
    {
        var pointerLine = new SDL_Point[2];
        pointerLine[0] = new SDL_Point
        {
            x = Position.X,
            y = Position.Y
        };

        pointerLine[1] = new SDL_Point
        {
            x = Position.X + (int)(Math.Cos(rotation) * length),
            y = Position.Y + (int)(Math.Sin(rotation) * length)
        };

        SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
        SDL_RenderDrawLines(renderer, pointerLine, 2);
    }
    private Point BarrelEndPos => new Point(
        Position.X + (int)(Math.Cos(TurretRotation) * TankPointerLineLength),
        Position.Y + (int)(Math.Sin(TurretRotation) * TankPointerLineLength)
    );

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

        DrawPointer(TankRotation, TankPointerLineLength, renderer, Color.Red);
        DrawPointer(TurretRotation, TankPointerLineLength, renderer, Color.Blue);
    }

    private double _lastX;
    private double _lastY;

    private double _lastFireTime;


    public void Update(GameField gameField, long currentTime)
    {
        // Update the tank's AI
        var tankAction = TankAi.Update(new SensorData(), gameField, currentTime, this);

        TankRotation = tankAction.TankRotation;
        TurretRotation = tankAction.TurretRotation;

        if (tankAction.Fire && Ammo > 0 && currentTime - _lastFireTime > TankShotCooldownMs)
        {
            gameField.Projectiles.Add(new Projectile(BarrelEndPos, TurretRotation));
            Ammo--;
            _lastFireTime = currentTime;
        }

        var requestedVelocityPercent = tankAction.TankVelocity;
        requestedVelocityPercent = Math.Clamp(requestedVelocityPercent, -1, 1);
        TankVelocity = requestedVelocityPercent * MaxSpeed;

        var newX = _lastX + (Math.Cos(TankRotation) * TankVelocity);
        var newY = _lastY + (Math.Sin(TankRotation) * TankVelocity);

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