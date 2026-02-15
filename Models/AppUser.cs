using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace InfoInfo.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Information = string.Empty;
            Photo = string.Empty;
            Texts = new List<Text>();
            Opinions = new List<Opinion>();
        }

        [Display(Name = "Imię:")]
        [MaxLength(20, ErrorMessage = "Imię nie może być dłuższe niż 20 znaków")]
        [RegularExpression(@"^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*$", ErrorMessage = "Imię powinno zaczynać się wielką literą i zawierać tylko litery")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko:")]
        [MaxLength(50, ErrorMessage = "Nazwisko nie może być dłuższe niż 50 znaków")]
        [RegularExpression(@"^[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*(?:[-][A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]*)?$", ErrorMessage = "Nazwisko powinno zaczynać się wielką literą (dopuszczalny jeden myślnik)")]
        public string LastName { get; set; }

        [NotMapped]
        [Display(Name = "Imię i nazwisko:")]
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return "Nie podano";
                return $"{FirstName} {LastName}".Trim();
            }
        }

        [Display(Name = "Informacja o użytkowniku:")]
        [MaxLength(800, ErrorMessage = "Opis nie może przekraczać 800 znaków")]
        [DataType(DataType.MultilineText)]
        public string Information { get; set; }

        [Display(Name = "Zdjęcie profilowe:")]
        [MaxLength(128)]
        public string Photo { get; set; }

        // Relacje
        public ICollection<Text> Texts { get; set; }
        public ICollection<Opinion> Opinions { get; set; }
    }
}