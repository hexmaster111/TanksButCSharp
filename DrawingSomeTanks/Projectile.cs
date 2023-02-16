using System.Drawing;
using SDL2;

namespace DrawingSomeTanks;

public class Projectile
{
    public const double Velocity = 4;

    public Point Position;
    public double Rotation;

    public void Update()
    {
        Position = new Point(
            Position.X + (int)(Math.Cos(Rotation) * Velocity),
            Position.Y + (int)(Math.Sin(Rotation) * Velocity)
        );
    }

    public bool IsOutOfBounds(int width, int height)
    {
        return Position.X < 0 || Position.X > width || Position.Y < 0 || Position.Y > height;
    }

    public bool IsCollidingWithTank(Tank tank)
    {
        return tank.Position.X - 5 < Position.X && tank.Position.X + 5 > Position.X &&
               tank.Position.Y - 5 < Position.Y && tank.Position.Y + 5 > Position.Y;
    }

    public void Render(IntPtr renderer)
    {
        const int projectileLength = 3;
        var projectileLine = new SDL.SDL_Point[2];
        projectileLine[0] = new SDL.SDL_Point
        {
            x = Position.X,
            y = Position.Y
        };

        projectileLine[1] = new SDL.SDL_Point
        {
            x = Position.X + (int)(Math.Cos(Rotation) * projectileLength),
            y = Position.Y + (int)(Math.Sin(Rotation) * projectileLength)
        };

        SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL.SDL_RenderDrawLines(renderer, projectileLine, 2);
    }

    public Projectile(Point position, double rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}