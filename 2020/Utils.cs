using System.Linq;

namespace Advent._2020
{
	public static class Utils
	{
		public static T Identity<T>(T x) => x;

		public static int Product(params int[] nums) => nums.Aggregate((x, y) => x * y);
		public static uint Product(params uint[] nums) => nums.Aggregate((x, y) => x * y);
		public static bool Between(int start, int end, int num) => num >= start && num <= end;
	}
}
