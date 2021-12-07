from itertools import groupby
from pathlib import Path

filename = "07.txt"
path = Path(__file__).parent.joinpath(filename)


def __get_part_one(positions: list[int]):
    midpoint = int(len(positions) / 2)
    destination = sorted(positions)[midpoint]
    return sum([abs(position - destination) for position in positions])


def __get_part_two(positions: list[int]):
    def get_fuel_cost(position: int, destination: int):
        fuel_cost = 0
        distance = abs(position - destination)
        while distance != 0:
            fuel_cost = fuel_cost + distance
            distance -= 1

        return fuel_cost

    def get_total_fuel_cost(position_counts):
        return lambda destination: sum(
            [
                get_fuel_cost(position, destination) * count
                for position, count in position_counts
            ]
        )

    positions_sum = sum(positions)
    destinations = [int(positions_sum / len(positions))]
    if positions_sum % 2 == 1:
        destinations.append(destinations[0] + 1)

    position_groups = groupby(sorted(positions))
    position_counts = [(position, len(list(g))) for position, g in position_groups]

    return min(map(get_total_fuel_cost(position_counts), destinations))


def run():
    with open(path) as file:
        positions = [int(val) for val in file.readline().split(",")]

        part_one = __get_part_one(positions)
        part_two = __get_part_two(positions)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
