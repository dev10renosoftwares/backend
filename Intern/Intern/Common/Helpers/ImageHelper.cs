namespace Intern.Common.Helpers
{
    public class ImageHelper
    {

        // Save base64 string as image file
      
        public async Task<string> SaveBase64ImageAsync(string base64Image, string saveFolder)
        {
            if (string.IsNullOrEmpty(base64Image))
                return null;

            var base64Data = base64Image.Contains(",")
                ? base64Image.Split(',')[1]
                : base64Image;

            byte[] imageBytes;

            try
            {
                imageBytes = Convert.FromBase64String(base64Data);
            }
            catch
            {
               
                return null;
            }

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(saveFolder, fileName);

            try
            {
                await File.WriteAllBytesAsync(filePath, imageBytes);
            }
            catch
            {
                return null;
            }

            return filePath;
        }


        // ✅ Save image from URL (Google profile image, etc.)
        public async Task<string> SaveImageFromUrlAsync(string imageUrl, string saveFolder)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(imageUrl);

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(saveFolder, fileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            return filePath;
        }

        // Convert local file path (from DB) to base64
        public string ConvertFileToBase64(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            var bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }

        // Convert Google Image URL (or any external URL) to base64
        public async Task<string> ConvertImageUrlToBase64Async(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(imageUrl);
            return Convert.ToBase64String(bytes);
        }
    }
}

