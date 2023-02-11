using Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl
{
    public interface IEmployeeService
    {
        Middle<Employee> ReadAll(int page = 1, int pageSize = 10);
        Middle<Employee> ReadName(string name);
        Middle<Employee> Create(IFormFile model);
    }
}
