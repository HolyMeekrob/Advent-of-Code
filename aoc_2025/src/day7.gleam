import common
import gleam/dict.{type Dict}
import gleam/int
import gleam/list
import gleam/result
import gleam/string
import iv

const day = 7

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let map = parse(input)

  common.Solution(solve_1(map), solve_2(map))
}

fn solve_1(map: Map) -> String {
  map
  |> iv.index_fold(#(map, 0), count_splits_row)
  |> fn(agg) { agg.1 }
  |> int.to_string
}

fn solve_2(map: Map) -> String {
  map
  |> count_paths
  |> int.to_string
}

fn parse(input: String) -> iv.Array(iv.Array(Node)) {
  input
  |> string.split("\n")
  |> list.map(parse_line)
  |> iv.from_list
}

fn parse_line(line: String) -> iv.Array(Node) {
  line
  |> common.chunk_string(1)
  |> list.map(parse_node)
  |> iv.from_list
}

fn parse_node(char: String) -> Node {
  case char {
    "S" -> Start
    "^" -> Splitter
    "." -> Empty
    _ -> panic as { "Unrecognized input: " <> char }
  }
}

fn count_splits_row(
  state: #(Map, Int),
  row: iv.Array(Node),
  index: Int,
) -> #(Map, Int) {
  row
  |> iv.index_fold(#(state.0, index, 0), count_splits)
  |> fn(result) { #(result.0, state.1 + result.2) }
}

fn count_splits(
  state: #(Map, Int, Int),
  node: Node,
  col_index: Int,
) -> #(Map, Int, Int) {
  let row_index = state.1
  let map = state.0

  let get_row = fn() -> iv.Array(Node) {
    let assert Ok(row) = iv.get(map, state.1)
    row
  }

  let get_above = fn() -> Node {
    let assert Ok(above) =
      map
      |> iv.get(row_index - 1)
      |> result.try(fn(map) { iv.get(map, col_index) })

    above
  }

  let update_node = fn(new_node: Node) -> Map {
    let row = get_row()
    let assert Ok(updated_row) = iv.set(row, col_index, new_node)
    let assert Ok(updated_map) = iv.set(map, row_index, updated_row)
    updated_map
  }

  let process_splitter = fn() -> #(Map, Int, Int) {
    case get_above() {
      // This shouldn't happen
      Start | Beam -> #(map, state.1, state.2 + 1)
      Splitter | Empty -> state
    }
  }

  let process_empty = fn() -> #(Map, Int, Int) {
    case get_above() {
      Start | Beam -> #(update_node(Beam), state.1, state.2)
      Splitter | Empty -> {
        let row = get_row()
        let left = iv.get_or_default(row, col_index - 1, Empty)
        let right = iv.get_or_default(row, col_index + 1, Empty)

        case left, right {
          Splitter, _ | _, Splitter -> #(update_node(Beam), state.1, state.2)
          _, _ -> state
        }
      }
    }
  }

  let process_node = fn() -> #(Map, Int, Int) {
    case node {
      // Start and Beam shouldn't happen
      Start | Beam -> state
      Splitter -> process_splitter()
      Empty -> process_empty()
    }
  }

  case row_index {
    // Array out of bounds - should mean we're on the first row so do nothing
    0 -> state
    _ -> process_node()
  }
}

fn next_splitter(map: Map, from coordinate: Point) -> Result(Point, Nil) {
  case coordinate.row >= iv.length(map) {
    True -> Error(Nil)
    False ->
      case get_node_at(map, coordinate) {
        Splitter -> Ok(coordinate)
        _ -> next_splitter(map, Point(coordinate.row + 1, coordinate.col))
      }
  }
}

fn get_node_at(map: Map, coordinate: Point) -> Node {
  map
  |> iv.get_or_default(coordinate.row, iv.new())
  |> iv.get_or_default(coordinate.col, Empty)
}

fn count_paths(map: Map) -> Int {
  let assert Ok(start) =
    map
    |> iv.get_or_default(0, iv.new())
    |> iv.find_index(fn(node) { node == Start })

  case next_splitter(map, Point(0, start)) {
    Ok(splitter) ->
      map
      |> count_paths_loop(splitter, dict.new(), 0)
      |> fn(result) { result.1 }
    _ -> panic as "Couldn't find the first splitter"
  }
}

fn count_paths_loop(
  map: Map,
  location: Point,
  cache: Dict(Point, Int),
  count: Int,
) -> #(Dict(Point, Int), Int) {
  case dict.get(cache, location) {
    Ok(n) -> #(cache, n)
    _ -> {
      let left = next_splitter(map, Point(location.row + 1, location.col - 1))
      let right = next_splitter(map, Point(location.row + 1, location.col + 1))
      case left, right {
        // No more splits
        Error(Nil), Error(Nil) -> {
          let cache = dict.insert(cache, location, count + 2)
          #(cache, count + 2)
        }
        Ok(l), Error(Nil) -> {
          let left_result = count_paths_loop(map, l, cache, count)
          let count = left_result.1 + 1
          let cache = dict.insert(left_result.0, location, count)
          #(cache, count)
        }
        Error(Nil), Ok(r) -> {
          let right_result = count_paths_loop(map, r, cache, count)
          let count = right_result.1 + 1
          let cache = dict.insert(right_result.0, location, count)
          #(cache, count)
        }
        Ok(l), Ok(r) -> {
          let left_result = count_paths_loop(map, l, cache, count)
          let right_result = count_paths_loop(map, r, left_result.0, count)
          let count = left_result.1 + right_result.1
          let cache = dict.insert(right_result.0, location, count)
          #(cache, count)
        }
      }
    }
  }
}

type Node {
  Start
  Splitter
  Beam
  Empty
}

type Map =
  iv.Array(iv.Array(Node))

type Point {
  Point(row: Int, col: Int)
}
