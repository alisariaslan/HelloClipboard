namespace HelloClipboard.Models
{
    public class MenuState
    {
        public bool CanCopy { get; set; }
        public bool CanOpen { get; set; }
        public bool CanSave { get; set; }
        public bool IsPinned { get; set; }
        public string PinText { get; set; }
    }
}
