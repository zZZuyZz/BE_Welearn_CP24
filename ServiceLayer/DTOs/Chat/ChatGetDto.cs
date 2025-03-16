namespace ServiceLayer.DTOs
{
    public class ChatGetDto
    {
        public string AccountFullName { get; set; }
        public string AccountUsername { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
