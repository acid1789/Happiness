using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatchLib;

namespace HappinessLauncher
{
    public partial class GameUpdate : Form
    {
        bool _patchingGame;

        public GameUpdate()
        {
            InitializeComponent();
        }

        private void GameUpdate_Load(object sender, EventArgs e)
        {

        }

        private void GameUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        public void Start(string patchServer)
        {
            // Update the game
            _patchingGame = true;
            UpdateManager updateManager = new UpdateManager(patchServer, "client/", "Happiness/");
            updateManager.OnStatusChange += UpdateStatusChanged;
            updateManager.OnUpdateFinished += UpdateFinished;
            updateManager.Start();
        }

        void UpdateStatusChanged(string message, float minor, float major)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { UpdateStatusChanged(message, minor, major); }));
            else
            {
                PatchLog.Print("PatchStatus: {0} - {1}/{2}", message, minor, major);
                progressBar1.Value = ((int)(major * 1000));
            }
        }

        void UpdateFinished(bool success)
        {
            // Launch happiness
            System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "/Happiness/Happiness.exe");

            // Mark done
            _patchingGame = false;
        }

        public bool Finished { get { return !_patchingGame; } }
    }
}
