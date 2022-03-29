using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResCryption
{
    public partial class Password : Form
    {
        private string fileName;
        private string algorithm;
        private string cipherMode;
        private bool deleteOrginalFile;
        private bool isEncrypt;
        private Stopwatch stopwatch = new Stopwatch();
        public Password(string fileName, string algorithm, string cipherMode, bool isEncrypt = true, bool deleteOrginalFile = false)
        {
            InitializeComponent();
            this.fileName = fileName;
            this.algorithm = algorithm;
            this.cipherMode = cipherMode;
            this.deleteOrginalFile = deleteOrginalFile;
            this.isEncrypt = isEncrypt;
            if (isEncrypt == false)
            {
                textBox_password2.Visible = false;
                label2.Visible = false;
            }
        }

        public Password(string fileName)
        {
            InitializeComponent();
            textBox_password2.Visible = false;
            label2.Visible = false;
            this.fileName = fileName;
            this.deleteOrginalFile = true;
            this.isEncrypt = false;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (textBox_password.Text == textBox_password2.Text || textBox_password2.Visible == false)
            {
                try
                {
                    
                    stopwatch.Start();
                    

                    List<byte[]> resData = new List<byte[]>();
                    byte[] fileData = FileManager.ReadAllBytes(fileName);
                    if (isEncrypt)
                    {
                        resData.Add(Encoding.UTF8.GetBytes(FileManager.GetFileExtension(fileName)));
                        resData.Add(fileData);
                        byte[] encryptedData = EncryptionManager.GetEncrypt(algorithm)(Helper.ConvertData(resData).ToArray(), textBox_password.Text, cipherMode);
                        resData.Clear();
                        resData.Add(Encoding.UTF8.GetBytes(algorithm));
                        resData.Add(Encoding.UTF8.GetBytes(cipherMode));
                        resData.Add(encryptedData);
                        string encFileName = FileManager.GetFileName(fileName) + "." + FileManager.ResEncryptionExtension;
                        FileManager.WriteToFile(encFileName, Helper.ConvertData(resData).ToArray());

                       /* Console.Write("Data:");
                        foreach (int i in encryptedData)
                        {
                            Console.WriteLine("{0:X4}", i);
                        }*/

                        if (deleteOrginalFile)
                            FileManager.deleteFile(fileName);
                    }
                    else
                    {
                        try
                        {
                            List<byte[]> parsedData = Helper.ParseData(fileData);
                            algorithm = Encoding.UTF8.GetString(parsedData.First());
                            cipherMode = Encoding.UTF8.GetString(parsedData[1]);
                            byte[] decryptedData = EncryptionManager.GetDecrypt(algorithm)(parsedData[2], textBox_password.Text, cipherMode);
                            parsedData = Helper.ParseData(decryptedData);
                            string fileExtension = Encoding.UTF8.GetString(parsedData.First());
                            string dcFileName = FileManager.GetFileName(fileName) + "." + fileExtension;
                            FileManager.WriteToFile(dcFileName, parsedData[1]);

                            if (deleteOrginalFile)
                                FileManager.deleteFile(fileName);
                        }
                        catch 
                        {
                            ErorManager.Show("ERR_WRONG_PASS");
                        }
                    }
                    stopwatch.Stop();
                    MessageBox.Show("Gecen Süre: "+stopwatch.ElapsedMilliseconds.ToString()+"ms");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    ErorManager.Show(ex.Message);
                }
            }
            else
            {
                ErorManager.Show("ERR_PASS_INCOMPATIBLE");
            }
            this.Close();
        }
    }
}
