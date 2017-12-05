defmodule DayFive do
	def part_one(instructions) do
		jump_one(instructions, 0, 0)
	end

	def part_two(instructions) do
		jump_two(instructions, 0, 0)
	end

	defp jump_one(_, index, count) when index < 0 do
		count
	end

	defp jump_one(instructions, index, count) do
		case :array.get(index, instructions) do
			:undefined -> count
			instruction ->
				new_index = index + instruction
				new_instructions = :array.set(index, instruction + 1, instructions)
				jump_one(new_instructions, new_index, count + 1)
		end
	end

	defp jump_two(_, index, count) when index < 0 do
		count
	end

	defp jump_two(instructions, index, count) do
		case :array.get(index, instructions) do
			:undefined -> count
			instruction ->
				new_index = index + instruction
				new_instructions = :array.set(index, get_new_instruction(instruction),
					instructions)
				jump_two(new_instructions, new_index, count + 1)
		end
	end

	defp get_new_instruction(instruction) do
		if (instruction > 2) do
			instruction - 1
		else
			instruction + 1
		end
	end
end

input =
	if (length(System.argv) == 0) do
		"input/day5.txt"
			|> File.read!
			|> String.split("\r\n")
			|> Enum.map(&String.to_integer/1)
			|> :array.from_list
	else
		System.argv
		|> List.first
		|> String.split
		|> Enum.map(&String.to_integer/1)
		|> :array.from_list
	end

# Expected answers for default input
# Part one: 336905
# Part two: 21985262

IO.puts("Part one: " <> Integer.to_string(DayFive.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DayFive.part_two(input)))