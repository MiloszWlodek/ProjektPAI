namespace InfoInfo.Models.ViewModels
{
    public class HomeDataVM
    {
        public IEnumerable<Category>? DisplayCategories { get; set; }
        public IEnumerable<AppUser>? Authors { get; set; }
    }
}