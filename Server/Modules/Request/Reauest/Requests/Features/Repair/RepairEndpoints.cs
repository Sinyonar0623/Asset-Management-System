using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Reauest.Data.Repository;
using Reauest.Requests.Model;
using System.Security.Claims;

namespace Reauest.Requests.Features.Repair;

public sealed class RepairEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/repair-requests").RequireAuthorization();

        group.MapGet("/", async (
            IRequestRepository repository,
            CancellationToken cancellationToken,
            ClaimsPrincipal user,
            int page = 1,
            int pageSize = 20,
            string? status = null,
            bool myOnly = false) =>
        {
            Guid? requesterId = null;
            var roleCode = user.FindFirstValue("role_code");
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (roleCode == "03" || myOnly)
            {
                if (Guid.TryParse(userIdStr, out var uid))
                    requesterId = uid;
            }

            var (items, total) = await repository.GetRepairsPagedAsync(page, pageSize, requesterId, status, cancellationToken);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            var result = items.Select(r => new
            {
                r.Id,
                r.RequesterId,
                r.RequesterName,
                r.AssetId,
                r.AssetName,
                r.AssetRealWorldId,
                r.ProblemDescription,
                r.Status,
                r.ApprovalRemark,
                r.ApprovedBy,
                r.ApprovedAt,
                r.CompletedAt,
                r.TechnicianNote,
                r.CreateOn
            });

            return Results.Ok(new { Items = result, TotalCount = total, Page = page, PageSize = pageSize, TotalPages = totalPages });
        })
        .WithName("GetRepairRequests")
        .WithTags("RepairRequests");

        group.MapGet("/{id:long}", async (long id, IRequestRepository repository, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetRepairByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Repair request not found." });

            return Results.Ok(new
            {
                request.Id,
                request.RequesterId,
                request.RequesterName,
                request.AssetId,
                request.AssetName,
                request.AssetRealWorldId,
                request.ProblemDescription,
                request.Status,
                request.ApprovalRemark,
                request.ApprovedBy,
                request.ApprovedAt,
                request.CompletedAt,
                request.TechnicianNote,
                request.CreateOn
            });
        })
        .WithName("GetRepairRequestById")
        .WithTags("RepairRequests");

        group.MapPost("/", async (CreateRepairRequest req, IRequestRepository repository, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var requesterName = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("unique_name") ?? "Unknown";

            if (!Guid.TryParse(userIdStr, out var requesterId))
                return Results.BadRequest(new { message = "Invalid user identity." });

            var repairRequest = RepairRequest.Create(
                requesterId,
                requesterName,
                req.AssetId,
                req.AssetName,
                req.AssetRealWorldId,
                req.ProblemDescription);

            await repository.AddRepairAsync(repairRequest, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.Created($"/repair-requests/{repairRequest.Id}", new { repairRequest.Id });
        })
        .WithName("CreateRepairRequest")
        .WithTags("RepairRequests");

        group.MapPut("/{id:long}/approve", async (long id, ApproveRepairRequest req, IRequestRepository repository, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetRepairByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Repair request not found." });

            if (request.Status != "pending")
                return Results.BadRequest(new { message = $"Cannot approve request with status '{request.Status}'." });

            var approverIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(approverIdStr, out var approverId))
                return Results.BadRequest(new { message = "Invalid approver identity." });

            request.Approve(approverId, req.Remark);
            await repository.UpdateRepairAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("ApproveRepairRequest")
        .WithTags("RepairRequests");

        group.MapPut("/{id:long}/reject", async (long id, ApproveRepairRequest req, IRequestRepository repository, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetRepairByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Repair request not found." });

            if (request.Status != "pending")
                return Results.BadRequest(new { message = $"Cannot reject request with status '{request.Status}'." });

            var rejectorIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(rejectorIdStr, out var rejectorId))
                return Results.BadRequest(new { message = "Invalid rejector identity." });

            request.Reject(rejectorId, req.Remark ?? "Rejected");
            await repository.UpdateRepairAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("RejectRepairRequest")
        .WithTags("RepairRequests");

        group.MapPut("/{id:long}/start", async (long id, IRequestRepository repository, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetRepairByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Repair request not found." });

            if (request.Status != "approved")
                return Results.BadRequest(new { message = $"Cannot start repair for request with status '{request.Status}'." });

            request.StartRepair();
            await repository.UpdateRepairAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("StartRepair")
        .WithTags("RepairRequests");

        group.MapPut("/{id:long}/complete", async (long id, CompleteRepairRequest req, IRequestRepository repository, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetRepairByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Repair request not found." });

            if (request.Status != "in_repair")
                return Results.BadRequest(new { message = $"Cannot complete repair for request with status '{request.Status}'." });

            request.Complete(req.TechnicianNote);
            await repository.UpdateRepairAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("CompleteRepair")
        .WithTags("RepairRequests");
    }
}

public record CreateRepairRequest(
    long AssetId,
    string AssetName,
    string AssetRealWorldId,
    string ProblemDescription
);

public record ApproveRepairRequest(string? Remark);
public record CompleteRepairRequest(string? TechnicianNote);
