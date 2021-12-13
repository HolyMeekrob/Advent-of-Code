from pathlib import Path
import re

from utils.functional import identity
from utils.iterable import flatten


filename = "12.txt"
path = Path(__file__).parent.joinpath(filename)

Connections = dict[str, list[str]]


def __get_connections(lines: list[str]):
    pattern = re.compile(r"(\w+)-(\w+)")

    def get_connection(line: str) -> tuple[str, str]:
        return pattern.match(line).groups()

    connections: Connections = {}
    for line in lines:
        (a, b) = get_connection(line)
        if a not in connections:
            connections[a] = []
        if b not in connections:
            connections[b] = []

        if b != "start":
            connections[a].append(b)

        if a != "start":
            connections[b].append(a)

    del connections["end"]

    return connections


def __is_small(cave: str):
    return cave.islower()


def __get_paths(
    connections: Connections,
    repeat_small_max: int,
    cave: str = "start",
    caves: list[str] = [],
):
    smalls = list(filter(__is_small, caves))
    repeats = len(smalls) - len(set(smalls))

    if __is_small(cave) and repeats >= repeat_small_max and cave in caves:
        return []

    path = caves + [cave]

    if cave == "end":
        return [path]

    paths = []
    for next_cave in connections[cave]:
        paths = paths + __get_paths(connections, repeat_small_max, next_cave, path)

    return list(filter(identity, paths))


def __get_part_one(connections: Connections):
    paths = __get_paths(connections, 0)
    return len(paths)


def __get_part_two(connections: Connections):
    paths = __get_paths(connections, 1)
    return len(paths)


def run():
    with open(path) as file:
        lines = [line.strip() for line in file.readlines()]
        connections = __get_connections(lines)

        part_one = __get_part_one(connections)
        part_two = __get_part_two(connections)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
