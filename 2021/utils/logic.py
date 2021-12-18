def between(val, small, big, inclusive_small: bool = True, inclusive_big: bool = True):
    bigger_than_small = val >= small if inclusive_small else val > small
    smaller_than_big = val <= big if inclusive_big else val < big

    return bigger_than_small and smaller_than_big
