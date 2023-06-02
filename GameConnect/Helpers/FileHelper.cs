namespace GameConnect.Helpers;

public class FileHelper
{
    public static void RemoveImage(string fileName)
    {
        File.Delete($"./wwwroot/images/{fileName}");
    }

    public static async Task AddImage(IFormFile uploadedImage, string fileName)
    {
        var file = $"./wwwroot/images/{fileName}";
        using var fileStream = new FileStream(file, FileMode.Create);
        await uploadedImage.CopyToAsync(fileStream);
    }
}
