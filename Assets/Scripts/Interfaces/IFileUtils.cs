using System.IO;
public interface IFileUtils
{
    abstract void WriteToFile(Stream stream, string fullPath, int bufferSize);

    abstract void RemoveFile(string fullPath);
}