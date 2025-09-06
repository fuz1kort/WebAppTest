using Microsoft.AspNetCore.Mvc;
using WebAppTest.Core.Contracts.Requests;
using WebAppTest.Core.Exceptions;
using WebAppTest.Core.Interfaces.Services;

namespace WebAppTest.WebApi.Controllers;

/// <summary>
/// Контроллер для управления сотрудниками
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger) : ControllerBase
{
    /// <summary>
    /// Добавляет нового сотрудника
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] EmployeeCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await employeeService.AddEmployeeAsync(request, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Удаляет сотрудника по идентификатору
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id, CancellationToken cancellationToken = default)
    {
        if (id < 0)
        {
            return BadRequest();
        }

        try
        {
            await employeeService.DeleteEmployeeAsync(id, cancellationToken);

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Employee with id {Id} not found", id);

            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting employee with id {Id}", id);

            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получает список сотрудников для указанной компании
    /// </summary>
    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetEmployeesByCompany(int companyId, CancellationToken cancellationToken = default)
    {
        if (companyId < 0)
        {
            return BadRequest();
        }

        var response = await employeeService.GetEmployeesByCompanyAsync(companyId, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Получает список сотрудников для указанного отдела компании
    /// </summary>
    [HttpGet("company/{companyId:int}/department/{departmentName}")]
    public async Task<IActionResult> GetEmployeesByDepartment(int companyId, string departmentName,
        CancellationToken cancellationToken = default)
    {
        if (companyId < 0 || string.IsNullOrWhiteSpace(departmentName))
        {
            return BadRequest();
        }

        var employees =
            await employeeService.GetEmployeesByDepartmentAsync(companyId, departmentName, cancellationToken);

        return Ok(employees);
    }

    /// <summary>
    /// Обновляет информацию о сотруднике
    /// </summary>
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await employeeService.UpdateEmployeeAsync(id, request, cancellationToken);

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Employee with id {Id} not found", id);

            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating employee with id {Id}", id);

            return StatusCode(500, "Internal server error");
        }
    }
}