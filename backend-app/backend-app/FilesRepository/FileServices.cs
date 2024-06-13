namespace backend_app.FilesRepository
{
    public class FileServices : IFileServices
    {
        private readonly IWebHostEnvironment _environment;

        public FileServices(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveImage(IFormFile imageFile)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var uploadsPath = Path.Combine(contentPath, "Uploads");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Check the allowed extensions
                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

                if (!allowedExtensions.Contains(ext))
                {
                    throw new ArgumentException("Only .jpg, .png, .jpeg extensions are allowed");
                }

                string uniqueFileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine("Uploads",uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }
    }
}