from pathlib import Path

filename = "03.txt"
path = Path(__file__).parent.joinpath(filename)


def __arr_to_binary(digits: list[int]):
    return sum(
        [2 ** (len(digits) - i - 1) for (i, digit) in enumerate(digits) if digit == 1]
    )


def __binary_str_to_int(binary_str: str):
    return __arr_to_binary([int(digit) for digit in binary_str])


def __get_most_common_digit(place: int, lines: list[str]):
    average = sum([int(line[place]) for line in lines]) / len(lines)

    return round(average) if average != 0.5 else 1


def __get_rating(lines: list[str], get_least_common: bool):
    digit_count = len(lines[0])

    remaining_lines = lines
    for place in range(digit_count):
        if len(remaining_lines) == 1:
            break

        digit = __get_most_common_digit(place, remaining_lines)

        if get_least_common:
            digit = 1 - digit

        remaining_lines = [
            line for line in remaining_lines if line[place] == str(digit)
        ]

    return __binary_str_to_int(remaining_lines[0])


def __get_oxygen_generator_rating(lines: list[str]):
    return __get_rating(lines, False)


def __get_co2_scrubber_rating(lines: list[str]):
    return __get_rating(lines, True)


def __get_part_two(lines: list[str]):
    oxygen_generator_rating = __get_oxygen_generator_rating(lines)
    co2_scrubber_rating = __get_co2_scrubber_rating(lines)

    return oxygen_generator_rating * co2_scrubber_rating


def __get_part_one(lines: list[str]):
    digit_count = len(lines[0])

    gamma_digits = [
        __get_most_common_digit(place, lines) for place in range(digit_count)
    ]
    epsilon_digits = [1 - digit for digit in gamma_digits]

    gamma = __arr_to_binary(gamma_digits)
    epsilon = __arr_to_binary(epsilon_digits)

    return gamma * epsilon


def run():
    with open(path) as file:
        lines = file.read().splitlines()

        part_one = __get_part_one(lines)
        part_two = __get_part_two(lines)

        print(f"Part one: {part_one}")
        print(f"Part two: {part_two}")


if __name__ == "__main__":
    run()
