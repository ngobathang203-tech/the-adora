using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DoAnCN_Net
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null) return null;

                string fileName = value.ToString();
                if (string.IsNullOrEmpty(fileName)) return null;

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = fileName.TrimStart('/').Replace('/', '\\');
                string imagePath = System.IO.Path.Combine(baseDir, relativePath);


                System.Diagnostics.Debug.WriteLine($"Đang load ảnh từ: {imagePath}");

                if (File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    System.Diagnostics.Debug.WriteLine($"Đã load ảnh thành công: {imagePath}");
                    return bitmap;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"File ảnh không tồn tại: {imagePath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load ảnh: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}