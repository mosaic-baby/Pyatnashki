using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyatnashki
{
    public class GameLogic
    {
        // Двумерный массив для логического представления поля
        // Значение 0 представляет собой пустую ячейку
        public int[,] Grid { get; private set; }

        public int Size { get; private set; }
        public bool IsGameWon { get; private set; }

        // Координаты пустой ячейки (нуля)
        private int emptyRow;
        private int emptyCol;

        // Конструктор инициализирует поле заданного размера
        public GameLogic(int size)
        {
            if (size < 2) size = 4; // Стандартный размер по умолчанию 4х4
            Size = size;
            Grid = new int[size, size];
            GenerateLevel();
        }

        // 1. Модуль генерации уровня
        public void GenerateLevel()
        {
            IsGameWon = false;
            int[] linearArray = new int[Size * Size];

            // Заполняем массив числами от 1 до (Size*Size - 1), последняя ячейка - 0
            for (int i = 0; i < linearArray.Length - 1; i++)
            {
                linearArray[i] = i + 1;
            }
            linearArray[linearArray.Length - 1] = 0;

            // Алгоритм перемешивания: совершаем случайные валидные ходы из победного состояния,
            // чтобы пятнашки гарантированно имели решение (правило математической разрешимости)
            Random rand = new Random();
            int currentEmptyIndex = linearArray.Length - 1;

            for (int i = 0; i < 1000; i++) // 1000 случайных сдвигов
            {
                int row = currentEmptyIndex / Size;
                int col = currentEmptyIndex % Size;

                // Выбираем случайное направление: 0 - вверх, 1 - вниз, 2 - влево, 3 - вправо
                int dir = rand.Next(4);
                int targetRow = row;
                int targetCol = col;

                if (dir == 0 && row > 0) targetRow--;
                else if (dir == 1 && row < Size - 1) targetRow++;
                else if (dir == 2 && col > 0) targetCol--;
                else if (dir == 3 && col < Size - 1) targetCol++;

                int targetIndex = targetRow * Size + targetCol;

                // Меняем местами пустую ячейку со случайным соседом
                int temp = linearArray[currentEmptyIndex];
                linearArray[currentEmptyIndex] = linearArray[targetIndex];
                linearArray[targetIndex] = temp;

                currentEmptyIndex = targetIndex;
            }

            // Переносим одномерный перемешанный массив в двумерный массив Grid
            for (int i = 0; i < linearArray.Length; i++)
            {
                int r = i / Size;
                int c = i % Size;
                Grid[r, c] = linearArray[i];

                if (linearArray[i] == 0)
                {
                    emptyRow = r;
                    emptyCol = c;
                }
            }
        }

        // Проверка допустимости действия пользователя и совершение хода
        public bool MakeMove(int targetRow, int targetCol)
        {
            if (IsGameWon) return false;

            // Проверяем, находится ли выбранная ячейка по соседству с пустой ячейкой (по горизонтали или вертикали)
            bool isNeighbor = (Math.Abs(targetRow - emptyRow) == 1 && targetCol == emptyCol) ||
                              (Math.Abs(targetCol - emptyCol) == 1 && targetRow == emptyRow);

            if (isNeighbor)
            {
                // Каждое действие приводит к изменению состояния поля
                Grid[emptyRow, emptyCol] = Grid[targetRow, targetCol];
                Grid[targetRow, targetCol] = 0;

                // Обновляем координаты пустой ячейки
                emptyRow = targetRow;
                emptyCol = targetCol;

                // Проверяем условия победы после каждого действия
                CheckWinCondition();
                return true; // Ход успешно совершен
            }

            return false; // Действие недопустимо
        }

        // Проверка условий победы
        private void CheckWinCondition()
        {
            int expectedValue = 1;

            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    // Последняя ячейка должна быть равна 0
                    if (r == Size - 1 && c == Size - 1)
                    {
                        if (Grid[r, c] == 0)
                        {
                            IsGameWon = true;
                            return;
                        }
                    }
                    // Все остальные ячейки должны идти строго по порядку от 1
                    else if (Grid[r, c] != expectedValue)
                    {
                        IsGameWon = false;
                        return;
                    }
                    expectedValue++;
                }
            }
        }
    }
}