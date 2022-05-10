using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemF
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
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "tests", nameof(TestProblemF));
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
                var threadText = ReadLine(_args);
                var dataProperies = threadText.Split(' ').Select(it => int.Parse(it));

                var rowCount = dataProperies.First();
                var columnCount = dataProperies.Last();

                var gameData = GetData(rowCount, columnCount);

                var gameBoard = new GameBoard(rowCount, columnCount);
                gameBoard.LoadData(gameData);
                gameBoard.StonesReplace();
                gameBoard.GetWater();

                stringBuilder.Append(gameBoard.DrawBoard());

                stringBuilder.Append(Environment.NewLine);
            }

            ConsoleWriter.WriteLine(stringBuilder.ToString());
        }

        private static IEnumerable<string> GetData(int rowCount, int columnCount)
        {
            var result = new List<string>();

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                result.Add(ReadLine(_args));
            }

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

        private BaseBlock[,] _board;

        public GameBoard(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
        }

        public void StonesReplace()
        {
            for (int columnIndex = 0; columnIndex < _columnCount; columnIndex++)
            {
                var blockCount = 0;

                for (int rowIndex = _rowCount - 1; rowIndex > -1; rowIndex--)
                {
                    var block = _board[rowIndex, columnIndex];

                    if (block is StoneBlock stone)
                    {
                        blockCount++;
                    }
                }

                for (int rowIndex = _rowCount - 1; rowIndex > -1; rowIndex--)
                {
                    if (blockCount < 1)
                    {
                        _board[rowIndex, columnIndex] = new EmptyBlock();
                    }
                    else
                    {
                        _board[rowIndex, columnIndex] = new StoneBlock();
                        blockCount--;
                    }
                }
            }
        }

        public void GetWater()
        {
            for (int rowIndex = _rowCount-1; rowIndex > -1; rowIndex--)
            {
                var inds = new List<int>();

                var firsStone = false;

                for (int columnIndex = 0; columnIndex < _columnCount; columnIndex++)
                {
                    var block = _board[rowIndex, columnIndex];

                    if (block is StoneBlock stone)
                    {
                        if (firsStone)
                        {
                            foreach (var id in inds)
                                _board[rowIndex, id] = new WaterBlock();

                            if (inds.Any())
                                inds.Clear();
                        }

                        firsStone = true;
                    }
                    else
                    {
                        if (firsStone)
                            inds.Add(columnIndex);
                    }
                }
            }
        }

        public void LoadData(IEnumerable<string> columnLines)
        {
            _board = new BaseBlock[_rowCount, _columnCount];

            var rowIndex = 0;

            foreach (var columnLine in columnLines)
            {
                for (int columnIndex = 0; columnIndex < columnLine.Length; columnIndex++)
                {
                    var markBlock = columnLine[columnIndex];

                    if (markBlock == '.')
                        _board[rowIndex, columnIndex] = new EmptyBlock();

                    if (markBlock == '*')
                        _board[rowIndex, columnIndex] = new StoneBlock();
                }

                rowIndex++;
            }
        }

        public string DrawBoard()
        {
            var stringBuilder = new StringBuilder();

            for (int rowIndex = 0; rowIndex < _rowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < _columnCount; columnIndex++)
                {
                    stringBuilder.Append(_board[rowIndex, columnIndex].Mark);
                }

                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }

    public abstract class BaseBlock
    {
        public virtual char Mark => '0';
    }

    public sealed class EmptyBlock : BaseBlock
    {
        public override char Mark => '.';
    }

    public sealed class StoneBlock : BaseBlock
    {
        public override char Mark => '*';
    }

    public sealed class WaterBlock : BaseBlock
    {
        public override char Mark => '~';
    }
}
