import common
import gleam/float
import gleam/int
import gleam/list
import gleam/order.{type Order}
import gleam/result
import gleam/set.{type Set}
import gleam/string

const day = 8

pub fn solve(is_test: Bool) -> common.Solution {
  let assert Ok(input) = common.get_input(day, is_test)
  let points = parse(input)
  let distances =
    points
    |> list.combination_pairs
    |> list.map(to_distance)
    |> list.sort(by_distance)

  common.Solution(solve_1(distances, is_test), solve_2(distances))
}

fn solve_1(distances: List(Distance), is_test: Bool) -> String {
  let count = case is_test {
    True -> 10
    False -> 1000
  }

  distances
  |> list.take(count)
  |> list.fold([], connect)
  |> keep_merging
  |> list.map(set.size)
  |> list.sort(int.compare)
  |> list.reverse
  |> list.take(3)
  |> int.product
  |> int.to_string
}

fn solve_2(distances: List(Distance)) -> String {
  distances
  |> list.fold(#(set.new(), set.new()), last_new_pair)
  |> fn(result) { result.1 }
  |> set.to_list
  |> list.map(fn(point) { point.x })
  |> int.product
  |> int.to_string
}

fn parse(input: String) -> List(Point) {
  input
  |> string.split("\n")
  |> list.filter(common.string_is_not_empty)
  |> list.map(parse_coordinates)
}

fn parse_coordinates(line: String) -> Point {
  line
  |> string.split(",")
  |> list.map(int.parse)
  |> list.map(result.unwrap(_, 0))
  |> list_to_point()
}

fn list_to_point(lst: List(Int)) -> Point {
  case lst {
    [x, y, z] -> Point(x, y, z)
    _ -> panic as "Incorrect point format"
  }
}

fn square(n: Int) -> Float {
  let assert Ok(sq) = int.power(n, 2.0)
  sq
}

fn distance(a: Point, b: Point) -> Float {
  let x = square(a.x - b.x)
  let y = square(a.y - b.y)
  let z = square(a.z - b.z)
  let sum = x |> float.add(y) |> float.add(z)

  let assert Ok(result) = float.square_root(sum)
  result
}

fn by_distance(a: Distance, b: Distance) -> Order {
  float.compare(a.2, b.2)
}

fn to_distance(pair: #(Point, Point)) -> Distance {
  #(pair.0, pair.1, distance(pair.0, pair.1))
}

fn connect(connections: List(Set(Point)), next: Distance) -> List(Set(Point)) {
  let connection = set.from_list([next.0, next.1])
  let result =
    list.map_fold(
      connections,
      False,
      fn(has_matched: Bool, points: Set(Point)) -> #(Bool, Set(Point)) {
        case has_matched, set.is_disjoint(points, connection) {
          True, _ -> #(True, points)
          _, True -> #(False, points)
          _, False -> #(True, set.union(points, connection))
        }
      },
    )

  case result.0 {
    True -> result.1
    False -> [connection, ..result.1]
  }
}

fn keep_merging(connections: List(Set(Point))) -> List(Set(Point)) {
  let result =
    list.fold(
      connections,
      #([], False),
      fn(state: #(List(Set(Point)), Bool), next: Set(Point)) -> #(
        List(Set(Point)),
        Bool,
      ) {
        let merge_set = merge(state.0, next)
        #(merge_set.0, state.1 || merge_set.1)
      },
    )

  case result.1 {
    True -> keep_merging(result.0)
    False -> result.0
  }
}

fn merge(
  circuits: List(Set(Point)),
  circuit: Set(Point),
) -> #(List(Set(Point)), Bool) {
  case circuits {
    [] -> #([circuit], False)
    [head, ..rest] ->
      case set.is_disjoint(head, circuit) {
        True -> {
          let result = merge(rest, circuit)
          #([head, ..result.0], result.1)
        }
        False -> {
          #([set.union(head, circuit), ..rest], True)
        }
      }
  }
}

fn last_new_pair(
  found: #(Set(Point), Set(Point)),
  next: Distance,
) -> #(Set(Point), Set(Point)) {
  case set.contains(found.0, next.0), set.contains(found.0, next.1) {
    True, True -> found
    _, _ -> {
      let next_set = set.from_list([next.0, next.1])
      #(set.union(found.0, next_set), next_set)
    }
  }
}

type Point {
  Point(x: Int, y: Int, z: Int)
}

type Distance =
  #(Point, Point, Float)
