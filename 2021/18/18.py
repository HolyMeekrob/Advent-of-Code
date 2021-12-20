from __future__ import annotations

import math
from copy import deepcopy
from dataclasses import dataclass
from functools import reduce
from operator import add
from pathlib import Path

filename = "18.txt"
path = Path(__file__).parent.joinpath(filename)


class ReduceResult:
    def __init__(self) -> None:
        pass


class ReduceCompleteResult(ReduceResult):
    def __init__(self) -> None:
        super().__init__()


class SplitResult(ReduceCompleteResult):
    def __init__(self) -> None:
        super().__init__()


class ExplodeCompleteResult(ReduceCompleteResult):
    def __init__(self) -> None:
        super().__init__()


class ExplodeBothResult(ReduceResult):
    def __init__(self, left: int, right: int) -> None:
        super().__init__()
        self.left = left
        self.right = right


class ExplodeOneResult(ReduceResult):
    def __init__(self, value: int) -> None:
        self.value = value


class ExplodeLeftResult(ExplodeOneResult):
    def __init__(self, left: int) -> None:
        super().__init__(left)


class ExplodeRightResult(ExplodeOneResult):
    def __init__(self, right: int) -> None:
        super().__init__(right)


@dataclass
class Pair:
    left: Pair | int
    right: Pair | int

    def __explode(self, result: ExplodeOneResult):
        def update_left(n: int):
            self.left += n

        def update_right(n: int):
            self.right += n

        values = (
            [self.right, self.left]
            if isinstance(result, ExplodeLeftResult)
            else [self.left, self.right]
        )

        updates = (
            [update_right, update_left]
            if isinstance(result, ExplodeLeftResult)
            else [update_left, update_right]
        )

        if isinstance(values[0], int):
            updates[0](result.value)
            return ExplodeCompleteResult()

        if isinstance(values[0].__explode(result), ExplodeCompleteResult):
            return ExplodeCompleteResult()

        if isinstance(values[1], int):
            updates[1](result.value)
            return ExplodeCompleteResult()

        return values[1].__explode(result)

    def __reduce_explodes(self, depth: int = 0):
        def handleReduceResult(is_left: bool, result: ReduceResult):
            if result is None:
                return None

            if isinstance(result, ReduceCompleteResult):
                return result

            if isinstance(result, ExplodeBothResult):
                if is_left:
                    self.left = 0
                    if isinstance(self.right, int):
                        self.right += result.right
                    else:
                        self.right.__explode(ExplodeRightResult(result.right))
                    return ExplodeLeftResult(result.left)
                else:
                    self.right = 0
                    if isinstance(self.left, int):
                        self.left += result.left
                    else:
                        self.left.__explode(ExplodeLeftResult(result.left))
                    return ExplodeRightResult(result.right)

            if isinstance(result, ExplodeLeftResult):
                if is_left:
                    return result

                if isinstance(self.left, int):
                    self.left += result.value
                    return ExplodeCompleteResult()

                return self.left.__explode(result)

            if isinstance(result, ExplodeRightResult):
                if not is_left:
                    return result

                if isinstance(self.right, int):
                    self.right += result.value
                    return ExplodeCompleteResult()

                return self.right.__explode(result)

        result = None

        if depth >= 4:
            result = ExplodeBothResult(self.left, self.right)

        elif isinstance(self.left, Pair):
            result = handleReduceResult(True, self.left.__reduce_explodes(depth + 1))

        if result is None and isinstance(self.right, Pair):
            result = handleReduceResult(False, self.right.__reduce_explodes(depth + 1))

        return result

    def __reduce_splits(self):
        def split(num: int):
            return Pair(math.floor(num / 2), math.floor((num / 2 + 0.5)))

        result = None

        if isinstance(self.left, Pair):
            result = self.left.__reduce_splits()

        elif self.left > 9:
            self.left = split(self.left)
            result = SplitResult()

        if result is None and isinstance(self.right, Pair):
            result = self.right.__reduce_splits()

        elif result is None and self.right > 9:
            self.right = split(self.right)
            result = SplitResult()

        return result

    def __reduce(self):
        result = self.__reduce_explodes()

        if result is None:
            result = self.__reduce_splits()

        if isinstance(result, ReduceCompleteResult | ExplodeOneResult):
            return self.__reduce()

    def __repr__(self):
        return f"[{self.left},{self.right}]"

    def __add__(self, other: Pair) -> Pair:
        pair = Pair(deepcopy(self), deepcopy(other))
        pair.__reduce()
        return pair

    def magnitude(self):
        left_magnitude = (
            self.left if isinstance(self.left, int) else self.left.magnitude()
        )
        right_magnitude = (
            self.right if isinstance(self.right, int) else self.right.magnitude()
        )
        return 3 * left_magnitude + 2 * right_magnitude

    @staticmethod
    def create(line: str):
        stack: list[Pair] = []

        for ch in line:
            match ch:
                case "[":
                    stack.append(Pair(0, 0))
                case "]":
                    right = stack.pop()
                    stack[-1].right = right
                case ",":
                    left = stack.pop()
                    stack[-1].left = left
                case _:
                    stack.append(int(ch))

        return stack[0]


def __get_part_one(pairs: list[Pair]):
    return reduce(add, pairs).magnitude()


def __get_part_two(pairs: list[Pair]):
    maximum = 0

    for (i, a) in enumerate(pairs):
        for b in pairs[i + 1 :]:
            maximum = max(maximum, (a + b).magnitude(), (b + a).magnitude())

    return maximum


def run():
    with open(path) as file:
        lines = [line.strip() for line in file.readlines() if line]

        pairs = list(map(Pair.create, lines))

        part_one = __get_part_one(pairs)
        part_two = __get_part_two(pairs)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
