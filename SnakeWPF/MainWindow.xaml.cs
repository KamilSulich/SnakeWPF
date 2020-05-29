﻿using SnakeWPF.GameEntities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Linq;
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
        List<Apple> _apples;
        private Random _RandomNumber;

        private Direction _currentDirection;
        private double _gameWith;
        private double _gameHeight;
        private long _elapsedTicks;
        private SnakeElement _tailBackup;

        public MainWindow()
        {
            InitializeComponent();
            _RandomNumber = new Random(DateTime.Now.Millisecond / DateTime.Now.Second);
            _apples = new List<Apple>();
            InitializeTimer();
            DrawGameWorld();
            InitializeSnake();
            DrawSnake();
            //MessageBox.Show("Sterowanie klawiszami A,W,S,D");
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
            CreateApple();
            DrawApples();
            _elapsedTicks++;
        }

        private void DrawApples()
        {
            foreach (var apple in _apples)
            {
                if (!GameWorld.Children.Contains(apple.UIElement))
                    GameWorld.Children.Add(apple.UIElement);

                Canvas.SetLeft(apple.UIElement, apple.X);
                Canvas.SetTop(apple.UIElement, apple.Y);

            }
        }

        private void CreateApple()
        {
            if ( _elapsedTicks / 20 == 0)
            {
                _apples.Add(new Apple(_elementSize)
                {X= _RandomNumber.Next(0, _numberOfColumns) *_elementSize,
                 Y= _RandomNumber.Next(0, _numberOfRows) * _elementSize
                });;
            }

        }

        private void CheckColision()
        {
            CheckColisionWitchWorldBounds();
            CheckColisionWitchSelf();
            CheckColisionWitchWorldItems();
        }

        private void CheckColisionWitchWorldItems()
        {

            SnakeElement head = _snakeElements[0];
            Apple collidedWitchSnake = null;
            foreach (var apple in _apples)
            {
                if(head.X==apple.X && head.Y==apple.Y)
                {
                    collidedWitchSnake = apple;
                    break;
                }
            }
            if (collidedWitchSnake != null)
            {
                _apples.Remove(collidedWitchSnake);
                GameWorld.Children.Remove(collidedWitchSnake.UIElement);
                GrowSnake();
            }
        }

        private void GrowSnake()
        {
            _snakeElements.Add(new SnakeElement(_elementSize) {X=_tailBackup.X, Y= _tailBackup.Y });
        }

        private void CheckColisionWitchSelf()
        {
            SnakeElement snakeHead = GetSnakeHead();
            if (snakeHead !=null)
            {
                foreach (var snakeElement in _snakeElements)
                {
                    if (!snakeElement.IsHead)
                    {
                        if(snakeElement.X == snakeHead.X && snakeElement.Y==snakeHead.Y)
                        {
                            MessageBox.Show("Wąż uderzył głową w samego siebie, koniec gry");
                        }
                        break;
                    }
                }
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
                MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry");
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