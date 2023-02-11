using Microsoft.AspNetCore.Http;

namespace Common
{
    public class Employee
    {
        public string name { get; set; }
        public string email { get; set; }
        public string tel { get; set; }
        public DateTime joined { get; set; }
    }
}