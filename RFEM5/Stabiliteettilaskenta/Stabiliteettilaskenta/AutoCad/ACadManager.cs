using AutoCAD;
using Stabiliteettilaskenta.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stabiliteettilaskenta.AutoCad
{
    public class ACadManager
    {
        private AcadApplication cadApp;
        AcadDocument cadDocument;
        private AcadSelectionSet selectionSet;
        AcadUCS userCoordinateSystem;

        private bool Connect(string _name = "")
        {
            try
            {
                cadApp = (AcadApplication)System.Runtime.InteropServices.Marshal.GetActiveObject("AutoCAD.Application");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find AutoCAD 2018!", "Open AutoCAD");
                return false;
            }

            cadDocument = cadApp.ActiveDocument;

            if (_name != "")
            {
                for (int i = 0; i < cadApp.Documents.Count; i++)
                {
                    cadDocument = cadApp.Documents.Item(i);

                    if (cadDocument.Name == _name)
                    {
                        break;
                    }
                }
                cadDocument = cadApp.ActiveDocument;
            }

            return true;
        }
        public AcadDocument GetDocument(string _name = "")
        {
            if (Connect(_name))
            {
                return cadDocument;
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (cadDocument != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cadDocument);
                cadDocument = null;
            }
            if (cadApp != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cadApp);
                cadApp = null;
            }
        }

        public List<string> ACad_GetLayers()
        {
            if (!this.Connect())
            {
                return null;
            }

            List<string> layerNames = new List<string>();

            AcadLayers layers = cadDocument.Layers;
            for (int i = 0; i < layers.Count; i++)
            {
                AcadLayer layer = layers.Item(i);
                layerNames.Add(layer.Name);
            }
            return layerNames;
        }

        public void SendText(Vector3 _point, string _text, double _width = 10, double _height = 25, string layer = "")
        {
            if (!this.Connect())
            {
                return;
            }

            double[] insertionPoint = { (double)_point.X, (double)_point.Y, (double)_point.Z };

            AcadMText text = cadDocument.Database.ModelSpace.AddMText(insertionPoint, _width, _text);

            if (layer != "")
            {
                text.Layer = layer;
            }
             
            text.Height = _height;
        }

        public void GetSelectedObjects()
        {
            if (!this.Connect())
            {
                return;
            }

            MessageBox.Show("Select Lines from AutoCAD and Press Enter.", "Select Lines");

            // If SelectionSet "AcDbPolyline" already exists, it should be deleted before adding new
            for (int i = 0; i < cadDocument.SelectionSets.Count; i++)
            {
                if (cadDocument.SelectionSets.Item(i).Name == "AcDbPolyline")
                {
                    cadDocument.SelectionSets.Item(i).Delete();
                    break;
                }
            }
            
            selectionSet = cadDocument.SelectionSets.Add("AcDbPolyline");
            selectionSet.SelectOnScreen();
            
            foreach (AcadEntity entity in selectionSet)
            {

            }
        }

        public void GetUCS()
        {
            if (!this.Connect())
            {
                return;
            }
            AcadUCSs userCoordinateSystems = cadDocument.ModelSpace.Database.UserCoordinateSystems;
            IEnumerator ucs = userCoordinateSystems.GetEnumerator();

            while (ucs.MoveNext())
            {
                AcadUCS coordinateSystem = (AcadUCS)ucs.Current;
                if (coordinateSystem.Name == "käännetty_koord.")
                {
                    userCoordinateSystem = coordinateSystem;
                }
            }
        }

        public void SetUCS()
        {
            if (!this.Connect())
            {
                return;
            }

            AcadUCSs userCoordinateSystems = cadDocument.ModelSpace.Database.UserCoordinateSystems;

            double[] x = new double[] { userCoordinateSystem.Origin[0] + userCoordinateSystem.XVector[0], userCoordinateSystem.Origin[1] + userCoordinateSystem.XVector[1], userCoordinateSystem.Origin[2] + userCoordinateSystem.XVector[2] };
            double[] y = new double[] { userCoordinateSystem.Origin[0] + userCoordinateSystem.YVector[0], userCoordinateSystem.Origin[1] + userCoordinateSystem.YVector[1], userCoordinateSystem.Origin[2] + userCoordinateSystem.YVector[2] };

            userCoordinateSystems.Add(userCoordinateSystem.Origin, x, y, userCoordinateSystem.Name);
        }

        public void ModifyLine()
        {
            if (!this.Connect())
            {
                return;
            }

            float round = 10f;
            AcadDatabase database = cadDocument.Database;
            var data = database.ModelSpace.GetEnumerator();

            while(data.MoveNext())
            {
                AcadEntity entity = data.Current as AcadEntity;
                AcadLine line = data.Current as AcadLine;
                AcadLWPolyline pLine = data.Current as AcadLWPolyline;
                AcadCircle circle = data.Current as AcadCircle;

                if (pLine == null && circle == null && line == null)
                {
                    continue;
                }

                if (line != null)
                {
                    double[] startPoint = line.StartPoint;
                    double[] endPoint = line.EndPoint;
                    for (int i = 0; i < startPoint.Length; i++)
                    {
                        startPoint[i] = Math.Round(startPoint[i] / round, 0) * round;
                    }
                    for (int i = 0; i < endPoint.Length; i++)
                    {
                        endPoint[i] = Math.Round(endPoint[i] / round, 0) * round;
                    }

                    line.StartPoint = startPoint;
                    line.EndPoint = endPoint;
                }
                if (pLine != null)
                {
                    double[] coordinates = pLine.Coordinates;
                    for (int i = 0; i < coordinates.Length; i++)
                    {
                        coordinates[i] = Math.Round(coordinates[i] / round, 0) * round;
                    }

                    pLine.Coordinates = coordinates;
                }
                if (circle != null)
                {
                    double[] coordinates = circle.Center;
                    for (int i = 0; i < coordinates.Length; i++)
                    {
                        coordinates[i] = Math.Round(coordinates[i] / round, 0) * round;
                    }

                    circle.Center = coordinates;
                }
            }
        }

        public void FitLine(float _tolerance, string _layer = "")
        {
            if (!this.Connect())
            {
                return;
            }

            List<AcadLine> fitlines = new List<AcadLine>();
            AcadDatabase database = cadDocument.Database;
            var data = database.ModelSpace.GetEnumerator();

            while (data.MoveNext())
            {
                AcadLine line = data.Current as AcadLine;
                if (line == null || (line.Layer != _layer && _layer != ""))
                {
                    continue;
                }
                fitlines.Add(line);
            }

            data = database.ModelSpace.GetEnumerator();

            while (data.MoveNext())
            {
                //AcadEntity entity = data.Current as AcadEntity;
                AcadLWPolyline pLine = data.Current as AcadLWPolyline;
                AcadCircle circle = data.Current as AcadCircle;

                //if (entity != null)
                //{
                //    if (entity.Layer == "0")
                //    {
                //        int a = 1;
                //    }
                //}

                if (pLine == null && circle == null)
                {
                    continue;
                }

                if (pLine != null)
                {
                    double[] coordinates = pLine.Coordinates;
                    List<Vector3> points = new List<Vector3>();
                    Vector3 point = new Vector3();

                    for (int i = 0; i < coordinates.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            point.X = (float)coordinates[i];
                        }
                        else if (i % 2 == 1)
                        {
                            point.Y = (float)coordinates[i];
                            point.Z = (float)pLine.Elevation;
                            points.Add(point);
                            point = new Vector3();
                        }
                    }

                    int index = 0;

                    for (int j = 0; j < points.Count; j++)
                    {
                        float distX = float.MaxValue;
                        float distY = float.MaxValue;
                        Vector3 newPoint = points[j];
                        for (int i = 0; i < fitlines.Count; i++)
                        {
                            double[] startPoint = fitlines[i].StartPoint;
                            double[] endPoint = fitlines[i].EndPoint;
                            Vector3 start = new Vector3((float)startPoint[0], (float)startPoint[1], (float)startPoint[2]);
                            Vector3 end = new Vector3((float)endPoint[0], (float)endPoint[1], (float)endPoint[2]);
                            float distance = ACadDataManager.DistanceFromLine(start, end, points[j]);

                            if (distance <= _tolerance)
                            {
                                Vector3 intersection = ACadDataManager.PointLineIntersection(start, end, points[j], false);
                                Vector3 dist = points[j] - intersection;
                                if (dist.Length() <= _tolerance)
                                {
                                    if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
                                    {
                                        // fitline is parallel to the x-axis
                                        float distanceY = Math.Abs(newPoint.Y - intersection.Y);

                                        if (distanceY <= _tolerance && distanceY < distY)
                                        {
                                            newPoint.Y = intersection.Y;
                                            distY = distanceY;
                                        }
                                    }
                                    else
                                    {
                                        // fitline is parallel to the y-axis
                                        float distanceX = Math.Abs(newPoint.X - intersection.X);

                                        if (distanceX <= _tolerance && distanceX < distX)
                                        {
                                            newPoint.X = intersection.X;
                                            distX = distanceX;
                                        }
                                    }
                                }
                            }
                        }
                        points[j] = newPoint;

                        // set coordinates
                        coordinates[index] = (double)points[j].X;
                        index++;
                        coordinates[index] = (double)points[j].Y;
                        index++;
                    }

                    pLine.Coordinates = coordinates;
                }
                if (circle != null)
                {
                    double[] coordinates = circle.Center;
                    Vector3 center = new Vector3((float)coordinates[0], (float)coordinates[1], (float)coordinates[2]);

                    float distX = float.MaxValue;
                    float distY = float.MaxValue;
                    Vector3 newPoint = center;
                    for (int i = 0; i < fitlines.Count; i++)
                    {
                        double[] startPoint = fitlines[i].StartPoint;
                        double[] endPoint = fitlines[i].EndPoint;
                        Vector3 start = new Vector3((float)startPoint[0], (float)startPoint[1], (float)startPoint[2]);
                        Vector3 end = new Vector3((float)endPoint[0], (float)endPoint[1], (float)endPoint[2]);
                        float distance = ACadDataManager.DistanceFromLine(start, end, center);
                        
                        if (distance <= _tolerance)
                        {
                            Vector3 intersection = ACadDataManager.PointLineIntersection(start, end, center, false);
                            Vector3 dist = center - intersection;
                            if (dist.Length() <= _tolerance)
                            {
                                if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
                                {
                                    // fitline is parallel to the x-axis
                                    float distanceY = Math.Abs(newPoint.Y - intersection.Y);

                                    if (distanceY <= _tolerance && distanceY < distY)
                                    {
                                        newPoint.Y = intersection.Y;
                                        distY = distanceY;
                                    }
                                }
                                else
                                {
                                    // fitline is parallel to the y-axis
                                    float distanceX = Math.Abs(newPoint.X - intersection.X);

                                    if (distanceX <= _tolerance && distanceX < distX)
                                    {
                                        newPoint.X = intersection.X;
                                        distX = distanceX;
                                    }
                                }
                            }
                        }
                    }
                    center = newPoint;

                    coordinates = new double[] { (double)center.X, (double)center.Y, (double)center.Z };
                    circle.Center = coordinates;
                }
            }
        }

        public void FitLine2(float _tolerance, string _layer = "")
        {
            if (!this.Connect())
            {
                return;
            }

            AcadDatabase database = cadDocument.Database;
            var data = database.ModelSpace.GetEnumerator();

            Line[] fitlineArr = ACadUtils.GetLines(cadDocument, _layer);

            AcadLWPolyline[] pLines = ACadUtils.GetEntities<AcadLWPolyline>(cadDocument);
            AcadCircle[] circles  = ACadUtils.GetEntities<AcadCircle>(cadDocument);

            Parallel.For(0, pLines.Length, p =>
            {
                AcadLWPolyline pLine = pLines[p];

                double[] coordinates = pLine.Coordinates;
                List<Vector3> points = new List<Vector3>();
                Vector3 point = new Vector3();

                for (int i = 0; i < coordinates.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        point.X = (float)coordinates[i];
                    }
                    else if (i % 2 == 1)
                    {
                        point.Y = (float)coordinates[i];
                        point.Z = (float)pLine.Elevation;
                        points.Add(point);
                        point = new Vector3();
                    }
                }

                int index = 0;

                for (int j = 0; j < points.Count; j++)
                {
                    float distX = float.MaxValue;
                    float distY = float.MaxValue;
                    Vector3 newPoint = points[j];
                    for (int i = 0; i < fitlineArr.Length; i++)
                    {
                        Vector3 start = fitlineArr[i].StartPoint;
                        Vector3 end = fitlineArr[i].EndPoint;
                        float distance = ACadDataManager.DistanceFromLine(start, end, points[j]);

                        if (distance <= _tolerance)
                        {
                            Vector3 intersection = ACadDataManager.PointLineIntersection(start, end, points[j], false);
                            Vector3 dist = points[j] - intersection;
                            if (dist.Length() <= _tolerance)
                            {
                                if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
                                {
                                    // fitline is parallel to the x-axis
                                    float distanceY = Math.Abs(newPoint.Y - intersection.Y);

                                    if (distanceY <= _tolerance && distanceY < distY)
                                    {
                                        newPoint.Y = intersection.Y;
                                        distY = distanceY;
                                    }
                                }
                                else
                                {
                                    // fitline is parallel to the y-axis
                                    float distanceX = Math.Abs(newPoint.X - intersection.X);

                                    if (distanceX <= _tolerance && distanceX < distX)
                                    {
                                        newPoint.X = intersection.X;
                                        distX = distanceX;
                                    }
                                }
                            }
                        }
                    }
                    points[j] = newPoint;

                    // set coordinates
                    coordinates[index] = (double)points[j].X;
                    index++;
                    coordinates[index] = (double)points[j].Y;
                    index++;
                }

                pLine.Coordinates = coordinates;
            });

            Parallel.For(0, circles.Length, c =>
            {
                AcadCircle circle = circles[c];

                double[] coordinates = circle.Center;
                Vector3 center = new Vector3((float)coordinates[0], (float)coordinates[1], (float)coordinates[2]);

                float distX = float.MaxValue;
                float distY = float.MaxValue;
                Vector3 newPoint = center;
                for (int i = 0; i < fitlineArr.Length; i++)
                {
                    Vector3 start = fitlineArr[i].StartPoint;
                    Vector3 end = fitlineArr[i].EndPoint;
                    float distance = ACadDataManager.DistanceFromLine(start, end, center);

                    if (distance <= _tolerance)
                    {
                        Vector3 intersection = ACadDataManager.PointLineIntersection(start, end, center, false);
                        Vector3 dist = center - intersection;
                        if (dist.Length() <= _tolerance)
                        {
                            if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
                            {
                                // fitline is parallel to the x-axis
                                float distanceY = Math.Abs(newPoint.Y - intersection.Y);

                                if (distanceY <= _tolerance && distanceY < distY)
                                {
                                    newPoint.Y = intersection.Y;
                                    distY = distanceY;
                                }
                            }
                            else
                            {
                                // fitline is parallel to the y-axis
                                float distanceX = Math.Abs(newPoint.X - intersection.X);

                                if (distanceX <= _tolerance && distanceX < distX)
                                {
                                    newPoint.X = intersection.X;
                                    distX = distanceX;
                                }
                            }
                        }
                    }
                }
                center = newPoint;

                coordinates = new double[] { (double)center.X, (double)center.Y, (double)center.Z };
                circle.Center = coordinates;
            });
        }
    }
}
