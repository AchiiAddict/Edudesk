namespace LearningService.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; } // KurumID
        public string Title { get; set; } // Dersin Adı
        public string Description { get; set; } // İçerik
        public string VideoUrl { get; set; } // Video Linki
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}