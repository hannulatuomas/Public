using System;
using Tekla.Structures.Plugins;

namespace TeklaDemoConnection
{
    public class StructuresData
    {
		#region Fields

		/// <summary>
		/// Picture tab
		/// </summary>
		/// 

		[StructuresField("Type")]
		public int Type;

		[StructuresField("PcAttributeFile")]
		public int PcAttributeFile;

		[StructuresField("PcsAttributeFile")]
		public int PcsAttributeFile;

		[StructuresField("LoadClass")]
		public int LoadClass;

		[StructuresField("CountryStandard")]
		public int CountryStandard;


		[StructuresField("BottomHeight")]   // ok
		public double BottomHeight;

		[StructuresField("CreateReinforcement")]	// ok
		public int CreateReinforcement;

		[StructuresField("ShowWarnings")]
		public int ShowWarnings;

		[StructuresField("ShoeAnchorDist")]
		public int ShoeAnchorDist;


		[StructuresField("BeamCoverTTop")]	// ok
		public double BeamTopCoverThickness;

		[StructuresField("BeamCoverTBottom")]   // ok
		public double BeamBottomCoverThickness;

		[StructuresField("BeamCoverTLeft")] // ok
		public double BeamLeftCoverThickness;

		[StructuresField("BeamCoverTRight")]    // ok
		public double BeamRightCoverThickness;


		[StructuresField("cItem")]    // ok
		public int cItem;

		[StructuresField("bItem")]    // ok
		public int bItem;

		[StructuresField("LColumnCoverThick")]  // ok
		public double ColumnCoverThickness;

		/// <summary>
		/// Attributes tab
		/// </summary>
		/// 

		[StructuresField("NumberingType")]
		public int NumberingType;


		[StructuresField("CStirrupPrefix")]    // ok
		public string ColumnStirrupPrefix;

		[StructuresField("CStirrupSrtNmb")]    // ok
		public int ColumnStirrupStartNumber;

		[StructuresField("CStirrupName")]    // ok
		public string ColumnStirrupName;

		[StructuresField("CStirrupGrade")]    // ok
		public string ColumnStirrupGrade;

		[StructuresField("CStirrupClass")]    // ok
		public int ColumnStirrupClass;

		[StructuresField("CUBarPrefix")]    // ok
		public string ColumnUBarPrefix;

		[StructuresField("CUBarStartNumber")]    // ok
		public int ColumnUBarStartNumber;

		[StructuresField("CUBarName")]    // ok
		public string ColumnUBarName;

		[StructuresField("CUBarGrade")]    // ok
		public string ColumnUBarGrade;

		[StructuresField("CUBarClass")]    // ok
		public int ColumnUBarClass;


		[StructuresField("BStirrupPrefix")]    // ok
		public string BeamStirrupPrefix;

		[StructuresField("BStirrupSrtNmb")]    // ok
		public int BeamStirrupStartNumber;

		[StructuresField("BStirrupName")]    // ok
		public string BeamStirrupName;

		[StructuresField("BStirrupGrade")]    // ok
		public string BeamStirrupGrade;

		[StructuresField("BStirrupClass")]    // ok
		public int BeamStirrupClass;

		[StructuresField("BUBarPrefix")]    // ok
		public string BeamUBarPrefix;

		[StructuresField("BUBarStartNumber")]    // ok
		public int BeamUBarStartNumber;

		[StructuresField("BUBarName")]    // ok
		public string BeamUBarName;

		[StructuresField("BUBarGrade")]    // ok
		public string BeamUBarGrade;

		[StructuresField("BUBarClass")]    // ok
		public int BeamUBarClass;

		#endregion
	}
}
