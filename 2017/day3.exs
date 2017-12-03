defmodule DayThree do
	def part_one(num) do
		num
		|> get_relative_coordinate({0, 0}, 0, 0, 0, 0, :right)
		|> get_distance
	end

	defp get_distance({x, y}) do
		abs(x) + abs(y)
	end

	defp get_relative_coordinate(1, {x, y}, _, _, _, _, _) do
		{x, y}
	end

	defp get_relative_coordinate(count, curr, min_x, max_x, min_y, max_y, dir) do
		{x, y} = get_next_coordinate(curr, dir)
		new_max_x = max(max_x, x)
		new_max_y = max(max_y, y)
		new_min_x = min(min_x, x)
		new_min_y = min(min_y, y)

		dir =
			if(new_max_x != max_x
				|| new_max_y != max_y
				|| new_min_x != min_x
				|| new_min_y != min_y) do
				get_next_dir(dir)
			else
				dir
			end

		get_relative_coordinate(count - 1, {x, y}, new_min_x,
			new_max_x, new_min_y, new_max_y, dir)
	end

	defp get_next_coordinate({x, y}, :right) do
		{x + 1, y}
	end

	defp get_next_coordinate({x, y}, :up) do
		{x, y + 1}
	end

	defp get_next_coordinate({x, y}, :left) do
		{x - 1, y}
	end

	defp get_next_coordinate({x, y}, :down) do
		{x, y - 1}
	end

	defp get_next_dir(:right) do
		:up
	end

	defp get_next_dir(:up) do
		:left
	end

	defp get_next_dir(:left) do
		:down
	end

	defp get_next_dir(:down) do
		:right
	end
end

input = 325489
input =
	if (length(System.argv) == 0) do
		input
	else
		String.to_integer(List.first(System.argv))
	end

IO.puts("Input: " <> Integer.to_string(input))
IO.puts("Part one: " <> Integer.to_string(DayThree.part_one(input)))