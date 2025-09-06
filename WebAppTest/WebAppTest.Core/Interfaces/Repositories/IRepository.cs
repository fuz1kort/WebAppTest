namespace WebAppTest.Core.Interfaces.Repositories;

/// <summary>
/// Представляет универсальный репозиторий для выполнения базовых CRUD-операций над сущностями.
/// Предоставляет стандартный набор методов для работы с данными в хранилище.
/// </summary>
/// <typeparam name="TEntity">Тип сущности, должен быть ссылочным типом</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Асинхронно получает сущность по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности для поиска</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <returns>Найденная сущность или null, если сущность не найдена</returns>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно создает новую сущность в хранилище данных.
    /// </summary>
    /// <param name="entity">Сущность для создания</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <returns>Идентификатор созданной сущности</returns>
    Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно обновляет существующую сущность в хранилище данных.
    /// </summary>
    /// <param name="entity">Сущность с обновленными данными</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <returns>true, если обновление выполнено успешно; false, если сущность не найдена</returns>
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Асинхронно удаляет сущность из хранилища данных по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности для удаления</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <returns>true, если удаление выполнено успешно; false, если сущность не найдена</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Универсальный метод для поиска сущности по указанному полю
    /// </summary>
    /// <param name="fieldName">Название поля для поиска</param>
    /// <param name="value">Значение поля для поиска</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденная сущность или null если не найдена</returns>
    Task<TEntity?> GetByFieldAsync(string fieldName, object value, CancellationToken cancellationToken = default);
}