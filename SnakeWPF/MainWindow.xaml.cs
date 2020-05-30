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
        int elementSize = 20;
        double gameWith;
        double gameHeight;
        int numberOfColumns;
        int numberOfRows;

        Apple apple;
        Random RandomNumber;
        SnakeElement tailBackup;
        Direction currentDirection;
        DispatcherTimer gameLoopTimer;
        List<SnakeElement> snakeElements;
        

        public MainWindow()
        {
            InitializeComponent();
           
        }

        protected override void OnContentRendered(EventArgs e)
        {
            MessageBox.Show("Sterowanie klawiszami A,W,S,D. Wciśnij przycisk OK, aby kontynuować.");
            InitializeGame();
            base.OnContentRendered(e);

        }

        void InitializeGame()
        {
            RandomNumber = new Random(DateTime.Now.Millisecond);
            InitializeTimer();
            DrawGameWorld();
            InitializeSnake();
            DrawSnake();           
        }

        void ResetGame()
        {
            if (gameLoopTimer != null)
            {
                gameLoopTimer.Stop();
                gameLoopTimer.Tick -= MainGameLoop;
                gameLoopTimer = null;
            }
            if (GameWorld !=null)
            {
                GameWorld.Children.Clear();
            }
            apple = null;
            if (snakeElements != null)
            {
                snakeElements.Clear();
                snakeElements = null;
            }
            tailBackup = null;
        }

        private void DrawSnake()
        {
            foreach (var snakeElement in snakeElements)
            {
                if (!GameWorld.Children.Contains(snakeElement.UIElement))
                    GameWorld.Children.Add(snakeElement.UIElement);

                Canvas.SetLeft(snakeElement.UIElement, snakeElement.X);
                Canvas.SetTop(snakeElement.UIElement, snakeElement.Y);

            }
        }

        private void InitializeSnake()
        {
            snakeElements = new List<SnakeElement>();
            snakeElements.Add(new SnakeElement(elementSize)
            {
                X = (numberOfColumns  / 2) * elementSize,
                Y = (numberOfRows / 2) * elementSize,
                IsHead = true
            });

    

            currentDirection = Direction.Right;
        }

        private void DrawGameWorld()
        {
             gameWith = GameWorld.ActualWidth;
             gameHeight = GameWorld.ActualHeight;
             numberOfColumns = (int)gameWith / elementSize;
             numberOfRows = (int)gameHeight / elementSize;

            for (int i=0; i< numberOfRows; i++ )
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = 0;
                line.Y1 = i * elementSize;
                line.X2 = gameWith;
                line.Y2 = i * elementSize;
                GameWorld.Children.Add(line);
            }

            for (int i = 0; i < numberOfColumns+1; i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = i * elementSize;
                line.Y1 = 0;
                line.X2 = i * elementSize;
                line.Y2 = gameHeight;
                GameWorld.Children.Add(line);
            }
        }

        private void InitializeTimer()
        {
            gameLoopTimer = new DispatcherTimer();
            gameLoopTimer.Interval = TimeSpan.FromSeconds(0.2);
            gameLoopTimer.Tick += new EventHandler(MainGameLoop); 
            gameLoopTimer.Start();
        }

        private void MakeGameFaster()
        {
            if (gameLoopTimer.Interval.TotalSeconds>0.05)
            gameLoopTimer.Interval = gameLoopTimer.Interval - TimeSpan.FromSeconds(0.05);
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
            if (apple == null)
                return;
           
                if (!GameWorld.Children.Contains(apple.UIElement))
                    GameWorld.Children.Add(apple.UIElement);

                Canvas.SetLeft(apple.UIElement, apple.X);
                Canvas.SetTop(apple.UIElement, apple.Y);

            
        }

        private void CreateApple()
        {
            if (apple != null)
                return;

            apple = new Apple(elementSize)
                {
                    X= RandomNumber.Next(0, numberOfColumns) *elementSize,
                 Y= RandomNumber.Next(0, numberOfRows) * elementSize
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
            if (apple == null)
                return;
            SnakeElement head = snakeElements[0];
            
            if(head.X== apple.X && head.Y== apple.Y)
            {
                GameWorld.Children.Remove(apple.UIElement);
                GrowSnake();
                apple = null;
                MakeGameFaster();
            }                  
        }

        private void GrowSnake()
        {
            snakeElements.Add(new SnakeElement(elementSize) {X=tailBackup.X, Y= tailBackup.Y });
        }

        private void CheckColisionWitchSelf()
        {
            SnakeElement snakeHead = GetSnakeHead();
            bool hasCollision = false;
            if (snakeHead !=null)
            {
                foreach (var snakeElement in snakeElements)
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
            foreach (var snakeElement in snakeElements)
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
            if (snakeHead.X > gameWith - elementSize ||
                snakeHead.X < 0 ||
                snakeHead.Y < 0 ||
                snakeHead.Y > gameHeight - elementSize)
            {
                MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry. Chcesz zagrać jeszcze raz?");
                ResetGame();
                InitializeGame();
            }
        }

        private void MoveSnake()
        {
            SnakeElement head = snakeElements[0];
            SnakeElement tail = snakeElements[snakeElements.Count - 1];
            tailBackup = new SnakeElement(elementSize)
            {
                X = tail.X,
                Y = tail.Y
            };
            head.IsHead = false;
            tail.IsHead = true;
            tail.X = head.X;
            tail.Y = head.Y;

            switch (currentDirection)
            {
                case Direction.Right:
                    tail.X += elementSize;
                    break;
                case Direction.Left:
                    tail.X -= elementSize;
                    break;
                case Direction.Up:
                    tail.Y -= elementSize;
                    break;
                case Direction.Down:
                    tail.Y += elementSize;
                    break;
                default:
                    break;
            }

            snakeElements.RemoveAt(snakeElements.Count - 1);
            snakeElements.Insert(0, tail);

        
        }
        private void KeyWasRelased(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    if (currentDirection != Direction.Down)
                    currentDirection = Direction.Up;
                    break;
                case Key.A:
                    if (currentDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case Key.S:
                    if (currentDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
                case Key.D:
                    if (currentDirection != Direction.Left)
                        currentDirection = Direction.Right;
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