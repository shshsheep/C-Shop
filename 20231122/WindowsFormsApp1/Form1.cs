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
                int height = this.pictureBox2.Image.Height;
                int width = this.pictureBox2.Image.Width;
                
                Bitmap oldbitmap = (Bitmap)this.pictureBox2.Image;
                int EdgeNum = 0;
                XYPoint[]EdgePoint = new XYPoint[width * height];
                LineParameters[]Line = new LineParameters[width * height];
                for (short x = 0; x < width; x++)
                    for (short y = 0; y < height; y++)
                        if (oldbitmap.GetPixel(x,y).G == 255)
                        {
                            EdgePoint[EdgeNum].X = x;
                            EdgePoint[EdgeNum].Y = y;
                            EdgeNum++;
                        }
                int AngleNum = 360;
                int DistNum = (int)Math.Sqrt(width * width + height * height)*2;
 
                int Threshold = Math.Min(width, height) / 5;
                int HoughSpaceMax = 0;
                Bitmap newbitmap = new Bitmap(AngleNum,DistNum);
                int pixH;
                double DeltaAngle, DeltaDist;
                double MaxDist, MinDist;
                double Angle, Dist;
                int LineCount;
                int[,] HoughSpace = new int[AngleNum,DistNum];
                MaxDist = Math.Sqrt(width*width + height*height);
                MinDist = (double)-width;
                DeltaAngle = Math.PI / AngleNum;
                DeltaDist = (MaxDist - MinDist) / DistNum;
                // space 
                for (int i = 0; i < AngleNum; i++)
                    for (int j = 0; j < DistNum; j++)
                        HoughSpace[i,j] = 0;
                for(int i = 0;i < EdgeNum; i++)
                    for(int j = 0;j < AngleNum; j++)
                    {
                        Angle = j * DeltaAngle;
                        Dist = EdgePoint[i].X * Math.Cos(Angle) + EdgePoint[i].Y * Math.Sin(Angle);
                        HoughSpace[j, (int)((Dist - MinDist) / DeltaDist)]++;
                    }
                 // line in space
                 LineCount = 0;
                for(int i = 0;i < AngleNum; i++)
                    for( int j = 0;j<DistNum; j++)
                    {
                        if (HoughSpace[i,j] > HoughSpaceMax) HoughSpaceMax = HoughSpace[i,j];
                        if (HoughSpace[i,j] >= Threshold)
                        {
                            Line[LineCount].Angle = i;
                            Line[LineCount].Distance = j;
                            LineCount++;
                        }
                    }
                 //draw tranform candidates
                 for (int x = 0;x < AngleNum; x ++)
                    for (int y = 0;y < DistNum; y ++)
                    {
                        pixH = 255 - (HoughSpaceMax - HoughSpace[x,y]) * 255 / HoughSpaceMax;
                        if (HoughSpace[x, y] > Threshold)
                            newbitmap.SetPixel(x, y, Color.FromArgb(pixH, 0, 0));
                        else
                            newbitmap.SetPixel(x, y, Color.FromArgb(pixH, pixH, pixH));
                    }
                this.pictureBox3.Image = newbitmap;
                //draw line
                for(int i = 0; i < LineCount & i < width*height; i++)
                {
                    for (int x = 0;x < width; x++)
                    {
                        int y = (int)((Line[i].Distance * DeltaDist + MinDist - x * Math.Cos(Line[i].Angle * DeltaAngle)) / Math.Sin(Line[i].Angle * DeltaAngle));
                        if(y >= 0 & y < height)
                        {
                            pixH = oldbitmap.GetPixel(x, y).G;
                            oldbitmap.SetPixel(x,y,Color.FromArgb(pixH ^ 255, pixH, pixH));
                        }
                    }
                    for (int y= 0; y < height; y ++)
                    {
                        int x= (int)((Line[i].Distance * DeltaDist + MinDist - y * Math.Sin(Line[i].Angle * DeltaAngle)) / Math.Cos(Line[i].Angle * DeltaAngle));
                        if(x >= 0 & x < width)
                        {
                            pixH = oldbitmap.GetPixel(x,y).G;
                            oldbitmap.SetPixel(x,y,Color.FromArgb(pixH ^ 255,pixH, pixH));
                        }
                    }
                }
                this.pictureBox2.Image = oldbitmap;
               // this.label1.Text = "Hough transfrom 完成";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
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
