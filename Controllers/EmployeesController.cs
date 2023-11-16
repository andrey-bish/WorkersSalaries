using Microsoft.AspNetCore.Mvc;
using WorkersSalaries.Models.DTO;
using WorkersSalaries.Services.Interfaces;

namespace WorkersSalaries.Controllers;

[ApiController]
[Route("Employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost]
    public IActionResult AddEmployee([FromBody] EmployeeDto employeeDto, CancellationToken ct = default)
    {
        try
        {
            _employeeService.AddEmployee(employeeDto, ct);
        }
        catch
        {
            return BadRequest("Invalid employee type");
        }
    
        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetEmployees(CancellationToken ct = default)
    {
        var result = _employeeService.GetEmployees(ct);
        
        return Ok(result.Result); 
    }
    
    [HttpGet("top5")]
    public IActionResult GetTop5Employees(CancellationToken ct = default)
    {
        var result = _employeeService.GetTop5Employees(ct);
        
        return Ok(result.Result);
    }
    
    [HttpGet("last3")]
    public IActionResult GetLast3Employees(CancellationToken ct = default)
    {
        var result = _employeeService.GetLast3Employees(ct);
        
        return Ok(result.Result);
    }
}