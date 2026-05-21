using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Request.Data;
using Request.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Request.Requests.Jobs;

public sealed class BorrowExpirationHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<BorrowExpirationHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);
    private const int BatchSize = 100;

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<BorrowExpirationHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunOnce(stoppingToken);

        using var timer = new PeriodicTimer(Interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunOnce(stoppingToken);
        }
    }

    private async Task RunOnce(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IRequestCommandHandlerService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<RequestDbContext>>();
            var expiredBeforeUtc = GetBangkokTodayStartUtc();

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var completedCount = await service.CompleteExpiredBorrowRequests(
                expiredBeforeUtc,
                BatchSize,
                cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            if (completedCount > 0)
            {
                _logger.LogInformation("Completed {Count} expired borrow request(s).", completedCount);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to complete expired borrow requests.");
        }
    }

    private static DateTime GetBangkokTodayStartUtc()
    {
        var bangkokTodayStart = DateTime.UtcNow.AddHours(7).Date;
        return DateTime.SpecifyKind(bangkokTodayStart.AddHours(-7), DateTimeKind.Utc);
    }
}
