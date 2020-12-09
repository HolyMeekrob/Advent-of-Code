using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Advent._2020.Utils;

namespace Advent._2020
{
	class Day07 : IDay
	{
		private static readonly Bag[] Input = File.ReadAllLines(@".\Day07.txt")
			.Select(CreateBag)
			.ToArray();

		public (string, string) GetResult() => (GetPartOne(), GetPartTwo());

		private static string GetPartOne() => GetAllBagsContaining("shiny gold", new HashSet<Bag>()).Count.ToString();

		private static string GetPartTwo() => GetAllContainedBags(GetBag("shiny gold")).Count.ToString();

		private static HashSet<Bag> GetAllBagsContaining(string bagName, HashSet<Bag> containers)
		{
			var newContainers = Input.Where(bag => bag.Rules.Any(rule => rule.BagName == bagName)).ToHashSet().Except(containers);
			if (newContainers.IsEmpty())
			{
				return containers;
			}

			var allContainers = newContainers.Union(containers).ToHashSet();

			foreach (var container in newContainers)
			{
				allContainers = GetAllBagsContaining(container.Name, allContainers);
			}

			return allContainers;
		}

		private static List<Bag> GetAllContainedBags(Bag bag)
		{
			static List<Bag> GetBagsForRule(Rule rule)
			{
				var ruleBag = GetBag(rule.BagName);
				var perBag = new[] { ruleBag }.Concat(GetAllContainedBags(ruleBag));
				return Enumerable.Repeat(perBag, rule.Num).SelectMany(Identity).ToList();
			}

			var innerBags = bag.Rules.SelectMany(GetBagsForRule).ToList();
			return bag.Rules.SelectMany(GetBagsForRule).ToList();
		}

		private static Bag GetBag(string bagName) => Input.First(bag => bag.Name == bagName);

		private static Bag CreateBag(string input)
		{
			var groups = Regex.Match(input, @"(\w+(?: \w+)*) bags contain (.+)\.").Groups;
			var rules = groups[2].Value == "no other bags"
				? Array.Empty<Rule>()
				: groups[2].Value.Split(", ").Select(CreateRule).Where(rule => rule != null).ToArray();

			return new Bag(groups[1].Value, rules);
		}

		private static Rule CreateRule(string input)
		{
			var groups = Regex.Match(input, @"(\d+) (\w+(?: \w+)*) bags?").Groups;
			return new Rule(groups[2].Value, Convert.ToInt32(groups[1].Value));
		}

		public record Rule(string BagName, int Num)
		{
			public override string ToString() => $"{Num} {BagName}{(Num == 1 ? "" : "s")}";
		}

		public record Bag(string Name, Rule[] Rules)
		{
			public override string ToString() => $"{Name} requires {string.Join("; ", Rules.ToList())}";
		}
	}
}
