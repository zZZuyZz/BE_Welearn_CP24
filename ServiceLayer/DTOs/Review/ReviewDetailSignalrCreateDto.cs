using DataLayer.Enums;

namespace ServiceLayer.DTOs
{
    public class ReviewDetailSignalrCreateDto  : BaseCreateDto
    {
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public ReviewResultEnum Result { get; set; }
    }
}
