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

namespace ResCryption
{
    public partial class MainScreen : Form
    {

        private string selectedFileName { get; set; }
        public MainScreen()
        {
            InitializeComponent();
            comboBox_algorithm.Items.AddRange(EncryptionManager.GetEncryptionKeys());
            comboBox_algorithm.SelectedIndex = 0;

            comboBox_cipherMode.Items.AddRange(EncryptionManager.GetCipherModeKeys());
            comboBox_cipherMode.SelectedIndex = 0;
            selectedFileName = "";
        }

        private void button_encrypt_Click(object sender, EventArgs e)
        {
            if (selectedFileName != "" )
            {
                Password password = new Password(selectedFileName, comboBox_algorithm.Text, comboBox_cipherMode.Text, true, checkBox_deleteOrgFile.Checked);
                password.Show();
            }
            else
                ErorManager.Show("ERR_NO_FILE_SELECTED");
        }

        private void button_decrypt_Click(object sender, EventArgs e)
        {
         
            if (selectedFileName != "")
            {
                if (FileManager.GetFileExtension(selectedFileName) == FileManager.ResEncryptionExtension)
                {
                    Password password = new Password(selectedFileName, comboBox_algorithm.Text, comboBox_cipherMode.Text, false, checkBox_deleteOrgFile.Checked);
                    password.Show();
                }
                else
                    ErorManager.Show("ERR_BAD_EXTENTION");
            }
            else
                ErorManager.Show("ERR_NO_FILE_SELECTED");
            
        }

        private void button_File_Select_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            selectedFileName = fileDialog.FileName;
            label_FileName.Text = FileManager.GetFileNameEdit(selectedFileName);
        }
    }
}
