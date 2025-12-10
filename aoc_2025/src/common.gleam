import gleam/int
import gleam/list
import gleam/result
import gleam/string
import simplifile

pub type Solution {
  Solution(puzzle_1: String, puzzle_2: String)
}

pub fn to_string(solution: Solution) -> String {
  "Puzzle 1: " <> solution.puzzle_1 <> "\n" <> "Puzzle 2: " <> solution.puzzle_2
}

pub fn get_input(day: Int, is_test: Bool) -> Result(String, String) {
  let filepath = case is_test {
    True -> "./input/day" <> int.to_string(day) <> "_test.txt"
    False -> "./input/day" <> int.to_string(day) <> ".txt"
  }

  case simplifile.read(from: filepath) {
    Ok(contents) -> Ok(contents)
    Error(_) -> Error("Error reading file")
  }
}

/// Divides a string into evenly sized chunks.
pub fn chunk_string(str: String, length: Int) -> List(String) {
  append_chunks(str, length, [])
}

fn append_chunks(str: String, length: Int, chunks: List(String)) -> List(String) {
  case string.length(str) {
    0 -> chunks
    _ ->
      append_chunks(string.drop_end(str, length), length, [
        string.slice(str, -length, length),
        ..chunks
      ])
  }
}

/// Drops `n` elements from the end of the list.
pub fn drop_end(from vals: List(a), count n: Int) -> List(a) {
  list.take(vals, list.length(vals) - n)
}

/// Takes a function that returns a boolean and returns
/// a function that returns the negation of the result of
/// calling the original function.
pub fn complement(fun: fn(a) -> Bool) -> fn(a) -> Bool {
  fn(x) { !fun(x) }
}

/// Returns the first index of a value within a list
/// or `-1` if the value doesn't appear within the list.
pub fn index_of(vals: List(a), val: a) -> Int {
  case vals {
    [] -> -1
    [head, ..] if head == val -> 0
    [_, ..rest] -> 1 + index_of(rest, val)
  }
}

/// Returns whether `val` is less than `comparison`.
pub fn int_less_than(val: Int, comparison: Int) -> Bool {
  val < comparison
}

/// Determines if a `String` is non-empty.
pub fn string_is_not_empty(str: String) -> Bool {
  !string.is_empty(str)
}

/// Parse a known integer
pub fn parse_int(str: String) -> Int {
  str
  |> int.parse
  |> result.unwrap(0)
}
