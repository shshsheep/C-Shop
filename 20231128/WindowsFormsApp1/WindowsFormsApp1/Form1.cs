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
                    Color pixel;
                    for (int x = 0; x < mybitmap.Width; x++)
                        for (int y = 0; y < mybitmap.Height; y++)
                        {
                            pixel = mybitmap.GetPixel(x, y);
                            int r, g, b, result = 0;
                            r = pixel.R;
                            g = pixel.G;
                            b = pixel.B;
                            result = (299 * r + 587 * g + 114 * b) / 1000;
                            mybitmap.SetPixel(x, y, Color.FromArgb(result, result, result));
                        }
                    this.pictureBox1.Image = mybitmap;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Bitmap Image|*.bmp";
                saveFileDialog1.Title = "儲存圖片";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName!="")
                {
                    System.IO.FileStream fs =(System.IO.FileStream)saveFileDialog1.OpenFile();
                    switch(saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            this.pictureBox3.Image.Save(fs,System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        struct XYPoint
        {
            public short X;
            public short Y;
        };
        struct LineParameters
        {
            public int Angle;
            public int Distance;
        };
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int width = this.pictureBox2.Image.Width;
                int height = this.pictureBox2.Image.Height;

                Bitmap oldbitmap = (Bitmap)this.pictureBox2.Image;
                int EdgeNum = 0;
                XYPoint[] EdgePoint = new XYPoint[width * height];

                for (short x = 0; x < width; x++)
                {
                    for (short y = 0; y < height; y++)
                    {
                        if (oldbitmap.GetPixel(x, y).G == 255)
                        {
                            EdgePoint[EdgeNum].X = x;
                            EdgePoint[EdgeNum].Y = y;
                            EdgeNum++;
                        }
                    }
                }

                int RadiusNum = Math.Min(width, height) / 5;
                int[,,] HoughSpace = new int[width, height, RadiusNum];
                int Threshold = 450; // 這是你需要調整的閾值，根據需要減少畫紅線的次數
                double CircularityThreshold = 0.8; // 這是圓形度的閾值，可以根據需要調整

                // 初始化 Hough 空間
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int r = 0; r < RadiusNum; r++)
                        {
                            HoughSpace[x, y, r] = 0;
                        }
                    }
                }

                // 在 Hough 空間中進行投票
                for (int i = 0; i < EdgeNum; i++)
                {
                    for (int r = 0; r < RadiusNum; r++)
                    {
                        for (int theta = 0; theta < 360; theta++)
                        {
                            double radianTheta = theta * Math.PI / 180;
                            int a = (int)(EdgePoint[i].X - r * Math.Cos(radianTheta));
                            int b = (int)(EdgePoint[i].Y - r * Math.Sin(radianTheta));

                            if (a >= 0 && a < width && b >= 0 && b < height)
                            {
                                HoughSpace[a, b, r]++;
                            }
                        }
                    }
                }

                int minRadius = 10; // 最小半徑
                int maxRadius = 100; // 最大半徑

                // 進行圓形度和閾值的檢查
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int r = 0; r < RadiusNum; r++)
                        {
                            if (IsCircular(HoughSpace, x, y, r, Threshold, CircularityThreshold, minRadius, maxRadius))
                            {
                                // 在原始影像上繪製檢測到的圓形
                                DrawCircle(oldbitmap, x, y, r, 0.5);
                            }
                        }
                    }
                }

                this.pictureBox3.Image = oldbitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }
        private void DrawCircle(Bitmap bitmap, int centerX, int centerY, int radius, double circularityThreshold)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Pen pen = new Pen(Color.Red, 2); // 這裡的 2 代表線條寬度，你可以根據需要調整

                int diameter = radius * 2;
                int x = centerX - radius;
                int y = centerY - radius;

                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
        }

        private bool IsCircular(int[,,] houghSpace, int x, int y, int radius, int threshold, double circularityThreshold, int minRadius, int maxRadius)
        {
            int voteCount = houghSpace[x, y, radius];

            // 檢查投票點是否是局部最大值
            bool isLocalMax = IsLocalMaximum(houghSpace, x, y, radius);

            // 檢查圓形度
            double expectedCircumference = 2 * Math.PI * radius;
            double actualCircumference = voteCount;
            double circularity = actualCircumference / expectedCircumference;

            // 檢查額外的條件，例如半徑範圍
            bool isReasonableRadius = IsReasonableRadius(radius, minRadius, maxRadius);

            // 最終條件檢查
            return (voteCount >= threshold && circularity >= circularityThreshold && isLocalMax && isReasonableRadius);

        }

        private bool IsLocalMaximum(int[,,] houghSpace, int x, int y, int radius)
        {
            int currentValue = houghSpace[x, y, radius];

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < houghSpace.GetLength(0) && j >= 0 && j < houghSpace.GetLength(1))
                    {
                        if (houghSpace[i, j, radius] > currentValue)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool IsReasonableRadius(int radius, int minRadius, int maxRadius)
        {
            return radius >= minRadius && radius <= maxRadius;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int height = this.pictureBox1.Image.Height;
                int width = this.pictureBox1.Image.Width;
                Bitmap newbitmap = new Bitmap(width, height);
                Bitmap oldbitmap = (Bitmap)this.pictureBox1.Image;
                int[,] sobelX = { {-1, 0, 1},
                                  {-2, 0, 2},
                                  {-1, 0, 1} };

                int[,] sobelY = { {-1, -2, -1},
                                  {0, 0, 0},
                                  {1, 2, 1} };

                int maxGradient = 0;
                int minGradient = int.MaxValue;

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        int gx = 0;
                        int gy = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                gx += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                                gy += oldbitmap.GetPixel(x + i, y + j).R * sobelY[i + 1, j + 1];
                            }
                        }

                        gx = Math.Abs(gx);
                        gy = Math.Abs(gy);

                        int gradient = gx + gy;
                        minGradient = Math.Min(minGradient, gradient);
                        maxGradient = Math.Max(maxGradient, gradient); // 更新最大梯度值
                    }
                }

                for (int x = 1; x < width - 1; x++)
                    for (int y = 1; y < height - 1; y++)
                    {
                        int gx = 0;
                        int gy = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                gx += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                                gy += oldbitmap.GetPixel(x + i, y + j).R * sobelY[i + 1, j + 1];
                            }
                        }

                        gx = Math.Abs(gx);
                        gy = Math.Abs(gy);

                        int gradient = gx + gy;

                        //gradient = (int)(gradient * 255.0 / maxGradient);
                        gradient = Math.Min(255, Math.Max(0, gradient));

                        newbitmap.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
                    }
                this.pictureBox2.Image = newbitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }
    }
}
