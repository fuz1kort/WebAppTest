using Microsoft.Extensions.Logging;
using WebAppTest.Core.Contracts.Requests;
using WebAppTest.Core.Contracts.Responses;
using WebAppTest.Core.Domain;
using WebAppTest.Core.Exceptions;
using WebAppTest.Core.Interfaces;
using WebAppTest.Core.Interfaces.Services;

namespace WebAppTest.Application.Services.EmployeeService;

/// <inheritdoc cref="IEmployeeService"/>
public class EmployeeService(IUnitOfWork unitOfWork, ILogger<EmployeeService> logger)
    : IEmployeeService
{
    public async Task<EmployeeCreateResponse> AddEmployeeAsync(EmployeeCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding a new employee: {Name} {Surname}", request.Name, request.Surname);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var department = await unitOfWork.DepartmentRepository.GetByFieldAsync("Name",
                request.DepartmentCreate.Name,
                cancellationToken);

            int departmentId;
            if (department is null)
            {
                departmentId = await unitOfWork.DepartmentRepository.CreateAsync(new Department
                {
                    Name = request.DepartmentCreate.Name,
                    Phone = request.DepartmentCreate.Phone
                }, cancellationToken);
            }
            else
            {
                departmentId = department.Id;
            }

            var employeeId = await unitOfWork.EmployeeRepository.CreateAsync(new Employee
            {
                Name = request.Name,
                Surname = request.Surname,
                Phone = request.Phone,
                CompanyId = request.CompanyId,
                DepartmentId = departmentId
            }, cancellationToken);

            await unitOfWork.PassportRepository.CreateAsync(new Passport
            {
                Id = employeeId,
                Type = request.PassportCreate.Type,
                Number = request.PassportCreate.Number
            }, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Employee added with Id: {Id}", employeeId);

            return new EmployeeCreateResponse { Id = employeeId };
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating employee with Id: {Id}", id);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var existingEmployee = await unitOfWork.EmployeeRepository.GetByIdAsync(id, cancellationToken);
            if (existingEmployee is null)
            {
                logger.LogError("Employee with Id {Id} not found", id);
                throw new NotFoundException($"Employee with id {id} not found");
            }

            if (request.Name != null) existingEmployee.Name = request.Name;
            if (request.Surname != null) existingEmployee.Surname = request.Surname;
            if (request.Phone != null) existingEmployee.Phone = request.Phone;
            if (request.CompanyId.HasValue) existingEmployee.CompanyId = request.CompanyId.Value;

            if (request.Department != null)
            {
                Department? department = null;

                if (!string.IsNullOrEmpty(request.Department.Name))
                {
                    department = await unitOfWork.DepartmentRepository.GetByFieldAsync("Name",
                        request.Department.Name, cancellationToken);
                }

                if (department is null && !string.IsNullOrEmpty(request.Department.Phone))
                {
                    department = await unitOfWork.DepartmentRepository.GetByFieldAsync("Phone",
                        request.Department.Phone, cancellationToken);
                }

                if (department is null)
                {
                    existingEmployee.DepartmentId = await unitOfWork.DepartmentRepository.CreateAsync(new Department
                    {
                        Name = request.Department.Name ?? string.Empty,
                        Phone = request.Department.Phone
                    }, cancellationToken);
                }
                else
                {
                    existingEmployee.DepartmentId = department.Id;

                    if (!string.IsNullOrEmpty(request.Department.Name) && department.Name != request.Department.Name ||
                        !string.IsNullOrEmpty(request.Department.Phone) && department.Phone != request.Department.Phone)
                    {
                        department.Name = request.Department.Name ?? department.Name;
                        department.Phone = request.Department.Phone ?? department.Phone;
                        await unitOfWork.DepartmentRepository.UpdateAsync(department, cancellationToken);
                    }
                }
            }

            var employeeUpdated = await unitOfWork.EmployeeRepository.UpdateAsync(existingEmployee, cancellationToken);

            if (request.Passport != null)
            {
                var existingPassport = await unitOfWork.PassportRepository.GetByIdAsync(id, cancellationToken);

                if (existingPassport is null)
                {
                    logger.LogError("Passport for employee with Id {Id} not found. Data integrity error!", id);
                    throw new NotFoundException($"Passport for employee with id {id} not found");
                }

                if (request.Passport.Type != null) existingPassport.Type = request.Passport.Type;
                if (request.Passport.Number != null) existingPassport.Number = request.Passport.Number;

                await unitOfWork.PassportRepository.UpdateAsync(existingPassport, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Employee update result for Id {Id}: {Result}", id, employeeUpdated);

            return employeeUpdated;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting employee with Id: {Id}", id);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var existingEmployee = await unitOfWork.EmployeeRepository.GetByIdAsync(id, cancellationToken);
            if (existingEmployee is null)
            {
                logger.LogError("Employee with Id {Id} not found for deletion", id);
                throw new NotFoundException($"Employee with id {id} not found");
            }

            var result = await unitOfWork.EmployeeRepository.DeleteAsync(id, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IEnumerable<EmployeeResponse>> GetEmployeesByCompanyAsync(int companyId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting employees for company Id: {CompanyId}", companyId);
        var employees = await unitOfWork.EmployeeRepository.GetEmployeesByCompanyAsync(companyId, cancellationToken);

        return employees.Select(MapToResponse);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetEmployeesByDepartmentAsync(int companyId, string departmentName,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting employees for company Id: {CompanyId}, department: {Department}",
            companyId, departmentName);
        var employees = await unitOfWork.EmployeeRepository.GetEmployeesByDepartmentAsync(
            companyId, departmentName, cancellationToken);

        return employees.Select(MapToResponse);
    }

    /// <summary>
    /// Маппит сущность Employee в ответ EmployeeResponse
    /// </summary>
    private static EmployeeResponse MapToResponse(Employee e) =>
        new()
        {
            Id = e.Id,
            Name = e.Name,
            Surname = e.Surname,
            Phone = e.Phone,
            CompanyId = e.CompanyId,
            Passport = new PassportResponse
            {
                Type = e.Passport.Type,
                Number = e.Passport.Number
            },
            Department = new DepartmentResponse
            {
                Name = e.Department.Name,
                Phone = e.Department.Phone
            }
        };
}