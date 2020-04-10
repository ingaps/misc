using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DecimalNumbersExampleMvc.Models
{
    public class ItemForSale
    {

        public int ID { get; set; }
        [Required]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        
        [Required(ErrorMessage = "Item price is required")]
        [RegularExpression(@"^\$?-?\d+((\.(\d{1,2}))|(\,(\d{1,2})))?$", ErrorMessage = "Decimal number can have max 2 decimals")]
        public decimal Price { get; set; }

        public DateTime Date { get; set; }

       
        [Display(Name = "Username")]
        public string User { get; set; }

       
    }
    
    public static class ModelCollections
    {
        public static List<ItemForSale> ItemsForSaleList = new List<ItemForSale>();
    }
}