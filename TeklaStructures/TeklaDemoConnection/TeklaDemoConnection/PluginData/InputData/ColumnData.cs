using Tekla.Structures.Model;

namespace TeklaDemoConnection
{
    public class ColumnData
    {
        public Beam Column { get; set; }

        public double ProfileWidth { 
            get 
            {
                double profileWidth = 0.0;
                Column.GetReportProperty("PROFILE.WIDTH", ref profileWidth);
                return profileWidth; 
            }
        }
        public double ProfileHeight {
            get
            {
                double profileHeight = 0.0;
                Column.GetReportProperty("PROFILE.HEIGHT", ref profileHeight);
                return profileHeight;
            }
        }

        public double CoverThickness;
        public int cItem;

        public StirrupData stirrup;
        public StirrupData uBar;

        public ColumnData()
        {
            this.stirrup = new StirrupData();
            this.uBar = new StirrupData();
        }
    }
}
