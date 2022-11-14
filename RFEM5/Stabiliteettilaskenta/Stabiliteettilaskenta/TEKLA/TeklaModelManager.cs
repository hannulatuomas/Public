using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Analysis;
using System.Numerics;
using System.Collections;

namespace Stabiliteettilaskenta.TEKLA
{
    class TeklaModelManager
    {
        public void AddBeam(Vector3 _startPoint, Vector3 _endPoint, string _profile = "IPE200", string _material = "S355J2", string _name = "BEAM", string _class = "120")
        {
            Model model = new Model();
            Point startPoint = new Point(_startPoint.X, _startPoint.Y, _startPoint.Z);
            Point endPoint = new Point(_endPoint.X, _endPoint.Y, _endPoint.Z);
            
            Beam beam = new Beam(startPoint, endPoint);
            beam.Name = _name;
            beam.Class = _class;

            Profile profile = new Profile { ProfileString = _profile };
            beam.Profile = profile;
            Material material = new Material { MaterialString = _material };
            beam.Material = material;

            Position position = new Position();
            position.Plane = Position.PlaneEnum.MIDDLE;
            position.Rotation = Position.RotationEnum.TOP;
            position.Depth = Position.DepthEnum.MIDDLE;
            beam.Position = position;

            beam.Insert();

            beam.Select();
            if (beam.Type == Beam.BeamTypeEnum.COLUMN)
            {
                beam.Name = "COLUMN";
            }

            beam.Modify();

            model.CommitChanges();
        }

        public void AddSlab(Vector3[] _cornerPoints, string _thickness)
        {
            Model model = new Model();
            List<Point> points = new List<Point>();

            foreach (Vector3 corner in _cornerPoints)
            {
                Point point = new Point(corner.X, corner.Y, corner.Z);
                points.Add(point);
            }

            ContourPlate cPlate = new ContourPlate();
            
            Profile profile = new Profile { ProfileString = _thickness };
            cPlate.Profile = profile;
            
            Position position = new Position();

            cPlate.Position = position;

            Contour contour = new Contour();
            
            cPlate.Contour = contour;


        }

        public void GetPartData()
        {
            Tekla.Structures.Model.UI.ModelObjectSelector modelObjectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var selector = modelObjectSelector.GetSelectedObjects();

            foreach (ModelObject item in selector)
            {
                Beam b = item as Beam;
                ContourPlate c = item as ContourPlate;
                Component component = item as Component;

                if (item.GetType() == typeof(LoadArea))
                {
                    int i = 1;
                }

                if (b != null)
                {
                    Point p = b.EndPoint;
                }
                else if (c != null)
                {
                    ArrayList arr = c.Contour.ContourPoints;
                }
                else if (component != null)
                {
                    int x = component.Identifier.ID;
                    ComponentInput input = component.GetComponentInput();
                }
                else
                {
                    int a = item.Identifier.ID;
                }
            }
        }
    }
}
