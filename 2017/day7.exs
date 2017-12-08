defmodule Tree do
	defstruct value: nil, children: []
end

defmodule Program do
	defstruct name: nil, weight: 0, children: []

	def get_weight(program) do
		if (length(program.children) === 0) do
			program.weight
		else
			List.foldl(program.children, program.weight, &(get_weight(&1) + &2))
		end
	end

	def get_weight_tree(program) do
		%Tree{
			value: %{name: program.name, weight: program.weight, total_weight: get_weight(program)},
			children: Enum.map(program.children, &get_weight_tree/1)
		}
	end
end

defmodule DaySeven do
	def part_one(programs) do
		get_bottom_program(programs)
	end

	def part_two(programs) do
		programs
		|> build(get_bottom_program(programs))
		|> Program.get_weight_tree
		# |> get_imbalanced_weights
		# |> odd_one_out
		|> get_weight_fix
	end

	defp get_weight_fix(weight_tree) do
		child_weights = Enum.map(weight_tree.children, &(&1.value.total_weight))
		balanced = all_match(child_weights)
		if (balanced) do
			0
		else
			weight_fix =
				weight_tree.children
					|> Enum.map(&get_weight_fix/1)
					|> Enum.max(fn -> 0 end)
			if(weight_fix > 0) do
				weight_fix
			else
				{common, unique} = odd_one_out(child_weights)
				fix_value = common - unique
				[fix_node | _] = Enum.filter(weight_tree.children, &(&1.value.total_weight === unique))
				fix_node.value.weight + fix_value
			end
		end
	end

	defp all_match(lst) do
		lst
			|> Enum.uniq
			|> Kernel.length
			|> Kernel.===(1)
	end

	defp odd_one_out([a | [b | [c | _]]]) when a === b do
		{a, c}
	end

	defp odd_one_out([a | [b | [c | _]]]) when b === c do
		{b, a}
	end

	defp odd_one_out([a | [b | [_ | _]]]) do
		{a, b}
	end

	#[%{:name: str, weight: num, children: [string]}]
	defp get_bottom_program(programs) do
		all_programs = MapSet.new(Enum.map(programs, &get_program_name/1))
		all_children = MapSet.new(List.foldl(programs, [], &append_children/2))

		all_programs
		|> MapSet.difference(all_children)
		|> MapSet.to_list
		|> List.first
	end

	defp build(input, bottom) do
		prog = Enum.find(input, &(&1.name === bottom))
		%Program{
			name: bottom,
			weight: prog.weight,
			children: Enum.map(prog.children, &(build(input, &1)))
		}
	end

	defp get_program_name(program) do
		program.name
	end

	defp append_children(program, child_list) do
		program
		|> get_children
		|> List.flatten(child_list)
	end

	defp get_children(program) do
		program.children
	end
end

regex = ~r/([a-z]+) \((\d+)\)(?: \-> (.+))?/

build_program =
	fn(line) ->
		[_ | [name | [weight | children_str]]] = Regex.run(regex, line)
		children =
			if (length(children_str) == 0) do
				[]
			else
				children_str
				|> List.first
				|> String.split(", ")
			end
		%{name: name, weight: String.to_integer(weight), children: children}
	end

input =
	"input/day7.txt"
		|> File.read!
		|> String.split("\r\n")
		|> Enum.map(build_program)

# Expected answers for default input
# Part one: wiapj
# Part two: 1072

IO.puts("Part one: " <> DaySeven.part_one(input))
IO.puts("Part two: " <> Integer.to_string(DaySeven.part_two(input)))