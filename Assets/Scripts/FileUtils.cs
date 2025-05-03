using System;
using System.IO;
using UnityEngine;

public class FileUtils : IFileUtils
{
    public void WriteToFile(Stream stream, string fullPath, int bufferSize = 8 * 1024)
    {
        try
        {
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                byte[] buffer = new byte[bufferSize];

                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }
        }catch (Exception ex)
        {
            Debug.Log($"Exception Occured: {ex.Message}");
        }
       
    }
    public void RemoveFile(string fullPath)
    {
        File.Delete(fullPath);
    }
}