using System.ComponentModel.DataAnnotations;

namespace Gospodarstwo.Models
{
    public class Unit
    {
        [Key]
        [Display(Name = "Identyfikator:")]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "Nazwa jednostki:")]
        [MaxLength(50, ErrorMessage = "Nazwa jednostki nie może być dłuższa niz 50 znaków.")]
        public string? UnitName { get; set; }

        [Required]
        [Display(Name = "Skrócona nazwa jednostki:")]
        [MaxLength(10, ErrorMessage = "Skrót nie może być dłuższy niż 10 znaków.")]
        public string? UnitShortName { get; set; }

        public virtual List<Item>? Items { get; set; }
    }
}
