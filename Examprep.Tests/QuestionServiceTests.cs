using Examprep.Application.DTOs;
using Examprep.Application.Repositories;
using Examprep.Application.Services;
using Examprep.Domain.Model;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Examprep.Tests
{
    public class QuestionServiceTests
    {
        private readonly Mock<IQuestionRepository> _repoMock;
        private readonly Mock<ILogger<QuestionService>> _loggerMock;
        private readonly IMemoryCache _cache;
        private readonly QuestionService _service;

        public QuestionServiceTests()
        {
            _repoMock = new Mock<IQuestionRepository>();
            _loggerMock = new Mock<ILogger<QuestionService>>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _service = new QuestionService(_repoMock.Object, _loggerMock.Object, _cache);
        }

        [Fact]
        public async Task GetAllQuestionsAsync_ReturnsMappedDtos()
        {
            // Arrange
            var dtos = new List<QuestionResponseDto>
{
    new QuestionResponseDto { Id = 1, Text = "Q1" },
    new QuestionResponseDto { Id = 2, Text = "Q2" }
};
            _repoMock.Setup(r => r.GetAllQuestionsAsync()).ReturnsAsync(dtos);

            // Act
            var result = await _service.GetAllQuestionsAsync();

            // Assert
            result.Should().HaveCount(2);
            result[0].Text.Should().Be("Q1");
            result[1].Text.Should().Be("Q2");
        }

        [Fact]
        public async Task GetQuestionByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetQuestionByIdAsync(99))
                     .ReturnsAsync((Question?)null);

            // Act
            var result = await _service.GetQuestionByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetQuestionByIdAsync_ReturnsDto_WhenFound()
        {
            // Arrange
            var q = new Question { id = 5, text = "Hello" };
            _repoMock.Setup(r => r.GetQuestionByIdAsync(5)).ReturnsAsync(q);

            // Act
            var result = await _service.GetQuestionByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(5);
            result.Text.Should().Be("Hello");
        }

        [Fact]
        public async Task CreateQuestionAsync_SavesAndReturnsDto()
        {
            // Arrange
            var dto = new QuestionCreateDto { Text = "New question" };

            _repoMock.Setup(r => r.AddQuestionWithCounterAsync(It.IsAny<Question>(), 1))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.GetQuestionByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync(new Question { id = 100, text = "New question", UserId = 1 });

            // Act
            var result = await _service.CreateQuestionAsync(dto, 1);

            // Assert
            result.Should().NotBeNull();
            result.Text.Should().Be("New question");
            _repoMock.Verify(r => r.AddQuestionWithCounterAsync(
                It.Is<Question>(q => q.text == "New question" && q.UserId == 1),
                1), Times.Once);
        }

        [Fact]
        public async Task UpdateQuestionAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetQuestionByIdAsync(77))
                     .ReturnsAsync((Question?)null);

            // Act
            var result = await _service.UpdateQuestionAsync(77, new QuestionUpdateDto { Text = "x" });

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateQuestionAsync_UpdatesAndReturnsTrue()
        {
            // Arrange
            var existing = new Question { id = 3, text = "Old" };
            _repoMock.Setup(r => r.GetQuestionByIdAsync(3)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateQuestionAsync(It.IsAny<Question>()))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateQuestionAsync(3, new QuestionUpdateDto { Text = "New" });

            // Assert
            result.Should().BeTrue();
            existing.text.Should().Be("New");
        }

        [Fact]
        public async Task DeleteQuestionAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetQuestionByIdAsync(55))
                     .ReturnsAsync((Question?)null);

            // Act
            var result = await _service.DeleteQuestionAsync(55);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteQuestionAsync_DeletesAndReturnsTrue()
        {
            // Arrange
            var existing = new Question { id = 4, text = "Delete me" };
            _repoMock.Setup(r => r.GetQuestionByIdAsync(4)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.DeleteQuestionAsync(It.IsAny<Question>()))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteQuestionAsync(4);

            // Assert
            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteQuestionAsync(existing), Times.Once);
        }
    }
}