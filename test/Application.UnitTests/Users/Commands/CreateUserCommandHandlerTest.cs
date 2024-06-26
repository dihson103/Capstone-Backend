﻿using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.CreateUser;
using Contract.Services.User.Command;
using Contract.Services.User.CreateUser;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Users.Command;

public class CreateUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly IValidator<CreateUserRequest> _validator;

    public CreateUserCommandHandlerTest()
    {
        _userRepositoryMock = new();
        _unitOfWorkMock = new();
        _companyRepositoryMock = new();
        _passwordServiceMock = new();
        _validator = new CreateUserValidator(_userRepositoryMock.Object, _companyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handler_Should_Return_SuccessResult()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Phone: "0976099351",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword@123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDay: 150,
            Guid.NewGuid(),
            RoleId: 2
        );
        var command = new CreateUserCommand(createUserRequest, createUserRequest.Id);

        var createUserCommandHandler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(createUserRequest.Id))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _passwordServiceMock.Setup(service => service.Hash(createUserRequest.Password))
            .Returns("hashed_password");
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await createUserCommandHandler.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.isSuccess);
        _userRepositoryMock.Verify(user => user.AddUser(It.Is<User>(u => u.Id == createUserRequest.Id)), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_MyValidationException_WhenUserExsited()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Phone: "0976099351",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword@123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDay: 150,
            Guid.NewGuid(),
            RoleId: 2
        );
        var command = new CreateUserCommand(createUserRequest, createUserRequest.Id);

        var createUserCommandHandler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(createUserRequest.Id)).ReturnsAsync(true);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
                await createUserCommandHandler.Handle(command, default));
    }

    [Fact]
    public async Task Handler_Should_Throw_MyValidationException_WhenFactoryIdNotExsited()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Phone: "0976099351",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword@123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDay: 150,
            Guid.NewGuid(),
            RoleId: 2
        );
        var command = new CreateUserCommand(createUserRequest, createUserRequest.Id);

        var createUserCommandHandler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(createUserRequest.Id)).ReturnsAsync(false);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
                await createUserCommandHandler.Handle(command, default));
    }

    [Fact]
    public async Task Handler_Should_Throw_MyValidationException_WhenPhoneNumberExsited()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Phone: "0976099351",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword@123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDay: 150,
            Guid.NewGuid(),
            RoleId: 2
        );
        var command = new CreateUserCommand(createUserRequest, createUserRequest.Id);

        var createUserCommandHandler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(createUserRequest.Id)).ReturnsAsync(false);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
                await createUserCommandHandler.Handle(command, default));
    }

    [Theory]
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", 150, 2)] //firstName contains number or specification character is not valid
    [InlineData("001201011091", "", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", 150, 2)] //firstName empty is not valid
    [InlineData("001201011091", "John", "Doe1112", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", 150, 2)] //lastName contains number or specification character is not valid
    [InlineData("001201011091", "John", "", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", 150, 2)] //lastName empty is not valid
    [InlineData("001201011091", "John", "Doe", "0sdfsd976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", 150, 2)] //phone contains characters is not valid
    [InlineData("001201011091", "John", "Doe", "", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", 150, 2)] //phone is empty is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10/03/2001", -150, 2)] //salary less than 0 is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "", "10/03/2001", 150, 2)] //gender empty is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "dfd", "10/03/2001", 150, 2)] //gender difference "Male" or "Female" is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "", 150, 2)] //date empty is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "SecurePassword123", "Male", "10-03-2001", 150, 2)] //date wrong format dd/MM/yyyy is not valid
    public async Task Handler_Should_Throw_MyValidationException_WhenInputNotValid(
        string id, string firstName, string lastName, string phone, string address, 
        string password, string gender, string dob, int salaryByDay, int roleId)
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(
            Id: id,
            FirstName: firstName,
            LastName: lastName,
            Phone: phone,
            Address: address,
            Password: password,
            Gender: gender,
            DOB: dob,
            SalaryByDay: salaryByDay,
            Guid.NewGuid(),
            RoleId: roleId
        );
        var command = new CreateUserCommand(createUserRequest, createUserRequest.Id);

        var createUserCommandHandler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(createUserRequest.Id)).ReturnsAsync(false);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
                await createUserCommandHandler.Handle(command, default));
    }

}
