using System.Text;

namespace Asynchronous_File_Download;

internal static class Progression
{
    public static Dictionary<string, DownloadInfo> OpenDownloads { get; set; } = new Dictionary<string, DownloadInfo>();

    private static Lock _lock = new Lock();
    public static void AddDownload(DownloadInfo info)
    {
        lock (_lock)
        {
            OpenDownloads.Add(info.FileName, info);
            for (int i = 1; i <= 5; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }
    }

    public static void RemoveDownload(string fileName)
    {
        lock (_lock)
        {
            OpenDownloads.Remove(fileName);
            Console.WriteLine("File \"{0}\" successfully downloaded!", fileName);
        }
    }
    public static void UpdateConsoleProgress(DownloadInfo downloadInfo)
    {
        lock (_lock)
        {
            OpenDownloads[downloadInfo.FileName] = downloadInfo;

            StringBuilder stringInfo = new StringBuilder();

            foreach (var download in OpenDownloads)
            {
                stringInfo.AppendLine($"File: {download.Value.FileName}; Bytes read: {download.Value.TotalSizeRead} / {download.Value.TotalSize}");
            }

            Console.SetCursorPosition(0, 1);
            Console.Write(stringInfo);
        }
    }
}
