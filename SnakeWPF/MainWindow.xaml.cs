﻿using SnakeWPF.GameEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        int points = 0;
        int pictureNumer = 0;
        int oldPictureNumer = 0;

        int PictureTimer = 0;

        Apple apple;
        Random RandomNumber;
        SnakeElement tailBackup;
        Direction currentDirection;
        DispatcherTimer gameLoopTimer;
        List<SnakeElement> snakeElements;

        List<string> listUrl;


        public MainWindow()
        {
            InitializeComponent();     
        }

        protected override void OnContentRendered(EventArgs e)
        {
            MessageBox.Show("Sterowanie klawiszami A,W,S,D lub strzałkami. Wciśnij przycisk OK, aby kontynuować.");
            listUrl = new List<string>();
            listUrl.Add("https://cdn.pixabay.com/photo/2015/02/28/15/25/snake-653639_960_720.jpg");
            listUrl.Add("https://cdn.pixabay.com/photo/2011/05/14/20/48/basilisk-rattlesnake-7303_960_720.jpg");
            listUrl.Add("https://cdn.pixabay.com/photo/2014/11/23/21/22/green-tree-python-543243_960_720.jpg");
            listUrl.Add("https://cdn.pixabay.com/photo/2019/02/06/17/09/snake-3979601_960_720.jpg");
            listUrl.Add("https://cdn.pixabay.com/photo/2014/11/21/15/33/snake-540656_960_720.jpg");

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
            points = 0;
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
            if (SnakePicture.Source==null)
            {
                MoveSnake();
                CheckColision();
                DrawSnake();
                CreateApple();
                DrawApples();
            }
           
            HideSnakePicture();
            

        }

        private void HideSnakePicture()
        {
            if (SnakePicture.Source != null)
                PictureTimer++;
            if (PictureTimer > 3)
            {
                SnakePicture.Source = null;
                PictureTimer = 0;
            }
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
                pictureNumer = RandomNumber.Next(0, 4);
                while (pictureNumer == oldPictureNumer)
                {
                    pictureNumer = RandomNumber.Next(0, 4);
                }
                oldPictureNumer = pictureNumer;

                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(listUrl[pictureNumer], UriKind.Absolute);
                src.EndInit();
                SnakePicture.Source = src;

                GameWorld.Children.Remove(apple.UIElement);
                GrowSnake();
                apple = null;
                MakeGameFaster();
                points++;
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
                if (points > 1 && points< 5)
                    MessageBox.Show("Wąż uderzył głową w samego siebie, koniec gry. Zdobyto " + points + " punkty. Kliknij OK, by zagrać ponownie.");
                else 
                    MessageBox.Show("Wąż uderzył głową w samego siebie, koniec gry. Zdobyto "+ points+ " punktów. Kliknij OK, by zagrać ponownie.");

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
                if (points == 0)
                    MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry. Tym razem nie zdobyto żadnych punktów. Kliknij OK, by zagrać ponownie.");
                else if (points == 1)
                    MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry. Zdobyto " + points + " punkt. Kliknij OK, by zagrać ponownie.");
                else if (points > 1 && points < 5)
                    MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry. Zdobyto " + points + " punkty. Kliknij OK, by zagrać ponownie.");
                else
                    MessageBox.Show("Wąż uderzył głową w ścianę, koniec gry. Zdobyto " + points + " punktów. Kliknij OK, by zagrać ponownie.");

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
                case Key.Up:
                    if (currentDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;
                case Key.Left:
                    if (currentDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case Key.Down:
                    if (currentDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
                case Key.Right:
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