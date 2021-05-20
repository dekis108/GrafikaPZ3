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
        private static double _latitudeMin = 45.2325;
        private static double _latitudeMax = 45.277031;

        private static double _longitudeMin = 19.793909;
        private static double _longitudeMax = 19.894459;

        private static double _objectSize = 0.008;

        Model3DGroup _map;

        public Drawer(Model3DGroup map)
        {
            _map = map;
        }

        public void DrawPowerEntities(Dictionary<long, PowerEntity> entities) 
        {
            foreach(var entity in entities.Values)
            {
                if (entity.TranslatedY < _longitudeMin || entity.TranslatedY > _longitudeMax || entity.TranslatedX < _latitudeMin || entity.TranslatedX > _latitudeMax)
                    continue;

                //int tempx = (int)(entity.TranslatedX*1000);
                //entity.TranslatedX = (double)tempx/ 1000; 

                /*
                double x = (entity.TranslatedX - _latitudeMin) / (_latitudeMax - _latitudeMin) * (1 - _objectSize); 
                double y = (entity.TranslatedY - _longitudeMin) / (_longitudeMax - _longitudeMin) * (1 - _objectSize);
                */

                double x, y;
                ScaleToMap(entity.TranslatedX, entity.TranslatedY, out x, out y);

                Point point = new Point(y, x);

                Draw(entity, point);

            }
        }

        private void ScaleToMap(double x, double y, out double outX, out double outY)
        {
            outX = (x - _latitudeMin) / (_latitudeMax - _latitudeMin) * (1 - _objectSize);
            outY = (y - _longitudeMin) / (_longitudeMax - _longitudeMin) * (1 - _objectSize);
        }

        public void DrawLines(Dictionary<long, LineEntity> lines)
        {
            foreach(LineEntity line in lines.Values)
            {
                
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

            var normals = new Vector3DCollection()
            {
                new Vector3D(0,0,0), new Vector3D(1,0,0),  new Vector3D(1,1,0), new Vector3D(0,1,0), new Vector3D(0,0,-1), new Vector3D(1,0,-1),
                new Vector3D(1,1,-1), new Vector3D(0,1,1), new Vector3D(0,0,2), new Vector3D(0,0,-2)
            };

            obj.Geometry = new MeshGeometry3D() { Positions = points, TriangleIndices = indicies};
            //obj.SetValue(IsToolTip, toolTip);
            _map.Children.Add(obj);
        }
    }
}
