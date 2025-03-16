using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class AccountsEndpoints
    {
        public const string SearchStudent = "[Student, Parent/Not Finnished/Authorize] Search students by id, username, mail, Full Name";
        public const string GetProfile = $"[{Actor.Student_Parent}/{Finnished.True}/{Auth.True}] Get the self profile";
        public const string UpdatePRofile = $"[{Actor.Student_Parent}/{Finnished.True}/{Auth.True}] Update logined profile";
        public const string ChangePassword = $"[{Actor.Student_Parent}/{Finnished.True}/{Auth.True}] Update account password";
        public const string ConfirmResetPassNoUse = $"[{Actor.Guest}/{Finnished.True}/{Auth.False}] DO NOT USE. Reset and send new password to email. Call the above api";
        public const string GetSuperviseRequest = $"[{Actor.Student}/{Finnished.True}/{Auth.True}]Get waiting supervision list for student";
        public const string RequestSupervise = $"[{Actor.Parent}/{Finnished.True}/{Auth.True}]Parent claim a student is their children";
        public const string ConfirmResetPassword = $"[{Actor.Guest}/{Finnished.True}/{Auth.False}] Send a link to email to reset password";
        public const string AcceptSupervise = $"[{Actor.Student}/{Finnished.False}/{Auth.True}]Student accept parent";
        public const string DeclineSupervise = $"[{Actor.Student}/{Finnished.True}/{Auth.True}]Student Decline parent";
        public const string DeleteSupervise = $"[{Actor.Student}/{Finnished.True}/{Auth.True}]Student Decline parent";
        public const string GetParentForStudent = $"[{Actor.Student}/{Finnished.True}/{Auth.True}]Get parents list for student";
        public const string GetStudentForParent = $"[{Actor.Parent}/{Finnished.True}/{Auth.True}]Get students list for parent";
        public const string GetAllAccount = $"[{Actor.Test}/{Finnished.False}] Get all the account";
        public const string GetUser = $"[{Actor.Test}/{Finnished.False}] Get account info ";
    }
}
