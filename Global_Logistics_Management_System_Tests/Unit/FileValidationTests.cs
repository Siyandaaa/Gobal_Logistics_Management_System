using Global_Logistics_Management_System.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;


namespace Global_Logistics_Management_System_Tests.Unit
{
    public class FileValidationTests
    {
        private readonly FileService _fileService;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<FileService>> _mockLogger;

        public FileValidationTests()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<FileService>>();

            var configData = new Dictionary<string,string>
            {
                ["FileUpload:AllowedExtensions:0"] = ".pdf",
                ["FileUpload:MaxFileSizeMB"] = "5"
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            _fileService = new FileService(_mockEnv.Object, config, _mockLogger.Object);
        }

        [Fact]
        public async Task SaveSignedAgreement_NonPdfExtension_ThrowsInvalidOperationException()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("virus.exe");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("application/octet-stream");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _fileService.SaveSignedAgreementAsync(fileMock.Object));
        }

        [Fact]
        public async Task SaveSignedAgreement_FileTooLarge_ThrowsException()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("big.pdf");
            fileMock.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6 MB
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _fileService.SaveSignedAgreementAsync(fileMock.Object));
        }

        [Fact]
        public async Task SaveSignedAgreement_ValidPdf_SavesFile()
        {
            // Setup temp environment
            var tempPath = Path.Combine(Path.GetTempPath(), "GLMS_Test_" + Guid.NewGuid());
            Directory.CreateDirectory(tempPath);
            _mockEnv.Setup(e => e.WebRootPath).Returns(tempPath);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("contract.pdf");
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Callback<Stream, CancellationToken>((s, _) => s.WriteByte(0x25))
                    .Returns(Task.CompletedTask);

            var result = await _fileService.SaveSignedAgreementAsync(fileMock.Object);

            Assert.StartsWith("/uploads/", result);
            Assert.EndsWith(".pdf", result);
            var savedFile = Path.Combine(tempPath, result.TrimStart('/'));
            Assert.True(File.Exists(savedFile));

            // Cleanup
            Directory.Delete(tempPath, true);
        }
    }
}
