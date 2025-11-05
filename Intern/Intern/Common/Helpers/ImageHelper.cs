namespace Intern.Common.Helpers
{
    public class ImageHelper
    {

        public async Task<string> SaveBase64FileAsync2(string base64File, string saveFolder, string extension)
        {
            if (string.IsNullOrEmpty(base64File))
                return null;

            // Remove "data:application/pdf;base64," or "data:image/png;base64," part if exists
            var base64Data = base64File.Contains(",")
                ? base64File.Split(',')[1]
                : base64File;

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(base64Data);
            }
            catch
            {
                return null; // Invalid base64 string
            }

            // Create folder if missing
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            // Generate unique file name with correct extension
            var fileName = $"{Guid.NewGuid()}.{extension}";
            var filePath = Path.Combine(saveFolder, fileName);

            try
            {
                await File.WriteAllBytesAsync(filePath, fileBytes);
            }
            catch
            {
                return null; // File write failed
            }

            return filePath;
        }


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

