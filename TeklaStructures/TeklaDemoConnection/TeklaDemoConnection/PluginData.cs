using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;

namespace TeklaConcreteConnection
{
    public class PluginData
    {
        #region Fields

        [StructuresField("d_input_1")]//ok
        public double _input_1;

        #endregion
    }

    [Plugin("TeklaConcreteConnection")]
    [PluginUserInterface("TeklaConcreteConnection.MainForm")]
    public class ConcreteConnection : PluginBase
    {
        #region Fields

        private TSM.Model _Model;
        private PluginData _Data;
        private PluginInputData _PluginInputData;
        //
        // Define variables for the field values.
        //
        /* Some examples:
        private string _RebarName = string.Empty;
        private string _RebarSize = string.Empty;
        private string _RebarGrade = string.Empty;
        private ArrayList _RebarBendingRadius = new ArrayList();
        private int _RebarClass = new int();
        private double _RebarSpacing;
        */

        private TSM.TransformationPlane currentTFP;
        private TSM.TransformationPlane pluginTFP;
        private TSG.CoordinateSystem pluginCoordSystem;
        private TSG.CoordinateSystem modelCoordSystem;

        private TSM.Assembly columnAssembly;
        private TSM.Assembly beamAssembly;

        private TSM.Beam columnPart;
        private TSM.Beam beamPart;

        #endregion

        #region Properties

        private TSM.Model Model
        {
            get { return this._Model; }
            set { this._Model = value; }
        }

        private PluginData Data
        {
            get { return this._Data; }
            set { this._Data = value; }
        }

        private PluginInputData InputData
        {
            get { return this._PluginInputData; }
            set { this._PluginInputData = value; }
        }

        #endregion

        #region Constructors

        public ConcreteConnection(PluginData data)
        {
            Model = new TSM.Model();
            Data = data;
            InputData = new PluginInputData();
        }

        #endregion

        #region Overrides

        public override List<InputDefinition> DefineInput()
        {
            TSM.Assembly columnAssembly = null;
            TSM.Assembly beamAssembly = null;

            TSM.UI.Picker picker_column = new TSM.UI.Picker();
            try
            {
                columnAssembly = (TSM.Assembly)picker_column.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "Select a Column.");
                TSM.Beam test = (TSM.Beam)columnAssembly.GetMainPart();
                if (test.Type != TSM.Beam.BeamTypeEnum.COLUMN)
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception ex)
            {
                return new List<InputDefinition>();
            }

            TSM.UI.Picker picker_beam = new TSM.UI.Picker();
            try
            {
                beamAssembly = (TSM.Assembly)picker_beam.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "Select a Beam.");
                TSM.Beam test = (TSM.Beam)beamAssembly.GetMainPart();
                if (test.Type != TSM.Beam.BeamTypeEnum.BEAM)
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception ex)
            {
                return new List<InputDefinition>();
            }

            List<InputDefinition> inputs = new List<InputDefinition>();
            InputDefinition Input1 = new InputDefinition(columnAssembly.Identifier);
            InputDefinition Input2 = new InputDefinition(beamAssembly.Identifier);
            inputs.Add(Input1);
            inputs.Add(Input2);

            return inputs;
        }

        public override bool Run(List<InputDefinition> Input)
        {
            currentTFP = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
            try
            {
                if (Input.Count < 3)
                {
                    throw new ArgumentException();
                }
                GetValuesFromDialog();

                columnAssembly = (TSM.Assembly)Model.SelectModelObject((Tekla.Structures.Identifier)Input[0].GetInput());
                columnPart = (TSM.Beam)columnAssembly.GetMainPart();
                beamAssembly = (TSM.Assembly)Model.SelectModelObject((Tekla.Structures.Identifier)Input[1].GetInput());
                beamPart = (TSM.Beam)beamAssembly.GetMainPart();

                changeCoordSystem();
                initializeInputData(); //Haetaan tarvittavat tiedot mallista

                //Model.CommitChanges();
            }
            catch (ArgumentException noArgs)
            {
                
            }
            catch (Exception Exc)
            {
                
            }

            return true;
        }

        #endregion

        #region Private methods

        private void initializeInputData()
        {

        }

        private void changeCoordSystem()
        {

        }
        private void GetValuesFromDialog()
        {
            if (!IsDefaultValue(Data._input_1))
            {
                InputData.column.height = Data._input_1;
            }
            else
            {
                InputData.column.height = 1.0;
            }
        }

        #endregion
    }
}
