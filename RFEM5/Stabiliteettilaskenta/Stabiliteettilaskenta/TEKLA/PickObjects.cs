using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;
using System.Collections;

namespace Stabiliteettilaskenta.TEKLA
{
    public static class PickObjects
    {
        public static Point GetPoint()
        {
            Point point = null;
            try
            {
                Picker picker = new Picker();
                point = picker.PickPoint("Pick a point.");
            }
            catch (ApplicationException e)
            {

                e.GetType();
            }
            return point;
        }

        public static Line GetLine()
        {
            Point point1 = GetPoint();
            if (point1 == null) return null;

            Point point2 = GetPoint();
            if (point2 == null) return null;

            Line line = new Line(point1, point2);

            return line;
        }

        public static Line GetLineFromBeam()
        {
            Beam beam = GetBeam();
            if (beam == null) return null;

            Line line = new Line(beam.StartPoint, beam.EndPoint);

            return line;
        }

        public static Beam GetBeam()
        {
            ModelObject modelObject;
            Beam beam = null;
            try
            {
                Picker picker = new Picker();
                modelObject = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick a BEAM.") as Beam;
            }
            catch (ApplicationException e)
            {
                e.GetType();
                return null;
            }
            if (modelObject is Beam)
            {
                beam = (Beam)modelObject;
            }
            return beam;
        }

        public static List<Beam> GetBeams()
        {
            ModelObjectEnumerator modelObjects;
            List<Beam> beamList = new List<Beam>();

            try
            {
                Picker picker = new Picker();
                modelObjects = picker.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick BEAMS.");
            }
            catch (ApplicationException e)
            {
                e.GetType();
                return null;
            }
            foreach (ModelObject modelObject in modelObjects)
            {
                if (modelObject is Beam)
                {
                    beamList.Add((Beam)modelObject);
                }
            }
            
            return beamList;
        }

        public static GeometricPlane GetPlane()
        {
            ModelObject modelObject1;
            ModelObject modelObject2;
            GeometricPlane plane = null;
            try
            {
                Picker picker = new Picker();
                modelObject1 = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick the First BEAM.") as Beam;
                modelObject2 = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick the Second BEAM.") as Beam;
            }
            catch (ApplicationException e)
            {
                e.GetType();
                return null;
            }
            Beam beam1 = (Beam)modelObject1;
            Beam beam2 = (Beam)modelObject2;
            if (beam1 != null && beam2 != null)
            {
                Vector vector1 = new Vector(beam1.StartPoint.X - beam1.EndPoint.X, beam1.StartPoint.Y - beam1.EndPoint.Y, beam1.StartPoint.Z - beam1.EndPoint.Z);
                Vector vector2 = new Vector(beam2.StartPoint.X - beam2.EndPoint.X, beam2.StartPoint.Y - beam2.EndPoint.Y, beam2.StartPoint.Z - beam2.EndPoint.Z);
                if (!Tekla.Structures.Geometry3d.Parallel.VectorToVector(vector1, vector2))
                {
                    plane = new GeometricPlane(beam1.StartPoint, vector1, vector2);
                }
            }
            return plane;
        }

        public static List<Beam> GetSelectedBeams()
        {
            List<Beam> beams = new List<Beam>();
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();

            selectedObjects.SelectInstances = true;
            foreach (ModelObject modelObject in selectedObjects)
            {
                Beam beam = (Beam)modelObject;
                if (beam != null)
                    beams.Add(beam);
            }
            return beams;
        }

        public static ArrayList GetFace()
        {
            Picker picker = new Picker();
            PickInput obj = picker.PickFace("Pick a face");
            ArrayList points = new ArrayList();

            IEnumerator myEnum = obj.GetEnumerator();
            while (myEnum.MoveNext())
            {
                InputItem item = myEnum.Current as InputItem;
                if (item.GetInputType() == InputItem.InputTypeEnum.INPUT_1_OBJECT)
                {
                    ModelObject modelObj = item.GetData() as ModelObject;
                    if (modelObj.GetType().IsAssignableFrom(typeof(Beam)))
                    {
                        Beam beam = item.GetData() as Beam;
                    }
                    else if (modelObj.GetType().IsAssignableFrom(typeof(ContourPlate)))
                    {
                        ContourPlate contourPlate = item.GetData() as ContourPlate;
                    }
                    else if (modelObj.GetType().IsAssignableFrom(typeof(PolyBeam)))
                    {
                        PolyBeam polyBeam = item.GetData() as PolyBeam;
                    }
                }
                else if (item.GetInputType() == InputItem.InputTypeEnum.INPUT_POLYGON)
                {
                    points = item.GetData() as ArrayList;
                }
            }
            return points;
        }
    }
}
