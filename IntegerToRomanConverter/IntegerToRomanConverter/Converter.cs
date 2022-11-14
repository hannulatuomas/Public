using System;

namespace IntegerToRomanConverter
{
    internal class Converter
    {
        public string ConvertToRoman(int x)
        {
            string result = String.Empty;
            int value = x;

            int a = 1000;

            while (value / a > 0)
            {
                result += "M";
                value -= a;
            }
            a = 900;
            while (value / a > 0)
            {
                result += "CM";
                value -= a;
            }
            a = 500;
            while (value / a > 0)
            {
                result += "D";
                value -= a;
            }
            a = 400;
            while (value / a > 0)
            {
                result += "CD";
                value -= a;
            }
            a = 100;
            while (value / a > 0)
            {
                result += "C";
                value -= a;
            }
            a = 90;
            while (value / a > 0)
            {
                result += "XC";
                value -= a;
            }
            a = 50;
            while (value / a > 0)
            {
                result += "L";
                value -= a;
            }
            a = 40;
            while (value / a > 0)
            {
                result += "XL";
                value -= a;
            }
            a = 10;
            while (value / a > 0)
            {
                result += "X";
                value -= a;
            }
            a = 9;
            while (value / a > 0)
            {
                result += "IX";
                value -= a;
            }
            a = 5;
            while (value / a > 0)
            {
                result += "V";
                value -= a;
            }
            a = 4;
            while (value / a > 0)
            {
                result += "IV";
                value -= a;
            }
            a = 1;
            while (value / a > 0)
            {
                result += "I";
                value -= a;
            }

            return result;
        }
    }
}
