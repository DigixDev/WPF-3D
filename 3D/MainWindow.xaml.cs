using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace _3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<GeometryModel3D> GeometryList;
        private PerspectiveCamera _pCamera;
        private const double _dGap = 0.01;
        private DirectionalLight _light;
        private double _dZ;
        private DoubleAnimation _anHAnimation, _anVAnimation;

        public MainWindow()
        {
            InitializeComponent();
            InitAnimations();
            BuildBoxes();
        }

        private void InitAnimations()
        {
            _anHAnimation = new DoubleAnimation();
            _anHAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _anVAnimation = new DoubleAnimation();
            _anVAnimation.Duration = TimeSpan.FromMilliseconds(500);
        }

        private void BuildBoxes()
        {
            GeometryList = new List<GeometryModel3D>();

            _pCamera = new PerspectiveCamera(new Point3D(0, 0, _dZ), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
            _pCamera.NearPlaneDistance = 0;
            _pCamera.FarPlaneDistance = 200;

            _light = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1));

            group.Children.Add(Box2D.Make(-2, -2, 4, 4, Brushes.Cyan));
            group.Children.Add(_light);

            viewport.Camera = _pCamera;
            MoveCamera();
            viewport.UpdateLayout();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            RotateScene(45);
        }

        private void RotateScene( double angle)
        {
            var dy = sdHor.Value;
            var dx = sdVer.Value;

            var mouseAngle = Math.Asin(dy / Math.Sqrt(Math.Pow(Math.Abs(dx), 2) + Math.Pow(Math.Abs(dy), 2)));

            if (dx < 0)
                mouseAngle = Math.PI - mouseAngle;

            var axisAngle = mouseAngle + Math.PI ;

            var axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

            var rotation = 0.01 * Math.Sqrt(Math.Pow(Math.Abs(dx), 2) + Math.Pow(Math.Abs(dy), 2));

            QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
            _pCamera.Transform = new RotateTransform3D(r);
            viewport.Camera = _pCamera;
        }

        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos=e.GetPosition(viewport);
            PointHitTestParameters pointparams = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewport, null, HTResult, pointparams);
        }

        private HitTestResultBehavior HTResult(HitTestResult result)
        {
            var isHorizantal = true;
            RayHitTestResult rayResult = result as RayHitTestResult;
            ApplyRotation(rayResult.PointHit);
            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
                if (rayMeshResult != null)
                {
                    GeometryModel3D hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
                    MeshGeometry3D geom = rayMeshResult.MeshHit;
                    isHorizantal = IsHorizantal(rayResult.PointHit, geom.Bounds);
                    Model3DGroup group = ((Model3DGroup)((System.Windows.Media.Media3D.ModelVisual3D)viewport.Children[0]).Content);
                    ReproductionBox(group, hitgeo, rayResult.PointHit, isHorizantal);
                }
            }
            return HitTestResultBehavior.Continue;
        }

        private void ApplyRotation(Point3D pointHit)
        {
            var angle = 30.0;
            _anHAnimation.From = _anHAnimation.To;

            if (pointHit.Y < 0)
                _anHAnimation.To = -angle;
            else if(pointHit.Y > 0)
                _anHAnimation.To = angle;
            if (_anHAnimation.To != _anHAnimation.From)
                sdVer.BeginAnimation(Slider.ValueProperty, _anHAnimation);

            _anVAnimation.From = _anVAnimation.To;

            if (pointHit.X < 0)
                _anVAnimation.To = angle;
            else if (pointHit.X > 0)
                _anVAnimation.To = -angle;
            if (_anVAnimation.To != _anVAnimation.From)
                sdHor.BeginAnimation(Slider.ValueProperty, _anVAnimation);

        }

        private bool IsHorizantal(Point3D pointHit, Rect3D bounds)
        {
            var hDist = Math.Min(Math.Abs(pointHit.X - bounds.X), Math.Abs(bounds.SizeX - pointHit.X + bounds.X));
            var VDist = Math.Min(Math.Abs(pointHit.Y - bounds.Y), Math.Abs(bounds.SizeY - pointHit.Y + bounds.Y));
            return (hDist < VDist);
        }

        private void ReproductionBox(Model3DGroup group, GeometryModel3D item,Point3D pointHit, bool horizantal)
        {
            var x = item.Bounds.X ;
            var y = item.Bounds.Y;
            var width = item.Bounds.Size.X;
            var height = item.Bounds.Size.Y;
            group.Children.Remove(item);

            if (horizantal)
            {
                group.Children.Add(Box2D.Make(x, y, width, pointHit.Y - y - _dGap, Brushes.Gold));
                group.Children.Add(Box2D.Make(x, pointHit.Y + _dGap, width, height - (pointHit.Y - y) - _dGap, Brushes.Gold));
            }
            else
            {
                group.Children.Add(Box2D.Make(x, y, pointHit.X-x-_dGap, height, Brushes.Gold));
                group.Children.Add(Box2D.Make(pointHit.X+_dGap, y, width - (pointHit.X - x) - _dGap, height, Brushes.Gold));
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _dZ+=e.Delta/60;
            MoveCamera();
        }

        private void MoveCamera()
        {
            _pCamera.Position = new Point3D(0, 0, _dZ);
        }

        private void sdHor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateScene(e.NewValue);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetAtHome();
        }

        private void sdVer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateScene(e.NewValue);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SetAtHome();
        }

        private void SetAtHome()
        {
            var r = new QuaternionRotation3D(new Quaternion(new Vector3D(0, 0, 1), 0));

            _pCamera.Transform = new RotateTransform3D(r);
            _dZ =10;
            _pCamera.Position = new Point3D(0, 0, _dZ);
        }
    }
}
