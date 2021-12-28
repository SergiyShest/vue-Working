using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBP.DataExport
{
    public class SWIFTTransliteration
    {
        private static string _engChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static char[,] _rules =
        {
            {'А', 'A'},
            {'Б', 'B'},
            {'В', 'V'},
            {'Г', 'G'},
            {'Д', 'D'},
            {'Е', 'E'},
            {'Ё', 'о'},
            {'Ж', 'J'},
            {'З', 'Z'},
            {'И', 'I'},
            {'Й', 'i'},
            {'К', 'K'},
            {'Л', 'L'},
            {'М', 'M'},
            {'Н', 'N'},
            {'О', 'O'},
            {'П', 'P'},
            {'Р', 'R'},
            {'С', 'S'},
            {'Т', 'T'},
            {'У', 'U'},
            {'Ф', 'F'},
            {'Х', 'H'},
            {'Ц', 'C'},
            {'Ч', 'c'},
            {'Ш', 'Q'},
            {'Щ', 'q'},
            {'Ъ', 'x'},
            {'Ы', 'Y'},
            {'Ь', 'X'},
            {'Э', 'e'},
            {'Ю', 'u'},
            {'Я', 'a'},
            {'\'', 'j'},
            {'0', '0'},
            {'1', '1'},
            {'2', '2'},
            {'3', '3'},
            {'4', '4'},
            {'5', '5'},
            {'6', '6'},
            {'7', '7'},
            {'8', '8'},
            {'9', '9'},
            {'(', '('},
            {')', ')'},
            {'?', '?'},
            {'+', '+'},
            {'№', 'n'},
            {'%', 'р'},
            {'&', 'd'},
            {',', ','},
            {'/', '/'},
            {'-', '-'},
            {'.', '.'},
            {':', ':'},
            {' ', ' '},
            {'\r', '\r'},
            {'\n', '\n'}
        };

        private static char _swChar = '\'';

        private static Dictionary<char, char> _htForward;
        private static Dictionary<char, char> _htBack;
        private static Dictionary<char, bool> _htEng;

        static SWIFTTransliteration()
        {
            _htForward = new Dictionary<char, char>();
            _htBack = new Dictionary<char, char>();
            for (int i = 0; i <= _rules.GetUpperBound(0); i++)
            {
                _htForward.Add(_rules[i, 0], _rules[i, 1]);
                _htBack.Add(_rules[i, 1], _rules[i, 0]);
            }

            _htEng = new Dictionary<char, bool>();
            foreach (char c in _engChars)
            {
                _htEng.Add(c, true);
            }
        }

        public static string Convert(string str)
        {
            if (str == null)
                return null;

            str = str.ToUpper();

            StringBuilder sb = new StringBuilder();
            bool rusMode = true;
            foreach (char c in str)
            {
                if (_htForward.ContainsKey(c))
                {
                    if (!rusMode)
                    {
                        sb.Append(_swChar);
                        rusMode = true;
                    }

                    char c1 = _htForward[c];
                    sb.Append(c1);
                }
                else if (_htEng.ContainsKey(c))
                {
                    if (rusMode)
                    {
                        sb.Append(_swChar);
                        rusMode = false;
                    }

                    sb.Append(c);
                }
            }

            if (!rusMode)
                sb.Append('\'');

            string str1 = sb.ToString();
            return str1;
        }

        public static string ConvertBack(string str)
        {
            if (str == null)
                return str;

            StringBuilder sb = new StringBuilder();
            bool rusMode = true;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == _swChar)
                {
                    rusMode = !rusMode;
                    continue;
                }

                if (rusMode)
                {
                    if (!_htBack.ContainsKey(c))
                        throw new Exception("Обнаружен неизвестный символ '" + c + "' в позиции: " + i);

                    char c1 = _htBack[c];
                    sb.Append(c1);
                }
                else
                {
                    if (!_htEng.ContainsKey(c))
                        throw new Exception("Обнаружен не латинский символ '" + c + "' в позиции: " + i);

                    sb.Append(c);
                }
            }

            string str1 = sb.ToString();
            return str1;
        }
    }
}
