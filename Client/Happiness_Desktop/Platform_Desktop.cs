using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace Happiness_Desktop
{
    public class Platform_Desktop : Happiness.Platform
    {
        public Game TheGame { get; set; }

        public Platform_Desktop()
        {
            _instance = this;
        }

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public override bool ApplicationIsActivated()
        {         
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public override void ExitApp()
        {
            TheGame.Exit();
        }

        public override Happiness.Vector2 RotateAroundAxis_Z(float rotation)
        {
            Matrix rot = Matrix.CreateFromAxisAngle(Vector3.UnitZ, rotation);
            Vector3 direction3 = Vector3.Normalize(Vector3.Transform(Vector3.UnitX, rot));
            Vector2 direction = new Vector2(direction3.X, direction3.Y);

            return Renderer_XNA.XNAV2ToHappinessV2(direction);

        }
    }
}
