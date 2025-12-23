using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PromptStamp.Converters
{
    public class ImagePathToImageSourceConverter : IValueConverter
    {
        // フォールバック画像（Resource でも OK）
        public ImageSource FallbackImage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string path)
            {
                return FallbackImage;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                return FallbackImage;
            }

            if (!File.Exists(path))
            {
                return FallbackImage;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze(); // ★ 重要（スレッド安全 & パフォーマンス）

                return bitmap;
            }
            catch
            {
                return FallbackImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}