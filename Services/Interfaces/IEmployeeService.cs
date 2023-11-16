using WorkersSalaries.Models;
using WorkersSalaries.Models.DTO;

namespace WorkersSalaries.Services.Interfaces;

public interface IEmployeeService
{
    public Task AddEmployee(EmployeeDto employeeDto, CancellationToken ct);
    public Task<List<Employee>> GetEmployees(CancellationToken ct);
    public Task<List<string>> GetTop5Employees(CancellationToken ct);
    public Task<List<string>> GetLast3Employees(CancellationToken ct);

}