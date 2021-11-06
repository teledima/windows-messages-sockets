using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HelperSockets;

namespace WindowsMessagesSockets
{
    public partial class MainForm : Form
    {
        private readonly string initial_directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
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
                Properties.Settings.Default["source_filepath"] = openDbFileDialog.FileName;
                buttonSend.Enabled = true;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default["source_filepath"].ToString()))
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
                var sourceGames = SourceGamesHelper.GetSource(Properties.Settings.Default["source_filepath"].ToString());
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
        }

        private void updateHistory(string message)
        {
            labelHistory.Text += message;
        }
    }
}
