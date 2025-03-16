using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIExtension.Validator
{
    public class ValidatorResult
    {
        public ValidatorResult()
        {
            Failures = new List<string?>();
            FailuresMap=new Dictionary<string, string>();
        }

        public bool IsValid => !Failures.Any();

        public List<string?> Failures { get;private set; }
        public Dictionary<string, string> FailuresMap { get; set; }
        public void Add(string error, string? errorType= "Exception")
        {
            Failures.Add(error);
                FailuresMap.Add(errorType, error);
            //if(FailuresMap.ContainsKey(errorType)) 
            //{
            //}
            //else
            //{
            //    FailuresMap
            //}

            //if (FailuresMap.ContainsKey(errorName))
            //{
            //    FailuresMap.Add(errorName, error);
            //}
        }
    }
    public static class ValidateErrType
    {
        public static string Exception = "Exception";
        public static string Role = "Role";
        public static string Unauthorized = "Unauthorized";
        public static string IdNotMatch = "IdNotMatch";
        
        public static string NotFound = "NotFound";
    }
}
