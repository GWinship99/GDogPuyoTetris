using GDogPuyoTetris;
using SDL2;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using static GDogPuyoTetris.PuyoGrid;


public class Program
{
    public static Renderer renderer = new Renderer();
    public static Window window = new Window();
    public static nint bluePuyoTexture;
    public static nint redPuyoTexture;
    public static nint purplePuyoTexture;
    public static nint yellowPuyoTexture;
    public static nint greenPuyoTexture;
    private static void Main(string[] args)
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_EVENTS | SDL.SDL_INIT_VIDEO) != 0)
        {
            throw new Exception("SDL_Init failed: " + SDL.SDL_GetError());
        }

        try
        {
            MainFunction();
        }
        catch
        {
            SDL.SDL_Quit();
            throw;
        }

        SDL.SDL_Quit();
    }
    static PuyoGrid.PuyoType puyoColour;
    private static void SetPuyoColour()
    {
        //sets the puyo to a random colour
        PuyoGrid.PuyoType[] puyoColours = { PuyoGrid.PuyoType.Red, PuyoGrid.PuyoType.Blue, PuyoGrid.PuyoType.Purple, PuyoGrid.PuyoType.Yellow, PuyoGrid.PuyoType.Green };
        Random random = new Random();
        puyoColour = puyoColours[random.Next(puyoColours.Length)];
    }

    public void PushPuyosDown()
    {

    }

    private static void MainFunction()
    {
        int puyoX = 96;
        int puyoY = 16;
        bool leftKey = false;
        bool rightKey = false;
        bool leftKeyPressed = false;
        bool rightKeyPressed = false;

        //creates the puyo puyo window and throws exception if the window creation fails for any reason
        SDL.SDL_SetHint("SDL_WINDOWS_DPI_SCALING", "1");
        window.m_Handle = SDL.SDL_CreateWindow("Puyo Puyo Tetris", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 1152, 720, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI);
        if (window.m_Handle == nint.Zero)
        {
            throw new Exception("SDL_CreateWindow failed: " + SDL.SDL_GetError());
        }

        //creates the render to render objects and throws exception if the renderer creation fails for any reason
        renderer.m_Handle = SDL.SDL_CreateRenderer(window.m_Handle, -1, SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (renderer.m_Handle == nint.Zero)
        {
            throw new Exception("SDL_CreateRenderer failed: " + SDL.SDL_GetError());
        }

        SDL.SDL_RenderSetLogicalSize(renderer.m_Handle, 320, 224);

        //loads the images for both the background and the different puyos
        nint LoadImage(string filename)
        {
            Bitmap bitmap = new Bitmap(filename);
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            PixelFormat format = PixelFormat.Format32bppArgb;
            BitmapData bmpData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, format);
            var texture = SDL.SDL_CreateTexture(renderer.m_Handle, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, bitmap.Width, bitmap.Height);
            SDL.SDL_SetTextureBlendMode(texture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            if (texture == nint.Zero)
            {
                throw new Exception("SDL_CreateTexture failed: " + SDL.SDL_GetError());
            }

            SDL.SDL_UpdateTexture(texture, nint.Zero, bmpData.Scan0, bmpData.Stride);
            bitmap.UnlockBits(bmpData);
            return texture;
        }

        var bkgTexture = LoadImage("image.png");
        bluePuyoTexture = LoadImage("blue.png");
        redPuyoTexture = LoadImage("red.png");
        yellowPuyoTexture = LoadImage("yellow.png");
        greenPuyoTexture = LoadImage("green.png");
        purplePuyoTexture = LoadImage("purple.png");

        bool keepsRunning = true;
        int speedometer = 0;
        SetPuyoColour();
        while (keepsRunning)
        {
            speedometer++;
            leftKeyPressed = false;
            rightKeyPressed = false;

            // event that signals when a key is pressed or the game is exited
            while (SDL.SDL_PollEvent(out SDL.SDL_Event sdlEvent) == 1)
            {
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        keepsRunning = false;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                    case SDL.SDL_EventType.SDL_KEYUP:
                        if (sdlEvent.key.repeat == 0)
                        {
                            var keyDown = sdlEvent.key.type == SDL.SDL_EventType.SDL_KEYDOWN;
                            if (sdlEvent.key.keysym.scancode == SDL.SDL_Scancode.SDL_SCANCODE_RIGHT)
                            {
                                rightKey = keyDown;
                                rightKeyPressed = keyDown;
                            }
                            else if (sdlEvent.key.keysym.scancode == SDL.SDL_Scancode.SDL_SCANCODE_LEFT)
                            {
                                leftKey = keyDown;
                                leftKeyPressed = keyDown;
                            }
                        }
                        break;

                }
            }

            if (speedometer == 1)
            {
                speedometer = 0;
                puyoY = puyoY + 1;
            }

            // code to move the puyos left and right
            if (rightKeyPressed)
            {
                int rightPuyoCollisionX = puyoX + 16;
                int rightPuyoCollisionY = puyoY + 16;
                if (puyoX < 16 * 6 && !PuyoGrid.CollidePuyo(rightPuyoCollisionX, rightPuyoCollisionY))
                {
                    puyoX = puyoX + 16;
                }
            }

            if (leftKeyPressed)
            {
                int leftPuyoCollisionX = puyoX - 16;
                int leftPuyoCollisionY = puyoY + 16;

                if (puyoX > 16 && !PuyoGrid.CollidePuyo(leftPuyoCollisionX, leftPuyoCollisionY))
                {
                    puyoX = puyoX - 16;
                }
            }

            // if puyo reaches bottom of the grid or collides with another puyo, then we teleport another puyo to the top
            if (puyoY == 192 || PuyoGrid.CollidePuyo(puyoX, puyoY + 16))
            {
                PuyoGrid.SetPuyo(puyoX, puyoY, puyoColour);
                SetPuyoColour();
                puyoY = 16;
                
                for (int xAxis = 0; xAxis < puyos.GetLength(0); xAxis++)
                {
                    for (int yAxis = 0; yAxis < puyos.GetLength(1); yAxis++)
                    {
                        static void SearchPuyo(int xAxis, int yAxis, bool[,] checkedPuyos, ref int totalPuyos)
                        {
                            totalPuyos++;

                            var firstcolour = GetPuyoSecurely(xAxis, yAxis);
                            checkedPuyos[xAxis, yAxis] = true;
                            if (GetPuyoSecurely(xAxis, yAxis - 1) == firstcolour && checkedPuyos[xAxis, yAxis - 1] == false)
                            {
                                SearchPuyo(xAxis, yAxis - 1, checkedPuyos, ref totalPuyos);
                            }

                            if (GetPuyoSecurely(xAxis, yAxis + 1) == firstcolour && checkedPuyos[xAxis, yAxis + 1] == false)
                            {
                                SearchPuyo(xAxis, yAxis + 1, checkedPuyos, ref totalPuyos);
                            }

                            if (GetPuyoSecurely(xAxis - 1, yAxis) == firstcolour && checkedPuyos[xAxis - 1, yAxis] == false)
                            {
                                SearchPuyo(xAxis - 1, yAxis, checkedPuyos,ref totalPuyos);
                            }

                            if (GetPuyoSecurely(xAxis + 1, yAxis) == firstcolour && checkedPuyos[xAxis + 1, yAxis] == false)
                            {
                                SearchPuyo(xAxis + 1, yAxis, checkedPuyos, ref totalPuyos);
                            }
                        }

                        static void DestroyPuyo(int xAxis, int yAxis, bool[,] checkedPuyos)
                        {
                            var firstcolour = puyos[xAxis, yAxis];
                            puyos[xAxis, yAxis] = PuyoType.Nothing;
                            checkedPuyos[xAxis, yAxis] = true;

                            if (GetPuyoSecurely(xAxis, yAxis - 1) == firstcolour && checkedPuyos[xAxis, yAxis - 1] == false)
                            {
                                DestroyPuyo(xAxis, yAxis - 1, checkedPuyos);
                            }

                            if (GetPuyoSecurely(xAxis, yAxis + 1) == firstcolour && checkedPuyos[xAxis, yAxis + 1] == false)
                            {
                                DestroyPuyo(xAxis, yAxis + 1, checkedPuyos);
                            }

                            if (GetPuyoSecurely(xAxis - 1, yAxis) == firstcolour && checkedPuyos[xAxis - 1, yAxis] == false)
                            {
                                DestroyPuyo(xAxis - 1, yAxis, checkedPuyos);
                            }

                            if (GetPuyoSecurely(xAxis + 1, yAxis) == firstcolour && checkedPuyos[xAxis + 1, yAxis] == false)
                            {
                                DestroyPuyo(xAxis + 1, yAxis, checkedPuyos);
                            }
                        }

                        if (puyos[xAxis,yAxis] == PuyoType.Nothing)
                        {
                            continue;
                        }

                        bool[,] checkedPuyos = new bool[puyos.GetLength(0), puyos.GetLength(1)];
                        int totalPuyos = 0;
                        SearchPuyo(xAxis, yAxis, checkedPuyos, ref totalPuyos);

                        if(totalPuyos >= 4)
                        {
                            for (int x = 0; x < checkedPuyos.GetLength(0); ++x)
                                for (int y = 0; y < checkedPuyos.GetLength(1); ++y)
                                    checkedPuyos[x, y] = false;

                            DestroyPuyo(xAxis,yAxis, checkedPuyos);
                        }
                    }
                }

                //call the function here
            }


            nint puyoTexture = GetPuyoTexture(puyoColour);
            if (puyoTexture != nint.Zero)
            {
                SDL.SDL_SetRenderDrawColor(renderer.m_Handle, 0, 0, 0, 255);
                SDL.SDL_RenderClear(renderer.m_Handle);
                SDL.SDL_RenderCopy(renderer.m_Handle, bkgTexture, nint.Zero, nint.Zero);
                PuyoGrid.DrawGrid();
                SDL.SDL_QueryTexture(puyoTexture, out uint format, out int access, out int puyoWidth, out int puyoHeight);
                SDL.SDL_Rect rectangle2 = new SDL.SDL_Rect { x = puyoX, y = puyoY, w = puyoWidth, h = puyoHeight };
                SDL.SDL_RenderCopy(renderer.m_Handle, puyoTexture, nint.Zero, ref rectangle2);
                SDL.SDL_RenderPresent(renderer.m_Handle);
            }
        }
    }

    public static nint GetPuyoTexture(PuyoType colour)
    {
        nint puyoTexture;
        switch (colour)
        {
            default:
            case PuyoType.Nothing:
                puyoTexture = nint.Zero; 
                break;
            case PuyoType.Red:
                puyoTexture = redPuyoTexture;
                break;
            case PuyoType.Blue:
                puyoTexture = bluePuyoTexture;
                break;
            case PuyoType.Purple:
                puyoTexture = purplePuyoTexture;
                break;
            case PuyoType.Yellow:
                puyoTexture = yellowPuyoTexture;
                break;
            case PuyoType.Green:
                puyoTexture = greenPuyoTexture;
                break;
        }

        return puyoTexture;
    }
}