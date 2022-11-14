using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeklaConcreteConnection
{
    class PluginInputData
    {
        public GeneralData generalData;
        public ColumnData column;
        public BeamData beam;

        public PluginInputData()
        {
            this.generalData = new GeneralData();
            this.column = new ColumnData();
            this.beam = new BeamData();
        }

        public class GeneralData
        {

        }

        public class ColumnData
        {
            public double height;
        }

        public class BeamData
        {

        }
    }
}
