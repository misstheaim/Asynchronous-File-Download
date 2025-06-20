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

        CancellationTokenSource cancel = new CancellationTokenSource();
        bool isDownloadFinished = false;
        Task cancelationTask = Task.Run(() => {
            Console.WriteLine("Press Esc to cancel downloading.");

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {

            }

            if (!isDownloadFinished)
            {
                Console.WriteLine("\nEcs key is pressed: canceling downloading!");
                cancel.Cancel();
            }
        });

        Task? commonTask = null;
        try
        {
            WebService service = new WebService();

            List<Task> tasks = new List<Task>();
            foreach(string url in trimUrls)
            {
                tasks.Add(service.DownloadFileAsync(url, cancel.Token));
            }
            commonTask = Task.WhenAll(tasks);
            await commonTask;
        }
        catch (HttpRequestException e)
        {
            if (commonTask?.Exception?.InnerExceptions != null && commonTask.Exception.InnerExceptions.Any())
            {
                foreach (Exception innerExc in commonTask.Exception.InnerExceptions)
                {
                    Console.WriteLine("\nDownload is failed, error message:\n{0}", innerExc.Message);
                }
            }
            else
            {
                Console.WriteLine("\nDownload is failed, error message:\n{0}", e.Message);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nDownloading canceled.");
        }
        catch (Exception e)
        {
            Console.WriteLine("\nException was thrown, error message:\n{0}", e.Message);
        }
        finally
        {
            isDownloadFinished = true;
        }

        Console.WriteLine("\n\n\nPress Esc to exit.");
        await cancelationTask;
    }
}
