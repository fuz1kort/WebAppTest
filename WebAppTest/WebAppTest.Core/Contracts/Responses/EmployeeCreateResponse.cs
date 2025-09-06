namespace WebAppTest.Core.Contracts.Responses;

/// <summary>
/// Ответ при создании сотрудника
/// </summary>
public class EmployeeCreateResponse
{
    /// <summary>
    /// Идентификатор нового сотрудника
    /// </summary>
    public int Id { get; init; }
}