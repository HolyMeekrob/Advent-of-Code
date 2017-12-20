defmodule Particle do
	defstruct [:pos, :vel, :acc]
end

defmodule DayTwenty do
	def parse_input(line) do
		regex = ~r/p\=\<(-?\d+),(-?\d+),(-?\d+)\>, v\=\<(-?\d+),(-?\d+),(-?\d+)\>, a\=\<(-?\d+),(-?\d+),(-?\d+)\>/

		[_, px, py, pz, vx, vy, vz, ax, ay, az] = Regex.run(regex, line)
		%Particle
		{
			pos:
			{
				String.to_integer(px),
				String.to_integer(py),
				String.to_integer(pz)
			},
			vel:
			{
				String.to_integer(vx),
				String.to_integer(vy),
				String.to_integer(vz)
			},
			acc:
			{
				String.to_integer(ax),
				String.to_integer(ay),
				String.to_integer(az)
			}
		}
	end

	def part_one(particles) do
		closest_to_origin = 
			particles
			|> get_smallest_for_attribute(:acc)
			|> get_smallest_for_attribute(:vel)
			|> get_smallest_for_attribute(:pos)
			|> Enum.at(0)
		
		Enum.find_index(particles, &(&1 === closest_to_origin))
	end

	defp get_smallest_for_attribute(particles, attr) do
			particles
			|> Enum.map(&(Map.fetch!(&1, attr)))
			|> List.foldl({0, [], nil}, &get_smallest/2)
			|> elem(1)
			|> Enum.map(&(Enum.at(particles, &1)))
	end

	defp get_smallest_acceleration(particle, {index, indexes, min}) do
		acceleration = total(particle.acc)
		cond do
			min === nil ->
				{index + 1, [index], acceleration}

			min === acceleration ->
				{index + 1, [index | indexes ], acceleration}

			acceleration > min ->
				{index + 1, indexes, min}
			
			true ->
				{index + 1, [index], acceleration}
		end
	end

	defp get_smallest(dimensions, {index, indexes, min}) do
		current = total(dimensions)
		cond do
			min === nil ->
				{index + 1, [index], current}

			min === current ->
				{index + 1, [index | indexes ], current}

			min < current ->
				{index + 1, indexes, min}
			
			true ->
				{index + 1, [index], current}
		end
	end

	defp total({x, y, z}) do
		abs(x) + abs(y) + abs(z)
	end
end


input =
	"input/day20.txt"
	|> File.read!
	|> String.split("\r\n")
	|> Enum.map(&DayTwenty.parse_input/1)


# Expected answers for default input
# Part one: 161
# Part two: 

IO.puts("Part one: " <> Integer.to_string(DayTwenty.part_one(input)))