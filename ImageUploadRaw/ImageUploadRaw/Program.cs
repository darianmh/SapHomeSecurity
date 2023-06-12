// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

var lastFile = "";
while (true)
{
    try
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var files = Directory.GetFiles(baseDir);
        if (files != null) files = files.Where(x => x.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x).ToArray();
        if (files.Length == 1) continue;
        var imageFile = files.Skip(1).FirstOrDefault();
        if (imageFile == null) continue;
        if (imageFile == lastFile) continue;
        lastFile = imageFile;
        var file = File.ReadAllBytes(imageFile);
        Console.WriteLine($"Sending: {lastFile}");
        using var client = new HttpClient();
        using var content =
            new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        content.Add(new StreamContent(new MemoryStream(file)), "file", "upload");

        using var message =
            await client.PostAsync("http://109.122.199.199:5026/video", content);
        if (message.IsSuccessStatusCode) Console.WriteLine($"Sent:{lastFile}");
        else Console.WriteLine($"Not Sent:{lastFile}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    Console.WriteLine("Sleep thread");
}
