using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Paint_Panel
{
    class MarkerPen : InkToolbarCustomPen
    {
        protected override InkDrawingAttributes CreateInkDrawingAttributesCore(Brush brush, double strokeWidth)
        {
            InkDrawingAttributes inkDrawingAttributes = new InkDrawingAttributes();
            inkDrawingAttributes.PenTip = PenTipShape.Circle;
            SolidColorBrush solidColorBrush = brush as SolidColorBrush;
            inkDrawingAttributes.Color = solidColorBrush?.Color ?? Colors.Black;
            inkDrawingAttributes.DrawAsHighlighter = true;
            inkDrawingAttributes.Size = new Windows.Foundation.Size(strokeWidth * 2, strokeWidth * 2);
            return inkDrawingAttributes;
        }
    }
}
