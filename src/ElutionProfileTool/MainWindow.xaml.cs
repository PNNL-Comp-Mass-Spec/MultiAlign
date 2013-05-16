using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using PNNLControls;
using System.Drawing;
using PNNLOmics.Data;
using ElutionProfileTool.Graders;

namespace ElutionProfileTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XicFileReader   m_reader;
        private List<XicData>   m_fits;
        private XicScore        m_state;
        private string          m_saved;
                
        private int             m_fitIndex;
        private List<int>       m_indexList;
        private ExpertBase      m_expertLevel;
        private GradeFactory    m_gradeFactory;

        private int m_step = 50;

        /// <summary>
        /// Determines if an indice has been used.
        /// </summary>
        private Dictionary<int, bool> m_usedIndices;
        
        public MainWindow()
        {
            InitializeComponent();
            m_reader        = new XicFileReader();
            m_fits          = new List<XicData>();
            m_fitIndex      = 0;
            m_state         = XicScore.NotSet;
            m_saved         = "fitScores.txt";                                    
            m_indexList     = new List<int>();
           // m_random        = new Random();
            m_expertLevel = new ScienceExpert();
            m_ratingCountLabel.Content = string.Format(" {0} XIC's Graded", m_expertLevel.TotalGraded);
            m_expertLevelLabel.Content = string.Format("{0} ", m_expertLevel.CurrentLevel.Name, m_expertLevel.CurrentLevel.NextLevel.MinimumRating);

            string name     = Environment.UserName;
            NameText.Text   = name;            
            m_fitIndex      = Racoon.Properties.Settings.Default.Index; 

            m_gradeFactory  = new GradeFactory();
            textBox1.Text   = Racoon.Properties.Settings.Default.LastXic;
            m_plot.Title    = "XIC - index = " + m_fitIndex.ToString();

            Load(textBox1.Text, false);            
            MoveNext(true, true);
        }

        
        public bool IsDoublePeakAnnotation
        {
            get { return (bool)GetValue(IsDoublePeakAnnotationProperty); }
            set { SetValue(IsDoublePeakAnnotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDoublePeakAnnotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDoublePeakAnnotationProperty =
            DependencyProperty.Register("IsDoublePeakAnnotation", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));


        private void SaveSettings()
        {
            Racoon.Properties.Settings.Default.Index = m_fitIndex;
            Racoon.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Retrieves the next point
        /// </summary>
        /// <returns></returns>
        private void GetNextPoint()
        {
            m_fitIndex += m_step;

            /// They have found all indices.
            if (m_usedIndices.Count == m_indexList.Count)
            {   
                m_fitIndex = 0;
                SaveSettings();
                return;
            }

            if (m_fitIndex >= m_indexList.Count)
            {
                m_step++;
                m_fitIndex = 0;                
                GetNextPoint();
                return;
            }
            
            if (m_usedIndices.ContainsKey(m_fitIndex))
            {
                m_fitIndex++;
                GetNextPoint();
                return;
            }            
            else
            {
                m_usedIndices.Add(m_fitIndex, true);
            }

            SaveSettings();
        }
        public void ViewFit(XicData data)
        {           
            BubbleShape shape           = new BubbleShape(8, false); 
            clsPlotParams plotParams    = new clsPlotParams(shape, System.Drawing.Color.Red);
            float[] xdata               = new float[data.X.Count];
            float[] ydata               = new float[data.Y.Count];
            data.X.CopyTo(xdata);

            int length          = Convert.ToInt32(Math.Abs(xdata[xdata.Length - 1] - xdata[0]));
            ScanLength.Content  = string.Format("Time Difference = {0}", length);

            data.Y.CopyTo(ydata);
            clsSeries series            = new clsSeries(ref xdata, ref ydata, plotParams);
            
            m_plot.ShouldUseScientificNotation = false;
            m_plot.SeriesCollection.Clear();
            m_plot.AddSeries(series);
            m_plot.AutoViewPort();
            m_plot.Title = string.Format("XIC - index = {0}", m_fitIndex); 
            
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            string xic = textBox1.Text.ToLower();
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = Path.GetDirectoryName(xic);
            dialog.FileName = xic;
            dialog.Filter = "XIC Files (.xic)|*.xic";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            Load(dialog.FileName, true);
            MoveNext(true, false);
        }

        private void Load(string name, bool shouldReset)
        {
            textBox1.Text   = name;
            string xic      = textBox1.Text.ToLower();
            m_saved         = xic.Replace(".xic", "_results.txt");
            try
            {
                File.Delete(m_saved);
            }
            catch
            {

            }
            m_fits       = m_reader.ReadXicData(xic);

            m_usedIndices = new Dictionary<int, bool>();
            m_indexList = new List<int>();
            for (int i = 0; i < m_fits.Count; i++)
            {
                m_indexList.Add(i);
            }

            /// Resets the index...
            if (shouldReset)
            {
                m_fitIndex  = m_indexList.Count - 1;                
            }

            SaveSettings();
            global::Racoon.Properties.Settings.Default.LastXic = name;
            global::Racoon.Properties.Settings.Default.Save();
        }
                
        private void MoveNext()
        {
            MoveNext(false, false);
        }        
        private void MoveNext(bool skipSave, bool skipNext)
        {
            if (m_indexList.Count == m_usedIndices.Count)
            {
                if (m_indexList.Count > 0)
                {
                    m_expertLevelLabel.Content = "You have evaluated every feature!  No MORE LEFT!  Choose a new file!";
                }
                return;
            }

            if (m_state == XicScore.NotSet && !skipSave)
                return;

            if (m_fits.Count < 1)
                return;

            if (m_fits.Count <= m_fitIndex)
                return;

            if (m_indexList.Count < 1)
                return;

            if (!skipSave)
            {
                
                XicData score      = m_fits[m_fitIndex];
                string name         = score.Name;                 
                string state        = m_gradeFactory.GetStateString(m_state);
                string data = string.Format("{0}\t{1}\t{2}\n", name, state, NameText.Text);

                global::Racoon.Properties.Settings.Default.Grader = NameText.Text;
                global::Racoon.Properties.Settings.Default.Save();

                m_saved = textBox1.Text.ToLower().Replace(".xic", string.Format("_{0}_results.txt", NameText.Text));
                File.AppendAllText(m_saved, data);
            }

            if (!skipNext)
            {
                GetNextPoint();
            }

            if (m_fits[m_fitIndex].X.Count < 5)
            {
                m_state = XicScore.AutoPoor;
                MoveNext();
            }
            else
            {
                ViewFit(m_fits[m_fitIndex]);
                IncreaseGrade();
                m_state = XicScore.NotSet;
            }
        }
        private void IncreaseGrade()
        {
            ExpertLevel tempLevel    = m_expertLevel.CurrentLevel;
            ExpertLevel currentLevel = m_expertLevel.AddRating();

            if (tempLevel.MinimumRating != currentLevel.MinimumRating)
            {
                int nextLevel = -1;
                if (currentLevel.NextLevel != null)
                {
                    nextLevel = currentLevel.NextLevel.MinimumRating;
                    m_expertLevelLabel.Content = string.Format("CONGRATS {0}! / Next Promotion in {1} XIC's",
                                    currentLevel.Name,
                                    nextLevel);
                }
                else
                {
                    m_expertLevelLabel.Content = string.Format("CONGRATS {0}!  You have achieved the highest level", currentLevel.Name);
                }
            }
            else
            {
                int nextLevel = -1;
                if (currentLevel.NextLevel != null)
                {
                    nextLevel = currentLevel.NextLevel.MinimumRating;
                    m_expertLevelLabel.Content = string.Format("CONGRATS {0}! / Next Promotion in {1} XIC's", currentLevel.Name,
                                                                                        m_expertLevel.NextPromotionCount);
                }
                else
                {
                    m_expertLevelLabel.Content = string.Format("{0} ", currentLevel.Name, nextLevel);
                }
            }

            m_ratingCountLabel.Content = string.Format(" {0} XIC's Graded", m_expertLevel.TotalGraded);
        }

        #region Event Handlers
        private void OneButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = XicScore.One;
            MoveNext();
        }
        private void TwoButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = XicScore.Two;
            MoveNext();
        }
        private void ThreeButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = XicScore.Three;
            MoveNext();
        }
        private void FourButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = XicScore.Four;
            MoveNext();
        }      
        #endregion
    }    
}
