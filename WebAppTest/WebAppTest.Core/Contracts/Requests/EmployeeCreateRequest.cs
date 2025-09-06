namespace WebAppTest.Core.Contracts.Requests;

/// <summary>
/// Запрос на создание сотрудника
/// </summary>
public class EmployeeCreateRequest
{
    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Фамилия сотрудника
    /// </summary>
    public string Surname { get; init; } = null!;

    /// <summary>
    /// Телефон сотрудника
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Идентификатор компании
    /// </summary>
    public int CompanyId { get; init; }

    /// <summary>
    /// Паспорт сотрудника
    /// </summary>
    public PassportCreateRequest PassportCreate { get; init; } = null!;

    /// <summary>
    /// Отдел сотрудника
    /// </summary>
    public DepartmentCreateRequest DepartmentCreate { get; init; } = null!;
}

/// <summary>
/// Паспорт сотрудника (в запросе)
/// </summary>
public class PassportCreateRequest
{
    /// <summary>
    /// Тип паспорта
    /// </summary>
    public string Type { get; init; } = null!;

    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string Number { get; init; } = null!;
}

/// <summary>
/// Отдел сотрудника (в запросе)
/// </summary>
public class DepartmentCreateRequest
{
    /// <summary>
    /// Название отдела
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Телефон отдела
    /// </summary>
    public string? Phone { get; init; }
}