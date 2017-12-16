defmodule Spin do
	@enforce_keys [:num]
	defstruct [:num]
end

defmodule Exchange do
	@enforce_keys [:pos_a, :pos_b]
	defstruct [:pos_a, :pos_b]
end

defmodule Partner do
	@enforce_keys [:prog_a, :prog_b]
	defstruct [:prog_a, :prog_b]
end

defmodule DaySixteen do
	def part_one(input) do
		input
		|> Enum.map(&build_instruction/1)
		|> dance(initialize_programs(), 1)
		|> :array.to_list
		|> Enum.join
	end

	def part_two(input, count) do
		instructions = Enum.map(input, &build_instruction/1)
		initial_programs = initialize_programs()
		cycle_length = get_cycle_length(instructions, initial_programs)
		runs = rem(count, cycle_length)

		dance(instructions, initial_programs, runs)
		|> :array.to_list
		|> Enum.join
	end

	defp build_instruction("s" <> num) do
		%Spin{num: String.to_integer(num)}
	end

	defp build_instruction("x" <> values) do
		regex = ~r/(\d+)\/(\d+)/
		[_, a, b] = Regex.run(regex, values)
		%Exchange{pos_a: String.to_integer(a), pos_b: String.to_integer(b)}
	end

	defp build_instruction("p" <> values) do
		regex = ~r/(\w+)\/(\w+)/
		[_, a, b] = Regex.run(regex, values)
		%Partner{prog_a: String.to_atom(a), prog_b: String.to_atom(b)}
	end

	defp get_cycle_length(instructions, programs, count \\ 0) do
		next = List.foldl(instructions, programs, &run_instruction/2)
		if (next === initialize_programs()) do
			count + 1
		else
			get_cycle_length(instructions, next, count + 1)
		end
	end

	defp dance(_, programs, 0) do
		programs
	end

	defp dance(instructions, programs, count) do
		post_dance = List.foldl(instructions, programs, &run_instruction/2)
		dance(instructions, post_dance, count - 1)
	end

	defp initialize_programs() do
		programs = [:a, :b, :c, :d, :e, :f, :g, :h, :i, :j, :k, :l, :m, :n, :o, :p]
		:array.from_list(programs)
	end

	defp run_instruction(%Spin{num: num}, programs) do
		size = :array.size(programs)
		cutoff = size - num

		spin = fn(idx, _) ->
			if(idx >= num) do
					:array.get(idx - num, programs)
			else
					:array.get(cutoff + idx, programs)
			end
		end
		:array.map(spin, programs)
	end

	defp run_instruction(%Exchange{pos_a: a, pos_b: b}, programs) do
		x = :array.get(a, programs)
		y = :array.get(b, programs)

		programs = :array.set(a, y, programs)
		:array.set(b, x, programs)
	end

	defp run_instruction(%Partner{prog_a: a, prog_b: b}, programs) do
		swap_programs =
			fn(_, program) ->
				case program do
					^a -> b
					^b -> a
					x -> x
				end
			end
		:array.map(swap_programs, programs)
	end
end

input =
	"input/day16.txt"
	|> File.read!
	|> String.split(",")

# Expected answers for default input
# Part one: iabmedjhclofgknp
# Part two: oildcmfeajhbpngk

IO.puts("Part one: " <> DaySixteen.part_one(input))
IO.puts("Part two: " <> DaySixteen.part_two(input, 1000000000))