using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace JPCToolLibrary
{
    /// <summary>
    /// 条形码
    /// </summary>
    public class BarCode
    {
        /// <summary>
        /// 绘制条形码
        /// </summary>
        /// <param name="text">条形码内容</param>
        /// <param name="width">条形码宽度</param>
        /// <param name="height">条形码高度</param>
        /// <param name="path">条形码存储路径</param>
        /// <param name="format">条形码图片格式（.Bmp .Png等）</param>
        public static void Generate(string text, int width, int height, string path, ImageFormat format)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.CODE_128;
            EncodingOptions options = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 2
            };
            writer.Options = options;
            Bitmap bitmap = writer.Write(text);
            bitmap.Save(path, format);
        }
    }
}
