using Microsoft.Toolkit.Uwp.Notifications;

namespace Paint_Panel.Control
{
    class ToastCollection
    {

        public static ToastContent NoneInsertedImage = new ToastContent()
        {
            Launch = "app-defined-string",
            Visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "None inserted image here"
                        },
                        new AdaptiveText()
                        {
                            Text = "Please open a image or choose \"Ink without Background\""
                        }
                    },
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "Assets/StoreLogo.scale-200.png"
                    }
                }
            }
        };

    }
}
