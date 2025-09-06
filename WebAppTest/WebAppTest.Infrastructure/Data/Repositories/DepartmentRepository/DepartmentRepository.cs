using Npgsql;
using WebAppTest.Core.Domain;
using WebAppTest.Core.Interfaces.Repositories;

namespace WebAppTest.Infrastructure.Data.Repositories.DepartmentRepository;

/// <inheritdoc cref="IDepartmentRepository"/>
public class DepartmentRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    : BaseRepository<Department>(connection, transaction), IDepartmentRepository;