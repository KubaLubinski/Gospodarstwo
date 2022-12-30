using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gospodarstwo.Models
{
    public class Item
    {
        [Key]
        [Display(Name = "Identyfikator:")]
        public int ItemId { get; set; }

        [Required]
        [Display(Name = "Nazwa zasobu:")]
        [MaxLength(75, ErrorMessage = "Nazwa nie może przekraczać 75 znaków.")]
        public string? ItemName { get; set; }

        [Required]
        [Display(Name = "Treść Wpisu:")]
        public string? Content {get; set; }

        [Display(Name = "Grafika do wpisu:")]
        [MaxLength(128)]
        [FileExtensions(Extensions = ".jpg, .png, .gif", ErrorMessage = "Niepoprawne rozszerzenie pliku.")]
        public string? Graphic { get; set; }

        [Required]
        [Display(Name = "Czy aktywne?")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Data dodania")]
        [DataType(DataType.Date, ErrorMessage = "Niepoprawny format daty")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public System.DateTime AddedDate { get; set; }

        [Display(Name = "Kategoria tekstu:")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Autor tekstu:")]
        public string? Id { get; set; }
        [ForeignKey("Id")]
        public virtual AppUser? User { get; set; }

        [Display(Name = "Ilośc zasobu:")]
        public int ItemQuantity { get; set; }

        [Display(Name = "Pojemność magazynu:")]
        public int MaxStoreCapacity { get; set; }

        #region
        [NotMapped]
        [Display(Name = "Procent wypełnienia magazynu:")]
        public double FillLevel
        {
            get { return (double)ItemQuantity / (double)MaxStoreCapacity * 100; }
        }
        #endregion

        [Display(Name = "Jednostka (Sposób zliczania):")]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit? Unit { get; set; }

        public virtual List<Note>? Notes { get; set; }
    }
}
