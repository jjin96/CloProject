using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Common
{
    public class Middle<T>
    {
        public IEnumerable<T> List { get; set; }
        public string Message { get; set; }

        public bool IsSuccess { get; set; } = false;

        public HttpStatusCode HttpCode { get; set; }
        public string Json { get; set; }
    }
}
