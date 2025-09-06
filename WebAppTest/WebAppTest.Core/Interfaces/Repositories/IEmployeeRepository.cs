using WebAppTest.Core.Domain;

namespace WebAppTest.Core.Interfaces.Repositories;

/// <summary>
/// Репозиторий для работы с сотрудниками.
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// Получает список сотрудников компании.
    /// </summary>
    /// <param name="companyId">Id компании.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список сотрудников.</returns>
    Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список сотрудников отдела компании.
    /// </summary>
    /// <param name="companyId">Id компании.</param>
    /// <param name="departmentName">Название отдела.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список сотрудников.</returns>
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName,
        CancellationToken cancellationToken = default);
}