import common
import gleam/int
import gleam/list
import gleam/result
import gleam/string

const day = 6

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)

  common.Solution(solve_1(input), solve_2(input))
}

fn solve_1(input: String) -> String {
  solve_puzzle(input, parse_ltr)
}

fn solve_2(input: String) -> String {
  solve_puzzle(input, parse_rtl)
}

fn solve_puzzle(input: String, parser: fn(String) -> List(Problem)) -> String {
  input
  |> parser
  |> list.map(solve_problem)
  |> int.sum
  |> int.to_string
}

fn parse_ltr(input: String) -> List(Problem) {
  let lines =
    input
    |> string.split("\n")
    |> list.filter(common.string_is_not_empty)

  let line_count = list.length(lines)

  let numbers =
    lines
    |> list.take(line_count - 1)
    |> list.map(get_numbers)
    |> list.transpose

  let assert Ok(operators) =
    lines
    |> list.last()
    |> result.map(get_operators)

  numbers
  |> list.zip(operators)
  |> list.map(to_problem)
}

fn parse_rtl(input: String) -> List(Problem) {
  let chunk_until_empty = fn(state: #(List(List(String)), Bool), digits: String) -> #(
    List(List(String)),
    Bool,
  ) {
    case state.1 {
      True -> #([[digits], ..state.0], False)
      False ->
        case digits {
          "" -> #(state.0, True)
          _ -> {
            let head =
              state.0 |> list.first |> result.unwrap([]) |> list.prepend(digits)
            let tail = state.0 |> list.drop(1)
            #([head, ..tail], False)
          }
        }
    }
  }
  let group_numbers = fn(nums: List(String)) -> List(List(String)) {
    nums
    |> list.fold_right(#([], True), chunk_until_empty)
    |> fn(pair) { pair.0 }
    // let i = common.index_of(nums, "")
    // nums
    //   |> list.filter(common.string_is_not_empty)
    //   |> list.sized_chunk(i)
  }

  let to_numbers = fn(groups: List(String)) -> List(Int) {
    groups
    |> list.map(common.parse_int)
  }

  let lines =
    input
    |> string.split("\n")
    |> list.filter(common.string_is_not_empty)

  let line_count = list.length(lines)

  let numbers =
    lines
    |> list.take(line_count - 1)
    |> list.map(common.chunk_string(_, 1))
    |> list.transpose
    |> list.map(string.join(_, ""))
    |> list.map(string.trim)
    |> group_numbers
    |> list.map(to_numbers)
    |> list.map(list.reverse)

  let assert Ok(operators) =
    lines
    |> list.last()
    |> result.map(get_operators)

  numbers
  |> list.zip(operators)
  |> list.map(to_problem)
  |> list.reverse
}

fn get_numbers(line: String) -> List(Int) {
  line
  |> string.split(" ")
  |> list.filter(common.string_is_not_empty)
  |> list.map(int.parse)
  |> list.map(result.unwrap(_, 0))
}

fn get_operators(line: String) -> List(Operator) {
  line
  |> string.split(" ")
  |> list.filter(common.string_is_not_empty)
  |> list.map(parse_operator)
}

fn parse_operator(char: String) {
  case char {
    "+" -> Add
    "*" -> Multiply
    _ -> panic as { "Unknown operator: " <> char }
  }
}

fn to_problem(args: #(List(Int), Operator)) -> Problem {
  Problem(args.0, args.1)
}

fn solve_problem(problem: Problem) -> Int {
  case problem.operator {
    Add -> int.sum(problem.nums)
    Multiply -> int.product(problem.nums)
  }
}

type Problem {
  Problem(nums: List(Int), operator: Operator)
}

type Operator {
  Add
  Multiply
}
