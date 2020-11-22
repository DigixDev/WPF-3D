using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D
{
    internal class Box3D
    {
        private const double _gap = 0.01;

        public static GeometryModel3D Make(double x, double y, double width, double height, Brush brush)
        {
            GeometryModel3D geometry = new GeometryModel3D();
            MeshGeometry3D box = new MeshGeometry3D();

            Point3DCollection points = new Point3DCollection();
            points.Add(new Point3D(x + _gap, y + _gap, 0));
            points.Add(new Point3D(x + width - _gap, y + _gap, 0));
            points.Add(new Point3D(x + width - _gap, y + height - _gap, 0));
            points.Add(new Point3D(x + _gap, y + height - _gap, 0));

            points.Add(new Point3D(x + _gap, y + _gap, -0.1));
            points.Add(new Point3D(x + width - _gap, y + _gap, -0.1));
            points.Add(new Point3D(x + width - _gap, y + height - _gap, -0.1));
            points.Add(new Point3D(x + _gap, y + height - _gap, -0.1));

            Int32Collection indices = new Int32Collection(new Int32[]{
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7
            });

            box.Positions = points;
            box.TriangleIndices = indices;

            geometry.Geometry = box;
            geometry.Material = new DiffuseMaterial(brush);
            return geometry;
        }
    }
}
