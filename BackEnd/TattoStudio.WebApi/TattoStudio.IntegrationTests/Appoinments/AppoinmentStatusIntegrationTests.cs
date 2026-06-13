using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TattoStudio.Application.DTOs.Appoinments;
using TattoStudio.Application.Mappings;
using TattoStudio.Domain.Entities;
using TattoStudio.Domain.Enums;
using TattoStudio.Domain.Exceptions;
using TattoStudio.Infraestructure.Persistence;
using TattoStudio.Infraestructure.Repositories.Appoinments;

namespace TattoStudio.IntegrationTests.Appoinments;

public class AppoinmentStatusIntegrationTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(
            typeof(AppoinmentProfile),
            typeof(AppoinmentAuditLogProfile)));
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    private static Appoinment BuildEntity(AppoinmentStatus status = AppoinmentStatus.Pending) => new()
    {
        Id = Guid.NewGuid(),
        ArtistId = Guid.NewGuid(),
        Name = "Test Client",
        MailClient = "client@test.com",
        AppoinmentDate = DateTime.UtcNow.AddDays(7),
        DurationMinutes = 60,
        Status = status,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task ChangeStatus_PendingToConfirmed_WritesAuditLog()
    {
        using var context = CreateContext();
        var entity = BuildEntity(AppoinmentStatus.Pending);
        context.Appoinments.Add(entity);
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());
        var changedBy = Guid.NewGuid();

        await repo.ChangeStatusAsync(entity.Id, AppoinmentStatus.Confirmed, null, changedBy, CancellationToken.None);

        var auditLogs = await context.AppoinmentAuditLogs
            .Where(l => l.AppoinmentId == entity.Id)
            .ToListAsync();

        auditLogs.Should().HaveCount(1);
        auditLogs[0].FieldName.Should().Be("Status");
        auditLogs[0].OldValue.Should().Be(AppoinmentStatus.Pending.ToString());
        auditLogs[0].NewValue.Should().Be(AppoinmentStatus.Confirmed.ToString());
        auditLogs[0].ChangedByUserId.Should().Be(changedBy);
    }

    [Fact]
    public async Task Update_AppoinmentDate_WritesAuditLog()
    {
        using var context = CreateContext();
        var entity = BuildEntity();
        var originalDate = entity.AppoinmentDate;
        context.Appoinments.Add(entity);
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());
        var changedBy = Guid.NewGuid();
        var newDate = originalDate.AddDays(3);

        await repo.UpdateAsync(entity.Id, new UpdateAppoinmentRequest { AppoinmentDate = newDate }, changedBy, CancellationToken.None);

        var auditLogs = await context.AppoinmentAuditLogs
            .Where(l => l.AppoinmentId == entity.Id && l.FieldName == "AppoinmentDate")
            .ToListAsync();

        auditLogs.Should().HaveCount(1);
        auditLogs[0].OldValue.Should().Be(originalDate.ToString("O"));
        auditLogs[0].NewValue.Should().Be(newDate.ToString("O"));
    }

    [Fact]
    public async Task Update_DurationMinutes_WritesAuditLog()
    {
        using var context = CreateContext();
        var entity = BuildEntity();
        entity.DurationMinutes = 60;
        context.Appoinments.Add(entity);
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());
        var changedBy = Guid.NewGuid();

        await repo.UpdateAsync(entity.Id, new UpdateAppoinmentRequest { DurationMinutes = 120 }, changedBy, CancellationToken.None);

        var auditLogs = await context.AppoinmentAuditLogs
            .Where(l => l.AppoinmentId == entity.Id && l.FieldName == "DurationMinutes")
            .ToListAsync();

        auditLogs.Should().HaveCount(1);
        auditLogs[0].OldValue.Should().Be("60");
        auditLogs[0].NewValue.Should().Be("120");
    }

    [Fact]
    public async Task GetAuditLog_ReturnsOrderedByDateDesc()
    {
        using var context = CreateContext();
        var entity = BuildEntity(AppoinmentStatus.Pending);
        context.Appoinments.Add(entity);
        var changedBy = Guid.NewGuid();
        var now = DateTime.UtcNow;
        context.AppoinmentAuditLogs.AddRange(
            new AppoinmentAuditLog { Id = Guid.NewGuid(), AppoinmentId = entity.Id, ChangedByUserId = changedBy, FieldName = "Status", OldValue = "Pending", NewValue = "Confirmed", ChangedAt = now.AddMinutes(-10) },
            new AppoinmentAuditLog { Id = Guid.NewGuid(), AppoinmentId = entity.Id, ChangedByUserId = changedBy, FieldName = "DurationMinutes", OldValue = "60", NewValue = "90", ChangedAt = now }
        );
        await context.SaveChangesAsync();

        var repo = new AppoinmentRepository(context, CreateMapper());
        var logs = (await repo.GetAuditLogAsync(entity.Id, CancellationToken.None)).ToList();

        logs.Should().HaveCount(2);
        logs[0].ChangedAt.Should().BeAfter(logs[1].ChangedAt);
    }

    [Fact]
    public async Task CreateAppoinment_DefaultStatusIsPending()
    {
        using var context = CreateContext();
        var repo = new AppoinmentRepository(context, CreateMapper());

        var result = await repo.CreateAsync(new CreateAppoinmentCommand
        {
            ArtistId = Guid.NewGuid(),
            Name = "New Client",
            MailClient = "new@test.com",
            AppoinmentDate = DateTime.UtcNow.AddDays(5),
            DurationMinutes = 90
        }, CancellationToken.None);

        result.Status.Should().Be(AppoinmentStatus.Pending);
    }
}
