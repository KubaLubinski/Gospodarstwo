using static System.Net.Mime.MediaTypeNames;

namespace Gospodarstwo.Models.ViewModels
{
    public class ItemWithNotes
    {
        public Item SelectedItem { get; set; }
        public int ReadingTime { get; set; }
        public int CommentsNumber { get; set; }
        public int OpinionsNumber { get; set; }
        public string Description { get; set; }
        public Note NewNote { get; set; }


        public ItemWithNotes()
        {
            ReadingTime = 0;
            CommentsNumber = 0;
            OpinionsNumber = 0;
        }
    }
}
