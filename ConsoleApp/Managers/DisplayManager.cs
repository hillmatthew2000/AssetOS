using Spectre.Console;

namespace ITAM.ConsoleApp.Managers;

public class DisplayManager
{
    //Display the main AssetOS header with styling
    public void DisplayAssetOSHeader()
    {
        Console.Clear();

        var panel = new Panel(
            new FigletText("AssetOS")
                .Centered()
                .Color(Spectre.Console.Color.Cyan1))
        {
            Border = BoxBorder.Double,
            BorderStyle = new Style(foreground: Spectre.Console.Color.Cyan1),
            Header = new PanelHeader("[bold yellow]IT Asset Management System[/]"),
            Padding = new Padding(2, 1, 2, 1)
        };

        AnsiConsole.Write(panel);

        var subtitle = new Rule("[bold blue]Phase 2 - SQLite Database Backend[/]")
        {
            Style = Style.Parse("cyan1"),
            Justification = Justify.Center
        };

        AnsiConsole.Write(subtitle);
        AnsiConsole.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
}
