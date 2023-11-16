using System.Text;
using WorkersSalaries.Enums;
using WorkersSalaries.Models;
using WorkersSalaries.Services.Interfaces;

namespace WorkersSalaries.Services;

public class EmployeeDataManagementService : IEmployeeDataManagementService
{
    public async Task SaveEmployeeToFile(Employee employee, CancellationToken ct)
    {
        var csv = new StringBuilder();
        
        if (!File.Exists("employees.csv"))
        {
            csv.AppendLine("Id,Name,Type,HourlyRate,MonthlySalary");
        }

        var type = employee is HourlyEmployee ? "Hourly" : "Fixed";
        var hourlyRate = employee is HourlyEmployee ? ((HourlyEmployee) employee).HourlyRate.ToString() : "";
        var monthlySalary = employee is FixedEmployee ? ((FixedEmployee) employee).MonthlySalary.ToString() : "";

        csv.AppendLine($"{employee.Id},{employee.Name},{type},{hourlyRate},{monthlySalary}");

        await File.AppendAllTextAsync("employees.csv", csv.ToString(), ct);
    }

    public async Task<List<Employee>> LoadEmployeesFromFile(CancellationToken ct)
    {
        var employees = new List<Employee>();

        using var reader = new StreamReader("employees.csv");
        var header = await reader.ReadLineAsync(ct);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(ct);
            var values = line.Split(',');

            var id = values[0];
            var name = values[1];
            var type = values[2];
            var hourlyRate = string.IsNullOrEmpty(values[3]) ? 0 : int.Parse(values[3]);
            var monthlySalary = string.IsNullOrEmpty(values[4]) ? 0 : double.Parse(values[4]);

            if (type == EmployeeType.Hourly.ToString())
            {
                employees.Add(new HourlyEmployee
                {
                    Id = id,
                    Name = name,
                    HourlyRate = hourlyRate
                });
            }
            else if (type == EmployeeType.Fixed.ToString())
            {
                employees.Add(new FixedEmployee
                {
                    Id = id,
                    Name = name,
                    MonthlySalary = monthlySalary
                });
            }
        }
        return employees;
    }

}