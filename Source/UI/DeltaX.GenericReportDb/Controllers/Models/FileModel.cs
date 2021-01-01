namespace DeltaX.GenericReportDb.Controllers.Models
{
    public class FileModel
    {
        public byte[] FileContents { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
