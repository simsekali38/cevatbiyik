using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;

namespace CevatBiyik.UnitTests;

public sealed class RichTextTests
{
    [Theory]
    [InlineData("<h2>Başlık</h2><p><strong>Kalın</strong> ve <em>italik</em></p>")]
    [InlineData("<p class=\"content-align-center\">Ortalanmış</p>")]
    [InlineData("<ul><li>Madde</li></ul><ol><li>Sıra</li></ol>")]
    [InlineData("<blockquote>Alıntı</blockquote><p><a href=\"/makaleler\">Makaleler</a></p>")]
    [InlineData("<p><a href=\"https://example.test/path\">Kaynak</a><br>Devam</p>")]
    public void PermittedFragments_RoundTripAndRemainSafe(string value)
    {
        Assert.True(RichText.IsSafe(value));
        var normalized = RichText.Normalize(value);
        Assert.Equal(normalized, RichText.Normalize(normalized));
        Assert.DoesNotContain('<', RichText.ToPlainText(normalized));
    }

    [Theory]
    [InlineData("<script>alert(1)</script><p>Metin</p>")]
    [InlineData("<p onclick=\"alert(1)\">Metin</p>")]
    [InlineData("<svg><a onload=\"alert(1)\">Metin</a></svg>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("<a href=\"javascript:alert(1)\">Metin</a>")]
    [InlineData("<a href=\"&#106;avascript:alert(1)\">Metin</a>")]
    [InlineData("<a href=\"//example.test\">Metin</a>")]
    [InlineData("<a href=\"https://user:pass@example.test\">Metin</a>")]
    [InlineData("<p id=\"location\" style=\"color:red\">Metin</p>")]
    [InlineData("<p class=\"ql-align-center\">Metin</p>")]
    [InlineData("<p class=\"content-align-center evil\">Metin</p>")]
    [InlineData("<a class=\"content-align-right\">Metin</a>")]
    [InlineData("<form name=\"document\"><input></form>")]
    [InlineData("<p><br></p>")]
    [InlineData("&lt;script&gt;alert(1)&lt;/script&gt;")]
    public void UnsafeOrEmptyFragments_AreRejected(string value) => Assert.False(RichText.IsSafe(value));

    [Fact]
    public void DefensiveRendering_EliminatesLegacyActiveMarkupAndEncodesPlainText()
    {
        var output = RichText.Render("<p onclick=\"alert(1)\">Güvenli</p><script>alert(1)</script>");
        Assert.Contains("<p>", output);
        Assert.DoesNotContain("onclick", output);
        Assert.DoesNotContain("script", output);
        Assert.Equal("A &amp; B", RichText.Render("A & B"));
        Assert.Contains("\n", RichText.ToPlainText("<p>Bir</p><p>İki<br>Üç</p>"));
        Assert.False(ContentPublication.Complete("<p><strong>CONTENT_REQUIRED</strong></p>"));
    }

    [Fact]
    public async Task FaqService_PersistsRichAnswer_ButRejectsUnsafeInputBeforeMutation()
    {
        var repository = new FakeRepository<FaqItem>();
        var service = new FaqService(repository);
        var input = new FaqInput { Question = "Test sorusu", Answer = "<p><strong>Test</strong> yanıtı</p>", IsReviewed = true, IsActive = true };
        await service.SaveAsync(null, input);
        Assert.Contains("<strong>", repository.Items.Single().Answer);
        input.Answer = "<p onmouseover=\"alert(1)\">Hatalı</p>";
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(1, input));
        Assert.Contains("<strong>", repository.Items.Single().Answer);
        input.Question = "<strong>Soru</strong>";
        Assert.False((await new FaqValidator().ValidateAsync(input)).IsValid);
    }

    [Fact]
    public void Limits_IncludeSerializedHtmlExpansion()
    {
        Assert.False(RichText.Fits("<p>" + new string('&', 1000) + "</p>", 4000));
        Assert.False(RichText.IsSafe(new string('x', 100001)));
    }
}
