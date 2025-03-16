using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class StatsEndpoints
    {
        public const string GetStatForAccountInMonthNew = $"[{Actor.Student_Parent}/{Finnished.False}/{Auth.True}] Student's stat by month";
        public const string GetStatForGroupInMonthNew = $"[{Actor.Student_Parent}/{Finnished.False}/{Auth.True}] Group's stat by month";
        public const string GetStatForStudent = $"[{Actor.Student_Parent}/{Finnished.False}/{Auth.True}] Student's stat by month";
        public const string GetStatForGroup = $"[{Actor.Student_Parent}/{Finnished.False}/{Auth.True}] Group's stat by month";
    }
}
