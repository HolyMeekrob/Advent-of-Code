defmodule Firewall do
	defstruct layers: %{}
end

defmodule Layer do
	defstruct range: 0, scanner: nil, direction: nil
end

defmodule DayThirteen do
	def part_one(input) do
		input
		|> initialize
		|> get_severity
	end

	def part_two(input) do
		input
		|> initialize
		|> run_until_never_caught
	end

	defp initialize(input) do
		create_layer = fn(x, acc) ->
			Map.put(acc, x, %Layer{range: input[x], scanner: 0, direction: :down})
		end

		firewall = %Firewall{
			layers: List.foldl(Map.keys(input), %{}, create_layer),
		}

		states = Enum.scan(1..get_total_depth(firewall), firewall, &move_scanners/2)
		[firewall | states]
	end

	defp get_total_depth(firewall) do
		Enum.max(Map.keys(firewall.layers))
	end

	defp move_scanners(_, %Firewall{layers: layers} = firewall) do
		move_scanner = fn(depth, acc) ->
			layer = layers[depth]
			cond do
				layer.scanner === 0 and layer.direction === :up ->
					Map.put(acc, depth, %Layer{range: layer.range, scanner: min(1, layer.range - 1), direction: :down})
				
				layer.scanner === layer.range - 1 and layer.direction === :down ->
					Map.put(acc, depth, %Layer{range: layer.range, scanner: max(0, layer.range - 2), direction: :up})
				
				layer.direction === :up ->
					Map.put(acc, depth, %Layer{range: layer.range, scanner: layer.scanner - 1, direction: :up})

				layer.direction === :down ->
					Map.put(acc, depth, %Layer{range: layer.range, scanner: layer.scanner + 1, direction: :down})
			end
		end

		updated_layers = List.foldl(Map.keys(layers), %{}, move_scanner)

		%Firewall{firewall | layers: updated_layers}
	end

	defp get_severity(states) do
		states
		|> List.foldl(%{caught: false, severity: 0, index: 0}, &accumulate_severity/2)
		|> Map.fetch!(:severity)
	end

	defp never_caught?(states) do
		states
		|> List.foldl(%{caught: false, severity: 0, index: 0}, &accumulate_severity/2)
		|> Map.fetch!(:caught)
		|> Kernel.not
	end

	defp accumulate_severity(%Firewall{layers: layers} = firewall, %{caught: been_caught, severity: acc, index: i}) do
		caught = packet_caught?(firewall, i)
		new_severity = 
			if (caught and Map.has_key?(layers, i)) do
				acc + i * layers[i].range
			else
				acc
		end

		%{caught: been_caught or caught, severity: new_severity, index: i + 1}
	end

	defp packet_caught?(%Firewall{layers: layers}, layer) do
		if (Map.has_key?(layers, layer)) do
			layers[layer].scanner === 0
		else
			false
		end
	end

	defp run_until_never_caught(states, count \\ 0) do
		if (never_caught?(states)) do
			count
		else
			states
			|> Enum.drop(1)
			|> List.insert_at(-1, move_scanners(nil, Enum.at(states, -1)))
			|> run_until_never_caught(count + 1)
		end
	end
end

parse_input = fn(line, map) ->
	regex = ~r/(\d+): (\d+)/
	[_, depth, range] = Regex.run(regex, line)
	Map.put(map, String.to_integer(depth), String.to_integer(range))
end

input =
	"input/day13.txt"
	|> File.read!
	|> String.split("\r\n")
	|> List.foldl(%{}, parse_input)

# Expected answers for default input
# Part one: 1316
# Part two: 3840052
IO.puts("Part one: " <> Integer.to_string(DayThirteen.part_one(input)))

# Warning: Part two is slow (takes about 3 minutes on my machine)
IO.puts("Part two: " <> Integer.to_string(DayThirteen.part_two(input)))