using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Paint_Panel.Pens
{
    class PencilBrush : InkToolbarCustomPen
    {

        protected override InkDrawingAttributes CreateInkDrawingAttributesCore(Brush brush, double strokeWidth)
        {
            InkDrawingAttributes inkDrawingAttributes = InkDrawingAttributes.CreateForPencil();
            SolidColorBrush solidColorBrush = brush as SolidColorBrush;
            inkDrawingAttributes.Color = solidColorBrush?.Color ?? Colors.Black;
            inkDrawingAttributes.Size = new Windows.Foundation.Size(strokeWidth * 8, strokeWidth * 8);
            inkDrawingAttributes.PencilProperties.Opacity = 0.99;

            return inkDrawingAttributes;
        }

    }
}
