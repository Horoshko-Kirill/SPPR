using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB_353505_Horoshko.Domain.Entities
{
    public class Book
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Categoty")]
        public Category? Category { get; set; }

        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        [Display(Name = "Image")]
        public string? Image { get; set; }
    }
}
