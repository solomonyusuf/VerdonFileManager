using System.ComponentModel.DataAnnotations;

namespace VerdonFileManager.Models
{
    public class Folder
    {
        [Key]
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string UploadToken { get; set; }
        public DateTime CreatedDate { get; set; }

        public Folder()
        {
            CreatedDate = DateTime.Now;
            var random = new Random(3000000);
            UploadToken = $"{random.Next()}GA";  
        }
    }
}
