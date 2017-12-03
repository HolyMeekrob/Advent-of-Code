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

	defp get_grid_length(num) do
		num
			|> :math.sqrt
			|> :math.ceil
			|> round
			|> incr
	end

	defp incr(num) do
		num + 1
	end

	defp initialize_grid(length) do
	0
	|> List.duplicate(length)
	|> List.duplicate(length)
	end

	defp get_initial_coordinate(length) do
		val =
			if (rem(length, 2) == 0) do
				div(length - 1, 2)
			else
				div(length, 2)
			end
		{val, val}
	end

		def part_two(num) do
			grid_length = get_grid_length(num)
			{x, y} = get_initial_coordinate(grid_length)
			initial_grid = initialize_grid(grid_length)
			initial_grid = List.update_at(initial_grid, y,
				&(List.update_at(&1, x, fn(_) -> 1 end)))

			initial_grid
			|> build_grid(num, {x + 1, y}, x, y, x, y, :right)
		end

	defp build_grid(grid, cap, {x, y}, min_x, min_y, max_x, max_y, dir) do
		new_val = get_adjacent_sum(grid, {x, y})
		if (new_val > cap) do
			new_val
		else
			updated_grid = List.update_at(grid, y,
				&(List.update_at(&1, x, fn(_) -> new_val end)))

			new_max_x = max(max_x, x)
			new_max_y = max(max_y, y)
			new_min_x = min(min_x, x)
			new_min_y = min(min_y, y)

			next_dir =
				if(new_max_x != max_x
					|| new_max_y != max_y
					|| new_min_x != min_x
					|| new_min_y != min_y) do
					get_next_dir(dir)
				else
					dir
				end

			{next_x, next_y} = get_next_coordinate({x, y}, next_dir)

			build_grid(updated_grid, cap, {next_x, next_y}, new_min_x, new_min_y,
				new_max_x, new_max_y, next_dir)
		end
	end

	defp get_adjacent_sum(grid, {x, y}) do
		get_value_at(grid, {x + 1, y}) +
			get_value_at(grid, {x + 1, y + 1}) +
			get_value_at(grid, {x + 1, y - 1}) +
			get_value_at(grid, {x, y + 1}) +
			get_value_at(grid, {x, y - 1}) +
			get_value_at(grid, {x - 1, y}) +
			get_value_at(grid, {x - 1, y + 1}) +
			get_value_at(grid, {x - 1, y - 1})
	end

	defp get_value_at(_, {x, _}) when (x < 0) do
		0
	end

	defp get_value_at(_, {_, y}) when (y < 0) do
		0
	end

	defp get_value_at(grid, {x, y}) do
		val = case Enum.fetch(grid, y) do
			{:ok, row} ->
				case Enum.fetch(row, x) do
					{:ok, val} -> val
					:error -> 0
				end
			:error -> 0
		end
		val
	end
end

input = 325489
input =
	if (length(System.argv) == 0) do
		input
	else
		String.to_integer(List.first(System.argv))
	end

# Expected answers for default input
# Part one: 552
# Part two: 330785

IO.puts("Input: " <> Integer.to_string(input))
IO.puts("Part one: " <> Integer.to_string(DayThree.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DayThree.part_two(input)))