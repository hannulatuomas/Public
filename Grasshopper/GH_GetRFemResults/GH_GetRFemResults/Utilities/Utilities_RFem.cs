using Dlubal.RFEM5;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.Utilities
{
    public static class Utilities_RFem
    {
        public static Curve RfLineToGhCurve(Dlubal.RFEM5.Line line, IModelData iModelData)
        {
            switch (line.Type)
            {
                case LineType.PolylineType:
                    if (line.NodeCount == 2)
                    {
                        int cpCount = line.ControlPoints.Length;
                        Point3d startPoint = new Point3d();
                        startPoint.X = line.ControlPoints[0].X;
                        startPoint.Y = line.ControlPoints[0].Y;
                        startPoint.Z = line.ControlPoints[0].Z;
                        Point3d endPoint = new Point3d();
                        endPoint.X = line.ControlPoints[cpCount - 1].X;
                        endPoint.Y = line.ControlPoints[cpCount - 1].Y;
                        endPoint.Z = line.ControlPoints[cpCount - 1].Z;
                        return new LineCurve(startPoint, endPoint);
                    }
                    else
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        return new PolylineCurve(controlPoints);
                    }
                case LineType.SplineType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        // Warning! Degree HardCoded !!
                        return Curve.CreateInterpolatedCurve(controlPoints, 3);
                    }
                case LineType.CircleType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        return new ArcCurve(new Circle(controlPoints[0], controlPoints[1], controlPoints[2]));
                    }
                case LineType.ArcType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        return Curve.CreateInterpolatedCurve(controlPoints, 3, CurveKnotStyle.Chord);
                    }
                case LineType.EllipseType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        // Warning
                        return new Ellipse((controlPoints[0] + controlPoints[2]) / 2, controlPoints[0], controlPoints[1]).ToNurbsCurve();
                    }
                case LineType.EllipticalArcType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        return Curve.CreateInterpolatedCurve(controlPoints, 3);
                    }
                case LineType.ParabolaType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        return Curve.CreateInterpolatedCurve(controlPoints, 3);
                    }
                case LineType.HyperbolaType:
                    {
                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }
                        return Curve.CreateInterpolatedCurve(controlPoints, 3);
                    }
                case LineType.NurbSplineType:
                    {
                        INurbSpline iNurbSpline = iModelData.GetNurbSpline(line.No, ItemAt.AtNo);
                        NurbSpline nurbSpline = iNurbSpline.GetExtraData();

                        Point3d[] controlPoints = new Point3d[line.ControlPoints.Length];
                        for (int i = 0; i < line.ControlPoints.Length; i++)
                        {
                            controlPoints[i].X = line.ControlPoints[i].X;
                            controlPoints[i].Y = line.ControlPoints[i].Y;
                            controlPoints[i].Z = line.ControlPoints[i].Z;
                        }

                        NurbsCurve nurbsCurve = NurbsCurve.Create(false, nurbSpline.Order - 1, controlPoints);
                        for (int j = 0; j < nurbsCurve.Points.Count; j++)
                        {
                            nurbsCurve.Points[j] = new ControlPoint(controlPoints[j], nurbSpline.Weights[j]);
                        }
                        for (int k = 0; k < nurbsCurve.Knots.Count; k++)
                        {
                            nurbsCurve.Knots[k] = nurbSpline.Knots[k + 1];
                        }
                        return nurbsCurve;
                    }
            }
            return null;
        }
    }
}
