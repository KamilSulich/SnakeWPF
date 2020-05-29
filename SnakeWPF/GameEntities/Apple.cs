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
    }
}
