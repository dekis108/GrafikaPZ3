using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PZ2.Model;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using System.Collections;

namespace PZ3.Classes
{
    public class Drawer
    {
        private static double _latitudeMin = 45.2325;
        private static double _latitudeMax = 45.277031;

        private static double _longitudeMin = 19.793909;
        private static double _longitudeMax = 19.894459;

        private static double _objectSize = 0.006;
        private static double _lineSize = 0.002;

        Model3DGroup _map;

        private Dictionary<long, GeometryModel3D> powerEntities = new Dictionary<long, GeometryModel3D>();
        private List<GeometryModel3D> powerLines = new List<GeometryModel3D>();

        public static readonly DependencyProperty TagDP = DependencyProperty.RegisterAttached("Tag", typeof(string), typeof(GeometryModel3D));

        public Drawer(Model3DGroup map)
        {
            _map = map;
        }

        public Dictionary<long, GeometryModel3D> DrawPowerEntities(Dictionary<long, PowerEntity> entities) 
        {
            foreach(var entity in entities.Values)
            {
                if (entity.TranslatedY < _longitudeMin || entity.TranslatedY > _longitudeMax || entity.TranslatedX < _latitudeMin || entity.TranslatedX > _latitudeMax)
                    continue;

                //int tempx = (int)(entity.TranslatedX*1000);
                //entity.TranslatedX = (double)tempx/ 1000; 

                double x, y;
                ScaleToMap(entity.TranslatedX, entity.TranslatedY, out x, out y);

                Point point = new Point(y, x);

                Draw(entity, point);

            }

            return powerEntities;
        }

        private void ScaleToMap(double x, double y, out double outX, out double outY)
        {
            outX = (x - _latitudeMin) / (_latitudeMax - _latitudeMin) * (1 - _objectSize);
            outY = (y - _longitudeMin) / (_longitudeMax - _longitudeMin) * (1 - _objectSize);
        }

        public List<GeometryModel3D> DrawLines(Dictionary<long, LineEntity> lines)
        {
            double x, y;
            foreach(LineEntity line in lines.Values)
            {
                List<Point> points = new List<Point>();

                foreach (Point vertice in line.Vertices)
                {
                    if (vertice.Y < _longitudeMin || vertice.Y > _longitudeMax || vertice.X < _latitudeMin || vertice.X > _latitudeMax)
                        continue;

                    ScaleToMap(vertice.X, vertice.Y, out x, out y);
                    points.Add(new Point(y, x)); //nzm ne pitaj
                   
                }


                for (int i = 1; i < points.Count; ++i) //draw a line between points[i] and points[i-1]
                {
                    DrawLine(points[i], points[i - 1]);
                }
            }

            return powerLines;
        }

        private void DrawLine(Point start, Point end)
        {
            GeometryModel3D powerLine = new GeometryModel3D();
            powerLine.Material = new DiffuseMaterial(Brushes.Black);

            
            var points = new Point3DCollection()
            {
                new Point3D(start.X - _lineSize/2 - 0.5, start.Y + _lineSize/2 - 0.5, 0),
                new Point3D(start.X - _lineSize/2 - 0.5, start.Y - _lineSize/2 - 0.5, 0),
                new Point3D(end.X + _lineSize/2 - 0.5, end.Y - _lineSize/2 - 0.5, 0),
                new Point3D(end.X + _lineSize/2 - 0.5, end.Y + _lineSize/2 - 0.5, 0),

                new Point3D(start.X - _lineSize/2 - 0.5, start.Y + _lineSize/2 - 0.5, _lineSize),
                new Point3D(start.X - _lineSize/2 - 0.5, start.Y - _lineSize/2 - 0.5, _lineSize),
                new Point3D(end.X + _lineSize/2 - 0.5, end.Y - _lineSize/2 - 0.5, _lineSize),
                new Point3D(end.X + _lineSize/2 - 0.5, end.Y + _lineSize/2 - 0.5, _lineSize),
            };


            var indicies = new Int32Collection()
            {
                2,1,0,  3,2,0,  5,7,4,   5,6,7,  3,0,7, 3,7,6,  0,1,4,  0,4,7,  2,3,5,  3,6,5,  1,2,4,  2,5,4
            };

            powerLine.Geometry = new MeshGeometry3D() { Positions = points, TriangleIndices = indicies };
            _map.Children.Add(powerLine);
            powerLines.Add(powerLine);
        }

        private void Draw(PowerEntity entity, Point point)
        {
            string tag = $"ID: {entity.Id} \nName: {entity.Name}\n";

            GeometryModel3D obj = new GeometryModel3D();

            obj.Material = new DiffuseMaterial(Brushes.Red);
            obj.SetValue(TagDP, tag);

            var points = new Point3DCollection()
            {
                new Point3D(point.X - _objectSize/2 - 0.5, point.Y + _objectSize/2 - 0.5, 0),
                new Point3D(point.X - _objectSize/2 - 0.5, point.Y - _objectSize/2 - 0.5, 0),
                new Point3D(point.X + _objectSize/2 - 0.5, point.Y - _objectSize/2 - 0.5, 0),
                new Point3D(point.X + _objectSize/2 - 0.5, point.Y + _objectSize/2 - 0.5, 0),

                new Point3D(point.X - _objectSize/2 - 0.5, point.Y + _objectSize/2 - 0.5, _objectSize),
                new Point3D(point.X - _objectSize/2 - 0.5, point.Y - _objectSize/2 - 0.5, _objectSize),
                new Point3D(point.X + _objectSize/2 - 0.5, point.Y - _objectSize/2 - 0.5, _objectSize),
                new Point3D(point.X + _objectSize/2 - 0.5, point.Y + _objectSize/2 - 0.5, _objectSize),
            };

            var indicies = new Int32Collection()
            {
                2,1,0,  3,2,0,  5,7,4,   5,6,7,  3,0,7, 3,7,6,  0,1,4,  0,4,7,  2,3,5,  3,6,5,  1,2,4,  2,5,4
            };


            obj.Geometry = new MeshGeometry3D() { Positions = points, TriangleIndices = indicies};
            //obj.SetValue(IsToolTip, toolTip);
            _map.Children.Add(obj);
            powerEntities.Add(entity.Id,  obj);
        }


    }
}
