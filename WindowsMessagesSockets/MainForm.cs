using System;
using System.ComponentModel;
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
            // if user select file
            if (openDbFileDialog.ShowDialog() == DialogResult.OK)
            {
                // update text box
                textBoxFileName.Text = openDbFileDialog.SafeFileName;
                // save filepath in properties
                Properties.Settings.Default["source_filepath"] = openDbFileDialog.FileName;
                // allow user to start export
                buttonSend.Enabled = true;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default["source_filepath"].ToString()))
                {
                    // Run send data to server
                    backgroundWorker.RunWorkerAsync();
                    // Block the submit button until the server processes the data 
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
            // background work started
            IDisplayMessage displayMessage = new DisplayLabel(labelHistory, updateHistory);

            try
            {
                // Try extract data from selected file
                var sourceGames = SourceGamesHelper.GetSource(Properties.Settings.Default["source_filepath"].ToString());
                // Initialize client instance
                if (radioButtonSockets.Checked)
                {
                    var client = new Client(displayMessage);
                    // Send data to server
                    client.SendData(sourceGames);
                }
                else if (radioButtonMessages.Checked)
                {
                    var client = new ClientMessages(displayMessage);
                    client.SendData(sourceGames);
                }
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
            // background work finished
            buttonSend.Enabled = true;
        }

        private void updateHistory(string message)
        {
            labelHistory.Text += message;
        }
    }
}
