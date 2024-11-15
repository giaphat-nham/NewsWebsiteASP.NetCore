using System.Text;

namespace NewsWebsite.Helpers
{
    public class Ultilities
    {
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qweasdzxcRTYFGHVBNuiojklMrtyfghvbnWQEASDZXC";
            var sb = new StringBuilder();
            var rd = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }

        public static string UploadImage(IFormFile image, string folder)
        {
            try { 
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folder, image.FileName);
            using (var myFile = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(myFile);
            }
            return image.FileName;
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
    }
}
