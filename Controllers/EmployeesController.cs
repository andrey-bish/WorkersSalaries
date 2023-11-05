using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WorkersSalaries;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly List<Employee> _employees;

    public EmployeesController()
    {
        
        _employees = new List<Employee>();
        if (LoadEmployeesFromFile() is BadRequestObjectResult)
        {
            _employees = new List<Employee>
            {
                new HourlyEmployee {Id = Guid.NewGuid().ToString(),  Name = "John", HourlyRate = 20},
                new FixedEmployee {Id = Guid.NewGuid().ToString(), Name = "Jane", MonthlySalary = 3000},
                new HourlyEmployee {Id = Guid.NewGuid().ToString(), Name = "Bob", HourlyRate = 25},
                new FixedEmployee {Id = Guid.NewGuid().ToString(), Name = "Alice", MonthlySalary = 4000},
                new HourlyEmployee {Id = Guid.NewGuid().ToString(), Name = "Tom", HourlyRate = 30}
            };
        }
    }

    [HttpPost]
    public IActionResult AddEmployee([FromBody] EmployeeDto employeeDto)
    {
        Employee employee;
        if (employeeDto.Type == "Hourly")
        {
            employee = new HourlyEmployee
            {
                Id = Guid.NewGuid().ToString(),
                Name = employeeDto.Name,
                HourlyRate = employeeDto.HourlyRate
            };
        }
        else if (employeeDto.Type == "Fixed")
        {
            employee = new FixedEmployee
            {
                Id = Guid.NewGuid().ToString(),
                Name = employeeDto.Name,
                MonthlySalary = employeeDto.MonthlySalary
            };
        }
        else
        {
            return BadRequest("Invalid employee type");
        }

        _employees.Add(employee);
        SaveEmployeesToFile();

        return Ok();
    }

    [HttpGet]
    public IActionResult GetEmployees()
    {
        var sortedEmployees = _employees.OrderByDescending(e => e.CalculateAverageMonthlySalary())
                                       .ThenBy(e => e.Name)
                                       .ToList();

        var result = sortedEmployees.Select(e => new 
        {
            e.Id,
            e.Name,
            AverageMonthlySalary = e.CalculateAverageMonthlySalary()
        });

        return Ok(result); 
    }
    
    [HttpGet("top5")]
    public IActionResult GetTop5Employees()
    {
        var sortedEmployees = _employees.OrderByDescending(e => e.CalculateAverageMonthlySalary())
                                       .ThenBy(e => e.Name)
                                       .Take(5)
                                       .ToList();

        var result = sortedEmployees.Select(e => e.Name);

        return Ok(result);
    }

    [HttpGet("last3")]
    public IActionResult GetLast3Employees()
    {
        var sortedEmployees = _employees.OrderByDescending(e => e.CalculateAverageMonthlySalary())
                                       .ThenBy(e => e.Name)
                                       .TakeLast(3)
                                       .ToList();

        var result = sortedEmployees.Select(e => e.Id);

        return Ok(result);
    }

    [HttpPost("file")]
    public IActionResult SaveEmployeesToFile()
    {
        try
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Type,HourlyRate,MonthlySalary");

            foreach (var employee in _employees)
            {
                var type = employee is HourlyEmployee ? "Hourly" : "Fixed";
                var hourlyRate = employee is HourlyEmployee ? ((HourlyEmployee)employee).HourlyRate.ToString() : "";
                var monthlySalary = employee is FixedEmployee ? ((FixedEmployee)employee).MonthlySalary.ToString() : "";

                csv.AppendLine($"{employee.Id},{employee.Name},{type},{hourlyRate},{monthlySalary}");
            }

            System.IO.File.WriteAllText("employees.csv", csv.ToString());
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("file")]
    public IActionResult LoadEmployeesFromFile()
    {
        try
        {
            var employees = new List<Employee>();

            using (var reader = new StreamReader("employees.csv"))
            {
                var header = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var id = values[0];
                    var name = values[1];
                    var type = values[2];
                    var hourlyRate = string.IsNullOrEmpty(values[3]) ? 0 : int.Parse(values[3]);
                    var monthlySalary = string.IsNullOrEmpty(values[4]) ? 0 : double.Parse(values[4]);

                    if (type == "Hourly")
                    {
                        employees.Add(new HourlyEmployee
                        {
                            Id = id,
                            Name = name,
                            HourlyRate = hourlyRate
                        });
                    }
                    else if (type == "Fixed")
                    {
                        employees.Add(new FixedEmployee
                        {
                            Id = id,
                            Name = name,
                            MonthlySalary = monthlySalary
                        });
                    }
                }
            }

            _employees.AddRange(employees);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    public class EmployeeDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int HourlyRate { get; set; }
        public double MonthlySalary { get; set; }
    }
}