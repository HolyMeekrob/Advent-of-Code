using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent._2020
{
	class Day08 : IDay
	{
		private static readonly Instruction[] Input = File.ReadAllLines(@".\Day08.txt")
			.Select(GetInstruction)
			.ToArray();

		public (string, string) GetResult() => (GetPartOne(), GetPartTwo());

		private static string GetPartOne() => RunInstruction(Input, 0, 0, new HashSet<int>()).Result.ToString();

		private static string GetPartTwo()
		{
			var swappableIndexes = Input
				.Where(instruction => instruction.Type != InstructionType.ACC)
				.Select(instruction => instruction.Index)
				.ToList();

			var completedNormally = false;
			int accumulator = 0; ;
			Instruction[] adjustedInput;
			for (int i = 0; !completedNormally && i < swappableIndexes.Count; ++i)
			{
				accumulator = 0;
				adjustedInput = AdjustInput(swappableIndexes[i]);

				(accumulator, completedNormally) = RunInstruction(adjustedInput, 0, 0, new HashSet<int>());
			}

			return accumulator.ToString();
		}

		private static (int Result, bool CompletedNormally) RunInstruction(Instruction[] input, int index, int accumulator, HashSet<int> visitedIndexes)
		{
			if (index == input.Length)
			{
				return (accumulator, true);
			}

			if (!visitedIndexes.Add(index))
			{
				return (accumulator, false);
			}

			var instruction = input[index];
			if (instruction.Type == InstructionType.NOP)
			{
				return RunInstruction(input, index + 1, accumulator, visitedIndexes);
			}

			if (instruction.Type == InstructionType.JMP)
			{
				return RunInstruction(input, index + instruction.Value, accumulator, visitedIndexes);
			}

			return RunInstruction(input, index + 1, accumulator + instruction.Value, visitedIndexes);
		}

		private static Instruction GetInstruction(string input, int index)
		{
			var groups = Regex.Match(input, @"(nop|acc|jmp) ([+-])(\d+)").Groups;
			var operation = GetInstructionType(groups[1].Value);
			var sign = groups[2].Value;
			var num = Convert.ToInt32(groups[3].Value);

			return new Instruction(operation, sign == "+" ? num : -num, index);
		}
		private static InstructionType GetInstructionType(string input) =>
			(InstructionType)Enum.Parse(typeof(InstructionType), input.ToUpper());

		private static Instruction[] AdjustInput(int index)
		{
			var result = new Instruction[Input.Length];
			Input.CopyTo(result, 0);
			result[index] = Swap(result[index]);
			return result;
		}

		private static Instruction Swap(Instruction instruction)
		{
			if (instruction.Type == InstructionType.JMP)
			{
				return instruction with { Type = InstructionType.NOP };
			}

			if (instruction.Type == InstructionType.NOP)
			{
				return instruction with { Type = InstructionType.JMP };
			}

			return instruction;
		}

		private record Instruction(InstructionType Type, int Value, int Index)
		{
			public override string ToString() => $"{Type}: {Value} @{Index}";
		}

		private enum InstructionType
		{
			NOP,
			ACC,
			JMP
		}
	}
}
