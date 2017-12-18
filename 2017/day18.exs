defmodule Play do
	defstruct [:val]
end

defmodule SetRegister do
	defstruct [:register, :val]
end

defmodule Add do
	defstruct [:register, :val]
end

defmodule Multiply do
	defstruct [:register, :val]
end

defmodule Modulo do
	defstruct [:register, :val]
end

defmodule Recover do
	defstruct [:val]
end

defmodule Jump do
	defstruct [:val, :jump]
end

defmodule DayEighteen do
	def part_one(input) do
		:array.map(&parse_instruction/2, input)
		|> run_instructions(%{}, 0, nil)
	end

	defp parse_instruction(_, "snd " <> val) do
		%Play{val: val}
	end

	defp parse_instruction(_, "set " <> data) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, register, val] = Regex.run(regex, data)
		%SetRegister{register: register, val: val}
	end

	defp parse_instruction(_, "add " <> data) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, register, val] = Regex.run(regex, data)
		%Add{register: register, val: val}
	end

	defp parse_instruction(_, "mul " <> data) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, register, val] = Regex.run(regex, data)
		%Multiply{register: register, val: val}
	end

	defp parse_instruction(_, "mod " <> data) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, register, val] = Regex.run(regex, data)
		%Modulo{register: register, val: val}
	end

	defp parse_instruction(_, "rcv " <> val) do
		%Recover{val: val}
	end

	defp parse_instruction(_, "jgz " <> data) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, val, jump] = Regex.run(regex, data)
		%Jump{val: val, jump: jump}
	end

	defp run_instructions(instructions, registers, index, last_played) do
		if (index < 0 or index >= :array.size(instructions)) do
			nil
		end

		case :array.get(index, instructions) do
			%Play{val: val} ->
				last_played =  get_value(registers, val)
				run_instructions(instructions, registers, index + 1, last_played)

			%SetRegister{register: register, val: val} ->
				registers = Map.put(registers, register, get_value(registers, val))
				run_instructions(instructions, registers, index + 1, last_played)

			%Add{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, register, current_val + operand)
				run_instructions(instructions, registers, index + 1, last_played)

			%Multiply{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, register, current_val * operand)
				run_instructions(instructions, registers, index + 1, last_played)

			%Modulo{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, register, rem(current_val, operand))
				run_instructions(instructions, registers, index + 1, last_played)

			%Recover{} ->
				last_played

			%Jump{val: val, jump: jump} ->
				check = get_value(registers, val)
				index =
					if (check > 0) do
						index + get_value(registers, jump)
					else
						index + 1
					end
				run_instructions(instructions, registers, index, last_played)
		end
	end

	defp get_value(registers, val) do
		case Integer.parse(val) do
			{num, _} -> num
			:error -> Map.get(registers, val, 0)
		end
	end
end

input =
	"input/day18.txt"
	|> File.read!
	|> String.split("\r\n")
	|> :array.from_list

# Expected answers for default input
# Part one: 8600
# Part two: 

IO.puts("Part one: " <> Integer.to_string(DayEighteen.part_one(input)))