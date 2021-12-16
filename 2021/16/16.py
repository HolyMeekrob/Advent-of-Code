from __future__ import annotations
from functools import reduce
from operator import mul
from pathlib import Path
from typing import Type

filename = "16.txt"
path = Path(__file__).parent.joinpath(filename)


class PacketFactory:
    @staticmethod
    def __get_packet_type_constructor(type_id: int) -> Type[Packet]:
        return packet_types[type_id] if type_id in packet_types else default_packet_type

    @staticmethod
    def __get_packet_type_id(bits: str):
        return int(bits[3:6], 2)

    @staticmethod
    def get_packet(bits: str) -> Packet:
        return PacketFactory.__get_packet_type_constructor(
            PacketFactory.__get_packet_type_id(bits)
        )(bits)


class Packet:
    def __init__(self, bits: str, bit_length: int):
        self.version = int(bits[:3], 2)
        self.bit_length = bit_length

    def get_version_total(self) -> int:
        raise NotImplementedError()

    def calculate(self) -> int:
        raise NotImplementedError()


class LiteralPacket(Packet):
    def __init__(self, bits: str):

        (value, literal_bit_length) = LiteralPacket.__parse(bits[6:])
        self.value = value

        super().__init__(bits, literal_bit_length + 6)

    def get_version_total(self):
        return self.version

    def calculate(self):
        return self.value

    def __repr__(self):
        return f"Literal {self.value} ({self.bit_length} bits)"

    @staticmethod
    def __parse(bits: str) -> tuple[int, int]:
        value_bits = ""
        index = 0
        last_group = False

        while not last_group:
            end_index = index + 5
            last_group = bits[index] == "0"
            value_bits += bits[index + 1 : end_index]
            index = end_index

        value = int(value_bits, 2)

        return (value, end_index)


class OperatorPacket(Packet):
    def __init__(self, bits: str):
        (packets, packets_bit_length) = OperatorPacket.__parse(bits[6:])
        self.packets = packets

        super().__init__(bits, packets_bit_length + 6)

    def get_version_total(self) -> int:
        return self.version + sum(
            [packet.get_version_total() for packet in self.packets]
        )

    @staticmethod
    def __parse(bits: str) -> list[Packet]:
        return (
            OperatorPacket.__get_packets_by_bit_count(bits)
            if bits[0] == "0"
            else OperatorPacket.__get_packets_by_number(bits)
        )

    @staticmethod
    def __get_packets_by_number(bits: str):
        packets: list[Packet] = []
        packet_count = int(bits[1:12], 2)

        bit_offset = 12
        for _ in range(packet_count):
            remaining_bits = bits[bit_offset:]
            packets.append(PacketFactory.get_packet(remaining_bits))
            bit_offset += packets[-1].bit_length

        return (packets, bit_offset)

    @staticmethod
    def __get_packets_by_bit_count(bits: str):
        bit_count = int(bits[1:16], 2)
        total_bit_count = bit_count + 16

        bit_offset = 16
        packets = []

        while bit_offset < total_bit_count:
            remaining_bits = bits[bit_offset:]
            packets.append(PacketFactory.get_packet(remaining_bits))
            bit_offset += packets[-1].bit_length

        return (packets, total_bit_count)


class SumPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self):
        return sum([packet.calculate() for packet in self.packets])


class ProductPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self):
        return reduce(mul, [packet.calculate() for packet in self.packets], 1)


class MinimumPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self) -> int:
        return min([packet.calculate() for packet in self.packets])


class MaximumPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self) -> int:
        return max([packet.calculate() for packet in self.packets])


class GreaterThanPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self) -> int:
        return 1 if self.packets[0].calculate() > self.packets[1].calculate() else 0


class LessThanPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self) -> int:
        return 1 if self.packets[0].calculate() < self.packets[1].calculate() else 0


class EqualToPacket(OperatorPacket):
    def __init__(self, bits: str):
        super().__init__(bits)

    def calculate(self) -> int:
        return 1 if self.packets[0].calculate() == self.packets[1].calculate() else 0


default_packet_type: Type[Packet] = OperatorPacket
packet_types: dict[int, Type[Packet]] = {
    0: SumPacket,
    1: ProductPacket,
    2: MinimumPacket,
    3: MaximumPacket,
    4: LiteralPacket,
    5: GreaterThanPacket,
    6: LessThanPacket,
    7: EqualToPacket,
}


def __get_part_one(packet: Packet):
    return packet.get_version_total()


def __get_part_two(packet: Packet):
    return packet.calculate()


def __hexToBITS(hex: str):
    return "".join([bin(int(hex_char, 16))[2:].rjust(4, "0") for hex_char in hex])


def run():
    with open(path) as file:
        lines = [__hexToBITS(line.strip()) for line in file.readlines() if line]
        packets = [PacketFactory.get_packet(bits) for bits in lines]

        part_one = list(map(__get_part_one, packets))
        part_two = list(map(__get_part_two, packets))

        print(f"Part one: {part_one[0]}")
        print(f"Part two: {part_two[0]}")


if __name__ == "__main__":
    run()
