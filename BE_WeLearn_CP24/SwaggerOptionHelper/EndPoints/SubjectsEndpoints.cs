using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class SubjectsEndpoints
    {
        public const string GetSubject = $"[{Actor.Student_Admin}/{Finnished.True}/{Auth.True}] Get list of subjects";
        public const string CreateSubject = $"[{Actor.Student_Admin}/{Finnished.True}/{Auth.True}] Create new subject";
        
    }
}
