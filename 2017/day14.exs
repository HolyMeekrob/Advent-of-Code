defmodule DayFourteen do
	def run(input) do
		grid =
			0..127
			|> Enum.map(&(input <> "-" <> Integer.to_string(&1)))
			|> Enum.map(&String.to_charlist/1)
			|> Enum.map(&DayTen.part_two/1)
			|> Enum.map(&(to_strings(&1, 2, 8)))
		{part_one(grid), part_two(grid)}
	end

	defp part_one(grid) do
		List.foldl(grid, 0, &count_ones/2)
	end

	defp part_two(grid) do
		grid
		|> List.foldl(%{y_index: 0, coordinates: []}, &populate_row/2)
		|> Map.fetch!(:coordinates)
		|> count_regions
	end

	defp to_strings(nums, base, pad_count) do
		nums
		|> Enum.map(&(Integer.to_string(&1, base)))
		|> Enum.map(&(String.pad_leading(&1, pad_count, "0")))
	end

	defp count_ones(nums, sum) do
		nums
		|> Enum.join
		|> String.replace("0", "")
		|> String.length
		|> Kernel.+(sum)
	end

	defp populate_row(row, %{y_index: y, coordinates: coordinates} = data) do
		row_coordinates =
			row
			|> Enum.join
			|> String.split("", trim: true)
			|> Enum.map(&String.to_integer/1)
			|> List.foldl(%{current_index: 0, matched_indices: []}, &record_ones/2)
			|> Map.fetch!(:matched_indices)
			|> Enum.map(&({&1, y}))
		
		%{data | y_index: y + 1, coordinates: coordinates ++ row_coordinates}
	end

	defp record_ones(0, %{current_index: i} = acc) do
		%{acc | current_index: i + 1}
	end

	defp record_ones(1, %{current_index: i, matched_indices: matches}) do
		%{current_index: i + 1, matched_indices: [i | matches]}
	end

	defp count_regions(unclaimed_coordinates, count \\ 0)

	defp count_regions([], count) do
		count
	end

	defp count_regions(unclaimed_coordinates, count) do
		[start | other_coordinates] = unclaimed_coordinates
		count_regions(gather_region(start, other_coordinates), count + 1)
	end

	defp gather_region({x, y}, unclaimed_coordinates) do
		possible_points =
			[
				{x - 1, y},
				{x + 1, y},
				{x, y - 1},
				{x, y + 1},
			]

		included_points = Enum.filter(unclaimed_coordinates, &(Enum.member?(possible_points, &1)))
		unclaimed_coordinates = Enum.reject(unclaimed_coordinates, &(Enum.member?(possible_points, &1)))

		List.foldl(included_points, unclaimed_coordinates, &gather_region/2)
	end
end


# Expected answers for default input
# Part one:8074
# Part two: 1212

Code.load_file("day10.exs")
input = "jzgqcdpd"
output = DayFourteen.run(input)

IO.puts("")
IO.puts("Part one: " <> Integer.to_string(elem(output, 0)))
IO.puts("Part two: " <> Integer.to_string(elem(output, 1)))
