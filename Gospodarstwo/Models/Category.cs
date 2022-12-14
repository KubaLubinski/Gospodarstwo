using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gospodarstwo.Models
{
    public class Category
    {
        [Key]
        [Display(Name = "Identyfikator kategorii:")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Proszę podać nazwę kategorii.")]
        [Display(Name = "Nazwa kategorii:")]
        [MaxLength(50, ErrorMessage = "Nazwa kategorii nie może być dłuższa niż 50 znaków.")]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Proszę podać opis kategorii.")]
        [Display(Name = "Opis kategorii:")]
        [MaxLength(255, ErrorMessage = "Opis kategorii nie może byc dłuższy niż 255 znaków.")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Czy aktywna?")]
        [DefaultValue(true)]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Czy wyświetlać?")]
        [DefaultValue(true)]
        public bool Display { get; set; }

        //lista wszystkich tekstów w kategorii
        public virtual List<Item>? Items { get; set; }
    }
}
