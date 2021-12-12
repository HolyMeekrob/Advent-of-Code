from pathlib import Path
from typing import Iterable

from utils.iterable import flatten

filename = "11.txt"
path = Path(__file__).parent.joinpath(filename)


class Energy:
    def __init__(self, level):
        self.level = level
        self.flashes = 0

    def increment(self):
        self.level += 1

    def process(self):
        if self.level > 9:
            self.level = 0
            self.flashes += 1

    def __repr__(self):
        return f"{self.level} ({self.flashes})"


Grid = list[list[Energy]]


def __initialize_grid(levels: Iterable[Iterable[int]]) -> Grid:
    return [list(map(Energy, level)) for level in [row for row in levels]]


def __step_energy(grid: Grid, r: int, c: int):
    if r < 0 or c < 0 or r >= len(grid) or c >= len(grid[r]):
        return

    energy = grid[r][c]
    energy.increment()

    if energy.level == 10:
        __step_energy(grid, r - 1, c - 1)
        __step_energy(grid, r - 1, c)
        __step_energy(grid, r - 1, c + 1)
        __step_energy(grid, r, c - 1)
        __step_energy(grid, r, c + 1)
        __step_energy(grid, r + 1, c - 1)
        __step_energy(grid, r + 1, c)
        __step_energy(grid, r + 1, c + 1)


def __step_grid(grid: Grid):
    for (r, row) in enumerate(grid):
        for (c, _) in enumerate(row):
            __step_energy(grid, r, c)

    for (r, row) in enumerate(grid):
        for (c, _) in enumerate(row):
            grid[r][c].process()


def __get_total_flashes(grid: Grid):
    return sum(
        flatten(
            [
                map(lambda energy: energy.flashes, energies)
                for energies in [row for row in grid]
            ]
        )
    )


def __all_flashed(grid: Grid):
    return all(energy.level == 0 for energy in flatten(grid))


def __get_part_one(grid: Grid):
    n = 0
    while n < 100:
        __step_grid(grid)
        n += 1

    return __get_total_flashes(grid)


def __get_part_two(grid: Grid):
    n = 0
    while not __all_flashed(grid):
        __step_grid(grid)
        n += 1

    return n


def run():
    with open(path) as file:
        levels = [
            list(map(int, line)) for line in [line.strip() for line in file.readlines()]
        ]

        part_one = __get_part_one(__initialize_grid(levels))
        part_two = __get_part_two(__initialize_grid(levels))

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
