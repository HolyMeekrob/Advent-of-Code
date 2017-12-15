import Bitwise

defmodule DayFifteen do
	def part_one(pair) do
		count_matches(pair, 0, 40000000)
	end

	defp count_matches(_, count, 0) do
		count
	end

	defp count_matches({a, b}, count, generations_remaining) do
		a = get_next_a(a)
		b = get_next_b(b)

		# IO.inspect(a)
		# IO.inspect(b)
		
		# IO.puts(display_binary(a))
		# IO.puts(display_binary(b))
		# IO.puts("")

		count =
			if (get_low_16(a) === get_low_16(b)) do
				count + 1
			else
				count
			end

		count_matches({a, b}, count, generations_remaining - 1)
	end

	defp get_low_16(num) do
		# 65535 === 0b1111111111111111
		num &&& 65535
	end

	defp get_next_a(a) do
		get_next(a, 16807)
	end

	defp get_next_b(b) do
		get_next(b, 48271)
	end

	defp get_next(num, factor) do
		rem(num * factor, 2147483647)
	end

	# defp display_binary(num) do
	# 	Integer.to_string(num, 2)
	# 	|> String.pad_leading(32, "0")
	# end
end


input = {277, 349}
input =
	if (length(System.argv) == 0) do
		input
	else
		[a | [b | _]] = System.argv
		{
			String.to_integer(a),
			String.to_integer(b)
		}
	end

# Expected answers for default input
# Part one: 
# Part two: 

IO.puts("Part one: " <> Integer.to_string(DayFifteen.part_one(input)))
# IO.puts("Part two: " <> Integer.to_string(DayFifteen.part_two(input)))