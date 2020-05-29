using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeWPF.GameEntities
{
    class Apple :GameEntity
    {
        public Apple(int size)
        {
            Rectangle rect = new Rectangle();
            rect.Width = size;
            rect.Height = size;
            rect.Fill = Brushes.Red;
            UIElement = rect;
        }

        public override bool Equals(object obj)
        {
            Apple apple = obj as Apple;
            if (apple != null)
            {
                return X == apple.X && Y == apple.Y;
            }
            else
            {
                return false;
            }
        }
    }
}
