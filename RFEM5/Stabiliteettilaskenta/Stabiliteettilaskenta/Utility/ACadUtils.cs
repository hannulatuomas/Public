using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AutoCAD;

namespace Stabiliteettilaskenta.Utility
{
    public static class ACadUtils
    {
        public static T[] GetEntities<T>(AcadDocument _cadDocument, string _layer = "") //where T : AcadObject
        {
            AcadSelectionSet selectionSet;

            //SelectionSet
            for (int i = 0; i < _cadDocument.SelectionSets.Count; i++)
            {
                if (_cadDocument.SelectionSets.Item(i).Name == "GetEntities")
                {
                    _cadDocument.SelectionSets.Item(i).Delete();
                    break;
                }
            }

            selectionSet = _cadDocument.SelectionSets.Add("GetEntities");
            selectionSet.Select(AcSelect.acSelectionSetAll);
            T[] arr = selectionSet.OfType<T>().ToArray();

            if (_layer != "" && arr[0] is AcadEntity)
            {
                List<T> temp = new List<T>();

                foreach (T item in arr)
                {
                    AcadEntity acadObject = item as AcadEntity;
                    if (acadObject.Layer == _layer)
                    {
                        temp.Add(item);
                    }
                }
                arr = temp.ToArray();
            }

            return arr;
        }

        public static Line[] GetLines(AcadDocument _cadDocument, string _layer = "")
        {
            AcadSelectionSet selectionSet;

            //SelectionSet
            for (int i = 0; i < _cadDocument.SelectionSets.Count; i++)
            {
                if (_cadDocument.SelectionSets.Item(i).Name == "GetEntities")
                {
                    _cadDocument.SelectionSets.Item(i).Delete();
                    break;
                }
            }

            selectionSet = _cadDocument.SelectionSets.Add("GetEntities");
            selectionSet.Select(AcSelect.acSelectionSetAll);
            AcadLine[] arr = selectionSet.OfType<AcadLine>().ToArray();

            if (_layer != "")
            {
                List<AcadLine> temp = new List<AcadLine>();

                foreach (AcadLine item in arr)
                {
                    if (item.Layer == _layer)
                    {
                        temp.Add(item);
                    }
                }
                arr = temp.ToArray();
            }

            Line[] lineArr = new Line[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                Vector3 start = new Vector3((float)arr[i].StartPoint[0], (float)arr[i].StartPoint[1], (float)arr[i].StartPoint[2]);
                Vector3 end = new Vector3((float)arr[i].EndPoint[0], (float)arr[i].EndPoint[1], (float)arr[i].EndPoint[2]);
                lineArr[i] = new Line(start, end);
            }
            return lineArr;
        }
    }

    public struct Line
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;

        public Line(Vector3 _startPoint, Vector3 _endPoint)
        {
            StartPoint = _startPoint;
            EndPoint = _endPoint;
        }
    }
}
