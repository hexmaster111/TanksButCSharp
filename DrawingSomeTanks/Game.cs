using System.Drawing;
using DrawingSomeTanks.TankAis;
using SDL2;

namespace DrawingSomeTanks;

public class GameField
{
    public int Width => Program.ScreenWidth;
    public int Height => Program.ScreenHeight;

    public List<Tank> Tanks = new();
    public List<AmmoPickup> AmmoPickups = new();
    public List<Projectile> Projectiles = new();
}

public class Game
{
    public GameField GameField;

    public void Render(IntPtr renderer)
    {
        GameField.Tanks.ForEach(x => x.Render(renderer));
        GameField.AmmoPickups.ForEach(x => x.Render(renderer));
        GameField.Projectiles.ForEach(x => x.Render(renderer));
    }

    public void Update(long currentTime)
    {
        GameField.Tanks.ForEach(t =>
        {
            t.Update(GameField, currentTime);
        });

        GameField.Projectiles.ForEach(p =>
        {
            p.Update();
            var tank = GameField.Tanks.FirstOrDefault(p.IsCollidingWithTank);
            if (tank == null) return;
            tank.Health -= 1;
            if (tank.Health > 0) return;
            GameField.Tanks.Remove(tank);
        });

        GameField.Projectiles.RemoveAll(p => p.IsOutOfBounds(GameField.Width, GameField.Height) ||
                                             GameField.Tanks.Any(p.IsCollidingWithTank));
    }


    public void StartNewGame(List<ITankAi> tankAis)
    {
        GameField = new GameField();

        GameField.AmmoPickups = Enumerable.Range(0, 10)
            .Select(_ => new AmmoPickup(
                new Point(
                    Random.Shared.Next(0, GameField.Width),
                    Random.Shared.Next(0, GameField.Height)))
            ).ToList();

        GameField.Tanks = tankAis.Select(tankAi =>
        {
            var tank = new Tank
            (
                tankAi,
                Color.FromArgb(
                    Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255)
                ),
                new Point(
                    GameField.Width / 2,
                    GameField.Height / 2)
            );
            return tank;
        }).ToList();

        //Debug only
        GameField.Projectiles = Enumerable.Range(0, 10)
            .Select(_ =>
                new Projectile(
                    new Point(
                        Random.Shared.Next(0, GameField.Width),
                        Random.Shared.Next(0, GameField.Height)),
                    Random.Shared.NextDouble() * Math.PI * 2
                )
            ).ToList();
    }

    public void DebugUpdate(SDL.SDL_Keycode key)
    {
        switch (key)
        {
            case SDL.SDL_Keycode.SDLK_a:
                //Add some ammo pickups
                GameField.AmmoPickups = GameField.AmmoPickups.Concat(Enumerable.Range(0, 10)
                    .Select(_ => new AmmoPickup(
                        new Point(
                            Random.Shared.Next(0, Program.ScreenWidth),
                            Random.Shared.Next(0, Program.ScreenHeight)))
                    )).ToList();
                break;

            case SDL.SDL_Keycode.SDLK_b:
                //Add some projectiles
                GameField.Projectiles = GameField.Projectiles.Concat(Enumerable.Range(0, 10)
                    .Select(_ =>
                        new Projectile(
                            new Point(
                                Random.Shared.Next(0, Program.ScreenWidth),
                                Random.Shared.Next(0, Program.ScreenHeight)),
                            Random.Shared.NextDouble() * Math.PI * 2
                        )
                    )).ToList();
                break;
            
            case SDL.SDL_Keycode.SDLK_t:
                //Add some random tanks
                GameField.Tanks = GameField.Tanks.Concat(Enumerable.Range(0, 10)
                    .Select(_ =>
                    {
                        var tank = new Tank
                        (
                            new BasicTestAi(),
                            Color.FromArgb(
                                Random.Shared.Next(0, 255),
                                Random.Shared.Next(0, 255),
                                Random.Shared.Next(0, 255)
                            ),
                            new Point(
                                Random.Shared.Next(0, Program.ScreenWidth),
                                Random.Shared.Next(0, Program.ScreenHeight))
                        );
                        return tank;
                    })).ToList();
                break;

            default:
                foreach (var projectile in GameField.Projectiles)
                {
                    projectile.Update();
                }

                break;
        }
    }
}