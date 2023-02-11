using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Bl;

namespace CloProject1.Controllers
{

    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetList([FromQuery] int page, [FromQuery] int pageSize)
        {
            Middle<Employee> result = new Middle<Employee>();

            result = _employeeService.ReadAll();

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("api/[controller]/{name}")]
        //[Route("api/[controller]/{id:name}")]
        public IActionResult SelectName(string name)
        {
            Middle<Employee> result = new Middle<Employee>();

            if (string.IsNullOrEmpty(name))
            {
                result.Message = "사원명을 입력해주세요.";
                return BadRequest(result);
            }

            result = _employeeService.ReadName(name);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Route("api/[controller]")]
        public IActionResult Create(IFormFile file)
        {
            Middle<Employee> result = new Middle<Employee>();

            result = _employeeService.Create(file);

            if (result.IsSuccess)
            {
                return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}