defmodule State do
	defstruct [:data, :index, :instruction]
end

defmodule DayTwentyFive do
	def run(step_count) do
		initialize()
		|> step(step_count)
		|> Map.fetch!(:data)
		|> Enum.filter(&(&1 === 1))
		|> Kernel.length
	end

	defp initialize() do
		%State{data: [] , index: 0, instruction: :a}
	end

	defp step(%State{} = state, 0) do
		state
	end

	defp step(%State{} = state, count) do
		state
		|> update_data
		|> process
		|> step(count - 1)
	end

	defp process(%State{data: data, index: index, instruction: :a} = state) do
		current_val = Enum.at(data, index)
		if (current_val === 0) do
			%{state |
				data: List.replace_at(data, index, 1),
				index: index + 1,
				instruction: :b
			}
		else
			%{state |
				data: List.replace_at(data, index, 0),
				index: index - 1,
				instruction: :c
			}
		end
	end

	defp process(%State{data: data, index: index, instruction: :b} = state) do
		current_val = Enum.at(data, index)
		if (current_val === 0) do
			%{state |
				data: List.replace_at(data, index, 1),
				index: index - 1,
				instruction: :a
			}
		else
			%{state |
				index: index + 1,
				instruction: :d
			}
		end
	end

	defp process(%State{data: data, index: index, instruction: :c} = state) do
		current_val = Enum.at(data, index)
		if (current_val === 0) do
			%{state |
				data: List.replace_at(data, index, 1),
				index: index + 1,
				instruction: :a
			}
		else
			%{state |
				data: List.replace_at(data, index, 0),
				index: index - 1,
				instruction: :e
			}
		end
	end

	defp process(%State{data: data, index: index, instruction: :d} = state) do
		current_val = Enum.at(data, index)
		if (current_val === 0) do
			%{state |
				data: List.replace_at(data, index, 1),
				index: index + 1,
				instruction: :a
			}
		else
			%{state |
				data: List.replace_at(data, index, 0),
				index: index + 1,
				instruction: :b
			}
		end
	end

	defp process(%State{data: data, index: index, instruction: :e} = state) do
		current_val = Enum.at(data, index)
		if (current_val === 0) do
			%{state |
				data: List.replace_at(data, index, 1),
				index: index - 1,
				instruction: :f
			}
		else
			%{state |
				index: index - 1,
				instruction: :c
			}
		end
	end

	defp process(%State{data: data, index: index, instruction: :f} = state) do
		current_val = Enum.at(data, index)
		if (current_val === 0) do
			%{state |
				data: List.replace_at(data, index, 1),
				index: index + 1,
				instruction: :d
			}
		else
			%{state |
				index: index + 1,
				instruction: :a
			}
		end
	end

	defp update_data(%State{data: data, index: -1} = state)  do
		%State{state | data: [0 | data], index: 0}
	end

	defp update_data(%State{data: d, index: i} = state) when i === length(d) do
		%State{state | data: d ++ [0]}
	end

	defp update_data(%State{} = state) do
		state
	end
end

input =
	if (length(System.argv) == 0) do
		12173597
	else
		String.to_integer(List.first(System.argv))
	end

# Expected answer for default input: 2870

IO.puts("Result: " <> Integer.to_string(DayTwentyFive.run(input)))