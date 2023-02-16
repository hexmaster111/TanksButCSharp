using System.Diagnostics;
using DrawingSomeTanks;
using DrawingSomeTanks.TankAis;
using static SDL2.SDL;

internal class Program
{
    public static IntPtr Window = IntPtr.Zero;
    public static IntPtr Renderer = IntPtr.Zero;

    public const int ScreenWidth = 640;
    public const int ScreenHeight = 480;

    public static bool Running = true;

    public static Game Game = new Game();

    public static void Main(string[] args)
    {
        if (!SetupSdl())
        {
            Console.WriteLine("Failed to setup SDL");
            Environment.Exit(-1);
        }


        Game.StartNewGame(new List<ITankAi>()
        {
            new BasicTestAi()
        });


        long lastTime = SDL_GetTicks();
        long currentTime = SDL_GetTicks();
        long deltaTime;

        while (Running)
        {
            currentTime = SDL_GetTicks();
            deltaTime = currentTime - lastTime;
            if (deltaTime < 1000 / 60)
            {
                continue;
            }

            lastTime = currentTime;

            HandleEvents();
            Game.Update(currentTime);
            Render();
        }
    }

    private static void HandleEvents()
    {
        SDL_Event e;
        while (SDL_PollEvent(out e) != 0)
        {
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    Running = false;
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    Game.DebugUpdate(e.key.keysym.sym);
                    break;
            }
        }
    }

    private static void Render()
    {
        SDL_SetRenderDrawColor(Renderer, 0x00, 0x00, 0x00, 0xFF);
        SDL_RenderClear(Renderer);
        Game.Render(Renderer);
        SDL_RenderPresent(Renderer);
    }

    private static bool SetupSdl()
    {
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL_GetError()}");
            return false;
        }

        var res = SDL_CreateWindowAndRenderer(ScreenWidth, ScreenHeight,
            SDL_WindowFlags.SDL_WINDOW_SHOWN,
            out Window,
            out Renderer);

        if (res < 0)
        {
            Console.WriteLine($"SDL could not create window and renderer! SDL_Error: {SDL_GetError()}");
            return false;
        }

        Debug.Assert(Window != IntPtr.Zero);
        Debug.Assert(Renderer != IntPtr.Zero);

        return true;
    }
}