defmodule DayTwelve do
		def parse_instruction(line, map) do
			regex = ~r/(\d+) <\-> (.+)/
			[_, source, destinations] = Regex.run(regex, line)
			destinations =
				destinations
				|> String.split(", ")
				|> Enum.map(&String.to_integer/1)

			Map.put(map, String.to_integer(source), destinations)
	end

	def part_one(map) do
		map
		|> get_connected_programs(0, MapSet.new)
		|> MapSet.size
	end

	def part_two(map) do
		all_programs =
			map
			|> Map.keys
			|> MapSet.new
		
		count_groups(map, all_programs, 0)
	end

	defp count_groups(map, remaining_programs, counted_groups) do
		if (MapSet.size(remaining_programs) === 0) do
			counted_groups
		else
			starting_program =
				remaining_programs
				|> MapSet.to_list
				|> List.first
			next_group = get_connected_programs(map, starting_program, MapSet.new)
			count_groups(map, MapSet.difference(remaining_programs, next_group), counted_groups + 1)
		end
	end

	defp get_connected_programs(map, value, counted) do
		if (MapSet.member?(counted, value)) do
			counted
		else
			counted = MapSet.put(counted, value)
			List.foldl(map[value], counted, &(get_connected_programs(map, &1, &2)))
		end
	end
end


input =
	"input/day12.txt"
		|> File.read!
		|> String.split("\r\n")
		|> List.foldl(%{}, &DayTwelve.parse_instruction/2)

# Expected answers for default input
# Part one: 306
# Part two: 200
IO.puts("Part one: " <> Integer.to_string(DayTwelve.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DayTwelve.part_two(input)))