using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D
{
    internal class Box2D
    {
        public const double Gap = 0;

        public static GeometryModel3D Make(double x, double y, double width, double height, Brush brush)
        {
            GeometryModel3D geometry = new GeometryModel3D();
            MeshGeometry3D box = new MeshGeometry3D();

            Point3DCollection points = new Point3DCollection();
            points.Add(new Point3D(x + Gap, y + Gap, 0));
            points.Add(new Point3D(x + width - Gap, y + Gap, 0));
            points.Add(new Point3D(x + width - Gap, y + height - Gap, 0));
            points.Add(new Point3D(x + Gap, y + height - Gap, 0));

            Int32Collection indices = new Int32Collection(new Int32[]{
                0,1,2,
                0,2,3 });

            box.Positions = points;
            box.TriangleIndices = indices;

            geometry.Geometry = box;
            geometry.Material = new DiffuseMaterial(brush);
            return geometry;
        }
    }
}
