using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "圖像文件(JPeg,GIF,Bump,etc)|*.jpg;*jpeg;*.gif;*.bump;*.tif;*tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap mybitmap = new Bitmap(openFileDialog.FileName);
                    
                    this.pictureBox1.Image = mybitmap;

                    // 找到紅點座標
                    Point redPoint1 = FindRedPointCenter(mybitmap, Color.FromArgb(255,0,0));

                    // 顯示座標
                    label5.Text = $"紅點座標: ({redPoint1.X}, {redPoint1.Y})";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "圖像文件(JPeg,GIF,Bump,etc)|*.jpg;*jpeg;*.gif;*.bump;*.tif;*tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap mybitmap = new Bitmap(openFileDialog.FileName);
                    
                    this.pictureBox2.Image = mybitmap;

                    // 找到紅點座標
                    Point redPoint2 = FindRedPointCenter(mybitmap, Color.FromArgb(255, 0, 0));

                    // 顯示座標
                    label6.Text = $"紅點座標: ({redPoint2.X}, {redPoint2.Y})";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        private Point FindRedPointCenter(Bitmap bitmap, Color targetColor, int tolerance = 80)
        {
            int totalX = 0;
            int totalY = 0;
            int count = 0;

            // 找到目標顏色的座標
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);

                    // 比較紅點顏色是否在容錯範圍內
                    if (Math.Abs(pixelColor.R - targetColor.R) <= tolerance &&
                        Math.Abs(pixelColor.G - targetColor.G) <= tolerance &&
                        Math.Abs(pixelColor.B - targetColor.B) <= tolerance)
                    {
                        totalX += x;
                        totalY += y;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                // 計算中心座標
                int centerX = totalX / count;
                int centerY = totalY / count;
                return new Point(centerX, centerY);
            }

            // 如果沒有找到目標顏色，可以返回一個特定的值或引發異常
            throw new Exception($"未找到目標顏色 {targetColor}");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // 檢查是否有選擇圖片
                if (pictureBox1.Image == null || pictureBox2.Image == null)
                {
                    MessageBox.Show("請先選擇兩張圖片");
                    return;
                }

                // 找到紅點中心座標
                Point redPoint1Center = FindRedPointCenter(new Bitmap(pictureBox1.Image), Color.FromArgb(255, 0, 0));
                Point redPoint2Center = FindRedPointCenter(new Bitmap(pictureBox2.Image), Color.FromArgb(255, 0, 0));

                // 讀取使用者輸入的基線值
                if (!double.TryParse(textBox1.Text, out double knownBaseline))
                {
                    MessageBox.Show("請輸入有效的基線值");
                    return;
                }

                // 已知的相機參數
                double focalLength = 12.07; // 單位：mm
                double sensorWidth = 7.6; // 單位：mm
                int imageWidth = 2272; // 影像寬度
                double pixelSize = 0.0033450704225352; // 像素大小，單位：mm

                // 計算 disparity（兩像素間的距離）
                double disparity = Math.Abs(redPoint2Center.X - redPoint1Center.X) * pixelSize;

                // 計算深度（結果單位：mm）
                double depth = (focalLength * knownBaseline) / disparity;


                depth = Math.Round(depth, 2);

                label7.Text = $"深度: {depth} 公分";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        /*private double CalculateDepth(Point point1, Point point2, double baseline)
        {
            // 計算兩點之間的水平距離，這可以視為基線
            double horizontalDistance = Math.Abs(point2.X - point1.X);

            // 計算深度（假設兩點在同一水平線上，且基線為已知值）
            double depth = baseline / Math.Tan(Math.Atan(horizontalDistance / baseline));

            return depth;
        }*/
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
