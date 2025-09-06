using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;

namespace WebAppTest.Infrastructure.Data.Migrations;

/// <summary>
/// Предоставляет функциональность для управления миграциями базы данных с использованием FluentMigrator.
/// Отвечает за применение миграций и откат изменений базы данных.
/// </summary>
/// <param name="migrationRunner">Сервис выполнения миграций FluentMigrator</param>
/// <param name="logger">Логгер для записи информации о процессе миграции</param>
public class DatabaseMigrator(IMigrationRunner migrationRunner, ILogger<DatabaseMigrator> logger)
{
    /// <summary>
    /// Применяет все незавершенные миграции к базе данных.
    /// Выполняет обновление схемы базы данных до последней версии.
    /// </summary>
    /// <exception cref="Exception">Выбрасывается при ошибке выполнения миграции</exception>
    public void Migrate()
    {
        try
        {
            migrationRunner.MigrateUp();
            logger.LogInformation("Database migrated successfully using FluentMigrator");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed");
            throw;
        }
    }

    /// <summary>
    /// Откатывает указанное количество последних миграций.
    /// Используется для отката изменений базы данных при необходимости.
    /// </summary>
    /// <param name="steps">Количество миграций для отката (по умолчанию 1)</param>
    /// <exception cref="Exception">Выбрасывается при ошибке выполнения отката</exception>
    public void Rollback(int steps = 1)
    {
        try
        {
            migrationRunner.Rollback(steps);
            logger.LogInformation("Database rollback completed successfully for {Steps} step(s)", steps);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database rollback failed");
            throw;
        }
    }
}