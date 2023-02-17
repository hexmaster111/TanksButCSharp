using System.Diagnostics;
using DrawingSomeTanks;
using DrawingSomeTanks.TankAis;
using static SDL2.SDL;

internal class Program
{
    public static IntPtr Window = IntPtr.Zero;
    public static IntPtr Renderer = IntPtr.Zero;
    public static IntPtr Font = IntPtr.Zero;

    public const int ScreenWidth = 640;
    public const int ScreenHeight = 480;

    public static bool Running = true;

    public static Game Game = new Game();
    public static int Fps = 0;

    public static void Main(string[] args)
    {
        if (!SetupSdl())
        {
            Console.WriteLine("Failed to setup SDL");
            Environment.Exit(-1);
        }


        Game.StartNewGame(new List<ITankAi>()
        {
            new BasicTestAi(),
            new BasicTestAi(),
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
            Fps = (int)(1000 / deltaTime);

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
        RenderFps();
        SDL_RenderPresent(Renderer);
    }

    private static void RenderFps()
    {
        var fpsText = $"FPS: {Fps:00}";
        var FPSSurface = SDL2.SDL_ttf.TTF_RenderText_Solid(Font, fpsText,
        new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF });
        var fpsTexture = SDL_CreateTextureFromSurface(Renderer, FPSSurface);
        SDL_Rect fpsRect = new SDL_Rect()
        {
            x = 0,
            y = 0,
            w = 100 / 2,
            h = 24 / 2
        };
        SDL_RenderCopy(Renderer, fpsTexture, IntPtr.Zero, ref fpsRect);
        SDL_DestroyTexture(fpsTexture);
        SDL_FreeSurface(FPSSurface);
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

        if (SDL2.SDL_ttf.TTF_Init() < 0)
        {
            Console.WriteLine($"SDL_ttf could not initialize! SDL_ttf Error:" +
            $"{SDL2.SDL_ttf.TTF_GetError()}");
            return false;
        }

        Font = SDL2.SDL_ttf.TTF_OpenFont("Assets/TerminusTTF.ttf", 12);
        if (Font == IntPtr.Zero)
        {
            Console.WriteLine($"Failed to load font! SDL_ttf Error: {SDL2.SDL_ttf.TTF_GetError()}");
            return false;
        }


        return true;
    }
}


public static class SDLExtentions
{
    public static SDL_Color ToSdlColor(this System.Drawing.Color color)
    {
        return new SDL_Color()
        {
            r = color.R,
            g = color.G,
            b = color.B,
            a = color.A
        };
    }
}