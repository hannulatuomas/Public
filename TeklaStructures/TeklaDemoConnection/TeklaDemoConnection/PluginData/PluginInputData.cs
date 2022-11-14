using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Geometry3d;

namespace TeklaDemoConnection
{
    public class PluginInputData
    {
        public GeneralData GeneralData;
        public ColumnData ColumnData;
        public BeamData BeamData;

        public Point GetIntersectionPoint()
        {
            Point point = new Point();

            try
            {
                Line columnCenterLine = new Line(ColumnData.Column.StartPoint, ColumnData.Column.EndPoint);
                Line beamCenterLine = new Line(BeamData.Beam.StartPoint, BeamData.Beam.EndPoint);
                LineSegment segment = Intersection.LineToLine(columnCenterLine, beamCenterLine);
                point = segment.Point2;
                System.Windows.Forms.MessageBox.Show("Point = " + point.X + "; " + point.Y + "; " + point.Z);
            }
            catch (Exception Exc)
            {
                System.Windows.Forms.MessageBox.Show(Exc.Message);
            }

            return point;
        }

        public void SortBeamEnds()
        {
            Point point = BeamData.Beam.StartPoint;
            ArrayList beamCenter = BeamData.Beam.GetCenterLine(false);
            Vector distStart = new Vector(BeamData.Beam.StartPoint.X - ColumnData.Column.StartPoint.X, BeamData.Beam.StartPoint.Y - ColumnData.Column.StartPoint.Y, 0);
            Vector distEnd = new Vector(BeamData.Beam.EndPoint.X - ColumnData.Column.StartPoint.X, BeamData.Beam.EndPoint.Y - ColumnData.Column.StartPoint.Y, 0);

            if (distStart.GetLength() > distEnd.GetLength())
            {
                BeamData.Beam.StartPoint = BeamData.Beam.EndPoint;
                BeamData.Beam.StartPoint.Z = (beamCenter[0] as Point).Z;
                BeamData.Beam.EndPoint = point;
                BeamData.Beam.EndPoint.Z = (beamCenter[0] as Point).Z;
            }
        }

        public PluginInputData()
        {
            this.GeneralData = new GeneralData();
            this.ColumnData = new ColumnData();
            this.BeamData = new BeamData();
        }
    }
}
