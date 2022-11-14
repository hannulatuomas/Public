using System;
using System.Collections.Generic;
using System.Numerics;
using Stabiliteettilaskenta.Utility;

namespace Stabiliteettilaskenta.MODEL
{
    public class ModelManager
    {
        //List<float> elevations;
        //List<Point> points;
        //List<Line> lines;
        //List<Member> members;
        //List<Surface> surfaces;
        //List<Load> loads;

        //ModelManager()
        //{
        //    elevations = new List<float>();
        //    points = new List<Point>();
        //    lines = new List<Line>();
        //    members = new List<Member>();
        //    surfaces = new List<Surface>();
        //    loads = new List<Load>();
        //}

        #region Add Data
        public void AddPoint(Vector3 _point)
        {
            //if (_point == Vector3.Zero)
            //{
            //    points.Add(new Point(_point));
            //}
        }
        public void AddLine(Vector3 _startPoint, Vector3 _endPoint)
        {

        }
        public void AddMember(Vector3 _startPoint, Vector3 _endPoint, string _crossSection, string _material)
        {

        }
        public void AddSurface(Vector3[] _points, float _thickness, string _material)
        {

        }
        public void AddPointLoad(Vector3 _point, float _value)
        {

        }
        public void AddMemberLoad(Vector3 _startPoint, Vector3 _endPoint, float _value)
        {

        }
        public void AddSurfaceLoad(Vector3[] _points, float _value)
        {

        }

        #endregion

        #region Get Data
        public void GetPoint()
        {

        }
        public void GetLine()
        {

        }
        public void GetMember()
        {

        }
        public void GetSurface()
        {

        }
        public void GetPointLoad()
        {

        }
        public void GetMemberLoad()
        {

        }
        public void GetSurfaceLoad()
        {

        }

        #endregion

        #region Delete Data
        public void DeletePoint()
        {

        }
        public void DeleteLine()
        {

        }
        public void DeleteMember()
        {

        }
        public void DeleteSurface()
        {

        }
        public void DeletePointLoad()
        {

        }
        public void DeleteMemberLoad()
        {

        }
        public void DeleteSurfaceLoad()
        {

        }

        #endregion

    }

    public abstract class ModelItem : IHeapItem<ModelItem>
    {
        public int HeapIndex { get; set; }
        public int Number { get; private set; }

        public virtual void Add()
        {

        }

        public virtual int CompareTo(ModelItem other)
        {
            int compare = Number.CompareTo(other.Number);
            if (compare == 0)
            {

            }
            return -compare;    // 1 = is less than other
        }
    }

    //public class Point
    //{
    //    int number;
    //    Vector3 coordinates;
    //    List<Line> lines;
    //    List<Member> members;
    //    List<Surface> surfaces;

    //    public Point(Vector3 _coordinates)
    //    {
    //        coordinates = _coordinates;
    //        lines = new List<Line>();
    //        members = new List<Member>();
    //        surfaces = new List<Surface>();
    //    }

    //    public Vector3 Coordinates { get => coordinates; }

    //    public void AddLine(Line _line)
    //    {
    //        if (!lines.Contains(_line))
    //        {
    //            lines.Add(_line);
    //        }
    //    }
    //    public void AddMember(Member _member)
    //    {
    //        if (!members.Contains(_member))
    //        {
    //            members.Add(_member);
    //        }
    //    }
    //    public void AddSurface(Surface _surface)
    //    {
    //        if (!surfaces.Contains(_surface))
    //        {
    //            surfaces.Add(_surface);
    //        }
    //    }
    //}
    //public class Line
    //{
    //    int number;
    //    Point startPoint;
    //    Point endPoint;
    //    List<Member> members;
    //    List<Surface> surfaces;

    //    public Line(Point _startPoint, Point _endPoint)
    //    {
    //        startPoint = _startPoint;
    //        endPoint = _endPoint;
    //        members = new List<Member>();
    //        surfaces = new List<Surface>();

    //        startPoint.AddLine(this);
    //        endPoint.AddLine(this);
    //    }

    //    public float Length
    //    {
    //        get
    //        {
    //            return Vector3.Distance(startPoint.Coordinates, endPoint.Coordinates);
    //        }
    //    }

    //    public Point StartPoint { get => startPoint; }
    //    public Point EndPoint { get => endPoint; }

    //    public void AddMember(Member _member)
    //    {
    //        if (!members.Contains(_member))
    //        {
    //            members.Add(_member);
    //        }
    //    }
    //    public void AddSurface(Surface _surface)
    //    {
    //        if (!surfaces.Contains(_surface))
    //        {
    //            surfaces.Add(_surface);
    //        }
    //    }
    //}
    //public class Member
    //{
    //    int number;
    //    Line line;
    //    string crossSection;
    //    string material;

    //    public Member(Line _line)
    //    {
    //        line = _line;

    //        line.AddMember(this);
    //    }
    //}
    //public class Surface
    //{
    //    int number;
    //    List<Point> points;
    //    Line[] lines;
    //    float thickness;
    //    string material;

    //    public Surface(Line[] _lines)
    //    {
    //        lines = _lines;
    //        points = new List<Point>();

    //        for (int i = 0; i < lines.Length; i++)
    //        {
    //            lines[i].AddSurface(this);

    //            if (!points.Contains(lines[i].StartPoint))
    //            {
    //                points.Add(lines[i].StartPoint);
    //                lines[i].StartPoint.AddLine(lines[i]);
    //            }
    //            if (!points.Contains(lines[i].EndPoint))
    //            {
    //                points.Add(lines[i].EndPoint);
    //                lines[i].EndPoint.AddLine(lines[i]);
    //            }
    //        }
    //    }
    //}
}
