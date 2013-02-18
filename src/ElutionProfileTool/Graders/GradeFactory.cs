using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    public class GradeFactory
    {
        IGrader m_grader;

        public GradeFactory()
        {
            Grader = new SimpleGrader();
        }
        public IGrader Grader
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the state string for a given score.
        /// </summary>
        /// <param name="fit"></param>
        /// <returns></returns>
        public string GetStateString(XicScore fit)
        {
            return Grader.GetScoreValue(fit);
        }
        /// <summary>
        /// Returns a list of Xic Names
        /// </summary>
        /// <returns></returns>
        public List<string> GetNames()
        {
            return Grader.GetNames();
        }        
    }
}
