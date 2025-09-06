namespace WebAppTest.Core.Domain;

/// <summary>
/// Представляет отдел компании
/// </summary>
public class Department
{
    /// <summary>
    /// Идентификатор отдела
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Название отдела
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Номер телефона отдела
    /// </summary>
    public string? Phone { get; set; }
}