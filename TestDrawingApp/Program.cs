using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace TestDrawingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MySFMLProgram app = new MySFMLProgram();
            app.StartSFMLProgram();
        }

        class MySFMLProgram
        {
            RenderWindow _window;
            public void StartSFMLProgram()
            {
                _window = new RenderWindow(new VideoMode(800, 600), "SFML window");
                _window.SetVisible(true);
                _window.Closed += new EventHandler(OnClosed);

                Tests tests = new Tests(_window);
                while (_window.IsOpen())
                {
                    _window.DispatchEvents();
                    _window.Clear();
                    tests.TestAABBProjectionList();
                    _window.Display();
                }
            }
            void OnClosed(object sender, EventArgs e)
            {
                _window.Close();
            }
        }
    }
}
