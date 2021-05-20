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

namespace PZ3.Classes
{
    public class Drawer
    {
        private static double _longitudeMin = 45.2325;
        private static double _longitudeMax = 45.277031;

        private static double _latitudeMin = 19.793909;
        private static double _latitudeMax = 19.894459;

        private static double _objectSize = 0.005;

        Model3DGroup _display;

        public Drawer(Model3DGroup display)
        {
            _display = display;
        }

        public void DrawPowerEntities(Dictionary<long, PowerEntity> entities) 
        {
            foreach(var entity in entities.Values)
            {
                if (entity.TranslatedX < _longitudeMin || entity.TranslatedX > _longitudeMax || entity.TranslatedY < _latitudeMin || entity.TranslatedY > _latitudeMax)
                    continue;

                double x = (entity.X - _longitudeMin) / (_longitudeMax - _longitudeMin); // * TODO velicina slicice da se griduju jedna na drugu
                double y = (entity.Y - _latitudeMin) / (_latitudeMax - _latitudeMin);

                Point point = new Point(x, y);

                Draw(entity, point);

            }
        }

        private void Draw(PowerEntity entity, Point point)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.Content = $"ID: {entity.Id} \n Name: {entity.Name}\n";

            GeometryModel3D obj = new GeometryModel3D();
            obj.Material = new DiffuseMaterial(Brushes.Red);

            var points = new Point3DCollection()
            {
                new Point3D(-point.X , 0, point.Y),
                new Point3D(-(point.X + _objectSize), 0, point.Y),
                new Point3D(-point.X, 0, point.Y + _objectSize),
                new Point3D(-(point.X + _objectSize), 0, point.Y + _objectSize),
                new Point3D(-point.X, _objectSize, point.Y),
                new Point3D(-(point.X + _objectSize), _objectSize, point.Y),
                new Point3D(-(point.X ), _objectSize,  point.Y + _objectSize),
                new Point3D(-(point.X + _objectSize), _objectSize,  point.Y + _objectSize),
            };

            var indicies = new Int32Collection()
            {
                2,3,1,  2,1,0,  7,1,3,  7,5,1,  6,5,7,  6,4,5,  6,2,4,  2,0,4,  2,7,3,  2,6,7,  0,1,5,  0,5,4
            };

            var normals = new Vector3DCollection()
            {
                new Vector3D(0,0,0), new Vector3D(1,0,0),  new Vector3D(1,1,0),  new Vector3D(0,1,0), new Vector3D(0,0,-1), new Vector3D(1,0,-1),new Vector3D(1,1,-1),new Vector3D(0,1,1)
            };

            obj.Geometry = new MeshGeometry3D() { Positions = points, TriangleIndices = indicies };
            //obj.SetValue(IsToolTip, toolTip);
            _display.Children.Add(obj);
        }
    }
}
