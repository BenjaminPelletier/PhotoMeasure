using OpenTK;
using OpenTK.Graphics.OpenGL;
using Easy3D.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Easy3D.Ui.GLUtils;
using Easy3D.Ply;

namespace Easy3D.Ui
{
    /// <summary>
    /// Manages a camera view for a GLControl surface based on azimuth and elevation.
    /// The world space is made up of an XY ground plane so that +Z is up.  +X is east
    /// and +Y is north.  The azimuth of the camera location is tracked, as well
    /// as the elevation of the camera location.  Azimuth is radians east of north,
    /// so azimuth=0 means the camera is located over or below the +Y axis and azimuth=PI/2 means the
    /// camera is located over or below +X.  Elevation is radians above the horizon, so
    /// elevation=0 means the camera's optic axis is contained in the XY plane and
    /// elevation=PI/2 means the camera is on the +Z axis.
    /// The camera's horizontal image axis is always parallel to the ground (XY plane).
    /// </summary>
    public partial class InteractiveGLControl : OpenTK.GLControl
    {
        private const double FOCAL_PLANE_DISTANCE = 1e-6;

        private bool _GLLoaded = false;

        private const double DEG2RAD = Math.PI / 180;

        #region Mouse interactions

        private Rayd _InitialMouseRay;
        private PointF _InitialMousePosition;
        private PointF _LastMousePosition;
        private const float MIN_DRAG_DISTANCE = 5;
        private bool _Dragging;
        private const double CAMERA_ROTATION_RATE = 1 * Math.PI / 180; // Radians per pixel
        private const double CAMERA_ZOOM_RATE = 0.3; // Fraction of distance per click

        #endregion

        #region Keyboard interactions

        private DateTime _LastMovement;
        public double MovementSpeed;
        static readonly Keys[] STRAFE_LEFT_KEY = new Keys[] { Keys.A, Keys.NumPad4, Keys.Left };
        static readonly Keys[] STRAFE_RIGHT_KEY = new Keys[] { Keys.D, Keys.NumPad6, Keys.Right };
        static readonly Keys[] MOVE_FORWARD_KEY = new Keys[] { Keys.W, Keys.NumPad8, Keys.Up };
        static readonly Keys[] MOVE_BACKWARD_KEY = new Keys[] { Keys.S, Keys.NumPad2, Keys.Down };
        static readonly Keys[] MOVE_UP_KEY = new Keys[] { Keys.Q, Keys.NumPad7, Keys.PageUp };
        static readonly Keys[] MOVE_DOWN_KEY = new Keys[] { Keys.E, Keys.NumPad9, Keys.PageDown };
        static readonly Keys[][] MOVEMENTS = new Keys[][] { STRAFE_LEFT_KEY, STRAFE_RIGHT_KEY, MOVE_FORWARD_KEY, MOVE_BACKWARD_KEY, MOVE_UP_KEY, MOVE_DOWN_KEY };
        private Dictionary<Keys, bool> _MovementActive = new Dictionary<Keys, bool>();
        private Timer tKeyboardMovement;

        #endregion

        #region Defining camera characteristics

        /// <summary>
        /// Defines the field of view of the camera (radians vertical)
        /// </summary>
        private double _FieldOfViewVertical;

        private double _Az;

        private double _El;

        /// <summary>
        /// Location of the focal point of the camera
        /// </summary>
        private Vector3d _PointAt;

        /// <summary>
        /// How far away from the focal point the camera is located
        /// </summary>
        private double _Distance = 1;

        #endregion

        #region Derived camera characteristics

        private Matrix4d _CameraMatrix;
        private Matrix4d _CameraMatrixInverse;
        private Matrix4d _PerspectiveMatrix;

        /// <summary>
        /// Transforms world coordinates relative to the camera's location into the frame where +x is right on the monitor, +y is up on the monitor, and -z is into the monitor (away from viewer)
        /// </summary>
        private Matrix4d _OrientationMatrix;

        #endregion

        #region Orientation cue information

        private const double CUE_SIZE = 0.1; // Fraction of vertical view size
        private Mesh _OrientationCue;
        private Matrix4d _CueNormalize;
        private Matrix4d _CueProjection;
        private Matrix4d _CueMatrix;

        #endregion

        public new event EventHandler<PaintEventArgs> Paint;
        public event EventHandler<MouseDragEventArgs> MouseDrag;
        public new event EventHandler<MouseEventArgs> MouseMove;
        public new event EventHandler<GLMouseUpEventArgs> MouseUp;

        public InteractiveGLControl()
            : base(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8))
        {
            _FieldOfViewVertical = 60 * Math.PI / 180;
            SetAzEl(Math.PI, 0);

            //InitializeComponent();
            tKeyboardMovement = new Timer();
            tKeyboardMovement.Interval = 1000;
            tKeyboardMovement.Tick += tKeyboardMovement_Tick;

            base.Paint += GLControl_Paint;
            base.Resize += GLControl_Resize;
            base.MouseDown += GLControl_MouseDown;
            base.MouseMove += GLControl_MouseMove;
            base.MouseUp += GLControl_MouseUp;
            base.MouseWheel += GLControl_MouseWheel;
            base.KeyDown += GLControl_KeyDown;
            base.KeyUp += GLControl_KeyUp;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.Site != null && this.Site.DesignMode)
                return; //Don't initialize at design time

            _OrientationCue = ObjectLibrary.GetSegmentedCube();
            Vector3 cueCenter = _OrientationCue.ComputeCenter();
            double cueExtent = _OrientationCue.ComputeExtent(cueCenter);
            _CueNormalize =
                Matrix4d.CreateTranslation(-(Vector3d)cueCenter) *
                Matrix4d.Scale(CUE_SIZE / cueExtent);

            RecomputePerspectiveMatrix();
            RecomputeCameraCharacteristics();
            GL.Enable(EnableCap.DepthTest);
            GL.LineWidth(2f);
            //GL.DepthMask(True)
            //GL.DepthFunc(DepthFunction.Less)

            _GLLoaded = true;
        }

        /// <summary>
        /// Must be invoked any time the camera optics change
        /// </summary>
        private void RecomputePerspectiveMatrix()
        {
            double aspectRatio = (double)this.Width / (double)this.Height;
            _PerspectiveMatrix = Matrix4d.CreatePerspectiveFieldOfView(_FieldOfViewVertical, aspectRatio, 0.1, 1000);

            _CueProjection = Matrix4d.CreateTranslation(-aspectRatio + CUE_SIZE, 1 - CUE_SIZE, 0) *
                Matrix4d.CreateOrthographic(2 * aspectRatio, 2, -1, 1);
        }

        /// <summary>
        /// Must be invoked any time any aspect of the camera changes (position, orientation, optics)
        /// </summary>
        private void RecomputeCameraCharacteristics()
        {
            _OrientationMatrix = Matrix4d.CreateRotationZ(_Az + Math.PI) * Matrix4d.CreateRotationX(_El - Math.PI / 2);
            _CueMatrix = _CueNormalize * _OrientationMatrix * _CueProjection;
            _CameraMatrix = Matrix4d.CreateTranslation(-CameraLocation) * _OrientationMatrix * _PerspectiveMatrix;
            _CameraMatrixInverse = _CameraMatrix.Inverted();
        }

        #region Camera manipulations

        public void ZoomAll(IEnumerable<Vector3d> pts)
        {
            throw new NotImplementedException();

            // Compute frustum bounding plane normals
            double fieldOfViewHorizontal = _FieldOfViewVertical * (double)this.Width / (double)this.Height;
            Vector3d[] fn = new Vector3d[] {
                new Vector3d(0, Math.Cos(_FieldOfViewVertical / 2), Math.Sin(_FieldOfViewVertical / 2)), // top
                new Vector3d(0, -Math.Cos(_FieldOfViewVertical / 2), Math.Sin(_FieldOfViewVertical / 2)), // bottom
                new Vector3d(Math.Cos(fieldOfViewHorizontal / 2), 0, Math.Sin(fieldOfViewHorizontal / 2)), // right
                new Vector3d(-Math.Cos(fieldOfViewHorizontal / 2), 0, Math.Sin(fieldOfViewHorizontal / 2)), // left
            };
            fn = fn.Select(n => Vector3d.Transform(n, _PerspectiveMatrix.Inverted() * _OrientationMatrix.Inverted())).ToArray();

            // Get the camera's optic axis ray
            var r = new Rayd(_PointAt, Vector3d.Transform(-Vector3d.UnitZ, _OrientationMatrix.Inverted()));

            // Determine the most-forward point on the camera's optic axis that still bounds all provided points within the frustum planes
            double kMin = double.PositiveInfinity;
            foreach (Vector3d pt in pts)
            {
                foreach (Vector3d n in fn)
                {
                    double k = r.DistanceTo(new Planed(pt, n));
                    if (k < kMin)
                        kMin = k;
                }
            }

            //_CameraLocation = r.PointAt(kMin);
            RecomputeCameraCharacteristics();
        }

        /// <summary>
        /// Change the orientation of the camera to a new azimuth and elevation
        /// </summary>
        /// <param name="az">Where the camera should be located, radians in the +x direction from +y in the xy plane</param>
        /// <param name="el">Where the camera should be located, radians in the +z direction from the xy plane</param>
        public void SetAzEl(double az, double el)
        {
            _Az = az;
            _El = el;

            RecomputeCameraCharacteristics();
            this.Invalidate();
        }

        /// <summary>
        /// Move the camera such that it's pointing at the specified location while being the specified distance away from that point
        /// </summary>
        private void SetTargetDistance(Vector3d pointAt, double distance)
        {
            _PointAt = pointAt;
            _Distance = distance;

            RecomputeCameraCharacteristics();
            this.Invalidate();
        }

        private void SetAzElDistance(double az, double el, Vector3d pointAt, double distance)
        {
            _Az = az;
            _El = el;
            _PointAt = pointAt;
            _Distance = distance;

            RecomputeCameraCharacteristics();
            this.Invalidate();
        }

        #endregion

        #region Public accessors

        public double Az { get { return _Az; } }

        public double El { get { return _Az; } }

        public Matrix4d CameraMatrix
        {
            get
            {
                if (!_GLLoaded)
                    return Matrix4d.Identity;
                else
                    return _CameraMatrix;
            }
        }

        public Vector3d CameraLocation
        {
            get
            {
                if (!_GLLoaded)
                    return Vector3d.Zero;
                else
                    return _PointAt + _Distance * new Vector3d(Math.Sin(_Az) * Math.Cos(_El), Math.Cos(_Az) * Math.Cos(_El), Math.Sin(_El));
            }
        }

        public Matrix4d OrientationMatrix
        {
            get
            {
                if (!_GLLoaded)
                    return Matrix4d.Identity;
                else
                    return _OrientationMatrix;
            }
        }

        public double FieldOfViewVertical
        {
            get { return _FieldOfViewVertical; }
            set
            {
                _FieldOfViewVertical = value;
                RecomputePerspectiveMatrix();
                RecomputeCameraCharacteristics();
                this.Invalidate();
            }
        }

        #endregion

        public void BeginSelection(object selectedObject)
        {
            //mSelectedObject = selectedObject
        }

        public PointF ScreenPointOf(Vector3d p)
        {
            Vector3d uv = Vector3d.TransformPerspective(p, CameraMatrix);
            return new PointF((float)(0.5 * (uv.X + 1) * (this.Width - 1)), (float)(0.5 * (1 - uv.Y) * (this.Height - 1)));
        }

        public Rayd GetPickRay(double u, double v)
        {
            Vector3d p = Vector3d.TransformPerspective(new Vector3d(u / (this.Width - 1) * 2 - 1, 1 - v / (this.Height - 1) * 2, -1), _CameraMatrixInverse);
            Vector3d d = p - CameraLocation;
            d.Normalize();
            return new Rayd(CameraLocation, d);
        }

        #region Mouse interactions

        private static float DistanceBetween(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void GLControl_MouseDown(object sender, MouseEventArgs e)
        {
            _InitialMouseRay = GetPickRay(e.X, e.Y);
            _InitialMousePosition = e.Location;
            _LastMousePosition = e.Location;
            _Dragging = false;
        }

        private void GLControl_MouseMove(object sender, MouseEventArgs e)
        {
            bool handled = false;

            if (_Dragging || DistanceBetween(e.Location, _InitialMousePosition) >= MIN_DRAG_DISTANCE)
            {
                _Dragging = true;

                if (e.Button == MouseButtons.Left)
                {
                    // Rotate the view by moving the camera around the target
                    _El += (e.Y - _LastMousePosition.Y) * CAMERA_ROTATION_RATE;
                    if (_El > Math.PI / 2)
                        _El = Math.PI / 2;
                    if (_El < -Math.PI / 2)
                        _El = -Math.PI / 2;
                    _Az += ((e.X - _LastMousePosition.X) * CAMERA_ROTATION_RATE) % (2 * Math.PI);
                    _LastMousePosition = e.Location;
                    SetAzEl(_Az, _El);
                    this.Invalidate();
                    handled = true;
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    // Pan the view by moving the camera and target
                    Rayd r0 = GetPickRay(_LastMousePosition.X, _LastMousePosition.Y);
                    Rayd r1 = GetPickRay(e.X, e.Y);
                    Planed targetPlane = new Planed(_PointAt, CameraLocation - _PointAt);
                    Vector3d p0 = r0.IntersectionWith(targetPlane);
                    Vector3d p1 = r1.IntersectionWith(targetPlane);
                    Vector3d dp = p0 - p1;
                    _PointAt += dp;
                    _LastMousePosition = e.Location;
                    RecomputeCameraCharacteristics();
                    this.Invalidate();
                    handled = true;
                }
            }

            if (!handled && MouseMove != null)
            {
                MouseMove(this, e);
            }
        }

        private void GLControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null)
            {
                MouseUp(sender, new GLMouseUpEventArgs(e, _InitialMouseRay, _Dragging));
            }
        }

        private void GLControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _Distance *= Math.Pow(1.1, e.Delta / 120.0);
            RecomputeCameraCharacteristics();
            this.Invalidate();
        }

        #endregion

        private void GLControl_Resize(object sender, System.EventArgs e)
        {
            if (!_GLLoaded)
                return;

            GL.Viewport(0, 0, this.Width, this.Height);
            RecomputePerspectiveMatrix();
            RecomputeCameraCharacteristics();
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            if (!_GLLoaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);

            // Draw user objects
            if (Paint != null)
            {
                GL.LoadMatrix(ref _CameraMatrix);
                Paint(sender, e);
                GL.PopMatrix();
            }

            // Draw 3D orientation cue
            GL.LoadMatrix(ref _CueMatrix);
            _OrientationCue.Draw();
            GL.PopMatrix();

            // Draw azimuth & elevation indicators
            double width = (double)this.Width / (double)this.Height;
            Matrix4d indicatorMat =
                Matrix4d.Scale(CUE_SIZE / 2) *
                Matrix4d.CreateTranslation(-0.5 * width + 1.5 * CUE_SIZE, 0.5 - CUE_SIZE / 2, 0) *
                Matrix4d.CreateOrthographic(width, 1, -1, 1);
            GL.LoadMatrix(ref indicatorMat);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(Color.Gray);
            int n = 16;
            double theta = 0;
            double dTheta = 2 * Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                GL.Vertex3(Math.Cos(theta), Math.Sin(theta), 0);
                theta += dTheta;
            }
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            const double AZ_INDICATOR_LENGTH = 0.3;
            const double EL_INDICATOR_LENGTH = 0.8;
            GL.Vertex3(Math.Sin(_Az), Math.Cos(_Az), 0);
            GL.Vertex3((1 - AZ_INDICATOR_LENGTH) * Math.Sin(_Az), (1 - AZ_INDICATOR_LENGTH) * Math.Cos(_Az), 0);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-0.5 * EL_INDICATOR_LENGTH, 0, 0);
            GL.Vertex3(0.5 * EL_INDICATOR_LENGTH, 0, 0);
            GL.Vertex3(-0.5 * EL_INDICATOR_LENGTH, 0, 0);
            GL.Vertex3(-0.5 * EL_INDICATOR_LENGTH + EL_INDICATOR_LENGTH * Math.Cos(_El), EL_INDICATOR_LENGTH * Math.Sin(_El), 0);
            GL.End();
            GL.PopMatrix();

            base.SwapBuffers();
        }

        private void GLControl_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (Keys[] movement in MOVEMENTS)
            {
                foreach (Keys k in movement)
                {
                    if (k == e.KeyCode)
                    {
                        lock (_MovementActive)
                        {
                            _MovementActive[movement[0]] = true;
                            if (_MovementActive.Any(kvp => kvp.Value) && !tKeyboardMovement.Enabled)
                            {
                                _LastMovement = DateTime.UtcNow - TimeSpan.FromMilliseconds(tKeyboardMovement.Interval);
                                tKeyboardMovement_Tick(sender, e);
                                tKeyboardMovement.Start();
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void GLControl_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Keys[] movement in MOVEMENTS)
            {
                foreach (Keys k in movement)
                {
                    if (k == e.KeyCode)
                    {
                        lock (_MovementActive)
                        {
                            _MovementActive[movement[0]] = false;
                            if (!_MovementActive.Any(kvp => kvp.Value) && tKeyboardMovement.Enabled)
                                tKeyboardMovement.Stop();
                        }
                        return;
                    }
                }
            }
        }

        private void tKeyboardMovement_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.UtcNow;
            double dt = (now - _LastMovement).TotalSeconds;
            _LastMovement = now;

            Vector3d dCamera = Vector3d.Zero;
            lock (_MovementActive)
            {
                if (_MovementActive[STRAFE_LEFT_KEY[0]])
                    dCamera += Vector3d.UnitX;
                if (_MovementActive[STRAFE_RIGHT_KEY[0]])
                    dCamera -= Vector3d.UnitX;
                if (_MovementActive[MOVE_FORWARD_KEY[0]])
                    dCamera += Vector3d.UnitY;
                if (_MovementActive[MOVE_BACKWARD_KEY[0]])
                    dCamera -= Vector3d.UnitY;
                if (_MovementActive[MOVE_UP_KEY[0]])
                    dCamera += Vector3d.UnitZ;
                if (_MovementActive[MOVE_DOWN_KEY[0]])
                    dCamera -= Vector3d.UnitZ;
            }
            dCamera = Vector3d.Transform(dCamera * MovementSpeed * dt, _OrientationMatrix.Inverted());
            _PointAt += dCamera;
            RecomputeCameraCharacteristics();
        }
    }
}
