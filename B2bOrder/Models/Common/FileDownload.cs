namespace B2bOrder.Models.Common
{
    public class FileDownloadModel
    {
        public byte[] FileContents { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}