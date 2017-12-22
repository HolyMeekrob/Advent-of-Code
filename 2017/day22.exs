defmodule State do
	defstruct [:pos, :dir, :points, :infections]
end

defmodule DayTwentyTwo do
	@directions [:up, :right, :down, :left]

	def parse_grid(grid) do
		{_, points} = List.foldl(grid, {0, %{}}, &append_row/2)
		{length(grid), points}
	end

	defp append_row(row, {idx, points}) do
		row
		|> String.to_charlist
		|> List.foldl({0, []}, &append_point/2)
		|> fn({_, cols}) -> {idx + 1, Map.put(points, idx, cols)} end.()
	end

	defp append_point(?#, {col, cols}) do
		{col + 1, [col | cols]}
	end

	defp append_point(?., {col, cols}) do
		{col + 1, cols}
	end

	def part_one(points, pos) do
		init(points, pos)
		|> step(10000)
		|> Map.fetch!(:infections)
	end

	defp init(points, pos) do
		%State{pos: pos, dir: :up, points: points, infections: 0}
	end

	defp step(%State{} = state, 0) do
		state
	end

	defp step(%State{} = state, count) do
		step(work(state), count - 1)
	end

	defp work(%State{} = state) do
		state
		|> turn
		|> act
		|> move
	end

	defp turn(%State{pos: pos, dir: dir, points: points} = state) do
		new_dir =
			if (infected?(pos, points)) do
				turn_right(dir)
			else
				turn_left(dir)
			end

		%State{state | dir: new_dir}
	end

	defp infected?({y, x}, points) do
			Map.has_key?(points, y) and
				points
				|> Map.fetch!(y)
				|> Enum.member?(x)
	end

	defp direction_at(i) do
		Enum.at(@directions, i)
	end

	defp turn_right(dir) do
		@directions
		|> Enum.find_index(fn(d) -> d === dir end)
		|> Kernel.+(1)
		|> rem(4)
		|> direction_at
	end

	defp turn_left(dir) do
		@directions
		|> Enum.find_index(fn(d) -> d === dir end)
		|> Kernel.-(1)
		|> direction_at
	end

	defp act(%State{pos: {y, x} = pos, points: points, infections: count} = state) do
		cols = case Map.fetch(points, y) do
			{:ok, nums} -> nums
			:error -> []
		end
	
		{new_points, new_count} =
			if (infected?(pos, points)) do
				{Map.put(points, y, Enum.reject(cols, &(&1 === x))), count}
			else
				{Map.put(points, y, [x | cols]), count + 1}
			end
		%State{state | points: new_points, infections: new_count}
	end

	defp move(%State{pos: {y, x}, dir: :up} = state) do
		%State{state | pos: {y - 1, x}}
	end

	defp move(%State{pos: {y, x}, dir: :down} = state) do
		%State{state | pos: {y + 1, x}}
	end

	defp move(%State{pos: {y, x}, dir: :left} = state) do
		%State{state | pos: {y, x - 1}}
	end

	defp move(%State{pos: {y, x}, dir: :right} = state) do
		%State{state | pos: {y, x + 1}}
	end
end

{size, points} =
	"input/day22.txt"
	|> File.read!
	|> String.split("\r\n")
	|> DayTwentyTwo.parse_grid

# Expected answers for default input
# Part one: 5447
# Part two: 

mid = div(size, 2)
IO.puts("Part one: " <> Integer.to_string(DayTwentyTwo.part_one(points, {mid, mid})))