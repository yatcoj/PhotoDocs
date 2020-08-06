using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace PhotoDocs
{
    public interface IDrawTools
    {
        void OnDrag(Point p);

        void OnMouseUp(Point p);

        void OnMouseDown(Point p);

        void SetThickness(int thickness);

        void SetColor(uint col);
    }
}
