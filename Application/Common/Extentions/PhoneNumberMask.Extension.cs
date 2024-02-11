using System;

namespace Application.Common.Extentions
{
    public static class PhoneNumberMaskExtension
    {
        public static string PhoneNumberMask(this string value)
        {
            string areaCode = value.Substring(0, Math.Min(3, value.Length));
            string prefix = value.Substring(Math.Min(3, value.Length), Math.Min(3, value.Length - 3));
            string lineNumber = value.Substring(Math.Min(6, value.Length), Math.Min(4, value.Length - 6));
            return string.Format("({0})-{1}-{2}", areaCode, prefix, lineNumber);
        }
    }
}
