namespace GoogleDriveUploader.Extensions
{
    public static class FileExtensions
    {
        public static double ToMegabytes(this long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}
