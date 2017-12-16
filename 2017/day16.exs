defmodule DaySixteen do
	def run(input, count \\ 1) do
		input
		|> dance(initialize_programs(), count)
		|> Enum.join
	end

	def dance(_, programs, 0) do
		programs
	end

	def dance(instructions, programs, count) do
		post_dance = List.foldl(instructions, programs, &run_instruction/2)
		dance(instructions, post_dance, count - 1)
	end

	defp initialize_programs() do
		[:a, :b, :c, :d, :e, :f, :g, :h, :i, :j, :k, :l, :m, :n, :o, :p]
	end

	defp run_instruction("s" <> num, programs) do
		num = String.to_integer(num)

		{head, tail} = Enum.split(programs, -num)
		tail ++ head
	end

	defp run_instruction( "x" <> values, programs) do
		regex = ~r/(\d+)\/(\d+)/
		[_, a, b] = Regex.run(regex, values)

		a = String.to_integer(a)
		b = String.to_integer(b)

		x = Enum.at(programs, a)
		y = Enum.at(programs, b)

		programs
		|> List.replace_at(a, y)
		|> List.replace_at(b, x)
	end

	defp run_instruction( "p" <> values, programs) do
		regex = ~r/(\w+)\/(\w+)/
		[_, a, b] = Regex.run(regex, values)

		a = String.to_atom(a)
		b = String.to_atom(b)

		swap_programs =
			fn(program) ->
				case program do
					^a -> b
					^b -> a
					x -> x
				end
			end
		Enum.map(programs, swap_programs)
	end
end

input =
	"input/day16.txt"
	|> File.read!
	|> String.split(",")

# Expected answers for default input
# Part one: iabmedjhclofgknp
# Part two: 

IO.puts("Part one: " <> (DaySixteen.run(input)))
# IO.puts("Part two: " <> (DaySixteen.run(input, 1000000000)))