defmodule State do
	defstruct [:routes, :pos, :dir, :letters]
end

defmodule DayNineteen do
	def part_one(input) do
		input
		|> initialize
		|> traverse
		|> Map.fetch!(:letters)
		|> Enum.reverse
		|> to_string
	end

	defp get_starting_position(input) do
		x =
			0
			|> :array.get(input)
			|> :array.to_list
			|> Enum.find_index(&(&1 === '|'))


		foo = :array.get(0, input) |> :array.to_list
		IO.inspect(foo)
		IO.inspect(Enum.find_index(foo, &(&1 === '|')))

		{0, x}
	end

	defp initialize(input) do
		%State
		{
			routes: input,
			pos: get_starting_position(input),
			dir: :down,
			letters: []
		}
	end

	defp traverse(%State{routes: routes, pos: {y, x}, letters: ltrs} = state) do
		case get_route(routes, y, x) do
			' ' -> state

			route when (route === '|' or route === '-') ->
				continue(state)

			route when(route === '+') ->
				cross(state)

			route ->
				continue(%State{state | letters: [route | ltrs]})
		end
	end

	defp continue(%State{pos: {y, x}, dir: :down} = state) do
		%State{state | pos: {y + 1, x}}
	end

	defp continue(%State{pos: {y, x}, dir: :up} = state) do
		%State{state | pos: {y - 1, x}}
	end

	defp continue(%State{pos: {y, x}, dir: :right} = state) do
		%State{state | pos: {y, x + 1}}
	end

	defp continue(%State{pos: {y, x}, dir: :left} = state) do
		%State{state | pos: {y, x - 1}}
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :down} = state) do
		cond do
			(:array.size >= y + 1) and (get_route(routes, y + 1, x) !== ' ') ->
				continue(state)

			(x - 1 >= 0) and (get_route(routes, y, x - 1) !== ' ') ->
				continue(%State{state | dir: :left})

			true ->
				continue(%State{state | dir: :right})
		end
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :up} = state) do
		cond do
			(y - 1 >= 0) and (get_route(routes, y - 1, x) !== ' ') ->
				continue(state)

			(x - 1 >= 0) and (get_route(routes, y, x - 1) !== ' ') ->
				continue(%State{state | dir: :left})

			true ->
				continue(%State{state | dir: :right})
		end
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :left} = state) do
		cond do
			(x - 1 >= 0) and (get_route(routes, y, x - 1) !== ' ') ->
				continue(state)

			(y - 1 >= 0) and (get_route(routes, y - 1, x) !== ' ') ->
				continue(%State{state | dir: :up})

			true ->
				continue(%State{state | dir: :down})
		end
	end

	defp cross(%State{routes: routes, pos: {y, x}, dir: :right} = state) do
		cond do
			(:array.size >= x + 1) and (get_route(routes, y, x + 1) !== ' ') ->
				continue(state)

			(y - 1 >= 0) and (get_route(routes, y - 1, x) !== ' ') ->
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

IO.inspect(DayNineteen.part_one(input))