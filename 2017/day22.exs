defmodule StateOne do
	defstruct [:pos, :dir, :points, :infections]
end

defmodule DayTwentyTwoA do
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

	def run(points, pos) do
		init(points, pos)
		|> step(10000)
		|> Map.fetch!(:infections)
	end

	defp init(points, pos) do
		%StateOne{pos: pos, dir: :up, points: points, infections: 0}
	end

	defp step(%StateOne{} = state, 0) do
		state
	end

	defp step(%StateOne{} = state, count) do
		step(work(state), count - 1)
	end

	defp work(%StateOne{} = state) do
		state
		|> turn
		|> act
		|> move
	end

	defp turn(%StateOne{pos: pos, dir: dir, points: points} = state) do
		new_dir =
			if (infected?(pos, points)) do
				turn_right(dir)
			else
				turn_left(dir)
			end

		%StateOne{state | dir: new_dir}
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

	defp act(%StateOne{pos: {y, x} = pos, points: points, infections: count} = state) do
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
		%StateOne{state | points: new_points, infections: new_count}
	end

	defp move(%StateOne{pos: {y, x}, dir: :up} = state) do
		%StateOne{state | pos: {y - 1, x}}
	end

	defp move(%StateOne{pos: {y, x}, dir: :down} = state) do
		%StateOne{state | pos: {y + 1, x}}
	end

	defp move(%StateOne{pos: {y, x}, dir: :left} = state) do
		%StateOne{state | pos: {y, x - 1}}
	end

	defp move(%StateOne{pos: {y, x}, dir: :right} = state) do
		%StateOne{state | pos: {y, x + 1}}
	end
end



defmodule StateTwo do
	defstruct [:pos, :dir, :weakened, :infected, :flagged, :infections]
end

defmodule DayTwentyTwoB do
	@directions [:up, :right, :down, :left]

	def run(infected, pos) do
		init(infected, pos)
		|> step(10000000)
		|> Map.fetch!(:infections)
	end

	defp init(infected, pos) do
		%StateTwo{pos: pos, dir: :up, infected: infected, weakened: %{}, flagged: %{}, infections: 0}
	end

	defp step(%StateTwo{} = state, 0) do
		state
	end

	defp step(%StateTwo{} = state, count) do
		step(work(state), count - 1)
	end

	defp work(%StateTwo{} = state) do
		state
		|> turn
		|> act
		|> move
	end

	defp turn(%StateTwo{pos: pos, dir: dir, infected: infected, weakened: weakened, flagged: flagged} = state) do
		new_dir =
			cond do
				found?(pos, infected) -> turn_right(dir)
				found?(pos, weakened) -> dir
				found?(pos, flagged) -> reverse_direction(dir)
				true -> turn_left(dir)
			end

		%StateTwo{state | dir: new_dir}
	end

	defp found?({y, x}, points) do
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

	defp reverse_direction(dir) do
		@directions
		|> Enum.find_index(fn(d) -> d === dir end)
		|> Kernel.+(2)
		|> rem(4)
		|> direction_at
	end

	defp act(%StateTwo{pos: {y, x} = pos, infected: infected, weakened: weakened, flagged: flagged, infections: count} = state) do
		infected_cols = Map.get(infected, y, [])
		weakened_cols = Map.get(weakened, y, [])
		flagged_cols = Map.get(flagged, y, [])

		is_x = fn(a) -> a === x end
	
		{new_infected, new_weakened, new_flagged, new_count} =
			cond do
				found?(pos, infected) ->
					{
						Map.put(infected, y, Enum.reject(infected_cols, is_x)),
						weakened,
						Map.put(flagged, y, [x | flagged_cols]),
						count
					}

				found?(pos, weakened) ->
					{
						Map.put(infected, y, [x | infected_cols]),
						Map.put(weakened, y, Enum.reject(weakened_cols, is_x)),
						flagged,
						count + 1
					}

					found?(pos, flagged) ->
						{
							infected,
							weakened,
							Map.put(flagged, y, Enum.reject(flagged_cols, is_x)),
							count
						}

					true ->
						{
							infected,
							Map.put(weakened, y, [x | weakened_cols]),
							flagged,
							count
						}
			end

		%StateTwo{state | infected: new_infected, weakened: new_weakened, flagged: new_flagged, infections: new_count}
	end

	defp move(%StateTwo{pos: {y, x}, dir: :up} = state) do
		%StateTwo{state | pos: {y - 1, x}}
	end

	defp move(%StateTwo{pos: {y, x}, dir: :down} = state) do
		%StateTwo{state | pos: {y + 1, x}}
	end

	defp move(%StateTwo{pos: {y, x}, dir: :left} = state) do
		%StateTwo{state | pos: {y, x - 1}}
	end

	defp move(%StateTwo{pos: {y, x}, dir: :right} = state) do
		%StateTwo{state | pos: {y, x + 1}}
	end
end

{size, points} =
	"input/day22.txt"
	|> File.read!
	|> String.split("\r\n")
	|> DayTwentyTwoA.parse_grid

# Expected answers for default input
# Part one: 5447
# Part two: 2511705

mid = div(size, 2)
IO.puts("Part one: " <> Integer.to_string(DayTwentyTwoA.run(points, {mid, mid})))

# Warning: slow!
IO.puts("Part two: " <> Integer.to_string(DayTwentyTwoB.run(points, {mid, mid})))