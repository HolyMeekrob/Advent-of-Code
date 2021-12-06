from itertools import groupby
from pathlib import Path

filename = "06.txt"
path = Path(__file__).parent.joinpath(filename)

timer = 7
initial_cycle_bonus = 2


def __cycle(fish_groups: list[int]):
    size = len(fish_groups)
    fish = [0] * size

    fish = fish_groups[1:]
    fish[timer - 1] = fish[timer - 1] + fish_groups[0]
    fish.append(fish_groups[0])

    return fish


def __get_lantern_fish_population(initial_groups: list[int], days: int):
    fish = initial_groups.copy()
    for _ in range(days):
        fish = __cycle(fish)

    return sum(fish)


def run():
    fish = [0] * (timer + initial_cycle_bonus)

    with open(path) as file:
        ungrouped = [int(num) for num in file.readline().split(",")]
        groups = {k: len(list(g)) for (k, g) in groupby(sorted(ungrouped))}

        for i in range(len(fish)):
            if i in groups:
                fish[i] = groups[i]

        part_one = __get_lantern_fish_population(fish, 80)
        part_two = __get_lantern_fish_population(fish, 256)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
