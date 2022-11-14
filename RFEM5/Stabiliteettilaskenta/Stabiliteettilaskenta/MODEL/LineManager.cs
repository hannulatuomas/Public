using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Stabiliteettilaskenta.MODEL
{
    public class LineManager
    {
        public PointManager pointManager;
        Dictionary<int, Line> lineNumbers;
        Dictionary<MainDirection, List<Line>> lineDirections;
        Dictionary<float, List<Line>> lineLowerEndHeights;
        int firstFreeNumber;

        public LineManager()
        {
            pointManager = new PointManager();
            lineNumbers = new Dictionary<int, Line>();
            lineDirections = new Dictionary<MainDirection, List<Line>>();
            lineLowerEndHeights = new Dictionary<float, List<Line>>();
            firstFreeNumber = 1;
        }

        #region Add Line
        public Line AddLine(Point _startPoint, Point _endPoint, int _number = 0)
        {
            int number;
            Line line = new Line(_startPoint, _endPoint);

            if (_number != 0 && !lineNumbers.ContainsKey(_number))
            {
                number = _number;
            }

            if (ContainsLineEndPoints(line, out number))
            {
                // Already Exists
                line = lineNumbers[number];
            }
            else
            {
                if (number == 0)
                {
                    if (lineNumbers.ContainsKey(firstFreeNumber))
                    {
                        GetFirstFreeIndex();
                    }
                    number = firstFreeNumber;
                    firstFreeNumber++;
                }
                line.Number = number;

                // Add new Line to lists
                lineNumbers.Add(line.Number, line);

                MainDirection direction = line.GetMainDirection();
                if (lineDirections.ContainsKey(direction))
                {
                    lineDirections[direction].Add(line);
                }
                else
                {
                    List<Line> list = new List<Line>();
                    list.Add(line);
                    lineDirections.Add(direction, list);
                }

                float lowerEndHeight = line.StartPoint.Coordinates.Z;
                if (line.EndPoint.Coordinates.Z < line.StartPoint.Coordinates.Z)
                {
                    lowerEndHeight = line.EndPoint.Coordinates.Z;
                }

                if (lineLowerEndHeights.ContainsKey(lowerEndHeight))
                {
                    lineLowerEndHeights[lowerEndHeight].Add(line);
                }
                else
                {
                    List<Line> list = new List<Line>();
                    list.Add(line);
                    lineLowerEndHeights.Add(lowerEndHeight, list);
                }
            }

            return line;
        }
        public Line AddLine(int _startPoint, int _endPoint)
        {
            Point startPoint = pointManager.GetNode(_startPoint);
            if (startPoint == null)
            {
                return null;
            }
            Point endPoint = pointManager.GetNode(_endPoint);
            if (endPoint == null)
            {
                return null;
            }
            Line line = AddLine(startPoint, endPoint);

            return line;
        }
        public Line AddLine(Vector3 _startPoint, Vector3 _endPoint)
        {
            Point startPoint = pointManager.GetNode(_startPoint);
            if (startPoint == null)
            {
                startPoint = pointManager.AddNode(_startPoint);
            }

            Point endPoint = pointManager.GetNode(_endPoint);
            if (endPoint == null)
            {
                endPoint = pointManager.AddNode(_endPoint);
            }
            Line line = AddLine(startPoint, endPoint);

            return line;
        }

        #endregion

        #region Remove Line
        public bool RemoveLine(Line _line)
        {
            bool successfully;
            successfully = lineNumbers.Remove(_line.Number);
            if (successfully)
            {
                GetFirstFreeIndex();
                MainDirection direction = _line.GetMainDirection();
                successfully = lineDirections.ContainsKey(direction);
                if (successfully)
                {
                    List<Line> lines = lineDirections[direction];
                    successfully = lines.Remove(_line);
                    if (successfully && lines.Count > 0)
                    {
                        lineDirections[direction] = lines;
                    }
                    else if (successfully && lines.Count == 0)
                    {
                        lineDirections.Remove(direction);
                    }

                    if (successfully)
                    {
                        float lowerEndHeight = _line.StartPoint.Coordinates.Z;
                        if (_line.EndPoint.Coordinates.Z < _line.StartPoint.Coordinates.Z)
                        {
                            lowerEndHeight = _line.EndPoint.Coordinates.Z;
                        }

                        lines = lineLowerEndHeights[lowerEndHeight];
                        successfully = lines.Remove(_line);
                        if (successfully && lines.Count > 0)
                        {
                            lineLowerEndHeights[lowerEndHeight] = lines;
                        }
                        else if (successfully && lines.Count == 0)
                        {
                            lineLowerEndHeights.Remove(lowerEndHeight);
                        }
                    }
                }
            }

            return successfully;
        }

        #endregion

        #region Get Line


        #endregion

        int GetFirstFreeIndex()
        {
            firstFreeNumber = 1;
            IEnumerator<int> numbers = lineNumbers.Keys.OrderBy(k => k).GetEnumerator();
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
        bool ContainsLineEndPoints(Line _line, out int _number)
        {
            _number = _line.Number;
            foreach (Line line in lineNumbers.Values)
            {
                if (line.StartPoint == _line.StartPoint)
                {
                    if (line.EndPoint == _line.EndPoint)
                    {
                        _number = line.Number;
                        return true;
                    }
                }
            }
            return false;
        }
        // SortByDirection?
        // SortByCoordinates?

        #endregion

    }

        // Line Class

        public class Line
        {
            public Line(Point _startPoint, Point _endPoint)
            {
                StartPoint = _startPoint;
                EndPoint = _endPoint;
            }

            public int Number { get; set; }
            public Point StartPoint { get;  private set; }
            public Point EndPoint { get; private set; }
            public Vector3 Direction
            {
                get
                {
                    return Vector3.Normalize(EndPoint.Coordinates - StartPoint.Coordinates);
                }
            }
            public MainDirection GetMainDirection()
            {
                Vector3 dir = Direction;

                if (Math.Abs(dir.X) >= Math.Abs(dir.Y) && Math.Abs(dir.X) >= Math.Abs(dir.Z))
                {
                    return MainDirection.X;
                }
                else if (Math.Abs(dir.Y) >= Math.Abs(dir.X) && Math.Abs(dir.Y) >= Math.Abs(dir.Z))
                {
                    return MainDirection.Y;
                }
                else if (Math.Abs(dir.Z) >= Math.Abs(dir.X) && Math.Abs(dir.Z) >= Math.Abs(dir.Y))
                {
                    return MainDirection.Z;
                }
                else
                {
                    return MainDirection.Unknown;
                }
            } 
        }

    public enum MainDirection { X, Y, Z, Unknown }
}
