using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace InfoInfo.Models
{
    public class Category
    {
        public Category()
        {
            Active = true;
            Display = true;
            Texts = new List<Text>();
        }
        [Key] //zbędne oznaczenie, gdyż w nazwie pola występuje ciąg Id
        [Display(Name = "Identyfikator kategorii: ")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Proszę podać nazwę kategorii.")]
        [Display(Name = "Nazwa kategorii: ")]
        [StringLength(50, ErrorMessage = "Nazwa kategorii nie może być dłuższa niż 50 znaków.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Proszę podać opis kategorii.")]
        [Display(Name = "Opis kategorii: ")]
        [StringLength(255, ErrorMessage = "Opis kategorii nie może być dłuższy niż 255 znaków.")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Proszę podać nazwę ikony.")]
        [Display(Name = "Nazwa ikony: ")]
        [StringLength(30, ErrorMessage = "Nazwa ikony nie może być dłuższa niż 30 znaków.")]
        public string Icon { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Czy aktywna?")]
        public bool Active { get; set; }
        [Required]
        [Display(Name = "Czy wyświetlać?")]
        public bool Display { get; set; }

        public ICollection<Text> Texts { get; set; }
    }
}
