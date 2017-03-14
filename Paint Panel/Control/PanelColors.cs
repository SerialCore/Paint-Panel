using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Paint_Panel.Control
{
    class PanelColors
    {
        public static Color Black = Color.FromArgb(255, 0, 0, 0);
        public static Color White = Color.FromArgb(255, 255, 255, 255);
        public static Color SilveryWhite = Color.FromArgb(255, 209, 210, 211);
        public static Color Gray = Color.FromArgb(255, 167, 169, 172);
        public static Color DeepGray = Color.FromArgb(255, 128, 130, 133);
        public static Color DeeperGray = Color.FromArgb(255, 88, 89, 91);

        public static Color Magenta = Color.FromArgb(255, 179, 21, 100);
        public static Color Red = Color.FromArgb(255, 230, 27, 27);
        public static Color RedOrange = Color.FromArgb(255, 255, 85, 0);
        public static Color Orange = Color.FromArgb(255, 255, 170, 0);
        public static Color Gold = Color.FromArgb(255, 255, 206, 0);
        public static Color Yellow = Color.FromArgb(255, 255, 230, 0);

        public static Color GrassGreen = Color.FromArgb(255, 162, 230, 27);
        public static Color Green = Color.FromArgb(255, 38, 230, 0);
        public static Color DeepGreen = Color.FromArgb(255, 0, 128, 85);
        public static Color Cyan = Color.FromArgb(255, 0, 170, 204);
        public static Color Blue = Color.FromArgb(255, 0, 76, 230);
        public static Color IndigoBlue = Color.FromArgb(255, 61, 0, 184);

        public static Color Violet = Color.FromArgb(255, 102, 0, 204);
        public static Color Purple = Color.FromArgb(255, 96, 0, 128);
        public static Color Beige = Color.FromArgb(255, 252, 230, 201);//不对
        public static Color LightBrown = Color.FromArgb(255, 187, 145, 103);
        public static Color Brown = Color.FromArgb(255, 142, 86, 46);
        public static Color DeepBrown = Color.FromArgb(255, 97, 61, 48);

        public static Color LightPink = Color.FromArgb(255, 255, 128, 255);
        public static Color PinkOrange = Color.FromArgb(255, 255, 198, 128);
        public static Color PinkYellow = Color.FromArgb(255, 255, 255, 128);
        public static Color PinkGreen = Color.FromArgb(255, 128, 255, 158);
        public static Color PinkBlue = Color.FromArgb(255, 128, 214, 255);
        public static Color PinkPurple = Color.FromArgb(255, 188, 179, 255);

        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// </summary>

        public static SolidColorBrush BlackBrush = new SolidColorBrush(Black);
        public static SolidColorBrush WhiteBrush = new SolidColorBrush(White);
        public static SolidColorBrush SilveryWhiteBrush = new SolidColorBrush(SilveryWhite);
        public static SolidColorBrush GrayBrush = new SolidColorBrush(Gray);
        public static SolidColorBrush DeepGrayBrush = new SolidColorBrush(DeepGray);
        public static SolidColorBrush DeeperGrayBrush = new SolidColorBrush(DeeperGray);

        public static SolidColorBrush MagentaBrush = new SolidColorBrush(Magenta);
        public static SolidColorBrush RedBrush = new SolidColorBrush(Red);
        public static SolidColorBrush RedOrangeBrush = new SolidColorBrush(RedOrange);
        public static SolidColorBrush OrangeBrush = new SolidColorBrush(Orange);
        public static SolidColorBrush GoldBrush = new SolidColorBrush(Gold);
        public static SolidColorBrush YellowBrush = new SolidColorBrush(Yellow);

        public static SolidColorBrush GrassGreenBrush = new SolidColorBrush(GrassGreen);
        public static SolidColorBrush GreenBrush = new SolidColorBrush(Green);
        public static SolidColorBrush DeepGreenBrush = new SolidColorBrush(DeepGreen);
        public static SolidColorBrush CyanBrush = new SolidColorBrush(Cyan);
        public static SolidColorBrush BlueBrush = new SolidColorBrush(Blue);
        public static SolidColorBrush IndigoBlueBrush = new SolidColorBrush(IndigoBlue);

        public static SolidColorBrush VioletBrush = new SolidColorBrush(Violet);
        public static SolidColorBrush PurpleBrush = new SolidColorBrush(Purple);
        public static SolidColorBrush BeigeBrush = new SolidColorBrush(Beige);
        public static SolidColorBrush LightBrownBrush = new SolidColorBrush(LightBrown);
        public static SolidColorBrush BrownBrush = new SolidColorBrush(Brown);
        public static SolidColorBrush DeepBrownBrush = new SolidColorBrush(DeepBrown);

        public static SolidColorBrush LightPinkBrush = new SolidColorBrush(LightPink);
        public static SolidColorBrush PinkOrangeBrush = new SolidColorBrush(PinkOrange);
        public static SolidColorBrush PinkYellowBrush = new SolidColorBrush(PinkYellow);
        public static SolidColorBrush PinkGreenBrush = new SolidColorBrush(PinkGreen);
        public static SolidColorBrush PinkBlueBrush = new SolidColorBrush(PinkBlue);
        public static SolidColorBrush PinkPurpleBrush = new SolidColorBrush(PinkPurple);

        /// <summary>
        /// Collections
        /// </summary>

        public static BrushCollection ToolColors = new BrushCollection()
        {
            WhiteBrush, BlackBrush, SilveryWhiteBrush, GrayBrush, DeepGrayBrush, DeeperGrayBrush,

            MagentaBrush, RedBrush, RedOrangeBrush, OrangeBrush, GoldBrush, YellowBrush,

            GrassGreenBrush, GreenBrush, DeepGreenBrush, CyanBrush, BlueBrush, IndigoBlueBrush,

            VioletBrush, PurpleBrush, BeigeBrush, LightBrownBrush, BrownBrush, DeepBrownBrush,

            LightPinkBrush, PinkOrangeBrush, PinkYellowBrush, PinkGreenBrush, PinkBlueBrush, PinkPurpleBrush
        };

        public static ObservableCollection<MyColors> PaneColors = new ObservableCollection<MyColors>
        {
            new MyColors { IndexColor = White, IndexColorBrush = WhiteBrush, ColorName = "White" },
            new MyColors { IndexColor = Black, IndexColorBrush = BlackBrush, ColorName = "Black" },
            new MyColors { IndexColor = SilveryWhite, IndexColorBrush = SilveryWhiteBrush, ColorName = "SilveryWhite" },
            new MyColors { IndexColor = Gray, IndexColorBrush = GrayBrush, ColorName = "Gray" },
            new MyColors { IndexColor = DeepGray, IndexColorBrush = DeepGrayBrush, ColorName = "DeepGray" },
            new MyColors { IndexColor = DeeperGray, IndexColorBrush = DeeperGrayBrush, ColorName = "DeeperGray" },

            new MyColors { IndexColor = Magenta, IndexColorBrush = MagentaBrush, ColorName = "Magenta" },
            new MyColors { IndexColor = Red, IndexColorBrush = RedBrush, ColorName = "Red" },
            new MyColors { IndexColor = RedOrange, IndexColorBrush = RedOrangeBrush, ColorName = "RedOrange" },
            new MyColors { IndexColor = Orange, IndexColorBrush = OrangeBrush, ColorName = "Orange" },
            new MyColors { IndexColor = Gold, IndexColorBrush = GoldBrush, ColorName = "Gold" },
            new MyColors { IndexColor = Yellow, IndexColorBrush = YellowBrush, ColorName = "Yellow" },

            new MyColors { IndexColor = GrassGreen, IndexColorBrush = GrassGreenBrush, ColorName = "GrassGreen" },
            new MyColors { IndexColor = Green, IndexColorBrush = GreenBrush, ColorName = "Green" },
            new MyColors { IndexColor = DeepGreen, IndexColorBrush = DeepGreenBrush, ColorName = "DeepGreen" },
            new MyColors { IndexColor = Cyan, IndexColorBrush = CyanBrush, ColorName = "Cyan" },
            new MyColors { IndexColor = Blue, IndexColorBrush = BlueBrush, ColorName = "Blue" },
            new MyColors { IndexColor = IndigoBlue, IndexColorBrush = IndigoBlueBrush, ColorName = "IndigoBlue" },

            new MyColors { IndexColor = Violet, IndexColorBrush = VioletBrush, ColorName = "Violet" },
            new MyColors { IndexColor = Purple, IndexColorBrush = PurpleBrush, ColorName = "Purple" },
            new MyColors { IndexColor = Beige, IndexColorBrush = BeigeBrush, ColorName = "Beige" },
            new MyColors { IndexColor = LightBrown, IndexColorBrush = LightBrownBrush, ColorName = "LightBrown" },
            new MyColors { IndexColor = Brown, IndexColorBrush = BrownBrush, ColorName = "Brown" },
            new MyColors { IndexColor = DeepBrown, IndexColorBrush = DeepBrownBrush, ColorName = "DeepBrown" },

            new MyColors { IndexColor = LightPink, IndexColorBrush = LightPinkBrush, ColorName = "LightPink" },
            new MyColors { IndexColor = PinkOrange, IndexColorBrush = PinkOrangeBrush, ColorName = "PinkOrange" },
            new MyColors { IndexColor = PinkYellow, IndexColorBrush = PinkYellowBrush, ColorName = "PinkYellow" },
            new MyColors { IndexColor = PinkGreen, IndexColorBrush = PinkGreenBrush, ColorName = "PinkGreen" },
            new MyColors { IndexColor = PinkBlue, IndexColorBrush = PinkBlueBrush, ColorName = "PinkBlue" },
            new MyColors { IndexColor = PinkPurple, IndexColorBrush = PinkPurpleBrush, ColorName = "PinkPurple" }
        };

    }

    public class MyColors
    {
        public Color IndexColor { get; set; }

        public SolidColorBrush IndexColorBrush { get; set; }

        public String ColorName { get; set; }
    }
}
