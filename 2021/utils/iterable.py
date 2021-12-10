from itertools import chain
from typing import Callable, Iterable, TypeVar

T = TypeVar("T")


def flatten(iterables: Iterable[Iterable[T]]):
    return chain.from_iterable(iterables)


def first(elems: Iterable[T], predicate: Callable[[T], bool], default: T | None = None):
    return next((elem for elem in elems if predicate(elem)), default)
