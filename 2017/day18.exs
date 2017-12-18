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

defmodule Send do
	defstruct [:val]
end

defmodule Receive do
	defstruct [:register]
end

defmodule Jump do
	defstruct [:val, :jump]
end

defmodule Program do
	defstruct [:registers, :index, :queue, :sent]
end

defmodule DayEighteenA do
	def run(input) do
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
				registers = Map.put(registers, String.to_atom(register), get_value(registers, val))
				run_instructions(instructions, registers, index + 1, last_played)

			%Add{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, String.to_atom(register), current_val + operand)
				run_instructions(instructions, registers, index + 1, last_played)

			%Multiply{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, String.to_atom(register), current_val * operand)
				run_instructions(instructions, registers, index + 1, last_played)

			%Modulo{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, String.to_atom(register), rem(current_val, operand))
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
			:error -> Map.get(registers, String.to_atom(val), 0)
		end
	end
end

defmodule DayEighteenB do
	def run(input) do
		programs = init_programs()
		instructions = :array.map(&parse_instruction/2, input)

		run_programs(instructions, programs)
		|> elem(1)
		|> Map.fetch!(:sent)
	end

	defp init_programs() do
		{ init_program(0), init_program(1)}
	end

	defp init_program(num) do
		%Program{
			registers: %{p: num},
			index: 0,
			queue: [],
			sent: 0
		}
	end

	defp parse_instruction(_, "snd " <> val) do
		%Send{val: val}
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

	defp parse_instruction(_, "rcv " <> register) do
		%Receive{register: register}
	end

	defp parse_instruction(_, "jgz " <> data) do
		regex = ~r/([\w\-]+) ([\w\-]+)/
		[_, val, jump] = Regex.run(regex, data)
		%Jump{val: val, jump: jump}
	end
	
	defp run_programs(instructions, programs) do
		first_run = run_instruction(instructions, programs, 0)
		next = run_instruction(instructions, first_run, 1)

		index_0a = programs
			|> elem(0)
			|> Map.fetch!(:index)

		index_0b = next
			|> elem(0)
			|> Map.fetch!(:index)

		index_1a = programs
			|> elem(1)
			|> Map.fetch!(:index)

		index_1b = next
			|> elem(1)
			|> Map.fetch!(:index)
		
		if (index_0a === index_0b and index_1a === index_1b) do
			programs
		else
			run_programs(instructions, next)
		end
	end

	defp run_instruction(instructions, programs, program_index) do
		program = elem(programs, program_index)
		other_index = 1 - program_index
		other_program = elem(programs, other_index)

		%Program{registers: registers, index: index, queue: queue, sent: sent} = program

		if (index < 0 or index >= :array.size(instructions)) do
			raise "Invalid instruction index"
		end

		case :array.get(index, instructions) do
			%Send{val: val} ->
				value_to_send = get_value(registers, val)

				programs = put_elem(programs, program_index,
					%Program{program | index: index + 1, sent: sent + 1})

				put_elem(programs, other_index,
					%Program{other_program | queue: other_program.queue ++ [value_to_send]})

			%SetRegister{register: register, val: val} ->
				registers = Map.put(registers, String.to_atom(register), get_value(registers, val))
				put_elem(programs, program_index, %Program{program | registers: registers, index: index + 1})

			%Add{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, String.to_atom(register), current_val + operand)
				put_elem(programs, program_index, %Program{program | registers: registers, index: index + 1})

			%Multiply{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, String.to_atom(register), current_val * operand)
				put_elem(programs, program_index, %Program{program | registers: registers, index: index + 1})

			%Modulo{register: register, val: val} ->
				current_val = get_value(registers, register)
				operand = get_value(registers, val)
				registers = Map.put(registers, String.to_atom(register), rem(current_val, operand))
				put_elem(programs, program_index, %Program{program | registers: registers, index: index + 1})

			%Receive{register: register} ->
				if (length(queue) === 0) do
					programs
				else
					[new_val | tail] = queue
					registers = Map.put(registers, String.to_atom(register), new_val)
					put_elem(programs, program_index, %Program{program | registers: registers, index: index + 1, queue: tail})
				end

			%Jump{val: val, jump: jump} ->
				check = get_value(registers, val)
				index =
					if (check > 0) do
						index + get_value(registers, jump)
					else
						index + 1
					end
				put_elem(programs, program_index, %Program{program | index: index})
		end
	end

	defp get_value(registers, val) do
		case Integer.parse(val) do
			{num, _} -> num
			:error -> Map.get(registers, String.to_atom(val), 0)
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
# Part two: 7239

IO.puts("Part one: " <> Integer.to_string(DayEighteenA.run(input)))
IO.puts("Part two: " <> Integer.to_string(DayEighteenB.run(input)))