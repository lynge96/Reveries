using System.Text.RegularExpressions;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Interfaces;
using Reveries.Core.Helpers;
using Reveries.Core.Models;

namespace Reveries.Console.Handlers;

public partial class BookSeriesHandler : BaseHandler
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex();
    public override MenuChoice MenuChoice => MenuChoice.BookSeries;
    private readonly ISaveEntityService _saveEntityService;
    
    public BookSeriesHandler(ISaveEntityService saveEntityService)
    {
        _saveEntityService = saveEntityService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var seriesInput = ConsolePromptUtility.GetUserInput("Enter the name of the new series:");
        var seriesName = MyRegex().Replace(seriesInput.Trim().ToTitleCase(), " ");
        
        var confirm = ConsolePromptUtility.ShowYesNoPrompt("Are you sure you want to create a new series with the name: " + seriesName.AsSecondary().Bold());
        if (!confirm)
            return;
        
        var newSeries = Series.Create(seriesName);

        await _saveEntityService.SaveSeriesAsync(newSeries, cancellationToken);
    }
}