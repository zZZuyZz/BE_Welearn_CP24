namespace ServiceLayer.DTOs
{
    public class MessageSignalrGetDto
    {
        public string SenderDisplayName { get; set; }
        public string SenderUsername { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
    }
}
