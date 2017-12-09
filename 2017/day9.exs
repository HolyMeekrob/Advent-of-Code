defmodule StreamState do
	defstruct score: 0, depth: 0, garbage_characters: 0, in_comment: false, ignore_next: false
end

defmodule DayNine do
	def run(stream) do
		final_state = read_next(stream, %StreamState{})
		{final_state.score, final_state.garbage_characters}
	end

	defp read_next(stream, state) do
		case (IO.read(stream, 1)) do
			:eof -> state
			x -> read_next(stream, process(x, state))
		end 
	end

	defp process(_, %{in_comment: true, ignore_next: true} = state) do
		%{state | ignore_next: false}
	end

	defp process("!", %{in_comment: true} = state) do
		%{state | ignore_next: true}
	end

	defp process(">", %{in_comment: true} = state) do
		%{state | in_comment: false}
	end

	defp process(_, %{in_comment: true} = state) do
		%{state | garbage_characters: state.garbage_characters + 1}
	end

	defp process("<", state) do
		%{state | in_comment: true}
	end

	defp process("{", state) do
		%{state | depth: state.depth + 1}
	end

	defp process("}", state) do
		%{state | score: state.score + state.depth, depth: state.depth - 1}
	end

	defp process(_, state) do
		state
	end
end

input =
	if (length(System.argv) == 0) do
		File.open!("input/day9.txt", [:read, :utf8])
	else
		System.argv
		|> List.first
	end

# Expected answers for default input
# Part one: 17390
# Part two: 7825

result = DayNine.run(input)
IO.puts("Part one: " <> Integer.to_string(elem(result, 0)))
IO.puts("Part two: " <> Integer.to_string(elem(result, 1)))