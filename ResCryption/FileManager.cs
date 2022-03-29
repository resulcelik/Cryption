using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResCryption
{
    public class FileManager
    {
        public static string ResEncryptionExtension = "res";
        public static string fileNameEdit;

        public static byte[] ReadAllBytes(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            return File.ReadAllBytes(fileName);
        }

        public static void WriteToFile(string toFileName, byte[] data)
        {
            try
            {
                using (var fs = new FileStream(toFileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                throw new Exception("ERR_FILE_WRITE");                
            }
        }

        public static string GetFileName(string fileName)
        {
            int index = fileName.LastIndexOf('.');
            string file = fileName.Substring(0, index);
            return file;
        }

        public static string GetFileExtension(string fileName)
        {
            return fileName.Split('.').Last();
        }

        public static void deleteFile(string fileName)
        {
            File.Delete(fileName);
        }
        public static string GetFileNameEdit(string fileName)
        {
            return  fileNameEdit = Path.GetFileName(fileName);      
        }
        
    }
}
