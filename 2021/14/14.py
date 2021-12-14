from functools import reduce
from itertools import groupby, pairwise
from pathlib import Path

filename = "14.txt"
path = Path(__file__).parent.joinpath(filename)


def __get_initial_pair_counts(template: str):
    pairs: list[str] = list(map(lambda s: "".join(s), pairwise(template)))

    counts: dict[str, int] = {}
    for pair in pairs:
        if pair not in counts:
            counts[pair] = 0

        counts[pair] += 1

    return counts


def __get_pair_counts(
    rules: dict[str, str], counts: dict[str, int], steps: int
) -> dict[str, int]:
    if steps == 0:
        return counts

    next_counts = {}
    for pair in counts:
        insertion = rules[pair]
        p1 = f"{pair[0]}{insertion}"
        p2 = f"{insertion}{pair[1]}"

        if p1 not in next_counts:
            next_counts[p1] = 0

        if p2 not in next_counts:
            next_counts[p2] = 0

        next_counts[p1] += counts[pair]
        next_counts[p2] += counts[pair]

    return __get_pair_counts(rules, next_counts, steps - 1)


def __get_counts(rules: dict[str, str], template: str, steps: int):
    pair_counts = __get_pair_counts(rules, __get_initial_pair_counts(template), steps)

    counts: dict[str, int] = {template[0]: 1}
    for (pair, count) in pair_counts.items():
        (_, b) = pair
        if b not in counts:
            counts[b] = 0

        counts[b] += count

    return max(counts.values()) - min(counts.values())


def __get_part_one(rules: dict[str, str], template: str):
    return __get_counts(rules, template, 10)


def __get_part_two(rules: dict[str, str], template: str):
    return __get_counts(rules, template, 40)


def run():
    with open(path) as file:
        template = file.readline().strip()

        rules = {
            pair: insertion
            for [pair, insertion] in [
                line.strip().split(" -> ") for line in file.readlines() if line.strip()
            ]
        }

        part_one = __get_part_one(rules, template)
        part_two = __get_part_two(rules, template)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
