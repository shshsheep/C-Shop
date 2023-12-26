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

                        gradient = (int)((gradient - minGradient) * 255 / (maxGradient - minGradient));

                        newbitmap.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
                    }
                this.pictureBox2.Image = newbitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            //try
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

                int threshold = 0;

                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    threshold = int.Parse(textBox1.Text);
                }

                int minGradient = int.MaxValue;
                int maxGradient = 0;
                
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

                        maxGradient = Math.Max(maxGradient, gradient); // 更新最大梯度值
                        minGradient = Math.Min(minGradient, gradient);

                        gradient = (int)((double)(gradient - minGradient) * 255 / (maxGradient - minGradient));

                        // 檢查是否超過門檻值，超過門檻值的像素設為255，否則設為0
                        gradient = (gradient > threshold) ? 255 : 0;

                        newbitmap.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
    
                     }

               this.pictureBox2.Image = newbitmap;
            }
            /*
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
            */
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int height = this.pictureBox1.Image.Height;
                int width = this.pictureBox1.Image.Width;
                Bitmap newbitmap = new Bitmap(width, height);
                Bitmap oldbitmap = (Bitmap)this.pictureBox1.Image;
                int[,] sobelX = { {-1, -2, -1},
                                  { 0,  0,  0},
                                  { 1,  2,  1} };

                int maxGradient = 0;
                int minGradient = int.MinValue;

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        int gx = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                gx += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                            }
                        }

                        gx = Math.Abs(gx);

                        maxGradient = Math.Max(maxGradient, gx); // 更新最大梯度值
                        minGradient = Math.Min(minGradient, gx);

                    }
                }

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        int gx = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                gx += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                            }
                        }

                        gx = Math.Abs(gx);

                        int gradient = (int)((double)gx / maxGradient * 255);
                        gradient = Math.Min(255, Math.Max(0, gradient));
                        newbitmap.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
                    }
                }

                this.pictureBox2.Image = newbitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int height = this.pictureBox1.Image.Height;
                int width = this.pictureBox1.Image.Width;
                Bitmap newbitmap = new Bitmap(width, height);
                Bitmap oldbitmap = (Bitmap)this.pictureBox1.Image;
                int[,] sobelY = { {-1, 0, 1},
                                  {-2, 0, 2},
                                  {-1, 0, 1} };

                int maxGradient = 0;
                int minGradient = int.MaxValue;

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        int gy = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                gy += oldbitmap.GetPixel(x + i, y + j).R * sobelY[i + 1, j + 1];
                            }
                        }

                        gy = Math.Abs(gy);
                        minGradient = Math.Min(minGradient, gy);
                        maxGradient = Math.Max(maxGradient, gy);
                    }
                }

                // 使用固定的範圍進行正規化
                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        int gy = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                gy += oldbitmap.GetPixel(x + i, y + j).R * sobelY[i + 1, j + 1];
                            }
                        }

                        gy = Math.Abs(gy);

                        // 使用固定的範圍進行正規化，確保在0到255的範圍內
                        int gradient = (int)((double)gy / maxGradient * 255);
                        gradient = Math.Min(255, Math.Max(0, gradient));

                        newbitmap.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
                    }
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
