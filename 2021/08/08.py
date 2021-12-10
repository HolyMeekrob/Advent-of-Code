from __future__ import annotations
from functools import reduce
from itertools import groupby
from pathlib import Path

from utils.dict import flip
from utils.iterable import first, flatten

filename = "08.txt"
path = Path(__file__).parent.joinpath(filename)


class Pattern:
    def __init__(self, pattern: str):
        self.letters = {letter for letter in pattern}

    def issubset(self, pattern: Pattern):
        return self.letters.issubset(pattern.letters)

    def issuperset(self, pattern: Pattern):
        return self.letters.issuperset(pattern.letters)

    def difference(self, pattern: Pattern):
        return Pattern("".join(self.letters.difference(pattern)))

    def union(self, pattern: Pattern):
        return Pattern(self.letters.union(pattern.letters))

    def __len__(self):
        return len(self.letters)

    def __str__(self):
        return "".join(sorted(self.letters))

    def __eq__(self, __o: object) -> bool:
        return isinstance(__o, Pattern) and self.letters == __o.letters

    def __iter__(self):
        yield from self.letters

    def __contains__(self, key):
        return key in self.letters


class Display:
    def __init__(self, line: tuple[str, str]):
        def get_patterns(input: str):
            return [Pattern(pattern) for pattern in input.split(" ")]

        (patterns, output) = line
        self.patterns = get_patterns(patterns)
        self.output = get_patterns(output)

    def __str__(self):
        return f"Patterns: {' '.join(map(str, self.patterns))} | Output: {' '.join(map(str, self.output))}"


digits = [
    Pattern("abcefg"),
    Pattern("cf"),
    Pattern("acdeg"),
    Pattern("acdfg"),
    Pattern("bcdf"),
    Pattern("abdfg"),
    Pattern("abdefg"),
    Pattern("acf"),
    Pattern("abcdefg"),
    Pattern("abcdfg"),
]


def __union_all(*patterns: Pattern):
    def union(pattern_union: Pattern, pattern: Pattern):
        return pattern_union.union(pattern)

    return reduce(union, patterns)


def __get_mappings(display: Display):
    def get_pattern_with_length(patterns: list[Pattern], length: int):
        return first(patterns, lambda pattern: len(pattern) == length)

    all_letters = "abcdefg"

    one: Pattern = get_pattern_with_length(display.patterns, 2)
    four: Pattern = get_pattern_with_length(display.patterns, 4)
    seven: Pattern = get_pattern_with_length(display.patterns, 3)
    eight: Pattern = get_pattern_with_length(display.patterns, 7)
    nine: Pattern = first(
        display.patterns, lambda pattern: len(pattern) == 6 and pattern.issuperset(four)
    )
    two: Pattern = first(
        display.patterns,
        lambda pattern: len(pattern) == 5 and not (pattern.issubset(nine)),
    )

    mappings = {letter: Pattern(all_letters) for letter in all_letters}

    mappings["a"] = seven.difference(one)
    mappings["b"] = eight.difference(two.union(one))
    mappings["f"] = eight.difference(two.union(mappings["b"]))
    mappings["c"] = one.difference(mappings["f"])
    mappings["e"] = eight.difference(nine)
    mappings["g"] = nine.difference(seven.union(four))
    mappings["d"] = eight.difference(
        __union_all(seven, mappings["b"], mappings["e"], mappings["g"])
    )

    return {k: str(v) for (k, v) in mappings.items()}


def __get_output_value_sets(display: Display):
    original_to_crossed = __get_mappings(display)
    crossed_to_original = flip(original_to_crossed)

    def get_number(pattern: Pattern):
        mapped_pattern = __union_all(
            *[Pattern(crossed_to_original[letter]) for letter in pattern]
        )
        return digits.index(mapped_pattern)

    numbers = list(map(get_number, display.output))

    return sum([number * pow(10, i) for (i, number) in enumerate(reversed(numbers))])


def __get_part_one(displays: list[Display]):
    counts = sorted([len(digit) for digit in digits])
    unique_counts = [count for (count, g) in groupby(counts) if len(list(g)) == 1]

    all_patterns = flatten([display.output for display in displays])
    pattern_counts = groupby(sorted([len(pattern) for pattern in all_patterns]))
    return sum(
        [len(list(g)) for (count, g) in pattern_counts if count in unique_counts]
    )


def __get_part_two(displays: list[Display]):
    return sum([__get_output_value_sets(display) for display in displays])


def run():
    with open(path) as file:
        lines = [
            (patterns.strip(), output.strip())
            for (patterns, output) in map(
                lambda line: line.split("|"), file.readlines()
            )
        ]

        displays = [Display(line) for line in lines]
        part_one = __get_part_one(displays)
        part_two = __get_part_two(displays)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
