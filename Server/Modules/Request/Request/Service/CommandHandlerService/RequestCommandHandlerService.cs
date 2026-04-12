using System.Diagnostics.Tracing;
using Request.Data.Repository.Read;
using Request.Data.Repository.Write;
using Request.Requests.Events;
using Shared.Pagination;

namespace Request.Service.CommandHandlerService;

public class RequestCommandHandlerService(
    IRequestReadRepository requestReadRepository,
    IRequestWriteRepository requestWriteRepository) : IRequestCommandHandlerService
{
    private readonly IRequestReadRepository _requestReadRepository = requestReadRepository;
    private readonly IRequestWriteRepository _requestWriteRepository = requestWriteRepository;

    public async Task<Guid> CreateRequest(CreateRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.RequestNo))
        {
            throw new ArgumentException("RequestNo is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.RequestType))
        {
            throw new ArgumentException("RequestType is required.", nameof(request));
        }

        if (request.TargetLaboratoryId == Guid.Empty)
        {
            throw new ArgumentException("TargetLaboratoryId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.RequestedAssetCategory))
        {
            throw new ArgumentException("RequestedAssetCategory is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new ArgumentException("Reason is required.", nameof(request));
        }

        var newRequest = Requests.Model.Request.Create(
            request.RequestNo,
            request.RequestType,
            request.TargetLaboratoryId,
            request.RequestedAssetCategory,
            request.RequesterId,
            request.Reason);

        ApplyDetail(newRequest, request.Detail);

        var createNewRequestEvent = new CreateNewRequestEvent(newRequest.Id, request.TargetLaboratoryId);

        newRequest.AddDomainEvent(createNewRequestEvent); 

        await _requestWriteRepository.AddAsync(newRequest, cancellationToken);

        return newRequest.Id;
    }

    public async Task<RequestDto> GetRequestById(Guid requestId, CancellationToken cancellationToken = default)
    {
        if (requestId == Guid.Empty)
        {
            throw new ArgumentException("Request id is required.", nameof(requestId));
        }

        var request = await _requestReadRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        return MapToDto(request);
    }

    public async Task<PaginatedResult<RequestDto>> GetRequests(
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(0, paginationRequest.PageNumber);
        var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
        var request = new PaginationRequest(pageNumber, pageSize);

        var requests = await _requestReadRepository.GetPaginatedAsync(request, cancellationToken);
        var requestDtos = requests.Items.Select(MapToDto).ToList();

        return new PaginatedResult<RequestDto>(
            requestDtos,
            requests.Count,
            requests.PageNumber,
            requests.PageSize);
    }

    public async Task<bool> UpdateRequest(Guid requestId, UpdateRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentRequest = await _requestWriteRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        if (string.IsNullOrWhiteSpace(request.RequestType))
        {
            throw new ArgumentException("RequestType is required.", nameof(request));
        }

        if (request.TargetLaboratoryId == Guid.Empty)
        {
            throw new ArgumentException("TargetLaboratoryId is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.RequestedAssetCategory))
        {
            throw new ArgumentException("RequestedAssetCategory is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new ArgumentException("Reason is required.", nameof(request));
        }

        currentRequest.Update(
            request.RequestType,
            request.TargetLaboratoryId,
            request.RequestedAssetCategory,
            request.RequesterId,
            request.Reason);

        if (request.Detail is not null)
        {
            ApplyDetail(currentRequest, request.Detail);
        }

        if (request.Items is not null)
        {
            SyncItems(currentRequest, request.Items);
        }

        return true;
    }

    public async Task<bool> DeleteRequest(Guid requestId, CancellationToken cancellationToken)
    {
        var currentRequest = await _requestWriteRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        await _requestWriteRepository.DeleteAsync(currentRequest, cancellationToken);

        return true;
    }

    private static void ApplyDetail(Requests.Model.Request request, RequestDetailDto? detail)
    {
        if (detail is null)
        {
            return;
        }

        request.SetOrUpdateDetail(
            detail.Purpose,
            detail.BorrowFrom,
            detail.BorrowTo,
            detail.IssueDescription,
            detail.RetireReason,
            detail.ExtraNote);
    }

    private static void SyncItems(Requests.Model.Request request, IReadOnlyCollection<RequestItemDto> items)
    {
        var incomingAssetIds = items.Select(x => x.AssetId).ToHashSet();
        var existingAssetIds = request.Items.Select(x => x.AssetId).ToList();

        foreach (var existingAssetId in existingAssetIds.Where(x => !incomingAssetIds.Contains(x)))
        {
            request.RemoveItem(existingAssetId);
        }

        foreach (var item in items)
        {
            var existingItem = request.Items.FirstOrDefault(x => x.AssetId == item.AssetId);
            if (existingItem is null)
            {
                request.AddOrIncreaseItem(item.AssetId, item.QuantityRequested, item.Note);
                existingItem = request.Items.First(x => x.AssetId == item.AssetId);
            }
            else
            {
                existingItem.ChangeQuantity(item.QuantityRequested);
                existingItem.UpdateNote(item.Note);
            }

            if (item.QuantityApproved.HasValue)
            {
                existingItem.SetApprovalQuantity(item.QuantityApproved.Value);
            }
        }
    }

    private static RequestDto MapToDto(Requests.Model.Request request)
    {
        return new RequestDto
        {
            Id = request.Id,
            RequestNo = request.RequestNo,
            RequestType = request.RequestType,
            TargetLaboratoryId = request.TargetLaboratoryId,
            RequestedAssetCategory = request.RequestedAssetCategory,
            Status = request.Status,
            RequesterId = request.RequesterId,
            Reason = request.Reason,
            CurrentStepNo = request.CurrentStepNo,
            NextApproverId = request.NextApproverId,
            SubmittedOn = request.SubmittedOn,
            FinalizedOn = request.FinalizedOn,
            Detail = request.Detail is null
                ? null
                : new RequestDetailDto
                {
                    Purpose = request.Detail.Purpose,
                    BorrowFrom = request.Detail.BorrowFrom,
                    BorrowTo = request.Detail.BorrowTo,
                    IssueDescription = request.Detail.IssueDescription,
                    RetireReason = request.Detail.RetireReason,
                    ExtraNote = request.Detail.ExtraNote
                },
            Items = request.Items.Select(item => new RequestItemDto
            {
                AssetId = item.AssetId,
                QuantityRequested = item.QuantityRequested,
                QuantityApproved = item.QuantityApproved,
                Note = item.Note
            }).ToList(),
            Trackings = request.Trackings.Select(tracking => new RequestTrackingDto
            {
                StepNo = tracking.StepNo,
                RequiredRoleCode = tracking.RequiredRoleCode,
                AssignedApproverId = tracking.AssignedApproverId,
                Status = tracking.Status,
                ActionByUserId = tracking.ActionByUserId,
                ActionOn = tracking.ActionOn,
                Comment = tracking.Comment,
                IsCurrent = tracking.IsCurrent
            }).OrderBy(x => x.StepNo).ToList()
        };
    }
}
