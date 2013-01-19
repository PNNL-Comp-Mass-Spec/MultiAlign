using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using PNNLControls;
using System.Drawing;
using PNNLOmics.Data;

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
        private XicFileReader m_reader;
        private List<FitData>   m_fits;
        private int             m_fitIndex;
        private FitEnum         m_state;
        private string          m_saved;
        private List<int>       m_indexList;
        private Random          m_random;

        public MainWindow()
        {
            InitializeComponent();
            m_reader = new XicFileReader();
            m_fits = new List<FitData>();
            m_fitIndex = 0;
            m_state = FitEnum.NotSet;
            m_saved = "fitScores.txt";
                        
            try
            {
                File.Delete(m_saved);
            }
            catch
            {
            }

            m_indexList     = new List<int>();
            m_random        = new Random();
        }
        
        public string GetStateString(FitEnum fit)
        {
            if (IsDoublePeakAnnotation)
            {
                return DoubleAnnotation(fit);
            }
            else
            {
                return SingleAnnotation(fit);
            }
        }
        public string DoubleAnnotation(FitEnum fit)
        {
            string stateData = "";
            switch (fit)
            {
                case FitEnum.Ideal0:
                    stateData = "0\tOnePeak";
                    break;
                case FitEnum.Good1:
                    stateData = "1\tOnePeakWithTail";
                    break;
                case FitEnum.Ok2:
                    stateData = "2\tTwoPeaks";
                    break;
                case FitEnum.Poor3:
                    stateData = "3\tThreePeaks";
                    break;
                case FitEnum.Bad4:
                    stateData = "4\tMoreThanThree";
                    break;
                case FitEnum.Double:
                    stateData = "5\tCannotTell";
                    break;
                case FitEnum.Bad3Points:
                    stateData = "6\tBad 5 or less Points";
                    break;
                case FitEnum.NotSet:
                    stateData = "-1\tNot Scored";
                    break;
                default:
                    break;
            }

            return stateData;
        }
        public string SingleAnnotation(FitEnum fit)
        {
             string stateData = "";
             switch (fit)
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
                     stateData = "6\tBad 5 Points";
                     break;
                 case FitEnum.NotSet:
                     stateData = "-1\tNot Scored";
                     break;
                 default:
                     break;
             }

             return stateData;
        }

        public bool IsDoublePeakAnnotation
        {
            get { return (bool)GetValue(IsDoublePeakAnnotationProperty); }
            set { SetValue(IsDoublePeakAnnotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDoublePeakAnnotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDoublePeakAnnotationProperty =
            DependencyProperty.Register("IsDoublePeakAnnotation", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));
     
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
            string xic = textBox1.Text.ToLower().Replace(".raw", ".xic"); 
            m_fits      = m_reader.ReadFits(xic);

            m_indexList = new List<int>();
            for (int i = 0; i < m_fits.Count; i++)
            {
                m_indexList.Add(i);
            }

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
                string name         = m_fits[m_fitIndex].Name;
                string stateData    = GetStateString(m_state);
                string data         = string.Format("{0}\t{1}\n", name, stateData);
                File.AppendAllText(m_saved, data);
            }
            
            // Have it randomly get the next guy.
            int randomPoint = m_random.Next(0, m_indexList.Count - 1);
            m_fitIndex      = randomPoint;          

            if (m_fits[m_fitIndex].X.Count < 5)
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

        private void modeButton_Click(object sender, RoutedEventArgs e)
        {
            IsDoublePeakAnnotation = (IsDoublePeakAnnotation == false);

            if (IsDoublePeakAnnotation)
            {
                ideaButton.Content      = "One Peak";
                goodOneButton.Content   = "One With Long Tail";
                okButton.Content        = "Two Peaks";
                badButton.Content       = "Three Peaks";
                failedButton.Content    = "More Than Three";
                doubleButton.Content    = "Cannot Tell";
                modeButton.Content      = "Set To Single Mode";
            }   
            else
            {
                
                ideaButton.Content      = "Ideal - 0";
                goodOneButton.Content   = "Good - 1";
                okButton.Content        = "Ok - 2";
                badButton.Content       = "Poor - 3";
                failedButton.Content    = "Failed - 4";
                doubleButton.Content    = "Double";
                modeButton.Content      = "Set To Double Mode";
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
        public int Charge { get; set; }
        public double Mz { get; set; }
        public int Scan { get; set; }
        public string Name      {get;set;}
        public List<float> X   {get;set;}
        public List<float> Y { get; set; }
    }
}
