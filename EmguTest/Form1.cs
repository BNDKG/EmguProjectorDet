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
        //原始路径
        string OriPath;

        //所有资源路径


        string PathCameraNow;
        string PathBlackCamera ;
        string PathWhiteCamera ;
        string PathWhite ;
        string PathBlack ;
        string PathBlackandWhite;
        string PathEffect ;
        string PathOriSourcePic ;
        string PathQTransfed ;
        string PathFinalEffect ;
        string PathDiffer  ;

        string PathVideoSource ;
        string PathTxtSave;


        //自对对位状态号AutoMatchState
        public int AutoMatchState = 0;
        //手动找点计数器
        public int ClickPosCounter = 0;
        //使用自动方式寻找位置进行变换的摄像头采集原图
        public Bitmap AutoUsedCurBitmap;
        //使用手动方法标记所使用的摄像头采集到的原图
        public Bitmap ManualUsedCurBitmap;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string path = OriPath;

            System.Diagnostics.Process.Start("explorer.exe", OriPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OriPath = System.IO.Directory.GetCurrentDirectory();

            //axWindowsMediaPlayer1.URL = OriPath + "\\testoutput.avi";
            axWindowsMediaPlayer1.settings.autoStart = false;

            axWindowsMediaPlayer1.Ctlcontrols.stop();
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

            string pathread3 = OriPath + "\\FinalEffect.jpg";

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

        private void button5_Click(object sender, EventArgs e)
        {

            //string PathCameraNow = OriPath + "\\CameraPic.jpg";

            Snap(PathCameraNow);



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

        private void button6_Click(object sender, EventArgs e)
        {
            inputoripic();


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("横坐标:" + e.X.ToString() + "\n纵坐标:" + e.Y.ToString());

            if (ClickPosCounter == 0)
            {
                int xxx = (int)(((double)ManualUsedCurBitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)ManualUsedCurBitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox1.Text = xxx.ToString();
                textBox2.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);

            }
            else if (ClickPosCounter == 1)
            {
                int xxx = (int)(((double)ManualUsedCurBitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)ManualUsedCurBitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox3.Text = xxx.ToString();
                textBox4.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);
            }
            else if (ClickPosCounter == 2)
            {
                int xxx = (int)(((double)ManualUsedCurBitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)ManualUsedCurBitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox5.Text = xxx.ToString();
                textBox6.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);
            }
            else if (ClickPosCounter == 3)
            {
                int xxx = (int)(((double)ManualUsedCurBitmap.Width * e.X) / (double)pictureBox1.Width);

                int yyy = (int)(((double)ManualUsedCurBitmap.Height * e.Y) / (double)pictureBox1.Height);

                textBox7.Text = xxx.ToString();
                textBox8.Text = yyy.ToString();

                bitmapupdate(xxx, yyy);

                ClickPosCounter = -1;
            }

            ClickPosCounter++;





        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox2.Dock = System.Windows.Forms.DockStyle.None;
            ActiveForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            qchange();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            fourbackchange();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //axWindowsMediaPlayer1.URL = OriPath + "\\xxxx.avi";
            //axWindowsMediaPlayer1.Ctlcontrols.play();
            //axWindowsMediaPlayer1.Dispose();
            //axWindowsMediaPlayer1.URL = OriPath + "\\other.avi";
            videomake();
            //playerstart();
            axWindowsMediaPlayer1.URL = OriPath + "\\testoutput.avi";
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox2.BringToFront();
            pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            ActiveForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            fullscreen();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            backscreen();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            diffanly();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            picdiffer();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            timer1.Start();


        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Bitmap black = new Bitmap(PathBlack);
            Bitmap blackandwhite = new Bitmap(PathBlackandWhite);
            Bitmap White = new Bitmap(PathWhite);

            if (AutoMatchState == 0)
            {
                pictureBox2.Image = black;
                fullscreen();
                AutoMatchState++;
            }
            else if(AutoMatchState == 1)
            {
                //先拍一张黑的图片
                Snap(PathBlackCamera);

                AutoMatchState++;
            }
            else if (AutoMatchState == 2)
            {
                pictureBox2.Image = blackandwhite;
                AutoMatchState++;
            }
            else if (AutoMatchState == 3)
            {
                //再拍一张黑白的图片
                Snap(PathWhiteCamera);
                pictureBox2.Image = White;
                AutoMatchState++;
            }
            else if (AutoMatchState == 4)
            {
                //最后拍个完整的图片
                Snap(PathCameraNow);
                //backscreen();
                //进行投影差分
                picdiffer();
                //进行差分分析
                diffanly();
                //导入摄像头图像
                inputoripic();
                //转换成摄像头视角
                qchange();
                //匹配
                GetMatch();
                //得到图案
                fourbackchange();


                timer1.Stop();
                AutoMatchState = 0;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {

            Bitmap Normalbitmap = new Bitmap(1920, 1080, PixelFormat.Format24bppRgb);


            //背景图片
            BitmapData curimageData = Normalbitmap.LockBits(new Rectangle(0, 0, Normalbitmap.Width, Normalbitmap.Height),
            ImageLockMode.ReadOnly, Normalbitmap.PixelFormat);


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
                            int delx = Math.Abs(width / 2 - x);
                            int dely = Math.Abs(height / 2 - y);

                            if((delx< (width / 4))&& (dely < (height / 4)))
                            {
                                p[RGB.R] = (byte)255;
                                p[RGB.G] = (byte)255;
                                p[RGB.B] = (byte)255;
                            }
                            else
                            {
                                p[RGB.R] = (byte)255;
                                p[RGB.G] = (byte)255;
                                p[RGB.B] = (byte)255;
                            }

                        }

                    }



                }
                finally
                {
                    Normalbitmap.UnlockBits(curimageData); //Unlock

                }




            }

            Normalbitmap.Save(PathBlackandWhite);






        }

        private void button17_Click(object sender, EventArgs e)
        {
            foundcornors();
        }

        private void textBox17_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            textBox17.Text = path;
        }

        private void textBox18_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            textBox18.Text = path;
        }

        private void textBox17_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void textBox18_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void button18_Click(object sender, EventArgs e)
        {

            // 生成视频生成读取器
            VideoFileReader readerzzz = new VideoFileReader();
            // 打开视频
            readerzzz.Open(textBox18.Text);

            //载入当前帧动画
            Bitmap curbitmapsource = readerzzz.ReadVideoFrame();


            curbitmapsource.Save(PathEffect);

            readerzzz.Dispose();
            curbitmapsource.Dispose();
        }

        private void button19_Click(object sender, EventArgs e)
        {

            Bitmap bufbitmap = new Bitmap(textBox17.Text);

            bufbitmap.Save(PathOriSourcePic);

        }

        private void button20_Click(object sender, EventArgs e)
        {

            //write txt
            StringBuilder builder = new StringBuilder();
            FileStream fs = new FileStream(PathTxtSave, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            builder.AppendLine(textBox1.Text);
            builder.AppendLine(textBox2.Text);
            builder.AppendLine(textBox3.Text);
            builder.AppendLine(textBox4.Text);
            builder.AppendLine(textBox5.Text);
            builder.AppendLine(textBox6.Text);
            builder.AppendLine(textBox7.Text);
            builder.AppendLine(textBox8.Text);

            sw.Write(builder);
            sw.Close();
            fs.Close();


        }

        private void button21_Click(object sender, EventArgs e)
        {

            //read txt
            string[] allLines = File.ReadAllLines(PathTxtSave);

            textBox1.Text = allLines[0];
            textBox2.Text = allLines[1];
            textBox3.Text = allLines[2];
            textBox4.Text = allLines[3];
            textBox5.Text = allLines[4];
            textBox6.Text = allLines[5];
            textBox7.Text = allLines[6];
            textBox8.Text = allLines[7];


        }
        /// <summary>
        /// 从文件中导入摄像头拍摄的照片
        /// </summary>
        public void inputoripic()
        {
            
            Bitmap bufbitmap = new Bitmap(PathCameraNow);

            AutoUsedCurBitmap = new Bitmap(bufbitmap);

            ManualUsedCurBitmap = new Bitmap(bufbitmap);
            

            pictureBox1.Image = ManualUsedCurBitmap;

            bufbitmap.Dispose();
        }
        /// <summary>
        /// 使用emgu库，设定摄像头分辨率为1600x896，并显示到控件中同时保存。
        /// </summary>
        /// <param name="pathsave">保存路径</param>
        private void Snap(string pathsave)
        {


            Mat frame = new Mat();


            /*
            Application.Idle += new EventHandler(delegate (object sender2, EventArgs e2)
            {  // “Idle”处理循环的事件处理过程  
                viewer.Image = capture.QueryFrame(); //在视窗中显示抓取的帧图像  
            });
            */

            //ImageViewer viewer = new ImageViewer(); //创建图像视窗  
            VideoCapture capture = new VideoCapture(); //创建摄像头捕获

            capture.SetCaptureProperty(CapProp.FrameHeight, 900);
            capture.SetCaptureProperty(CapProp.FrameWidth, 1600);
            //capture.ImageGrabbed += ProcessFrame;



            capture.Retrieve(frame, 0);    //接收数据

            Bitmap bufbitmap = new Bitmap(frame.Bitmap);

            pictureBox1.Image = bufbitmap;
            //Image<Bgra, byte> a = frame.ToImage<Bgra, byte>();
            //Bitmap bufbitmap = a.Bitmap;


            frame.Save(pathsave);


            frame.Dispose();
            capture.Dispose();

        }

        private void GetMatch()
        {
            
            Image<Bgra, byte> a = new Image<Bgra, byte>(PathOriSourcePic).Resize(0.4, Inter.Area);  //模板
            Image<Bgra, byte> b = new Image<Bgra, byte>(PathQTransfed).Resize(0.4, Inter.Area);  //待匹配的图像

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


            b.Dispose();
            a.Dispose();


        }
        private void fourbackchange()
        {

            List<IntPoint> corners = new List<IntPoint>();

            corners.Add(new IntPoint(Convert.ToInt32(textBox9.Text), Convert.ToInt32(textBox10.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox11.Text), Convert.ToInt32(textBox12.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox13.Text), Convert.ToInt32(textBox14.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox15.Text), Convert.ToInt32(textBox16.Text)));


            Bitmap a = new Bitmap(PathOriSourcePic);
            //效果图
            Bitmap bufsourceImage = new Bitmap(PathEffect);
            //Bitmap sourceImage = new Bitmap(pathread3);
            Bitmap sourceImage = new Bitmap(bufsourceImage, a.Size);
            //变换图片的
            Bitmap bufbitmap = new Bitmap(PathQTransfed);

            Bitmap image = new Bitmap(bufbitmap.Width, bufbitmap.Height);

            bufbitmap.Dispose();
            a.Dispose();
            // create filter
            BackwardQuadrilateralTransformation filter =
                new BackwardQuadrilateralTransformation(sourceImage, corners);
            // apply the filter
            Bitmap newImage = filter.Apply(image);

            newImage.Save(PathFinalEffect);

            //Bitmap bufimage = new Bitmap(pathread3);

            pictureBox1.Image = newImage;
            pictureBox2.Image = newImage;

        }

        private void videomake()
        {

            Bitmap sourcepic = new Bitmap(PathEffect);

            // 生成视频生成读取器
            VideoFileReader readerzzz = new VideoFileReader();
            // 打开视频
            readerzzz.Open(PathVideoSource);

            //变换图片的大小
            Bitmap bufbitmap = new Bitmap(PathQTransfed);

            // 生成视频写入器
            VideoFileWriter writerzzz = new VideoFileWriter();
            // 新建一个视频(帧必须是二的倍数)
            writerzzz.Open("testoutput.avi", (bufbitmap.Width / 2) * 2, (bufbitmap.Height / 2) * 2, readerzzz.FrameRate, VideoCodec.MPEG4, 15000000);


            //确认变换位置
            List<IntPoint> corners = new List<IntPoint>();

            corners.Add(new IntPoint(Convert.ToInt32(textBox9.Text), Convert.ToInt32(textBox10.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox11.Text), Convert.ToInt32(textBox12.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox13.Text), Convert.ToInt32(textBox14.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox15.Text), Convert.ToInt32(textBox16.Text)));




            // 对视频的所有帧进行操作
            for (int i = 0; i < (readerzzz.FrameCount - 1); i++)
            {
                //载入当前帧动画
                Bitmap curbitmapsource = readerzzz.ReadVideoFrame();

                Bitmap image = new Bitmap(bufbitmap.Width, bufbitmap.Height, curbitmapsource.PixelFormat);

                // create filter
                BackwardQuadrilateralTransformation filter =
                    new BackwardQuadrilateralTransformation(curbitmapsource, corners);
                // apply the filter
                Bitmap newImage = filter.Apply(image);

                //写入当前帧
                writerzzz.WriteVideoFrame(newImage);

                image.Dispose();
                curbitmapsource.Dispose();
                newImage.Dispose();

            }
            writerzzz.Close();
            readerzzz.Close();

            bufbitmap.Dispose();
        }
        public void bitmapupdate(int xxx, int yyy)
        {

            //int xxx = 100;
            //int yyy = 100;

            BitmapData curimageData = ManualUsedCurBitmap.LockBits(new Rectangle(0, 0, ManualUsedCurBitmap.Width, ManualUsedCurBitmap.Height),
            ImageLockMode.ReadOnly, ManualUsedCurBitmap.PixelFormat);

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
                    ManualUsedCurBitmap.UnlockBits(curimageData); //Unlock

                }

            }

            pictureBox1.Image = ManualUsedCurBitmap;


        }


        public void qchange()
        {

            List<IntPoint> corners = new List<IntPoint>();
            corners.Add(new IntPoint(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox4.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text)));
            corners.Add(new IntPoint(Convert.ToInt32(textBox7.Text), Convert.ToInt32(textBox8.Text)));
            // create filter
            QuadrilateralTransformation filter =
                new QuadrilateralTransformation(corners, 1920, 1080);
            // apply the filter
            Bitmap ProjectorPos = filter.Apply(AutoUsedCurBitmap);

            pictureBox2.Image = ProjectorPos;

            ProjectorPos.Save(PathQTransfed);
        }
        private void fullscreen()
        {
            this.SetVisibleCore(false);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.SetVisibleCore(true);

            pictureBox2.BringToFront();
            pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            ActiveForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }
        private void backscreen()
        {
            //this.SetVisibleCore(false);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            //this.SetVisibleCore(true);
            this.WindowState = FormWindowState.Normal;
        }
        public void diffanly()
        {

            Bitmap temp1;
            Bitmap temp2;
            Bitmap temp3;
            Bitmap temp4;


            temp1 = new Bitmap(PathDiffer);
            //灰度转换
            temp2 = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp1);
            temp1.Dispose();

            //二值化
            temp3 = new Threshold(15).Apply(temp2);

            temp4 = new BlobsFiltering(40, 40, temp3.Width, temp3.Height).Apply(temp3);

            Bitmap testbitmap = new Bitmap(temp4);
            // get corners of the quadrilateral
            QuadrilateralFinder qf = new QuadrilateralFinder();
            List<IntPoint> corners = qf.ProcessImage(testbitmap);

            List<IntPoint> corners2 = CornersChange(corners, temp4.Width, temp4.Height);

            PointF aaa = new PointF(corners2[0].X, corners2[0].Y);
            PointF bbb = new PointF(corners2[1].X, corners2[1].Y);
            PointF ccc = new PointF(corners2[2].X, corners2[2].Y);
            PointF ddd = new PointF(corners2[3].X, corners2[3].Y);

            PointF testsdd = GetIntersection(aaa, ccc, bbb, ddd);

            PointF aa = poschange2(aaa, testsdd);
            PointF bb = poschange2(bbb, testsdd);
            PointF cc = poschange2(ccc, testsdd);
            PointF dd = poschange2(ddd, testsdd);

            List<IntPoint> corners3 = new List<IntPoint>();

            IntPoint buff = new IntPoint((int)aa.X, (int)aa.Y);
            corners3.Add(buff);
            buff = new IntPoint((int)bb.X, (int)bb.Y);
            corners3.Add(buff);
            buff = new IntPoint((int)cc.X, (int)cc.Y);
            corners3.Add(buff);
            buff = new IntPoint((int)dd.X, (int)dd.Y);
            corners3.Add(buff);

            textBox1.Text = ((int)aa.X).ToString();
            textBox2.Text = ((int)aa.Y).ToString();
            textBox3.Text = ((int)bb.X).ToString();
            textBox4.Text = ((int)bb.Y).ToString();
            textBox5.Text = ((int)cc.X).ToString();
            textBox6.Text = ((int)cc.Y).ToString();
            textBox7.Text = ((int)dd.X).ToString();
            textBox8.Text = ((int)dd.Y).ToString();


            BitmapData data = testbitmap.LockBits(new Rectangle(0, 0, testbitmap.Width, testbitmap.Height),
    ImageLockMode.ReadWrite, testbitmap.PixelFormat);

            Drawing.Polygon(data, corners3, Color.Red);
            for (int i = 0; i < corners3.Count; i++)
            {
                Drawing.FillRectangle(data,
                    new Rectangle(corners3[i].X - 2, corners3[i].Y - 2, 5, 5),
                    Color.FromArgb(i * 32 + 127 + 32, i * 64, i * 64));
            }

            testbitmap.UnlockBits(data);


            pictureBox1.Image = testbitmap;
        }


        /// <summary>
        /// 四边形角点重新排序改为左上 右上 右下 左下 的顺序
        /// </summary>
        /// <param name="CornersInput">乱序四边型角点集合</param>
        /// <param name="width">四边形角点信息所对应的图片的宽</param>
        /// <param name="height">四边形角点信息所对应的图片的高</param>
        /// <returns></returns>
        public List<IntPoint> CornersChange(List<IntPoint> CornersInput, int width, int height)
        {
            if (CornersInput.Count() != 4)
            {
                return CornersInput;
            }


            //double[] order = new double[4];
            List<IntPoint> CornersOutput = new List<IntPoint>();

            int min = width + height;
            int index = 0;

            for (int i = 0; i < CornersInput.Count(); i++)
            {
                int curbuf = CornersInput[i].X + CornersInput[i].Y;
                if (curbuf < min)
                {
                    min = curbuf;
                    index = i;
                }

            }
            CornersOutput.Add(CornersInput[index]);
            CornersInput.Remove(CornersInput[index]);

            index = 0;
            min = width + height;
            for (int i = 0; i < CornersInput.Count(); i++)
            {
                int curbuf = (width - CornersInput[i].X) + CornersInput[i].Y;
                if (curbuf < min)
                {
                    min = curbuf;
                    index = i;
                }
            }
            CornersOutput.Add(CornersInput[index]);
            CornersInput.Remove(CornersInput[index]);

            index = 0;
            min = width + height;
            for (int i = 0; i < CornersInput.Count(); i++)
            {
                int curbuf = (width - CornersInput[i].X) + (height - CornersInput[i].Y);
                if (curbuf < min)
                {
                    min = curbuf;
                    index = i;
                }
            }
            CornersOutput.Add(CornersInput[index]);
            CornersInput.Remove(CornersInput[index]);

            CornersOutput.Add(CornersInput[0]);


            return CornersOutput;
        }

        private void picdiffer()
        {  

            Bitmap buf = new Bitmap(PathBlackCamera);

            Bitmap Normalbitmap = new Bitmap(buf);

            buf.Dispose();

            buf = new Bitmap(PathWhiteCamera);

            Bitmap Whitebitmap = new Bitmap(buf);


            buf.Dispose();

            //背景图片
            BitmapData curimageData = Normalbitmap.LockBits(new Rectangle(0, 0, Normalbitmap.Width, Normalbitmap.Height),
            ImageLockMode.ReadOnly, Normalbitmap.PixelFormat);

            //灯光图片
            BitmapData curimageData2 = Whitebitmap.LockBits(new Rectangle(0, 0, Whitebitmap.Width, Whitebitmap.Height),
            ImageLockMode.ReadOnly, Whitebitmap.PixelFormat);

            unsafe
            {
                try
                {
                    UnmanagedImage img = new UnmanagedImage(curimageData);

                    int height = img.Height;
                    int width = img.Width;
                    int pixelSize = (img.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    byte* p = (byte*)img.ImageData.ToPointer();

                    UnmanagedImage img2 = new UnmanagedImage(curimageData2);

                    int height2 = img2.Height;
                    int width2 = img2.Width;
                    int pixelSize2 = (img2.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    byte* p2 = (byte*)img2.ImageData.ToPointer();

                    // for each line
                    for (int y = 0; y < height; y++)
                    {

                        // for each pixel
                        for (int x = 0; x < width; x++, p += pixelSize, p2 += pixelSize2)
                        {

                            float rr = Math.Abs(p2[RGB.R] - p[RGB.R]);
                            float gg = Math.Abs(p2[RGB.G] - p[RGB.G]);
                            float bb = Math.Abs(p2[RGB.B] - p[RGB.B]);


                            p[RGB.R] = (byte)rr;
                            p[RGB.G] = (byte)gg;
                            p[RGB.B] = (byte)bb;

                        }

                    }



                }
                finally
                {
                    Normalbitmap.UnlockBits(curimageData); //Unlock
                    Whitebitmap.UnlockBits(curimageData2);
                }




            }

            Normalbitmap.Save(PathDiffer);

            //Normalbitmap.Save(pathsave3);

            Normalbitmap.Dispose();
            Whitebitmap.Dispose();



        }
        public void foundcornors()
        {

            PointF aaa = new PointF(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
            PointF bbb = new PointF(Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox4.Text));
            PointF ccc = new PointF(Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text));
            PointF ddd = new PointF(Convert.ToInt32(textBox7.Text), Convert.ToInt32(textBox8.Text));

            PointF testsdd = GetIntersection(aaa, ccc, bbb, ddd);

            PointF aa = poschange2(aaa, testsdd);
            PointF bb = poschange2(bbb, testsdd);
            PointF cc = poschange2(ccc, testsdd);
            PointF dd = poschange2(ddd, testsdd);


            textBox1.Text = ((int)aa.X).ToString();
            textBox2.Text = ((int)aa.Y).ToString();
            textBox3.Text = ((int)bb.X).ToString();
            textBox4.Text = ((int)bb.Y).ToString();
            textBox5.Text = ((int)cc.X).ToString();
            textBox6.Text = ((int)cc.Y).ToString();
            textBox7.Text = ((int)dd.X).ToString();
            textBox8.Text = ((int)dd.Y).ToString();


            PointF easdad = new PointF(6, 1);
        }
        public PointF poschange2(PointF point, PointF sentpoint)
        {
            PointF changedpoint = new PointF(0, 0);
            changedpoint.X = 2 * point.X - sentpoint.X;
            changedpoint.Y = 2 * point.Y - sentpoint.Y;
            return changedpoint;
        }

        public static PointF GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd)
        {
            /*
             * L1，L2都存在斜率的情况：
             * 直线方程L1: ( y - y1 ) / ( y2 - y1 ) = ( x - x1 ) / ( x2 - x1 ) 
             * => y = [ ( y2 - y1 ) / ( x2 - x1 ) ]( x - x1 ) + y1
             * 令 a = ( y2 - y1 ) / ( x2 - x1 )
             * 有 y = a * x - a * x1 + y1   .........1
             * 直线方程L2: ( y - y3 ) / ( y4 - y3 ) = ( x - x3 ) / ( x4 - x3 )
             * 令 b = ( y4 - y3 ) / ( x4 - x3 )
             * 有 y = b * x - b * x3 + y3 ..........2
             * 
             * 如果 a = b，则两直线平等，否则， 联解方程 1,2，得:
             * x = ( a * x1 - b * x3 - y1 + y3 ) / ( a - b )
             * y = a * x - a * x1 + y1
             * 
             * L1存在斜率, L2平行Y轴的情况：
             * x = x3
             * y = a * x3 - a * x1 + y1
             * 
             * L1 平行Y轴，L2存在斜率的情况：
             * x = x1
             * y = b * x - b * x3 + y3
             * 
             * L1与L2都平行Y轴的情况：
             * 如果 x1 = x3，那么L1与L2重合，否则平等
             * 
            */
            float a = 0, b = 0;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y) / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y) / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1与L2都平行Y轴
                    {
                        if (lineFirstStar.X == lineSecondStar.X)
                        {
                            //throw new Exception("两条直线互相重合，且平行于Y轴，无法计算交点。");
                            return new PointF(0, 0);
                        }
                        else
                        {
                            //throw new Exception("两条直线互相平行，且平行于Y轴，无法计算交点。");
                            return new PointF(0, 0);
                        }
                    }
                case 1: //L1存在斜率, L2平行Y轴
                    {
                        float x = lineSecondStar.X;
                        float y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
                case 2: //L1 平行Y轴，L2存在斜率
                    {
                        float x = lineFirstStar.X;
                        //网上有相似代码的，这一处是错误的。你可以对比case 1 的逻辑 进行分析
                        //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y;
                        float y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new PointF(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            // throw new Exception("两条直线平行或重合，无法计算交点。");
                            return new PointF(0, 0);
                        }
                        float x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        float y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
            }
            // throw new Exception("不可能发生的情况");
            return new PointF(0, 0);
        }

        public void PathInit()
        {
            PathCameraNow = OriPath + "\\CameraPic.jpg";
            PathBlackCamera = OriPath + "\\BlackCameraPic.jpg";
            PathWhiteCamera = OriPath + "\\WhiteCameraPic.jpg";


            PathWhite = OriPath + "\\White.jpg";
            PathBlack = OriPath + "\\Black.jpg";
            PathBlackandWhite = OriPath + "\\BlackandWhite.jpg";
            PathEffect = OriPath + "\\Effect.jpg";

            PathOriSourcePic = OriPath + "\\OriSourcePic.jpg";
            PathQTransfed = OriPath + "\\QuadrilateralTransfedPic.jpg";
            PathFinalEffect = OriPath + "\\FinalEffect.jpg";
            PathDiffer = OriPath + "\\Diffedpic.jpg";

            PathVideoSource = OriPath + "\\SourceVideo.mp4";
            PathTxtSave = OriPath + "\\test.txt";
        }
    }
}
