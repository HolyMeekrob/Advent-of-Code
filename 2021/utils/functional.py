from functools import reduce
from typing import TypeVar

T = TypeVar("T")


def __compose2(f, g):
    return lambda *a, **kw: f(g(*a, **kw))


def compose(*fs):
    return reduce(__compose2, fs)


def identity(x: T):
    return x
