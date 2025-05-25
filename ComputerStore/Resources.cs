namespace ComputerStore
{
    public static class Resources
    {
        public static void SaveBase64Image(string base64String, string fileName)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            System.IO.File.WriteAllBytes(imagePath, imageBytes);
        }
    }
}
