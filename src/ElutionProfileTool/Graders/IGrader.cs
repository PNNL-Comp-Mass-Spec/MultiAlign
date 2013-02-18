using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    /// <summary>
    /// Interface for setting up new graders.
    /// </summary>
    public interface IGrader
    {
        string GetScoreValue(XicScore fit);
        List<string> GetNames();
    }
}
