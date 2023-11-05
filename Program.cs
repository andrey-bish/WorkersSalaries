var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public abstract class Employee
{
    private string _id;

    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public string Name { get; set; }

    public abstract double CalculateAverageMonthlySalary();
}

// Потомок с почасовой оплатой
public class HourlyEmployee : Employee
{
    public int HourlyRate { get; set; }

    public override double CalculateAverageMonthlySalary()
    {
        return Math.Round(20.8 * 8 * HourlyRate, 2);
    }
}

// Потомок с фиксированной оплатой
public class FixedEmployee : Employee
{
    public double MonthlySalary { get; set; }

    public override double CalculateAverageMonthlySalary()
    {
        return MonthlySalary;
    }
}
