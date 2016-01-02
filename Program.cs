using System;

namespace Happiness
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Happiness game = new Happiness())
            {
                Happiness.Game = game;
                game.Run();
            }
        }
    }
}

