defmodule DayTwentyFour do
	def parse(line) do
		regex = ~r/(\d+)\/(\d+)/
		[_, a, b] = Regex.run(regex, line)
		{String.to_integer(a), String.to_integer(b)}
	end

	def part_one(input) do
		input
		|> build_bridges(0)
		|> Enum.map(&bridge_value/1)
		|> Enum.max
	end

	defp has_port?({a, b}, port) do
		(a === port) or (b === port)
	end

	defp build_bridges(components, port) do
		possible = Enum.filter(components, &(has_port?(&1, port)))

		if (length(possible) === 0) do
			[]
		else
			build_possible =
				fn(comp) ->
					tails = 
						components
						|> Enum.reject(&(&1 === comp))
						|> build_bridges(other_port(comp, port))

					if (Enum.empty?(tails)) do
						[[comp]]
					else
						Enum.map(tails, &([comp | &1]))
					end
				end
			
			possible
			|> Enum.map(build_possible)
			|> flatten
		end
	end

	defp other_port({a, b}, port) do
		if (a === port) do
			b
		else
			a
		end
	end

	defp bridge_value(components) do
		List.foldl(components, 0, &component_value/2)
	end

	defp component_value({a, b}, total) do
		total + a + b
	end

	defp flatten(lst) do
		List.foldl(lst, [], &(&2 ++ &1))
	end
end

input = 
	"input/day24.txt"
	|> File.read!
	|> String.split("\r\n")
	|> Enum.map(&DayTwentyFour.parse/1)


# Expected answers for default input
# Part one: 1511
# Part two: 

IO.puts("Part one: " <> Integer.to_string(DayTwentyFour.part_one(input)))