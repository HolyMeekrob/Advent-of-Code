use Bitwise, only_operators: true

defmodule Knot do
	defstruct sequence: [], current_position: 0, skip: 0
end

defmodule DayTen do
	def part_one(lengths) do
		final_knot = List.foldl(lengths, initialize_knot(255), &process/2)
		[a | [b | _]] = final_knot.sequence
		a * b
	end

	def part_two(lengths) do
		lengths
		|> iterate(initialize_knot(255), 64)
		|> Enum.chunk_every(16)
		|> Enum.map(&hash/1)
	end

	defp iterate(_, knot, 0) do
		knot.sequence
	end

	defp iterate(lengths, knot, remaining_runs) do
		next_knot = List.foldl(lengths, knot, &process/2)
		iterate(lengths, next_knot, remaining_runs - 1)
	end

	defp hash([first | rest]) do
		List.foldl(rest, first, &(&1 ^^^ &2))
	end

	defp initialize_knot(max) do
		%Knot{sequence: Enum.to_list(0..max)}
	end

	defp process(num, knot) do
		knot
		|> reverse_sequence(num)
		|> update_position(num)
		|> increment_skip
	end

	defp reverse_sequence(knot, num) do
		%{knot | sequence: reverse_in_place(knot.sequence, knot.current_position, num)}
	end

	defp reverse_in_place(lst, start, count) do
		list_to_reverse = slice_wrapped(lst, start, count)
		reversed_list = Enum.reverse(list_to_reverse)
		replace(lst, reversed_list, start)
	end

	defp replace(lst, replacement, start) do
		size = length(lst)
		replacement_size = length(replacement)

		tail_start = start + replacement_size
		head_start = max(0, (tail_start) - size)
		rest_of_list =
			Enum.drop(lst, start + replacement_size)
			++ Enum.slice(lst, head_start, start - head_start)

		list_to_shift = replacement ++ rest_of_list
		shift(list_to_shift, start)
	end

	defp shift(enumerable, num) do
		index = length(enumerable) - num
		Enum.drop(enumerable, index) ++ Enum.take(enumerable, index)
	end

	defp slice_wrapped(enumerable, start, count) do
		enumerable
		|> Enum.slice(start, count)
		|> Kernel.++(Enum.slice(enumerable, 0, max(0, start + count - length(enumerable))))
	end

	defp update_position(knot, num) do
		sequence_length = length(knot.sequence)
		new_position = knot.current_position + knot.skip + num
		new_position = rem(new_position, sequence_length)

		%{knot | current_position: new_position}
	end

	defp increment_skip(knot) do
		%{knot | skip: knot.skip + 1}
	end
end

part_one_input =
	if (length(System.argv) == 0) do
		[199, 0, 255, 136, 174, 254, 227, 16, 51, 85, 1, 2, 22, 17, 7, 192]
	else
		System.argv
		|> List.first
		|> String.split(",")
		|> Enum.map(&String.to_integer/1)
	end

part_two_input =
	if (length(System.argv) == 0) do
		"199,0,255,136,174,254,227,16,51,85,1,2,22,17,7,192"
	else
		List.first(System.argv)
	end

part_two_input =
	part_two_input
		|> String.to_charlist
		|> Enum.concat([17, 31, 73, 47, 23])

# Expected answers for default input
# Part one:3770
# Part two: a9d0e68649d0174c8756a59ba21d4dc6


IO.puts("Part one: " <> Integer.to_string(DayTen.part_one(part_one_input)))
part_two_output = DayTen.part_two(part_two_input)
IO.write("Part two: ")
IO.inspect(part_two_output, base: :hex)

A9D0E68649D0174C8756A59BA21D4DC6