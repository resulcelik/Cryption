using System.Windows.Forms;

namespace ResCryption
{
    partial class MainScreen
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
            this.comboBox_algorithm = new System.Windows.Forms.ComboBox();
            this.button_encrypt = new System.Windows.Forms.Button();
            this.button_decrypt = new System.Windows.Forms.Button();
            this.checkBox_deleteOrgFile = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label_FileName = new System.Windows.Forms.Label();
            this.button_File_Select = new System.Windows.Forms.Button();
            this.comboBox_cipherMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBox_algorithm
            // 
            this.comboBox_algorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_algorithm.FormattingEnabled = true;
            this.comboBox_algorithm.Location = new System.Drawing.Point(42, 52);
            this.comboBox_algorithm.Name = "comboBox_algorithm";
            this.comboBox_algorithm.Size = new System.Drawing.Size(121, 21);
            this.comboBox_algorithm.TabIndex = 0;
            // 
            // button_encrypt
            // 
            this.button_encrypt.Location = new System.Drawing.Point(228, 66);
            this.button_encrypt.Name = "button_encrypt";
            this.button_encrypt.Size = new System.Drawing.Size(75, 23);
            this.button_encrypt.TabIndex = 1;
            this.button_encrypt.Text = "Şifrele";
            this.button_encrypt.UseVisualStyleBackColor = true;
            this.button_encrypt.Click += new System.EventHandler(this.button_encrypt_Click);
            // 
            // button_decrypt
            // 
            this.button_decrypt.Location = new System.Drawing.Point(228, 107);
            this.button_decrypt.Name = "button_decrypt";
            this.button_decrypt.Size = new System.Drawing.Size(75, 23);
            this.button_decrypt.TabIndex = 2;
            this.button_decrypt.Text = "Şifre Çöz";
            this.button_decrypt.UseVisualStyleBackColor = true;
            this.button_decrypt.Click += new System.EventHandler(this.button_decrypt_Click);
            // 
            // checkBox_deleteOrgFile
            // 
            this.checkBox_deleteOrgFile.AutoSize = true;
            this.checkBox_deleteOrgFile.Checked = true;
            this.checkBox_deleteOrgFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_deleteOrgFile.Location = new System.Drawing.Point(42, 118);
            this.checkBox_deleteOrgFile.Name = "checkBox_deleteOrgFile";
            this.checkBox_deleteOrgFile.Size = new System.Drawing.Size(121, 17);
            this.checkBox_deleteOrgFile.TabIndex = 3;
            this.checkBox_deleteOrgFile.Text = "Orijinal dosya silinsin";
            this.checkBox_deleteOrgFile.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label_FileName
            // 
            this.label_FileName.AutoSize = true;
            this.label_FileName.Location = new System.Drawing.Point(39, 28);
            this.label_FileName.Name = "label_FileName";
            this.label_FileName.Size = new System.Drawing.Size(0, 13);
            this.label_FileName.TabIndex = 4;
            // 
            // button_File_Select
            // 
            this.button_File_Select.Location = new System.Drawing.Point(228, 28);
            this.button_File_Select.Name = "button_File_Select";
            this.button_File_Select.Size = new System.Drawing.Size(75, 23);
            this.button_File_Select.TabIndex = 5;
            this.button_File_Select.Text = "Dosya Seç";
            this.button_File_Select.UseVisualStyleBackColor = true;
            this.button_File_Select.Click += new System.EventHandler(this.button_File_Select_Click);
            // 
            // comboBox_cipherMode
            // 
            this.comboBox_cipherMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_cipherMode.FormattingEnabled = true;
            this.comboBox_cipherMode.Location = new System.Drawing.Point(42, 89);
            this.comboBox_cipherMode.Name = "comboBox_cipherMode";
            this.comboBox_cipherMode.Size = new System.Drawing.Size(121, 21);
            this.comboBox_cipherMode.TabIndex = 6;
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 155);
            this.Controls.Add(this.comboBox_cipherMode);
            this.Controls.Add(this.button_File_Select);
            this.Controls.Add(this.label_FileName);
            this.Controls.Add(this.checkBox_deleteOrgFile);
            this.Controls.Add(this.button_decrypt);
            this.Controls.Add(this.button_encrypt);
            this.Controls.Add(this.comboBox_algorithm);
            this.Name = "MainScreen";
            this.Text = "MainScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_algorithm;
        private System.Windows.Forms.Button button_encrypt;
        private Button button_decrypt;
        private CheckBox checkBox_deleteOrgFile;
        private OpenFileDialog openFileDialog1;
        private Label label_FileName;
        private Button button_File_Select;
        private ComboBox comboBox_cipherMode;
    }
}