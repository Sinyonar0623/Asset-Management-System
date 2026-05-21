using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asset.Assets.Features.AssetUnitFeature.UploadAssetUnitImage;

public class UploadAssetUnitImageEndpoint : ICarterModule
{
    private const int MaxDescriptionLength = 1000;
    private const int MaxFileNameLength = 255;
    private const string PngContentType = "image/png";
    private const string JpegContentType = "image/jpeg";
    private const string JpgContentType = "image/jpg";
    private const string ProgressiveJpegContentType = "image/pjpeg";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/AssetUnit/{assetUnitId:guid}/Images", async (
            Guid assetUnitId,
            HttpRequest request,
            IWebHostEnvironment environment,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest("Content-Type must be multipart/form-data.");
            }

            var form = await request.ReadFormAsync(cancellationToken);
            var files = form.Files.GetFiles("files");
            var description = form["description"].FirstOrDefault();

            if (files.Count == 0)
            {
                return Results.BadRequest("At least one image file is required.");
            }

            if (description?.Trim().Length > MaxDescriptionLength)
            {
                return Results.BadRequest("Image description must not exceed 1000 characters.");
            }

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    return Results.BadRequest("Image file is required.");
                }

                if (!await IsAllowedPngOrJpegAsync(file, cancellationToken))
                {
                    return Results.BadRequest("Only PNG and JPEG image files are allowed.");
                }
            }

            var relativeDirectory = Path.Combine("uploads", "asset-units", assetUnitId.ToString("N"));
            var webRootPath = environment.WebRootPath
                              ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            var uploadDirectory = Path.Combine(webRootPath, relativeDirectory);
            Directory.CreateDirectory(uploadDirectory);

            var savedFiles = new List<string>(files.Count);
            var imageItems = new List<CreateAssetUnitImageDto>(files.Count);

            try
            {
                foreach (var file in files)
                {
                    var extension = NormalizeExtension(file.FileName);
                    var storedFileName = $"{Guid.NewGuid():N}{extension}";
                    var physicalPath = Path.Combine(uploadDirectory, storedFileName);

                    await using (var stream = System.IO.File.Create(physicalPath))
                    {
                        await file.CopyToAsync(stream, cancellationToken);
                    }

                    savedFiles.Add(physicalPath);

                    imageItems.Add(new CreateAssetUnitImageDto
                    {
                        ImageUrl = $"/uploads/asset-units/{assetUnitId:N}/{storedFileName}",
                        Description = description,
                        FileName = NormalizeOriginalFileName(file.FileName),
                        ContentType = file.ContentType,
                        FileSizeBytes = file.Length
                    });
                }

                var command = new UploadAssetUnitImageCommand(assetUnitId, imageItems);

                var result = await sender.Send(command, cancellationToken);
                var response = new UploadAssetUnitImageResponse(result.Images);

                return Results.Created($"/AssetUnit/{assetUnitId}/Images", response);
            }
            catch
            {
                foreach (var savedFile in savedFiles)
                {
                    System.IO.File.Delete(savedFile);
                }

                throw;
            }
        })
        .WithName("UploadAssetUnitImage")
        .Accepts<UploadAssetUnitImageForm>("multipart/form-data")
        .Produces<UploadAssetUnitImageResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .DisableAntiforgery()
        .WithSummary("Upload Asset Unit Image");
    }

    private static async Task<bool> IsAllowedPngOrJpegAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension is not ".png" and not ".jpg" and not ".jpeg")
        {
            return false;
        }

        var contentType = file.ContentType?.ToLowerInvariant();
        if (contentType is not PngContentType
            && contentType is not JpegContentType
            && contentType is not JpgContentType
            && contentType is not ProgressiveJpegContentType)
        {
            return false;
        }

        var buffer = new byte[8];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);

        return contentType == PngContentType
            ? IsPng(buffer, bytesRead)
            : IsJpeg(buffer, bytesRead);
    }

    private static bool IsPng(byte[] buffer, int bytesRead)
    {
        return bytesRead >= 8
               && buffer[0] == 0x89
               && buffer[1] == 0x50
               && buffer[2] == 0x4E
               && buffer[3] == 0x47
               && buffer[4] == 0x0D
               && buffer[5] == 0x0A
               && buffer[6] == 0x1A
               && buffer[7] == 0x0A;
    }

    private static bool IsJpeg(byte[] buffer, int bytesRead)
    {
        return bytesRead >= 3
               && buffer[0] == 0xFF
               && buffer[1] == 0xD8
               && buffer[2] == 0xFF;
    }

    private static string NormalizeExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".jpeg" ? ".jpg" : extension;
    }

    private static string? NormalizeOriginalFileName(string fileName)
    {
        var normalizedFileName = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(normalizedFileName))
        {
            return null;
        }

        return normalizedFileName.Length > MaxFileNameLength
            ? normalizedFileName[..MaxFileNameLength]
            : normalizedFileName;
    }

    private sealed record UploadAssetUnitImageForm(
        List<IFormFile> Files,
        string? Description);
}
