namespace Asynchronous_File_Download;

internal record struct DownloadInfo
{
    public string FileName {  get; init; }
    public long totalSize { get; init; }
    public long totalSizeRead { get; init; }
}
