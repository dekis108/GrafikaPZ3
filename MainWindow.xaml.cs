using PZ3.Classes;
using System;
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
        private Point start = new Point();
        private Point diffOffset = new Point();
        private int zoomMax = 30;
        private int zoomCurent = 1;
        private bool middleMouseDown = false;
        private Point middleClickPoint;

        public MainWindow()
        {
            InitializeComponent();
            _importer = new Importer();
            //viewPortDisplay.RotateGesture = new MouseGesture(MouseAction.MiddleClick);
            //_importer.LoadModel();
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
            if (viewPortDisplay.IsMouseCaptured)
            {
                Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = (offsetX * 100) / w;
                double translateY = -(offsetY * 100) / h;
                trasnlation.OffsetX = diffOffset.X + (translateX / (100 * scale.ScaleX));
                trasnlation.OffsetY = diffOffset.Y + (translateY / (100 * scale.ScaleX));

            }

            if (middleMouseDown)
            {
                Point mouse = e.GetPosition(this);
                double diffX = mouse.X - middleClickPoint.X;
                double diffY = mouse.Y - middleClickPoint.Y;

                rotation.Axis = new Vector3D(diffY, diffX, 0);
                rotation.Angle = Math.Sqrt(diffX * diffX + diffY * diffY) * 0.3;


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
            else if (e.Delta <= 0 && zoomCurent > -zoomMax)
            {
                scaleX = scale.ScaleX - 0.1;
                scaleY = scale.ScaleY - 0.1;
                zoomCurent--;
                scale.ScaleX = scaleX;
                scale.ScaleY = scaleY;
            }
        }


        private void AjustRoration(double offsetX, double offsetY)
        {
            if ((Math.Abs(offsetX) > Math.Abs(offsetY) && offsetX < 0))
            {
                rotation.Axis = new Vector3D(1, 0, 1);
                rotation.Angle = rotation.Angle + 0.2;
            }
            if ((Math.Abs(offsetX) > Math.Abs(offsetY) && offsetX > 0))
            {
                rotation.Axis = new Vector3D(1, 1, 1);
                rotation.Angle = rotation.Angle - 0.2;
            }
            if ((Math.Abs(offsetX) < Math.Abs(offsetY) && offsetY > 0))
            {
                if (rotation.Angle < 90)
                {
                    rotation.Axis = new Vector3D(1, 0, 1);
                    rotation.Angle = rotation.Angle + 0.2;
                }
            }
            if ((Math.Abs(offsetX) < Math.Abs(offsetY) && offsetY < 0))
            {
                if (rotation.Angle >= 0)
                {
                    rotation.Axis = new Vector3D(1, 1, 1);
                    rotation.Angle = rotation.Angle - 0.2;
                }
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
        }

        private void viewPortDisplay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                middleMouseDown = false;
                Console.WriteLine(middleMouseDown);
            }
        }

        private void viewPortDisplay_MouseLeave(object sender, MouseEventArgs e)
        {
            middleMouseDown = false;
        }


    }
}
