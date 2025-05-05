namespace ComputerStore
{
    public static class Resources
    {
        public static void SaveBase64Image(string base64String, string fileName)
        {
            var base64Data = base64String.Substring(base64String.IndexOf(",") + 1);
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            System.IO.File.WriteAllBytes(imagePath, imageBytes);
        }
    }
}
