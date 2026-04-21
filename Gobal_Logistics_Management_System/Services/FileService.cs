namespace Global_Logistics_Management_System.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment env, IConfiguration config, ILogger<FileService> logger)
        {
            _env = env;
            _config = config;
            _logger = logger;
        }

        public async Task<string> SaveSignedAgreementAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            // Extension validation
            var allowedExtensions = _config.GetSection("FileUpload:AllowedExtensions").Get<string[]>();
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException($"Only {string.Join(", ", allowedExtensions)} files are allowed.");

            // Size validation
            var maxSizeBytes = _config.GetValue<int>("FileUpload:MaxFileSizeMB") * 1024 * 1024;
            if (file.Length > maxSizeBytes)
                throw new InvalidOperationException($"File size exceeds {_config.GetValue<int>("FileUpload:MaxFileSizeMB")} MB.");

            // Content type check (additional security)
            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid file content type. Only PDF accepted.");

            // Generate unique filename
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("File saved: {FileName} for contract", fileName);
            return $"/uploads/{fileName}"; // relative path for DB
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
