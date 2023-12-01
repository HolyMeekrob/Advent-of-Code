namespace _2022;

public static class EnumerableExtensions
{
	public static bool IsEmpty<T>(this IEnumerable<T> source) => !source.Any();
	public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => !source.Any(predicate);
}

public static class StringExtensions
{
	public static bool IsEmpty(this string str) => str == string.Empty;
}

public static class FunctionalUtils
{
	public static T Identity<T>(T x) => x;
	public static Func<T, bool> Complement<T>(Func<T, bool> f) => x => !f(x);

	public static Func<T, bool> All<T>(params Func<T, bool>[] fs) =>
		x => fs.Aggregate(true, (all, f) => all && f(x));
}
