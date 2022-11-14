using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;

namespace Stabiliteettilaskenta.TEKLA
{
    class TeklaMoveTools
    {
        public void MoveToPoint()
        {
            Point point = PickObjects.GetPoint();
            List<Beam> beams = PickObjects.GetSelectedBeams();
            foreach (Beam beam in beams)
            {
                Move2Point(beam, point);
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        private void Move2Point(Beam beam, Point point)
        {
            beam.Select();
            double dist1 = Distance.PointToPoint(beam.StartPoint, point);
            double dist2 = Distance.PointToPoint(beam.EndPoint, point);
            if (Math.Abs(dist1 - dist2) < 0.0001)
                return;
            if (dist1 < dist2)
            {
                beam.StartPoint = point;
            }
            else
            {
                beam.EndPoint = point;
            }
            beam.Modify();
        }

        public void MoveToPlane()
        {
            GeometricPlane plane = PickObjects.GetPlane();
            List<Beam> beams = PickObjects.GetSelectedBeams();
            if (plane == null || beams.Count < 1)
                return;
            foreach (Beam beam in beams)
            {
                beam.Select();
                Point point1 = Projection.PointToPlane(beam.StartPoint, plane);
                Point point2 = Projection.PointToPlane(beam.EndPoint, plane);
                if (Point.AreEqual(point1, point2))
                    continue;
                beam.StartPoint = point1;
                beam.EndPoint = point2;

                beam.Modify();
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void ExtendToLine(bool toLine)
        {
            Line line = PickObjects.GetLineFromBeam();
            List<Beam> beams = PickObjects.GetSelectedBeams();
            if (line == null || beams.Count < 1)
                return;
            foreach (Beam beam in beams)
            {
                beam.Select();
                Line line2 = new Line(beam.StartPoint, beam.EndPoint);
                LineSegment segment = Intersection.LineToLine(line, line2);
                if (segment == null)
                    continue;
                if (toLine)
                {
                    Move2Point(beam, segment.Point1);
                }
                else
                {
                    Move2Point(beam, segment.Point2);
                }
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void ExtendToPlane()
        {
            GeometricPlane plane = PickObjects.GetPlane();
            List<Beam> beams = PickObjects.GetSelectedBeams();
            if (plane == null || beams.Count < 1)
                return;
            foreach (Beam beam in beams)
            {
                beam.Select();
                Point point = Intersection.LineToPlane(new Line(beam.StartPoint, beam.EndPoint), plane);
                if (point == null)
                    continue;
                Move2Point(beam, point);
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void ProjectToLine()
        {
            Line line = PickObjects.GetLineFromBeam();
            List<Beam> beams = PickObjects.GetSelectedBeams();
            if (line == null || beams.Count < 1)
                return;
            foreach (Beam beam in beams)
            {
                beam.Select();
                Point point1 = Projection.PointToLine(beam.StartPoint, line);
                Point point2 = Projection.PointToLine(beam.EndPoint, line);
                double dist1 = Distance.PointToPoint(beam.StartPoint, point1);
                double dist2 = Distance.PointToPoint(beam.EndPoint, point2);
                if (Math.Abs(dist1 - dist2) < 0.0001)
                    continue;
                if (dist1 < dist2)
                {
                    beam.StartPoint = point1;
                }
                else
                {
                    beam.EndPoint = point2;
                }
                beam.Modify();
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void ProjectToPlane()
        {
            GeometricPlane plane = PickObjects.GetPlane();
            List<Beam> beams = PickObjects.GetSelectedBeams();
            if (plane == null || beams.Count < 1)
                return;
            foreach (Beam beam in beams)
            {
                beam.Select();
                Point point1 = Projection.PointToPlane(beam.StartPoint, plane);
                Point point2 = Projection.PointToPlane(beam.EndPoint, plane);
                double dist1 = Distance.PointToPoint(beam.StartPoint, point1);
                double dist2 = Distance.PointToPoint(beam.EndPoint, point2);
                if (Math.Abs(dist1 - dist2) < 0.0001)
                    continue;
                if (dist1 < dist2)
                {
                    beam.StartPoint = point1;
                }
                else
                {
                    beam.EndPoint = point2;
                }
                beam.Modify();
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void MoveBetween2Points()
        {
            List<Beam> beams = PickObjects.GetSelectedBeams();
            Point point1 = PickObjects.GetPoint();
            Point point2 = PickObjects.GetPoint();
            Vector dist = new Vector(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
            Point midPoint = new Point(point2.X + dist.X / 2, point2.Y + dist.Y / 2, point2.Z + dist.Z / 2);
            foreach (Beam beam in beams)
            {
                Move2Point(beam, midPoint);
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void ModifyFitting(Plane plane)
        {
            Picker picker = new Picker();
            Fitting fitting = (Fitting)picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick a Fitting");

            fitting.Plane = plane;

            try
            {
                fitting.Modify();
                Model model = new Model();
                model.CommitChanges();
            }
            catch (Exception exc) { }
        }

        public void RotateBeam()
        {
            List<Beam> beams = PickObjects.GetSelectedBeams();
            Point basePoint = PickObjects.GetPoint();
            Point pointOfPart = PickObjects.GetPoint();
            Point newPoint = PickObjects.GetPoint();
            
            Plane plane = new Plane();
            plane.Origin = basePoint;
            plane.AxisX = new Vector(pointOfPart - basePoint);
            plane.AxisY = new Vector(newPoint - basePoint);
            Vector normal = Vector.Cross(plane.AxisX, plane.AxisY);

            double angle = Utility.Utility.GetVectorAngle(plane.AxisX, plane.AxisY);

            foreach (Beam beam in beams)
            {
                beam.Select();
                double rotation = beam.Position.RotationOffset;
                beam.Position.RotationOffset = rotation + angle;
                //Move2Point(beam, basePoint);
                beam.Modify();
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }

        public void CloseGap()
        {
            List<Beam> beams = PickObjects.GetBeams();
            if (beams == null || beams.Count < 2)
                return;
            for (int i = 0; i < beams.Count; i += 2)
            {
                Beam beam1 = beams[i];
                Beam beam2 = beams[i + 1];

                double dist1 = Distance.PointToPoint(beam1.StartPoint, beam2.StartPoint);
                double dist2 = Distance.PointToPoint(beam1.StartPoint, beam2.EndPoint);
                double dist3 = Distance.PointToPoint(beam1.EndPoint, beam2.StartPoint);
                double dist4 = Distance.PointToPoint(beam1.EndPoint, beam2.EndPoint);

                Point point1 = null;
                Point point2 = null;

                if (dist1 < dist2)
                {
                    if (dist1 < dist3)
                    {
                        if (dist1 < dist4)
                        {
                            // dist1 => Move: beam1.StartPoint, beam2.StartPoint
                            point1 = beam1.StartPoint;
                            point2 = beam2.StartPoint;
                        }
                        else
                        {
                            // dist4 => Move: beam1.EndPoint, beam2.EndPoint
                            point1 = beam1.EndPoint;
                            point2 = beam2.EndPoint;
                        }
                    }
                    else
                    {
                        // dist3 < dist1
                        if (dist3 < dist2)
                        {
                            // dist3 < (dist1 & dist2)
                            if (dist3 < dist4)
                            {
                                // dist3 => Move: beam1.EndPoint, beam2.StartPoint
                                point1 = beam1.EndPoint;
                                point2 = beam2.StartPoint;
                            }
                            else
                            {
                                // dist4 => Move: beam1.EndPoint, beam2.EndPoint
                                point1 = beam1.EndPoint;
                                point2 = beam2.EndPoint;
                            }
                        }
                        else
                        {
                            // dist2 < (dist1 & dist3)
                            if (dist2 < dist4)
                            {
                                // dist2 => Move: beam1.StartPoint, beam2.EndPoint
                                point1 = beam1.StartPoint;
                                point2 = beam2.EndPoint;
                            }
                            else
                            {
                                // dist4 => Move: beam1.EndPoint, beam2.EndPoint
                                point1 = beam1.EndPoint;
                                point2 = beam2.EndPoint;
                            }
                        }
                    }
                }
                else if (dist2 < dist3)
                {
                    // dist2 < (dist1 & dist3)
                    if (dist2 < dist4)
                    {
                        // dist2 => Move: beam1.StartPoint, beam2.EndPoint
                        point1 = beam1.StartPoint;
                        point2 = beam2.EndPoint;
                    }
                    else
                    {
                        // dist4 => Move: beam1.EndPoint, beam2.EndPoint
                        point1 = beam1.EndPoint;
                        point2 = beam2.EndPoint;
                    }
                }                
                else if (dist3 < dist4)
                {
                    // dist3 => Move: beam1.EndPoint, beam2.StartPoint
                    point1 = beam1.EndPoint;
                    point2 = beam2.StartPoint;
                }
                else
                {
                    // dist4 => Move: beam1.EndPoint, beam2.EndPoint
                    point1 = beam1.EndPoint;
                    point2 = beam2.EndPoint;
                }

                Vector dist = new Vector(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
                Point midPoint = new Point(point2.X + dist.X / 2, point2.Y + dist.Y / 2, point2.Z + dist.Z / 2);
                
                Move2Point(beam1, midPoint);
                Move2Point(beam2, midPoint);
            }
            Model myModel = new Model();
            myModel.CommitChanges();
        }
    }
}
