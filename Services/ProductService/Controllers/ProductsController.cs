﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using ProductService.Data;
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
        private readonly IEndpointInstance _endpoint;
        private readonly ProductContext _productContext;

        public ProductsController(ProductContext context, IEndpointInstance endpoint)
        {
            _endpoint = endpoint;
            _productContext = context ?? throw new ArgumentNullException(nameof(context));

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]        
        public async Task<IActionResult> Get()
        {
            var items = await _productContext.ProductItems
                .OrderBy(c => c.Name)                
                .ToListAsync();            

            return Ok(items);
        }        

        [HttpGet("{id}")]        
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = await _productContext.ProductItems.SingleOrDefaultAsync(ci => ci.Id == id);
            if (item != null)
            {
                return Ok(item);
            }

            return NotFound();
        }
    }
}
