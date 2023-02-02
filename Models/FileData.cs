using System.ComponentModel.DataAnnotations;

namespace VerdonFileManager.Models
{
    public class FileData
    {
        [Key]
        public Guid FileDataId { get; set; }
        public Guid FolderId { get; set; }
        public string UploadToken { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedDate { get; set; }

        public FileData()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
