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
    private readonly IBookLookupService _bookLookupService;
    private readonly IBookSeriesService _bookSeriesService;
    
    public BookSeriesHandler(IBookLookupService bookLookupService, IBookSeriesService bookSeriesService)
    {
        _bookLookupService = bookLookupService;
        _bookSeriesService = bookSeriesService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var booksInDb = await _bookLookupService.GetAllBooksAsync(cancellationToken);
        if (booksInDb.Count == 0)
            return;
        // Opret ny serie eller tilføj bøger til en eksisterende serie
        string[] options = ["Add books to existing series", "Create new series", "Exit"];
        var selection = ConsolePromptUtility.ShowSelectionPrompt("Do you want to create a new series or add books to an existing series?", options);

        switch (selection)
        {
            case "Add books to existing series":
                var existingSeries = booksInDb
                    .Select(b => b.Series!.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct()
                    .ToList();
                if (existingSeries.Count > 0)
                {
                    var selectedSeries = ConsolePromptUtility.ShowSelectionPrompt("Select the series you want to add books to:", 
                        existingSeries);
                    
                    // Vælge de bøger der skal tilføjes til serien
                    // Derefter skal man angive hvilket nummer de skal have i serien
                }
                break;
            case "Create new series":
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
                
                break;
            case "Exit":
                break;
        }
    }


}