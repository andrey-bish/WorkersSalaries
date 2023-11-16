using WorkersSalaries.Enums;
using WorkersSalaries.Models;
using WorkersSalaries.Models.DTO;
using WorkersSalaries.Services.Interfaces;

namespace WorkersSalaries.Services;

public class EmployeeService: IEmployeeService
{
    private readonly IEmployeeDataManagementService _employeeDataManagementService;

    public EmployeeService(IEmployeeDataManagementService employeeDataManagementService)
    {
        _employeeDataManagementService = employeeDataManagementService;
    }
    
    public async Task AddEmployee(EmployeeDto employeeDto, CancellationToken ct)
    {
        Employee? employee = null;
        if (employeeDto.Type == EmployeeType.Hourly.ToString())
        {
            employee = new HourlyEmployee
            {
                Id = Guid.NewGuid().ToString(),
                Name = employeeDto.Name,
                HourlyRate = employeeDto.HourlyRate
            };
        }
        else if (employeeDto.Type == EmployeeType.Fixed.ToString())
        {
            employee = new FixedEmployee
            {
                Id = Guid.NewGuid().ToString(),
                Name = employeeDto.Name,
                MonthlySalary = employeeDto.MonthlySalary
            };
        }

        if (employee != null) await SaveEmployeeToFile(employee, ct);
    }

    public async Task<List<Employee>> GetEmployees(CancellationToken ct)
    {
        var employees = await _employeeDataManagementService.LoadEmployeesFromFile(ct);
        
        var sortedEmployees = employees.OrderByDescending(e => e.CalculateAverageMonthlySalary())
            .ThenBy(e => e.Name)
            .ToList();
        
        return sortedEmployees;
    }

    public async Task<List<string>> GetTop5Employees(CancellationToken ct)
    {
        var employees = await _employeeDataManagementService.LoadEmployeesFromFile(ct);
        var sortedEmployees = employees.OrderByDescending(e => e.CalculateAverageMonthlySalary())
            .ThenBy(e => e.Name)
            .Take(5)
            .ToList();

        return sortedEmployees.Select(e => e.Name).ToList();
    }

    public async Task<List<string>> GetLast3Employees(CancellationToken ct)
    {
        var employees = await _employeeDataManagementService.LoadEmployeesFromFile(ct);
        var sortedEmployees = employees.OrderByDescending(e => e.CalculateAverageMonthlySalary())
            .ThenBy(e => e.Name)
            .TakeLast(3)
            .ToList();

        return sortedEmployees.Select(e => e.Id).ToList();
    }

    private Task SaveEmployeeToFile(Employee employee, CancellationToken ct)
    {
        _employeeDataManagementService.SaveEmployeeToFile(employee, ct);
        return Task.CompletedTask;
    }
}