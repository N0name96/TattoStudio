using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Commands.Appoinments;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.Appoinments.Handlers;

public class ChangeAppoinmentStatusHandlerTests
{
    private readonly Mock<IAppoinmentRepository> _appoinmentRepoMock;
    private readonly Mock<IArtistRepository> _artistRepoMock;
    private readonly ChangeAppoinmentStatusHandler _handler;

    private static readonly Guid AppoinmentId = Guid.NewGuid();
    private static readonly Guid ArtistId = Guid.NewGuid();
    private static readonly Guid AssignedUserId = Guid.NewGuid();

    private static AppoinmentDTO BuildAppoinment(AppoinmentStatus status = AppoinmentStatus.Pending) => new()
    {
        Id = AppoinmentId,
        ArtistId = ArtistId,
        Name = "Test Client",
        MailClient = "client@test.com",
        Status = status
    };

    private static ArtistDTO BuildArtist(Guid? userId = null) => new()
    {
        Id = ArtistId,
        UserId = userId ?? AssignedUserId
    };

    public ChangeAppoinmentStatusHandlerTests()
    {
        _appoinmentRepoMock = new Mock<IAppoinmentRepository>();
        _artistRepoMock = new Mock<IArtistRepository>();
        _handler = new ChangeAppoinmentStatusHandler(_appoinmentRepoMock.Object, _artistRepoMock.Object);
    }

    [Fact]
    public async Task Handle_AdminConfirmsPendingAppoinment_ReturnsConfirmedDTO()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Pending);
        var expected = appoinment with { Status = AppoinmentStatus.Confirmed };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, AppoinmentStatus.Confirmed, null, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Confirmed, null, Guid.NewGuid(), IsAdmin: true);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Status.Should().Be(AppoinmentStatus.Confirmed);
        _artistRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AssignedUserConfirmsPendingAppoinment_ReturnsConfirmedDTO()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Pending);
        var artist = BuildArtist(AssignedUserId);
        var expected = appoinment with { Status = AppoinmentStatus.Confirmed };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artist);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, AppoinmentStatus.Confirmed, null, AssignedUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Confirmed, null, AssignedUserId, IsAdmin: false);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Status.Should().Be(AppoinmentStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_AdminCompletesConfirmedAppoinment_ReturnsCompletedDTO()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Confirmed);
        var expected = appoinment with { Status = AppoinmentStatus.Completed };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, AppoinmentStatus.Completed, null, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Completed, null, Guid.NewGuid(), IsAdmin: true);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Status.Should().Be(AppoinmentStatus.Completed);
    }

    [Theory]
    [InlineData(AppoinmentStatus.Pending,   AppoinmentStatus.Confirmed)]
    [InlineData(AppoinmentStatus.Pending,   AppoinmentStatus.Cancelled)]
    [InlineData(AppoinmentStatus.Confirmed, AppoinmentStatus.Completed)]
    [InlineData(AppoinmentStatus.Confirmed, AppoinmentStatus.Cancelled)]
    public async Task Handle_AllValidTransitions_DelegateToRepository(AppoinmentStatus from, AppoinmentStatus to)
    {
        var appoinment = BuildAppoinment(from);
        var expected = appoinment with { Status = to };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, to, null, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, to, null, Guid.NewGuid(), IsAdmin: true);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Status.Should().Be(to);
    }

    [Fact]
    public async Task Handle_InvalidTransitionPendingToCompleted_PropagatesRepositoryException()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Pending);

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, AppoinmentStatus.Completed, null, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AppoinmentInvalidStatusTransitionException(AppoinmentStatus.Pending, AppoinmentStatus.Completed));

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Completed, null, Guid.NewGuid(), IsAdmin: true);
        var act = async () => await _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentInvalidStatusTransitionException>();
    }

    [Fact]
    public async Task Handle_TransitionFromCompletedState_PropagatesAlreadyFinalizedException()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Completed);

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, It.IsAny<AppoinmentStatus>(), null, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AppoinmentAlreadyFinalizedException(AppoinmentId, AppoinmentStatus.Completed));

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Cancelled, null, Guid.NewGuid(), IsAdmin: true);
        var act = async () => await _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentAlreadyFinalizedException>();
    }

    [Fact]
    public async Task Handle_TransitionFromCancelledState_PropagatesAlreadyFinalizedException()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Cancelled);

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, It.IsAny<AppoinmentStatus>(), null, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AppoinmentAlreadyFinalizedException(AppoinmentId, AppoinmentStatus.Cancelled));

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Confirmed, null, Guid.NewGuid(), IsAdmin: true);
        var act = async () => await _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentAlreadyFinalizedException>();
    }

    [Fact]
    public async Task Handle_NonAssignedUser_ThrowsUnauthorizedException()
    {
        var appoinment = BuildAppoinment(AppoinmentStatus.Pending);
        var artist = BuildArtist(Guid.NewGuid()); // different UserId

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artist);

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Confirmed, null, Guid.NewGuid(), IsAdmin: false);
        var act = async () => await _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentUnauthorizedStatusChangeException>();
        _appoinmentRepoMock.Verify(r => r.ChangeStatusAsync(It.IsAny<Guid>(), It.IsAny<AppoinmentStatus>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AppoinmentNotFound_PropagatesNotFoundException()
    {
        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AppoinmentNotFoundException(AppoinmentId));

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Confirmed, null, Guid.NewGuid(), IsAdmin: true);
        var act = async () => await _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentNotFoundException>();
    }

    [Fact]
    public async Task Handle_CancellationWithReason_PassesReasonToRepository()
    {
        const string reason = "Client cancelled due to illness";
        var appoinment = BuildAppoinment(AppoinmentStatus.Confirmed);
        var expected = appoinment with { Status = AppoinmentStatus.Cancelled, CancellationReason = reason };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appoinment);
        _appoinmentRepoMock.Setup(r => r.ChangeStatusAsync(AppoinmentId, AppoinmentStatus.Cancelled, reason, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cmd = new ChangeAppoinmentStatusCommand(AppoinmentId, AppoinmentStatus.Cancelled, reason, Guid.NewGuid(), IsAdmin: true);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.CancellationReason.Should().Be(reason);
        _appoinmentRepoMock.Verify(r => r.ChangeStatusAsync(AppoinmentId, AppoinmentStatus.Cancelled, reason, It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
