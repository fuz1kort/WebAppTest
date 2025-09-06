using WebAppTest.Core.Contracts.Requests;
using WebAppTest.Core.Contracts.Responses;

namespace WebAppTest.Core.Interfaces.Services;

/// <summary>
/// Интерфейс сервиса для управления сотрудниками.
/// Содержит бизнес-логику поверх репозитория.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Добавляет нового сотрудника.
    /// </summary>
    /// <param name="request">Данные сотрудника для добавления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Id добавленного сотрудника.</returns>
    Task<EmployeeCreateResponse> AddEmployeeAsync(EmployeeCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет сотрудника по идентификатору.
    /// </summary>
    /// <param name="id">Id сотрудника.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>true, если удалён; иначе false.</returns>
    Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список сотрудников компании.
    /// </summary>
    /// <param name="companyId">Id компании.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список сотрудников.</returns>
    Task<IEnumerable<EmployeeResponse>> GetEmployeesByCompanyAsync(int companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список сотрудников отдела компании.
    /// </summary>
    /// <param name="companyId">Id компании.</param>
    /// <param name="departmentName">Название отдела.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список сотрудников.</returns>
    Task<IEnumerable<EmployeeResponse>> GetEmployeesByDepartmentAsync(int companyId, string departmentName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет данные сотрудника.
    /// </summary>
    /// <param name="id">Id сотрудника.</param>
    /// <param name="request">Данные для обновления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>true, если обновлён; иначе false.</returns>
    Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateRequest request,
        CancellationToken cancellationToken = default);
}