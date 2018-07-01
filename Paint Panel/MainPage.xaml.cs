﻿using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using Paint_Panel.Control;
using Paint_Panel.Pens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Paint_Panel
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<MyColors> myColors { get; set; }

        public ObservableCollection<PensCollection> pensCollection { get; set; }

        public Stack<InkStroke> UndoStrokes { get; set; }

        StorageFolder folder = ApplicationData.Current.LocalFolder;

        public MainPage()
        {
            // 页面的初始化
            this.InitializeComponent();
            // 窗口颜色初始化
            if(ApplicationData.Current.LocalSettings.Values.ContainsKey("colorA"))
            {
                Color color = Color.FromArgb(Convert.ToByte(ApplicationData.Current.LocalSettings.Values["colorA"]),
                    Convert.ToByte(ApplicationData.Current.LocalSettings.Values["colorR"]),
                    Convert.ToByte(ApplicationData.Current.LocalSettings.Values["colorG"]),
                    Convert.ToByte(ApplicationData.Current.LocalSettings.Values["colorB"]));
                GlassColor.Background = new SolidColorBrush(color);
            }
            // 整个绘画面板尺寸的初始化，因为用了ViewBox，InkCanvas会自适应Image的尺寸
            Initialize_Panel();
            // ink的初始化
            this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            inkToolbar.Loaded += InkToolbar_Loaded;
            // ListView必需的代码
            myColors = PanelColors.PaneColors;
            pensCollection = new ObservableCollection<PensCollection>()
            {
                new PensCollection { Pen = new UsualPen(), PenName = "Usual Pen" },
                new PensCollection { Pen = new MarkerPen(), PenName = "Marker Pen" },
                new PensCollection { Pen = new CalligraphyPen(), PenName = "Calligraphy Pen" },
                new PensCollection { Pen = new PencilBrush(), PenName = "Pencil Brush" },
                new PensCollection { Pen = new InkBrush(), PenName = "Ink Brush" }
            };
            this.DataContext = this;
            // 墨迹堆栈
            UndoStrokes = new Stack<InkStroke>();
        }

        // 全局变量
        private IRandomAccessStream x;   //图片加载和图片编辑用到的随机读取流
        private Color currentPanel = Colors.White;    //当前选定的背景颜色
        private Color currentWindow = Colors.Azure;    //当前选定的背景颜色
        private PrintHelper printHelper;

        #region 用户操作

        private void inputDevice_Click(object sender, RoutedEventArgs e)
        {
            if (inputDevice.IsChecked == true)
            {
                this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            }
            else
            {
                this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            }
        }

        private void pens_colors_Click(object sender, RoutedEventArgs e)
        {
            set_panel.IsPaneOpen = true;
            color_picker.Visibility = Visibility.Collapsed;
            pens_list.Visibility = Visibility.Visible;
        }

        private async void save_composite(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            picker.FileTypeChoices.Add("Image files", new string[] { ".png" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FilesOperator.generateImage(file, inkCanvas, currentPanel, back_image, x);
            }
        }

        private async void save_nocolorInk(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            picker.FileTypeChoices.Add("Image files", new string[] { ".png" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FilesOperator.generateImage(file, inkCanvas);
            }
        }

        private void share_img(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private async void save_ink(object sender, RoutedEventArgs e)
        {
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            try
            {
                await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
            }
            catch
            {
                return;
            }
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".ink";
            picker.FileTypeChoices.Add("Ink files", new string[] { ".ink" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                var bt = await ConvertImagetoByte(stream);
                await FileIO.WriteBytesAsync(file, bt);
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // 加载图片要用到的延迟代码
            DataRequestDeferral deferral = args.Request.GetDeferral();

            // 共享请求的信息
            DataRequest request = args.Request;
            request.Data.Properties.Title = "手绘";
            request.Data.Properties.Description = "Share your painting";

            // 图片生成
            StorageFile file = await folder.CreateFileAsync(DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", CreationCollisionOption.ReplaceExisting);
            if (x == null)
            {
                await FilesOperator.generateImage(file, inkCanvas, currentPanel);
            }
            else
            {
                await FilesOperator.generateImage(file, inkCanvas, currentPanel, back_image, x);
            }

            // 将图片打包
            RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(file);
            request.Data.Properties.Thumbnail = imageStreamRef;
            request.Data.SetBitmap(imageStreamRef);

            // 延迟结束
            deferral.Complete();
        }

        private async void pick_img(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            StorageFile image_file = await picker.PickSingleFileAsync();
            if (image_file != null)
            {
                BitmapImage bi3 = new BitmapImage();
                x = await image_file.OpenAsync(FileAccessMode.Read);
                bi3.SetSource(x);
                back_image.Source = bi3;
            }
        }

        private async void open_ink(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".ink");
            var pickedFile = await picker.PickSingleFileAsync();
            if (pickedFile != null)
            {
                var file = await pickedFile.OpenReadAsync();
                await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(file);
            }
        }

        private void ink_undo(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (strokes.Count > 0)
            {
                strokes[strokes.Count - 1].Selected = true;
                UndoStrokes.Push(strokes[strokes.Count - 1]); // 入栈
                inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            }
        }

        private void ink_redo(object sender, RoutedEventArgs e)
        {
            if (UndoStrokes.Count > 0)
            {
                var stroke = UndoStrokes.Pop();

                // This will blow up sky high:
                // InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);

                var strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(stroke.DrawingAttributes);
                System.Numerics.Matrix3x2 matr = stroke.PointTransform;
                IReadOnlyList<InkPoint> inkPoints = stroke.GetInkPoints();
                InkStroke stk = strokeBuilder.CreateStrokeFromInkPoints(inkPoints, matr);
                inkCanvas.InkPresenter.StrokeContainer.AddStroke(stk);
            }
        }

        private void color_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MyColors;
            panel_color.Fill = item.IndexColorBrush;
            currentPanel = item.IndexColor;
        }

        private void pen_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as PensCollection;
            customPen.CustomPen = item.Pen;
            color_picker.Visibility = Visibility.Visible;
            pens_list.Visibility = Visibility.Collapsed;
            set_panel.IsPaneOpen = false;
        }

        private async void new_size(object sender, RoutedEventArgs e)
        {
            float hight, width;
            try
            {
                hight = (float)Convert.ToDouble(size_hight.Text);
                width = (float)Convert.ToDouble(size_width.Text);
            }
            catch { return; }

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            using (var renderTarget = new CanvasRenderTarget(device, width, hight, 240))
            {
                await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
            }
            if (x != null)
            {
                x.Dispose();
                x = null;
            }
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            back_image.Source = bitmap;
        }

        private void open_functions(object sender, RoutedEventArgs e)
        {
            set_panel.IsPaneOpen = !set_panel.IsPaneOpen;
        }

        private async void print_image(object sender, RoutedEventArgs e)
        {
            StorageFile printFile = await folder.CreateFileAsync(DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", CreationCollisionOption.ReplaceExisting);
            if (x != null)
                await FilesOperator.generateImage(printFile, inkCanvas, currentPanel, back_image, x);
            else
                await FilesOperator.generateImage(printFile, inkCanvas, currentPanel);

            var stream = await printFile.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);
            printImage.Source = bitmapImage;

            if (DirectPrintContainer.Children.Contains(PrintableContent))
            {
                DirectPrintContainer.Children.Remove(PrintableContent);
            }
            printHelper = new PrintHelper(Container);
            printHelper.AddFrameworkElementToPrint(PrintableContent);

            printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            await printHelper.ShowPrintUIAsync("Paint Panel Page");
        }

        #endregion

        #region 方法调用

        private void InitializeFrostedGlass(UIElement glassHost)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;
            var backdropBrush = compositor.CreateHostBackdropBrush();
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = backdropBrush;
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);
            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

        private void clear_img(object sender, RoutedEventArgs e)
        {
            Initialize_Panel();
            if (x != null)
            {
                x.Dispose();
                x = null;
            }
        }

        private async void Initialize_Panel()
        {
            StorageFile image_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Background/PC_Background.png"));
            BitmapImage image = new BitmapImage();
            x = await image_file.OpenAsync(FileAccessMode.Read);
            image.SetSource(x);
            back_image.Source = image;
        }

        private async Task<byte[]> ConvertImagetoByte(IRandomAccessStream fileStream)
        {
            var reader = new DataReader(fileStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)fileStream.Size);
            byte[] pixels = new byte[fileStream.Size];
            reader.ReadBytes(pixels);
            return pixels;
        }

        private BitmapImage getBitmapImage(IRandomAccessStream indexStream)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.SetSource(indexStream);
            return bi3;
        }

        #endregion

        #region 事件驱动

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            InitializeFrostedGlass(GlassHost);

            var indexFile = e.Parameter as StorageFile;
            if (indexFile != null)
            {
                if(indexFile.FileType.Equals(".ink"))
                {
                    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = indexFile.Name;

                    var file = await indexFile.OpenReadAsync();
                    await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(file);
                    return;
                }
                if(indexFile.FileType.Equals(".png") || indexFile.FileType.Equals(".jpg"))
                {
                    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = indexFile.Name;

                    BitmapImage image = new BitmapImage();
                    x = await indexFile.OpenAsync(FileAccessMode.Read);
                    image.SetSource(x);
                    back_image.Source = image;
                }
            }
        }

        private void InkToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            inkToolbar.ActiveTool = inkToolbar.GetToolButton(InkToolbarTool.BallpointPen);
            customPen.CustomPen = new UsualPen();
            customPen.Palette = PanelColors.ToolColors;
        }

        private void flyoutBase_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void inkCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            size_hight.Text = (inkCanvas.ActualHeight / 2.5).ToString();
            size_width.Text = (inkCanvas.ActualWidth / 2.5).ToString();
        }

        private void colorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if((bool)(color_pen.IsChecked))
            {
                InkDrawingAttributes drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                drawingAttributes.Color = args.NewColor;
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
                inkToolbar.InkDrawingAttributes.Color = args.NewColor;
            }
            else if ((bool)(color_panel.IsChecked))
            {
                panel_color.Fill = new SolidColorBrush(args.NewColor);
                currentPanel = args.NewColor;
            }
            else
            {
                GlassColor.Background = new SolidColorBrush(args.NewColor);
                currentWindow = args.NewColor;
            }
        }

        private void color_window_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["colorA"] = Convert.ToInt32(currentWindow.A);
            ApplicationData.Current.LocalSettings.Values["colorR"] = Convert.ToInt32(currentWindow.R);
            ApplicationData.Current.LocalSettings.Values["colorG"] = Convert.ToInt32(currentWindow.B);
            ApplicationData.Current.LocalSettings.Values["colorB"] = Convert.ToInt32(currentWindow.G);
        }

        private async void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
            await new MessageDialog("Printing done.").ShowAsync();
        }

        private async void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();
            await new MessageDialog("Printing failed.").ShowAsync();
        }

        private void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }

        private void ReleasePrintHelper()
        {
            printHelper.Dispose();

            //While code could be used to re-add the printable content, it's not done here
            //as it wasn't intended to be displayed.
            //if (!DirectPrintContainer.Children.Contains(PrintableContent))
            //{
            //    DirectPrintContainer.Children.Add(PrintableContent);
            //}
        }

        #endregion

    }

    public class PensCollection
    {
        public InkToolbarCustomPen Pen { get; set; }

        public string PenName { get; set; }
    }
}