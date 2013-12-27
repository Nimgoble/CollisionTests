using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                tests = new Tests(_window);
                Stopwatch watch = new Stopwatch();
                watch.Start();

                Stopwatch pauseWatch = new Stopwatch();
                pauseWatch.Start();
                while (_window.IsOpen())
                {
                    _window.DispatchEvents();
                    _window.Clear();

                    if (watch.ElapsedMilliseconds > 500)
                    {
                        if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Left) && currentTest > 0)
                        {
                            --currentTest;
                            watch.Reset();
                            watch.Start();
                            tests.TestCounter = 1;
                            tests.Paused = false;
                        }
                        else if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Right) && currentTest < 6)
                        {
                            ++currentTest;
                            watch.Reset();
                            watch.Start();
                            tests.TestCounter = 1;
                            tests.Paused = false;
                        }
                    }

                    if (pauseWatch.ElapsedMilliseconds > 250)
                    {
                        if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.Space))
                        {
                            tests.Paused = !tests.Paused;
                            pauseWatch.Reset();
                            pauseWatch.Start();
                        }
                    }

                    RunCurrentTest();

                    _window.Display();
                }
            }
            void OnClosed(object sender, EventArgs e)
            {
                _window.Close();
            }

            private Int32 currentTest = 0;
            private Tests tests;
            private void RunCurrentTest()
            {
                switch (currentTest)
                {
                    case 0:
                        tests.AABBProjectionTestA();
                        break;
                    case 1:
                        tests.AABBProjectionTestB();
                        break;
                    case 2:
                        tests.AABBProjectionTestC();
                        break;
                    case 3:
                        tests.AABBProjectionTestD();
                        break;
                    case 4:
                        tests.AABBProjectionTestE();
                        break;
                    case 5:
                        tests.SimulateWorld();
                        break;
                }
            }
        }
    }
}
