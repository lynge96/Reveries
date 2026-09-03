namespace Reveries.Domain.Helpers;

public static class PageCountNormalizer
{
    private const int MaxReasonablePageCount = 50_000;

    public static int? Normalize(int? pages)
    {
        if (pages is null or <= 0 or > MaxReasonablePageCount)
            return null;

        return pages;
    }
}