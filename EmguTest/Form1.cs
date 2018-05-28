using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using System.Drawing.Imaging;
using System.Management;
using System.Diagnostics;

using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math.Geometry;
using AForge.Video.FFMPEG;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.XFeatures2D;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;

using Image = System.Drawing.Image; //Remove ambiguousness between AForge.Image and System.Drawing.Image
using Point = System.Drawing.Point; //Remove ambiguousness between AForge.Point and System.Drawing.Point

namespace EmguTest
{
    public partial class Form1 : Form
    {
        string OriPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = OriPath;

            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OriPath = System.IO.Directory.GetCurrentDirectory();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(320, 240, new Bgr(0, 0, 255));//创建一张320*240尺寸颜色为红色的图像。  
            imageBox1.Image = image;//在ImageBox1控件中显示所创建好的图像。
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string pathread = OriPath + "\\test1.jpg";

            string pathread2 = OriPath + "\\test2.jpg";

            string pathread3 = OriPath + "\\testoutput.jpg";

            //输入图像
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(pathread);//从文件加载图片
            //输出图像
            Image<Bgr, Byte> image2 = new Image<Bgr, Byte>(pathread2);//从文件加载图片



            PointF[] srcquad = new PointF[4];
            PointF[] dstquad = new PointF[4];
            /*
            srcquad[0].X = 400;
            srcquad[0].Y = 400;
            srcquad[1].X = 2600;
            srcquad[1].Y = 400;
            srcquad[2].X = 400;
            srcquad[2].Y = 2600;
            srcquad[3].X = 3600;
            srcquad[3].Y = 2600;

            dstquad[0].X = 800;
            dstquad[0].Y = 400;
            dstquad[1].X = 2000;
            dstquad[1].Y = 400;
            dstquad[2].X = 400;
            dstquad[2].Y = 2600;
            dstquad[3].X = 3600;
            dstquad[3].Y = 2600;
            */
            srcquad[0].X = 664;
            srcquad[0].Y = 372;
            srcquad[1].X = 1011;
            srcquad[1].Y = 605;
            srcquad[2].X = 486;
            srcquad[2].Y = 878;
            srcquad[3].X = 1420;
            srcquad[3].Y = 809;

            dstquad[0].X = 577;
            dstquad[0].Y = 477;
            dstquad[1].X = 823;
            dstquad[1].Y = 641;
            dstquad[2].X = 434;
            dstquad[2].Y = 839;
            dstquad[3].X = 1140;
            dstquad[3].Y = 800;
            Mat mywarpmat = CvInvoke.GetPerspectiveTransform(srcquad, dstquad);

            Mat image2fe = new Mat();

            CvInvoke.WarpPerspective(image, image2fe, mywarpmat, new Size(1280, 960));


            /*
            mymat = (Matrix<double>)mywarpmat;

            gray1 = gray.WarpPerspective<double>(mywarpmat, CV_INTER_NN, WARP.CV_WARP_FILL_OUTLIERS, new Gray(255));
            mywarpmat = Emgu.CV.CameraCalibration.GetPerspectiveTransform(dstquad, srcquad);
            mymat = (Matrix<double>)mywarpmat;
            gray = gray1.WarpPerspective<double>(mymat, Emgu.CV.CvEnum.INTER.CV_INTER_NN, Emgu.CV.CvEnum.WARP.CV_WARP_FILL_OUTLIERS, new Gray(255));


            map_matrix = CvInvoke.GetPerspectiveTransform(srcQuad, desQuad, map_matrix)

            */



            imageBox1.Image = image2fe;//显示图片

            image2fe.Save(pathread3);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            GetMatch();
        }
        private void GetMatch()
        {
            string pathread = OriPath + "\\OriSourcePic.jpg";
            string pathread2 = OriPath + "\\QuadrilateralTransfedPic.jpg";
            string pathread3 = OriPath + "\\Effect.jpg";
            string pathsave = OriPath + "\\testoutput.jpg";

            Image<Bgra, byte> a = new Image<Bgra, byte>(pathread).Resize(0.4, Inter.Area);  //模板
            Image<Bgra, byte> b = new Image<Bgra, byte>(pathread2).Resize(0.4, Inter.Area);  //待匹配的图像

            Mat homography = null;
            Mat mask = null;
            VectorOfKeyPoint modelKeyPoints = new VectorOfKeyPoint();
            VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();



            UMat a1 = a.ToUMat();
            UMat b1 = b.ToUMat();

            SURF surf = new SURF(300);
            UMat modelDescriptors = new UMat();
            UMat observedDescriptors = new UMat();

            surf.DetectAndCompute(a1, null, modelKeyPoints, modelDescriptors, false);       //进行检测和计算，把opencv中的两部分和到一起了，分开用也可以
            surf.DetectAndCompute(b1, null, observedKeyPoints, observedDescriptors, false);

            BFMatcher matcher = new BFMatcher(DistanceType.L2);       //开始进行匹配
            matcher.Add(modelDescriptors);
            matcher.KnnMatch(observedDescriptors, matches, 2, null);
            mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(255));
            Features2DToolbox.VoteForUniqueness(matches, 0.8, mask);   //去除重复的匹配

            int Count = CvInvoke.CountNonZero(mask);      //用于寻找模板在图中的位置
            if (Count >= 4)
            {
                Count = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                if (Count >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
            }

            Mat result = new Mat();
            Features2DToolbox.DrawMatches(a.Convert<Gray, byte>().Mat, modelKeyPoints, b.Convert<Gray, byte>().Mat, observedKeyPoints, matches, result, new MCvScalar(255, 0, 255), new MCvScalar(0, 255, 255), mask);
            //绘制匹配的关系图

            List<IntPoint> corners = new List<IntPoint>();

            if (homography != null)     //如果在图中找到了模板，就把它画出来
            {
                Rectangle rect = new Rectangle(Point.Empty, a.Size);
                PointF[] points = new PointF[]
                {
                  new PointF(rect.Left, rect.Bottom),
                  new PointF(rect.Right, rect.Bottom),
                  new PointF(rect.Right, rect.Top),
                  new PointF(rect.Left, rect.Top)
                };
                points = CvInvoke.PerspectiveTransform(points, homography);
                Point[] points2 = Array.ConvertAll<PointF, Point>(points, Point.Round);
                VectorOfPoint vp = new VectorOfPoint(points2);
                CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 15);


                textBox9.Text = Convert.ToString((int)(points2[3].X * 2.5));
                textBox10.Text = Convert.ToString((int)(points2[3].Y * 2.5));

                textBox11.Text = Convert.ToString((int)(points2[2].X * 2.5));
                textBox12.Text = Convert.ToString((int)(points2[2].Y * 2.5));

                textBox13.Text = Convert.ToString((int)(points2[1].X * 2.5));
                textBox14.Text = Convert.ToString((int)(points2[1].Y * 2.5));

                textBox15.Text = Convert.ToString((int)(points2[0].X * 2.5));
                textBox16.Text = Convert.ToString((int)(points2[0].Y * 2.5));
                /*
                corners.Add(new IntPoint(points2[3].X, points2[3].Y));
                corners.Add(new IntPoint(points2[2].X, points2[2].Y));
                corners.Add(new IntPoint(points2[1].X, points2[1].Y));
                corners.Add(new IntPoint(points2[0].X, points2[0].Y));
                */
            }

            // define quadrilateral's corners

            //Bitmap bufsourceImage = a.ToBitmap();
            /*
            Bitmap bufsourceImage = new Bitmap(pathread3);

            Bitmap sourceImage = new Bitmap(bufsourceImage, a.Size);

            Bitmap bufbitmap = b.ToBitmap();

            Bitmap image = new Bitmap(bufbitmap.Width, bufbitmap.Height);


            // create filter
            BackwardQuadrilateralTransformation filter =
                new BackwardQuadrilateralTransformation(sourceImage, corners);
            // apply the filter
            Bitmap newImage = filter.Apply(image);


            pictureBox2.Image = newImage;
            
            newImage.Save(pathsave);
            */

            imageBox1.Image = result;
        }
        private void fourbackchange()
        {
            string pathread = OriPath + "\\OriSourcePic.jpg";
            string pathread2 = OriPath + "\\QuadrilateralTransfedPic.jpg";
            string pathread3 = OriPath + "\\Effect.jpg";
            string pathsave = OriPath + "\\testoutput.jpg";


            List<IntPoint> corners = new List<IntPoint>();

            corners.Add(new IntPoint(Convert.ToInt32(textBox9.Text), Convert.ToInt32(textBox10.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox11.Text), Convert.ToInt32(textBox12.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox13.Text), Convert.ToInt32(textBox14.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox15.Text), Convert.ToInt32(textBox16.Text)));


            Bitmap a = new Bitmap(pathread);
            //效果图
            Bitmap bufsourceImage = new Bitmap(pathread3);

            Bitmap sourceImage = new Bitmap(bufsourceImage, a.Size);
            //变换图片的
            Bitmap bufbitmap = new Bitmap(pathread2);

            Bitmap image = new Bitmap(bufbitmap.Width, bufbitmap.Height);


            // create filter
            BackwardQuadrilateralTransformation filter =
                new BackwardQuadrilateralTransformation(sourceImage, corners);
            // apply the filter
            Bitmap newImage = filter.Apply(image);

            newImage.Save(pathsave);

            //Bitmap bufimage = new Bitmap(pathread3);

            pictureBox1.Image = newImage;
            pictureBox2.Image = newImage;

        }


        private void button5_Click(object sender, EventArgs e)
        {

            string pathsave = OriPath + "\\CameraPic.jpg";

            Mat frame = new Mat();


            /*
            Application.Idle += new EventHandler(delegate (object sender2, EventArgs e2)
            {  // “Idle”处理循环的事件处理过程  
                viewer.Image = capture.QueryFrame(); //在视窗中显示抓取的帧图像  
            });
            */

            ImageViewer viewer = new ImageViewer(); //创建图像视窗  
            VideoCapture capture = new VideoCapture(); //创建摄像头捕获

            capture.SetCaptureProperty(CapProp.FrameHeight, 900);
            capture.SetCaptureProperty(CapProp.FrameWidth, 1600);
            //capture.ImageGrabbed += ProcessFrame;



            capture.Retrieve(frame, 0);    //接收数据

            pictureBox1.Image = frame.Bitmap;


            frame.Save(pathsave);

            capture.Dispose();
            viewer.Dispose();

            //Image<Bgra, byte> zzz = frame.ToImage<Bgra, byte>();

            /*
            for (int i = 0; i < zzz.Height; i++)
            {
                for (int ii = 0; ii < zzz.Width; ii++)
                {
                    zzz.Data[100, 100, 0] = (byte)(zzz.Data[100, 100, 0] * 0.8);
                    zzz.Data[100, 100, 1] = (byte)(zzz.Data[100, 100, 1] * 0.8);
                    zzz.Data[100, 100, 2] = (byte)(zzz.Data[100, 100, 2] * 0.8);

                    //byte pic = zzz.Data[100, 100, 0];

                }
            }
            */

            //VideoWriter vw = new public VideoWriter(string fileName, int fps, int width, int height, bool isColor);

            //Then write your frame




            //Mat frame2 = zzz.Mat;



            //frame2.Save(pathsave);
            //viewer.ShowDialog(); //显示图像视窗 

        }
        public Bitmap newbitmap;
        public Bitmap curbitmap;

        private void button6_Click(object sender, EventArgs e)
        {
            string pathread = OriPath + "\\CameraPic.jpg";


            newbitmap = new Bitmap(pathread);

            curbitmap = new Bitmap(pathread);


            pictureBox1.Image = curbitmap;


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }
        public int posfinder = 0;
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("横坐标:" + e.X.ToString() + "\n纵坐标:" + e.Y.ToString());

            if (posfinder == 0)
            {
                int xxx = (int)(((double)curbitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)curbitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox1.Text = xxx.ToString();
                textBox2.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);

            }
            else if (posfinder == 1)
            {
                int xxx = (int)(((double)curbitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)curbitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox3.Text = xxx.ToString();
                textBox4.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);
            }
            else if (posfinder == 2)
            {
                int xxx = (int)(((double)curbitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)curbitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox5.Text = xxx.ToString();
                textBox6.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);
            }
            else if (posfinder == 3)
            {
                int xxx = (int)(((double)curbitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)curbitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox7.Text = xxx.ToString();
                textBox8.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);

                posfinder = -1;
            }

            posfinder++;





        }

        public void bitmapupdate(int xxx, int yyy)
        {

            //int xxx = 100;
            //int yyy = 100;

            BitmapData curimageData = curbitmap.LockBits(new Rectangle(0, 0, curbitmap.Width, curbitmap.Height),
            ImageLockMode.ReadOnly, curbitmap.PixelFormat);

            unsafe
            {
                try
                {
                    UnmanagedImage img = new UnmanagedImage(curimageData);

                    int height = img.Height;
                    int width = img.Width;
                    int pixelSize = (img.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    byte* p = (byte*)img.ImageData.ToPointer();

                    // for each line
                    for (int y = 0; y < height; y++)
                    {

                        // for each pixel
                        for (int x = 0; x < width; x++, p += pixelSize)
                        {
                            if (Math.Abs(x - xxx) < 20 && Math.Abs(y - yyy) < 20)
                            {
                                p[RGB.R] = 255;
                                p[RGB.G] = 0;
                                p[RGB.B] = 255;
                            }
                        }

                    }



                }
                finally
                {
                    curbitmap.UnlockBits(curimageData); //Unlock

                }

            }

            pictureBox1.Image = curbitmap;


        }


        public void qchange()
        {
            //Convert.ToInt32(textBox1.Text);
            string pathread2 = OriPath + "\\QuadrilateralTransfedPic.jpg";

            List<IntPoint> corners = new List<IntPoint>();
            corners.Add(new IntPoint(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox4.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox7.Text), Convert.ToInt32(textBox8.Text)));
            // create filter
            QuadrilateralTransformation filter =
                new QuadrilateralTransformation(corners, 1920, 1080);
            // apply the filter
            Bitmap ProjectorPos = filter.Apply(newbitmap);

            pictureBox2.Image = ProjectorPos;

            ProjectorPos.Save(pathread2);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox2.Dock = System.Windows.Forms.DockStyle.None;
            ActiveForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            pictureBox2.BringToFront();
            pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            ActiveForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            qchange();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            fourbackchange();
        }
    }
}
