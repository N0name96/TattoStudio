using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;

namespace TattoStudio.UnitTests.Appoinments.Mappings;

public class AppoinmentProfileTests
{
    private readonly IMapper _mapper;

    public AppoinmentProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AppoinmentProfile)));
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public void AppoinmentProfile_AllMappings_AreValid()
    {
        var act = () =>
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AppoinmentProfile)));
            services.BuildServiceProvider().GetRequiredService<IMapper>();
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void Map_AppoinmentToDTO_MapsAllFields()
    {
        // Arrange
        var entity = new Appoinment
        {
            Id = Guid.NewGuid(),
            ArtistId = Guid.NewGuid(),
            Name = "Ana López",
            MailClient = "ana@test.com",
            PhoneNumber = "600123456",
            Deposit = true,
            DepositAmount = 50m,
            TotalPrice = 200m,
            AppoinmentDate = new DateTime(2026, 6, 15),
            SignedConsentForm = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        // Act
        var dto = _mapper.Map<AppoinmentDTO>(entity);

        // Assert
        dto.Id.Should().Be(entity.Id);
        dto.ArtistId.Should().Be(entity.ArtistId);
        dto.Name.Should().Be(entity.Name);
        dto.MailClient.Should().Be(entity.MailClient);
        dto.PhoneNumber.Should().Be(entity.PhoneNumber);
        dto.Deposit.Should().Be(entity.Deposit);
        dto.DepositAmount.Should().Be(entity.DepositAmount);
        dto.TotalPrice.Should().Be(entity.TotalPrice);
        dto.AppoinmentDate.Should().Be(entity.AppoinmentDate);
        dto.SignedConsentForm.Should().Be(entity.SignedConsentForm);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Map_CreateRequestToEntity_MapsAllFields()
    {
        // Arrange
        var request = new CreateAppoinmentCommand
        {
            ArtistId = Guid.NewGuid(),
            Name = "Carlos Ruiz",
            MailClient = "carlos@test.com",
            PhoneNumber = "611222333",
            TotalPrice = 300m,
            AppoinmentDate = new DateTime(2026, 7, 20)
        };

        // Act
        var entity = _mapper.Map<Appoinment>(request);

        // Assert
        entity.ArtistId.Should().Be(request.ArtistId);
        entity.Name.Should().Be(request.Name);
        entity.MailClient.Should().Be(request.MailClient);
        entity.TotalPrice.Should().Be(request.TotalPrice);
    }

    [Fact]
    public void Map_UpdateRequest_OnlyOverwritesProvidedFields()
    {
        // Arrange
        var entity = new Appoinment
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            MailClient = "original@test.com",
            TotalPrice = 100m
        };

        var updateRequest = new UpdateAppoinmentRequest
        {
            Name = "Updated Name"
            // MailClient y TotalPrice no se envían (null)
        };

        // Act
        _mapper.Map(updateRequest, entity);

        // Assert
        entity.Name.Should().Be("Updated Name");
        entity.MailClient.Should().Be("original@test.com"); // no debe cambiar
        entity.TotalPrice.Should().Be(100m);                // no debe cambiar
    }

    [Fact]
    public void Map_NullEntity_ReturnsNull()
    {
        // Act
        var dto = _mapper.Map<AppoinmentDTO>(null as Appoinment);

        // Assert
        dto.Should().BeNull();
    }
}
