using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        private Point matchPoint1;  // 存儲 button1 的匹配座標
        private Point matchPoint2;  // 存儲 button4 的匹配座標
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
                    Point redPoint1 = FindRedPointCenter(mybitmap, Color.FromArgb(255, 0, 0));

                    // 切出紅點附近小塊圖像
                    Rectangle templateRect1 = new Rectangle(redPoint1.X - 20, redPoint1.Y - 20, 40, 40); // 設定切割範圍
                    Bitmap templateImage1 = CropImage(mybitmap, templateRect1);

                    // 顯示座標
                    label5.Text = $"紅點座標: ({redPoint1.X}, {redPoint1.Y})";

                    // 顯示切割後的模板圖像
                    pictureBox3.Image = templateImage1;

                    // 轉換模板圖像為 Color 類型的二維數組
                    Color[][] templateArray = ConvertBitmapToColorArray(templateImage1);

                    // 轉換主圖像為 Color 類型的二維數組
                    Color[][] mainImageArray = ConvertBitmapToColorArray(mybitmap);

                    // 計算特徵模板匹配
                    matchPoint1 = CalculateTemplateMatchingSAD(mainImageArray, templateArray);


                    // 顯示匹配座標
                    label3.Text = $"匹配座標: ({matchPoint1.X}, {matchPoint1.Y})";

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

                    // 切出紅點附近小塊圖像
                    Rectangle templateRect2 = new Rectangle(redPoint2.X - 20, redPoint2.Y - 20, 40, 40); // 設定切割範圍
                    Bitmap templateImage2 = CropImage(mybitmap, templateRect2);

                    // 顯示座標
                    label6.Text = $"紅點座標: ({redPoint2.X}, {redPoint2.Y})";

                    // 顯示切割後的模板圖像
                    pictureBox4.Image = templateImage2;

                    // 轉換模板圖像為 Color 類型的二維數組
                    Color[][] templateArray = ConvertBitmapToColorArray(templateImage2);

                    // 轉換主圖像為 Color 類型的二維數組
                    Color[][] mainImageArray = ConvertBitmapToColorArray(mybitmap);

                    // 計算特徵模板匹配
                    matchPoint2 = CalculateTemplateMatchingSAD(mainImageArray, templateArray);


                    // 顯示匹配座標
                    label8.Text = $"匹配座標: ({matchPoint2.X}, {matchPoint2.Y})";

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

        private Bitmap CropImage(Bitmap source, Rectangle cropRect)
        {
            // 切割圖像
            Bitmap croppedImage = source.Clone(cropRect, source.PixelFormat);
            return croppedImage;
        }

        private Color[][] ConvertBitmapToColorArray(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            Color[][] colorArray = new Color[height][];

            for (int y = 0; y < height; y++)
            {
                colorArray[y] = new Color[width];

                for (int x = 0; x < width; x++)
                {
                    colorArray[y][x] = bitmap.GetPixel(x, y);
                }
            }

            return colorArray;
        }

        private Point CalculateTemplateMatchingSAD(Color[][] mainImage, Color[][] template)
        {
            int minSAD = int.MaxValue;
            Point bestPosition = Point.Empty;

            int S_rows = mainImage.Length;
            int S_cols = mainImage[0].Length;
            int T_rows = template.Length;
            int T_cols = template[0].Length;

            for (int x = 0; x <= S_cols - T_cols; x++)
            {
                for (int y = 0; y <= S_rows - T_rows; y++)
                {
                    int SAD = 0;

                    for (int j = 0; j < T_cols; j++)
                    {
                        for (int i = 0; i < T_rows; i++)
                        {
                            Color p_SearchIMG = mainImage[y + i][x + j];
                            Color p_TemplateIMG = template[i][j];

                            // 計算每個色道的 SAD，但保持在 0 到 255 之間
                            SAD += Math.Max(0, Math.Min(255, Math.Abs(p_SearchIMG.R - p_TemplateIMG.R)));
                            SAD += Math.Max(0, Math.Min(255, Math.Abs(p_SearchIMG.G - p_TemplateIMG.G)));
                            SAD += Math.Max(0, Math.Min(255, Math.Abs(p_SearchIMG.B - p_TemplateIMG.B)));
                        }
                    }

                    if (minSAD > SAD)
                    {
                        minSAD = SAD;

                        bestPosition = new Point(x, y);
                    }
                }
            }

            return bestPosition;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                
                // 檢查是否已選擇兩張圖片
                if (pictureBox1.Image == null || pictureBox2.Image == null)
                {
                    MessageBox.Show("請先選擇兩張圖片。");
                    return;
                }
                
                // 從 button1 獲得匹配座標
                //Point matchPoint1 = ParsePoint(label3.Text);

                // 從 button4 獲得匹配座標
                //Point matchPoint2 = ParsePoint(label8.Text);

                // 從使用者輸入中讀取已知的基線值
                if (!double.TryParse(textBox1.Text, out double knownBaseline))
                {
                    MessageBox.Show("請輸入有效的基線值。");
                    return;
                }

                // 已知的相機參數
                double focalLength = 12.07; // 單位：mm
                double pixelSize = 0.0033450704225352; // 像素大小，單位：mm 每像素

                // 計算視差（兩像素之間的距離）
                double disparity = Math.Abs(matchPoint2.X - matchPoint1.X) * pixelSize;

                // 計算深度（結果單位：mm）
                double depth = (focalLength * knownBaseline) / disparity;

                // 將深度值四捨五入到兩位小數
                depth = Math.Round(depth, 2);

                // 以公分顯示深度
                label7.Text = $"深度: {depth} 公分";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        /*
        private Point ParsePoint(string text)
        {
            try
            {
                text = text.Trim('(', ')');
                string[] coordinates = text.Split(',');

                Console.WriteLine($"Debug: Coordinates - {coordinates[0]}, {coordinates[1]}");

                if (coordinates.Length == 2 &&
                    int.TryParse(coordinates[0].Trim(), out int x) &&
                    int.TryParse(coordinates[1].Trim(), out int y))
                {
                    return new Point(x, y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Debug: Error - {ex.Message}");
            }

            throw new FormatException("座標格式錯誤。");
        }
        */


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        
    }
}
/*
minSAD = VALUE_MAX;

for (size_t x = 0; x <= S_cols - T_cols; x++)
{
    for(size_t y = 0; y <= S_rows - T_rows; y++)
    {
        SAD = 0.0;

        for (size_t j = 0; j < T_cols; j++)
            for( size_t i = 0; i < T_rows; i++)
            {
                pixel p_SearchIMG = S[y + i][x + j];
                pixel p_TemplateIMG = T[i][j];

                SAD += abs(p_SearchIMG.Grey - p_TemplateIMG.Grey);
            }

        if(minSAD > SAD)
        {
            minSAD = SAD;

            position.bestRow = y;
            position.bestCol = x;
            position.bestSAD = SAD;
        }
    }
}
*/
