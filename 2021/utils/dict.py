from typing import TypeVar


T = TypeVar("T")
U = TypeVar("U")


def flip(d: dict[T, U]):
    return {v: k for (k, v) in d.items()}
