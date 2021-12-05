from itertools import chain
from typing import Iterable


def flatten(iterables: Iterable[Iterable]):
    return chain(*iterables)
