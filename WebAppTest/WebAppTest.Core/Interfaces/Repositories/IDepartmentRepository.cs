using WebAppTest.Core.Domain;

namespace WebAppTest.Core.Interfaces.Repositories;

/// <summary>
/// Представляет репозиторий для работы с сущностями Department (отделы).
/// Расширяет базовый репозиторий, предоставляя специфичные методы для управления отделами.
/// </summary>
public interface IDepartmentRepository : IRepository<Department>;