namespace WorkersSalaries.Models;

public abstract class Employee
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double MonthlySalary { get; set; }
    
    public abstract double CalculateAverageMonthlySalary();
}

// Потомок с почасовой оплатой
public class HourlyEmployee : Employee
{
    public int HourlyRate { get; set; }

    public override double CalculateAverageMonthlySalary()
    {
        return MonthlySalary = Math.Round(20.8 * 8 * HourlyRate, 2);
    }
}

// Потомок с фиксированной оплатой
public class FixedEmployee : Employee
{
    public override double CalculateAverageMonthlySalary()
    {
        return MonthlySalary;
    }
}