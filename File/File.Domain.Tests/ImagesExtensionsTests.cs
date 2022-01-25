using File.Application.Extensions;
using FluentAssertions;
using Xunit;

namespace File.Domain.Tests
{
    public class ImagesExtensionsTests
    {
        [Theory]
        [InlineData(128)]
        [InlineData(256)]
        [InlineData(512)]
        [InlineData(1024)]
        public void WidthMustBeEqualsToResizeTo_and_HeightMustBeLessThanResizeTo(int resizeTo)
        {
            // Arrange
            const int imageWidth = 1526;
            const int imageHeight = 1306;

            // Act
            var (width, height) = ImagesExtensions.CalculateResizedImageDimensions(imageWidth, imageHeight, resizeTo);

            // Assert
            width.Should().Be(resizeTo);
            height.Should().BeLessThan(resizeTo);
        }
    }
}
