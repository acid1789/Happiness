using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Microsoft.Xna.Framework;
using Happiness_Shared;

namespace Happiness_Android
{
    class Platform_Android : Happiness.Platform
    {
        public Game TheGame { get; set; }

        public Platform_Android()
        {
            _instance = this;
        }

        public override bool ApplicationIsActivated()
        {
            return true;
        }

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