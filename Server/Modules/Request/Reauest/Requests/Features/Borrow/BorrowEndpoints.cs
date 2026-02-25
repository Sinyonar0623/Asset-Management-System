using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Reauest.Data.Repository;
using Reauest.Requests.Model;
using System.Security.Claims;

namespace Reauest.Requests.Features.Borrow;

public sealed class BorrowEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/borrow-requests").RequireAuthorization();

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

            // Students can only see their own requests
            if (roleCode == "03" || myOnly)
            {
                if (Guid.TryParse(userIdStr, out var uid))
                    requesterId = uid;
            }

            var (items, total) = await repository.GetBorrowsPagedAsync(page, pageSize, requesterId, status, cancellationToken);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            var result = items.Select(b => new
            {
                b.Id,
                b.RequesterId,
                b.RequesterName,
                b.AssetId,
                b.AssetName,
                b.AssetRealWorldId,
                b.BorrowDate,
                b.ReturnDate,
                b.Purpose,
                b.Status,
                b.ApprovalRemark,
                b.ApprovedBy,
                b.ApprovedAt,
                b.ActualReturnDate,
                b.CreateOn
            });

            return Results.Ok(new { Items = result, TotalCount = total, Page = page, PageSize = pageSize, TotalPages = totalPages });
        })
        .WithName("GetBorrowRequests")
        .WithTags("BorrowRequests");

        group.MapGet("/{id:long}", async (long id, IRequestRepository repository, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetBorrowByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Borrow request not found." });

            return Results.Ok(new
            {
                request.Id,
                request.RequesterId,
                request.RequesterName,
                request.AssetId,
                request.AssetName,
                request.AssetRealWorldId,
                request.BorrowDate,
                request.ReturnDate,
                request.Purpose,
                request.Status,
                request.ApprovalRemark,
                request.ApprovedBy,
                request.ApprovedAt,
                request.ActualReturnDate,
                request.CreateOn
            });
        })
        .WithName("GetBorrowRequestById")
        .WithTags("BorrowRequests");

        group.MapPost("/", async (CreateBorrowRequest req, IRequestRepository repository, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var requesterName = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("unique_name") ?? "Unknown";

            if (!Guid.TryParse(userIdStr, out var requesterId))
                return Results.BadRequest(new { message = "Invalid user identity." });

            if (req.BorrowDate >= req.ReturnDate)
                return Results.BadRequest(new { message = "Return date must be after borrow date." });

            var borrowRequest = BorrowRequest.Create(
                requesterId,
                requesterName,
                req.AssetId,
                req.AssetName,
                req.AssetRealWorldId,
                req.BorrowDate,
                req.ReturnDate,
                req.Purpose);

            await repository.AddBorrowAsync(borrowRequest, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.Created($"/borrow-requests/{borrowRequest.Id}", new { borrowRequest.Id });
        })
        .WithName("CreateBorrowRequest")
        .WithTags("BorrowRequests");

        group.MapPut("/{id:long}/approve", async (long id, ApproveRequest req, IRequestRepository repository, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetBorrowByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Borrow request not found." });

            if (request.Status != "pending")
                return Results.BadRequest(new { message = $"Cannot approve request with status '{request.Status}'." });

            var approverIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(approverIdStr, out var approverId))
                return Results.BadRequest(new { message = "Invalid approver identity." });

            request.Approve(approverId, req.Remark);
            await repository.UpdateBorrowAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("ApproveBorrowRequest")
        .WithTags("BorrowRequests");

        group.MapPut("/{id:long}/reject", async (long id, ApproveRequest req, IRequestRepository repository, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetBorrowByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Borrow request not found." });

            if (request.Status != "pending")
                return Results.BadRequest(new { message = $"Cannot reject request with status '{request.Status}'." });

            var rejectorIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(rejectorIdStr, out var rejectorId))
                return Results.BadRequest(new { message = "Invalid rejector identity." });

            request.Reject(rejectorId, req.Remark ?? "Rejected");
            await repository.UpdateBorrowAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("RejectBorrowRequest")
        .WithTags("BorrowRequests");

        group.MapPut("/{id:long}/return", async (long id, IRequestRepository repository, CancellationToken cancellationToken) =>
        {
            var request = await repository.GetBorrowByIdAsync(id, cancellationToken);
            if (request is null)
                return Results.NotFound(new { message = "Borrow request not found." });

            if (request.Status != "approved")
                return Results.BadRequest(new { message = $"Cannot mark returned for request with status '{request.Status}'." });

            request.MarkReturned();
            await repository.UpdateBorrowAsync(request, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("ReturnBorrowRequest")
        .WithTags("BorrowRequests");
    }
}

public record CreateBorrowRequest(
    long AssetId,
    string AssetName,
    string AssetRealWorldId,
    DateTime BorrowDate,
    DateTime ReturnDate,
    string Purpose
);

public record ApproveRequest(string? Remark);
