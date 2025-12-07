import common
import gleam/int
import gleam/list
import gleam/string

const day = 1

//  starting position
const start = 50

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let rotations = parse(input)

  common.Solution(solve_1(rotations), solve_2(rotations))
}

fn parse(input: String) -> List(Rotation) {
  input
  |> string.split("\n")
  |> list.filter(fn(str) { !string.is_empty(str) })
  |> list.map(read)
}

fn read(line: String) -> Rotation {
  let assert Ok(digits) = line |> string.drop_start(1) |> int.parse

  let result = case line {
    "L" <> _ -> Ok(Left(digits))
    "R" <> _ -> Ok(Right(digits))
    _ -> Error("Unrecognized input: " <> line)
  }

  case result {
    Ok(rotation) -> rotation
    _ -> panic as "Number not formatted correctly"
  }
}

fn solve_1(rotations: List(Rotation)) -> String {
  solve_part(rotations, count_ending_zeroes)
}

fn solve_2(rotations: List(Rotation)) -> String {
  solve_part(rotations, count_zero_crossings)
}

fn solve_part(
  rotations: List(Rotation),
  counter: fn(Result, Rotation) -> Result,
) -> String {
  rotations
  |> list.fold(Result(start, 0), counter)
  |> fn(result) { result.zeroes }
  |> int.to_string
}

fn count_zero_crossings(current: Result, rotation: Rotation) -> Result {
  let new_position = rotate(rotation, current.position)

  let hundreds = rotation.count / 100
  let rotation_remainder = rotation.count % 100
  let position_remainder = int.absolute_value(current.position % 100)

  let remainder_crosses_zero = case position_remainder, rotation {
    // If we're starting at position zero, we can't have a remainder crossing
    0, _ -> False
    _, Left(_) if current.position < 0 ->
      rotation_remainder >= { 100 - position_remainder }
    _, Left(_) -> rotation_remainder >= position_remainder
    _, Right(_) if current.position < 0 ->
      rotation_remainder >= position_remainder
    _, Right(_) -> rotation_remainder >= { 100 - position_remainder }
  }

  let remainder_crossing = case remainder_crosses_zero {
    True -> 1
    False -> 0
  }

  let zero_crossings = hundreds + remainder_crossing

  Result(new_position, current.zeroes + zero_crossings)
}

fn count_ending_zeroes(current: Result, rotation: Rotation) -> Result {
  let new_position = rotate(rotation, current.position)
  case new_position % 100 {
    0 -> Result(new_position, current.zeroes + 1)
    _ -> Result(new_position, current.zeroes)
  }
}

fn rotate(rotation: Rotation, position: Int) -> Int {
  case rotation {
    Left(n) -> position - n
    Right(n) -> position + n
  }
}

type Rotation {
  Left(count: Int)
  Right(count: Int)
}

type Result {
  Result(position: Int, zeroes: Int)
}
