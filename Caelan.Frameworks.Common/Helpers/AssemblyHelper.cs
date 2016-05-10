using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Caelan.Frameworks.Common.Helpers
{
    public static class AssemblyHelper
    {
        public static Assembly GetWebEntrAssembly() => GetWebAssembly(HttpContext.Current?.ApplicationInstance?.GetType());

        private static Assembly GetWebAssembly(Type type) => type?.Namespace != "ASP" ? type?.Assembly : GetWebAssembly(type.BaseType);
    }
}
