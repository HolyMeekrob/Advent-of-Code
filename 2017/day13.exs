defmodule Firewall do
	defstruct layers: %{}, packet: -1, severity: 0
end

defmodule Layer do
	defstruct range: 0, scanner: nil, direction: nil
end

defmodule DayThirteen do
	def part_one(input) do
		firewall = initialize(input)
		firewall
		|> step(get_total_depth(firewall))
		|> Map.fetch!(:severity)
	end

	defp initialize(input) do
		create_layer = fn(x, acc) ->
			Map.put(acc, x, %Layer{range: input[x], scanner: 0, direction: :down})
		end

		%Firewall{
			layers: List.foldl(Map.keys(input), %{}, create_layer),
			packet: -1,
			severity: 0
		}
	end

	defp get_total_depth(firewall) do
		Enum.max(Map.keys(firewall.layers))
	end

	defp step(firewall, remaining_layers) when remaining_layers < 0 do
		firewall
	end

	defp step(firewall, remaining_layers) do
		firewall
		|> increment_packet
		|> accumulate_severity
		|> move_scanners
		|> step(remaining_layers - 1)
	end

	defp increment_packet(%Firewall{packet: x} = firewall) do
		%Firewall{firewall | packet: x + 1}
	end

	defp accumulate_severity(%Firewall{severity: x} = firewall) do
		severity = 
			if (packet_caught?(firewall)) do
				x + calculate_severity(firewall)
			else
				x
			end
		%Firewall{firewall | severity: severity}
	end

	defp packet_caught?(%Firewall{layers: layers, packet: packet}) do
		if (Map.has_key?(layers, packet)) do
			layers[packet].scanner === 0
		else
			false
		end
	end

	defp calculate_severity(%Firewall{layers: layers, packet: packet}) do
		packet * layers[packet].range
	end

	defp move_scanners(%Firewall{layers: layers} = firewall) do
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
# Part two: 
IO.puts("Part one: " <> Integer.to_string(DayThirteen.part_one(input)))
# IO.puts("Part two: " <> Integer.to_string(DayThirteen.part_two(input)))