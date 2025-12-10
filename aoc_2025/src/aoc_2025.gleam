import argv
import common
import day1
import day2
import day3
import day4
import day5
import day6
import gleam/int
import gleam/io
import gleam/string
import in

pub fn main() -> Nil {
  let args = argv.load()
  case args.arguments {
    [] -> input_day()
    ["test", day, ..] -> get_day(day, True)
    [day, "test", ..] -> get_day(day, True)
    [day, ..] -> get_day(day, False)
  }
}

fn input_day() -> Nil {
  io.print("Enter day: ")
  let assert Ok(day) = in.read_line()

  io.print("Is test (y/[n])? ")
  let assert Ok(test_response) = in.read_chars(1)
  let is_test = case test_response {
    "y" -> True
    "Y" -> True
    _ -> False
  }

  day
  |> string.trim_end()
  |> get_day(is_test)
}

fn get_day(day: String, is_test: Bool) -> Nil {
  case int.parse(day) {
    Ok(num) -> run_day(num, is_test)
    Error(_) -> io.println("Invalid input: day must be an integer")
  }
}

fn run_day(day: Int, is_test: Bool) -> Nil {
  let solution = case day {
    1 -> day1.solve(is_test)
    2 -> day2.solve(is_test)
    3 -> day3.solve(is_test)
    4 -> day4.solve(is_test)
    5 -> day5.solve(is_test)
    6 -> day6.solve(is_test)
    _ -> common.Solution("Not implemented", "Not implemented")
  }

  io.println(common.to_string(solution))
}
