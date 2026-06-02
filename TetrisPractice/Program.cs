using System;
using System.Threading;

namespace TetrisPractice
{
    class Program
    {
        static int width = 10;
        static int height = 20;
        static int[,] field = new int[20, 10];

        static int[,] currentFigure;
        static int figureX;
        static int figureY;

        static Random random = new Random();

        static void Main()
        {
            Console.CursorVisible = false;
            SpawnFigure();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.LeftArrow)
                        MoveFigure(-1, 0);
                    if (key == ConsoleKey.RightArrow)
                        MoveFigure(1, 0);
                    if (key == ConsoleKey.DownArrow)
                        MoveFigure(0, 1);
                    if (key == ConsoleKey.UpArrow)
                        RotateFigure();
                }

                if (!MoveFigure(0, 1))
                {
                    MergeFigure();
                    ClearLines();
                    SpawnFigure();

                    if (!IsValidPosition())
                    {
                        Console.Clear();
                        Console.WriteLine("GAME OVER");
                        break;
                    }
                }

                Draw();
                Thread.Sleep(300);
            }

            Console.ReadKey();
        }

        static void SpawnFigure()
        {
            currentFigure = GetRandomFigure();
            figureX = width / 2 - 1;
            figureY = 0;
        }

        static int[,] GetRandomFigure()
        {
            int choice = random.Next(2);

            if (choice == 0)
                return new int[,] { { 1, 1 }, { 1, 1 } }; // квадрат
            else
                return new int[,] { { 1, 1, 1, 1 } }; // линия
        }

        static bool MoveFigure(int dx, int dy)
        {
            figureX += dx;
            figureY += dy;

            if (!IsValidPosition())
            {
                figureX -= dx;
                figureY -= dy;
                return false;
            }

            return true;
        }

        static void RotateFigure()
        {
            int rows = currentFigure.GetLength(0);
            int cols = currentFigure.GetLength(1);

            int[,] rotated = new int[cols, rows];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    rotated[j, rows - 1 - i] = currentFigure[i, j];

            currentFigure = rotated;

            if (!IsValidPosition())
                RotateFigure(); // откат если нельзя вращать
        }

        static bool IsValidPosition()
        {
            for (int i = 0; i < currentFigure.GetLength(0); i++)
            {
                for (int j = 0; j < currentFigure.GetLength(1); j++)
                {
                    if (currentFigure[i, j] == 1)
                    {
                        int x = figureX + j;
                        int y = figureY + i;

                        if (x < 0 || x >= width || y >= height)
                            return false;

                        if (y >= 0 && field[y, x] == 1)
                            return false;
                    }
                }
            }
            return true;
        }

        static void MergeFigure()
        {
            for (int i = 0; i < currentFigure.GetLength(0); i++)
            {
                for (int j = 0; j < currentFigure.GetLength(1); j++)
                {
                    if (currentFigure[i, j] == 1)
                        field[figureY + i, figureX + j] = 1;
                }
            }
        }

        static void ClearLines()
        {
            for (int i = height - 1; i >= 0; i--)
            {
                bool full = true;

                for (int j = 0; j < width; j++)
                {
                    if (field[i, j] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    for (int k = i; k > 0; k--)
                        for (int j = 0; j < width; j++)
                            field[k, j] = field[k - 1, j];
                }
            }
        }

        static void Draw()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (field[i, j] == 1 || IsFigureCell(i, j))
                        Console.Write("■");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        static bool IsFigureCell(int row, int col)
        {
            for (int i = 0; i < currentFigure.GetLength(0); i++)
            {
                for (int j = 0; j < currentFigure.GetLength(1); j++)
                {
                    if (currentFigure[i, j] == 1)
                    {
                        if (figureY + i == row && figureX + j == col)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}