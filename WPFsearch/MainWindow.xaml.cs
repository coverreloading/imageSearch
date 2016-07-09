using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFsearch
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(getData);
            t.Start();
        }

        public void getData()
        {

            // Get the current directory.获得程序当前路径
            string path = Directory.GetCurrentDirectory();
            Console.WriteLine("The current directory is {0}", path);
            DirectoryInfo dicInfo = new DirectoryInfo(path);
            Directory.CreateDirectory(path + "\\data\\");
            // Directory.CreateDirectory(path + "\\data\\gray\\");
            int fileNumber = dicInfo.GetFiles("jpeg\\").Length;
            int sum = 0;
            foreach (var fi in dicInfo.GetFiles("jpeg\\*.jpg"))
            {
                Console.WriteLine(fi.Name);

                // openUrl("http://www.baidu.com");
                // System.Diagnostics.Process.Start();

                Bitmap jp = new Bitmap(fi.FullName);


                //低效转灰度
                jp = toGray(jp);

                Console.WriteLine(path);
                Console.WriteLine(path + @"\data\" + "_" + fi.Name);
                // jp.Save(path + @"\data\gray\" + "_" + fi.Name);
                // System.Diagnostics.Process.Start(path + "\\data\\gray\\_" + fi.Name);
                // System.Diagnostics.Process.Start(fi.FullName);

                // [m, n]   //获取图像尺寸
                int m = jp.Width;
                int n = jp.Height;
                Console.WriteLine("m:" + m + "\nn:" + n);

                // 生成一个矩阵用于后面保存像素点个数
                int[] x = new int[256];


                //Console.WriteLine(jp.GetPixel(x, y).B);



                for (int i = 1; i < m; i++)
                {
                    for (int j = 1; j < n; j++)
                    {
                        x[jp.GetPixel(i, j).B]++;
                    }
                }
                // 进度条初始化 最大值为文件数


                this.proBar.Dispatcher.Invoke(
            new Action(
                 delegate
                 {
                     proBar.Value = 0;
                     proBar.Maximum = fileNumber;
                 }
            )
      );

                //编写器 
                StreamWriter mStreamWriter = new StreamWriter(path + "\\data\\" + System.IO.Path.GetFileNameWithoutExtension(fi.FullName) + ".dat", false, System.Text.Encoding.UTF8);
                for (int i = 0; i < x.Length; i++)
                {
                    mStreamWriter.WriteLine(x[i]);
                }
                //用完StreamWriter的对象后一定要及时销毁 
                mStreamWriter.Close();
                mStreamWriter.Dispose();
                mStreamWriter = null;
                //MessageBox.Show(path + "\\jpeg\\" + System.IO.Path.GetFileNameWithoutExtension(fi.FullName) + ".jpg");

                this.image.Dispatcher.Invoke(
           new Action(
                delegate
                {
                    image.Source = new BitmapImage(new Uri(path + "\\jpeg\\" + System.IO.Path.GetFileNameWithoutExtension(fi.FullName) + ".jpg", UriKind.RelativeOrAbsolute));
                }
           )
     );

                sum++;

                this.label3.Dispatcher.Invoke(
           new Action(
                delegate
                {
                    label3.Content = (sum + "/" + fileNumber);
                }
           )
     );
                this.proBar.Dispatcher.Invoke(
           new Action(
                delegate
                {
                    proBar.Value = sum;
                }
           )
     );

                //App.DoEvents();

            }
            this.image.Dispatcher.Invoke(
           new Action(
                delegate
                {
                    image.Source = new BitmapImage();
                }
           )
     );
            MessageBox.Show("Done!");
        }

        //转灰度图
        public static Bitmap toGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    System.Drawing.Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }


        

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            
        }
        //搜索
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != "")
            {
                string searchFileName = textBox.Text;

                // Get the current directory.获得程序当前路径
                string path = Directory.GetCurrentDirectory();
                Console.WriteLine("The current directory is {0}", path);
                Console.WriteLine("M越低，相似度越高");
                DirectoryInfo dicInfo = new DirectoryInfo(path);
                // Directory.CreateDirectory(path + "\\search\\");
                // Console.WriteLine(path + @"\search\无标题.jpg");
                DirectoryInfo f = new DirectoryInfo(path + @"\jpeg\" + searchFileName + ".jpg");
                // FileInfo[] f = dicInfo.GetFiles("jpeg\\*.jpg");

                DirectoryInfo fi = new DirectoryInfo(path + "\\data\\" + searchFileName + ".dat");
                // Console.WriteLine(fi.FullName);
                // MessageBox.Show(textBox.Text);

                int fileNumber=dicInfo.GetFiles("jpeg\\").Length;
                Console.WriteLine("文件数:" + fileNumber);
                double[] getFileLength = new double[fileNumber];

                

                foreach (var fi2 in dicInfo.GetFiles("data\\*.dat"))
                {
                    StreamReader fs1 = new StreamReader(fi.FullName, Encoding.Default);
                    StreamReader fs2 = new StreamReader(fi2.FullName, Encoding.Default);
                    double distance = 0;
                    double temp = 0;
                    double buf1 = 0;
                    double buf2 = 0;
                    // while (!fs2.EndOfStream)
                    int ms = 256;
                    while (ms > 0)
                    {
                        buf1 = Convert.ToDouble(fs1.ReadLine());
                        //Console.WriteLine(buf1);
                        buf2 = Convert.ToDouble(fs2.ReadLine());
                        //Console.WriteLine(buf2);
                        temp = (Math.Abs(buf1 - buf2));    ////// 求出相似度 数值越小，相似度越大
                        distance += temp;
                        ms--;
                    }
                    //string aFirstName = fi2.Substring(aFile.LastIndexOf("\\") + 1, (aFile.LastIndexOf(".") - aFile.LastIndexOf("\\") - 1));
                    if (distance <=slider.Value)
                    {
                        string nameNoEx = System.IO.Path.GetFileNameWithoutExtension(fi2.FullName);
                        // 获取没有扩展名的文件名System.IO.Path.GetFileNameWithoutExtension(fi2.FullName);
                        // MessageBox.Show(path + @"\jpeg\" + nameNoEx + ".jpg");
                        System.Diagnostics.Process.Start(path + @"\jpeg\" + nameNoEx + ".jpg");

                    }
                    


                }
                
                MessageBox.Show("搜索结束");
                //Console.ReadLine();
            }
            else
                MessageBox.Show("输入文件名");
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        
    }
}
