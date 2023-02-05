using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VerdonFileManager.Models
{
    public class Folder
    {
        [Key]
        public Guid FolderId { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string UploadToken { get; set; }
        public List<FileData> Files { get; set; }
        public DateTime CreatedDate { get; set; }

        public Folder()
        {
            CreatedDate = DateTime.Now;
            var random = new Random();
            UploadToken = $"X{random.Next(3000000)}GA";
            Files = new List<FileData>();
        }
    }
}
