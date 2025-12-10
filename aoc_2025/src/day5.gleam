import common
import gleam/int
import gleam/list
import gleam/order
import gleam/result
import gleam/string

const day = 5

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let #(ranges, ingredients) = parse(input)

  common.Solution(solve_1(ranges, ingredients), solve_2(ranges))
}

fn parse(input: String) -> #(List(Range), List(Int)) {
  let lines = string.split(input, "\n")
  let separator = common.index_of(lines, "")
  assert separator != -1 as "Input is missing blank line"

  let ranges =
    lines
    |> list.take(separator)
    |> list.filter(common.complement(string.is_empty))
    |> list.map(parse_range)

  let ingredients =
    lines
    |> list.drop(separator + 1)
    |> list.filter(common.complement(string.is_empty))
    |> list.map(int.parse)
    |> list.map(result.unwrap(_, 0))

  #(ranges, ingredients)
}

fn parse_range(line: String) -> Range {
  let ends =
    line
    |> string.split("-")
    |> list.map(int.parse)

  case ends {
    [start, end] -> {
      let assert Ok(start) = start
      let assert Ok(end) = end
      Range(start, end)
    }
    _ -> panic as { "Invalid range input: " <> line }
  }
}

fn solve_1(ranges: List(Range), ingredients: List(Int)) -> String {
  let is_in_any_range = fn(ingredient) {
    list.any(ranges, is_in_range(ingredient, _))
  }

  ingredients
  |> list.count(is_in_any_range)
  |> int.to_string
}

fn solve_2(ranges: List(Range)) -> String {
  ranges
  |> list.sort(range_compare)
  |> list.fold([], consolidate_ranges)
  |> list.fold(0, sum_lengths)
  |> int.to_string
}

fn is_in_range(ingredient: Int, range: Range) {
  ingredient >= range.from && ingredient <= range.to
}

fn consolidate_ranges(ranges: List(Range), range: Range) -> List(Range) {
  merge_range_into(range, ranges, [])
}

fn merge_range_into(
  range: Range,
  unmerged: List(Range),
  merged: List(Range),
) -> List(Range) {
  case unmerged {
    [] -> [range, ..merged]
    [head, ..rest] -> {
      case overlap(head, range) {
        True -> [merge_ranges(head, range), ..list.append(rest, merged)]
        False -> merge_range_into(range, rest, [head, ..merged])
      }
    }
  }
}

fn overlap(a: Range, b: Range) {
  a.from <= b.to && a.to >= b.from
}

fn merge_ranges(a: Range, b: Range) {
  Range(int.min(a.from, b.from), int.max(a.to, b.to))
}

fn sum_lengths(lengths: Int, range: Range) -> Int {
  lengths + range_length(range)
}

fn range_length(range: Range) -> Int {
  range.to - range.from + 1
}

fn range_compare(a: Range, b: Range) -> order.Order {
  int.compare(a.from, b.from)
}

type Range {
  Range(from: Int, to: Int)
}
