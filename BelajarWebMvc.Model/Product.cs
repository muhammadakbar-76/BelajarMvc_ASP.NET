using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string ISBN { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        [Range(1, 300000)]
        [Display(Name = "Price for 1-50")]
        public double Price { get; set; }
        [Required]
        [Range(1, 300000)]
        [Display(Name = "Price for 51-100")]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 300000)]
        [Display(Name = "Price for 100+")]
        public double Price100 { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")] // we don't need explicit type this, but if somehow the id name it's not (field)Id
        [ValidateNever]
        public Category Category { get; set; } 
        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType CoverType { get; set; }
    }
}
