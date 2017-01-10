using System;

namespace ParticleEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ParticleEditorGame game = new ParticleEditorGame())
            {
                game.Run();
            }
        }
    }
}

