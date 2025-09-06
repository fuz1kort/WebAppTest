namespace WebAppTest.Core.Domain;

/// <summary>
/// Представляет сотрудника компании
/// </summary>
public class Employee
{
    /// <summary>
    /// Уникальный идентификатор сотрудника
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Фамилия сотрудника
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Номер телефона сотрудника
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Идентификатор компании, в которой работает сотрудник
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Идентификатор отдела
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// Паспортные данные сотрудника
    /// </summary>
    public Passport Passport { get; init; } = null!;

    /// <summary>
    /// Информация об отделе, в котором работает сотрудник
    /// </summary>
    public Department Department { get; init; } = null!;
}