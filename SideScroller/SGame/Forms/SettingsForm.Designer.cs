namespace RPG2D.SGame.Forms
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxResWidth = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxResHeight = new System.Windows.Forms.ComboBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelUI = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelUI.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Resolution:";
            // 
            // comboBoxResWidth
            // 
            this.comboBoxResWidth.FormattingEnabled = true;
            this.comboBoxResWidth.Location = new System.Drawing.Point(7, 26);
            this.comboBoxResWidth.Name = "comboBoxResWidth";
            this.comboBoxResWidth.Size = new System.Drawing.Size(121, 24);
            this.comboBoxResWidth.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(134, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "x";
            // 
            // comboBoxResHeight
            // 
            this.comboBoxResHeight.FormattingEnabled = true;
            this.comboBoxResHeight.Location = new System.Drawing.Point(157, 26);
            this.comboBoxResHeight.Name = "comboBoxResHeight";
            this.comboBoxResHeight.Size = new System.Drawing.Size(121, 24);
            this.comboBoxResHeight.TabIndex = 1;
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(768, 576);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(120, 44);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            // 
            // buttonBack
            // 
            this.buttonBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBack.Location = new System.Drawing.Point(12, 576);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(120, 44);
            this.buttonBack.TabIndex = 3;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(187, 91);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(100, 50);
            this.pictureBox.TabIndex = 4;
            this.pictureBox.TabStop = false;
            // 
            // panelUI
            // 
            this.panelUI.Controls.Add(this.textBoxName);
            this.panelUI.Controls.Add(this.label3);
            this.panelUI.Controls.Add(this.label1);
            this.panelUI.Controls.Add(this.comboBoxResWidth);
            this.panelUI.Controls.Add(this.comboBoxResHeight);
            this.panelUI.Controls.Add(this.label2);
            this.panelUI.Location = new System.Drawing.Point(27, 158);
            this.panelUI.Name = "panelUI";
            this.panelUI.Size = new System.Drawing.Size(386, 324);
            this.panelUI.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(7, 75);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(271, 22);
            this.textBoxName.TabIndex = 3;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(900, 632);
            this.Controls.Add(this.panelUI);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsForm";
            this.Text = "Souless";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelUI.ResumeLayout(false);
            this.panelUI.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxResWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxResHeight;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panelUI;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label3;
    }
}