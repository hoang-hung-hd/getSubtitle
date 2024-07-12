using getSubtitle.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace getSubtitle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger,IWebHostEnvironment environment)
        {
            _hostEnvironment = environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> getVideoContent(IFormFile video)
        {
            string originalPath = "";
            string destinationPath = "";

            try
            {
                if (video != null && video.Length > 0)
                {
                    var fileName = Path.GetFileName(video.FileName);
                    originalPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);
                    using (var fileStream = new FileStream(originalPath, FileMode.Create))
                    {
                        await video.CopyToAsync(fileStream);
                    }

                    destinationPath = "D:\\videoSub\\out%d.png";
                    string strCmdText = "";
                    Process cmd = new Process();

                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.UseShellExecute = false;

                    cmd.Start();
                    using (StreamWriter sw = cmd.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {

                            strCmdText = $"ffmpeg -i {originalPath} -vf fps=1 {destinationPath}";
                            sw.WriteLine(strCmdText);

                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
