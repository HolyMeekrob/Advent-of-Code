import gleam/int
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

/// Divides a string into evenly sized chunks
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
