using PZ2.Model;
using PZ3.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PZ3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Importer _importer;
        Drawer _drawer;

        private bool middleMouseDown = false;
        private Point middleClickPoint;
        private Point start = new Point();
        private Point diffOffset = new Point();

        private static int zoomMax = 30;
        private static double zoomMin = 5;
        private static int zoomCurent = 1;
        private static double _rotateOffset = 0.5;

        private GeometryModel3D hitgeo;
        private ArrayList models = new ArrayList();

        public MainWindow()
        {
            InitializeComponent();
            _importer = new Importer();
            _importer.LoadModel();

            _drawer = new Drawer(Map);
            models.AddRange(_drawer.DrawPowerEntities(_importer.PowerGrid.PowerEntities));
            models.AddRange(_drawer.DrawLines(_importer.PowerGrid.LineEntities));
        }


        private void viewport1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewPortDisplay.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = trasnlation.OffsetX;
            diffOffset.Y = trasnlation.OffsetY;
        }

        private void viewport1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewPortDisplay.ReleaseMouseCapture();
        }

        private void viewport1_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewPortDisplay.IsMouseCaptured && !middleMouseDown)
            {
                Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = -(offsetX * 100) / w;
                double translateY = +(offsetY * 100) / h;
                trasnlation.OffsetX = diffOffset.X + (translateX / (100 * scale.ScaleX));
                trasnlation.OffsetY = diffOffset.Y + (translateY / (100 * scale.ScaleX));

            }

            if (middleMouseDown)
            {
                viewPortDisplay.CaptureMouse();
                Point mouse = e.GetPosition(this);
                double diffX = mouse.X - middleClickPoint.X;
                double diffY = mouse.Y - middleClickPoint.Y;

                diffX *= -1;
                diffY *= -1;
                //double diffX = mouse.X - viewPortDisplay.X;
                //double diffY = mouse.Y - middleClickPoint.Y;



                rotation.Axis = new Vector3D(diffY, diffX, 0);
                rotation.Angle = Math.Sqrt(diffX * diffX + diffY * diffY) * _rotateOffset;


                Console.WriteLine("X" + diffX + " Y" + diffY);
                
            }
        }


        private void viewport1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(this);
            double scaleX = 1;
            double scaleY = 1;
            if (e.Delta > 0 && zoomCurent < zoomMax)
            {
                scaleX = scale.ScaleX + 0.1;
                scaleY = scale.ScaleY + 0.1;
                zoomCurent++;
                scale.ScaleX = scaleX;
                scale.ScaleY = scaleY;
            }
            else if (e.Delta <= 0 && zoomCurent > -zoomMin)
            {
                scaleX = scale.ScaleX - 0.1;
                scaleY = scale.ScaleY - 0.1;
                zoomCurent--;
                scale.ScaleX = scaleX;
                scale.ScaleY = scaleY;
            }
        }


       

        private void viewPortDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Middle)
            {
                middleMouseDown = true;
                middleClickPoint = e.GetPosition(this);
                Console.WriteLine(middleMouseDown);
            }


            //hit testing

            Point mouseposition = e.GetPosition(this);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D     
            hitgeo = null;
            VisualTreeHelper.HitTest(viewPortDisplay, null, HTResult, pointparams);
        }

        private void viewPortDisplay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                middleMouseDown = false;
                viewPortDisplay.ReleaseMouseCapture();
                Console.WriteLine(middleMouseDown);
            }
        }

        private void viewPortDisplay_MouseLeave(object sender, MouseEventArgs e)
        {
            middleMouseDown = false;
        }

        private HitTestResultBehavior HTResult(HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                bool gasit = false;
                for (int i = 0; i < models.Count; i++)
                {
                    if ((GeometryModel3D)models[i] == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                    }
                    else
                    {
                        
                    }
                }
                if (!gasit)
                {
                    hitgeo = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }
    }
}
