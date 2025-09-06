using Dapper;
using Npgsql;
using WebAppTest.Core.Domain;
using WebAppTest.Core.Interfaces.Repositories;

namespace WebAppTest.Infrastructure.Data.Repositories.EmployeeRepository;

/// <inheritdoc cref="IEmployeeRepository"/>
public class EmployeeRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    : BaseRepository<Employee>(connection, transaction), IEmployeeRepository
{
    private record EmployeeFlatDto(
        int Id,
        string Name,
        string Surname,
        string Phone,
        int CompanyId,
        string PassportType,
        string PassportNumber,
        string DepartmentName,
        string DepartmentPhone
    );

    public async Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId,
        CancellationToken cancellationToken = default)
        => await GetEmployeesAsync("""
                                       SELECT e."Id", e."Name", e."Surname", e."Phone", e."CompanyId",
                                              p."Type" AS "PassportType", p."Number" AS "PassportNumber",
                                              d."Name" AS "DepartmentName", d."Phone" AS "DepartmentPhone"
                                       FROM "Employees" e
                                       INNER JOIN "Passports" p ON e."Id" = p."Id"
                                       INNER JOIN "Departments" d ON e."DepartmentId" = d."Id"
                                       WHERE e."CompanyId" = @CompanyId;
                                   """, new { CompanyId = companyId }, cancellationToken);

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName,
        CancellationToken cancellationToken = default)
        => await GetEmployeesAsync("""
                                       SELECT e."Id", e."Name", e."Surname", e."Phone", e."CompanyId",
                                              p."Type" AS "PassportType", p."Number" AS "PassportNumber",
                                              d."Name" AS "DepartmentName", d."Phone" AS "DepartmentPhone"
                                       FROM "Employees" e
                                       INNER JOIN "Passports" p ON e."Id" = p."Id"
                                       INNER JOIN "Departments" d ON e."DepartmentId" = d."Id"
                                       WHERE e."CompanyId" = @CompanyId AND d."Name" = @DepartmentName;
                                   """, new { CompanyId = companyId, DepartmentName = departmentName },
            cancellationToken);

    private async Task<IEnumerable<Employee>> GetEmployeesAsync(string sql, object param,
        CancellationToken cancellationToken)
    {
        var result = await Connection.QueryAsync<EmployeeFlatDto>(
            new CommandDefinition(sql, param, Transaction, cancellationToken: cancellationToken));

        return result.Select(r => new Employee
        {
            Id = r.Id,
            Name = r.Name,
            Surname = r.Surname,
            Phone = r.Phone,
            CompanyId = r.CompanyId,
            Passport = new Passport { Type = r.PassportType, Number = r.PassportNumber },
            Department = new Department { Name = r.DepartmentName, Phone = r.DepartmentPhone }
        });
    }
}