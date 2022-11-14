using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;

namespace TeklaDemoConnection
{
    [Plugin("TeklaConcreteConnection")]
    [PluginUserInterface("TeklaConcreteConnection.MainForm")]
    public class Plugin : PluginBase
    {
        #region Fields

        private TSM.Model _Model;
        private StructuresData _Data;
        private PluginInputData _PluginInputData;

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

        private StructuresData Data
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

        public Plugin(StructuresData data)
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
            try
            {
                if (Input.Count != 2)
                {
                    MessageBox.Show("Input count = " + Input.Count);
                    throw new ArgumentException();
                }
                GetValuesFromDialog();

                columnAssembly = (TSM.Assembly)Model.SelectModelObject((Tekla.Structures.Identifier)Input[0].GetInput());
                columnPart = (TSM.Beam)columnAssembly.GetMainPart();
                beamAssembly = (TSM.Assembly)Model.SelectModelObject((Tekla.Structures.Identifier)Input[1].GetInput());
                beamPart = (TSM.Beam)beamAssembly.GetMainPart();

                InputData.ColumnData.Column = columnPart;
                InputData.BeamData.Beam = beamPart;

                InitializeInputData(); //Haetaan tarvittavat tiedot mallista

                Creator creator = new Creator(Model, InputData);
                creator.Create();

                //Model.CommitChanges();
            }
            catch (ArgumentException noArgs)
            {
                MessageBox.Show("ArgumentException: " + noArgs.Message);
            }
            catch (Exception Exc)
            {
                MessageBox.Show("Run Exception: " + Exc.Message);
                MessageBox.Show("Run Exception: " + Exc.StackTrace);
                MessageBox.Show("Run Exception: " + Exc.InnerException.Message);
                MessageBox.Show("Run Exception: " + Exc.InnerException.StackTrace);
            }

            return true;
        }

        #endregion

        #region Private methods

        private void InitializeInputData()
        {
            MessageBox.Show("InitializeInputData");
        }

        private void GetValuesFromDialog()
        {
            // GeneralData
            if (!IsDefaultValue(Data.Type))
            {
                InputData.GeneralData.Type = Data.Type;
            }
            else
            {
                InputData.GeneralData.Type = DefaultData.GeneralData.Type;
            }
            if (!IsDefaultValue(Data.PcAttributeFile))
            {
                InputData.GeneralData.PcAttributeFile = Data.PcAttributeFile;
            }
            if (!IsDefaultValue(Data.NumberingType))
            {
                InputData.GeneralData.NumberingType = Data.NumberingType;
            }
            else
            {
                InputData.GeneralData.NumberingType = DefaultData.GeneralData.NumberingType;
            }
            if (!IsDefaultValue(Data.PcAttributeFile))
            {
                InputData.GeneralData.PcAttributeFile = Data.PcAttributeFile;
            }
            else
            {
                InputData.GeneralData.PcAttributeFile = DefaultData.GeneralData.PcAttributeFile;
            }
            if (!IsDefaultValue(Data.PcsAttributeFile))
            {
                InputData.GeneralData.PcsAttributeFile = Data.PcsAttributeFile;
            }
            else
            {
                InputData.GeneralData.PcsAttributeFile = DefaultData.GeneralData.PcsAttributeFile;
            }
            if (!IsDefaultValue(Data.LoadClass))
            {
                InputData.GeneralData.LoadClass = Data.LoadClass;
            }
            else
            {
                InputData.GeneralData.LoadClass = DefaultData.GeneralData.LoadClass;
            }
            if (!IsDefaultValue(Data.CountryStandard))
            {
                InputData.GeneralData.CountryStandard = Data.CountryStandard;
            }
            else
            {
                InputData.GeneralData.CountryStandard = DefaultData.GeneralData.CountryStandard;
            }
            if (!IsDefaultValue(Data.CreateReinforcement))
            {
                InputData.GeneralData.CreateReinforcement = Data.CreateReinforcement;
            }
            else
            {
                InputData.GeneralData.CreateReinforcement = DefaultData.GeneralData.CreateReinforcement;
            }
            if (!IsDefaultValue(Data.ShoeAnchorDist))
            {
                InputData.GeneralData.ShoeAnchorDist = Data.ShoeAnchorDist;
            }
            else
            {
                InputData.GeneralData.ShoeAnchorDist = DefaultData.GeneralData.ShoeAnchorDist;
            }
            if (!IsDefaultValue(Data.ShowWarnings))
            {
                InputData.GeneralData.ShowWarnings = Data.ShowWarnings;
            }
            else
            {
                InputData.GeneralData.ShowWarnings = DefaultData.GeneralData.ShowWarnings;
            }

            // ColumnData

            if (!IsDefaultValue(Data.ColumnCoverThickness))
            {
                InputData.ColumnData.CoverThickness = Data.ColumnCoverThickness;
            }
            else
            {
                InputData.ColumnData.CoverThickness = DefaultData.ColumnData.CoverThickness;
            }
            if (!IsDefaultValue(Data.cItem))
            {
                InputData.ColumnData.cItem = Data.cItem;
            }
            else
            {
                InputData.ColumnData.cItem = DefaultData.ColumnData.cItem;
            }

            if (!IsDefaultValue(Data.ColumnStirrupPrefix))
            {
                InputData.ColumnData.stirrup.prefix = Data.ColumnStirrupPrefix;
            }
            else
            {
                InputData.ColumnData.stirrup.prefix = DefaultData.ColumnData.stirrup.prefix;
            }
            if (!IsDefaultValue(Data.ColumnStirrupStartNumber))
            {
                InputData.ColumnData.stirrup.no = Data.ColumnStirrupStartNumber;
            }
            else
            {
                InputData.ColumnData.stirrup.no = DefaultData.ColumnData.stirrup.no;
            }
            if (!IsDefaultValue(Data.ColumnStirrupName))
            {
                InputData.ColumnData.stirrup.name = Data.ColumnStirrupName;
            }
            else
            {
                InputData.ColumnData.stirrup.name = DefaultData.ColumnData.stirrup.name;
            }
            if (!IsDefaultValue(Data.ColumnStirrupGrade))
            {
                InputData.ColumnData.stirrup.grade = Data.ColumnStirrupGrade;
            }
            else
            {
                InputData.ColumnData.stirrup.grade = DefaultData.ColumnData.stirrup.grade;
            }
            if (!IsDefaultValue(Data.ColumnStirrupClass))
            {
                InputData.ColumnData.stirrup.tsClass = Data.ColumnStirrupClass;
            }
            else
            {
                InputData.ColumnData.stirrup.tsClass = DefaultData.ColumnData.stirrup.tsClass;
            }

            if (!IsDefaultValue(Data.ColumnUBarPrefix))
            {
                InputData.ColumnData.uBar.prefix = Data.ColumnUBarPrefix;
            }
            else
            {
                InputData.ColumnData.uBar.prefix = DefaultData.ColumnData.uBar.prefix;
            }
            if (!IsDefaultValue(Data.ColumnUBarStartNumber))
            {
                InputData.ColumnData.uBar.no = Data.ColumnUBarStartNumber;
            }
            else
            {
                InputData.ColumnData.uBar.no = DefaultData.ColumnData.uBar.no;
            }
            if (!IsDefaultValue(Data.ColumnUBarName))
            {
                InputData.ColumnData.uBar.name = Data.ColumnUBarName;
            }
            else
            {
                InputData.ColumnData.uBar.name = DefaultData.ColumnData.uBar.name;
            }
            if (!IsDefaultValue(Data.ColumnUBarGrade))
            {
                InputData.ColumnData.uBar.grade = Data.ColumnUBarGrade;
            }
            else
            {
                InputData.ColumnData.uBar.grade = DefaultData.ColumnData.uBar.grade;
            }
            if (!IsDefaultValue(Data.ColumnUBarClass))
            {
                InputData.ColumnData.uBar.tsClass = Data.ColumnUBarClass;
            }
            else
            {
                InputData.ColumnData.uBar.tsClass = DefaultData.ColumnData.uBar.tsClass;
            }

            // BeamData

            if (!IsDefaultValue(Data.BeamTopCoverThickness))
            {
                InputData.BeamData.CoverThicknessTop = Data.BeamTopCoverThickness;
            }
            else
            {
                InputData.BeamData.CoverThicknessTop = DefaultData.BeamData.CoverThicknessTop;
            }
            if (!IsDefaultValue(Data.BeamRightCoverThickness))
            {
                InputData.BeamData.CoverThicknessRight = Data.BeamRightCoverThickness;
            }
            else
            {
                InputData.BeamData.CoverThicknessRight = DefaultData.BeamData.CoverThicknessRight;
            }
            if (!IsDefaultValue(Data.BeamBottomCoverThickness))
            {
                InputData.BeamData.CoverThicknessBottom = Data.BeamBottomCoverThickness;
            }
            else
            {
                InputData.BeamData.CoverThicknessBottom = DefaultData.BeamData.CoverThicknessBottom;
            }
            if (!IsDefaultValue(Data.BeamLeftCoverThickness))
            {
                InputData.BeamData.CoverThicknessLeft = Data.BeamLeftCoverThickness;
            }
            else
            {
                InputData.BeamData.CoverThicknessLeft = DefaultData.BeamData.CoverThicknessLeft;
            }
            if (!IsDefaultValue(Data.BottomHeight))
            {
                InputData.BeamData.BottomHeight = Data.BottomHeight;
            }
            else
            {
                InputData.BeamData.BottomHeight = DefaultData.BeamData.BottomHeight;
            }
            if (!IsDefaultValue(Data.bItem))
            {
                InputData.BeamData.bItem = Data.bItem;
            }
            else
            {
                InputData.BeamData.bItem = DefaultData.BeamData.bItem;
            }

            if (!IsDefaultValue(Data.BeamStirrupPrefix))
            {
                InputData.BeamData.stirrup.prefix = Data.BeamStirrupPrefix;
            }
            else
            {
                InputData.BeamData.stirrup.prefix = DefaultData.BeamData.stirrup.prefix;
            }
            if (!IsDefaultValue(Data.BeamStirrupStartNumber))
            {
                InputData.BeamData.stirrup.no = Data.BeamStirrupStartNumber;
            }
            else
            {
                InputData.BeamData.stirrup.no = DefaultData.BeamData.stirrup.no;
            }
            if (!IsDefaultValue(Data.BeamStirrupName))
            {
                InputData.BeamData.stirrup.name = Data.BeamStirrupName;
            }
            else
            {
                InputData.BeamData.stirrup.name = DefaultData.BeamData.stirrup.name;
            }
            if (!IsDefaultValue(Data.BeamStirrupGrade))
            {
                InputData.BeamData.stirrup.grade = Data.BeamStirrupGrade;
            }
            else
            {
                InputData.BeamData.stirrup.grade = DefaultData.BeamData.stirrup.grade;
            }
            if (!IsDefaultValue(Data.BeamStirrupClass))
            {
                InputData.BeamData.stirrup.tsClass = Data.BeamStirrupClass;
            }
            else
            {
                InputData.BeamData.stirrup.tsClass = DefaultData.BeamData.stirrup.tsClass;
            }

            if (!IsDefaultValue(Data.BeamUBarPrefix))
            {
                InputData.BeamData.uBar.prefix = Data.BeamUBarPrefix;
            }
            else
            {
                InputData.BeamData.uBar.prefix = DefaultData.BeamData.uBar.prefix;
            }
            if (!IsDefaultValue(Data.BeamUBarStartNumber))
            {
                InputData.BeamData.uBar.no = Data.BeamUBarStartNumber;
            }
            else
            {
                InputData.BeamData.uBar.no = DefaultData.BeamData.uBar.no;
            }
            if (!IsDefaultValue(Data.BeamUBarName))
            {
                InputData.BeamData.uBar.name = Data.BeamUBarName;
            }
            else
            {
                InputData.BeamData.uBar.name = DefaultData.BeamData.uBar.name;
            }
            if (!IsDefaultValue(Data.BeamUBarGrade))
            {
                InputData.BeamData.uBar.grade = Data.BeamUBarGrade;
            }
            else
            {
                InputData.BeamData.uBar.grade = DefaultData.BeamData.uBar.grade;
            }
            if (!IsDefaultValue(Data.BeamUBarClass))
            {
                InputData.BeamData.uBar.tsClass = Data.BeamUBarClass;
            }
            else
            {
                InputData.BeamData.uBar.tsClass = DefaultData.BeamData.uBar.tsClass;
            }
        }

        #endregion
    }
}
