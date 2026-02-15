using System.ComponentModel.DataAnnotations;

namespace InfoInfo.Models
{
    public enum Rating
    {
        [Display(Name = "Brak")]
        Unrated = 0,
        [Display(Name = "Nieprzydatny")]
        Useless = 1,
        [Display(Name = "Słaby")]
        Poor = 2,
        [Display(Name = "Przeciętny")]
        Average = 3,
        [Display(Name = "Dobry")]
        Good = 4,
        [Display(Name = "Świetny")]
        Excellent = 5
    }

    public class Opinion
    {
        [Key]
        [Display(Name = "Identyfikator opinii:")]
        public int OpinionId { get; set; }

        [Display(Name = "Treść komentarza:")]
        [Required(ErrorMessage = "Proszę podać treść komentarza.")]
        [StringLength(1000, ErrorMessage = "Komentarz nie może być dłuższy niż 1000 znaków.")]
        public string Comment { get; set; } = "";

        [Display(Name = "Ocena tekstu:")]
        [Range(0, 5, ErrorMessage = "Proszę wybrać ocenę od 0 do 5")]
        public Rating? Rating { get; set; }

        [Display(Name = "Data dodania:")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedDate { get; set; }

        [Display(Name = "Komentowany tekst:")]
        public int TextId { get; set; }
        [Display(Name = "Komentowany tekst:")]
        public virtual Text? Text { get; set; }

        [Display(Name = "Autor komentarza:")]
        public string? UserId { get; set; }
        [Display(Name = "Autor komentarza:")]
        public virtual AppUser? Author { get; set; }
    }
}