defmodule DayEleven do
	def run(dirs) do
		end_position = List.foldl(dirs, {{0, 0}, 0}, &move_and_record/2)
		{get_distance(elem(end_position, 0)), elem(end_position, 1)}
	end

	defp move_and_record(dir, data) do
		new_pos = move(dir, elem(data, 0))
		{new_pos, max(elem(data, 1), get_distance(new_pos))}
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

	defp get_distance(position) do
		{x, y} = absolute_coordinate(position)
		case y - div(x, 2) do
			n when n < 0 -> div(x, 2)
			n -> div(x, 2) + div(n, 2)
		end
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
# Part two: 1585

result = DayEleven.run(input)
IO.puts("Part one: " <> Integer.to_string(elem(result, 0)))
IO.puts("Part two: " <> Integer.to_string(elem(result, 1)))