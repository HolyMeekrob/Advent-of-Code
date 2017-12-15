import Bitwise

defmodule DayFifteen do
	def part_one(pair) do
		accept? = fn(_) -> true end
		count_matches(pair, 0, accept?, accept?, 40000000)
	end

	def part_two(pair) do
		accept_a? = fn(a) -> rem(a, 4) === 0 end
		accept_b? = fn(b) -> rem(b, 8) === 0 end
		count_matches(pair, 0, accept_a?, accept_b?, 5000000)
	end

	defp count_matches(_, count, _, _, 0) do
		count
	end

	defp count_matches({a, b}, count, a_ok?, b_ok?, generations_remaining) do
		a = get_next_a(a, a_ok?)
		b = get_next_b(b, b_ok?)

		count =
			if (nums_match?(a, b)) do
				count + 1
			else
				count
			end

		count_matches({a, b}, count, a_ok?, b_ok?, generations_remaining - 1)
	end

	defp nums_match?(a, b) do
		get_low_16(a) === get_low_16(b)
	end

	defp get_low_16(num) do
		# 65535 === 0b1111111111111111
		num &&& 65535
	end

	defp get_next_a(a, a_ok?) do
		get_next(a, 16807, a_ok?)
	end

	defp get_next_b(b, b_ok?) do
		get_next(b, 48271, b_ok?)
	end

	defp get_next(num, factor, ok?) do
		next = rem(num * factor, 2147483647)
		if (ok?.(next)) do
			next
		else
			get_next(next, factor, ok?)
		end
	end
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
# Part one: 592
# Part two: 320

IO.puts("Part one: " <> Integer.to_string(DayFifteen.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DayFifteen.part_two(input)))