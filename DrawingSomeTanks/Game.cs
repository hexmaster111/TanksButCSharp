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

            if (GameField.AmmoPickups.Any(t.IsCollidingWithAmmoPickup))
            {
                t.Ammo += 1;
                t.Ammo = Math.Clamp(t.Ammo, 0, Tank.TankMaxAmmo);
                GameField.AmmoPickups.RemoveAll(t.IsCollidingWithAmmoPickup);
            }
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


        if (GameField.Tanks.Count == 1)
            StartNewGame(new List<ITankAi> { new BasicTestAi(), new DogingAi() });
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

        foreach (var item in tankAis)
        {
            var tank = new Tank
            (
                item,
                Color.FromArgb(
                    Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255),
                    Random.Shared.Next(0, 255)
                ),
                new Point(
                    Random.Shared.Next(0, GameField.Width),
                    Random.Shared.Next(0, GameField.Height))
            );
            GameField.Tanks.Add(tank);
        }
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