using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlubal.RFEM5;
using Tekla.Structures.Catalogs;

namespace Stabiliteettilaskenta.Mapping
{
    static class ProfileMapping
    {
        static Dictionary<string, string> profileNames = new Dictionary<string, string>();

        public static void GetProfiles()
        {
            IModel iModel;
            Tekla.Structures.Catalogs.CatalogHandler catalogHandler = new CatalogHandler();
            ProfileItemEnumerator teklaProfiles = catalogHandler.GetProfileItems();

            while (teklaProfiles.MoveNext())
            {
                ProfileItem teklaProfile = teklaProfiles.Current;

                if (teklaProfile.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_I)
                {
                    Tekla.Structures.Catalogs.CrossSection crossSection = teklaProfile.GetCrossSection();
                    string name = crossSection.Profile.Name;
                    profileNames.Add(name, "");
                }
            }
            string s = profileNames["IPE100"];
        }


        ///////////////////////////////
        ///

        public static List<string> MapTeklaProfiles(List<string> teklaProfiles)
        {
            List<string> rfemProfiles = new List<string>();

            foreach (string teklaProfile in teklaProfiles)
            {
                // Rules:
                string rule = "IPE";
                string mapped = "";

                if (teklaProfile.Contains(rule))
                {
                    mapped = Utility.TxtFile.GetMappedString(teklaProfile, rule, "IPE ");
                    rfemProfiles.Add(mapped);
                    continue;
                }
                rule = "HEA";
                if (teklaProfile.Contains(rule))
                {
                    mapped = Utility.TxtFile.GetMappedString(teklaProfile, rule, "HE A ");
                    rfemProfiles.Add(mapped);
                    continue;
                }
                rule = "HEB";
                if (teklaProfile.Contains(rule))
                {
                    mapped = Utility.TxtFile.GetMappedString(teklaProfile, rule, "HE B ");
                    rfemProfiles.Add(mapped);
                    continue;
                }
                rule = "HEM";
                if (teklaProfile.Contains(rule))
                {
                    mapped = Utility.TxtFile.GetMappedString(teklaProfile, rule, "HE M ");
                    rfemProfiles.Add(mapped);
                    continue;
                }
                rule = "HFSHS";
                if (teklaProfile.Contains(rule))
                {
                    string height = Utility.TxtFile.GetBetween(teklaProfile, rule, "*");
                    string width = Utility.TxtFile.GetBetween(teklaProfile, "*", "*");

                    if (height == width)
                    {
                        mapped = Utility.TxtFile.GetMappedString(teklaProfile, height + "*" + width, height);
                        mapped = Utility.TxtFile.GetMappedString(mapped, rule, "QRO ");
                    }
                    else
                    {
                        mapped = teklaProfile;
                        mapped = Utility.TxtFile.GetMappedString(mapped, rule, "RRO ");
                    }
                    mapped = Utility.TxtFile.GetMappedString(mapped, "*", "x");
                    mapped = Utility.TxtFile.GetMappedString(mapped, ".0", "");
                    mapped = mapped + " (Hot Formed)";
                    rfemProfiles.Add(mapped);
                    continue;
                }
                rule = "CFSHS";
                if (teklaProfile.Contains(rule))
                {
                    string height = Utility.TxtFile.GetBetween(teklaProfile, rule, "X");
                    string width = Utility.TxtFile.GetBetween(teklaProfile, "X", "X");

                    if (height == width)
                    {
                        mapped = Utility.TxtFile.GetMappedString(teklaProfile, height + "X" + width, height);
                        mapped = Utility.TxtFile.GetMappedString(mapped, rule, "QRO ");
                    }
                    else
                    {
                        mapped = teklaProfile;
                        mapped = Utility.TxtFile.GetMappedString(mapped, rule, "RRO ");
                    }
                    mapped = Utility.TxtFile.GetMappedString(mapped, "X", "x");
                    mapped = mapped + " (Cold Formed)";
                    rfemProfiles.Add(mapped);
                    continue;
                }

            }

            return rfemProfiles;
        }
    }
}
