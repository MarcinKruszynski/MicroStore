using System;

namespace ProductService.Model
{
    public class ProductItem
    {
        public int Id { get; set; }

        public string Name { get; set; }        

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int RemoveStock(int quantity)
        {
            if (StockQuantity == 0 || quantity <= 0)            
                return -1;                       

            int removed = Math.Min(quantity, StockQuantity);

            StockQuantity -= removed;

            return removed;
        }
    }
}
