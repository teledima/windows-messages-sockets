
namespace WindowsMessagesSockets
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.updateLabel += updateHistory;
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.openDbFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.labelTypeSend = new System.Windows.Forms.Label();
            this.radioButtonSockets = new System.Windows.Forms.RadioButton();
            this.buttonSend = new System.Windows.Forms.Button();
            this.labelHistory = new System.Windows.Forms.Label();
            this.radioButtonMessages = new System.Windows.Forms.RadioButton();
            this.radioButtonGrpc = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SendData);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SendDataFinished);
            // 
            // openDbFileDialog
            // 
            this.openDbFileDialog.FileName = "openFileDialog1";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(13, 13);
            this.textBoxFileName.Multiline = true;
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.ReadOnly = true;
            this.textBoxFileName.Size = new System.Drawing.Size(284, 25);
            this.textBoxFileName.TabIndex = 0;
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(323, 13);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(111, 25);
            this.buttonSelectFile.TabIndex = 1;
            this.buttonSelectFile.Text = "Выбрать файл";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // labelTypeSend
            // 
            this.labelTypeSend.AutoSize = true;
            this.labelTypeSend.Location = new System.Drawing.Point(13, 57);
            this.labelTypeSend.Name = "labelTypeSend";
            this.labelTypeSend.Size = new System.Drawing.Size(76, 13);
            this.labelTypeSend.TabIndex = 2;
            this.labelTypeSend.Text = "Тип отправки";
            // 
            // radioButtonSockets
            // 
            this.radioButtonSockets.AutoSize = true;
            this.radioButtonSockets.Location = new System.Drawing.Point(16, 74);
            this.radioButtonSockets.Name = "radioButtonSockets";
            this.radioButtonSockets.Size = new System.Drawing.Size(64, 17);
            this.radioButtonSockets.TabIndex = 3;
            this.radioButtonSockets.TabStop = true;
            this.radioButtonSockets.Text = "Sockets";
            this.radioButtonSockets.UseVisualStyleBackColor = true;
            // 
            // buttonSend
            // 
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(9, 157);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(425, 23);
            this.buttonSend.TabIndex = 4;
            this.buttonSend.Text = "Отправить";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // labelHistory
            // 
            this.labelHistory.AutoEllipsis = true;
            this.labelHistory.AutoSize = true;
            this.labelHistory.Location = new System.Drawing.Point(13, 162);
            this.labelHistory.Name = "labelHistory";
            this.labelHistory.Size = new System.Drawing.Size(0, 13);
            this.labelHistory.TabIndex = 5;
            this.labelHistory.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelHistory_MouseClick);
            // 
            // radioButtonMessages
            // 
            this.radioButtonMessages.AutoSize = true;
            this.radioButtonMessages.Checked = true;
            this.radioButtonMessages.Location = new System.Drawing.Point(16, 97);
            this.radioButtonMessages.Name = "radioButtonMessages";
            this.radioButtonMessages.Size = new System.Drawing.Size(103, 17);
            this.radioButtonMessages.TabIndex = 6;
            this.radioButtonMessages.TabStop = true;
            this.radioButtonMessages.Text = "Message Queue";
            this.radioButtonMessages.UseVisualStyleBackColor = true;
            // 
            // radioButtonGrpc
            // 
            this.radioButtonGrpc.AutoSize = true;
            this.radioButtonGrpc.Location = new System.Drawing.Point(16, 120);
            this.radioButtonGrpc.Name = "radioButtonGrpc";
            this.radioButtonGrpc.Size = new System.Drawing.Size(53, 17);
            this.radioButtonGrpc.TabIndex = 7;
            this.radioButtonGrpc.TabStop = true;
            this.radioButtonGrpc.Text = "gRPC";
            this.radioButtonGrpc.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 300);
            this.Controls.Add(this.radioButtonGrpc);
            this.Controls.Add(this.radioButtonMessages);
            this.Controls.Add(this.labelHistory);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.radioButtonSockets);
            this.Controls.Add(this.labelTypeSend);
            this.Controls.Add(this.buttonSelectFile);
            this.Controls.Add(this.textBoxFileName);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openDbFileDialog;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Label labelTypeSend;
        private System.Windows.Forms.RadioButton radioButtonSockets;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label labelHistory;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private HelperSockets.updateLabelDelegate updateLabel;
        private System.Windows.Forms.RadioButton radioButtonMessages;
        private System.Windows.Forms.RadioButton radioButtonGrpc;
    }
}

