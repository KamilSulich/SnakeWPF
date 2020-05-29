using SnakeWPF.GameEntities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        double _gameWith;
        double _gameHeight;
        int _numberOfColumns;
        int _numberOfRows;

        Apple _apple;
        Random _RandomNumber;
        SnakeElement _tailBackup;
        Direction _currentDirection;
        DispatcherTimer _gameLoopTimer;
        List<SnakeElement> _snakeElements;
        

        public MainWindow()
        {
            InitializeComponent();
           
        }

        protected override void OnContentRendered(EventArgs e)
        {
            InitializeGame();
            base.OnContentRendered(e);

        }

        void InitializeGame()
        {
            _RandomNumber = new Random(DateTime.Now.Millisecond);
            InitializeTimer();
            DrawGameWorld();
            InitializeSnake();
            DrawSnake();
            //MessageBox.Show("Sterowanie klawiszami A,W,S,D");
        }

        void ResetGame()
        {
            if (_gameLoopTimer != null)
            {
                _gameLoopTimer.Stop();
                _gameLoopTimer.Tick -= MainGameLoop;
                _gameLoopTimer = null;
            }
            if (GameWorld !=null)
            {
                GameWorld.Children.Clear();
            }
            _apple = null;
            if (_snakeElements != null)
            {
                _snakeElements.Clear();
                _snakeElements = null;
            }
            _tailBackup = null;
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
                X = (_numberOfColumns  / 2) * _elementSize,
                Y = (_numberOfRows / 2) * _elementSize,
                IsHead = true
            });

    

            _currentDirection = Direction.Right;
        }

        private void DrawGameWorld()
        {
             _gameWith = GameWorld.ActualWidth;
             _gameHeight = GameWorld.ActualHeight;
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

            for (int i = 0; i < _numberOfColumns+1; i++)
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

        private void InitializeTimer()
        {
            _gameLoopTimer = new DispatcherTimer();
            _gameLoopTimer.Interval = TimeSpan.FromSeconds(0.2);
            _gameLoopTimer.Tick += new EventHandler(MainGameLoop); 
            _gameLoopTimer.Start();
        }

        private void MakeGameFaster()
        {
            if (_gameLoopTimer.Interval.TotalSeconds>0.05)
            _gameLoopTimer.Interval = _gameLoopTimer.Interval - TimeSpan.FromSeconds(0.05);
        }

        private void  MainGameLoop(object sender, EventArgs e)
        {
            MoveSnake();
            CheckColision();
            DrawSnake();
            CreateApple();
            DrawApples();
        }

        private void DrawApples()
        {
            if (_apple == null)
                return;
           
                if (!GameWorld.Children.Contains(_apple.UIElement))
                    GameWorld.Children.Add(_apple.UIElement);

                Canvas.SetLeft(_apple.UIElement, _apple.X);
                Canvas.SetTop(_apple.UIElement, _apple.Y);

            
        }

        private void CreateApple()
        {
            if (_apple != null)
                return;

            _apple = new Apple(_elementSize)
                {
                    X= _RandomNumber.Next(0, _numberOfColumns) *_elementSize,
                 Y= _RandomNumber.Next(0, _numberOfRows) * _elementSize
                };
            

        }

        private void CheckColision()
        {
            CheckColisionWitchWorldBounds();
            CheckColisionWitchSelf();
            CheckColisionWitchWorldItems();
        }

        private void CheckColisionWitchWorldItems()
        {
            if (_apple == null)
                return;
            SnakeElement head = _snakeElements[0];
            
            if(head.X== _apple.X && head.Y== _apple.Y)
            {
                GameWorld.Children.Remove(_apple.UIElement);
                GrowSnake();
                _apple = null;
                MakeGameFaster();
            }                  
        }

        private void GrowSnake()
        {
            _snakeElements.Add(new SnakeElement(_elementSize) {X=_tailBackup.X, Y= _tailBackup.Y });
        }

        private void CheckColisionWitchSelf()
        {
            SnakeElement snakeHead = GetSnakeHead();
            bool hasCollision = false;
            if (snakeHead !=null)
            {
                foreach (var snakeElement in _snakeElements)
                {
                    if (!snakeElement.IsHead)
                    {
                        if(snakeElement.X == snakeHead.X && snakeElement.Y==snakeHead.Y)
                        {
                            hasCollision = true;
                            break;
                        }
                    }
                }
            }

            if (hasCollision)
            {
                MessageBox.Show("Wąż uderzył głową w samego siebie, koniec gry");
                ResetGame();
                InitializeGame();
            }
        }
        private SnakeElement GetSnakeHead() 
        {
            SnakeElement snakeHead = null;
            foreach (var snakeElement in _snakeElements)
            {
                if (snakeElement.IsHead)
                {
                    snakeHead = snakeElement;
                    break;
                }
            }
            return snakeHead;
        }
        private void CheckColisionWitchWorldBounds()
        {
            SnakeElement snakeHead = GetSnakeHead();
            if (snakeHead.X > _gameWith - _elementSize ||
                snakeHead.X < 0 ||
                snakeHead.Y < 0 ||
                snakeHead.Y > _gameHeight - _elementSize)
            {
                MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry. Chcesz zagrać jeszcze raz?");
                ResetGame();
                InitializeGame();
            }
        }

        private void MoveSnake()
        {
            SnakeElement head = _snakeElements[0];
            SnakeElement tail = _snakeElements[_snakeElements.Count - 1];
            _tailBackup = new SnakeElement(_elementSize)
            {
                X = tail.X,
                Y = tail.Y
            };
            head.IsHead = false;
            tail.IsHead = true;
            tail.X = head.X;
            tail.Y = head.Y;

            switch (_currentDirection)
            {
                case Direction.Right:
                    tail.X += _elementSize;
                    break;
                case Direction.Left:
                    tail.X -= _elementSize;
                    break;
                case Direction.Up:
                    tail.Y -= _elementSize;
                    break;
                case Direction.Down:
                    tail.Y += _elementSize;
                    break;
                default:
                    break;
            }

            _snakeElements.RemoveAt(_snakeElements.Count - 1);
            _snakeElements.Insert(0, tail);

        
        }
        private void KeyWasRelased(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    if (_currentDirection != Direction.Down)
                    _currentDirection = Direction.Up;
                    break;
                case Key.A:
                    if (_currentDirection != Direction.Right)
                        _currentDirection = Direction.Left;
                    break;
                case Key.S:
                    if (_currentDirection != Direction.Up)
                        _currentDirection = Direction.Down;
                    break;
                case Key.D:
                    if (_currentDirection != Direction.Left)
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