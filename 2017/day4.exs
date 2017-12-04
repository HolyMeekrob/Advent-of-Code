defmodule DayFour do
	def part_one(passphrases) do
		passphrases
		|> Enum.filter(&is_valid_one/1)
		|> length
	end

	def part_two(passphrases) do
		passphrases
		|> Enum.filter(&is_valid_two/1)
		|> length
	end

	defp is_valid_one(passphrase) do
		words = String.split(passphrase)
		length(Enum.uniq(words)) == length(words)
	end

	defp is_valid_two(passphrase) do
		sort_string = fn(str) ->
			str
			|> to_charlist
			|> Enum.sort
			|> to_string
		end

		words =
			passphrase
			|> String.split
			|> Enum.map(sort_string)

		length(Enum.uniq(words)) == length(words)
	end
end

input =
	if (length(System.argv) == 0) do
		"input/day4.txt"
			|> File.read!
			|> String.split("\r\n")
	else
		System.argv
		|> List.first
		|> String.split("\r\n")
	end

# Expected answers for default input
# Part one: 451
# Part two: 223

IO.puts("Part one: " <> Integer.to_string(DayFour.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DayFour.part_two(input)))