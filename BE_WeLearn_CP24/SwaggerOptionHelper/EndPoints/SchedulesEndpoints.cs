using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class SchedulesEndpoints
    {
        public const string GetSchedulesForGroup = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Get all Schedule meetings of group";
    }
}
