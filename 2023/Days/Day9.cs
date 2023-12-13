using static _2023.Utils.InputUtils;
using Sequence = System.Collections.Generic.IReadOnlyList<System.Collections.Generic.IReadOnlyList<int>>;

namespace _2023.Days;

public class Day9 : IDay
{
	public (string PartOne, string PartTwo) Run(bool isTest)
	{
		var history = GetAllLines(isTest, 9).Select(GetHistory).ToList();
		return (RunPuzzleOne(history).ToString(), RunPuzzleTwo(history).ToString());
	}

	#region Puzzle one

	private static int RunPuzzleOne(IReadOnlyList<IReadOnlyList<int>> histories) =>
		histories.Select(GetNextStep).Sum();

	private static int GetNextStep(IReadOnlyList<int> history)
	{
		var sequences = BuildSequences([history]);
		sequences = AddNextPrediction(sequences);
		return sequences.Last().Last();
	}

	private static Sequence AddNextPrediction(Sequence sequences)
	{
		var predictedSequences = new List<List<int>>();
		for (var i = sequences.Count - 1; i >= 0; --i)
		{
			var predictedSequence = sequences[i].ToList();
			if (i == sequences.Count - 1)
			{
				predictedSequence.Add(0);
			}
			else
			{
				predictedSequence.Add(predictedSequence.Last() + predictedSequences.Last().Last());
			}
			predictedSequences.Add(predictedSequence);
		}

		return predictedSequences.ToList();
	}

	#endregion Puzzle one

	#region Puzzle two

	private static int RunPuzzleTwo(IReadOnlyList<IReadOnlyList<int>> histories) =>
		histories.Select(GetPreviousStep).Sum();

	private static int GetPreviousStep(IReadOnlyList<int> history)
	{
		var sequences = BuildSequences([history]);
		var updatedSequences = AddPreviousPrediction(sequences);
		return updatedSequences.First().First();
	}

	private static LinkedList<LinkedList<int>> AddPreviousPrediction(Sequence sequences)
	{
		var predictedSequences = new LinkedList<LinkedList<int>>();
		for (var i = sequences.Count - 1; i >= 0; --i)
		{
			var predictedSequence = new LinkedList<int>(sequences[i]);
			if (i == sequences.Count - 1)
			{
				predictedSequence.AddFirst(0);
			}
			else
			{
				predictedSequence.AddFirst(
					predictedSequence.First() - predictedSequences.First().First()
				);
			}
			predictedSequences.AddFirst(predictedSequence);
		}

		return predictedSequences;
	}

	#endregion Puzzle two

	#region Common

	private static List<int> GetHistory(string line) =>
		line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

	private static Sequence BuildSequences(Sequence sequences)
	{
		var lastSequence = sequences.Last();
		if (lastSequence.All(x => x == 0))
		{
			return sequences;
		}

		var nextSequence = new List<int>();
		for (var i = 0; i < lastSequence.Count - 1; ++i)
		{
			nextSequence.Add(lastSequence[i + 1] - lastSequence[i]);
		}

		return BuildSequences(sequences.Append(nextSequence).ToList());
	}

	#endregion Common
}
