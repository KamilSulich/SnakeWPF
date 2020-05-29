using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeWPF.GameEntities
{
    class GameEntity
    {
        public GameEntity(int size)
        {
            Rectangle rect = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = Brushes.Green
            };
            UIElement = rect;
        }
        public UIElement UIElement { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
