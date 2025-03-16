using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.DTOs
{
    public class AccountUpdateDto  : BaseUpdateDto
    {
        private string? fullName;
        public string? FullName
        {
            get { return fullName?.Trim(); }
            set { fullName = value?.Trim(); }
        }

        private string? phone;
        public string? Phone
        {
            get { return phone?.Trim(); }
            set { phone = value?.Trim(); }
        }

        private string? career { get; set; }
        public string? Career
        {
            get { return career; }
            set { career = value.Trim(); }
        }


        private DateTime? dateOfBirth;

        public DateTime? DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value.Value.Date; }
        }
        public IFormFile? Image { get; set; }

    }
}
