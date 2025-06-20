namespace Asynchronous_File_Download;

internal record struct DownloadInfo
{
    public string FileName {  get; init; }
    public long TotalSize { get; init; }
    public long TotalSizeRead { get; init; }
}
