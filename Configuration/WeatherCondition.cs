namespace ApiMonitoringBot.Configuration;

public record WeatherCondition
{
    public required string City { get; init; }
    public PriceCondition? Temperature { get; init; } // Используем PriceCondition, так как там уже есть GreaterThan и LessThan
}