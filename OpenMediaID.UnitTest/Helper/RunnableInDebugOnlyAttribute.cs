using System.Diagnostics;

namespace OpenMediaID.UnitTest.Helper
{
    /// <summary>
    /// Unit Test Helper for Debug
    /// </summary>
    public sealed class RunnableInDebugOnlyAttribute : FactAttribute
    {
        public RunnableInDebugOnlyAttribute()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Only running in interactive mode. Not automatically";
            }
        }
    }
}