using System.Text;

namespace AppMonederoCommand.Utils
{
    public static class StringExtensions
    {
        public static string ToUTF8(this string text)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            string _text = Encoding.UTF8.GetString(iso.GetBytes(text));

            if (_text.IndexOf('�') != -1)
            {
                _text = text;
            }

            return _text;
        }
    }
}

