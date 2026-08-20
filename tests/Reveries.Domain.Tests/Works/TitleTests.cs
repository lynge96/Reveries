using Reveries.Domain.Exceptions;
using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class TitleTests
{
    [Fact]
    public void Create_TrimsValue()
    {
        var title = Title.Create("  Dune  ");

        Assert.Equal("Dune", title.Text);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ThrowsMissingTitle(string value)
    {
        Assert.Throws<MissingTitleException>(() => Title.Create(value));
    }

    [Fact]
    public void Create_ExceedingMaxLength_ThrowsTitleTooLong()
    {
        var tooLong = new string('a', 501);

        Assert.Throws<TitleTooLongException>(() => Title.Create(tooLong));
    }
}