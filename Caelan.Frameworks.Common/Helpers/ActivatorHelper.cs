using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caelan.Frameworks.Common.Helpers
{
    public static class ActivatorHelper
    {
        public static T CreateInstance<T>() where T : class => Activator.CreateInstance(typeof(T)) as T;
    }
}
