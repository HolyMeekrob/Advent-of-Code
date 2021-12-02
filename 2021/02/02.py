from dataclasses import dataclass
from enum import Enum, auto
from pathlib import Path
from typing import Callable

filename = "02.txt"
path = Path(__file__).parent.joinpath(filename)


class Direction(Enum):
    FORWARD = auto()
    UP = auto()
    DOWN = auto()

    @staticmethod
    def parse(input: str):
        return Direction[input.upper()]


@dataclass
class Instruction:
    direction: Direction
    value: int

    @staticmethod
    def parse(input: str):
        [direction, value] = input.strip().split(" ")
        return Instruction(Direction.parse(direction), int(value))


@dataclass
class Position:
    horizontal: int
    depth: int
    aim: int

    @property
    def product(self):
        return self.horizontal * self.depth

    def run_instruction_part_one(self, instruction: Instruction):
        match instruction.direction:
            case Direction.FORWARD:
                return Position(
                    self.horizontal + instruction.value, self.depth, self.aim
                )

            case Direction.UP:
                return Position(
                    self.horizontal, self.depth - instruction.value, self.aim
                )

            case Direction.DOWN:
                return Position(
                    self.horizontal, self.depth + instruction.value, self.aim
                )

            case _:
                raise Exception("Unknown direction")

    def run_instruction_part_two(self, instruction: Instruction):
        match instruction.direction:
            case Direction.FORWARD:
                return Position(
                    self.horizontal + instruction.value,
                    self.depth + (self.aim * instruction.value),
                    self.aim,
                )

            case Direction.UP:
                return Position(
                    self.horizontal, self.depth, self.aim - instruction.value
                )

            case Direction.DOWN:
                return Position(
                    self.horizontal, self.depth, self.aim + instruction.value
                )


def run_instructions(
    instructions: list[Instruction],
    position: Position,
    runner: Callable[[Instruction, Position], Position],
):
    current_position = position

    for instruction in instructions:
        current_position = runner(instruction, current_position)

    return current_position


def run():
    with open(path) as file:
        instructions = [Instruction.parse(line) for line in file.readlines() if line]

        part_one_runner = (
            lambda instruction, position: position.run_instruction_part_one(instruction)
        )
        part_two_runner = (
            lambda instruction, position: position.run_instruction_part_two(instruction)
        )

        part_one = run_instructions(instructions, Position(0, 0, 0), part_one_runner)
        print(f"Part one: {part_one.product}")

        part_two = run_instructions(instructions, Position(0, 0, 0), part_two_runner)
        print(f"Part two: {part_two.product}")
