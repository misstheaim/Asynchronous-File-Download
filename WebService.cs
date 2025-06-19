using System.Text.RegularExpressions;

namespace Asynchronous_File_Download;

internal class WebService
{
    private HttpClient _httpClient;

    private CancellationTokenSource _tokenSource;

    private readonly string saveDirectory = "Files";

    public WebService()
    {
        _httpClient = new HttpClient();
        _tokenSource = new CancellationTokenSource();
        Directory.CreateDirectory(saveDirectory);
        Task.Run( CancelTask );
    }

    public async Task DownloadFileAsync(string url)
    {
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        string fileName = GetFileName(url);
        long totalBytes = response.Content.Headers.ContentLength ?? -1L;
        long totalBytesRead = 0;
        int chunkSize = 8192;

        using Stream remoteFileStream = await response.Content.ReadAsStreamAsync();
        using FileStream localFileStream = new FileStream(fileName, FileMode.OpenOrCreate);

        byte[] buffer = new byte[chunkSize];
        int bytesRead;

        while((bytesRead = await remoteFileStream.ReadAsync(buffer, 0, chunkSize, _tokenSource.Token)) > 0)
        {
            await localFileStream.WriteAsync(buffer, 0, bytesRead, _tokenSource.Token);
            totalBytesRead += bytesRead;
            Console.Write("\rBytes read - {0} from - {1}", totalBytesRead, totalBytes);
        }
    }

    private string GetFileName(string url)
    {
        string fileName = saveDirectory + "/" + Regex.Match(url, "[^/]+(?=/?$)").Value;
        int i = 0;
        Match matchGroups = Regex.Match(fileName, @"^([^\.]+)(\..+)$");
        string originalName = matchGroups.Groups[1].Value;
        while (File.Exists(fileName))
        {
            fileName = originalName;
            fileName += "(" + i + ")" + matchGroups.Groups[2].Value;
            i++;
        }
        return fileName;
    }

    private void CancelTask()
    {
        Console.WriteLine("Press Esc to cancel downloading.");

        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
            
        }

        Console.WriteLine("\n EEcs key is pressed: canceling downloading!");

        _tokenSource.Cancel();
    }
}
