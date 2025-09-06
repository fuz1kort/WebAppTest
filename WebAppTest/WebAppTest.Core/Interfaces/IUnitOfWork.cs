using System.Data;
using WebAppTest.Core.Interfaces.Repositories;

namespace WebAppTest.Core.Interfaces;

/// <summary>
/// Единица работы для управления транзакциями и сохранением изменений
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Репозиторий для работы с сотрудниками
    /// </summary>
    IEmployeeRepository EmployeeRepository { get; }

    /// <summary>
    /// Репозиторий для работы с отделами
    /// </summary>
    IDepartmentRepository DepartmentRepository { get; }

    /// <summary>
    /// Репозиторий для работы с паспортами
    /// </summary>
    IPassportRepository PassportRepository { get; }

    /// <summary>
    /// Начинает новую транзакцию в базе данных
    /// </summary>
    /// <param name="isolationLevel">Уровень изоляции</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Асинхронная задача</returns>
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Snapshot,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет все изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество затронутых записей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Откатывает все изменения
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Асинхронная задача</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}