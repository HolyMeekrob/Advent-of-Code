from enum import Enum, auto
from functools import reduce
from pathlib import Path
import re

from utils.iterable import flatten

filename = "13.txt"
path = Path(__file__).parent.joinpath(filename)

Point = tuple[int, int]
Grid = list[list[bool]]


class Direction(Enum):
    HORIZONTAL = (auto(),)
    VERTICAL = auto()


class Instruction:
    def __init__(self, line: str):
        pattern = re.compile("([x|y])=(\d+)")
        (direction, axis) = pattern.search(line).groups()

        self.direction = (
            Direction.VERTICAL if direction == "x" else Direction.HORIZONTAL
        )
        self.axis = int(axis)

    def __str__(self):
        return f"{self.direction} along {self.axis}"


def __initialize_grid(points: list[Point], width: int, height: int) -> Grid:
    grid = [False] * height
    for y in range(height):
        grid[y] = [False] * width

    for (x, y) in points:
        grid[y][x] = True

    return grid


def __get_printable_grid(grid: Grid):
    s = "\n"
    for row in grid:
        for col in row:
            if col:
                s += "#"
            else:
                s += "."
        s += "\n"

    return s


def __either(t: tuple[bool, bool]):
    return t[0] or t[1]


def __fold_horizontally(axis: int, grid: Grid) -> Grid:
    top = grid[:axis]
    bottom = grid[(axis + 1) :]

    if len(top) != len(bottom):
        raise Exception("Horizontally folded grid lengths do not match")

    halves = zip(top, reversed(bottom))

    return [list(map(__either, zip(a, b))) for (a, b) in halves]


def __fold_vertically(axis: int, grid: Grid) -> Grid:
    left = list(map(lambda row: row[:axis], grid))
    right = list(map(lambda row: row[(axis + 1) :], grid))

    if len(left) != len(right):
        raise Exception("Vertically folded grid lengths do not match")

    halves = zip(left, map(reversed, right))

    return [list(map(__either, zip(a, b))) for (a, b) in halves]


def __run_instruction(grid: Grid, instruction: Instruction) -> Grid:
    return (
        __fold_horizontally(instruction.axis, grid)
        if instruction.direction == Direction.HORIZONTAL
        else __fold_vertically(instruction.axis, grid)
    )


def __get_part_one(grid: Grid, instructions: list[Instruction]):
    result = reduce(__run_instruction, instructions, grid)

    points = list(flatten(result))
    return sum([1 for point in points if point])


def __get_part_two(grid: Grid, instructions: list[Instruction]):
    result = reduce(__run_instruction, instructions, grid)

    return __get_printable_grid(result)


def run():
    with open(path) as file:
        lines = [line.strip() for line in file.readlines()]

        point_pattern = re.compile("^(\d+),(\d+)")
        points = [
            tuple(map(int, point_pattern.match(line).groups()))
            for line in lines
            if point_pattern.match(line)
        ]

        file.seek(0, 0)
        instruction_pattern = re.compile("^fold along")
        instructions = [
            Instruction(line.strip())
            for line in file.readlines()
            if instruction_pattern.match(line)
        ]

        width = (
            max(
                instruction.axis
                for instruction in instructions
                if instruction.direction == Direction.VERTICAL
            )
            * 2
            + 1
        )
        height = (
            max(
                instruction.axis
                for instruction in instructions
                if instruction.direction == Direction.HORIZONTAL
            )
            * 2
            + 1
        )

        grid = __initialize_grid(points, width, height)

        part_one = __get_part_one(grid, instructions[:1])
        part_two = __get_part_two(grid, instructions)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
