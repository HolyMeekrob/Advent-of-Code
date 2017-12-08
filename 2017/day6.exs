defmodule DaySix do
	def run(banks) do
		historical_banks = :array.new()
		historical_banks = :array.set(0, banks, historical_banks)
		count_reallocations(banks, historical_banks)
	end

	defp count_reallocations(banks, historical_banks, reallocation_count \\ 1) do
		new_banks = reallocate(banks)
		repeat_gap = get_repeat_gap(new_banks, historical_banks)

		if (repeat_gap > 0) do
			{reallocation_count, repeat_gap}
		else
			historical_banks = :array.set(:array.size(historical_banks), new_banks, historical_banks)
			count_reallocations(new_banks, historical_banks, reallocation_count + 1)
		end
	end

	defp get_repeat_gap(banks, historical_banks) do
		result = :array.foldl(&array_contains/3, {banks, -1}, historical_banks)
		if(elem(result, 1) > -1) do
			:array.size(historical_banks) - elem(result, 1)
		else
			-1
		end
	end

	defp array_contains(_, _, {banks, match_index}) when match_index > -1 do
		{banks, match_index}
	end

	defp array_contains(index, banks_snapshot, {banks, _}) do
		elements_match =
			fn(idx, val, {arr, all_match}) ->
				{arr, (all_match and (val === :array.get(idx, arr)))}
			end

		array_equals =
			fn(arr1, arr2) ->
				(:array.size(arr1) === :array.size(arr2))
					and elem(:array.foldl(elements_match, {arr2, true}, arr1), 1)
			end

		if (array_equals.(banks_snapshot, banks)) do
				{banks, index}
		else
			{banks, -1}
		end
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
# Part two: 1695

result = DaySix.run(input)
IO.puts("Part one: " <> Integer.to_string(elem(result, 0)))
IO.puts("Part two: " <> Integer.to_string(elem(result, 1)))