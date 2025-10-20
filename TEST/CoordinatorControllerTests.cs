using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10448895_CMCS_PROG.Controllers;
using ST10448895_CMCS_PROG.Data;
using ST10448895_CMCS_PROG.Models;
using Xunit;

namespace ST10448895_CMCS_PROG.Tests
{
    public class CoordinatorControllerTests
    {
        private ApplicationDbContext GetInMemoryContext(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithDashboardModel()
        {
            using var context = GetInMemoryContext();

            // seed a claim for dashboard calculations
            context.Claims.Add(new ClaimModel
            {
                Id = 1,
                HoursWorked = 5,
                HourlyRate = 100,
                Verified = false,
                Approved = false,
                SubmitDate = DateTime.UtcNow
            });
            context.SaveChanges();

            var controller = new CoordinatorController(context);
            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void VerifyClaim_VerifyAction_SetsClaimVerified()
        {
            using var context = GetInMemoryContext();
            var claim = new ClaimModel
            {
                Id = 10,
                Verified = false,
                Status = "Pending",
                HoursWorked = 3,
                HourlyRate = 120
            };
            context.Claims.Add(claim);
            context.SaveChanges();

            var controller = new CoordinatorController(context);

            // Act
            var result = controller.VerifyClaim(10, "verify", null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Verify", result.ActionName);

            var updated = context.Claims.Find(10);
            Assert.True(updated.Verified);
            Assert.Equal("Verified", updated.Status);
        }

        [Fact]
        public void VerifyClaim_InvalidId_RedirectsToVerifyWithError()
        {
            using var context = GetInMemoryContext();
            var controller = new CoordinatorController(context);

            var result = controller.VerifyClaim(9999, "verify", null) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Verify", result.ActionName);
        }
    }
}
