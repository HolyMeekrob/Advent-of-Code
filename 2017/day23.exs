defmodule SetRegister do
	defstruct [:register, :value]
end

defmodule Subtract do
	defstruct [:register, :value]
end

defmodule Multiply do
	defstruct [:register, :value]
end

defmodule Jump do
	defstruct [:value, :jump]
end

defmodule State do
	defstruct [:instructions, :registers, :index, :mult_count]
end

defmodule DayTwentyThree do
	def parse_instruction("set " <> vals) do
		{register, value} = get_values(vals)
		%SetRegister{register: register, value: value}
	end

	def parse_instruction("sub " <> vals) do
		{register, value} = get_values(vals)
		%Subtract{register: register, value: value}
	end

	def parse_instruction("mul " <> vals) do
		{register, value} = get_values(vals)
		%Multiply{register: register, value: value}
	end

	def parse_instruction("jnz " <> vals) do
		{value, jump} = get_values(vals)
		%Jump{value: value, jump: jump}
	end

	defp get_values(vals) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, register, val] = Regex.run(regex, vals)
		{register, val}
	end

	def part_one(instructions) do
		instructions
		|> init
		|> run
		|> Map.fetch!(:mult_count)
	end

	defp init(instructions) do
		registers = %{ a: 0, b: 0, c: 0, d: 0, e: 0, f: 0, g: 0, h: 0 }
		%State
		{
			instructions: instructions,
			registers: registers,
			index: 0,
			mult_count: 0
		}
	end

	defp run(%State{instructions: instructions, index: index} = state)
			when index < 0 or index >= length(instructions) do
		state
	end

	defp run(%State{instructions: instructions, index: index} = state) do
		instructions
		|> Enum.at(index)
		|> run_instruction(state)
		|> run
	end

	defp run_instruction(%SetRegister{register: register, value: value},
			%State{registers: registers, index: i} = state) do
		value = get_value(registers, value)
		registers = Map.put(registers, String.to_atom(register), value)
		%State{state | registers: registers, index: i + 1}
	end

	defp run_instruction(%Subtract{register: register, value: value},
			%State{registers: registers, index: i} = state) do
		value = Map.fetch!(registers, String.to_atom(register)) -
			get_value(registers, value)
		registers = Map.put(registers, String.to_atom(register), value)
		%State{state | registers: registers, index: i + 1}
	end

	defp run_instruction(%Multiply{register: register, value: value},
			%State{registers: registers, index: i, mult_count: count} = state) do
		value = Map.fetch!(registers, String.to_atom(register)) *
			get_value(registers, value)
		registers = Map.put(registers, String.to_atom(register), value)
		%State{state | registers: registers, index: i + 1, mult_count: count + 1}
	end

	defp run_instruction(%Jump{value: value, jump: jump},
			%State{registers: registers, index: i} = state) do
		check = get_value(registers, value)
		jump =
			if (check === 0) do
				1
			else
				get_value(registers, jump)
		end
		
		%State{state | index: i + jump}
	end

	defp get_value(registers, val) do
		case Integer.parse(val) do
			{num, _} -> num
			:error -> Map.fetch!(registers, String.to_atom(val))
		end
	end
end

# Expected answers for default input
# Part one: 6724
# Part two: 

input =
	"input/day23.txt"
	|> File.read!
	|> String.split("\r\n")
	|> Enum.map(&DayTwentyThree.parse_instruction/1)

IO.puts("Part one: " <> Integer.to_string(DayTwentyThree.part_one(input)))