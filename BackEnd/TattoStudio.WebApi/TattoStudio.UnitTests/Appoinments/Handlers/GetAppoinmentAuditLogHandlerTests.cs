using FluentAssertions;
using Moq;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.DTOs.Artists;
using TattoStudio.Application.Interfaces;
using TattoStudio.Application.UsesCases.Queries.Appoinments;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;

namespace TattoStudio.UnitTests.Appoinments.Handlers;

public class GetAppoinmentAuditLogHandlerTests
{
    private readonly Mock<IAppoinmentRepository> _appoinmentRepoMock;
    private readonly Mock<IArtistRepository> _artistRepoMock;
    private readonly GetAppoinmentAuditLogHandler _handler;

    private static readonly Guid AppoinmentId = Guid.NewGuid();
    private static readonly Guid ArtistId = Guid.NewGuid();
    private static readonly Guid AssignedUserId = Guid.NewGuid();

    private static AppoinmentDTO BuildAppoinment() => new()
    {
        Id = AppoinmentId,
        ArtistId = ArtistId,
        Name = "Test",
        MailClient = "t@test.com"
    };

    private static ArtistDTO BuildArtist(Guid? userId = null) => new()
    {
        Id = ArtistId,
        UserId = userId ?? AssignedUserId
    };

    public GetAppoinmentAuditLogHandlerTests()
    {
        _appoinmentRepoMock = new Mock<IAppoinmentRepository>();
        _artistRepoMock = new Mock<IArtistRepository>();
        _handler = new GetAppoinmentAuditLogHandler(_appoinmentRepoMock.Object, _artistRepoMock.Object);
    }

    [Fact]
    public async Task Handle_AdminCanReadLog_ReturnsOrderedEntries()
    {
        var logs = new List<AppoinmentAuditLogDTO>
        {
            new() { Id = Guid.NewGuid(), AppoinmentId = AppoinmentId, FieldName = "Status", ChangedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), AppoinmentId = AppoinmentId, FieldName = "DurationMinutes", ChangedAt = DateTime.UtcNow.AddMinutes(-5) }
        };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildAppoinment());
        _appoinmentRepoMock.Setup(r => r.GetAuditLogAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync(logs);

        var query = new GetAppoinmentAuditLogQuery(AppoinmentId, Guid.NewGuid(), IsAdmin: true);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        _artistRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AssignedUserCanReadLog_ReturnsEntries()
    {
        var logs = new List<AppoinmentAuditLogDTO>
        {
            new() { Id = Guid.NewGuid(), AppoinmentId = AppoinmentId, FieldName = "Status", ChangedAt = DateTime.UtcNow }
        };

        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildAppoinment());
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist(AssignedUserId));
        _appoinmentRepoMock.Setup(r => r.GetAuditLogAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync(logs);

        var query = new GetAppoinmentAuditLogQuery(AppoinmentId, AssignedUserId, IsAdmin: false);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_EmptyLog_ReturnsEmptyList()
    {
        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildAppoinment());
        _appoinmentRepoMock.Setup(r => r.GetAuditLogAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var query = new GetAppoinmentAuditLogQuery(AppoinmentId, Guid.NewGuid(), IsAdmin: true);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_NonAssignedUser_ThrowsUnauthorizedException()
    {
        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildAppoinment());
        _artistRepoMock.Setup(r => r.GetByIdAsync(ArtistId, It.IsAny<CancellationToken>())).ReturnsAsync(BuildArtist(Guid.NewGuid())); // different user

        var query = new GetAppoinmentAuditLogQuery(AppoinmentId, Guid.NewGuid(), IsAdmin: false);
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentUnauthorizedStatusChangeException>();
        _appoinmentRepoMock.Verify(r => r.GetAuditLogAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AppoinmentNotFound_PropagatesNotFoundException()
    {
        _appoinmentRepoMock.Setup(r => r.GetByIdAsync(AppoinmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AppoinmentNotFoundException(AppoinmentId));

        var query = new GetAppoinmentAuditLogQuery(AppoinmentId, Guid.NewGuid(), IsAdmin: true);
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<AppoinmentNotFoundException>();
    }
}
