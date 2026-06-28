using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pyatnashki
{
    public partial class MainWindow : Window
    {
        private GameLogic game;

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame(); // Запуск стандартной игры 4х4 при старте
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            // Проверка ввода корректного размера поля пользователем
            if (!int.TryParse(TxtSize.Text, out int size) || size < 2 || size > 6)
            {
                MessageBox.Show("Пожалуйста, введите корректный размер поля от 2 до 6.", "Ошибка ввода");
                TxtSize.Text = "4";
                return;
            }

            // Создаем экземпляр логики «Генератор уровней»
            game = new GameLogic(size);

            // Настраиваем сетку отображения под заданный размер
            GameGrid.Rows = size;
            GameGrid.Columns = size;

            UpdateVisualGrid();
        }

        // Функция визуального отображения игрового поля
        private void UpdateVisualGrid()
        {
            GameGrid.Children.Clear();

            for (int r = 0; r < game.Size; r++)
            {
                for (int c = 0; c < game.Size; c++)
                {
                    int val = game.Grid[r, c];
                    Button btn = new Button();

                    // Задаем координаты ячейки в тег кнопки, чтобы знать куда кликнул игрок
                    btn.Tag = new Point(r, c);
                    btn.Click += ButtonTile_Click;
                    btn.Margin = new Thickness(2);
                    btn.FontSize = 20;
                    btn.FontWeight = FontWeights.Bold;

                    if (val == 0)
                    {
                        // Пустая ячейка (ноль) делается невидимой и неактивной
                        btn.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        btn.Content = val.ToString();
                        btn.Background = Brushes.LightSkyBlue;
                    }

                    GameGrid.Children.Add(btn);
                }
            }
        }

        // Обработка действий пользователя
        private void ButtonTile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.Tag is Point coords)
            {
                int row = (int)coords.X;
                int col = (int)coords.Y;

                // Передаем координаты клика в логический модуль
                if (game.MakeMove(row, col))
                {
                    // Если ход валидный, обновляем графику
                    UpdateVisualGrid();

                    // После каждого действия проверяется условие победы
                    if (game.IsGameWon)
                    {
                        MessageBox.Show("Поздравляем! Вы успешно собрали Пятнашки!", "Победа!");
                    }
                }
            }
        }
    }
}