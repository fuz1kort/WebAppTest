using Dapper;
using Npgsql;
using WebAppTest.Core.Domain;
using WebAppTest.Core.Interfaces.Repositories;

namespace WebAppTest.Infrastructure.Data.Repositories.PassportRepository;

/// <inheritdoc cref="IPassportRepository"/>
public class PassportRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    : BaseRepository<Passport>(connection, transaction), IPassportRepository
{
    protected override string TableName => "\"Passports\"";

    public override async Task<int> CreateAsync(Passport entity, CancellationToken cancellationToken = default)
    {
        var sql =
            $"""INSERT INTO {TableName} ("Id", "Type", "Number") VALUES (@Id, @Type, @Number) RETURNING "Id" """;

        return await Connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, entity, Transaction, cancellationToken: cancellationToken));
    }
}