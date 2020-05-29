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
using System.Windows.Threading;
namespace SnakeWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _elementSize = 20;
        private int _numberOfColumns;
        private int _numberOfRows;

        DispatcherTimer _gameLoopTimer;
        List<SnakeElement> _snakeElements;

        private Direction _currentDirection;
        private double _gameWith;
        private double _gameHeight;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            DrawGameWorld();
            InitializeSnake();
            DrawSnake();
        }

        private void DrawSnake()
        {
            foreach (var snakeElement in _snakeElements)
            {
                if (!GameWorld.Children.Contains(snakeElement.UIElement))
                    GameWorld.Children.Add(snakeElement.UIElement);

                Canvas.SetLeft(snakeElement.UIElement, snakeElement.X);
                Canvas.SetTop(snakeElement.UIElement, snakeElement.Y);

            }
        }

        private void InitializeSnake()
        {
            _snakeElements = new List<SnakeElement>();
            _snakeElements.Add(new SnakeElement(_elementSize)
            {
                X = (_numberOfRows / 2) * _elementSize,
                Y = (_numberOfColumns / 2) * _elementSize,
                IsHead = true
            });
            _currentDirection = Direction.Left;
        }

        private void DrawGameWorld()
        {
             _gameWith = Width;
             _gameHeight = Height;
             _numberOfColumns = (int)_gameWith / _elementSize;
             _numberOfRows = (int)_gameHeight / _elementSize;

            for (int i=0; i< _numberOfRows; i++ )
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = 0;
                line.Y1 = i * _elementSize;
                line.X2 = _gameWith;
                line.Y2 = i * _elementSize;
                GameWorld.Children.Add(line);
            }

            for (int i = 0; i < _numberOfColumns; i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = i * _elementSize;
                line.Y1 = 0;
                line.X2 = i * _elementSize;
                line.Y2 = _gameHeight;
                GameWorld.Children.Add(line);
            }
        }

        public void InitializeTimer()
        {
            _gameLoopTimer = new DispatcherTimer();
            _gameLoopTimer.Interval = TimeSpan.FromSeconds(0.2);
            _gameLoopTimer.Tick += new EventHandler(MainGameLoop); 
            _gameLoopTimer.Start();
        }

        private void  MainGameLoop(object sender, EventArgs e)
        {
            MoveSnake();
            CheckColision();
            DrawSnake();
        }

        private void CheckColision()
        {
            CheckColisionWitchWorldBounds();
            CheckColisionWitchSelf();
            CheckColisionWitchWorldItems();
        }

        private void CheckColisionWitchWorldItems()
        {
            foreach (var snakeElement in _snakeElements)
                if (snakeElement.X > _gameWith || snakeElement.X < 0 || snakeElement.Y < 0 || snakeElement.Y > _gameHeight)
                    MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry");
        }

        private void CheckColisionWitchSelf()
        {
        }

        private void CheckColisionWitchWorldBounds()
        {
        }

        private void MoveSnake()
        {
            foreach (var snakeElement in _snakeElements)
            {
                switch (_currentDirection)
                {
                    case Direction.Right:
                        snakeElement.X += _elementSize;
                        break;
                    case Direction.Left:
                        snakeElement.X -= _elementSize;
                        break;
                    case Direction.Up:
                        snakeElement.Y -= _elementSize;
                        break;
                    case Direction.Down:
                        snakeElement.Y += _elementSize;
                        break;
                    default:
                        break;
                }
            }
        }
        private void KeyWasRelased(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    _currentDirection = Direction.Up;
                    break;
                case Key.A:
                    _currentDirection = Direction.Left;
                    break;
                case Key.S:
                    _currentDirection = Direction.Down;
                    break;
                case Key.D:
                    _currentDirection = Direction.Right;
                    break;

            }
        }
    }
}

enum Direction
{
    Right,
    Left,
    Up,
    Down
}