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
}
