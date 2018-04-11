using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDataGrid.Utilities
{
    public static class PropertyGridExtensions
    {
        public static string GetAllMessages(this Exception exception) => GetAllMessages(exception, Environment.NewLine);
        public static string GetAllMessages(this Exception exception, string separator)
        {
            if (exception == null)
                return null;

            var sb = new StringBuilder();
            AppendMessages(sb, exception, separator);
            return sb.ToString().Replace("..", ".");
        }

        private static void AppendMessages(StringBuilder sb, Exception e, string separator)
        {
            if (e == null)
                return;

            // this one is not interesting...
            if (!(e is TargetInvocationException))
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(e.Message);
            }
            AppendMessages(sb, e.InnerException, separator);
        }

        public static string GetInterestingExceptionMessage(this Exception exception) => GetInterestingException(exception)?.Message;
        public static Exception GetInterestingException(this Exception exception)
        {
            if (exception is TargetInvocationException tie && tie.InnerException != null)
                return GetInterestingException(tie.InnerException);

            return exception;
        }
    }
}
