using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Models;

namespace MauiApp1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public float CostPrice { get; set; }
        public int CategoryId { get; set; }
        public int? Quantity { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int RequiresStock { get; set; }
        public string Image { get; set; }
        public int? WholeSaleQty { get; set; }
        public decimal? WholeSalePrice { get; set; }
        public string Location { get; set; }
        public int ShopId { get; set; }
        public int LoggedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Min { get; set; }
        public string Signal { get; set; }
        public string Code { get; set; }
        public int Max { get; set; }
        public DateTime? DeletedAt { get; set; }
        public decimal Total { get; set; }
        public Category Category { get; set; }
    }
}
