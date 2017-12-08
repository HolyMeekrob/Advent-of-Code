defmodule Instruction do
	defstruct [:register, :modifier, :modifier_operand, :check_register, :check, :check_operand]
end

defmodule DayEight do
	def parse_instruction(line) do
		regex = ~r/(\w+) (\w+) (\-?\d+) if (\w+) (.+) (\-?\d+)/
		[_, register1, modifier, operand1, register2, check, operand2] = Regex.run(regex, line)
			%Instruction{
				register: register1,
				modifier: modifier,
				modifier_operand: String.to_integer(operand1),
				check_register: register2,
				check: check,
				check_operand: String.to_integer(operand2)
			}
	end

	def run(instructions) do
		final_state = List.foldl(instructions, %{max_value: 0}, &run_instruction/2)
		part_two = final_state.max_value
		{part_one(final_state), part_two}
	end

	defp part_one(state) do
		state
			|> Map.pop(:max_value)
			|> elem(1)
			|> Map.values
			|> Enum.max
	end

	defp run_instruction(instruction, state) do
		state = Map.put_new(state, instruction.register, 0)
		state = Map.put_new(state, instruction.check_register, 0)

		if (instruction_should_run(instruction, state)) do
			update_state(instruction, state)
		else
			state
		end
	end

	defp instruction_should_run(%{check: ">"} = instruction, state) do
		state[instruction.check_register] > instruction.check_operand
	end

	defp instruction_should_run(%{check: "<"} = instruction, state) do
	state[instruction.check_register] < instruction.check_operand
	end

	defp instruction_should_run(%{check: ">="} = instruction, state) do
		state[instruction.check_register] >= instruction.check_operand
	end

	defp instruction_should_run(%{check: "<="} = instruction, state) do
		state[instruction.check_register] <= instruction.check_operand
	end

	defp instruction_should_run(%{check: "=="} = instruction, state) do
		state[instruction.check_register] === instruction.check_operand
	end

	defp instruction_should_run(%{check: "!="} = instruction, state) do
		state[instruction.check_register] !== instruction.check_operand
	end

	defp update_state(%{modifier: "inc"} = instruction, state) do
		register = instruction.register
		current_value = Map.fetch!(state, register)
		new_value = current_value + instruction.modifier_operand
		state = Map.put(state, instruction.register, new_value)
		%{state | max_value: max(state.max_value, new_value)}
	end

	defp update_state(%{modifier: "dec"} = instruction, state) do
		register = instruction.register
		current_value = Map.fetch!(state, register)
		new_value = current_value - instruction.modifier_operand
		state = Map.put(state, instruction.register, new_value)
		%{state | max_value: max(state.max_value, new_value)}
	end
end

input =
	"input/day8.txt"
		|> File.read!
		|> String.split("\r\n")
		|> Enum.map(&DayEight.parse_instruction/1)

output = DayEight.run(input)

# Expected answers for default input
# Part one: 5075
# Part two: 7310
IO.puts("Part one: " <> Integer.to_string(elem(output, 0)))
IO.puts("Part two: " <> Integer.to_string(elem(output, 1)))