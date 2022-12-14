using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Gospodarstwo.Models
{
    public class Note
    {
        [Key]
        [Display(Name = "Identyfikator:")]
        public int NoteId { get; set; }

        [Required(ErrorMessage = "Proszę podać treść notatki.")]
        [Display(Name = "Treść notatki:")]
        [DataType(DataType.MultilineText)]
        public string? Content { get; set; }

        [Display(Name = "Data dodania:")]
        [DataType(DataType.Date, ErrorMessage = "Niepoprawny format daty")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public System.DateTime AddedDate { get; set; }

        [Display(Name = "Komentowany tekst:")]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }

        [Display(Name = "Autor notatki:")]
        public string? Id { get; set; }
        [ForeignKey("Id")]
        public virtual AppUser? User { get; set; }
    }
}
