using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameFr { get; set; }
        public string DescriptionFr { get; set; }
        public string BusinessId { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public int LoggedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CategoryId { get; set; }
        public string Type { get; set; }
        public string Parent { get; set; }
    }




}
