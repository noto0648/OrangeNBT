namespace OrangeNBT.NBT.IO
{
    public class JsonOptions
    {
        private static JsonOptions _default = new JsonOptions();
        public static JsonOptions Default { get { return _default; } }

        private string _quotation = "\"";
        public string StringQuotationMark { get { return _quotation; } set { _quotation = value; } }

        private string _keyQuotation = "";
        public string KeyQuotationMark { get { return _keyQuotation; } set { _keyQuotation = value; } }

        private string[] _digits = new string[] { "", "b", "s", "", "l", "f", "d", "", "", "", "", "", "" };

        public JsonOptions() { }

        internal string GetDigit(TagType type)
        {
            if ((int)type < _digits.Length && !string.IsNullOrEmpty(_digits[(int)type]))
                return _digits[(int)type];
            return string.Empty;
        }
    }
}
