using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Stabiliteettilaskenta.MODEL
{
    public class SurfaceManager
    {
        LineManager lineManager;
        Dictionary<int, Surface> surfaceNumbers;
        int firstFreeNumber;

        public SurfaceManager()
        {
            lineManager = new LineManager();
            surfaceNumbers = new Dictionary<int, Surface>();
            firstFreeNumber = 1;
        }

        #region Add Surface
        public Surface AddSurface(Line[] _lines, int _number)
        {
            int number;
            Surface surface = new Surface(_lines);
            if (surface.Lines.Length > 0)
            {
                if (_number != 0 && !surfaceNumbers.ContainsKey(_number))
                {
                    number = _number;
                }
                if (ContainsSurface(surface, out number))
                {
                    // Already Exists
                    surface = surfaceNumbers[number];
                }
                else
                {
                    if (number == 0)
                    {
                        if (surfaceNumbers.ContainsKey(firstFreeNumber))
                        {
                            GetFirstFreeIndex();
                        }
                        number = firstFreeNumber;
                        firstFreeNumber++;
                    }
                    surface.Number = number;

                    // Add new Surface to lists
                    surfaceNumbers.Add(surface.Number, surface);
                }
            }
            else
            {
                surface = null;
            }

            return surface;
        }

        #endregion

        #region Remove Surface
        public bool RemoveSurface(Surface _surface)
        {
            bool successfully;
            successfully = surfaceNumbers.Remove(_surface.Number);
            if (successfully)
            {
                GetFirstFreeIndex();

                // Add Other Lists here
                ///
            }

            return successfully;
        }

        #endregion

        #region Get Surface


        #endregion

        int GetFirstFreeIndex()
        {
            firstFreeNumber = 1;
            IEnumerator<int> numbers = surfaceNumbers.Keys.OrderBy(k => k).GetEnumerator();
            while (numbers.MoveNext())
            {
                if (numbers.Current != firstFreeNumber)
                {
                    break;
                }
                firstFreeNumber++;
            }
            return firstFreeNumber;
        }

        #region Linq
        bool ContainsSurface(Surface _surface, out int _number)
        {
            _number = _surface.Number;
            foreach (Surface surface in surfaceNumbers.Values)
            {
                if (surface.Number == _surface.Number)
                {
                    _number = surface.Number;
                    return true;
                }
            }
            return false;
        }

        #endregion

    }

    public class Surface
    {
        public Surface(Line[] _lines)
        {
            List<Point> points;
            if (ValidateLines(_lines))
            {
                Lines = _lines;
            }         
        }

        bool ValidateLines(Line[] _lines)
        {
            if (_lines.Length < 2)
            {
                return false;
            }
            List<Point> points = new List<Point>();

            foreach (Line line in _lines)
            {
                Point point = line.StartPoint;
                if (!points.Contains(point))
                {
                    points.Add(point);
                }
                point = line.EndPoint;
                if (!points.Contains(point))
                {
                    points.Add(point);
                }
            }

            if (points.Count < 3)
            {
                return false;
            }

            Plane plane = new Plane(points[0].Coordinates, points[1].Coordinates, points[2].Coordinates);

            if (!plane.IsValid)
            {
                return false;
            }
            else if (points.Count > 3)
            {
                for (int i = 3; i < points.Count; i++)
                {
                    if (!plane.Contains(points[i].Coordinates))
                    {
                        return false;
                    }
                }
            }
            Plane = plane;
            Points = points;
            return true;
        }

        public int Number { get; set; }
        public Line[] Lines { get; private set; }
        public List<Point> Points { get; private set; }
        public Plane Plane { get; private set; }
        public Material Material { get; set; }
        public float Thickness { get; set; }

    }

    public class Plane
    {
        private const double TOLERANCE = 0.0001f;

        private readonly double independentTerm;
        public Vector3 Normal { get; }
        public float Magnitude { get; }
        public bool IsValid { get; }

        public Plane(Vector3 _p1, Vector3 _p2, Vector3 _p3)
        {
            Normal = Vector3.Cross(new Vector3(_p2.X - _p1.X, _p2.Y - _p1.Y, _p2.Z - _p1.Z), new Vector3(_p3.X - _p1.X, _p3.Y - _p1.Y, _p3.Z - _p1.Z));

            Magnitude = Vector3.Distance(Normal, Vector3.Zero);
            
            if (Magnitude < TOLERANCE)
            {
                IsValid = false;
                return;
            }
            else
            {
                IsValid = true;
                independentTerm = -(Normal.X * _p1.X + Normal.Y * _p1.Y + Normal.Z * _p1.Z);
            }
        }

        public bool Contains(Vector3 _point)
        {
            return Math.Abs(Normal.X * _point.X + Normal.Y * _point.Y + Normal.Z * _point.Z + independentTerm) < TOLERANCE;
        }
    }
}
