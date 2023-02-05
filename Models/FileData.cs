using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VerdonFileManager.Models
{
    public class FileData
    {
        [Key]
        public Guid FileDataId { get; set; }
        [ForeignKey("Folder")]
        public Guid FolderId { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public string UploadToken { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedDate { get; set; }

        public FileData()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
