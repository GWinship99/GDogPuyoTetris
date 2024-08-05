using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDogPuyoTetris
{
    public class Renderer : IDisposable
    {
        public nint m_Handle;

        public void Dispose()
        {
            if (m_Handle != nint.Zero)
                SDL.SDL_DestroyRenderer(m_Handle);
            GC.SuppressFinalize(this);
        }

        ~Renderer()
        {
            Dispose();
        }
    }
}

