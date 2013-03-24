using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElutionProfileTool.Graders
{
    public abstract class ExpertBase
    {
        private ExpertLevel         m_currentLevel;                 
        public ExpertBase()
        {
            m_currentLevel  = BuildExpertLevels();
            TotalGraded     = global::Racoon.Properties.Settings.Default.Grade;
        }

        /// <summary>
        /// Total number of items graded.
        /// </summary>
        public int TotalGraded
        {
            get;
            set;
        }

        /// <summary>
        /// Builds expert level.
        /// </summary>
        /// <returns></returns>
        protected abstract ExpertLevel BuildExpertLevels();

        /// <summary>
        /// Increases the count of ratings and updates the expert level
        /// if it passes 
        /// </summary>
        /// <returns></returns>
        public ExpertLevel AddRating()
        {
            TotalGraded++;
            global::Racoon.Properties.Settings.Default.Grade = TotalGraded;
            global::Racoon.Properties.Settings.Default.Save();

            if (m_currentLevel.NextLevel == null)
                return m_currentLevel;

            else if (m_currentLevel.NextLevel.MinimumRating <= TotalGraded)
            {
                m_currentLevel = m_currentLevel.NextLevel;
            }

            return m_currentLevel;
        }
        /// <summary>
        /// Gets the current Level
        /// </summary>
        public ExpertLevel CurrentLevel
        {
            get
            {
                return m_currentLevel;
            }
        }
        /// <summary>
        /// Gets the next promotion count.
        /// </summary>
        public int NextPromotionCount
        {
            get
            {
                ExpertLevel level = m_currentLevel.NextLevel;
                if (level == null)
                    return 0;

                return (level.MinimumRating - TotalGraded);
            }
        }
    }

    public class ScienceExpert: ExpertBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ScienceExpert()
        {
            BuildExpertLevels();
        }
        /// <summary>
        /// Builds a list of expert levels.
        /// </summary>
        /// <returns></returns>
        protected override ExpertLevel BuildExpertLevels()
        {
            List<string> names = new List<string>()
                                        {
                                        "Peon",
                                        "Novice",
                                        "Begginner",
                                        "Assistant Professor",
                                        "Professor",
                                        "Senior Scientist",
                                        "PI",
                                        "Jedi"};

            ExpertLevel tempLevel   = new ExpertLevel();
            tempLevel.Name          = names[names.Count - 1];
            tempLevel.NextLevel     = null;
            tempLevel.MinimumRating = 1000;

            double rating = 1000;

            for(int i = names.Count - 1; i >=0; i--)
            {
                ExpertLevel level    = new ExpertLevel();
                level.Name           = names[i];
                level.MinimumRating  = Convert.ToInt32(rating / 2);
                level.NextLevel      = tempLevel;  
                rating               = rating / 2;
                tempLevel            = level;
            }

            return tempLevel;
        }
    }
}
