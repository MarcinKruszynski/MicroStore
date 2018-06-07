using ProductService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService
{
    public static class Config
    {
        public static IEnumerable<ProductItem> GetProductItems()
        {
            return new List<ProductItem>
            {
                new ProductItem
                {
                    Name = "Progressive apps - 31.06.2018",
                    StockQuantity = 5000,
                    Price = 404
                },
                new ProductItem
                {
                    Name = ".NET microservices - 31.09.2018",
                    StockQuantity = 6000,
                    Price = 500
                }
            };
        }
    }
}
