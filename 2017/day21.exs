defmodule Rule do
	defstruct [:in, :out]
end

defmodule DayTwentyOne do
	def parse_rule(line) do
		{regex, in_length} =
			if (String.length(line) === 20) do
				{~r/([.#]{2})\/([.#]{2}) \=\> ([.#]{3})\/([.#]{3})\/([.#]{3})/, 2}
			else
				{~r/([.#]{3})\/([.#]{3})\/([.#]{3}) \=\> ([.#]{4})\/([.#]{4})\/([.#]{4})\/([.#]{4})/, 3}
			end

		[_ | lines] = Regex.run(regex, line)

		parse_lines(lines, in_length)
	end

	defp parse_lines(lines, in_length) do
		lines_in = Enum.take(lines, in_length)
		lines_out = Enum.drop(lines, in_length)

		in_vals = Enum.map(lines_in, &convert_input_characters/1)
		out_vals = Enum.map(lines_out, &convert_input_characters/1)

		%Rule{in: in_vals, out: out_vals}
	end

	defp convert_input_characters(characters) do
		characters
		|> String.to_charlist
		|> Enum.map(&(&1 === ?#))
	end

	def part_one(rules) do
		get_initial_state()
		|> step(rules, 1)

	end

	defp get_initial_state do
		[
			[false, true, false],
			[false, false, true],
			[true, true, true]
		]
	end

	defp step(state, _, 0) do
		state
	end

	defp step(state, rules, count) do
		state
		|> enhance(rules)
		|> step(rules, count - 1)
	end

	defp enhance(state, rules) do
		state
		|> sections
		|> Enum.map(&(process(&1, rules)))
		# |> merge
	end

	defp sections(state) do
		num =
			if (rem(length(state), 2) === 0) do
				2
			else
				3
			end

		state
		|> Enum.map(&Enum.chunk_every(&1, num))
		|> Enum.chunk_every(num)
		|> Enum.map(&List.zip/1)
		|> flatten
		|> Enum.map(&Tuple.to_list/1)
	end

	defp flatten(lst) do
		List.foldl(lst, [], &(&2 ++ &1))
	end

	defp process(section, rules) do
		rules
		|> Map.fetch!(length(section))
		|> Enum.find(&(rule_matches?(&1, section)))
		|> Map.fetch!(:out)
	end

	defp merge(sections) do
		
	end

	defp rule_matches?(%Rule{in: input}, section) do
		(section === input)
			or (section === flip_vertical(input))
			or (section === flip_horizontal(input))
			or (section === rotate(input, 1))
			or (section === rotate(input, 2))
			or (section === rotate(input, 3))
	end

	defp flip_vertical(grid) do
		Enum.reverse(grid)
	end

	defp flip_horizontal(grid) do
		Enum.map(&Enum.reverse/1)
	end

	defp rotate(grid, 0) do
		grid
	end

	defp rotate(grid, num) do
		grid
		|> List.zip
		|> Enum.map(&Tuple.to_list/1)
		|> Enum.map(&Enum.reverse/1)
		|> rotate(num - 1)
	end
end


input =
	"input/day21.txt"
	|> File.read!
	|> String.split("\r\n")
	|> Enum.map(&DayTwentyOne.parse_rule/1)
	|> Enum.group_by(&(length(Map.fetch!(&1, :in))))

DayTwentyOne.part_one(input)