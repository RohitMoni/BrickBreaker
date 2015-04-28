using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    static class FileServices
    {
        private static string debug = "";

        private static void SaveFile(string targetFile, string data)
        {
            if (targetFile == null)
                return;

            ClearFile(targetFile);
            var fileName = Application.persistentDataPath + "/" + targetFile;
            var file = new StreamWriter(fileName, false);
            file.Write(data);

            file.Close();
        }

        private static void ClearFile(string targetFile)
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

        private static string LoadFile(string targetFile)
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

        public static void LoadGame()
        {
            var data = LoadFile(GameVariablesScript.GameFile);

            if (data == null)
                GameVariablesScript.HighScore = 0;

            foreach (var line in data.Split('|'))
            {
                var location = line.IndexOf(':');

                if (location == 0)
                    continue;

                var identifier = line.Substring(0, location);
                var relevantData = line.Substring(location + 1);

                switch (identifier)
                {
                    case "High Score":
                        GameVariablesScript.HighScore = Int32.Parse(relevantData);
                        break;
                    case "Bottom Slider":
                        GameVariablesScript.SliderMovement = bool.Parse(relevantData);
                        break;
                    case "Relative Paddle":
                        GameVariablesScript.RelativePaddle = bool.Parse(relevantData);
                        break;
                    case "Paddle Sensitivity":
                        GameVariablesScript.PaddleSensitivity = float.Parse(relevantData);
                        break;
                    case "Ball Speed":
                        GameVariablesScript.BallSpeed = float.Parse(relevantData);
                        break;
                }
            }
        }

        public static void SaveGame()
        {
            var data = "High Score: " + GameVariablesScript.HighScore + " |" +
                       "Bottom Slider: " + GameVariablesScript.SliderMovement + " |" +
                       "Relative Paddle: " + GameVariablesScript.RelativePaddle + " |" +
                       "Paddle Sensitivity: " + GameVariablesScript.PaddleSensitivity + " |" +
                       "Ball Speed: " + GameVariablesScript.BallSpeed;

            SaveFile(GameVariablesScript.GameFile, data);
        }

        public static void Log(string log)
        {
            debug += log + "\n";
            SaveLog();
        }

        public static void SaveLog()
        {
            SaveFile("debug.log", debug);
        }
    }
}
