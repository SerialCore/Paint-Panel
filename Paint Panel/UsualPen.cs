using System.Numerics;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Paint_Panel
{
    class UsualPen : InkToolbarCustomPen
    {

        protected override InkDrawingAttributes CreateInkDrawingAttributesCore(Brush brush, double strokeWidth)
        {
            InkDrawingAttributes inkDrawingAttributes = new InkDrawingAttributes();
            inkDrawingAttributes.PenTip = PenTipShape.Circle;
            inkDrawingAttributes.Size = new Windows.Foundation.Size(strokeWidth * 1.5, strokeWidth * 2.5);
            SolidColorBrush solidColorBrush = brush as SolidColorBrush;
            inkDrawingAttributes.Color = solidColorBrush?.Color ?? Colors.Black;

            Matrix3x2 matrix = Matrix3x2.CreateRotation(90);
            inkDrawingAttributes.PenTipTransform = matrix;

            return inkDrawingAttributes;
        }

    }
}
