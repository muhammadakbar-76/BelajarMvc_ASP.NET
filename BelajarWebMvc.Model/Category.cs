using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BelajarMvcWeb.Models
{
    public class Category
    {   
        [Key] //this is unnecessary, EF smart enough
        public int Id { get; set; }
        [Required] //string in default will be nullable, so we need this for not null
        public string Name { get; set; } = string.Empty;
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage = "Display Order range is from 1 to 100")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
