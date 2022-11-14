using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AutoCAD;

namespace Stabiliteettilaskenta.AutoCad
{
    public class ACadDataManager
    {
        public List<double[]> MarkColumns(AcadDocument cadDocument, string _searchLayer, string _newLayer, double _radius)
        {
            AcadDatabase database = cadDocument.Database;
            var data = database.ModelSpace.GetEnumerator();

            AcadLayers layers = cadDocument.Layers;
            bool found = false;
            List<double[]> points = new List<double[]>();

            for (int i = 0; i < layers.Count; i++)
            {
                if (layers.Item(i).Name == _newLayer)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AcadLayer layer = layers.Add(_newLayer);
                layer.color = AcColor.acGreen;
                layer.Lineweight = ACAD_LWEIGHT.acLnWt030;
            }

            while (data.MoveNext())
            {
                AcadEntity item = (AcadEntity)data.Current;

                if (item.Layer == _searchLayer)
                {
                    if (item.ObjectName == "AcDbBlockReference")
                    {
                        AcadBlockReference block = (AcadBlockReference)item;
                        double[] point = new double[3];
                        point[0] = Math.Round(block.InsertionPoint[0], 0);
                        point[1] = Math.Round(block.InsertionPoint[1], 0);
                        point[2] = Math.Round(block.InsertionPoint[2], 0);
                        AcadCircle circle = database.ModelSpace.AddCircle(point, _radius);
                        points.Add(point);
                        circle.Layer = _newLayer;
                    }
                }
            }

            return points;
        }

        public void MarkBeams(AcadDocument cadDocument, string _searchLayer, string _newLayer, bool dirX)
        {
            AcadDatabase database = cadDocument.Database;

            AcadLayers layers = cadDocument.Layers;
            bool found = false;
            List<double[]> columns;

            for (int i = 0; i < layers.Count; i++)
            {
                if (layers.Item(i).Name == _newLayer)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AcadLayer layer = layers.Add(_newLayer);
                layer.color = AcColor.acMagenta;
                layer.Lineweight = ACAD_LWEIGHT.acLnWt030;
            }

            columns = GetColumns(database, _searchLayer);

            // Sort Columns
            Dictionary<double, List<double>> columnLines = new Dictionary<double, List<double>>();
            List<double> lines;
            double x;
            double y;

            for (int i = 0; i < columns.Count; i++)
            {
                double[] point = columns[i];
                x = point[0];
                y = point[1];

                if (dirX)
                {
                    if (!columnLines.ContainsKey(y))
                    {
                        lines = new List<double>();
                        lines.Add(x);
                        columnLines.Add(y, lines);
                    }
                    else
                    {
                        columnLines.TryGetValue(y, out lines);
                        lines.Add(x);
                        columnLines[y] = lines;
                    }
                }
                else
                {
                    if (!columnLines.ContainsKey(x))
                    {
                        lines = new List<double>();
                        lines.Add(y);
                        columnLines.Add(x, lines);
                    }
                    else
                    {
                        columnLines.TryGetValue(x, out lines);
                        lines.Add(y);
                        columnLines[x] = lines;
                    }
                }
            }
            // Add Beams
            Dictionary<double, List<double>>.Enumerator items = columnLines.GetEnumerator();

            while (items.MoveNext())
            {
                KeyValuePair<double, List<double>> item = items.Current;

                if (dirX)
                {
                    y = item.Key;
                    lines = item.Value;
                    if (lines.Count > 1)
                    {
                        lines.Sort();
                        double[] pointArray = new double[2 * lines.Count];

                        for (int i = 0; i < lines.Count; i++)
                        {
                            pointArray[2 * i] = lines[i];
                            pointArray[2 * i + 1] = y;
                        }

                        AcadLWPolyline polyline = database.ModelSpace.AddLightWeightPolyline(pointArray);
                        polyline.Layer = _newLayer;
                    }
                }
                else
                {
                    x = item.Key;
                    lines = item.Value;
                    if (lines.Count > 1)
                    {
                        lines.Sort();
                        double[] pointArray = new double[2 * lines.Count];

                        for (int i = 0; i < lines.Count; i++)
                        {
                            pointArray[2 * i] = x;
                            pointArray[2 * i + 1] = lines[i];
                        }

                        AcadLWPolyline polyline = database.ModelSpace.AddLightWeightPolyline(pointArray);
                        polyline.Layer = _newLayer;
                    }
                }
            }
        }

        public List<double[]> GetColumns(AcadDatabase database, string _searchLayer)
        {
            var data = database.ModelSpace.GetEnumerator();
            List<double[]> columns = new List<double[]>();
            // Get Columns
            while (data.MoveNext())
            {
                AcadEntity item = (AcadEntity)data.Current;

                if (item.Layer == _searchLayer)
                {
                    if (item.ObjectName == "AcDbCircle")
                    {
                        AcadCircle circle = (AcadCircle)item;
                        columns.Add(circle.Center);
                    }
                }
            }
            return columns;
        }

        public List<double[]> GetBeams(AcadDatabase database, string _searchLayer)
        {
            var data = database.ModelSpace.GetEnumerator();
            List<double[]> beams = new List<double[]>();
            // Get Beams
            while (data.MoveNext())
            {
                AcadEntity item = (AcadEntity)data.Current;

                if (item.Layer == _searchLayer)
                {
                    if (item.ObjectName == "AcDbPolyline")
                    {
                        AcadLWPolyline poly = (AcadLWPolyline)item;

                        double[] coordinates = poly.Coordinates;

                        for (int i = 0; i < coordinates.Length - 2;)
                        {
                            double[] beam = new double[6];
                            // StartPoint
                            beam[0] = (float)poly.Coordinates[i];
                            beam[1] = (float)poly.Coordinates[i + 1];
                            beam[2] = (float)poly.Elevation;

                            i = i + 2;

                            // EndPoint
                            beam[3] = (float)poly.Coordinates[i];
                            beam[4] = (float)poly.Coordinates[i + 1];
                            beam[5] = (float)poly.Elevation;

                            beams.Add(beam);
                        }
                        if (poly.Closed)
                        {
                            double[] beam = new double[6];
                            // StartPoint
                            int i = coordinates.Length - 2;
                            beam[0] = (float)poly.Coordinates[i];
                            beam[1] = (float)poly.Coordinates[i + 1];
                            beam[2] = (float)poly.Elevation;

                            // EndPoint
                            i = 0;
                            beam[3] = (float)poly.Coordinates[i];
                            beam[4] = (float)poly.Coordinates[i + 1];
                            beam[5] = (float)poly.Elevation;

                            beams.Add(beam);
                        }
                    }
                    else if (item.ObjectName == "AcDbLine")
                    {
                        AcadLine line = (AcadLine)item;
                        double[] beam = new double[6];

                        // StartPoint
                        beam[0] = (float)line.StartPoint[0];
                        beam[1] = (float)line.StartPoint[1];
                        beam[2] = (float)line.StartPoint[2];

                        // EndPoint
                        beam[3] = (float)line.EndPoint[0];
                        beam[4] = (float)line.EndPoint[1];
                        beam[5] = (float)line.EndPoint[2];

                        beams.Add(beam);
                    }
                }
            }
            return beams;
        }

        public static float DistanceFromLine(Vector3 _startPoint, Vector3 _endPoint, Vector3 _point)
        {
            float distance = 0;
            Vector3 line = new Vector3(_endPoint.X - _startPoint.X, _endPoint.Y - _startPoint.Y, _endPoint.Z - _startPoint.Z);
            Vector3 tempLine = new Vector3(_point.X - _startPoint.X, _point.Y - _startPoint.Y, _point.Z - _startPoint.Z);
            Vector3 pLine = Vector3.Cross(line, tempLine);
            distance = pLine.Length() / line.Length();

            return distance;
        }

        public static Vector3 PointLineIntersection(Vector3 _startPoint, Vector3 _endPoint, Vector3 _point, bool extrapolation = true)
        {
            Vector3 intersection = new Vector3();
            Vector3 line = new Vector3(_endPoint.X - _startPoint.X, _endPoint.Y - _startPoint.Y, _endPoint.Z - _startPoint.Z);
            Vector3 tempLine = new Vector3(_point.X - _startPoint.X, _point.Y - _startPoint.Y, _point.Z - _startPoint.Z);
            Vector3 between = tempLine - Vector3.Multiply(line, Vector3.Dot(tempLine, line) / line.LengthSquared());

            intersection = _point - between;

            if (!extrapolation)
            {
                if (_startPoint.X <= _endPoint.X && _startPoint.Y <= _endPoint.Y)
                {
                    if (intersection.X < _startPoint.X || intersection.Y < _startPoint.Y)
                    {
                        intersection = _startPoint;
                    }
                    else if (intersection.X > _endPoint.X || intersection.Y > _endPoint.Y)
                    {
                        intersection = _endPoint;
                    }
                }
                else if (_startPoint.X >= _endPoint.X && _startPoint.Y >= _endPoint.Y)
                {
                    if (intersection.X > _startPoint.X || intersection.Y > _startPoint.Y)
                    {
                        intersection = _startPoint;
                    }
                    else if (intersection.X < _endPoint.X || intersection.Y < _endPoint.Y)
                    {
                        intersection = _endPoint;
                    }
                }
                else if (_startPoint.X > _endPoint.X && _startPoint.Y < _endPoint.Y)
                {
                    if (intersection.X > _startPoint.X || intersection.Y < _startPoint.Y)
                    {
                        intersection = _startPoint;
                    }
                    else if (intersection.X < _endPoint.X || intersection.Y > _endPoint.Y)
                    {
                        intersection = _endPoint;
                    }
                }
                else if (_startPoint.X < _endPoint.X && _startPoint.Y > _endPoint.Y)
                {
                    if (intersection.X < _startPoint.X || intersection.Y > _startPoint.Y)
                    {
                        intersection = _startPoint;
                    }
                    else if (intersection.X > _endPoint.X || intersection.Y < _endPoint.Y)
                    {
                        intersection = _endPoint;
                    }
                }
            }

            return intersection;
        }
    }
}
