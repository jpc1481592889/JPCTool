using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCToolLibrary
{
    /// <summary>
    /// 图像处理
    /// </summary>
    public class ImageProcessing
    {
        /// <summary>
        /// 图像的灰度化
        /// </summary>
        /// <param name="filePath">源文件路径</param>
        /// <param name="savePath">保存的目标路径</param>
        public static void ImageGrayscale(string filePath, string savePath)
        {
            Bitmap bitmap = new Bitmap(filePath);
            int height = bitmap.Height;
            int width = bitmap.Width;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bitmap.GetPixel(x, y);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bitmap.SetPixel(x, y, newColor);
                }
            }
            bitmap.Save(savePath);
            bitmap.Dispose();
        }
        /// <summary>
        /// 二值化图像
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <param name="threshold">阈值</param>
        /// <param name="saveImage">
        /// 是否保存图片
        /// 默认保存地址：程序所在目录 + BinImage.bmp
        /// </param>
        /// <returns>二值化后的位图</returns>
        public static Bitmap Binaryzation(Bitmap bitmap, int threshold, bool saveImage = false)
        {
            if (bitmap == null)
            {
                return null;
            }
            try
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                Bitmap bmp = new Bitmap(width, height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        //int value = 255 - color.B;
                        Color newColor = color.B > threshold ? Color.FromArgb(255, 255, 255) : Color.FromArgb(0, 0, 0);
                        bmp.SetPixel(x, y, newColor);
                    }
                }
                if (saveImage)
                {
                    string binPath = AppDomain.CurrentDomain.BaseDirectory + "tempImg_BinImage.png";
                    bmp.Save(binPath, ImageFormat.Png);
                }
                return bmp;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 位图转文本 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<string> ImageToText(Bitmap item)
        {
            List<string> text = new List<string>();
            StringBuilder line = new StringBuilder();

            using (MagickImage image = new MagickImage(item))
            using (IPixelCollection pixels = image.GetPixels())
            {
                //获取RGB像素数组
                byte[] areaPixels = pixels.ToByteArray(PixelMapping.RGB);
                //转为单通道数据数组
                byte[] data = GetSingleChannelData(areaPixels, 0);
                //图像转文本
                int index = 0;
                for (int i = 0; i < image.Height; i++)
                {
                    index = i * image.Width;
                    for (int j = 0; j < image.Width; j++)
                    {
                        line.Append(data[index + j] == 255 ? "1" : "0");//黑0，白1
                    }
                    text.Add(line.ToString());
                    line.Clear();
                }
            }
            return text;
        }
        /// <summary>
        /// 将RGB数组转为单通道数组
        /// </summary>
        /// <param name="rgbData">RGB数据数组</param>
        /// <param name="channel">通道 R-0 G-1 B-2/param>
        /// <returns></returns>
        private static byte[] GetSingleChannelData(byte[] rgbData, int channel)
        {
            //获取RGB像素数组
            byte[] areaPixels = rgbData;
            int count = (int)areaPixels.LongCount();
            int index = 0;
            byte[] data = new byte[count / 3];
            //转为单通道数据数组
            for (int i = 0; i < count; i += 3)
            {
                data[index++] = areaPixels[i + channel];
            }
            return data;
        }
        /// <summary>
        /// 将字符串写入到txt文本
        /// </summary>
        /// <param name="str"></param>
        /// <param name="file"></param>
        public static void writtxt(string str, string file)
        {
            File.AppendAllText(file, str + "\r\n", Encoding.Default);
        }
        /// <summary>
        /// 将待识别的图片分行处理
        /// </summary>
        /// <param name="path">需要分行的图片的路径</param>
        /// <param name="outputText">是否输出图片转文本的结果</param>
        /// <returns>返回Bitmap的泛型集合</returns>
        public static List<Bitmap> BranchProcessing(string path, bool outputText)
        {
            List<Bitmap> bitmaps = new List<Bitmap>();
            Bitmap bitmap = new Bitmap(path);
            List<string> str = ImageToText(bitmap);
            bool b = false;
            Dictionary<string, int> imageStart = new Dictionary<string, int>();//识别图像的信息区起始位置组成的
            Dictionary<string, int> imageStop = new Dictionary<string, int>();
            int k = 1;
            for (int i = 0; i < str.Count; i++)
            {
                //int num = 0;
                //for (int j = 0; j < str[i].Length; j++)
                //{
                //    num += int.Parse(str[i][j].ToString());
                //}
                //if (num > 0 && !b)
                //{
                //    imageStart.Add($"第{k}行", i - 1);
                //    b = true;
                //}
                //else if (num == 0 && b)
                //{
                //    imageStop.Add($"第{k++}行", i);
                //    b = false;
                //}


                if (str[i].Contains("1") && !b)
                {
                    //txt_heightStart.Text += i.ToString() + ",";
                    imageStart.Add($"第{k}行", i - 1);
                    b = true;
                }
                else if (!str[i].Contains("1") && b)
                {
                    //txt_heightStop.Text += i.ToString() + ",";
                    imageStop.Add($"第{k++}行", i);
                    b = false;
                }
                if (outputText)
                {
                    writtxt(str[i], @"D:\\2.txt");
                }
            }
            int width = bitmap.Width;
            int height = bitmap.Height / imageStart.Count;
            for (int i = 0; i < imageStart.Count; i++)
            {
                Bitmap newBitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(newBitmap);
                Rectangle destRect2 = new Rectangle(0, 0, width, height);
                GraphicsUnit units = GraphicsUnit.Pixel;
                g.DrawImage(bitmap, 0, imageStart[$"第{i + 1}行"], width, imageStop[$"第{i + 1}行"] - imageStart[$"第{i + 1}行"]);
                ImageAttributes imageAttr = new ImageAttributes();
                imageAttr.SetGamma(4.0F);
                g.DrawImage(bitmap, destRect2, 0, imageStart[$"第{i + 1}行"], width, imageStop[$"第{i + 1}行"] - imageStart[$"第{i + 1}行"], units, imageAttr);

                g.Dispose();
                bitmaps.Add(newBitmap);
            }
            return bitmaps;
        }
    }
}
