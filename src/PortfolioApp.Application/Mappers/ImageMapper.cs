using PortfolioApp.Application.DTOs;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Application.Mappers;

/// <summary>
/// Provides manual mapping functionalities between Image entities and Image DTOs.
/// This mapper ensures a clear separation of concerns and avoids direct exposure
/// of domain entities to the presentation layer, adhering to Clean Architecture principles.
/// </summary>
public static class ImageMapper
{
    /// <summary>
    /// Maps an Image entity to an ImageDto.
    /// This method transforms the domain model into a presentation-friendly DTO.
    /// </summary>
    /// <param name="image">The Image entity to map.</param>
    /// <returns>A new ImageDto instance.</returns>
    public static ImageDto ToDto(Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        return new ImageDto
        {
            Id = image.Id,
            ProjectId = image.ProjectId,
            Title = image.Title,
            Description = image.Description,
            Url = image.Url,
            AltText = image.AltText,
            Width = image.Width,
            Height = image.Height,
            SizeInBytes = image.SizeInBytes,
            FormattedFileSize = FormatFileSize(image.SizeInBytes),
            Dimensions = $"{image.Width}×{image.Height}",
            AspectRatio = CalculateAspectRatio(image.Width, image.Height),
            IsLandscape = image.Width > image.Height,
            IsPortrait = image.Height > image.Width,
            IsSquare = image.Width == image.Height,
            FileExtension = GetFileExtension(image.Url),
            UploadedAt = image.UploadedAt
        };
    }

    /// <summary>
    /// Maps a CreateImageDto to an Image entity.
    /// This method transforms a DTO from the presentation layer into a domain entity.
    /// </summary>
    /// <param name="dto">The CreateImageDto to map.</param>
    /// <returns>A new Image entity instance.</returns>
    public static Image ToEntity(CreateImageDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        return new Image(
            dto.ProjectId,
            dto.Title,
            dto.Description,
            dto.Url,
            dto.AltText,
            dto.Width,
            dto.Height,
            dto.SizeInBytes
        );
    }

    /// <summary>
    /// Updates an existing Image entity from an UpdateImageDto.
    /// This method modifies an existing domain entity based on the DTO.
    /// </summary>
    /// <param name="dto">The UpdateImageDto containing updated information.</param>
    /// <param name="image">The existing Image entity to update.</param>
    public static void ToEntity(UpdateImageDto dto, Image image)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        image.UpdateMetadata(
            dto.Title,
            dto.Description,
            dto.AltText
        );
    }

    /// <summary>
    /// Maps a collection of Image entities to ImageDto collection.
    /// </summary>
    /// <param name="images">Collection of Image entities to map.</param>
    /// <returns>Collection of ImageDto instances.</returns>
    public static IEnumerable<ImageDto> ToDtoCollection(IEnumerable<Image> images)
    {
        if (images == null)
        {
            throw new ArgumentNullException(nameof(images));
        }

        return images.Select(ToDto).ToList();
    }

    /// <summary>
    /// Maps a FileUploadDto to a CreateImageDto.
    /// This method transforms upload information into a creation DTO.
    /// </summary>
    /// <param name="fileUpload">The FileUploadDto containing upload information.</param>
    /// <param name="projectId">The ID of the project this image belongs to.</param>
    /// <param name="title">The title for the image.</param>
    /// <param name="description">The description for the image.</param>
    /// <param name="altText">The alternative text for the image.</param>
    /// <returns>A new CreateImageDto instance.</returns>
    public static CreateImageDto FromFileUpload(FileUploadDto fileUpload, int projectId, string title, string description = "", string altText = "")
    {
        if (fileUpload == null)
        {
            throw new ArgumentNullException(nameof(fileUpload));
        }

        if (!fileUpload.IsImage)
        {
            throw new ArgumentException("FileUploadDto must represent an image file.", nameof(fileUpload));
        }

        return new CreateImageDto
        {
            ProjectId = projectId,
            Title = title,
            Description = description,
            Url = fileUpload.Url,
            AltText = string.IsNullOrWhiteSpace(altText) ? title : altText,
            Width = fileUpload.Width ?? 0,
            Height = fileUpload.Height ?? 0,
            SizeInBytes = fileUpload.SizeInBytes
        };
    }

    /// <summary>
    /// Formats a file size in bytes to a human-readable string.
    /// </summary>
    /// <param name="sizeInBytes">The file size in bytes.</param>
    /// <returns>A formatted file size string (e.g., "1.2 MB").</returns>
    private static string FormatFileSize(long sizeInBytes)
    {
        if (sizeInBytes == 0)
        {
            return "0 B";
        }

        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = sizeInBytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Calculates the aspect ratio of an image.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <returns>The aspect ratio as a double.</returns>
    private static double CalculateAspectRatio(int width, int height)
    {
        if (height == 0)
        {
            return 0;
        }

        return Math.Round((double)width / height, 2);
    }

    /// <summary>
    /// Extracts the file extension from a URL.
    /// </summary>
    /// <param name="url">The URL to extract the extension from.</param>
    /// <returns>The file extension (e.g., ".jpg").</returns>
    private static string GetFileExtension(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        try
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            var path = uri.IsAbsoluteUri ? uri.LocalPath : url;
            return Path.GetExtension(path).ToLowerInvariant();
        }
        catch
        {
            // If URL parsing fails, try to extract extension from the string directly
            var lastDotIndex = url.LastIndexOf('.');
            if (lastDotIndex >= 0 && lastDotIndex < url.Length - 1)
            {
                var extension = url.Substring(lastDotIndex);
                // Remove query parameters if present
                var queryIndex = extension.IndexOf('?');
                if (queryIndex >= 0)
                {
                    extension = extension.Substring(0, queryIndex);
                }
                return extension.ToLowerInvariant();
            }
            return string.Empty;
        }
    }
}

