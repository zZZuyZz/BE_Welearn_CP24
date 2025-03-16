using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class AuthEndpoints
    {
        public const string Login = $"[{Actor.Student_Parent}/{Finnished.True}] Login for student with username or email. Return JWT Token";
        public const string LoginWithGGIdToken = $"[{Actor.Student_Parent}/{Finnished.False}/{Auth.GoogleId}] Login for student with googel id token. Return JWT Token";
        public const string LoginWithGGAcessToken = $"[{Actor.Student_Parent}/{Finnished.False}/{Auth.GoogleAccess}] Login for student with googel access token. Return JWT Token";
        public const string StudentRegister = $"[{Actor.Student}/{Finnished.True}] Register for student with form";
        public const string ParentRegister = $"[{Actor.Guest}/{Finnished.True}] Register for parent with form";
        public const string StudentRegisterWithGgAccessToken = $"[{Actor.Student}/{Finnished.False}] Register for student with form";
        public const string ParentRegisterWithGgAccessToken = $"[{Actor.Guest}/{Finnished.True}] Register for parent with form";
        public const string GetTokens = $"[{Actor.Test}/{Finnished.True}] Get all the token sent in the header of the swagger request";
        public const string GetRoleData = $"[{Actor.Test}/{Finnished.True}] Get all the data of the account of the swagger request";
        public const string GetStudentData = $"[{Actor.Test}/{Finnished.True}] Test if the account of the swagger request is a student";
        public const string GetUserData = $"[{Actor.Test}/{Finnished.True}] Test if the account of the swagger request is a student";
    }
}
