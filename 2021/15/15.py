import heapq
from itertools import product
from operator import pos
from pathlib import Path
import sys

from utils.iterable import flatten

filename = "15.txt"
path = Path(__file__).parent.joinpath(filename)


class Point:
    def __init__(self, row: int, col: int, risk: int):
        self.position = (row, col)
        self.risk = risk % 9 or 9
        self.distance: int = sys.maxsize
        self.visited: bool = False

    def __repr__(self):
        return f"{self.risk} ({self.distance})"

    def __eq__(self, other) -> bool:
        return self.position == other.position

    def __lt__(self, other) -> bool:
        return (
            self.distance < other.distance
            if self.distance != other.distance
            else self.position < other.position
        )


Grid = list[list[Point]]


def __get_grid(lines: list[str], multiplier: int):
    def get_inner_grid(add_amount: int):
        return [[int(val) + add_amount for val in line] for line in lines]

    grids: list[list[list[int]]] = [[] for _ in range(multiplier)]
    for vertical_count in range(multiplier):
        for horizontal_count in range(multiplier):
            grids[vertical_count].append(
                get_inner_grid(vertical_count + horizontal_count)
            )

    line_count = len(lines)
    value_grid: list[list[int]] = [[] for _ in range(len(lines) * multiplier)]

    for (grid_row_index, grid_row) in enumerate(grids):
        for inner_grid in grid_row:
            for (r, row) in enumerate(inner_grid):
                value_grid[r + (line_count * grid_row_index)].extend(row)

    grid = [
        [Point(r, c, value) for (c, value) in enumerate(row)]
        for (r, row) in enumerate(value_grid)
    ]
    grid[0][0].distance = 0

    return grid


def __visit(grid: Grid, position: tuple[int, int], allow_diagonal: bool):
    (row, col) = position
    current_point = grid[row][col]
    if current_point.visited:
        return []

    neighbors = [
        grid[row + r][col + c]
        for (r, c) in product(range(-1, 2), range(-1, 2))
        if (r, c) != (0, 0)
        and (allow_diagonal or abs(r) + abs(c) < 2)
        and row + r > -1
        and col + c > -1
        and row + r < len(grid)
        and col + c < len(grid[r])
        and (not grid[row + r][col + c].visited)
    ]

    for neighbor in neighbors:
        neighbor.distance = min(
            neighbor.distance, current_point.distance + neighbor.risk
        )

    current_point.visited = True
    return neighbors


def __get_shortest_path(grid: Grid):
    to_visit = []
    start = grid[0][0]
    heapq.heappush(to_visit, start)

    reached_end = False
    while (not reached_end) and len(to_visit) > 0:
        point = heapq.heappop(to_visit)
        neighbors = __visit(grid, point.position, False)
        for neighbor in neighbors:
            if neighbor == grid[-1][-1]:
                reached_end = True
                break
            heapq.heappush(to_visit, neighbor)

    return grid[-1][-1].distance


def __get_part_one(lines: list[str]):
    return __get_shortest_path(__get_grid(lines, 1))


def __get_part_two(lines: list[str]):
    return __get_shortest_path(__get_grid(lines, 5))


def run():
    with open(path) as file:
        lines = [line.strip() for line in file.readlines() if line]

        part_one = __get_part_one(lines)
        part_two = __get_part_two(lines)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
