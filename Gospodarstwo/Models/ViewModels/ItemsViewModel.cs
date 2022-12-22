using static System.Net.Mime.MediaTypeNames;

namespace Gospodarstwo.Models.ViewModels
{
    public class ItemsViewModel
    {
        public IEnumerable<Item>? Items { get; set; }
        public ItemsView? ItemsView { get; set; }

    }
}
