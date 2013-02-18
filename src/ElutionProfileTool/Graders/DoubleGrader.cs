using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    public class DoubleGrader : IGrader
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
                    state = "1\tOnePeakWithTail";
                    break;
                case XicScore.Two:
                    state = "2\tTwoPeaks";
                    break;
                case XicScore.Three:
                    state = "3\tThreePeaks";
                    break;
                case XicScore.Four:
                    state = "4\tMoreThanThree";
                    break;
                case XicScore.AutoPoor:         
                    state = "6\tBad 5 or less Points";           
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
            names.Add("One Peak With Tail");
            names.Add("Two Peaks");
            names.Add("Three Peaks");
            names.Add("More Than Three");
            names.Add("Poor");
            return names;
        }

        #endregion
    }   
}
