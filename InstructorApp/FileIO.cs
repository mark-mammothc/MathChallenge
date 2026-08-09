using System;
using System.IO;

namespace MathQuestionChallenge
{
    internal class FileIO
    {
        /// <summary>
        /// Writes any string content directly to a file.
        /// </summary>
        public static bool SaveToFile(string filePath, string content)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(content);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileIO Error] Failed to save file to {filePath}: {ex.Message}");
                return false;
            }
        }
    }
}