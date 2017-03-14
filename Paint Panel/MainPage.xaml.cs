using Microsoft.Graphics.Canvas;
using Paint_Panel.Control;
using Paint_Panel.Pens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

        public ObservableCollection<ImageCollection> imageCollection { get; set; }

        StorageFolder folder = ApplicationData.Current.LocalFolder;

        public MainPage()
        {
            // 页面的初始化
            this.InitializeComponent();
            // 整个绘画面板尺寸的初始化，因为用了ViewBox，InkCanvas会自适应Image的尺寸
            restore_image();
            // ink的初始化
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Pen;
                inputDevice_icon_mobile.Visibility = Visibility.Collapsed;
                inputDevice.Click += inputDevice_Click;
            }
            else
            {
                this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Touch;
                inputDevice_icon.Visibility = Visibility.Collapsed;
                inputDevice.Click += inputDevice_Click_Mobile;
            }
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
            imageCollection = new ObservableCollection<ImageCollection>();
            refreshList();
            this.DataContext = this;
        }

        // 公用的变量
        private IRandomAccessStream x;   //图片加载和图片编辑用到的随机读取流
        private FilesOperator operate = new FilesOperator();  //文件操作的方法集合
        private Color currentColor = PanelColors.White;    //当前选定的背景颜色
        private ImageCollection currentImageItem = null;    //当前选定的背景集合项目

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var file = e.Parameter as IRandomAccessStreamWithContentType;
            if (file != null)
            {
                await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(file);
                return;
            }
        }

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

        private void inputDevice_Click_Mobile(object sender, RoutedEventArgs e)
        {
            if (inputDevice.IsChecked == true)
            {
                this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            }
            else
            {
                this.inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Touch;
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

        private async void save_composite(object sender, RoutedEventArgs e)
        {
            if (x == null)
            {
                ToastNotification notification = new ToastNotification(ToastCollection.NoneInsertedImage.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
                return;
            }
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            picker.FileTypeChoices.Add("Image files", new string[] { ".png" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                operate.generateImage(file, inkCanvas, back_image, currentColor, x);
                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        private async void save_colorInk(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            picker.FileTypeChoices.Add("Image files", new string[] { ".png" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                operate.generateImage(file, inkCanvas, currentColor);
                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        private async void save_nocolorInk(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            picker.FileTypeChoices.Add("Image files", new string[] { ".png" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                operate.generateImage(file, inkCanvas);
                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        private void share_img(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // 加载图片要用到的延迟代码
            DataRequestDeferral deferral = args.Request.GetDeferral();

            // 共享请求的信息
            DataRequest request = args.Request;
            request.Data.Properties.Title = "手绘";
            request.Data.Properties.Description = "向朋友共享你的涂鸦";

            // 图片生成
            StorageFolder storageFolder = ApplicationData.Current.RoamingFolder;
            StorageFile file = await storageFolder.CreateFileAsync("paint.png", CreationCollisionOption.ReplaceExisting);
            if (x == null)
            {
                operate.generateImage(file, inkCanvas, currentColor);
            }
            else
            {
                operate.generateImage(file, inkCanvas, back_image, currentColor, x);
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

        private void clear_img(object sender, RoutedEventArgs e)
        {
            restore_image();
            if (x != null)
            {
                x.Dispose();
                x = null;
            }
        }

        private async void restore_image()
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                StorageFile image_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Background/PC_Background.png"));
                BitmapImage image = new BitmapImage();
                x = await image_file.OpenAsync(FileAccessMode.Read);
                image.SetSource(x);
                back_image.Source = image;
            }
            else
            {
                StorageFile image_file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Background/Mobile_Background.png"));
                BitmapImage image = new BitmapImage();
                x = await image_file.OpenAsync(FileAccessMode.Read);
                image.SetSource(x);
                back_image.Source = image;
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
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".ink";
            picker.FileTypeChoices.Add("Ink files", new string[] { ".ink" });
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                var bt = await ConvertImagetoByte(stream);
                await FileIO.WriteBytesAsync(file, bt);
                await CachedFileManager.CompleteUpdatesAsync(file);

                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        private async Task<byte[]> ConvertImagetoByte(IRandomAccessStream fileStream)
        {
            var reader = new DataReader(fileStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)fileStream.Size);
            byte[] pixels = new byte[fileStream.Size];
            reader.ReadBytes(pixels);
            return pixels;
        }

        private void ink_undo(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            if (strokes.Count > 0)
            {
                strokes[strokes.Count - 1].Selected = true;
                inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            }
        }

        private void choose_color(object sender, RoutedEventArgs e)
        {
            panel_colors.IsPaneOpen = !panel_colors.IsPaneOpen;
        }

        private void color_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MyColors;
            panel_color.Fill = item.IndexColorBrush;
            currentColor = item.IndexColor;
        }

        private void flyoutBase_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void pen_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as PensCollection;
            switch (item.PenName)
            {
                case "Usual Pen":
                    customPen.CustomPen = item.Pen;
                    break;
                case "Marker Pen":
                    customPen.CustomPen = item.Pen;
                    break;
                case "Calligraphy Pen":
                    customPen.CustomPen = item.Pen;
                    break;
                case "Pencil Brush":
                    customPen.CustomPen = item.Pen;
                    break;
                case "Ink Brush":
                    customPen.CustomPen = item.Pen;
                    break;
            }
        }

        private void open_Pens(object sender, RoutedEventArgs e)
        {
            panel_pens.IsPaneOpen = !panel_pens.IsPaneOpen;
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
            BitmapImage bi3 = new BitmapImage();
            bi3.SetSource(stream);
            back_image.Source = bi3;
        }

        private void show_sizePanel(object sender, RoutedEventArgs e)
        {
            if (sizePanel_control.IsChecked == true)
                sizePanel.Visibility = Visibility.Collapsed;
            else
                sizePanel.Visibility = Visibility.Visible;
        }

        private async void refreshList()
        {
            IReadOnlyList<StorageFile> storageFiles = await folder.GetFilesAsync();
            if (imageCollection != null)
                imageCollection.Clear();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                foreach (StorageFile item in storageFiles)
                {
                    stream = await item.OpenAsync(FileAccessMode.Read);
                    imageCollection.Add(new ImageCollection { FileName = item.Name, ImageStream = stream, ImageFile = getBitmapImage(stream) });
                    await Task.Delay(100);
                }
            });
        }

        private BitmapImage getBitmapImage(IRandomAccessStream indexStream)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.SetSource(indexStream);
            return bi3;
        }

        private async void collection_addnew(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            IReadOnlyList<StorageFile> image_file = await picker.PickMultipleFilesAsync();
            if (image_file != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    IRandomAccessStream stream = new InMemoryRandomAccessStream();
                    foreach (StorageFile item in image_file)
                    {
                        var file = await item.CopyAsync(folder);
                        await file.RenameAsync(DateTime.Now.GetHashCode().ToString() + ".png");
                        stream = await file.OpenAsync(FileAccessMode.Read);

                        imageCollection.Add(new ImageCollection { FileName = file.Name, ImageStream = stream, ImageFile = getBitmapImage(stream) });
                        stream.Dispose();
                    }
                });
            }
        }

        private void collection_choose(object sender, RoutedEventArgs e)
        {
            if (currentImageItem != null)
            {
                x = currentImageItem.ImageStream;
                back_image.Source = currentImageItem.ImageFile;
            }
        }

        private async void collection_delete(object sender, RoutedEventArgs e)
        {
            if (currentImageItem != null && imageCollection.IndexOf(currentImageItem) >= 0)
            {
                var file_delete = await folder.GetFileAsync(currentImageItem.FileName);
                await file_delete.DeleteAsync();
                imageCollection.Remove(currentImageItem);
            }
        }

        private void image_ItemClick(object sender, ItemClickEventArgs e)
        {
            currentImageItem = e.ClickedItem as ImageCollection;
        }

        private async void collection_composite(object sender, RoutedEventArgs e)
        {
            if (x == null)
            {
                ToastNotification notification = new ToastNotification(ToastCollection.NoneInsertedImage.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
                return;
            }
            StorageFile file = await folder.CreateFileAsync(DateTime.Now.GetHashCode().ToString() + ".png");
            if (file != null)
            {
                file = await operate.returnImage(file, inkCanvas, back_image, currentColor, x);
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                stream = await file.OpenAsync(FileAccessMode.Read);
                imageCollection.Add(new ImageCollection { FileName = file.Name, ImageStream = stream, ImageFile = getBitmapImage(stream) });

                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        private async void collection_colorInk(object sender, RoutedEventArgs e)
        {
            StorageFile file = await folder.CreateFileAsync(DateTime.Now.GetHashCode().ToString() + ".png");
            if (file != null)
            {
                file = await operate.returnImage(file, inkCanvas, currentColor);
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                stream = await file.OpenAsync(FileAccessMode.Read);
                imageCollection.Add(new ImageCollection { FileName = file.Name, ImageStream = stream, ImageFile = getBitmapImage(stream) });

                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

        private async void collection_nocolorInk(object sender, RoutedEventArgs e)
        {
            StorageFile file = await folder.CreateFileAsync(DateTime.Now.GetHashCode().ToString() + ".png");
            if (file != null)
            {
                file = await operate.returnImage(file, inkCanvas);
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                stream = await file.OpenAsync(FileAccessMode.Read);
                imageCollection.Add(new ImageCollection { FileName = file.Name, ImageStream = stream, ImageFile = getBitmapImage(stream) });

                ToastNotification notification = new ToastNotification(ToastCollection.SaveSuccessfull.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
        }

    }

    public class ImageCollection
    {
        public string FileName { get; set; }

        public IRandomAccessStream ImageStream { get; set; }

        public BitmapImage ImageFile { get; set; }

    }

    public class PensCollection
    {
        public InkToolbarCustomPen Pen { get; set; }

        public string PenName { get; set; }
    }
}
