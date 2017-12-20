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

	def part_two(particles) do
		run(particles, 10000)
		|> length
	end

	defp run(particles, 0) do
		particles
	end

	defp run(particles, num) do
		particles
			|> remove_collisions
			|> Enum.map(&step/1)
			|> run(num - 1)
	end

	defp remove_collisions(particles) do
		particles
		|> Enum.group_by(&(Map.fetch!(&1, :pos)))
		|> Map.values
		|> Enum.filter(&(length(&1) === 1))
		|> Enum.map(&(Enum.at(&1, 0)))
	end

	defp step(%Particle{pos: pos, vel: vel, acc: acc} = particle) do
		vel = add_dimensions(vel, acc)
		pos = add_dimensions(pos, vel)
		%Particle{particle | pos: pos, vel: vel}
	end

	defp add_dimensions({x1, y1, z1}, {x2, y2, z2}) do
		{x1 + x2, y1 + y2, z1 + z2}
	end

	defp get_smallest_for_attribute(particles, attr) do
			particles
			|> Enum.map(&(Map.fetch!(&1, attr)))
			|> List.foldl({0, [], nil}, &get_smallest/2)
			|> elem(1)
			|> Enum.map(&(Enum.at(particles, &1)))
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
# Part two: 438

IO.puts("Part one: " <> Integer.to_string(DayTwenty.part_one(input)))
IO.puts("Part two: " <> Integer.to_string(DayTwenty.part_two(input)))