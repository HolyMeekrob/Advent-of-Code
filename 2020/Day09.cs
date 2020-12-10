using System;
using System.IO;
using System.Linq;

namespace Advent._2020
{
	class Day09 : IDay
	{
		private static readonly ulong[] Input = File.ReadAllLines(@".\Day09.txt")
			.Where(line => !string.IsNullOrWhiteSpace(line))
			.Select(line => Convert.ToUInt64(line))
			.ToArray();

		private const int SequenceSize = 25;

		public (string, string) GetResult()
		{
			var firstInvalidNum = GetPartOne();
			return (firstInvalidNum.ToString(), GetPartTwo(firstInvalidNum).ToString());
		}

		private static ulong GetPartOne() => GetNextInvalidNumber(0);

		private static ulong GetPartTwo(ulong target)
		{
			var (index, range) = SumToTarget(target);
			var sequence = Input.Skip(index).Take(range).ToArray();
			return sequence.Min() + sequence.Max();
		}

		private static ulong GetNextInvalidNumber(int index)
		{
			var range = Input.Skip(index).Take(SequenceSize).ToArray();
			var check = Input[index + SequenceSize];

			return IsValid(range, check) ? GetNextInvalidNumber(index + 1) : check;
		}

		private static bool IsValid(ulong[] range, ulong check)
		{
			if (range.Length <= 1)
			{
				return false;
			}

			var target = check - range[0];
			var newRange = range.Skip(1).ToArray();
			return newRange.Contains(target) || IsValid(newRange, check);
		}

		private static (int index, int range) SumToTarget(ulong target, int index = 0)
		{
			ulong sum = 0;
			int range = 0;

			while (sum < target)
			{
				sum += Input[index + range];
				range++;
			}

			return sum == target ? (index, range) : SumToTarget(target, index + 1);
		}
	}
}
