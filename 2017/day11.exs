defmodule DayEleven do
	def part_one(dirs) do
		dirs
		|> List.foldl({0, 0}, &move/2)
		|> absolute_coordinate
		|> get_distance(0)
	end

	defp move("n", {x, y}) do
		{x, y + 2}
	end

	defp move("s", {x, y}) do
		{x, y - 2}
	end

	defp move("ne", {x, y}) do
		{x + 2, y + 1}
	end

	defp move("nw", {x, y}) do
		{x - 2, y + 1}
	end

	defp move("se", {x, y}) do
		{x + 2, y - 1}
	end

	defp move("sw", {x, y}) do
		{x - 2, y - 1}
	end

	defp get_distance({0, 0}, steps) do
		steps
	end

	defp get_distance({x, 0}, steps) do
		get_distance({x - 2, 1}, steps + 1)
	end

	defp get_distance({0, y}, steps) do
		get_distance({0, y - 2}, steps + 1)
	end

	defp get_distance({x, y}, steps) do
		get_distance({x - 2, y - 1}, steps + 1)
	end

	defp absolute_coordinate({x, y}) do
		{abs(x), abs(y)}
	end
end

input =
	if (length(System.argv) == 0) do
		"input/day11.txt"
			|> File.read!
			|> String.split(",")
	else
		System.argv
		|> List.first
		|> String.split(",")
	end

# Expected answers for default input
# Part one: 796
# Part two: 

IO.puts("Part one: " <> Integer.to_string(DayEleven.part_one(input)))