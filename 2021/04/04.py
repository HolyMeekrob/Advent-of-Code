from pathlib import Path
from itertools import chain

from utils.iterable import flatten

filename = "04.txt"
path = Path(__file__).parent.joinpath(filename)


class Board:
    def __init__(self, numbers: list[list[int]]):
        self.numbers = numbers
        self.row_call_counts = [0] * 5
        self.column_call_counts = [0] * 5

    def get_all_numbers(self):
        return flatten(self.numbers)

    def mark_number(self, draw: int):
        for (r, col) in enumerate(self.numbers):
            for (c, num) in enumerate(col):
                if num == draw:
                    self.row_call_counts[r] += 1
                    self.column_call_counts[c] += 1
                    break
            else:
                continue

            break

    def reset(self):
        self.row_call_counts = [0] * 5
        self.column_call_counts = [0] * 5

    def is_winner(self):
        return 5 in chain(self.row_call_counts, self.column_call_counts)


def __get_numbers(nums: str, separator: str):
    num_arr = [num for num in nums.split(separator) if num]
    return list(map(int, num_arr))


def __get_score(draws: list[int], board: Board):
    return draws[-1] * sum([n for n in board.get_all_numbers() if n not in draws])


def __get_part_one(
    draws: list[int], boards: list[Board], lookup: dict[Board, list[int]]
):
    winner = None
    completed_draws = None

    for (i, draw) in enumerate(draws):
        matching_boards = lookup[draw]
        for matching_board in matching_boards:
            boards[matching_board].mark_number(draw)
            if boards[matching_board].is_winner():
                winner = boards[matching_board]
                completed_draws = draws[: i + 1]
                break

        else:
            continue

        break

    return __get_score(completed_draws, winner)


def __get_part_two(
    draws: list[int], boards: list[Board], lookup: dict[Board, list[int]]
):
    for board in boards:
        board.reset()

    loser = None
    completed_draws = None
    remaining_boards = list(range(len(boards)))

    for (i, draw) in enumerate(draws):
        matching_boards = lookup[draw]
        for matching_board in matching_boards:
            boards[matching_board].mark_number(draw)
            if (
                boards[matching_board].is_winner()
                and matching_board in remaining_boards
            ):
                remaining_boards.remove(matching_board)
                if len(remaining_boards) == 0:
                    loser = boards[matching_board]
                    completed_draws = draws[: i + 1]
                    break
        else:
            continue

        break

    return __get_score(completed_draws, loser)


def run():
    with open(path) as file:
        boards: list[Board] = []
        lookup: dict[Board, list[int]] = {}

        draws = __get_numbers(file.readline(), ",")

        index = 0
        while file.readline():
            board = Board([__get_numbers(file.readline(), " ") for x in range(5)])
            boards.append(board)

            for number in board.get_all_numbers():
                if number in lookup:
                    lookup[number].append(index)
                else:
                    lookup[number] = [index]
            index += 1

        part_one = __get_part_one(draws, boards, lookup)
        part_two = __get_part_two(draws, boards, lookup)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
