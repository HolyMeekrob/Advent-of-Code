defmodule DaySix do
	def part_one(banks) do
		historical_banks = :array.new()
		historical_banks = :array.set(0, banks, historical_banks)
		count_reallocations(banks, historical_banks, :array.size(banks))
	end

	def part_two(banks) do

	end

	defp count_reallocations(banks, historical_banks, bank_count, reallocation_count \\ 1) do
		new_banks = reallocate(banks)
		if (is_repeated(new_banks, historical_banks)) do
			reallocation_count
		else
			historical_banks = :array.set(:array.size(historical_banks), new_banks, historical_banks)
			count_reallocations(new_banks, historical_banks, bank_count, reallocation_count + 1)
		end
	end

	defp is_repeated(banks, historical_banks) do
		elements_match =
			fn(idx, val, {arr, all_match}) ->
				{arr, (all_match and (val === :array.get(idx, arr)))}
			end
			
		array_equals =
			fn(arr1, arr2) ->
				(:array.size(arr1) === :array.size(arr2))
					and elem(:array.foldl(elements_match, {arr2, true}, arr1), 1)
			end

		array_contains =
			fn(_, banks_snapshot, previous_match) ->
				previous_match or array_equals.(banks_snapshot, banks)
			end

		:array.foldl(array_contains, false, historical_banks)
	end

	defp reallocate(banks) do
		max_index = get_max_index(banks)
		max_value = :array.get(max_index, banks)
		banks = :array.set(max_index, 0, banks)
		increment_banks(banks, max_index + 1, max_value)
	end

	defp get_max_index(banks) do
		acc_fn = fn(current_index, bank, max_index) ->
			if (bank > :array.get(max_index, banks)) do
				current_index
			else
				max_index
			end
		end

		:array.foldl(acc_fn, 0, banks)
	end

	defp increment_banks(banks, _, 0) do
		banks
	end

	defp increment_banks(banks, index, remaining_blocks) do
		index =
			if (index > :array.size(banks) - 1) do
				0
			else
				index
			end

		banks = :array.set(index, :array.get(index, banks) + 1, banks)
		increment_banks(banks, index + 1, remaining_blocks - 1)
	end
end

input =
	if (length(System.argv) == 0) do
		:array.from_list([0, 5, 10, 0, 11, 14, 13, 4, 11, 8, 8, 7, 1, 4, 12, 11])
	else
		System.argv
		|> List.first
		|> String.split
		|> Enum.map(&String.to_integer/1)
		|> :array.from_list
	end

# Expected answers for default input
# Part one: 7864
# Part two: 

IO.puts("Part one: " <> Integer.to_string(DaySix.part_one(input)))
#IO.puts("Part two: " <> Integer.to_string(DayFive.part_two(input)))