import common
import gleam/int
import gleam/list
import gleam/result
import gleam/string
import iv

const day = 4

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let map = parse(input)

  common.Solution(solve_1(map), solve_2(map))
}

fn solve_1(map: Map) -> String {
  map.coordinates
  |> iv.fold(State(Coordinate(0, 0), 0, map), iterate_row)
  |> get_count
  |> int.to_string
}

fn solve_2(map: Map) -> String {
  Iteration(map, map, 0, False)
  |> solve_2_loop
  |> fn(iter) { iter.total }
  |> int.to_string
}

fn solve_2_loop(iteration: Iteration) -> Iteration {
  case iteration {
    Iteration(_curr, _next, _total, done) if done -> iteration
    _ -> {
      iteration
      |> iterate
      |> solve_2_loop
    }
  }
}

fn parse(input: String) -> Map {
  input
  |> string.split("\n")
  |> list.filter(common.complement(string.is_empty))
  |> list.map(parse_line)
  |> to_map
}

fn parse_line(line: String) -> List(Location) {
  let results =
    line
    |> common.chunk_string(1)
    |> list.map(parse_location)

  assert list.all(results, result.is_ok)

  list.map(results, fn(loc) { result.unwrap(loc, Empty) })
}

fn parse_location(symbol: String) -> Result(Location, String) {
  case symbol {
    "." -> Ok(Empty)
    "@" -> Ok(Roll)
    _ -> Error("Invalid symbol: " <> symbol)
  }
}

fn iterate(iteration: Iteration) -> Iteration {
  iteration.curr.coordinates
  |> iv.fold(State(Coordinate(0, 0), 0, iteration.curr), iterate_row)
  |> clear_map
  |> fn(state) {
    Iteration(
      state.map,
      state.map,
      iteration.total + state.count,
      state.count == 0,
    )
  }
}

fn clear_map(state: State) -> State {
  let update_location = fn(location: Location) -> Location {
    case location {
      Empty -> Empty
      Roll -> Roll
      Removed -> Empty
    }
  }
  let update_row = fn(row: iv.Array(Location)) -> iv.Array(Location) {
    iv.map(row, update_location)
  }
  state.map.coordinates
  |> iv.map(update_row)
  |> Map()
  |> State(state.coordinate, state.count, _)
}

fn iterate_row(state: State, row: iv.Array(Location)) -> State {
  let row_result =
    iv.fold(
      row,
      State(Coordinate(state.coordinate.row, 0), 0, state.map),
      check_accessibility,
    )
  State(down(state.coordinate), state.count + row_result.count, row_result.map)
}

fn check_accessibility(state: State, location: Location) -> State {
  let next = right(state.coordinate)
  assert location != Removed
  case location {
    Empty -> State(..state, coordinate: next)
    Roll -> {
      case is_accessible(state.coordinate, state.map) {
        True ->
          State(
            next,
            state.count + 1,
            mark_removed(state.coordinate, state.map),
          )
        False -> State(..state, coordinate: next)
      }
    }
    Removed ->
      panic as "We should not ever be checking the accessibility of a removed row"
  }
}

fn mark_removed(coordinate: Coordinate, map: Map) -> Map {
  let location = get_location(coordinate, map)
  let set_removed = fn(row) { iv.set(row, coordinate.col, Removed) }
  let update_row = fn(row) { iv.set(map.coordinates, coordinate.row, row) }

  case location {
    Roll -> {
      let assert Ok(updated_map) =
        map.coordinates
        |> iv.get(coordinate.row)
        |> result.try(set_removed)
        |> result.try(update_row)
        |> result.map(Map)

      updated_map
    }
    _ -> panic as "Can't remove non-Roll location"
  }
}

fn is_accessible(coordinate: Coordinate, map: Map) -> Bool {
  let coordinates = adjacent(coordinate)
  coordinates
  |> list.count(fn(coordinate) { is_roll(coordinate, map) })
  |> common.int_less_than(4)
}

fn is_roll(coordinate: Coordinate, map: Map) -> Bool {
  let max_row = iv.length(map.coordinates) - 1
  let max_col =
    map.coordinates
    |> iv.first
    |> result.unwrap(iv.new())
    |> iv.length
    |> int.subtract(1)

  case coordinate {
    Coordinate(row, _col) if row < 0 -> False
    Coordinate(_row, col) if col < 0 -> False
    Coordinate(row, _col) if row > max_row -> False
    Coordinate(_row, col) if col > max_col -> False
    Coordinate(_row, _col) -> get_location(coordinate, map) != Empty
  }
}

fn get_count(state: State) -> Int {
  state.count
}

fn down(coordinate: Coordinate) -> Coordinate {
  Coordinate(..coordinate, row: coordinate.row + 1)
}

fn up(coordinate: Coordinate) -> Coordinate {
  Coordinate(..coordinate, row: coordinate.row - 1)
}

fn right(coordinate: Coordinate) -> Coordinate {
  Coordinate(..coordinate, col: coordinate.col + 1)
}

fn left(coordinate: Coordinate) -> Coordinate {
  Coordinate(..coordinate, col: coordinate.col - 1)
}

fn adjacent(coordinate: Coordinate) -> List(Coordinate) {
  [
    up(coordinate),
    down(coordinate),
    right(coordinate),
    left(coordinate),
    up(right(coordinate)),
    down(right(coordinate)),
    up(left(coordinate)),
    down(left(coordinate)),
  ]
}

fn get_location(coordinate: Coordinate, map: Map) -> Location {
  map.coordinates
  |> iv.get(coordinate.row)
  |> result.try(fn(row) { iv.get(row, coordinate.col) })
  |> result.unwrap(Empty)
}

fn to_map(coordinates: List(List(Location))) -> Map {
  iv.from_list(coordinates)
  |> iv.map(iv.from_list)
  |> Map()
}

// fn map_to_string(map: Map) -> String {
//   map.coordinates
//   |> iv.map(fn (row) { row |> iv.map(loc_to_string) |> iv.join("")})
//   |> iv.join("\n")
// }
//
// fn loc_to_string(location: Location) {
//   case location {
//     Empty -> "."
//     Roll -> "@"
//     Removed -> "x"
//   }
// }

type Map {
  Map(coordinates: iv.Array(iv.Array(Location)))
}

type Location {
  Empty
  Roll
  Removed
}

type State {
  State(coordinate: Coordinate, count: Int, map: Map)
}

type Coordinate {
  Coordinate(row: Int, col: Int)
}

type Iteration {
  Iteration(curr: Map, next: Map, total: Int, done: Bool)
}
