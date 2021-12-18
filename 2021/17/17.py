import re
from itertools import takewhile
from operator import itemgetter
from pathlib import Path

from utils.iterable import any_pass
from utils.logic import between

filename = "17.txt"
path = Path(__file__).parent.joinpath(filename)


class State:
    def __init__(self, velocity: tuple[int, int]):
        self.velocity = velocity
        self.position = (0, 0)


class Range:
    def __init__(self, x_range: tuple[int, int], y_range: tuple[int, int]):
        self.x_range = sorted(x_range)
        self.y_range = sorted(y_range)

    @property
    def x_start(self):
        return self.x_range[0]

    @property
    def x_end(self):
        return self.x_range[1]

    @property
    def y_start(self):
        return self.y_range[0]

    @property
    def y_end(self):
        return self.y_range[1]

    def __repr__(self):
        return f"({self.x_start}, {self.y_start}) .. ({self.x_end} .. {self.y_end})"


def __is_in_range(position: tuple[int, int], trench: Range):
    (x, y) = position
    return between(x, trench.x_start, trench.x_end) and between(
        y, trench.y_start, trench.y_end
    )


def __get_min_x_velocity(x_coordinate: int):
    range = 0
    velocity = 0
    while range < x_coordinate:
        velocity += 1
        range += velocity

    return velocity


def __get_positions_iterator(x_velocity: int, y_velocity: int):
    x = 0
    y = 0

    while True:
        yield (x, y)
        x = x + x_velocity
        y = y + y_velocity

        if x_velocity != 0:
            x_velocity = x_velocity - 1 if x_velocity > 0 else x_velocity + 1

        y_velocity = y_velocity - 1


def __get_positions(x_velocity: int, y_velocity: int, trench: Range):
    return list(
        takewhile(
            lambda pos: pos[0] <= trench.x_end and pos[1] >= trench.y_start,
            __get_positions_iterator(x_velocity, y_velocity),
        )
    )


def __any_in_range(positions: list[tuple[int, int]], trench: Range):
    return any_pass(lambda position: __is_in_range(position, trench), positions)


# Returns None if the shot can't be made at this x velocity
def __get_max_height_for_made_shot(x_velocity: int, trench: Range):
    if x_velocity > trench.x_end:
        return None

    y_velocity = 1
    max_y = 0
    while True:
        positions = __get_positions(x_velocity, y_velocity, trench)
        if (-1 - y_velocity) < trench.y_start:
            return max_y

        if __any_in_range(positions, trench):
            max_y = max(list(map(itemgetter(1), positions)) + [max_y])

        y_velocity += 1


def __get_valid_y_velocities(x_velocity: int, trench: Range):
    velocities = []

    if x_velocity > trench.x_end:
        return velocities

    y_velocity = trench.y_start

    while True:
        positions = __get_positions(x_velocity, y_velocity, trench)
        if (-1 - y_velocity) < trench.y_start:
            return velocities

        if __any_in_range(positions, trench):
            velocities.append(y_velocity)

        y_velocity += 1


def __get_part_one(trench: Range):
    start_x_velocity = __get_min_x_velocity(trench.x_start)
    max_x_velocity = trench.x_end

    x_velocity = start_x_velocity
    max_height = 0

    while x_velocity <= max_x_velocity:
        max_height = max(max_height, __get_max_height_for_made_shot(x_velocity, trench))
        x_velocity += 1

    return max_height


def __get_part_two(trench: Range):
    start_x_velocity = __get_min_x_velocity(trench.x_start)
    max_x_velocity = trench.x_end

    x_velocity = start_x_velocity
    velocity_counts = 0

    while x_velocity <= max_x_velocity:
        velocity_counts += len(__get_valid_y_velocities(x_velocity, trench))
        x_velocity += 1

    return velocity_counts


def run():
    with open(path) as file:
        line = file.readline()

        input = re.match(
            r"^target area: x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)$", line
        )

        (x1, x2, y1, y2) = map(int, input.groups())

        trench = Range((x1, x2), (y1, y2))

        part_one = __get_part_one(trench)
        part_two = __get_part_two(trench)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
