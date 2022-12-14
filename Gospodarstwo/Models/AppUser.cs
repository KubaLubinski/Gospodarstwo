using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gospodarstwo.Models
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Imię użytkownika:")]
        [MaxLength(25)]
        public string? FirstName { get; set; }

        [Display(Name = "Nazwisko użytkownika:")]
        [MaxLength(50)]
        public string? LastName { get; set; }

        #region
        [NotMapped]
        [Display(Name = "Pan/Pani:")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        #endregion

        [Display(Name = "Informacje o użytkowniku:")]
        [MaxLength(255, ErrorMessage = "Opis może mieć maksymalnie 255 znaków")]
        public string? Information { get; set; }

        [Display(Name = "Zdęcie użytkownika:")]
        [FileExtensions(Extensions = ". jpg,. png,. gif", ErrorMessage = "Niepoprawne rozszerzenie pliku")]
        [MaxLength(128)]
        public string? Photo { get; set; }

        //lista tekstow uzytkownika
        public virtual List<Item>? Items { get; set; }

        //lista noatatek uzytkownika
        public virtual List<Note>? Notes { get; set; }
    }
}
