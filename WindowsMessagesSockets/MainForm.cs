using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HelperSockets;
using System.Threading.Tasks;

namespace WindowsMessagesSockets
{
    public partial class MainForm : Form
    {
        private readonly string initial_directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        private string db_file_path;
        public MainForm()
        {
            InitializeComponent();
            labelHistory.MaximumSize = new Size { Height = labelHistory.MaximumSize.Height, Width = Width - 50};
            openDbFileDialog.InitialDirectory = initial_directory;
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (openDbFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = openDbFileDialog.SafeFileName;
                db_file_path = openDbFileDialog.FileName;
                buttonSend.Enabled = true;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(db_file_path))
                {
                    backgroundWorker.RunWorkerAsync();
                    buttonSend.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                backgroundWorker.CancelAsync();
                MessageBox.Show(ex.Message);
            }
        }

        private void SendData(object sender, DoWorkEventArgs args)
        {
            IDisplayMessage displayMessage = new DisplayLabel(labelHistory, updateHistory);

            try
            {
                var sourceGames = SourceGamesHelper.GetSource(db_file_path).Result;
                var client = new Client(displayMessage);
                client.SendData(sourceGames);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    displayMessage.Display(ex.InnerException.Message + '\n');
                else
                    displayMessage.Display(ex.Message + '\n');
            }
        }

        private void SendDataFinished(object sender, RunWorkerCompletedEventArgs args)
        {
            buttonSend.Enabled = true;
            Task.Run(() => SourceGamesHelper.ClearSourceGames(db_file_path));
        }

        private void updateHistory(string message)
        {
            labelHistory.Text += message;
        }
    }
}
