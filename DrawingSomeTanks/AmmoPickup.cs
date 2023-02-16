using System.Drawing;
using SDL2;

namespace DrawingSomeTanks;

public class AmmoPickup
{
    public Point Position;
    public const int AmmoPickupSize = 5;

    public AmmoPickup(Point position)
    {
        Position = position;
    }

    public void Render(IntPtr renderer)
    {
        var rect = new SDL.SDL_Rect
        {
            x = Position.X - (AmmoPickupSize / 2),
            y = Position.Y - (AmmoPickupSize / 2),
            w = AmmoPickupSize,
            h = AmmoPickupSize
        };
        SDL.SDL_SetRenderDrawColor(renderer, 0x66, 0xFF, 0x66, 0xFF);
        SDL.SDL_RenderFillRect(renderer, ref rect);
    }
}