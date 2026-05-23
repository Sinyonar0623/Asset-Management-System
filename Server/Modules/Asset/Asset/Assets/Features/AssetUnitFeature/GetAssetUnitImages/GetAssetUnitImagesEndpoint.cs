using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitImages;

public class GetAssetUnitImagesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/AssetUnit/{assetUnitId:guid}/images", async (
                Guid assetUnitId,
                HttpRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetAssetUnitImagesQuery(assetUnitId),
                    cancellationToken);
                var images = result.Images
                    .Select(image => image with
                    {
                        ImageUrl = ToAbsoluteImageUrl(request, image.ImageUrl)
                    })
                    .ToList();
                var response = new GetAssetUnitImagesResponse(images);

                return Results.Ok(response);
            })
            .WithName("GetAssetUnitImages")
            .Produces<GetAssetUnitImagesResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Asset Unit Images By Asset Unit Id");
    }

    private static string ToAbsoluteImageUrl(HttpRequest request, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)
            || imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            || imageUrl.StartsWith("blob:", StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl;
        }

        var path = imageUrl.StartsWith('/') ? imageUrl : $"/{imageUrl}";
        return $"{request.Scheme}://{request.Host}{path}";
    }
}
