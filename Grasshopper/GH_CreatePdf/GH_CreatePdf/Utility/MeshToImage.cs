using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_CreatePdf.Utility
{
    public static class MeshToImage
    {
        public static Image ConstructImageFromMeshList(int width, List<Mesh> meshList_1, List<Mesh> meshList_2 = null, List<Mesh> meshList_3 = null)
        {
            float xMax = float.MinValue;
            float yMax = float.MinValue;
            float xMin = float.MaxValue;
            float yMin = float.MaxValue;
            List<PointF[]> pointArrayList_1 = new List<PointF[]>();
            List<Color> colorList_1 = new List<Color>();
            List<PointF[]> pointArrayList_2 = null;
            List<Color> colorList_2 = null;
            List<PointF[]> pointArrayList_3 = null;
            List<Color> colorList_3 = null;

            #region Layer 1
            foreach (Mesh mesh in meshList_1)
            {
                List<PointF> pointList = new List<PointF>();
                Polyline[] outlines = mesh.GetOutlines(Plane.WorldXY);
                MeshVertexColorList vertexColors = mesh.VertexColors;

                if (vertexColors.Count > 0)
                {
                    colorList_1.Add(vertexColors[0]);
                }
                else
                {
                    colorList_1.Add(Color.Black);
                }

                foreach (Polyline outline in outlines)
                {
                    Line[] lines = outline.GetSegments();
                    
                    foreach (Line line in lines)
                    {
                        PointF start = new PointF((float)line.From.X, (float)line.From.Y);
                        PointF end = new PointF((float)line.To.X, (float)line.To.Y);

                        if (!pointList.Contains(start))
                        {
                            pointList.Add(end);
                        }

                        if (!pointList.Contains(end))
                        {
                            pointList.Add(end);
                        }

                        if (start.X > xMax) xMax = start.X;
                        if (start.Y > yMax) yMax = start.Y;
                        if (start.X < xMin) xMin = start.X;
                        if (start.Y < yMin) yMin = start.Y;

                        if (end.X > xMax) xMax = end.X;
                        if (end.Y > yMax) yMax = end.Y;
                        if (end.X < xMin) xMin = end.X;
                        if (end.Y < yMin) yMin = end.Y;
                    }
                }

                pointArrayList_1.Add(pointList.ToArray());
            }
            #endregion

            #region Layer 2
            if (meshList_2 != null)
            {
                pointArrayList_2 = new List<PointF[]>();
                colorList_2 = new List<Color>();

                foreach (Mesh mesh in meshList_2)
                {
                    List<PointF> pointList = new List<PointF>();
                    Polyline[] outlines = mesh.GetOutlines(Plane.WorldXY);
                    MeshVertexColorList vertexColors = mesh.VertexColors;

                    if (vertexColors.Count > 0)
                    {
                        colorList_2.Add(vertexColors[0]);
                    }
                    else
                    {
                        colorList_2.Add(Color.Black);
                    }

                    foreach (Polyline outline in outlines)
                    {
                        Line[] lines = outline.GetSegments();

                        foreach (Line line in lines)
                        {
                            PointF start = new PointF((float)line.From.X, (float)line.From.Y);
                            PointF end = new PointF((float)line.To.X, (float)line.To.Y);

                            if (!pointList.Contains(start))
                            {
                                pointList.Add(end);
                            }

                            if (!pointList.Contains(end))
                            {
                                pointList.Add(end);
                            }

                            if (start.X > xMax) xMax = start.X;
                            if (start.Y > yMax) yMax = start.Y;
                            if (start.X < xMin) xMin = start.X;
                            if (start.Y < yMin) yMin = start.Y;

                            if (end.X > xMax) xMax = end.X;
                            if (end.Y > yMax) yMax = end.Y;
                            if (end.X < xMin) xMin = end.X;
                            if (end.Y < yMin) yMin = end.Y;
                        }
                    }

                    pointArrayList_2.Add(pointList.ToArray());
                }
            }
            #endregion

            #region Layer 3
            if (meshList_3 != null)
            {
                pointArrayList_3 = new List<PointF[]>();
                colorList_3 = new List<Color>();

                foreach (Mesh mesh in meshList_3)
                {
                    List<PointF> pointList = new List<PointF>();
                    Polyline[] outlines = mesh.GetOutlines(Plane.WorldXY);
                    MeshVertexColorList vertexColors = mesh.VertexColors;

                    if (vertexColors.Count > 0)
                    {
                        colorList_3.Add(vertexColors[0]);
                    }
                    else
                    {
                        colorList_3.Add(Color.Black);
                    }

                    foreach (Polyline outline in outlines)
                    {
                        Line[] lines = outline.GetSegments();

                        foreach (Line line in lines)
                        {
                            PointF start = new PointF((float)line.From.X, (float)line.From.Y);
                            PointF end = new PointF((float)line.To.X, (float)line.To.Y);

                            if (!pointList.Contains(start))
                            {
                                pointList.Add(end);
                            }

                            if (!pointList.Contains(end))
                            {
                                pointList.Add(end);
                            }

                            if (start.X > xMax) xMax = start.X;
                            if (start.Y > yMax) yMax = start.Y;
                            if (start.X < xMin) xMin = start.X;
                            if (start.Y < yMin) yMin = start.Y;

                            if (end.X > xMax) xMax = end.X;
                            if (end.Y > yMax) yMax = end.Y;
                            if (end.X < xMin) xMin = end.X;
                            if (end.Y < yMin) yMin = end.Y;
                        }
                    }

                    pointArrayList_3.Add(pointList.ToArray());
                }
            }
            #endregion

            int height = (int)(width * ((yMax - yMin) / (xMax - xMin)));

            Image image = new Bitmap(width, height);

            #region Draw Layer 1
            for (int i = 0; i < pointArrayList_1.Count; i++)
            {
                PointF[] pointArray = pointArrayList_1[i];

                for (int j = 0; j < pointArray.Length; j++)
                {
                    pointArray[j] = new PointF(image.Width * (pointArray[j].X - xMin) / (xMax - xMin), image.Height * (1 - (pointArray[j].Y - yMin) / (yMax - yMin)));
                }

                pointArrayList_1[i] = pointArray;
            }

            for (int i = 0; i < pointArrayList_1.Count; i++)
            {
                PointF[] pointArray = pointArrayList_1[i];
                Color color = colorList_1[i];

                using (Graphics g = Graphics.FromImage(image))
                    g.FillPolygon(new SolidBrush(color), pointArray);
            }
            #endregion

            #region Draw Layer 2
            if (meshList_2 != null)
            {
                for (int i = 0; i < pointArrayList_2.Count; i++)
                {
                    PointF[] pointArray = pointArrayList_2[i];

                    for (int j = 0; j < pointArray.Length; j++)
                    {
                        pointArray[j] = new PointF(image.Width * (pointArray[j].X - xMin) / (xMax - xMin), image.Height * (1 - (pointArray[j].Y - yMin) / (yMax - yMin)));
                    }

                    pointArrayList_2[i] = pointArray;
                }

                for (int i = 0; i < pointArrayList_2.Count; i++)
                {
                    PointF[] pointArray = pointArrayList_2[i];
                    Color color = colorList_2[i];

                    using (Graphics g = Graphics.FromImage(image))
                        g.FillPolygon(new SolidBrush(color), pointArray);
                }
            }
            #endregion

            #region Draw Layer 3
            if (meshList_3 != null)
            {
                for (int i = 0; i < pointArrayList_3.Count; i++)
                {
                    PointF[] pointArray = pointArrayList_3[i];

                    for (int j = 0; j < pointArray.Length; j++)
                    {
                        pointArray[j] = new PointF(image.Width * (pointArray[j].X - xMin) / (xMax - xMin), image.Height * (1 - (pointArray[j].Y - yMin) / (yMax - yMin)));
                    }

                    pointArrayList_3[i] = pointArray;
                }

                for (int i = 0; i < pointArrayList_3.Count; i++)
                {
                    PointF[] pointArray = pointArrayList_3[i];
                    Color color = colorList_3[i];

                    using (Graphics g = Graphics.FromImage(image))
                        g.FillPolygon(new SolidBrush(color), pointArray);
                }
            }
            #endregion

            //image.Save(@"C:\Users\TuHan\OneDrive - A-Insinöörit Oy\Työpöytä\TestImage_2.png");
            return image;
        }

        public static Image ConstructImageFromOneMesh(Mesh mesh, Image image)
        {
            List<PointF> pointList = new List<PointF>();
            PointF[] pointArray;
            Polyline[] outlines = mesh.GetOutlines(Plane.WorldXY);
            float xMax = float.MinValue;
            float yMax = float.MinValue;
            float xMin = float.MaxValue;
            float yMin = float.MaxValue;
            MeshVertexColorList vertexColors = mesh.VertexColors;
            Color color;

            if (vertexColors.Count > 0)
            {
                color = vertexColors[0];
            }
            else
            {
                color = Color.Black;
            }

            foreach (Polyline outline in outlines)
            {
                Line[] lines = outline.GetSegments();
                foreach (Line line in lines)
                {
                    Point3d startPoint = line.From;
                    Point3d endPoint = line.To;
                    PointF start = new PointF((float)startPoint.X, (float)startPoint.Y);
                    PointF end = new PointF((float)endPoint.X, (float)endPoint.Y);

                    if (!pointList.Contains(start))
                    {
                        pointList.Add(end);
                        if (start.X > xMax) xMax = start.X;
                        if (start.Y > yMax) yMax = start.Y;
                        if (start.X < xMin) xMin = start.X;
                        if (start.Y < yMin) yMin = start.Y;
                    }

                    if (!pointList.Contains(end))
                    {
                        pointList.Add(end);
                        if (end.X > xMax) xMax = end.X;
                        if (end.Y > yMax) yMax = end.Y;
                        if (end.X < xMin) xMin = end.X;
                        if (end.Y < yMin) yMin = end.Y;
                    }
                }
            }

            pointArray = pointList.ToArray();
            int width = image.Width;
            int height = (int)(image.Width * ((yMax - yMin) / (xMax - xMin)));

            image = new Bitmap(width, height);

            for (int i = 0; i < pointList.Count; i++)
            {
                pointArray[i] = new PointF(image.Width * pointList[i].X / xMax, image.Height * (1 - pointList[i].Y / yMax));
            }

            using (Graphics g = Graphics.FromImage(image))
                g.FillPolygon(new SolidBrush(color), pointArray);

            return image;
        }

        public static Image ConstructImage_test(Mesh mesh)
        {
            MeshVertexColorList colors = mesh.VertexColors;
            MeshVertexList vertices = mesh.Vertices;
            MeshFaceList faces = mesh.Faces;
            MeshTextureCoordinateList textureCoordinates = mesh.TextureCoordinates;

            Polyline[] edges = mesh.GetNakedEdges();
            Polyline[] outlines = mesh.GetOutlines(Plane.WorldXY);
            List<Point3d> edgePoints = new List<Point3d>();
            double xMax = double.MinValue;
            double yMax = double.MinValue;
            double xMin = double.MaxValue;
            double yMin = double.MaxValue;

            foreach (Polyline outline in outlines)
            {
                Line[] lines = outline.GetSegments();
                foreach (Line line in lines)
                {
                    Point3d startPoint = line.From;
                    Point3d endPoint = line.To;

                    if (!edgePoints.Contains(startPoint))
                    {
                        edgePoints.Add(startPoint);
                        if (startPoint.X > xMax) xMax = startPoint.X;
                        if (startPoint.Y > yMax) yMax = startPoint.Y;
                        if (startPoint.X < xMin) xMin = startPoint.X;
                        if (startPoint.Y < yMin) yMin = startPoint.Y;
                    }

                    if (!edgePoints.Contains(endPoint))
                    {
                        edgePoints.Add(endPoint);
                        if (endPoint.X > xMax) xMax = endPoint.X;
                        if (endPoint.Y > yMax) yMax = endPoint.Y;
                        if (endPoint.X < xMin) xMin = endPoint.X;
                        if (endPoint.Y < yMin) yMin = endPoint.Y;
                    }
                }
            }

            edgePoints = edgePoints.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            Point3d testPoint = new Point3d();
            MeshPoint meshPoint = mesh.ClosestMeshPoint(testPoint, 0.0);
            Brep brep = Brep.CreateFromMesh(mesh, true);
            PolylineCurve curve = outlines[0].ToPolylineCurve();
            brep = Brep.CreatePlanarBreps(curve, 0.001)[0];
            brep.ClosestPoint(testPoint);

            // 32,64,128,256,512,1024
            int imageSize = 1024;
            Bitmap image = new Bitmap(imageSize, imageSize);

            float[,] pixels = new float[imageSize, imageSize];

            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                for (int x = 0; x < pixels.GetLength(0); x++)
                {
                    image.SetPixel(x, y, Color.Black);
                }
            }

            PointF[] pointArray = new PointF[8];
            using (Graphics g = Graphics.FromImage(image))
                g.FillClosedCurve(Brushes.Black, pointArray);

            return image;
        }
    }
}
