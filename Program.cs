namespace Asynchronous_File_Download;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Please enter url sources divided by ','.");
        List<string> urlList = Console.ReadLine().Split(new char[] { ',' }).ToList();

        List<string> trimUrls = new List<string>();
        foreach (string url in urlList)
        {
            trimUrls.Add(url.Trim());
        }

        try
        {
            WebService service = new WebService();

            List<Task> tasks = new List<Task>();
            foreach(string url in trimUrls)
            {
                tasks.Add(service.DownloadFileAsync(url));
            }
            await Task.WhenAll(tasks);
            
        }
        catch (HttpProtocolException e)
        {
            Console.WriteLine("\nDownload is failed, error message:\n{0}", e.Message);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nDownloading canceled.");
        }
        catch (Exception e)
        {
            Console.WriteLine("\nUnhandled exception, error message:\n{0}", e.Message);
        }
        
    }
}
