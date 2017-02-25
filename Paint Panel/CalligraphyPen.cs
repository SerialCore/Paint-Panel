using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Paint_Panel
{
    class CalligraphyPen : InkToolbarCustomPen
    {
        protected override InkDrawingAttributes CreateInkDrawingAttributesCore(Brush brush, double strokeWidth)
        {
            InkDrawingAttributes inkDrawingAttributes = new InkDrawingAttributes();
            inkDrawingAttributes.PenTip = PenTipShape.Rectangle;
            SolidColorBrush solidColorBrush = brush as SolidColorBrush;
            inkDrawingAttributes.Color = solidColorBrush?.Color ?? Colors.Black;
            inkDrawingAttributes.Size = new Windows.Foundation.Size(strokeWidth * 2, strokeWidth * 2);

            Matrix3x2 matrix = Matrix3x2.CreateSkew((float)Math.PI / 4, 0);
            inkDrawingAttributes.PenTipTransform = matrix;

            return inkDrawingAttributes;
        }

    }
}
