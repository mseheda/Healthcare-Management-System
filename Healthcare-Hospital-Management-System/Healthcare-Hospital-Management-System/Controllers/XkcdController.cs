using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System;
using Healthcare_Hospital_Management_System.Infrastructure;

namespace Xkcd.Controllers
{
    /// <summary>
    /// Enter to download XKCD comics from #{number1} to {number2 - 1}
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class XkcdController : ControllerBase
    {
        private readonly IXkcdClient _xkcdClient;

        public XkcdController(IXkcdClient xkcdClient)
        {
            _xkcdClient = xkcdClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var number1 = RandomNumberGenerator.GetInt32(1, 2048);
            var number2 = number1 + 5;

            var images = _xkcdClient.GetComicImageAsync(number1, number2);

            await foreach (var image in images.WithCancellation(default))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"xkcd-{image.Name}");

                await System.IO.File.WriteAllBytesAsync(filePath, image.Content, default);
            }

            return Ok();
        }
    }
}
