using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProductsController: Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "DM 4", "Drużyna mistrzów 5" };
        //}
        
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"DM {id}";
        }       
    }
}
