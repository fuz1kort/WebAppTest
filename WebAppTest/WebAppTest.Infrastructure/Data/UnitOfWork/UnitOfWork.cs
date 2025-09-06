using System.Data;
using Npgsql;
using WebAppTest.Core.Interfaces;
using WebAppTest.Core.Interfaces.Repositories;
using WebAppTest.Infrastructure.Data.Repositories.DepartmentRepository;
using WebAppTest.Infrastructure.Data.Repositories.EmployeeRepository;
using WebAppTest.Infrastructure.Data.Repositories.PassportRepository;

namespace WebAppTest.Infrastructure.Data.UnitOfWork;

/// <inheritdoc cref="IUnitOfWork"/>
public class UnitOfWork : IUnitOfWork
{
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _transaction;
    private IEmployeeRepository? _employeeRepository;
    private IDepartmentRepository? _departmentRepository;
    private IPassportRepository? _passportRepository;
    private bool _disposed;

    public UnitOfWork(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public IEmployeeRepository EmployeeRepository
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
            return _employeeRepository ??= new EmployeeRepository(_connection, _transaction);
        }
    }

    public IDepartmentRepository DepartmentRepository
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
            return _departmentRepository ??= new DepartmentRepository(_connection, _transaction);
        }
    }

    public IPassportRepository PassportRepository
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
            return _passportRepository ??= new PassportRepository(_connection, _transaction);
        }
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Snapshot,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
        _transaction ??= await _connection.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));

        if (_transaction is null)
            return 0;

        try
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;

            return 1;
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null && !_disposed)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
        }

        await _connection.DisposeAsync();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}