using LearningService.Data;
using LearningService.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly LearningDbContext _context;

        public CourseController(LearningDbContext context)
        {
            _context = context;
        }

        // POST: api/course , İçerik eklemek için.
        [HttpPost]
        public IActionResult CreateCourse([FromBody] Course newCourse)
        {
            newCourse.Id = Guid.NewGuid();
            newCourse.CreatedAt = DateTime.UtcNow;

            _context.Courses.Add(newCourse);
            _context.SaveChanges();

            return Ok(new { Mesaj = "Eğitim içeriği başarıyla eklendi.", Data = newCourse });
        }

        // GET: api/course/{tenantId} , Kuruma özel eğitimlerin listelenmesi
        [HttpGet("{tenantId}")]
        public IActionResult GetCoursesByTenant(Guid tenantId)
        {
            // tenanta ait dersleri getirme.
            var courses = _context.Courses
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return Ok(courses);
        }
    }
}