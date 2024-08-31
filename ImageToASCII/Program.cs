using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageToASCII
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Image image = Image.Load("images/Screenshot_25-8-2024_111117_www.instagram.com.jpeg");

            Console.SetWindowSize(160 * 2, 80 * 2);
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(GenerateASCII("#:.", image, 2));
            Console.ReadLine();
        }

        public static string GenerateASCII(string ASCII, Image image, int scaler)
        {
            if(image == null) return "";

            // Convert the image to grayscale and resize it.
            image.Mutate(x => x.Grayscale().Resize(160 * scaler, 80 * scaler));

            // Create a new stringbuilder to create the string / image in ASCII.
            StringBuilder asciiString = new StringBuilder();

            // Convert our image to an array of pixels.
            using (var imageRGB = image.CloneAs<Rgba32>())
            {
                // Iterate across the new pixel array (imageRGB).
                for(int y = 0; y < image.Height; y++)
                {
                    for(int x = 0; x < image.Width; x++)
                    {
                        // Create a new pixel variable to store the value of the current pixel.
                        var pixel = imageRGB[x, y];

                        // Find the grayscale value.
                        var grayscaleValue = pixel.R;

                        // Normalize the grayscale and set it to a value within the ASCII string.
                        int index = (int)((grayscaleValue / 255.0) * (ASCII.Length - 1));
                        char asciiChar = ASCII[index];

                        asciiString.Append(asciiChar);
                    }
                    asciiString.AppendLine();
                }
            }
            return asciiString.ToString();
        }
    }
}