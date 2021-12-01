from functools import partial, reduce
from pathlib import Path
import time
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


with open(path) as file:
    depths = list(map(int, file.readlines()))

start_slow = time.perf_counter_ns()
part_one = get_increasing_numbers_count(depths)
part_two = get_increasing_triplets_count(depths)
end_slow = time.perf_counter_ns()

start_quick = time.perf_counter_ns()
part_one_quick = sum(
    [1 for index in range(1, len(depths)) if depths[index - 1] < depths[index]]
)

part_two_quick = sum(
    [1 for index in range(3, len(depths)) if depths[index] > depths[index - 3]]
)
end_quick = time.perf_counter_ns()

slow_time = end_slow - start_slow
quick_time = end_quick - start_quick

print(f"Part one (slow): {part_one}")
print(f"Part two (slow): {part_two}")
print(f"Slow timer: {slow_time} ns")

print()

print(f"Part one (quick): {part_one_quick}")
print(f"Part two (quick): {part_two_quick}")
print(f"Quick timer: {quick_time} ns")

print()

print(f"Slow was {round(((slow_time - quick_time) / quick_time) * 100, 2)}% slower")
