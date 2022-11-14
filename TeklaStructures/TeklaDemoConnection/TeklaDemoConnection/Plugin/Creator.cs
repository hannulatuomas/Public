using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace TeklaDemoConnection
{
    internal class Creator
    {
        private TSM.TransformationPlane currentTFP;

        public TSM.Model Model { get; set; }
        private PluginInputData InputData { get; set; }

        internal Creator(TSM.Model model, PluginInputData inputData)
        {
            Model = model;
            InputData = inputData;
        }

        public void Create()
        {
            try
            {
                TSM.Component columnComponent = default(TSM.Component);
                TSM.Component beamComponent = default(TSM.Component);

                currentTFP = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                Point intersectionPoint = InputData.GetIntersectionPoint();
                InputData.SortBeamEnds();
                Vector columnVector = new Vector(InputData.ColumnData.Column.EndPoint - InputData.ColumnData.Column.StartPoint);
                Vector beamVector = new Vector(InputData.BeamData.Beam.EndPoint - InputData.BeamData.Beam.StartPoint);
                beamVector.Z = 0.0;
                Vector crossVector = Vector.Cross(beamVector, columnVector);
                crossVector.Z = 0.0;

                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(
                    new TSM.TransformationPlane(InputData.ColumnData.Column.StartPoint,
                            crossVector, beamVector));

                columnComponent = CreateColumnPart();

                Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(
                    new TSM.TransformationPlane(InputData.BeamData.Beam.EndPoint,
                            new Vector(InputData.BeamData.Beam.StartPoint - InputData.BeamData.Beam.EndPoint), crossVector));

                beamComponent = CreateBeamPart();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private TSM.Component CreateColumnPart()
        {
            TSM.Component component = new TSM.Component();

            try
            {
                component.Name = "PCsOneSided";
                component.Number = -100000;
                component.SetAttribute("Type", "COMPONENT");

                Point insertPoint = InputData.GetIntersectionPoint();
                insertPoint.Z -= InputData.BeamData.ProfileHeight / 2;
                insertPoint.Z += InputData.BeamData.BottomHeight;
                System.Windows.Forms.MessageBox.Show("InsertPoint = " + insertPoint.X + "; " + insertPoint.Y + "; " + insertPoint.Z);

                TSM.ComponentInput componentInput = new TSM.ComponentInput();
                componentInput.AddInputObject(InputData.ColumnData.Column);
                componentInput.AddOneInputPosition(insertPoint);
                component.SetComponentInput(componentInput);

                int createReinforcement = 1;
                if (InputData.GeneralData.CreateReinforcement < 2)
                {
                    createReinforcement = 0;
                }

                component.SetAttribute("CountryStandard", InputData.GeneralData.CountryStandard);
                component.SetAttribute("CoverThickness", InputData.ColumnData.CoverThickness);
                component.SetAttribute("CreateReinforcemnt", createReinforcement);
                component.SetAttribute("BoltHoleTolerance", 10.0);
                component.SetAttribute("DetailLevel", 0);
                component.SetAttribute("LoadClass", InputData.GeneralData.LoadClass);
                component.SetAttribute("LockVerticalLenght", 200.0);
                component.SetAttribute("NumberingType", InputData.GeneralData.NumberingType);
                component.SetAttribute("PCsCorbelType", InputData.ColumnData.cItem);
                component.SetAttribute("PCsEccentricity", 0.0);
                component.SetAttribute("PCsSubType", 0);
                component.SetAttribute("PeikkoBoltAttrFile", "standard");
                component.SetAttribute("PeikkoBoltName", "EB_PEIKKO_BOLT");
                component.SetAttribute("StartNumber", 1);
                component.SetAttribute("ShowWarnings", InputData.GeneralData.ShowWarnings);
                component.SetAttribute("StirrBendRadMult", 4.0);
                component.SetAttribute("StirrupClass", InputData.ColumnData.stirrup.tsClass);
                component.SetAttribute("StirrupGrade", InputData.ColumnData.stirrup.grade);
                component.SetAttribute("StirrupName", InputData.ColumnData.stirrup.name);
                component.SetAttribute("StirrupStartNumber", InputData.ColumnData.stirrup.no);
                component.SetAttribute("UBarClass", InputData.ColumnData.uBar.tsClass);
                component.SetAttribute("UBarGrade", InputData.ColumnData.uBar.grade);
                component.SetAttribute("UBarName", InputData.ColumnData.uBar.name);
                component.SetAttribute("UBarStartNumber", InputData.ColumnData.uBar.no);
                component.SetAttribute("xs_command_1", 4);
                component.SetAttribute("xs_command_2", 1);
                component.SetAttribute("xs_nobjects_1", 1);
                component.SetAttribute("xs_nobjects_1", 1);

                if (!component.Insert())
                {
                    System.Windows.Forms.MessageBox.Show("Component failed");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Component Done");
                }
            }
            catch (Exception Exc)
            {
                System.Windows.Forms.MessageBox.Show("Component failed: " + Exc.Message);
            }

            return component;
        }

        private TSM.Component CreateBeamPart()
        {
            TSM.Component component = new TSM.Component();

            try
            {
                string type = "L";
                if (InputData.BeamData.bItem == 1) type = "H";
                component.Name = "PeikkoPcBeamShoe";
                component.Number = -100000;
                component.SetAttribute("Type", "COMPONENT");

                Point insertPoint = InputData.GetIntersectionPoint();
                insertPoint.Z -= InputData.BeamData.ProfileHeight / 2;
                System.Windows.Forms.MessageBox.Show("InsertPoint = " + insertPoint.X + "; " + insertPoint.Y + "; " + insertPoint.Z);

                TSM.ComponentInput componentInput = new TSM.ComponentInput();
                componentInput.AddInputObject(InputData.BeamData.Beam);
                componentInput.AddOneInputPosition(insertPoint);
                component.SetComponentInput(componentInput);

                int createReinforcement = 1;
                if (InputData.GeneralData.CreateReinforcement < 1)
                {
                    createReinforcement = 0;
                }

                component.SetAttribute("BeamBottomToCorbel", InputData.BeamData.BottomHeight);
                component.SetAttribute("BeamCoverTBottom", InputData.BeamData.CoverThicknessBottom);
                component.SetAttribute("BeamCoverTLeft", InputData.BeamData.CoverThicknessLeft);
                component.SetAttribute("BeamCoverTRight", InputData.BeamData.CoverThicknessRight);
                component.SetAttribute("BeamCoverTTop", InputData.BeamData.CoverThicknessTop);
                component.SetAttribute("BStirrupClass", InputData.BeamData.stirrup.tsClass);
                component.SetAttribute("BStirrupGrade", InputData.BeamData.stirrup.grade);
                component.SetAttribute("BStirrupName", InputData.BeamData.stirrup.name);
                component.SetAttribute("BStirrupSrtNmb", InputData.BeamData.stirrup.no);
                component.SetAttribute("BUBarClass", InputData.BeamData.uBar.tsClass);
                component.SetAttribute("BUBarGrade", InputData.BeamData.uBar.grade);
                component.SetAttribute("BUBarName", InputData.BeamData.uBar.name);
                component.SetAttribute("BUBarStartNumber", InputData.BeamData.uBar.no);
                component.SetAttribute("CreateReinforcemnt", createReinforcement);
                component.SetAttribute("DistanceOver100", InputData.GeneralData.ShoeAnchorDist);
                component.SetAttribute("NumberingType", InputData.GeneralData.NumberingType);
                component.SetAttribute("PCEccentricity", 0.0);
                component.SetAttribute("ShoeName", InputData.GeneralData.LoadClass);
                component.SetAttribute("ShowWarnings", InputData.GeneralData.ShowWarnings);
                component.SetAttribute("StirrBendRadMult", 4.0);
                component.SetAttribute("Type", type);
                component.SetAttribute("xs_command_1", 4);
                component.SetAttribute("xs_command_2", 1);
                component.SetAttribute("xs_nobjects_1", 1);
                component.SetAttribute("xs_nobjects_2", 1);


                if (!component.Insert())
                {
                    System.Windows.Forms.MessageBox.Show("Component failed");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Component Done");
                }
            }
            catch (Exception Exc)
            {
                System.Windows.Forms.MessageBox.Show("Component failed: " + Exc.Message);
            }

            return component;
        }
    }
}
