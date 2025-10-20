using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10448895_CMCS_PROG.Controllers;
using ST10448895_CMCS_PROG.Data;
using ST10448895_CMCS_PROG.Models;
using Xunit;

namespace ST10448895_CMCS_PROG.Tests
{
    public class LoginControllerTests
    {
        private ApplicationDbContext GetInMemoryContext(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName ?? System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Index_Get_ClearsSessionAndReturnsView()
        {
            using var context = GetInMemoryContext();
            var controller = new LoginController(context);

            // Need HttpContext to be able to call .Index()
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_Post_ValidLecturer_RedirectsToLecturerIndex()
        {
            using var context = GetInMemoryContext();
            // seed a lecturer with Name matching the Login model
            context.Lecturers.Add(new LecturerModel { Id = 3, Name = "Alice" });
            context.SaveChanges();

            var controller = new LoginController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var login = new Login { Name = "Alice", Role = "Lecturer" };
            var result = controller.Index(login) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Lecturer", result.ControllerName);
        }
    }
}
