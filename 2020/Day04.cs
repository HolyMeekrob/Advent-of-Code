using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Advent._2020.Utils;

namespace Advent._2020
{
	class Day04 : IDay
	{
		private static readonly Passport[] Input = string.Join("\n", File.ReadAllLines(@".\Day04.txt"))
			.Split($"\n\n")
			.Select(input => new Passport(input))
			.ToArray();

		public (string, string) GetResult() => (GetPartOne(), GetPartTwo());

		private static string GetPartOne() => Input.Count(passport => passport.IsValidPartOne()).ToString();

		private static string GetPartTwo() => Input.Count(passport => passport.IsValidPartTwo()).ToString();

		private record Passport
		{
			public int? CountryId { get; init; }
			public int? BirthYear { get; init; }
			public int? IssueYear { get; init; }
			public int? ExpirationYear { get; init; }
			public Height Height { get; init; }
			public string HairColor { get; init; }
			public string EyeColor { get; init; }
			public string PassportId { get; init; }

			public Passport(string input) : this(
				Regex.Matches(input, @"([a-z]{3}):([^\s]+)")
					.ToDictionary(
						match => match.Groups[1].Value,
						match => match.Groups[2].Value
					))
			{
			}

			public Passport(IDictionary<string, string> input)
			{
				int? GetInt(string key) => input.ContainsKey(key) ? Convert.ToInt32(input[key]) : null;
				string GetString(string key) => input.ContainsKey(key) ? input[key] : null;
				Height GetLength(string key) => input.ContainsKey(key) ? new Height(input[key]) : null;

				CountryId = GetInt("cid");
				BirthYear = GetInt("byr");
				IssueYear = GetInt("iyr");
				ExpirationYear = GetInt("eyr");
				Height = GetLength("hgt");
				HairColor = GetString("hcl");
				EyeColor = GetString("ecl");
				PassportId = GetString("pid");
			}

			public bool IsValidPartOne() =>
				BirthYear.HasValue
				&& IssueYear.HasValue
				&& ExpirationYear.HasValue
				&& Height != null
				&& HairColor != null
				&& EyeColor != null
				&& PassportId != null;

			public bool IsValidPartTwo() =>
				IsValidPartOne()
					&& Between(1920, 2002, BirthYear.Value)
					&& Between(2010, 2020, IssueYear.Value)
					&& Between(2020, 2030, ExpirationYear.Value)
					&& Height.IsValid()
					&& IsValidColor(HairColor)
					&& EyeColors.Contains(EyeColor)
					&& IsValidPassportId(PassportId);
		}

		private static Unit GetUnit(string str)
		{
			switch (str)
			{
				case "in":
					return Unit.Inch;
				case "cm":
					return Unit.Centimeter;
				default:
					return Unit.Unknown;
			}
		}

		private static bool IsValidPassportId(string input) => Regex.Match(input, @"^[0-9]{9}$").Success;
		private static bool IsValidColor(string input) => Regex.Match(input, @"^#[0-9a-f]{6}$").Success;
		private static string[] EyeColors = new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };

		private record Height
		{
			public int Length { get; init; }
			public Unit Units { get; init; }

			public Height(string input)
			{
				var match = Regex.Match(input, @"(\d+)(\w+)");
				Length = Convert.ToInt32(match.Groups[1].Value);
				Units = GetUnit(match.Groups[2].Value);
			}

			public bool IsValid() =>
				(Units == Unit.Inch && Between(59, 76, Length))
				|| (Units == Unit.Centimeter && Between(150, 193, Length));
		}

		private enum Unit
		{
			Unknown,
			Centimeter,
			Inch
		}
	}
}
