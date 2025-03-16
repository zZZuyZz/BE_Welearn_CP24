using DataLayer.Enums;

namespace ServiceLayer.DTOs
{
    public class ReviewDetailSignalrGetDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public ReviewResultEnum Result { get; set; }
        public int? ReviewerId { get; set; }
        public string? ReviewerUsername { get; set; }

    }
}