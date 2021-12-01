from functools import partial, reduce
from pathlib import Path
from typing import Callable, Iterable, Tuple

from utils.functional import compose

filename = "01.txt"
path = Path(__file__).parent.joinpath(filename)

Triplet = Tuple[int, int, int]


def count_increasing_numbers(agg: Tuple[int, int], curr: int) -> Tuple[int, int]:
    if agg == None:
        return (0, curr)

    (count, prev) = agg

    return (count + 1, curr) if prev < curr else (count, curr)


def get_increasing_numbers_count(nums: list[int]) -> int:
    return reduce(count_increasing_numbers, nums, None)[0]


def aggregate_triplets(triplets: list[Triplet], curr: int) -> list[Triplet]:
    if not triplets:
        return [(curr, -1, -1)]

    (_, a, b) = triplets[-1]

    return triplets + [(a, b, curr)]


def get_triplets(nums: list[int]) -> list[Triplet]:
    return reduce(aggregate_triplets, nums, [])[2:]


get_sums: Callable[[Iterable[int]], int] = partial(map, sum)

get_increasing_triplets_count: Callable[[list[int]], int] = compose(
    get_increasing_numbers_count, get_sums, get_triplets
)


def get_depths(lines: list[str]) -> list[int]:
    return [int(line) for line in lines]


get_increasing_depths_count: Callable[[list[str]], int] = compose(
    get_increasing_numbers_count, get_depths
)

get_increasing_depth_triplets_count: Callable[[list[str]], int] = compose(
    get_increasing_triplets_count, get_depths
)


with open(path) as file:
    lines = file.readlines()

part_one = get_increasing_depths_count(lines)
part_two = get_increasing_depth_triplets_count(lines)

print(f"Part one: {part_one}")
print(f"Part two: {part_two}")
