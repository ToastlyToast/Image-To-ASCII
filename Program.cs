using System.Text;
using System.CommandLine;
using System.CommandLine.Invocation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageToASCII;

class Program
{
    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Turn an image into ASCII characters");
        var imageArgument = new Argument<string>("image", "Path to the image to process");
        var imageOutputOption = new Option<string>("--file", "Write resulting image to a file");
        rootCommand.AddArgument(imageArgument);
        rootCommand.AddOption(imageOutputOption);

        // Set up the handler for the root command
        rootCommand.SetHandler(async (string imagePath, string imageOutput) => {
            try
            {
                using Image image = Image.Load(imagePath);

                string asciiArt = GenerateASCII("@%#WMB8&$0ZOLXo=+~-;:,. ", image, 2);

                if (!string.IsNullOrEmpty(imageOutput))
                {
                    // Write the ASCII art to the specified file
                    await File.WriteAllTextAsync(imageOutput, asciiArt);
                    Console.WriteLine($"ASCII art written to {imageOutput}");
                }
                else
                {
                    // Print the ASCII art to the console
                    Console.WriteLine(asciiArt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
            }

        }, imageArgument, imageOutputOption);

        // Invoke the root command
        return rootCommand.InvokeAsync(args).Result;
    }

    public static string GenerateASCII(string asciiChars, Image image, int scaler)
    {
        if (image == null) return "";

        // Convert the image to grayscale and resize it
        image.Mutate(x => x.Grayscale().Resize(image.Width / scaler, image.Height / scaler));

        // Create a new StringBuilder to create the ASCII string
        StringBuilder asciiString = new StringBuilder();

        // Convert the image to an array of pixels
        using (var imageRGB = image.CloneAs<Rgba32>())
        {
            // Iterate across the pixel array
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Get the current pixel
                    var pixel = imageRGB[x, y];

                    // Find the grayscale value
                    var grayscaleValue = pixel.R;

                    // Normalize the grayscale value to the range of the ASCII characters
                    int index = (int)((grayscaleValue / 255.0) * (asciiChars.Length - 1));
                    char asciiChar = asciiChars[index];

                    // Append the ASCII character to the string
                    asciiString.Append(asciiChar);
                }
                asciiString.AppendLine();
            }
        }

        return asciiString.ToString();
    }
}
