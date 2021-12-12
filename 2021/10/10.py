from operator import itemgetter
from pathlib import Path

filename = "10.txt"
path = Path(__file__).parent.joinpath(filename)


class Chunk:
    def __init__(
        self, start: str, end: str, corrupt_score: int, autocomplete_score: int
    ) -> None:
        self.start = start
        self.end = end
        self.corrupt_score = corrupt_score
        self.autocomplete_score = autocomplete_score


chunks = [
    Chunk("(", ")", 3, 1),
    Chunk("[", "]", 57, 2),
    Chunk("{", "}", 1197, 3),
    Chunk("<", ">", 25137, 4),
]

start_map = {chunk.start: chunk for chunk in chunks}
end_map = {chunk.end: chunk for chunk in chunks}

scores = {")"}


def __get_remaining_stack(line: str):
    stack: list[Chunk] = []
    for char in line:
        if char in start_map:
            stack.append(start_map[char])
        elif char == stack[-1].end:
            stack.pop()
        else:
            return (True, [end_map[char]])
    return (False, stack)


def __get_corrupt_score(line: str):
    (is_corrupt, chunks) = __get_remaining_stack(line)

    return chunks[0].corrupt_score if is_corrupt else 0


def __get_autocomplete_score(stack: list[Chunk]):
    score = 0
    while stack:
        score = score * 5 + stack.pop().autocomplete_score

    return score


def __get_part_one(lines: list[str]):
    return sum([__get_corrupt_score(line) for line in lines])


def __get_part_two(lines: list[str]):
    stacks = map(
        itemgetter(1),
        filter(
            lambda result: not result[0],
            [__get_remaining_stack(line) for line in lines],
        ),
    )

    scores = [__get_autocomplete_score(stack) for stack in stacks]
    return sorted(scores)[int(len(scores) / 2)]


def run():
    with open(path) as file:
        lines = [line.strip() for line in file.readlines()]

        part_one = __get_part_one(lines)
        part_two = __get_part_two(lines)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
