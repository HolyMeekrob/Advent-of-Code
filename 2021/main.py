import importlib
import sys

day = int(sys.argv[1])
day_str = f"{day:02}"

module = importlib.import_module(f"{day_str}.{day_str}")

module.run()
