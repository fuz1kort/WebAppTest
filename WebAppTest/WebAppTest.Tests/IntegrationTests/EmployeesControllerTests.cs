using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebAppTest.Core.Contracts.Requests;
using WebAppTest.Core.Contracts.Responses;
using WebAppTest.Core.Exceptions;
using WebAppTest.Core.Interfaces.Services;
using WebAppTest.WebApi.Controllers;

namespace WebAppTest.Tests.IntegrationTests;

[TestFixture]
public class EmployeesControllerTests
{
    private Mock<IEmployeeService> _employeeServiceMock;
    private Mock<ILogger<EmployeesController>> _loggerMock;
    private EmployeesController _controller;

    [SetUp]
    public void Setup()
    {
        _employeeServiceMock = new Mock<IEmployeeService>();
        _loggerMock = new Mock<ILogger<EmployeesController>>();
        _controller = new EmployeesController(_employeeServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task AddEmployee_ValidRequest_ReturnsOkWithId()
    {
        var request = new EmployeeCreateRequest
        {
            Name = "John",
            Surname = "Doe",
            Phone = "1234567890",
            CompanyId = 1,
            PassportCreate = new PassportCreateRequest { Type = "Internal", Number = "AB123456" },
            DepartmentCreate = new DepartmentCreateRequest { Name = "IT", Phone = "0987654321" }
        };

        var serviceResponse = new EmployeeCreateResponse { Id = 123 };
        _employeeServiceMock.Setup(x => x.AddEmployeeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var result = await _controller.AddEmployee(request, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<EmployeeCreateResponse>());
        var response = okResult.Value as EmployeeCreateResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.EqualTo(123));
    }

    [Test]
    public async Task DeleteEmployee_ExistingId_ReturnsOk()
    {
        _employeeServiceMock.Setup(x => x.DeleteEmployeeAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteEmployee(1, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        var okResult = result as NoContentResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() => { Assert.That(okResult.StatusCode, Is.EqualTo(204)); });
    }

    [Test]
    public async Task DeleteEmployee_NonExistingId_ReturnsNotFound()
    {
        _employeeServiceMock.Setup(x => x.DeleteEmployeeAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Employee not found"));

        var result = await _controller.DeleteEmployee(999, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("Employee not found"));
        });
    }

    [Test]
    public async Task GetEmployeesByCompany_ValidCompanyId_ReturnsOkWithEmployees()
    {
        var expectedResponse = new List<EmployeeResponse>
        {
            new() { Id = 1, Name = "John", Surname = "Doe" },
            new() { Id = 2, Name = "Jane", Surname = "Smith" }
        };

        _employeeServiceMock.Setup(x => x.GetEmployeesByCompanyAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetEmployeesByCompany(1, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<List<EmployeeResponse>>());
        var response = okResult.Value as List<EmployeeResponse>;
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetEmployeesByDepartment_ValidParameters_ReturnsOkWithEmployees()
    {
        var expectedResponse = new List<EmployeeResponse>
        {
            new() { Id = 1, Name = "John", Surname = "Doe" }
        };

        _employeeServiceMock.Setup(x => x.GetEmployeesByDepartmentAsync(1, "IT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetEmployeesByDepartment(1, "IT", CancellationToken.None);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<List<EmployeeResponse>>());
        var response = okResult.Value as List<EmployeeResponse>;
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task UpdateEmployee_ExistingId_ReturnsOk()
    {
        var request = new EmployeeUpdateRequest
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        };

        _employeeServiceMock.Setup(x => x.UpdateEmployeeAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        var result = await _controller.UpdateEmployee(1, request, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        var okResult = result as NoContentResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() => { Assert.That(okResult.StatusCode, Is.EqualTo(204)); });
    }

    [Test]
    public async Task UpdateEmployee_NonExistingId_ReturnsNotFound()
    {
        var request = new EmployeeUpdateRequest { Name = "UpdatedName" };

        _employeeServiceMock.Setup(x => x.UpdateEmployeeAsync(999, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Employee not found"));

        var result = await _controller.UpdateEmployee(999, request, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.Not.Null);
        });
    }

    [Test]
    public Task AddEmployee_ServiceThrowsException_ThrowsException()
    {
        var request = new EmployeeCreateRequest();
        _employeeServiceMock.Setup(x => x.AddEmployeeAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _controller.AddEmployee(request, CancellationToken.None));

        return Task.CompletedTask;
    }

    [Test]
    public async Task UpdateEmployee_BigId_ReturnsNotFound()
    {
        var request = new EmployeeUpdateRequest { Name = "Test" };
        _employeeServiceMock.Setup(x => x.UpdateEmployeeAsync(1000, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Employee not found"));

        var result = await _controller.UpdateEmployee(1000, request, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetEmployeesByCompany_NoEmployees_ReturnsEmptyList()
    {
        _employeeServiceMock.Setup(x => x.GetEmployeesByCompanyAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EmployeeResponse>());

        var result = await _controller.GetEmployeesByCompany(999, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var response = okResult.Value as List<EmployeeResponse>;
        Assert.That(response, Is.Empty);
    }
}