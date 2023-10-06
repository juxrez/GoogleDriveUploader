using ConsoleTables;
using GoogleDriveUploader.Extensions;
using GoogleDriveUploader.Services;

namespace GoogleDriveUploader
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var filesToUpload = FindFilesInCurrentDirectory();
            if (filesToUpload is not null && filesToUpload.Any())
            {
                await HandleFiles(filesToUpload);
            } 
        }

        private static FileInfo[]? FindFilesInCurrentDirectory()
        {
            try
            {
                string path = AppContext.BaseDirectory;
                Console.WriteLine($"Founf this files in: {path} \r\n");

                var parentPath = Directory.GetParent(path).Parent;
                var files = parentPath.GetFiles("*.pdf");

                return files;
            }
            catch (Exception ex)
            {
                Console.Write($"\r\n {ex.Message}");
                return null;
            }
        }

        private static async Task HandleFiles(FileInfo[] filesToUpload)
        {
                var filesTable = new ConsoleTable("File Name", "Size");
                foreach (var file in filesToUpload)
                {
                    filesTable.AddRow(
                        file.Name,
                        string.Format("{0:00.##} MB", file.Length.ToMegabytes()));
                }

                filesTable.Write(Format.MarkDown);

                Console.WriteLine("Do you want to upload them? Y/N");
                var key = Console.ReadKey();

                if (((char)key.Key) is 'Y' or 'y')
                {
                    var service = new GoogleDriveService();
                    //var s = filesToUpload.Select(async file =>
                    //{
                    //    await service.UploadFile(file.DirectoryName!, file.Name!);
                    //});
                    foreach (var file in filesToUpload)
                    {
                        await service.UploadFile(file.DirectoryName!, file.Name!);
                    }
                }
            
        }
    }
}