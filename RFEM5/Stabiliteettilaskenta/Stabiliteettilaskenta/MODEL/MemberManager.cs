using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stabiliteettilaskenta.MODEL
{
    public class MemberManager
    {
        LineManager lineManager;
        Dictionary<int, Member> memberNumbers;
        int firstFreeNumber;

        public MemberManager()
        {
            lineManager = new LineManager();
            memberNumbers = new Dictionary<int, Member>();
            firstFreeNumber = 1;
        }

        #region Add Member
        public Member AddMember(Line _line, int _number)
        {
            int number;
            Member member = new Member(_line);

            if (_number != 0 && !memberNumbers.ContainsKey(_number))
            {
                number = _number;
            }
            if (ContainsMember(member, out number))
            {
                // Already Exists
                member = memberNumbers[number];
            }
            else
            {
                if (number == 0)
                {
                    if (memberNumbers.ContainsKey(firstFreeNumber))
                    {
                        GetFirstFreeIndex();
                    }
                    number = firstFreeNumber;
                    firstFreeNumber++;
                }
                member.Number = number;

                // Add new Member to lists
                memberNumbers.Add(member.Number, member);
            }

            return member;
        }

        #endregion

        #region Remove Member
        public bool RemoveMember(Member _member)
        {
            bool successfully;
            successfully = memberNumbers.Remove(_member.Number);
            if (successfully)
            {
                GetFirstFreeIndex();
                
                // Add Other Lists here
                ///
            }

            return successfully;
        }

        #endregion

        #region Get Member


        #endregion

        int GetFirstFreeIndex()
        {
            firstFreeNumber = 1;
            IEnumerator<int> numbers = memberNumbers.Keys.OrderBy(k => k).GetEnumerator();
            while (numbers.MoveNext())
            {
                if (numbers.Current != firstFreeNumber)
                {
                    break;
                }
                firstFreeNumber++;
            }
            return firstFreeNumber;
        }

        #region Linq
        bool ContainsMember(Member _member, out int _number)
        {
            _number = _member.Number;
            foreach (Member member in memberNumbers.Values)
            {
                if (member.Number == _member.Number)
                {
                    _number = member.Number;
                    return true;
                }
            }
            return false;
        }

        #endregion

    }

    public class Member
    {
        public Member(Line _line)
        {
            Line = _line;
        }

        public int Number { get; set; }
        public Line Line { get; private set; }
        public Material Material { get; set; }
        public CrossSection CrossSection { get; set; }
    }

    public class Material
    {
        public string Name { get; set; }
        public MaterialType Type { get; set; }
        public enum MaterialType { Steel, Conctere, Timber, Unknown}
    }
    public class CrossSection
    {
        public string Name { get; set; }
        public CrossSectionType Type { get; set; }
        public enum CrossSectionType { IPE, HEA, HEB, HEM, RHS, SHS, WI, Unknown }
    }
}
