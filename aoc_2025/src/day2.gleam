import common
import gleam/int
import gleam/list
import gleam/result
import gleam/string

const day = 2

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let ranges = parse(input)

  common.Solution(solve_1(ranges), solve_2(ranges))
}

fn solve_1(ranges: List(Range)) -> String {
  solve_part(ranges, is_single_repeat)
}

fn solve_2(ranges: List(Range)) -> String {
  solve_part(ranges, is_any_repeat)
}

fn solve_part(ranges: List(Range), filter: fn(Int) -> Bool) {
  ranges
  |> list.flat_map(to_list)
  |> list.filter(filter)
  |> int.sum
  |> int.to_string()
}

fn parse(input: String) -> List(Range) {
  input
  |> string.trim()
  |> string.split(",")
  |> list.map(get_range)
}

fn get_range(line: String) -> Range {
  let assert Ok(strs) = string.split_once(line, "-")
  let assert Ok(start) = int.parse(strs.0)
  let assert Ok(end) = int.parse(strs.1)

  Range(start, end)
}

fn to_list(range: Range) -> List(Int) {
  list.range(range.start, range.end)
}

fn is_single_repeat(num: Int) -> Bool {
  let digits = int.to_string(num)
  let length = string.length(digits)
  case int.is_even(length) {
    False -> False
    True ->
      string.drop_end(digits, length / 2)
      == string.drop_start(digits, length / 2)
  }
}

fn is_any_repeat(num: Int) -> Bool {
  let digits = int.to_string(num)
  let max_size =
    digits
    |> string.length
    |> int.divide(2)
    |> result.unwrap(0)

  is_repeat(digits, max_size)
}

fn is_repeat(str: String, length: Int) -> Bool {
  case length {
    0 -> False
    _ ->
      case string.length(str) % length {
        // Can't be divided evenly; continue to the next length
        n if n > 0 -> is_repeat(str, length - 1)
        _ ->
          // Check this length; if it fails, continue to the next length
          case
            str |> common.chunk_string(length) |> list.unique |> list.length
          {
            1 -> True
            _ -> is_repeat(str, length - 1)
          }
      }
  }
}

type Range {
  Range(start: Int, end: Int)
}
