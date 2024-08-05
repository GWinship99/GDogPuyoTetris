using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDogPuyoTetris
{
    public static class PuyoGrid
    {

        //the enum for the different puyo states/colours as well as an empty spot in the grid.
        public enum PuyoType
        {
            Nothing,
            Red,
            Blue,
            Purple,
            Yellow,
            Green
        }

        // the array that represents the grid full of puyos,
        public static PuyoType[,] puyos = new PuyoType[6,12];
        public static void DrawGrid()
        {
            for(int i = 0; i < puyos.GetLength(0); i++)
            {
                for(int j = 0; j < puyos.GetLength(1); j++)
                {
                    var puyoTexture = Program.GetPuyoTexture(puyos[i,j]);
                    if (puyoTexture != nint.Zero)
                    {
                        SDL.SDL_QueryTexture(puyoTexture, out uint format, out int access, out int puyoWidth, out int puyoHeight);
                        SDL.SDL_Rect rectangle2 = new SDL.SDL_Rect { x = i * 16 + 16, y = j * 16 + 16, w = puyoWidth, h = puyoHeight };
                        SDL.SDL_RenderCopy(Program.renderer.m_Handle, puyoTexture, nint.Zero, ref rectangle2);
                    }
                }
            }
            
        }
        public static void SetPuyo(int x,  int y, PuyoType colour)
        {
            puyos[(x - 16) / 16, (y - 16) / 16] = colour;
        }

        //code to handle the formula for when a puyo collides with something
        public static bool CollidePuyo(int x, int y)
        {
            return puyos[(x - 16) / 16, (y - 16) / 16] != PuyoType.Nothing;
        }

        // a way to get the puyos withoout causing any crashing
        public static PuyoType GetPuyoSecurely(int x, int y)
        {
            if (x < 0 || y < 0 || x >= puyos.GetLength(0) || y >= puyos.GetLength(1))
            {
                return PuyoType.Nothing;
            }
            return puyos[x,y];
        }
    }
}
