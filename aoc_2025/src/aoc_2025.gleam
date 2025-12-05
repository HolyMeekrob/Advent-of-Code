import argv
import gleam/int
import gleam/io
import gleam/string
import in

pub fn main() -> Nil {
  let args = argv.load()
  case args.arguments {
    [] -> input_day()
    [day, ..] -> get_day(day)
  }
}

fn input_day() -> Nil {
  io.print("Enter day: ")
  let assert Ok(day) = in.read_line()
  day
  |> string.trim_end()
  |> get_day()
}

fn get_day(day: String) -> Nil {
  case int.parse(day) {
    Ok(num) -> run_day(num)
    Error(_) -> io.println("Invalid input: day must be an integer")
  }
}

fn run_day(day: Int) -> Nil {
  case day {
    _ -> io.println("Day " <> int.to_string(day) <> " not implemented")
  }
}
