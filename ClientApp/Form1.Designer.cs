namespace ClientApp
{
    partial class ClientAppForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientAppForm));
            this.ClientSubmitButton = new System.Windows.Forms.Button();
            this.StudentLabel = new System.Windows.Forms.Label();
            this.ClientAnswerGroupBox = new System.Windows.Forms.GroupBox();
            this.SystemMsgLabel = new System.Windows.Forms.Label();
            this.ClientAnswerLabel = new System.Windows.Forms.Label();
            this.ClientAnswerTextBox = new System.Windows.Forms.TextBox();
            this.ClientQuestionLabel = new System.Windows.Forms.Label();
            this.ClientQuestionTextBox = new System.Windows.Forms.TextBox();
            this.ClientExitButton = new System.Windows.Forms.Button();
            this.ClientAnswerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClientSubmitButton
            // 
            this.ClientSubmitButton.Location = new System.Drawing.Point(300, 122);
            this.ClientSubmitButton.Name = "ClientSubmitButton";
            this.ClientSubmitButton.Size = new System.Drawing.Size(105, 25);
            this.ClientSubmitButton.TabIndex = 8;
            this.ClientSubmitButton.Text = "Submit";
            this.ClientSubmitButton.UseVisualStyleBackColor = true;
            this.ClientSubmitButton.Click += new System.EventHandler(this.ClientSubmitButton_Click);
            // 
            // StudentLabel
            // 
            this.StudentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StudentLabel.BackColor = System.Drawing.SystemColors.HotTrack;
            this.StudentLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StudentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StudentLabel.ForeColor = System.Drawing.Color.White;
            this.StudentLabel.Location = new System.Drawing.Point(7, 9);
            this.StudentLabel.Margin = new System.Windows.Forms.Padding(0);
            this.StudentLabel.Name = "StudentLabel";
            this.StudentLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.StudentLabel.Size = new System.Drawing.Size(411, 50);
            this.StudentLabel.TabIndex = 7;
            this.StudentLabel.Text = "Student";
            this.StudentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ClientAnswerGroupBox
            // 
            this.ClientAnswerGroupBox.Controls.Add(this.SystemMsgLabel);
            this.ClientAnswerGroupBox.Controls.Add(this.ClientSubmitButton);
            this.ClientAnswerGroupBox.Controls.Add(this.ClientAnswerLabel);
            this.ClientAnswerGroupBox.Controls.Add(this.ClientAnswerTextBox);
            this.ClientAnswerGroupBox.Controls.Add(this.ClientQuestionLabel);
            this.ClientAnswerGroupBox.Controls.Add(this.ClientQuestionTextBox);
            this.ClientAnswerGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientAnswerGroupBox.Location = new System.Drawing.Point(7, 79);
            this.ClientAnswerGroupBox.Name = "ClientAnswerGroupBox";
            this.ClientAnswerGroupBox.Size = new System.Drawing.Size(411, 226);
            this.ClientAnswerGroupBox.TabIndex = 1;
            this.ClientAnswerGroupBox.TabStop = false;
            this.ClientAnswerGroupBox.Text = "Enter your answer and click SUBMIT";
            // 
            // SystemMsgLabel
            // 
            this.SystemMsgLabel.AutoSize = true;
            this.SystemMsgLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SystemMsgLabel.Location = new System.Drawing.Point(6, 198);
            this.SystemMsgLabel.Name = "SystemMsgLabel";
            this.SystemMsgLabel.Size = new System.Drawing.Size(62, 15);
            this.SystemMsgLabel.TabIndex = 9;
            this.SystemMsgLabel.Text = "Initializing";
            // 
            // ClientAnswerLabel
            // 
            this.ClientAnswerLabel.AutoSize = true;
            this.ClientAnswerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientAnswerLabel.Location = new System.Drawing.Point(6, 87);
            this.ClientAnswerLabel.Name = "ClientAnswerLabel";
            this.ClientAnswerLabel.Size = new System.Drawing.Size(85, 16);
            this.ClientAnswerLabel.TabIndex = 6;
            this.ClientAnswerLabel.Text = "Your Answer:";
            // 
            // ClientAnswerTextBox
            // 
            this.ClientAnswerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ClientAnswerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientAnswerTextBox.Location = new System.Drawing.Point(165, 83);
            this.ClientAnswerTextBox.Name = "ClientAnswerTextBox";
            this.ClientAnswerTextBox.Size = new System.Drawing.Size(240, 24);
            this.ClientAnswerTextBox.TabIndex = 5;
            // 
            // ClientQuestionLabel
            // 
            this.ClientQuestionLabel.AutoSize = true;
            this.ClientQuestionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientQuestionLabel.Location = new System.Drawing.Point(6, 47);
            this.ClientQuestionLabel.Name = "ClientQuestionLabel";
            this.ClientQuestionLabel.Size = new System.Drawing.Size(63, 16);
            this.ClientQuestionLabel.TabIndex = 4;
            this.ClientQuestionLabel.Text = "Question:";
            // 
            // ClientQuestionTextBox
            // 
            this.ClientQuestionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ClientQuestionTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientQuestionTextBox.Location = new System.Drawing.Point(165, 43);
            this.ClientQuestionTextBox.Name = "ClientQuestionTextBox";
            this.ClientQuestionTextBox.Size = new System.Drawing.Size(240, 24);
            this.ClientQuestionTextBox.TabIndex = 0;
            // 
            // ClientExitButton
            // 
            this.ClientExitButton.Location = new System.Drawing.Point(307, 323);
            this.ClientExitButton.Name = "ClientExitButton";
            this.ClientExitButton.Size = new System.Drawing.Size(105, 25);
            this.ClientExitButton.TabIndex = 9;
            this.ClientExitButton.Text = "Exit";
            this.ClientExitButton.UseVisualStyleBackColor = true;
            this.ClientExitButton.Click += new System.EventHandler(this.ClientExitButton_Click);
            // 
            // ClientAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 379);
            this.Controls.Add(this.ClientExitButton);
            this.Controls.Add(this.StudentLabel);
            this.Controls.Add(this.ClientAnswerGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientAppForm";
            this.Text = "Student";
            this.ClientAnswerGroupBox.ResumeLayout(false);
            this.ClientAnswerGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ClientSubmitButton;
        private System.Windows.Forms.Label StudentLabel;
        private System.Windows.Forms.GroupBox ClientAnswerGroupBox;
        private System.Windows.Forms.Label ClientQuestionLabel;
        private System.Windows.Forms.TextBox ClientQuestionTextBox;
        private System.Windows.Forms.Label ClientAnswerLabel;
        private System.Windows.Forms.TextBox ClientAnswerTextBox;
        private System.Windows.Forms.Button ClientExitButton;
        private System.Windows.Forms.Label SystemMsgLabel;
    }
}

