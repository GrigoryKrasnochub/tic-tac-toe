﻿namespace Cross
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.startNewGameButton = new System.Windows.Forms.Button();
            this.resetGameButton = new System.Windows.Forms.Button();
            this.startServerButton = new System.Windows.Forms.Button();
            this.connectToServerButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chatTextBox = new System.Windows.Forms.RichTextBox();
            this.sendMessageTextBox = new System.Windows.Forms.TextBox();
            this.sendTextButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ScoreLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(831, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = ":";
            // 
            // startNewGameButton
            // 
            this.startNewGameButton.Location = new System.Drawing.Point(233, 16);
            this.startNewGameButton.Name = "startNewGameButton";
            this.startNewGameButton.Size = new System.Drawing.Size(100, 23);
            this.startNewGameButton.TabIndex = 2;
            this.startNewGameButton.Text = "Новая игра";
            this.startNewGameButton.UseVisualStyleBackColor = true;
            this.startNewGameButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // resetGameButton
            // 
            this.resetGameButton.Location = new System.Drawing.Point(381, 16);
            this.resetGameButton.Name = "resetGameButton";
            this.resetGameButton.Size = new System.Drawing.Size(75, 23);
            this.resetGameButton.TabIndex = 3;
            this.resetGameButton.Text = "Сброс";
            this.resetGameButton.UseVisualStyleBackColor = true;
            this.resetGameButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // startServerButton
            // 
            this.startServerButton.Location = new System.Drawing.Point(529, 16);
            this.startServerButton.Margin = new System.Windows.Forms.Padding(2);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(128, 23);
            this.startServerButton.TabIndex = 4;
            this.startServerButton.Text = "Запустить сервер";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.startServerButton_Click);
            // 
            // connectToServerButton
            // 
            this.connectToServerButton.Location = new System.Drawing.Point(767, 171);
            this.connectToServerButton.Margin = new System.Windows.Forms.Padding(2);
            this.connectToServerButton.Name = "connectToServerButton";
            this.connectToServerButton.Size = new System.Drawing.Size(105, 23);
            this.connectToServerButton.TabIndex = 6;
            this.connectToServerButton.Text = "Подключиться";
            this.connectToServerButton.UseVisualStyleBackColor = true;
            this.connectToServerButton.Click += new System.EventHandler(this.connectToServerButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(663, 173);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(93, 20);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = "127.0.0.1";
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(663, 316);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.ReadOnly = true;
            this.chatTextBox.Size = new System.Drawing.Size(233, 249);
            this.chatTextBox.TabIndex = 8;
            this.chatTextBox.Text = "";
            // 
            // sendMessageTextBox
            // 
            this.sendMessageTextBox.Location = new System.Drawing.Point(665, 574);
            this.sendMessageTextBox.Name = "sendMessageTextBox";
            this.sendMessageTextBox.Size = new System.Drawing.Size(153, 20);
            this.sendMessageTextBox.TabIndex = 9;
            this.sendMessageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sendMessageTextBox_KeyDown);
            // 
            // sendTextButton
            // 
            this.sendTextButton.Location = new System.Drawing.Point(830, 572);
            this.sendTextButton.Name = "sendTextButton";
            this.sendTextButton.Size = new System.Drawing.Size(62, 23);
            this.sendTextButton.TabIndex = 10;
            this.sendTextButton.Text = "Send";
            this.sendTextButton.UseVisualStyleBackColor = true;
            this.sendTextButton.Click += new System.EventHandler(this.SendTextButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(662, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Счет:";
            // 
            // ScoreLabel
            // 
            this.ScoreLabel.AutoSize = true;
            this.ScoreLabel.Location = new System.Drawing.Point(717, 81);
            this.ScoreLabel.Name = "ScoreLabel";
            this.ScoreLabel.Size = new System.Drawing.Size(0, 15);
            this.ScoreLabel.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScrollMargin = new System.Drawing.Size(0, 5);
            this.ClientSize = new System.Drawing.Size(916, 670);
            this.Controls.Add(this.ScoreLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sendTextButton);
            this.Controls.Add(this.sendMessageTextBox);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.connectToServerButton);
            this.Controls.Add(this.startServerButton);
            this.Controls.Add(this.resetGameButton);
            this.Controls.Add(this.startNewGameButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.Text = "Tic-tac-toe";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button startNewGameButton;
        private System.Windows.Forms.Button resetGameButton;
        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Button connectToServerButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox chatTextBox;
        private System.Windows.Forms.TextBox sendMessageTextBox;
        private System.Windows.Forms.Button sendTextButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label ScoreLabel;
    }
}

