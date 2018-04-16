using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace PropertyDataGrid
{
    [Serializable]
    public class PropertyGridException : Exception
    {
        public const string Prefix = "PDG";

        public PropertyGridException()
            : base(Prefix + "0001: PropertyDataGrid exception.")
        {
        }

        public PropertyGridException(string message)
            : base(message)
        {
        }

        public PropertyGridException(Exception innerException)
            : base(null, innerException)
        {
        }

        public PropertyGridException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PropertyGridException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static int GetCode(string message)
        {
            if (message == null)
                return -1;

            if (!message.StartsWith(Prefix, StringComparison.Ordinal))
                return -1;

            int pos = message.IndexOf(':', Prefix.Length);
            if (pos < 0)
                return -1;

            if (int.TryParse(message.Substring(Prefix.Length, pos - Prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out int i))
                return i;

            return -1;
        }

        public int Code => GetCode(Message);
    }
}
