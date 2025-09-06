using System.Reflection;
using Dapper;
using Npgsql;
using WebAppTest.Core.Interfaces.Repositories;

namespace WebAppTest.Infrastructure.Data.Repositories;

/// <inheritdoc cref="IRepository{TEntity}"/>
public abstract class BaseRepository<TEntity>(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    : IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Подключение к базе данных PostgreSQL
    /// </summary>
    protected readonly NpgsqlConnection Connection = connection;

    /// <summary>
    /// Опциональная транзакция базы данных
    /// </summary>
    protected readonly NpgsqlTransaction? Transaction = transaction;

    /// <summary>
    /// Название таблицы в базе данных. По умолчанию формируется из имени типа сущности.
    /// Может быть переопределено в наследниках для указания конкретного имени таблицы.
    /// </summary>
    protected virtual string TableName => $"\"{typeof(TEntity).Name}s\"";

    /// <summary>
    /// Название колонки первичного ключа. По умолчанию "Id".
    /// Может быть переопределено в наследниках для указания другой колонки ключа.
    /// </summary>
    protected virtual string IdColumn => "\"Id\"";

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {TableName} WHERE {IdColumn} = @Id";

        return await Connection.QueryFirstOrDefaultAsync<TEntity>(
            new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: cancellationToken));
    }

    public virtual async Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.Name != "Id" && p.CanWrite && !IsNavigationProperty(p)).ToList();

        var columns = string.Join(", ", properties.Select(p => $"\"{p.Name}\""));
        var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        var sql = $"INSERT INTO {TableName} ({columns}) VALUES ({values}) RETURNING {IdColumn}";

        return await Connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, entity, Transaction, cancellationToken: cancellationToken));
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.Name != "Id" && p.CanWrite && !IsNavigationProperty(p));

        var setClause = string.Join(", ", properties.Select(p => $"\"{p.Name}\" = @{p.Name}"));

        var sql = $"UPDATE {TableName} SET {setClause} WHERE {IdColumn} = @Id";

        var affected = await Connection.ExecuteAsync(
            new CommandDefinition(sql, entity, Transaction, cancellationToken: cancellationToken));

        return affected > 0;
    }

    /// <summary>
    /// Проверяет, является ли свойство навигационным (ссылка на другую сущность).
    /// </summary>
    /// <param name="property">Свойство для проверки</param>
    /// <returns>true, если свойство является навигационным; иначе false</returns>
    private static bool IsNavigationProperty(PropertyInfo property)
        => property.PropertyType.IsClass && property.PropertyType != typeof(string);

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"DELETE FROM {TableName} WHERE {IdColumn} = @Id";
        var affected = await Connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: cancellationToken));

        return affected > 0;
    }

    public virtual async Task<TEntity?> GetByFieldAsync(string fieldName, object value,
        CancellationToken cancellationToken = default)
    {
        ValidateFieldName(fieldName);

        var sql = $"""SELECT * FROM {TableName} WHERE "{fieldName}" = @Value LIMIT 1""";

        return await Connection.QueryFirstOrDefaultAsync<TEntity>(
            new CommandDefinition(sql, new { Value = value }, Transaction, cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Проверяет допустимость имени поля для защиты от SQL injection
    /// </summary>
    protected virtual void ValidateFieldName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
            throw new ArgumentException("Field name cannot be empty", nameof(fieldName));

        if (!fieldName.All(c => char.IsLetterOrDigit(c) || c == '_'))
            throw new ArgumentException($"Invalid field name: {fieldName}", nameof(fieldName));
    }
}