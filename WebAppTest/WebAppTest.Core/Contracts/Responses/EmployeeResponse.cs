namespace WebAppTest.Core.Contracts.Responses;

/// <summary>
/// Ответ с данными сотрудника
/// </summary>
public class EmployeeResponse
{
    /// <summary>
    /// Идентификатор сотрудника
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Фамилия сотрудника
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Телефон сотрудника
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Идентификатор компании
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Паспорт сотрудника
    /// </summary>
    public PassportResponse Passport { get; set; } = null!;

    /// <summary>
    /// Отдел сотрудника
    /// </summary>
    public DepartmentResponse Department { get; set; } = null!;
}

/// <summary>
/// Паспорт сотрудника (в ответе)
/// </summary>
public class PassportResponse
{
    /// <summary>
    /// Тип паспорта
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string Number { get; set; } = null!;
}

/// <summary>
/// Отдел сотрудника (в ответе)
/// </summary>
public class DepartmentResponse
{
    /// <summary>
    /// Название отдела
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Телефон отдела
    /// </summary>
    public string? Phone { get; set; }
}