using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RezaTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        private readonly string Name = "1.Json";

        [HttpGet]
        public string Get()
        {
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            var text = System.IO.File.ReadAllText(System.IO.Path.Combine(Path, Name));
            //var model = JsonConvert.DeserializeObject(text);
            return text;
        }

        [HttpPost]
        public IActionResult Post([FromBody]TestC content)
        {
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            //var model = JsonConvert.SerializeObject(content);
            System.IO.File.WriteAllText(System.IO.Path.Combine(Path, Name), content.Content);
            return Ok();
        }
    }
}

public class TestC
{
    public string Content { get; set; }
}