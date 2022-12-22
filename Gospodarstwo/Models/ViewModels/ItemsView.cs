namespace Gospodarstwo.Models.ViewModels
{
    public class ItemsView
    {
        public ItemsView(int pageSize = 5)
        {
            PageSize = pageSize;
        }

        public int ItemCount { get; set; }
         public int PageSize { get; set; }
         public int PageNumber { get; set; }
         public int PageCount => (int)Math.Ceiling((decimal)ItemCount / PageSize);
        public int? Category { get; set; }
        public string? Author { get; set; }
        public string? Phrase { get; set; }


    }
}
