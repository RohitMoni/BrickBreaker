using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    static class FileServices
    {
        public static void SaveFile(string targetFile, string data)
        {
            if (targetFile == null)
                return;

            ClearFile(targetFile);
            var fileName = Application.persistentDataPath + "/" + targetFile;
            var file = new StreamWriter(fileName, false);
            file.Write(data);

            file.Close();
        }

        public static void ClearFile(string targetFile)
        {
            if (targetFile == null)
                return;

            var fileName = Application.persistentDataPath + "/" + targetFile;
            var directoryName = fileName.Substring(0, fileName.LastIndexOf('/'));
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(fileName, string.Empty);
        }

        public static string LoadFile(string targetFile)
        {
            if (targetFile == null)
                return null;

            var fileName = Application.persistentDataPath + "/" + targetFile;
            if (!File.Exists(fileName))
                return null;

            string fileData;
            using (var reader = new StreamReader(fileName, Encoding.Default))
            {
                fileData = reader.ReadToEnd();
            }

            return fileData;
        }
    }
}
