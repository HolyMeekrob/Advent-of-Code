from functools import reduce
from operator import mul
from pathlib import Path

filename = "09.txt"
path = Path(__file__).parent.joinpath(filename)

Point = tuple[int, int]


def __get_low_points(heightmap: list[list[int]]) -> list[Point]:
    low_points = []
    for (r, row) in enumerate(heightmap):
        for (c, col) in enumerate(row):
            height = heightmap[r][c]
            if (
                (r == 0 or heightmap[r - 1][c] > height)
                and (c == 0 or heightmap[r][c - 1] > height)
                and (r == len(heightmap) - 1 or heightmap[r + 1][c] > height)
                and (c == len(row) - 1 or heightmap[r][c + 1] > height)
            ):
                low_points.append((r, c))

    return low_points


def __get_basin(
    heightmap: list[list[int]], point: Point, points: set[Point] = set()
) -> list[Point]:
    (r, c) = point

    if heightmap[r][c] == 9 or point in points:
        return points

    if not points:
        points = {point}
    else:
        points = points.union({point})

    if r > 0:
        points = points.union(__get_basin(heightmap, (r - 1, c), points))

    if r < (len(heightmap) - 1):
        points = points.union(__get_basin(heightmap, (r + 1, c), points))

    if c > 0:
        points = points.union(__get_basin(heightmap, (r, c - 1), points))

    if c < (len(heightmap[0]) - 1):
        points = points.union(__get_basin(heightmap, (r, c + 1), points))

    return points


def __get_part_one(heightmap: list[list[int]]):
    low_points = __get_low_points(heightmap)
    return sum([heightmap[r][c] + 1 for (r, c) in low_points])


def __get_part_two(heightmap: list[list[int]]):
    low_points = __get_low_points(heightmap)
    basins = [__get_basin(heightmap, point) for point in low_points]
    sizes = sorted([len(basin) for basin in basins], reverse=True)
    return reduce(mul, sizes[:3])


def run():
    with open(path) as file:

        def get_row(line: str):
            return [int(n) for n in line.strip()]

        heightmap = [get_row(line) for line in file.readlines()]

        part_one = __get_part_one(heightmap)
        part_two = __get_part_two(heightmap)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
