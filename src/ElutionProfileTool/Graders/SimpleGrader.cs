using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    public class SimpleGrader : IGrader
    {
        #region IGrader Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>      
        public string GetScoreValue(XicScore fit)
        {
            string state = "";

            switch (fit)
	        {
		        case XicScore.One:
                    state = "1\tGood";
                    break;
                case XicScore.Two:
                    state = "2\tGood";
                    break;
                case XicScore.Three:
                    state = "3\tGood";
                    break;
                case XicScore.Four:
                    state = "4\tGood";
                    break;
                default:
                    state = "-1\tunknown";
                    break;
	        }

            return state;
        }
                   
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>               
        public List<string> GetNames()
        {
            List<string> names = new List<string>();            
            names.Add("Good");
            names.Add("Ok");
            names.Add("Poor");
            names.Add("Failed");
            names.Add("Multiple");

            return names;
        }

        #endregion
    }   
}
