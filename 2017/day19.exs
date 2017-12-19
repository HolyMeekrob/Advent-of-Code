defmodule State do
	defstruct [:routes, :pos, :dir, :letters, :steps]
end

defmodule DayNineteen do
	def run(input) do
		result =
			input
			|> initialize
			|> traverse

		part_one =
			result
			|> Map.fetch!(:letters)
			|> Enum.reverse
			|> to_string

		part_two = Map.fetch!(result, :steps)

		{part_one, part_two}
	end

	defp get_starting_position(input) do
		x =
			0
			|> :array.get(input)
			|> :array.to_list
			|> Enum.find_index(&(&1 === ?|))

		{0, x}
	end

	defp initialize(input) do
		%State
		{
			routes: input,
			pos: get_starting_position(input),
			dir: :down,
			letters: [],
			steps: 0
		}
	end

	defp traverse(%State{routes: routes, pos: {y, x}, letters: ltrs} = state) do
		case get_route(routes, y, x) do
			?\s ->
				state

			route when (route === ?| or route === ?-) ->
				state
				|> continue
				|> traverse

			route when(route === ?+) ->
				state
				|> cross
				|> traverse

			route ->
				%State{state | letters: [route | ltrs]}
				|> continue
				|> traverse
		end
	end

	defp continue(%State{pos: {y, x}, steps: steps, dir: :down} = state) do
		%State{state | pos: {y + 1, x}, steps: steps + 1}
	end

	defp continue(%State{pos: {y, x}, steps: steps, dir: :up} = state) do
		%State{state | pos: {y - 1, x}, steps: steps + 1}
	end

	defp continue(%State{pos: {y, x}, steps: steps, dir: :right} = state) do
		%State{state | pos: {y, x + 1}, steps: steps + 1}
	end

	defp continue(%State{pos: {y, x}, steps: steps, dir: :left} = state) do
		%State{state | pos: {y, x - 1}, steps: steps + 1}
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :down} = state) do
		cond do
			(:array.size(routes) > y + 1)
					and (get_route(routes, y + 1, x) !== ?\s) ->
				continue(state)

			(x - 1 >= 0) and (get_route(routes, y, x - 1) !== ?\s) ->
				continue(%State{state | dir: :left})

			true ->
				continue(%State{state | dir: :right})
		end
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :up} = state) do
		cond do
			(y - 1 >= 0) and (get_route(routes, y - 1, x) !== ?\s) ->
				continue(state)

			(x - 1 >= 0) and (get_route(routes, y, x - 1) !== ?\s) ->
				continue(%State{state | dir: :left})

			true ->
				continue(%State{state | dir: :right})
		end
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :left} = state) do
		cond do
			(x - 1 >= 0) and (get_route(routes, y, x - 1) !== ?\s) ->
				continue(state)

			(y - 1 >= 0) and (get_route(routes, y - 1, x) !== ?\s) ->
				continue(%State{state | dir: :up})

			true ->
				continue(%State{state | dir: :down})
		end
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :right} = state) do
		cond do
			(:array.size(:array.get(y, routes)) > x + 1)
					and (get_route(routes, y, x + 1) !== ?\s) ->
				continue(state)

			(y - 1 >= 0) and (get_route(routes, y - 1, x) !== ?\s) ->
				continue(%State{state | dir: :up})

			true ->
				continue(%State{state | dir: :down})
		end
	end

	defp get_route(routes, y, x) do
		:array.get(x, :array.get(y, routes))
	end
end


input =
	"input/day19.txt"
	|> File.read!
	|> String.split("\r\n")
	|> Enum.map(&String.to_charlist/1)
	|> Enum.map(&:array.from_list/1)
	|> :array.from_list


# Expected answers for default input
# Part one: MKXOIHZNBL
# Part two: 17872

result = DayNineteen.run(input)
IO.puts("Part one: " <> elem(result, 0))
IO.puts("Part two: " <> Integer.to_string(elem(result, 1)))