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

                double A = 0;

                double[,] sobelX = new double[,] { { 0, -1, 0 }, { -1, A + 4, -1 }, { 0, -1, 0 } };

                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    if (double.TryParse(textBox1.Text, out A))
                    {
                        sobelX[1, 1] = A + 4;
                    }
                    else
                    {
                        // 處理無法解析為 double 的情況，可以提示用戶或採取其他適當的措施。
                        MessageBox.Show("請輸入有效的數字", "錯誤");
                        return;
                    }
                }

                double maxGradient = 0;
                double minGradient = double.MaxValue;

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        double sum = 0;

                        // 使用新的濾波器
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                sum += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                            }
                        }

                        double gradient = Math.Abs(sum);

                        minGradient = Math.Min(minGradient, gradient);
                        maxGradient = Math.Max(maxGradient, gradient);
                    }
                }

                double scaleFactor = 255.0 / (maxGradient - minGradient);

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        double sum = 0;

                        // 使用新的濾波器
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                sum += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                            }
                        }

                        double gradient = Math.Abs(sum);

                        //gradient = (int)((gradient - minGradient) * scaleFactor);

                        gradient = Math.Max(0, Math.Min(gradient, 255));

                        newbitmap.SetPixel(x, y, Color.FromArgb((int)gradient, (int)gradient, (int)gradient));
                    }
                }

                this.pictureBox2.Image = newbitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int height = this.pictureBox1.Image.Height;
                int width = this.pictureBox1.Image.Width;
                Bitmap newbitmap = new Bitmap(width, height);
                Bitmap oldbitmap = (Bitmap)this.pictureBox1.Image;

                double A = 0;

                double[,] sobelX = new double[,] { { -1, -1, -1 }, { -1, A + 8, -1 }, { -1, -1, -1 } };

                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    if (double.TryParse(textBox1.Text, out A))
                    {
                        sobelX[1, 1] = A + 8;
                    }
                    else
                    {
                        // 處理無法解析為 double 的情況，可以提示用戶或採取其他適當的措施。
                        MessageBox.Show("請輸入有效的數字", "錯誤");
                        return;
                    }
                }

                double maxGradient = 0;
                double minGradient = double.MaxValue;

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        double sum = 0;

                        // 使用新的濾波器
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                sum += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                            }
                        }

                        double gradient = Math.Abs(sum);

                        minGradient = Math.Min(minGradient, gradient);
                        maxGradient = Math.Max(maxGradient, gradient);
                    }
                }

                double scaleFactor = 255.0 / (maxGradient - minGradient);

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        double sum = 0;

                        // 使用新的濾波器
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                sum += oldbitmap.GetPixel(x + i, y + j).R * sobelX[i + 1, j + 1];
                            }
                        }

                        double gradient = Math.Abs(sum);

                        //gradient = (int)((gradient - minGradient) * scaleFactor);

                        gradient = Math.Max(0, Math.Min(gradient, 255));

                        newbitmap.SetPixel(x, y, Color.FromArgb((int)gradient, (int)gradient, (int)gradient));
                    }
                }

                this.pictureBox3.Image = newbitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        
    }
}
