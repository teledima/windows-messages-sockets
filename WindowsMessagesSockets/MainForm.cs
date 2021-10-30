using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsMessagesSockets
{
    public partial class MainForm : Form
    {
        private string initial_directory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        public MainForm()
        {
            InitializeComponent();
            openDbFileDialog.InitialDirectory = initial_directory;
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (openDbFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = openDbFileDialog.SafeFileName;
            } 
        }
    }
}
