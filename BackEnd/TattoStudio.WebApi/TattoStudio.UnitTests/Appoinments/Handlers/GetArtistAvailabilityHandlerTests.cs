using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.DTOs.StudioSettings;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Queries.Appoinments;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.Appoinments.Handlers;

public class GetArtistAvailabilityHandlerTests
{
    private readonly Mock<IArtistRepository> _artistRepoMock;
    private readonly Mock<IAppoinmentRepository> _appoinmentRepoMock;
    private readonly Mock<IStudioSettingsRepository> _settingsRepoMock;
    private readonly GetArtistAvailabilityHandler _handler;

    private static readonly Guid ArtistId = Guid.NewGuid();
    private static readonly DateOnly TestDate = new(2026, 7, 15);

    private static ArtistDTO BuildArtist() => new() { Id = ArtistId, Name = "Test Artist" };

    private static AppoinmentDTO BuildAppoinment(int hour, int durationMinutes, AppoinmentStatus status = AppoinmentStatus.Pending)
    {
        var date = TestDate.ToDateTime(new TimeOnly(hour, 0), DateTimeKind.Utc);
        return new AppoinmentDTO
        {
            Id = Guid.NewGuid(),
            ArtistId = ArtistId,
            AppoinmentDate = date,
            DurationMinutes = durationMinutes,
            Status = status
        };
    }

    public GetArtistAvailabilityHandlerTests()
    {
        _artistRepoMock = new Mock<IArtistRepository>();
        _appoinmentRepoMock = new Mock<IAppoinmentRepository>();
        _settingsRepoMock = new Mock<IStudioSettingsRepository>();
        _handler = new GetArtistAvailabilityHandler(_artistRepoMock.Object, _appoinmentRepoMock.Object, _settingsRepoMock.Object);
    }

    [Fact]
    public async Task Handle_NoAppointments_NoSettings_ReturnsFullDay()
    {
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync((StudioSettingsDTO?)null);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(1);
        result.AvailableSlots.First().Start.Should().Be(new TimeOnly(0, 0));
        result.AvailableSlots.First().End.Should().Be(new TimeOnly(23, 59));
    }

    [Fact]
    public async Task Handle_OneAppointmentInMiddle_ReturnsTwoSlots()
    {
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync([BuildAppoinment(10, 120)]); // 10:00-12:00
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync((StudioSettingsDTO?)null);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(2);
        result.AvailableSlots.First().Start.Should().Be(new TimeOnly(0, 0));
        result.AvailableSlots.First().End.Should().Be(new TimeOnly(10, 0));
        result.AvailableSlots.Last().Start.Should().Be(new TimeOnly(12, 0));
        result.AvailableSlots.Last().End.Should().Be(new TimeOnly(23, 59));
    }

    [Fact]
    public async Task Handle_MultipleAppointments_ReturnsGapsBetween()
    {
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                BuildAppoinment(9,  60),  // 09:00-10:00
                BuildAppoinment(11, 120)  // 11:00-13:00
            ]);
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync((StudioSettingsDTO?)null);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        // gaps: 00:00-09:00, 10:00-11:00, 13:00-23:59
        result.AvailableSlots.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithSettings_SlotsClampedToWorkday()
    {
        var settings = new StudioSettingsDTO { WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) };
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync([BuildAppoinment(12, 60)]); // 12:00-13:00
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        result.StudioStart.Should().Be(new TimeOnly(9, 0));
        result.StudioEnd.Should().Be(new TimeOnly(18, 0));
        result.AvailableSlots.Should().HaveCount(2);
        result.AvailableSlots.First().Start.Should().Be(new TimeOnly(9, 0));
        result.AvailableSlots.First().End.Should().Be(new TimeOnly(12, 0));
        result.AvailableSlots.Last().Start.Should().Be(new TimeOnly(13, 0));
        result.AvailableSlots.Last().End.Should().Be(new TimeOnly(18, 0));
    }

    [Fact]
    public async Task Handle_NoAppointmentsWithSettings_ReturnsFullWorkday()
    {
        var settings = new StudioSettingsDTO { WorkdayStart = new TimeOnly(9, 0), WorkdayEnd = new TimeOnly(18, 0) };
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(1);
        result.AvailableSlots.Single().Start.Should().Be(new TimeOnly(9, 0));
        result.AvailableSlots.Single().End.Should().Be(new TimeOnly(18, 0));
    }

    [Fact]
    public async Task Handle_CompletedAndCancelledAppointments_AreNotReturnedByRepository()
    {
        // The repository filters to only Pending+Confirmed — this test verifies the handler
        // works correctly when the repo returns an empty list (completed/cancelled already filtered)
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync((StudioSettingsDTO?)null);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(1);
        _appoinmentRepoMock.Verify(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PendingAppointmentsBlock_SlotsAreReduced()
    {
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist());
        _appoinmentRepoMock.Setup(r => r.GetByArtistAndDateAsync(ArtistId, TestDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync([BuildAppoinment(10, 60, AppoinmentStatus.Pending)]); // Pending blocks 10:00-11:00
        _settingsRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync((StudioSettingsDTO?)null);

        var result = await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(2);
        result.AvailableSlots.Should().NotContain(s => s.Start == new TimeOnly(10, 0) && s.End == new TimeOnly(11, 0));
    }

    [Fact]
    public async Task Handle_ArtistNotFound_PropagatesArtistNotFoundException()
    {
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArtistNotFoundException(ArtistId));

        var act = async () => await _handler.Handle(new GetArtistAvailabilityQuery(ArtistId, TestDate), CancellationToken.None);

        await act.Should().ThrowAsync<ArtistNotFoundException>();
        _appoinmentRepoMock.Verify(r => r.GetByArtistAndDateAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
