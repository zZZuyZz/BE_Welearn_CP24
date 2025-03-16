using API.SwaggerOption.Const;

namespace API.SwaggerOption.Descriptions
{
    public class AuthDescriptions
    {
        public const string Login = "Login for student with username or email. Return JWT Token if successfull";
        public const string LoginWithGGId = "Login for student with googel id token in Header. Return JWT Token if successfull";
        public const string LoginWithGGAcessToken = "Login for student with googel access token in Header. Return JWT Token if successfull";
        public const string StudentRegister = "Register for student with form";
        public const string ParentRegister = "Register for parent with form";
        public const string StudentRegisterWithGgAccessToken = "Register for student with form";
        public const string ParentRegisterWithGgAccessToken = "Register for parent with form";
    }
}
