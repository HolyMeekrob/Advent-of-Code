defmodule DaySeventeen do
	def part_one(input) do
		num = 2017
		final_state = insert([0], input, 0, num)
		prev_index = Enum.find_index(final_state, &(&1 === num))
		Enum.at(final_state, rem(prev_index + 1, num + 1))
	end

	def part_two(input) do
		track_first_number(0, 1, input, 0, 50000000)
	end

	defp insert(state, _, _, 0) do
		state
	end

	defp insert(state, steps, current_position, remaining_inserts) do
		num = length(state)
		next_position = get_next_position(current_position, steps, num)
		next_state = List.insert_at(state, next_position, num)
		insert(next_state, steps, next_position, remaining_inserts - 1)
	end

	defp track_first_number(num, _, _, _, 0) do
		num
	end

	defp track_first_number(num, size, steps, current_position, remaining_inserts) do
		next_position = get_next_position(current_position, steps, size)
		num =
			if (next_position === 1) do
				size
			else
				num
			end

		track_first_number(num, size + 1, steps, next_position, remaining_inserts - 1)
	end

	defp get_next_position(position, steps, size) do
		rem(position + steps, size) + 1
	end
end

input = 363
input =
	if (length(System.argv) == 0) do
		input
	else
		String.to_integer(List.first(System.argv))
	end

# Expected answers for default input
# Part one: 136
# Part two: 1080289

IO.puts("Part one: " <> Integer.to_string(DaySeventeen.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DaySeventeen.part_two(input)))