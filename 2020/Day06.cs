using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent._2020
{
	class Day06 : IDay
	{
		private static readonly Group[] Input = File.ReadAllText(@".\Day06.txt")
			.Split("\n\n")
			.Select(CreateGroup)
			.ToArray();

		public (string, string) GetResult() => (GetPartOne(), GetPartTwo());

		private static string GetPartOne() => Input
			.Select(GetQuestionsAnsweredByAnyone)
			.Sum(answers => answers.Length)
			.ToString();

		private static string GetPartTwo() => Input
			.Select(GetQuestionsAnsweredByEveryone)
			.Sum(answers => answers.Length)
			.ToString();

		private static Group CreateGroup(string input) => new Group(input
			.Split("\n")
			.Where(str => !string.IsNullOrWhiteSpace(str))
			.Select(CreatePerson)
			.ToArray());

		private static Person CreatePerson(string input) => new Person(input.ToHashSet());

		private static char[] GetQuestionsAnsweredByAnyone(Group group) => group
			.People
			.Aggregate(new HashSet<char>(), (answers, person) => answers.Union(person.Answers).ToHashSet())
			.ToArray();

		private static char[] GetQuestionsAnsweredByEveryone(Group group) => group
			.People
			.Aggregate(group.People.First().Answers, (answers, person) => answers.Intersect(person.Answers).ToHashSet())
			.ToArray();

		private record Group(Person[] People);

		private record Person(HashSet<char> Answers);
	}
}
