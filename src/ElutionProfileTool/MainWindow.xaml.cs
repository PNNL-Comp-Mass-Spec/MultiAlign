using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using PNNLControls;
using System.Drawing;

namespace ElutionProfileTool
{
    public enum FitEnum
    {
        Ideal0,
        Good1,
        Ok2,
        Poor3,
        Bad4,
        Bad3Points,
        Double,
        NotSet
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FitData>   m_fits;
        private int             m_fitIndex;
        private FitEnum         m_state;
        private string          m_saved;
        private List<int>       m_indexList;
        private Random          m_random;

        public MainWindow()
        {
            InitializeComponent();
            m_fits = new List<FitData>();
            m_fitIndex = 0;
            m_state = FitEnum.NotSet;
            m_saved = "fitScores.txt";

            File.AppendAllText(m_saved, "-------------------\n");


            m_indexList     = new List<int>();
            m_random        = new Random();
        }
        public List<FitData> ReadFits(string file)
        {
            if (!File.Exists(file))
            {
                return new List<FitData>();
            }
            List<FitData> fits = new List<FitData>();

            try
            {
                string [] data = File.ReadAllLines(file);                
                int state       = 0;
                FitData fit     = null;
                foreach(string line in data)
                {
                    if (line.Length < 3)
                        continue;

                    if (line.Contains("feature"))
                    {   
                        if (fit != null)
                        {
                            fits.Add(fit);
                        }
                        fit         = new FitData();
                        fit.Name    = line.Replace("feature", "").Replace(",", "");                        
                        state       = 0;
                    }
                    else if (line.Contains("mz"))
                    {
                        state = 1;
                    }
                    else
                    {
                        state = 2;
                    }

                    if (fit == null)
                        continue;

                    switch (state)
	                {
                        case 2:
                            string[] lineData = line.Split(',');
                            fit.X.Add(Convert.ToSingle(lineData[1]));
                            fit.Y.Add(Convert.ToSingle(lineData[2]));
                            break;
		                default:
                             break;
	                }
                }
                if (fit != null)
                {                    
                    fits.Add(fit);
                }
            }
            catch
            {
            }

            m_indexList  = new List<int>();
            for (int i = 0; i < fits.Count; i++)
            {
                m_indexList.Add(i);
            }

            return fits;
        }
        public void ViewFit(FitData data)
        {            

            BubbleShape shape           = new BubbleShape(8, false); 
            clsPlotParams plotParams    = new clsPlotParams(shape, System.Drawing.Color.Red);
            float[] xdata               = new float[data.X.Count];
            float[] ydata               = new float[data.Y.Count];
            data.X.CopyTo(xdata);
            data.Y.CopyTo(ydata);
            clsSeries series            = new clsSeries(ref xdata, ref ydata, plotParams);

            m_plot.SeriesCollection.Clear();
            m_plot.AddSeries(series);
            m_plot.AutoViewPort();

            m_plot2.SeriesCollection.Clear();
            m_plot2.AddSeries(series);
            m_plot2.AutoViewPort();

            m_plot.Title  = data.Name;
            m_plot2.Title = data.Name;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            m_fits      = ReadFits(textBox1.Text);
            m_fitIndex  = -1;
            MoveNext(true);
        }
        private void doubleButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = FitEnum.Double;
            MoveNext();
        }
        private void badButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = FitEnum.Poor3;
            MoveNext();
        }
        private void goodOneButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = FitEnum.Good1;
            MoveNext();
        }
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = FitEnum.Ok2;
            MoveNext();
        }
        private void failedButton_Click(object sender, RoutedEventArgs e)
        {
            m_state = FitEnum.Bad4;
            MoveNext();
        }
        private void MoveNext()
        {
            MoveNext(false);
        }
        private void ideaButton_Click(object sender, RoutedEventArgs e)
        {

            m_state = FitEnum.Ideal0;
            MoveNext();
        }

        private void MoveNext(bool skip)
        {
            if (m_state == FitEnum.NotSet && !skip)
                return;

            if (m_fits.Count < 1)
                return;

            if (m_fits.Count <= m_fitIndex)
                return;

            if (m_indexList.Count < 1)
                return;

            if (!skip)
            {
                string name = m_fits[m_fitIndex].Name;


                string stateData = "";
                switch (m_state)
                {
                    case FitEnum.Ideal0:
                        stateData = "0\tIdeal";
                        break;
                    case FitEnum.Good1:
                        stateData = "1\tGood";
                        break;
                    case FitEnum.Ok2:
                        stateData = "2\tOk";
                        break;
                    case FitEnum.Poor3:
                        stateData = "3\tPoor";
                        break;
                    case FitEnum.Bad4:
                        stateData = "4\tBad";
                        break;
                    case FitEnum.Double:
                        stateData = "5\tDouble Peak";
                        break;
                    case FitEnum.Bad3Points:
                        stateData = "6\tBad 3 Points";
                        break;
                    case FitEnum.NotSet:
                        stateData = "-1\tNot Scored";
                        break;
                    default:
                        break;
                }

                string data = string.Format("{0}\t{1}\n", name, stateData);
                File.AppendAllText(m_saved, data);
            }

            
            // Have it randomly get the next guy.
            int randomPoint = m_random.Next(0, m_indexList.Count - 1);
            m_fitIndex      = randomPoint;

            if (m_fits[m_fitIndex].X.Count < 4)
            {
                m_state = FitEnum.Bad3Points;
                MoveNext();
            }
            else
            {
                ViewFit(m_fits[m_fitIndex]);
                m_state = FitEnum.NotSet;
            }
        }        
    }

    public class FitData
    {
        public FitData()
        {
            X = new List<float>();
            Y = new List<float>();
        }
        public string Name      {get;set;}
        public List<float> X   {get;set;}
        public List<float> Y { get; set; }
    }
}
