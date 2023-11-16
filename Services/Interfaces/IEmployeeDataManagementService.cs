using WorkersSalaries.Models;

namespace WorkersSalaries.Services.Interfaces;

public interface IEmployeeDataManagementService
{
    public Task SaveEmployeeToFile(Employee employee, CancellationToken ct);
    public Task<List<Employee>> LoadEmployeesFromFile(CancellationToken ct);
}