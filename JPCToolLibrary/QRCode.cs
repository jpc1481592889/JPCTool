using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCToolLibrary
{
    /// <summary>
    /// 二维码
    /// </summary>
    public class QRCode
    {
        /// <summary>
        /// 生成带图标的二维码存储到指定的路径
        /// </summary>
        /// <param name="msg">二维码内容</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <param name="pixel">像素点大小（二维码的大小）</param>
        /// <param name="icon_path">图标路径</param>
        /// <param name="icon_size">图标尺寸</param>
        /// <param name="icon_border">图标边框厚度</param>
        /// <param name="white_edge">二维码白边</param>
        /// <param name="path">二维码存储路径</param>
        /// <param name="format">二维码图片格式（.Bmp .Png等）</param>
        public static void Code(string msg, int version, int pixel, string icon_path, int icon_size, int icon_border, bool white_edge, string path, ImageFormat format)
        {
            QRCoder.QRCodeGenerator code_generator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M/* 这里设置容错率的一个级别 */, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);
            QRCoder.QRCode code = new QRCoder.QRCode(code_data);
            Bitmap icon = new Bitmap(icon_path);
            Bitmap bmp = code.GetGraphic(pixel, Color.Black, Color.White, icon, icon_size, icon_border, white_edge);
            bmp.Save(path, format);
        }
        /// <summary>
        /// 生成带图标的二维码
        /// </summary>
        /// <param name="msg">二维码内容</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <param name="pixel">像素点大小（二维码的大小）</param>
        /// <param name="icon_path">图标路径</param>
        /// <param name="icon_size">图标尺寸</param>
        /// <param name="icon_border">图标边框厚度</param>
        /// <param name="white_edge">二维码白边</param>
        /// <returns></returns>
        public static Bitmap Code(string msg, int version, int pixel, string icon_path, int icon_size, int icon_border, bool white_edge)
        {
            QRCoder.QRCodeGenerator code_generator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M/* 这里设置容错率的一个级别 */, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);
            QRCoder.QRCode code = new QRCoder.QRCode(code_data);
            Bitmap icon = new Bitmap(icon_path);
            Bitmap bmp = code.GetGraphic(pixel, Color.Black, Color.White, icon, icon_size, icon_border, white_edge);
            return bmp;
        }
        /// <summary>
        /// 生成不带图标的二维码
        /// </summary>
        /// <param name="msg">二维码内容</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <param name="pixel">像素点大小（二维码的大小）</param>
        /// <param name="white_edge">二维码白边</param>
        /// <returns></returns>
        public static Bitmap Code(string msg, int version, int pixel, bool white_edge)
        {
            QRCoder.QRCodeGenerator code_generator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M/* 这里设置容错率的一个级别 */, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);
            QRCoder.QRCode code = new QRCoder.QRCode(code_data);
            Bitmap bmp = code.GetGraphic(pixel, Color.Black, Color.White, null, 0, 0, white_edge);
            return bmp;
        }
        /// <summary>
        /// 生成不带图标的二维码到指定的路径
        /// </summary>
        /// <param name="msg">二维码内容</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <param name="pixel">像素点大小（二维码的大小）</param>
        /// <param name="white_edge">二维码白边</param>
        /// <param name="path">二维码存储路径</param>
        /// <param name="format">二维码图片格式（.Bmp .Png等）</param>
        public static void Code(string msg, int version, int pixel, bool white_edge, string path, ImageFormat format)
        {
            QRCoder.QRCodeGenerator code_generator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M/* 这里设置容错率的一个级别 */, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);
            QRCoder.QRCode code = new QRCoder.QRCode(code_data);
            Bitmap bmp = code.GetGraphic(pixel, Color.Black, Color.White, null, 0, 0, white_edge);
            bmp.Save(path, format);
        }
    }
}
