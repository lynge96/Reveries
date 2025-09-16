using System.Text.RegularExpressions;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public partial class BookSeriesHandler : BaseHandler
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex();
    public override MenuChoice MenuChoice => MenuChoice.BookSeries;
    private readonly IBookSeriesService _bookSeriesService;
    
    public BookSeriesHandler(IBookSeriesService bookSeriesService)
    {
        _bookSeriesService = bookSeriesService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var seriesInput = ConsolePromptUtility.GetUserInput("Enter the name of the new series:");
        var seriesName = MyRegex().Replace(seriesInput.Trim().ToTitleCase()!, " ");
        
        var confirm = ConsolePromptUtility.ShowYesNoPrompt("Are you sure you want to create a new series with the name: " + seriesName.AsSecondary().Bold());
        if (!confirm)
            return;
        
        var newSeries = new Series(seriesName);
        var newSeriesId = await _bookSeriesService.CreateSeriesAsync(newSeries);
        
        if (newSeriesId == -1)
        {
            AnsiConsole.MarkupLine($"The series: {seriesName.AsSecondary().Bold()} already exists in the database".AsWarning());
            return;
        }
        
        AnsiConsole.MarkupLine($"Success! The series: {newSeriesId.AsSecondary().Bold()} {seriesName.AsSecondary().Bold()} - has been saved in the database.".AsSuccess());
    }
}