from dataclasses import dataclass
from pathlib import Path

from utils.iterable import flatten

filename = "05.txt"
path = Path(__file__).parent.joinpath(filename)


@dataclass
class Point:
    x: int
    y: int

    def __str__(self):
        return f"({self.x}, {self.y})"


@dataclass
class LineSegment:
    start: Point
    end: Point

    def __str__(self):
        return f"{str(self.start)} -> {str(self.end)}"

    def __repr__(self):
        return self.__str__()

    def is_horizontal(self):
        return self.start.y == self.end.y

    def is_vertical(self):
        return self.start.x == self.end.x

    def get_path(self):
        delta = (
            abs(self.end.x - self.start.x)
            if self.start.x != self.end.x
            else abs(self.end.y - self.start.y)
        )

        x_step = int((self.end.x - self.start.x) / delta)
        y_step = int((self.end.y - self.start.y) / delta)

        return [
            Point(self.start.x + (x_step * index), self.start.y + (y_step * index))
            for index in range(0, delta + 1)
        ]


def __get_coord(nums: str):
    return tuple(map(int, nums.split(",")))


def __get_coords(line: str):
    return map(__get_coord, line.split(" -> "))


def __print_grid(grid: list[list[int]]):
    for row in grid:
        print(",".join(map(str, row)))


def __get_overlapped_points_count(segments: list[LineSegment]):
    width = max([max(seg.start.x, seg.end.x) for seg in segments]) + 1
    height = max([max(seg.start.y, seg.end.y) for seg in segments]) + 1

    grid = [[0] * width for _ in range(height)]

    for segment in segments:
        for point in segment.get_path():
            grid[point.y][point.x] += 1

    # __print_grid(grid)
    # print("\n")
    return sum([1 for val in flatten(grid) if val > 1])


def __get_part_one(line_segments: list[LineSegment]):
    return __get_overlapped_points_count(
        [
            line_segment
            for line_segment in line_segments
            if line_segment.is_horizontal() or line_segment.is_vertical()
        ]
    )


def __get_part_two(line_segments: list[LineSegment]):
    return __get_overlapped_points_count(line_segments)


def run():
    with open(path) as file:
        line_segments = [
            LineSegment(Point(x1, y1), Point(x2, y2))
            for ((x1, y1), (x2, y2)) in map(__get_coords, file.readlines())
        ]
        part_one = __get_part_one(line_segments)
        part_two = __get_part_two(line_segments)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
