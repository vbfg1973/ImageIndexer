using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Dto;
using Core.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ImageApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageRetrieveController : ControllerBase
    {
        private ILogger _logger;

        public ImageRetrieveController(ILogger logger)
        {
            _logger = logger;
        }

        // POST: api/ImageRetrieve
        [HttpPost]
        public void Post([FromBody] ImageRetrieve image)
        {
            _logger.Information(image.RedditId);

            ImageFound i = new ImageFound()
            {
                RedditId = image.RedditId,
                Subreddit = image.Subreddit,
                Author = image.Author,
                Url = image.Url,
                CreatedUtc = image.CreatedUtc
            };
        }
    }
}
