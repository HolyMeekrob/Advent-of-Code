defmodule DayFourteen do
	def part_one(input) do
		rows = 0..127
		|> Enum.map(&(input <> "-" <> Integer.to_string(&1)))
		|> Enum.map(&String.to_charlist/1)
		|> Enum.map(&DayTen.part_two/1)
		|> Enum.map(&(to_strings(&1, 2, 8)))
		|> List.foldl(0, &count_ones/2)
	end

	defp to_strings(nums, base, pad_count) do
		nums
		|> Enum.map(&(Integer.to_string(&1, base)))
		|> Enum.map(&(String.pad_leading(&1, pad_count, "0")))
	end

	defp count_ones(nums, sum) do
		nums
		|> Enum.join
		|> String.replace("0", "")
		|> String.length
		|> Kernel.+(sum)
	end
end


# Expected answers for default input
# Part one:8074
# Part two: 

Code.load_file("day10.exs")
input = "jzgqcdpd"
IO.puts("")
IO.puts("Part one: " <> Integer.to_string(DayFourteen.part_one(input)))