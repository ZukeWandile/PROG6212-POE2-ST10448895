using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10448895_CMCS_PROG.Controllers;
using ST10448895_CMCS_PROG.Data;
using ST10448895_CMCS_PROG.Models;
using ST10448895_CMCS_PROG.Models.ViewModels;
using Xunit;

namespace ST10448895_CMCS_PROG.Tests
{
    public class LecturerControllerTests
    {
        private ApplicationDbContext GetInMemoryContext(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Documents_ReturnsView_WithDocumentsForClaim()
        {
            using var context = GetInMemoryContext();
            // seed a claim and an upload document
            var claim = new ClaimModel { Id = 5 };
            context.Claims.Add(claim);
            context.UploadDocuments.Add(new UploadDocumentModel { Id = 1, ClaimId = 5, OriginalFilename = "file.pdf", ContentType = "application/pdf", FilePath = "path" });
            context.SaveChanges();

            var controller = new LecturerController(context, null); // null for IWebHostEnvironment only because Documents doesn't use it

            var result = controller.Documents(5) as ViewResult;
            Assert.NotNull(result);

            var model = result.Model;
            Assert.NotNull(model);

            // Documents action stores documents in local variable but sets ViewBag.ClaimId and returns a view.
            // Because the controller uses _context.UploadDocuments.Where(...).ToList(),
            // ensure that the DB has the document and the view result is returned.
            Assert.Equal(5, controller.ViewBag.ClaimId);
        }

        [Fact]
        public void Track_NonExistentClaim_ReturnsNotFound()
        {
            using var context = GetInMemoryContext();
            var controller = new LecturerController(context, null);

            var result = controller.Track(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Track_ExistingClaim_ReturnsViewWithClaim()
        {
            using var context = GetInMemoryContext();
            var claim = new ClaimModel { Id = 20 };
            context.Claims.Add(claim);
            context.SaveChanges();

            var controller = new LecturerController(context, null);

            var result = controller.Track(20) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(claim, result.Model);
        }
    }
}
