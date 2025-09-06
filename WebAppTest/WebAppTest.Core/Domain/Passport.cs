namespace WebAppTest.Core.Domain;

/// <summary>
/// Представляет паспортные данные
/// </summary>
public class Passport
{
    /// <summary>
    /// Идентификатор паспорта (сотрудника)
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Тип паспорта
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Сотрудник
    /// </summary>
    public Employee Employee { get; init; } = null!;
}