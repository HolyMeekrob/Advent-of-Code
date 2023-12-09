namespace _2023.Utils;

public static class EnumerableExtensions
{
	/// <summary>
	/// Identifies whether the given IEnumerable sequence is empty.
	/// </summary>
	/// <param name="source">The IEnumerable sequence to check for emptiness.</param>
	/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
	/// <returns>True if the sequence is empty, false otherwise.</returns>
	public static bool IsEmpty<T>(this IEnumerable<T> source) => !source.Any();

	/// <summary>
	/// Checks if none of the elements of a sequence satisfy a given condition.
	/// </summary>
	/// <param name="source">The IEnumerable sequence to check.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
	/// <returns>True if no elements in the sequence satisfy the condition, false otherwise.</returns>
	public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
		!source.Any(predicate);

	/// <summary>
	/// Calculates the product of all the integers in the given sequence.
	/// </summary>
	/// <param name="source">The sequence of integers.</param>
	/// <returns>The product of all the integers in the sequence.</returns>
	public static int Product(this IEnumerable<int> source) => source.Aggregate((x, y) => x * y);
}
