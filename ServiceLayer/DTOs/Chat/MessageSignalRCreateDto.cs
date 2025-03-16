namespace ServiceLayer.DTOs
{
    public class MessageSignalrCreateDto
    {
        private string content;

        public string Content
        {
            get { return content.Trim(); }
            set { content = value.Trim(); }
        }

        public DateTime? TimeSent { get; set; } = DateTime.Now;
    }
}
