import common
import gleam/int
import gleam/list
import gleam/result
import gleam/string

const day = 3

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let banks = parse(input)
  common.Solution(solve_1(banks), solve_2(banks))
}

fn solve_1(banks: List(Bank)) -> String {
  solve_part(banks, 2)
}

fn solve_2(banks: List(Bank)) -> String {
  solve_part(banks, 12)
}

fn solve_part(banks: List(Bank), num_digits: Int) -> String {
  banks
  |> list.map(fn(bank) { get_max_joltage(bank, num_digits) })
  |> int.sum
  |> int.to_string
}

fn parse(input: String) -> List(Bank) {
  input
  |> string.split("\n")
  |> list.filter(common.complement(string.is_empty))
  |> list.map(to_bank)
}

fn to_bank(line: String) -> Bank {
  line
  |> common.chunk_string(1)
  |> list.map(fn(digit: String) { digit |> int.parse |> result.unwrap(0) })
  |> Bank()
}

fn get_max_joltage(bank: Bank, count: Int) -> Int {
  let digits = get_joltage_digits(bank.batteries, count, [])
  digits
  |> list.map(int.to_string)
  |> string.join("")
  |> int.parse
  |> result.unwrap(0)
}

fn get_joltage_digits(
  batteries: List(Int),
  remaining: Int,
  digits: List(Int),
) -> List(Int) {
  case remaining {
    0 -> digits
    _ -> {
      let available = common.drop_end(batteries, remaining - 1)
      case available {
        [] -> digits
        _ -> {
          let digit = list.max(available, int.compare) |> result.unwrap(0)
          let index = common.index_of(available, digit)
          get_joltage_digits(
            list.drop(batteries, index + 1),
            remaining - 1,
            list.append(digits, [digit]),
          )
        }
      }
    }
  }
}

type Bank {
  Bank(batteries: List(Int))
}
