using DirectXWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FontMetricsUwp
{
    public sealed partial class MainPage : Page
    {
        WriteFactory writeFactory;
        WriteFontCollection writeFontCollection;
        WriteFontMetrics curMetrics;

        public MainPage()
        {
            this.InitializeComponent();

            ContentTextBox.Text = "Type quick!";
            txtblk.Text = "Type quick!";
            FontSizeTextBox.Text = "192";
            txtblk.FontSize = 192;

            writeFactory = new WriteFactory();
            writeFontCollection = writeFactory.GetSystemFontCollection();
            int count = writeFontCollection.GetFontFamilyCount();
            string[] fonts = new string[count];

            for (int i = 0; i < count; i++)
            {
                WriteFontFamily writeFontFamily = writeFontCollection.GetFontFamily(i);

                WriteLocalizedStrings writeLocalizedStrings = writeFontFamily.GetFamilyNames();

                int nameCount = writeLocalizedStrings.GetCount();
                int index;

                if (writeLocalizedStrings.FindLocaleName("en-us", out index))
                {
                    fonts[i] = writeLocalizedStrings.GetString(index);
                }
            }

            lstbox.ItemsSource = fonts;

            Loaded += (sender, args) =>
            {
                lstbox.SelectedIndex = 0;
            };
        }

        void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            string fontFamily = (sender as ListBox).SelectedItem as string;

            if (fontFamily == null)
                return;

            txtblk.FontFamily = new FontFamily(fontFamily);

            int index;
            if (writeFontCollection.FindFamilyName(fontFamily, out index))
            {
                WriteFontFamily writeFontFamily = writeFontCollection.GetFontFamily(index);
                WriteFont writeFont = writeFontFamily.GetFirstMatchingFont(FontWeights.Normal,
                                                                           FontStretch.Normal,
                                                                           FontStyle.Normal);
                curMetrics = writeFont.GetMetrics();
                updateMetricLines();
            }
        }

        void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs args)
        {
            double width = txtblk.ActualWidth;
            ascenderLine.X2 = width;
            capsHeightLine.X2 = width;
            xHeightLine.X2 = width;
            baselineLine.X2 = width;
            descenderLine.X2 = width;
            lineGapLine.X2 = width;
        }

        private void ContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            txtblk.Text = tb.Text;
        }

        private void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            
            if (tb.Text != "")
            {
                int fs = int.Parse(tb.Text);
                txtblk.FontSize = fs;
                updateMetricLines();
            }
        }

        private void updateMetricLines()
        {
            double fontSize = txtblk.FontSize;
            double ascent = fontSize * curMetrics.Ascent / curMetrics.DesignUnitsPerEm;
            double capsHeight = fontSize * curMetrics.CapHeight / curMetrics.DesignUnitsPerEm;
            double xHeight = fontSize * curMetrics.XHeight / curMetrics.DesignUnitsPerEm;
            double descent = fontSize * curMetrics.Descent / curMetrics.DesignUnitsPerEm;
            double lineGap = fontSize * curMetrics.LineGap / curMetrics.DesignUnitsPerEm;

            AscenderTextBlock.Text = "Ascender: " + curMetrics.Ascent + ", " + ascent + "px";
            CapHeightTextBlock.Text = "CapHeight: " + curMetrics.CapHeight + ", " + capsHeight + "px";
            XHeightTextBlock.Text = "X-Height: " + curMetrics.XHeight + ", " + xHeight + "px";
            DescenderTextBlock.Text = "Descender: " + curMetrics.Descent + ", " + descent + "px";
            LineGapTextBlock.Text = "Line-Gap: " + curMetrics.LineGap + ", " + lineGap + "px";
            DesignUnitsTextBlock.Text = "Design Units per Em: " + curMetrics.DesignUnitsPerEm + ", " + fontSize + "px";

            baselineLine.Y1 = baselineLine.Y2 = ascent;
            capsHeightLine.Y1 = capsHeightLine.Y2 = ascent - capsHeight;
            xHeightLine.Y1 = xHeightLine.Y2 = ascent - xHeight;
            descenderLine.Y1 = descenderLine.Y2 = ascent + descent;
            lineGapLine.Y1 = lineGapLine.Y2 = ascent + descent + lineGap;
        }
    }
}
