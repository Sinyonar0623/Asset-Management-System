using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Response;

public sealed record ReleaseAssetCommandResponse(
    bool IsSuccess
) : IIntegration;
