using MassTransit;
using Request.Data.Repository.Read;
using Request.Data.Repository.Write;
using Shared.Pagination;
using Shared.Security;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Request.Service.CommandHandlerService;

public class RequestCommandHandlerService(
    IRequestReadRepository requestReadRepository,
    IRequestWriteRepository requestWriteRepository,
    IRequestClient<ReserveAssetCommand> reserveAssetClient,
    IRequestClient<ReleaseAssetCommand> releaseAssetClient) : IRequestCommandHandlerService
{
    private readonly IRequestReadRepository _requestReadRepository = requestReadRepository;
    private readonly IRequestWriteRepository _requestWriteRepository = requestWriteRepository;
    private readonly IRequestClient<ReserveAssetCommand> _reserveAssetClient = reserveAssetClient;
    private readonly IRequestClient<ReleaseAssetCommand> _releaseAssetClient = releaseAssetClient;

    public async Task<Guid> CreateRequest(CreateRequestDto request, Guid requesterId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.RequestType))
        {
            throw new ArgumentException("RequestType is required.", nameof(request));
        }

        if (request.TargetLaboratoryId == Guid.Empty)
        {
            throw new ArgumentException("TargetLaboratoryId is required.", nameof(request));
        }

        if (requesterId == Guid.Empty)
        {
            throw new ArgumentException("RequesterId is required.", nameof(requesterId));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new ArgumentException("Reason is required.", nameof(request));
        }

        var newRequest = Requests.Model.Request.Create(
            request.RequestType,
            request.TargetLaboratoryId,
            requesterId,
            request.Reason);

        ApplyDetail(newRequest, request.Detail);

        var createNewRequestEvent = new Requests.Events.CreateNewRequestEvent(newRequest.Id, request.TargetLaboratoryId);

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

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new ArgumentException("Reason is required.", nameof(request));
        }

        currentRequest.Update(
            request.RequestType,
            request.TargetLaboratoryId,
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
        if (detail is null) return;

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
        var incomingAssetIds = items
            .Where(x => x.AssetId != Guid.Empty)
            .Select(x => x.AssetId)
            .ToHashSet();
        SyncItems(request, incomingAssetIds);
    }

    private static void SyncItems(Requests.Model.Request request, IReadOnlyCollection<Guid> assetIds)
    {
        var incomingAssetIds = assetIds
            .Where(x => x != Guid.Empty)
            .ToHashSet();
        var existingAssetIds = request.Items.Select(x => x.AssetId).ToList();

        foreach (var existingAssetId in existingAssetIds.Where(x => !incomingAssetIds.Contains(x)))
        {
            request.RemoveItem(existingAssetId);
        }

        foreach (var assetId in incomingAssetIds)
        {
            request.AddItem(assetId);
        }
    }

    private static RequestDto MapToDto(Requests.Model.Request request)
    {
        return new RequestDto
        {
            Id = request.Id,
            RequestType = request.RequestType,
            TargetLaboratoryId = request.TargetLaboratoryId,
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
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
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

    public async Task<bool> MarkRequestProcessed(
        Guid requestId,
        Guid approverId,
        string approverRoleCode,
        string decision,
        string? comment,
        List<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        var request = await _requestWriteRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        if (!string.Equals(request.Status, RequestStatusCodes.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Request with id {requestId} is not in pending status.");
        }

        if (!request.CurrentStepNo.HasValue)
        {
            throw new InvalidOperationException($"Request with id {requestId} has no active approval step.");
        }

        var currentStep = request.Trackings
            .FirstOrDefault(x => x.StepNo == request.CurrentStepNo.Value)
            ?? throw new InvalidOperationException($"Current tracking step {request.CurrentStepNo.Value} was not found.");

        if (currentStep.AssignedApproverId.HasValue && currentStep.AssignedApproverId.Value != approverId)
        {
            throw new UnauthorizedAccessException("You are not the assigned approver for this step.");
        }

        if (!string.Equals(currentStep.RequiredRoleCode, approverRoleCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Your role is not allowed to process this step.");
        }

        if (string.Equals(decision, RequestDecisionCodes.Reject, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(currentStep.RequiredRoleCode, RoleCodes.Hod, StringComparison.OrdinalIgnoreCase))
            {
                var previousStep = request.Trackings
                    .Where(x => x.StepNo < currentStep.StepNo)
                    .OrderByDescending(x => x.StepNo)
                    .FirstOrDefault();

                if (previousStep is not null)
                {
                    var reservedAssetIds = request.Items
                        .Select(x => x.AssetId)
                        .Where(x => x != Guid.Empty)
                        .Distinct()
                        .ToList();

                    if (reservedAssetIds.Count > 0)
                    {
                        await ReleaseAssetsFromRequest(requestId, approverId, reservedAssetIds, cancellationToken);
                    }

                    currentStep.Reject(approverId, comment);
                    request.ActivateStep(previousStep.StepNo, previousStep.AssignedApproverId);
                    return true;
                }
            }

            currentStep.Reject(approverId, comment);
            request.MarkRejected();
            return true;
        }

        if (!string.Equals(decision, RequestDecisionCodes.Approve, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Decision must be either APPROVE or REJECT.", nameof(decision));
        }

        var normalizedAssetIds = assetIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (string.Equals(currentStep.RequiredRoleCode, RoleCodes.Teacher, StringComparison.OrdinalIgnoreCase))
        {
            if (normalizedAssetIds.Count == 0)
            {
                throw new ArgumentException("Teacher approval requires at least one asset id.", nameof(assetIds));
            }

            await ReserveAssetsFromRequest(requestId, approverId, normalizedAssetIds, cancellationToken);
            SyncItems(request, normalizedAssetIds);
        }
        else if (normalizedAssetIds.Count > 0)
        {
            throw new InvalidOperationException("Asset reservation is only allowed during the teacher approval step.");
        }

        currentStep.Approve(approverId, comment);

        var nextStep = request.Trackings
            .Where(x => x.StepNo > currentStep.StepNo)
            .OrderBy(x => x.StepNo)
            .FirstOrDefault();

        if (nextStep is null)
        {
            request.MarkApproved();
            return true;
        }

        request.ActivateStep(nextStep.StepNo, nextStep.AssignedApproverId);

        return true;
    }

    public async Task<bool> MarkRequestApproved(Guid requestId, CancellationToken cancellationToken)
    {
        var request = await _requestWriteRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        request.MarkApproved();

        return true;
    }

    public async Task<bool> ReserveAssetsFromRequest(Guid requestId, Guid approverId, List<Guid> assetIds, CancellationToken cancellationToken)
    {
        var request = await _requestReadRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        var response = await _reserveAssetClient.GetResponse<ReserveAssetCommandResponse>(
            new ReserveAssetCommand(request.Id, approverId, assetIds),
            cancellationToken);

        return response.Message.IsSuccess;
    }

    public async Task<bool> ReleaseAssetsFromRequest(Guid requestId, Guid approverId, List<Guid> assetIds, CancellationToken cancellationToken)
    {
        var request = await _requestReadRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        var response = await _releaseAssetClient.GetResponse<ReleaseAssetCommandResponse>(
            new ReleaseAssetCommand(request.Id, approverId, assetIds),
            cancellationToken);

        return response.Message.IsSuccess;
    }


}
