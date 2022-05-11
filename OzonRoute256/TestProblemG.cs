using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemG
    {
        private static int _readCount;
        private static string[] _args;

        public static void Main(string[] args)
        {
            var testFiles = GetTestFiles();

            if (testFiles == null || !testFiles.Any())
            {
                RunGame();
            }
            else
            {
                foreach (var file in testFiles)
                {
                    _readCount = 0;
                    _args = GetDataFromFile(file);
                    RunGame();
                }
            }
        }

        private static IEnumerable<string>? GetTestFiles()
        {
            try
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "tests", nameof(TestProblemG));
                return Directory.EnumerateFiles(dir);
            }
            catch
            {
                return null;
            }
        }

        private static string[] GetDataFromFile(string file)
        {
            return File.ReadAllLines(file);
        }

        private static string? ReadLine(string[] args)
        {
            if (args == null || !args.Any())
            {
                return Console.ReadLine();
            }
            else
            {
                var arg = args[_readCount];
                _readCount++;
                return arg;
            }
        }

        private static void RunGame()
        {
            var dataCount = GetCount(ReadLine(_args));

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < dataCount; i++)
            {
                ReadLine(_args);

                var rowCount = 3;
                var columnCount = 3;

                var gameData = GetData(rowCount);

                var gameBoard = new GameBoard(rowCount, columnCount);
                gameBoard.LoadData(gameData);
                gameBoard.CheckState();

                stringBuilder.Append(gameBoard.DrawState());
                stringBuilder.Append(Environment.NewLine);
            }

            ConsoleWriter.WriteLine(stringBuilder.ToString());
        }

        private static IEnumerable<string> GetData(int rowCount)
        {
            var result = new List<string>();

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                result.Add(ReadLine(_args));
            }

            result.Reverse();

            return result;
        }

        private static int GetCount(string? input)
        {
            if (int.TryParse(input, out var collectionCount))
            {
                return collectionCount;
            }

            throw new Exception("Bad count");
        }
    }

    public class ConsoleWriter
    {
        public static void WriteLine(object? output) => Console.WriteLine(output);
    }

    public sealed class GameBoard
    {
        private readonly int _rowCount;
        private readonly int _columnCount;

        private BaseCell[,] _board;

        private List<CrossCell> _crosses;
        private List<ZeroCell> _zeros;

        private bool _boardState = false;

        public GameBoard(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _crosses = new List<CrossCell>();
            _zeros = new List<ZeroCell>();
        }

        public void CheckState()
        {
            if (_zeros.Count > _crosses.Count)
                return;

            if ((_crosses.Count - _zeros.Count) > 1)
                return;

            if (_zeros.Count == 0 && _crosses.Count == 0)
            {
                _boardState = true;
                return;
            }

            var rows = new List<IReadOnlyList<BaseCell>>();

            var lines = GetAllBoardLines();

            var fullCrossLines = lines.Where(l => l.FullCrosses).ToList();
            var fullNullesLines = lines.Where(l => l.FullNulles).ToList();

            if (fullCrossLines.Count > 1 || fullNullesLines.Count > 1)
                return;

            if (fullCrossLines.Count == 0 && fullNullesLines.Count == 0)
            {
                if (_zeros.Count <= _crosses.Count)
                {
                    _boardState = true;
                    return;
                }
            }

            if (fullCrossLines.Count == 0 && fullNullesLines.Count > 0)
            {
                if (_zeros.Count == _crosses.Count)
                {
                    _boardState = true;
                    return;
                }
            }

            if (fullCrossLines.Count > 0 && fullNullesLines.Count == 0)
            {
                if (_zeros.Count < _crosses.Count)
                {
                    _boardState = true;
                    return;
                }
            }
        }

        public void LoadData(IEnumerable<string> columnLines)
        {
            _board = new BaseCell[_rowCount, _columnCount];

            var rowIndex = 0;

            foreach (var columnLine in columnLines)
            {
                for (int columnIndex = 0; columnIndex < columnLine.Length; columnIndex++)
                {
                    var markBlock = columnLine[columnIndex];

                    if (markBlock == '.')
                    {
                        _board[rowIndex, columnIndex] = new EmptyCell();
                    }

                    if (markBlock == 'X')
                    {
                        var cross = new CrossCell();
                        _board[rowIndex, columnIndex] = cross;
                        _crosses.Add(cross);
                    }

                    if (markBlock == '0')
                    {
                        var nullCell = new ZeroCell();
                        _board[rowIndex, columnIndex] = nullCell;
                        _zeros.Add(nullCell);
                    }
                }

                rowIndex++;
            }
        }

        public string DrawState()
        {
            if (_boardState)
                return "YES";
            else
                return "NO";
        }

        private IReadOnlyList<BoardLine> GetAllBoardLines()
        {
            var result = new List<BoardLine>(_columnCount);

            for (int columnIndex = 0; columnIndex < _columnCount; columnIndex++)
            {
                var columnLine = new BoardLine();

                for (int rowIndex = 0; rowIndex < _rowCount; rowIndex++)
                {
                    columnLine.AddCell(_board[rowIndex, columnIndex]);
                }
                result.Add(columnLine);
            }

            for (int rowIndex = 0; rowIndex < _rowCount; rowIndex++)
            {
                var rowLine = new BoardLine();

                for (int columnIndex = 0; columnIndex < _columnCount; columnIndex++)
                {
                    rowLine.AddCell(_board[rowIndex, columnIndex]);
                }
                result.Add(rowLine);
            }

            var firstDiagonal = new BoardLine();

            firstDiagonal.AddCell(_board[0, 0]);
            firstDiagonal.AddCell(_board[1, 1]);
            firstDiagonal.AddCell(_board[2, 2]);

            var secondDiagonal = new BoardLine();

            secondDiagonal.AddCell(_board[2, 0]);
            secondDiagonal.AddCell(_board[1, 1]);
            secondDiagonal.AddCell(_board[0, 2]);

            result.Add(firstDiagonal);
            result.Add(secondDiagonal);

            return result;
        }
    }

    public class BoardLine
    {
        private bool _fullCrosses;
        private bool _fullNulles;
        private List<BaseCell> _cells;
        public bool FullCrosses => _fullCrosses;
        public bool FullNulles => _fullNulles;

        public BoardLine()
        {
            _cells = new List<BaseCell>();
        }
        public void AddCell(BaseCell baseCell)
        {
            _cells.Add(baseCell);
            GetFullState();
        }

        private void GetFullState()
        {
            var crossCount = _cells.Where(cell => cell is CrossCell).Count();
            var nullCount = _cells.Where(cell => cell is ZeroCell).Count();

            if (crossCount == 3)
                _fullCrosses = true;

            if (nullCount == 3)
                _fullNulles = true;
        }
    }

    public abstract class BaseCell
    {
        public virtual char Mark => '?';
    }

    public sealed class EmptyCell : BaseCell
    {
        public override char Mark => '.';
    }

    public sealed class CrossCell : BaseCell
    {
        public override char Mark => 'X';
    }

    public sealed class ZeroCell : BaseCell
    {
        public override char Mark => '0';
    }
}
