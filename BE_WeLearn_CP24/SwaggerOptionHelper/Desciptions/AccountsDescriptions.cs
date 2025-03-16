using API.SwaggerOption.Const;

namespace API.SwaggerOption.Descriptions
{
    public class AccountsDescriptions
    {
        public const string SearchStudent = "\"Search theo tên, username, id\" +\r\n            \"<br/>Để search thêm thành viên mới cho group, thêm groupId để loại ra hết những student đã liên quan đến nhóm\" +\r\n            \"<br/>Để phụ huynh search để tìm con, chỉ cần có token của parent là dc, sẽ ko search ra con của mình\"";
        public const string GetProfile = "Lấy profile của account đang login, accountId dc lấy từ jwtToken";
        public const string UpdateProfile = "accountId là id của account đang cập nhật profile";
        public const string ChangePassword = "accountId là id của account đang cập nhật profile";
        public const string ConfirmResetPassword = $"[{Actor.Guest}/{Finnished.True}/{Auth.False}] Send a link to email to reset password";
        public const string ConfirmResetPassNoUse = "Không sử dụng. Gọi api trên để nhận được mail chứa 1 link gọi api này";

    }
}
