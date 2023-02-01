using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class FallBackController : Controller
    {
        /* this is going to tell our api to go if it is a route it doesn't know about, it is going to tell it to go to the index.html 
        which is going to responsible for routes that our app doesn't know about 
        */
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }

    }
}