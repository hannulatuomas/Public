using Stabiliteettilaskenta.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace Stabiliteettilaskenta.MODEL
{
    public class PointManager
    {
        private Dictionary<int, Point> pointNumbers;
        private Dictionary<float, List<Point>> pointsByHeight;
        int firstFreeNumber;

        public PointManager()
        {
            pointNumbers = new Dictionary<int, Point>();
            pointsByHeight = new Dictionary<float, List<Point>>();
            firstFreeNumber = 1;
        }

        #region Add Node
        public Point AddNode(Vector3 _coordinates, int _number = 0)
        {
            Point node;
            int number;

            if (_number != 0 && !pointNumbers.ContainsKey(_number))
            {
                number = _number;
                node = new Point(_coordinates, number);
            }
            else
            {
                node = new Point(_coordinates);
            }           

            if (ContainsPointCoordinates(node, out number))
            {
                node = pointNumbers[number];
            }
            else
            {
                if (number == 0)
                {
                    if (pointNumbers.ContainsKey(firstFreeNumber))
                    {
                        GetFirstFreeIndex();
                    }
                    number = firstFreeNumber;
                    firstFreeNumber++;
                    node.Number = number;
                }

                // Add new Node to Lists;
                pointNumbers.Add(node.Number, node);
                float height = node.Coordinates.Z;
                if (pointsByHeight.ContainsKey(height))
                {
                    pointsByHeight[height].Add(node);
                }
                else
                {
                    List<Point> list = new List<Point>();
                    list.Add(node);
                    pointsByHeight.Add(height, list);
                }
            }

            return node;
        }

        #endregion

        #region Remove Node
        public bool RemoveNode(Vector3 _coordinates)
        {
            Point point = GetNode(_coordinates);
            if (point == null)
            {
                return false;
            }
            return RemoveNode(point);
        }
        public bool RemoveNode(Point _point)
        {
            bool successfully;
            successfully = pointNumbers.Remove(_point.Number);
            if (successfully)
            {
                GetFirstFreeIndex();
                float height = _point.Coordinates.Z;               
                successfully = pointsByHeight.ContainsKey(height);

                if (successfully)
                {
                    List<Point> points = pointsByHeight[height];
                    successfully = points.Remove(_point);
                    if (successfully && points.Count > 0)
                    {
                        pointsByHeight[height] = points;
                    }
                    else if (successfully && points.Count == 0)
                    {
                        pointsByHeight.Remove(height);
                    }
                }
            }
            return successfully;
        }

        #endregion

        #region Get Node
        public Point GetNode(Vector3 _coordinates)
        {
            // Maybe from pointsByHeight?
            Dictionary<int, Point>.ValueCollection.Enumerator points = pointNumbers.Values.GetEnumerator();
            while (points.MoveNext())
            {
                Vector3 coordinates = points.Current.Coordinates;
                if (coordinates.X == _coordinates.X)
                {
                    if (coordinates.Y == _coordinates.Y)
                    {
                        if (coordinates.Z == _coordinates.Z)
                        {
                            return points.Current;
                        }
                    }
                }
            }
            return null;
        }
        public Point GetNode(int _number)
        {
            if (pointNumbers.ContainsKey(_number))
            {
                return pointNumbers[_number];
            }
            return null;
        }

        #endregion

        int GetFirstFreeIndex()
        {
            firstFreeNumber = 1;
            IEnumerator<int> numbers = pointNumbers.Keys.OrderBy(k => k).GetEnumerator();
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
        bool ContainsPoint(Point _point)
        {
            return pointNumbers.ContainsValue(_point);
        }
        bool ContainsPointNumber(Point _point)
        {
            return pointNumbers.ContainsKey(_point.Number);
        }
        bool ContainsPointCoordinates(Point _point, out int _number)
        {
            _number = _point.Number;
            foreach (Point point in pointNumbers.Values)
            {
                if (point.Coordinates.X == _point.Coordinates.X)
                {
                    if (point.Coordinates.Y == _point.Coordinates.Y)
                    {
                        if (point.Coordinates.Z == _point.Coordinates.Z)
                        {
                            _number = point.Number;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool ContainsElevation(Point _point)
        {
            return pointsByHeight.ContainsKey(_point.Coordinates.Z);
        }
        List<Point> SortByNumbers()
        {
            List<Point> points = new List<Point>();
            points = pointNumbers.Values.OrderBy(p => p.Number).ToList();
            return points;
        }
        Dictionary<float, List<Point>> SortByHeight()
        {
            Dictionary<float, List<Point>> floors = new Dictionary<float, List<Point>>();
            floors = pointsByHeight.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
            return floors;
        }
        List<Point> SortByCoordinates()
        {
            Dictionary<float, List<Point>> floors = SortByHeight();
            List<Point> points = new List<Point>(floors.Values.Count);

            foreach (float floor in floors.Keys)
            {
                List<Point> nodes = new List<Point>();
                nodes = floors[floor].OrderBy(n => n.Coordinates.Z).ThenBy(n => n.Coordinates.Y).ThenBy(n => n.Coordinates.X).ToList();
                points.AddRange(nodes);
            }
            return points;
        }

        #endregion

    }

    // Point Class

    public class Point : IHeapItem<Point>
    {
        public Point(Vector3 _point)
        {
            Coordinates = _point;
        }
        public Point(Vector3 _point, int _number)
        {
            Coordinates = _point;
            Number = _number;
        }

        public int HeapIndex { get; set; }
        public int Number { get; set; }
        public Vector3 Coordinates { get; private set; }

        public int CompareTo(Point other)
        {
            int compare = Coordinates.Z.CompareTo(other.Coordinates.Z);
            if (compare == 0)
            {
                compare = Coordinates.Y.CompareTo(other.Coordinates.Y);
                if (compare == 0)
                {
                    compare = Coordinates.X.CompareTo(other.Coordinates.X);
                    if (compare == 0)
                    {
                        MessageBox.Show("Point: " + Coordinates.X + ", " + Coordinates.Y + ", " + Coordinates.Z + " found twice?", "Error");
                    }
                }
            }
            return -compare;
        }
    }
}
