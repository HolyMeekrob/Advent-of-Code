using System.Text.RegularExpressions;

namespace _2022._07;

public static class Day7
{
	private const string InputFileName = "Day7.txt";
	private const string TestInputFileName = "Day7_Test.txt";

	public static (object Part1, object Part2) Run(bool isTest)
	{
		var filename = isTest ? TestInputFileName : InputFileName;
		var lines = File.ReadAllLines(Path.Combine("07", filename));

		var filesystem = BuildFilesystem(lines);

		return (GetPartOne(filesystem), GetPartTwo(filesystem));
	}

	private static ElfDir BuildFilesystem(IReadOnlyList<string> terminalLines)
	{
		var root = new ElfDir("Root");

		var result = ProcessLines(new ProcessState
		{
			CurrentDir = root,
			TerminalLines = terminalLines,
		});

		return GetRoot(result.CurrentDir);
	}

	private static long GetPartOne(ElfDir filesystem) =>
		GetAllDirectories(filesystem).Where(dir => dir.Size <= 100000).Sum(dir => dir.Size);

	private static long GetPartTwo(ElfDir filesystem)
	{
		const long totalDiskSpace = 70000000;
		const long requiredDiskSpace = 30000000;
		var usedDiskSpace = filesystem.Size;
		var unusedDiskSpace = totalDiskSpace - usedDiskSpace;
		var neededDiskSpace = requiredDiskSpace - unusedDiskSpace;

		return GetAllDirectories(filesystem)
			.Select(dir => dir.Size)
			.Where(size => size >= neededDiskSpace)
			.Order()
			.First();
	}

	private static IReadOnlyList<ElfDir> GetAllDirectories(ElfDir currentDir) =>
		currentDir.Directories.SelectMany(GetAllDirectories).Append(currentDir).ToList();

	private static ProcessState ProcessLines(ProcessState state)
	{
		if (!state.TerminalLines.Any())
		{
			return state;
		}

		var line = state.TerminalLines[0];

		var dir = line switch
		{
			['$', ' ', ..] => ProcessCommand(line[2..], state.CurrentDir),
			['d', 'i', 'r', ..] => ProcessDirectory(line[4..], state.CurrentDir),
			_ => ProcessFile(line, state.CurrentDir)
		};

		return ProcessLines(new ProcessState
		{
			CurrentDir = dir,
			TerminalLines = state.TerminalLines.Skip(1).ToList(),
		});
	}

	private static ElfDir ProcessCommand(string command, ElfDir currentDir) =>
		command switch
		{
			['c', 'd', ..] => ChangeDirectory(command[3..], currentDir),
			_ => currentDir,
		};

	private static ElfDir ChangeDirectory(string dirName, ElfDir currentDir)
	{
		if (dirName == "/")
		{
			return GetRoot(currentDir);
		}

		return dirName == ".."
			? currentDir.Parent!
			: currentDir.Directories.First(dir => dir.Name == dirName);
	}

	private static ElfDir ProcessDirectory(string dirName, ElfDir currentDir)
	{
		if (currentDir.Directories.Any(dir => dir.Name == dirName))
		{
			return currentDir;
		}

		currentDir.Directories.Add(new ElfDir(dirName, currentDir));
		return currentDir;
	}

	private static ElfDir ProcessFile(string line, ElfDir currentDir)
	{
		var regex = new Regex(@"(?<size>\d+) (?<name>\w+)", RegexOptions.Compiled);
		var match = regex.Match(line);

		var file = new ElfFile(match.Groups["name"].Value, long.Parse(match.Groups["size"].Value));
		currentDir.Files.Add(file);

		return currentDir;
	}

	private static ElfDir GetRoot(ElfDir dir) =>
		dir.Parent == null ? dir : GetRoot(dir.Parent);

	private sealed record ElfDir(string Name, ElfDir? Parent = null)
	{
		public List<ElfDir> Directories { get; } = new();
		public List<ElfFile> Files { get; } = new();

		private long? size = null;

		public long Size
		{
			get
			{
				this.size ??= this.Files.Sum(file => file.Size) + this.Directories.Sum(dir => dir.Size);
				return this.size.Value;
			}
		}
	}

	private sealed record ElfFile(string Name, long Size);

	private sealed record ProcessState
	{
		public required ElfDir CurrentDir { get; init; }
		public required IReadOnlyList<string> TerminalLines { get; init; }
	}
}
