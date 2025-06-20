using System.Text.RegularExpressions;

namespace Asynchronous_File_Download;

internal class WebService
{
    private HttpClient _httpClient;

    private readonly string saveDirectory = "Files";

    private const int bufferSizeOf8Kb = 8192;

    private const int countOfPackagesToReport = 100;

    public WebService()
    {
        SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        };

        _httpClient = new HttpClient(socketsHttpHandler);
        Directory.CreateDirectory(saveDirectory);
    }

    public async Task DownloadFileAsync(string url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        try
        {
            response.EnsureSuccessStatusCode();
        } catch (HttpRequestException e)
        {
            throw new HttpRequestException($"Bad request with URL - {url}; Error message:\n{e.Message}");
        }

        string fileName = GetFileName(url);
        long totalBytes = response.Content.Headers.ContentLength ?? -1L;
        long totalBytesRead = 0;

        using Stream remoteFileStream = await response.Content.ReadAsStreamAsync();
        using FileStream localFileStream = new FileStream(fileName, FileMode.OpenOrCreate);

        byte[] buffer = new byte[bufferSizeOf8Kb];
        int bytesRead;

        Progression.AddDownload(new DownloadInfo() { FileName = fileName, TotalSize = totalBytes, TotalSizeRead = totalBytesRead });

        int i = 0;
        while((bytesRead = await remoteFileStream.ReadAsync(buffer, 0, bufferSizeOf8Kb, cancellationToken)) > 0)
        {
            i++;
            await localFileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            totalBytesRead += bytesRead;
            if ((i % countOfPackagesToReport) == 0)
            {
                Progression.UpdateConsoleProgress(new DownloadInfo() { FileName = fileName, TotalSize = totalBytes, TotalSizeRead = totalBytesRead });
            }
        }
        Progression.UpdateConsoleProgress(new DownloadInfo() { FileName = fileName, TotalSize = totalBytesRead, TotalSizeRead = totalBytesRead });
        //Progression.RemoveDownload(fileName);
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
}
