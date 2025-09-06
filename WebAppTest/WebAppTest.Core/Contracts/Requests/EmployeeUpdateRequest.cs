namespace WebAppTest.Core.Contracts.Requests;

/// <summary>
/// Запрос на обновление сотрудника
/// </summary>
public class EmployeeUpdateRequest
{
    /// <summary>
    /// Имя сотрудника (опционально)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Фамилия сотрудника (опционально)
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// Телефон сотрудника (опционально)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Идентификатор компании (опционально)
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Паспортные данные (опционально)
    /// </summary>
    public PassportUpdateRequest? Passport { get; set; }

    /// <summary>
    /// Информация о департаменте (опционально)
    /// </summary>
    public DepartmentUpdateRequest? Department { get; set; }
}

/// <summary>
/// Запрос на обновление паспортных данных
/// </summary>
public class PassportUpdateRequest
{
    /// <summary>
    /// Тип паспорта (опционально)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Номер паспорта (опционально)
    /// </summary>
    public string? Number { get; set; }
}

/// <summary>
/// Запрос на обновление информации о департаменте
/// </summary>
public class DepartmentUpdateRequest
{
    /// <summary>
    /// Название департамента (опционально)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Телефон департамента (опционально)
    /// </summary>
    public string? Phone { get; set; }
}