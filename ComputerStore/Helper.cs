using System.Security.Cryptography;
using System.Text;

namespace ComputerStore
{
    public static class Helper
    {
        private const string IMAGE_FOLDER = "images";
        public static string ComputeMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-","").ToLower();
            }
        }

        public static async Task<string> SaveImageHash(string userId, IFormFile file)
        {
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                throw new("Định dạng file không phù hợp");
            }
            string fileName = file.FileName;
            string fileExt = Path.GetExtension(fileName);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string rawString = $"{userId}{fileName}{timestamp}";
            string hashFileName = ComputeMd5(rawString);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IMAGE_FOLDER, $"{hashFileName}{fileExt}");
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                 await file.CopyToAsync(stream);
            }
            return $"{IMAGE_FOLDER}/{hashFileName}{fileExt}";
        }

        public static void DeleteFile(string filePath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            File.Delete(path);
        }
    }
}
