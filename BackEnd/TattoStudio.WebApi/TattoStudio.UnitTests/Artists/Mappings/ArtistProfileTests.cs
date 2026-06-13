using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;

namespace TattoStudio.UnitTests.Artists.Mappings;

public class ArtistProfileTests
{
    private readonly IMapper _mapper;

    public ArtistProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ArtistProfile)));
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public void ArtistProfile_AllMappings_AreValid()
    {
        var act = () =>
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ArtistProfile)));
            services.BuildServiceProvider().GetRequiredService<IMapper>();
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void Map_ArtistToDTO_MapsAllFields()
    {
        var entity = new Artist
        {
            Id = Guid.NewGuid(),
            Name = "Laura",
            Surname = "Gómez",
            Mail = "laura@tattostudio.com",
            PhoneNumber = "600111222",
            Comision = 15.50m
        };

        var dto = _mapper.Map<ArtistDTO>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Name.Should().Be(entity.Name);
        dto.Surname.Should().Be(entity.Surname);
        dto.Mail.Should().Be(entity.Mail);
        dto.PhoneNumber.Should().Be(entity.PhoneNumber);
        dto.Comision.Should().Be(entity.Comision);
    }

    [Fact]
    public void Map_CreateRequestToEntity_MapsAllFields()
    {
        var command = new CreateArtistCommand
        {
            Name = "Laura",
            Surname = "Gómez",
            Mail = "laura@tattostudio.com",
            PhoneNumber = "600111222",
            Comision = 15.50m
        };

        var entity = _mapper.Map<Artist>(command);

        entity.Name.Should().Be(command.Name);
        entity.Surname.Should().Be(command.Surname);
        entity.Mail.Should().Be(command.Mail);
        entity.PhoneNumber.Should().Be(command.PhoneNumber);
        entity.Comision.Should().Be(command.Comision);
    }

    [Fact]
    public void Map_UpdateRequest_OnlyOverwritesProvidedFields()
    {
        var entity = new Artist
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            Surname = "Surname",
            Mail = "original@test.com",
            Comision = 10m
        };

        var updateRequest = new UpdateArtistRequest { Name = "Updated" };

        _mapper.Map(updateRequest, entity);

        entity.Name.Should().Be("Updated");
        entity.Surname.Should().Be("Surname");
        entity.Mail.Should().Be("original@test.com");
        entity.Comision.Should().Be(10m);
    }

    [Fact]
    public void Map_NullEntity_ReturnsNull()
    {
        var dto = _mapper.Map<ArtistDTO>(null as Artist);

        dto.Should().BeNull();
    }
}
