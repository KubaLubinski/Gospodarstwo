namespace Gospodarstwo.Models.ViewModels
{
    public class HomeDataVM
    {
        public IEnumerable<Category>? DisplayCategories { get; set; }
        public IEnumerable<AppUser>? Authors { get; set; }
        public IEnumerable<Item>? Items { get; set; }
        public IEnumerable<Unit>? Units { get; set; }
    }
}
