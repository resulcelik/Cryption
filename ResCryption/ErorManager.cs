using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResCryption
{
    class ErorManager
    {
        private static Dictionary<string, string> ErrorDictionary;
        public static void Init()
        {
            ErrorDictionary = new Dictionary<string, string>();
            ErrorDictionary["ERR_BAD_EXTENTION"] = "Hatalı Uzantı!!";
            ErrorDictionary["ERR_WRONG_PASS"] = "Şifre Yanlış!!!";
            ErrorDictionary["ERR_ENC"] = "Şifreleme Başarısız Oldu!!";
            ErrorDictionary["ERR_FILE_WRITE"] = "Dosya Yazma Başarısız Oldu!!";
            ErrorDictionary["ERR_PASS_INCOMPATIBLE"] = "Şifre Doğrulama Sağlanamadı!!";
            ErrorDictionary["ERR_NO_FILE_SELECTED"] = "Dosya Seçili Değil!!";
            ErrorDictionary["ERR_NOT_SUPPORTED"] = "Bu Mod Desteklenmiyor!!";
        }
        
        public static void Show(String ErorKey)
        {
            if (ErrorDictionary[ErorKey] != null)
                MessageBox.Show(ErrorDictionary[ErorKey]);
            else
                MessageBox.Show(ErorKey);
        }
    }
}
