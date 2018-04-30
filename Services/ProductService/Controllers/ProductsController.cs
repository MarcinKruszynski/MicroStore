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
        public IEnumerable<string> Get()
        {
            return new string[] { "My Album", "My Album de Lux" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"DM {id}";
        }       
    }
}
