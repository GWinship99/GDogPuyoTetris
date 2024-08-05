using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDogPuyoTetris
{
    public class Window : IDisposable
    {
        public nint m_Handle;

        public void Dispose()
        {
            if(m_Handle != nint.Zero)
                SDL.SDL_DestroyWindow(m_Handle);
            GC.SuppressFinalize(this);
        }

        ~Window() { 
            Dispose();
        }
    }
}
