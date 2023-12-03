using System.Drawing;
using System.Text.RegularExpressions;
using static _2023.Utils.InputUtils;
using Reveal = System.Collections.ObjectModel.ReadOnlyDictionary<System.Drawing.Color, int>;

namespace _2023.Days;

public sealed partial class Day2 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var games = GetAllLines(isTest, 2).Select(CreateGame).ToList();
		return (RunPuzzleOne(games).ToString(), RunPuzzleTwo(games).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(IReadOnlyCollection<Game> games)
	{
		var contents = new Reveal(
			new Dictionary<Color, int>
			{
				{ Color.Red, 12 },
				{ Color.Green, 13 },
				{ Color.Blue, 14 },
			}
		);

		return games.Where(game => IsPossible(contents, game)).Select(game => game.Id).Sum();
	}

	private static bool IsPossible(Reveal contents, Game game) =>
		game.Reveals.All(reveal => IsPossible(contents, reveal));

	private static bool IsPossible(Reveal contents, Reveal reveal)
	{
		contents.TryGetValue(Color.Red, out var redContents);
		reveal.TryGetValue(Color.Red, out var redReveal);
		contents.TryGetValue(Color.Green, out var greenContents);
		reveal.TryGetValue(Color.Green, out var greenReveal);
		contents.TryGetValue(Color.Blue, out var blueContents);
		reveal.TryGetValue(Color.Blue, out var blueReveal);

		return redContents >= redReveal
			&& greenContents >= greenReveal
			&& blueContents >= blueReveal;
	}

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(IReadOnlyCollection<Game> games) =>
		games.Select(GetPower).Sum();

	private static int GetPower(Game game) =>
		GetRequiredCount(Color.Red, game.Reveals)
		* GetRequiredCount(Color.Green, game.Reveals)
		* GetRequiredCount(Color.Blue, game.Reveals);

	private static int GetRequiredCount(Color color, IReadOnlyCollection<Reveal> reveals) =>
		reveals
			.Select(reveal =>
			{
				reveal.TryGetValue(color, out var count);
				return count;
			})
			.Max();

	#endregion Puzzle two

	[GeneratedRegex(@"^Game (?<Id>\d+): (?<Reveals>.*)$", RegexOptions.Compiled)]
	private static partial Regex GameRegex();

	[GeneratedRegex(@"(?<Count>\d+) (?<Color>(?:red|green|blue))")]
	private static partial Regex RevealColorRegex();

	private static Game CreateGame(string line)
	{
		var match = GameRegex().Match(line);
		var id = int.Parse(match.Groups["Id"].ValueSpan);
		var reveals = match.Groups["Reveals"].Value.Split(";").Select(CreateReveal).ToList();
		return new Game(id, reveals);
	}

	private static Reveal CreateReveal(string text) =>
		new(
			text.Trim()
				.Split(",")
				.Select(CreateRevealColor)
				.ToDictionary(revealColor => revealColor.Color, revealColor => revealColor.Count)
		);

	private static RevealColor CreateRevealColor(string text)
	{
		var match = RevealColorRegex().Match(text);
		var count = int.Parse(match.Groups["Count"].ValueSpan);
		var color = Color.FromName(match.Groups["Color"].Value);
		return new RevealColor(count, color);
	}

	private sealed record Game(int Id, IReadOnlyCollection<Reveal> Reveals);

	private sealed record RevealColor(int Count, Color Color);
}
