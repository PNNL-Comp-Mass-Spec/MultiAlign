﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MultiAlignCore.Data.MetaData;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Data.Features;
using ScatterSeries = OxyPlot.Series.ScatterSeries;

namespace MultiAlignRogue
{
    class MSFeatureViewModel : PlotViewModelBase
    {
        public PlotModel Model { get; private set; }

        private readonly LinearAxis netAxis;
        private readonly LinearAxis massAxis;

        private int numSections;
        private int featuresPerSection;

        private IList<UMCLight> features; 

        public MSFeatureViewModel()
        {

        }

        public MSFeatureViewModel(Dictionary<DatasetInformation, IList<UMCLight>> features)
        {   
            this.Model = new PlotModel{Title = "MS Features"};
            this.Model.IsLegendVisible = true;
            this.Model.LegendPlacement = LegendPlacement.Inside;
            this.Model.LegendPosition = LegendPosition.LeftTop;
            this.Model.LegendOrientation = LegendOrientation.Vertical;

            this.netAxis = new LinearAxis
            {
                Title = "NET",
                Position = AxisPosition.Bottom,
                IsAxisVisible = true,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1.0
            };

            this.massAxis = new LinearAxis
            {
                Title = "Monoisotopic Mass",
                Position = AxisPosition.Left,
                IsAxisVisible = true,
                AbsoluteMinimum = 0
            };

            this.Model.Axes.Add(this.netAxis);
            this.Model.Axes.Add(this.massAxis);
            PlotFeatures(features);
            
        }

        public ScatterSeries GetPoints(IList<UMCLight> Features)
        {
            ScatterSeries scatter = new ScatterSeries { MarkerFill = OxyColors.Red, MarkerType = MarkerType.Circle };
            foreach (var feature in Features)
            {
                ScatterPoint point = new ScatterPoint(feature.Net, feature.MassMonoisotopic, .8, 0);
                try
                {
                    scatter.Points.Add(point);
                }
                catch
                {
                    continue;
                }
            }
            return scatter;
        }

        private void PlotFeatures(Dictionary<DatasetInformation, IList<UMCLight>> Features)
        {
            try
            {
                int i = 0;
                foreach (var file in Features.Keys)
                {
                    ScatterSeries currentFeatures = GetPoints(Features[file]);
                    currentFeatures.MarkerFill = Colors.ElementAt(i);
                    currentFeatures.Title = file.DatasetName;
                    this.Model.Series.Add(currentFeatures);
                    i = (i + 1)%Colors.Count; //Cycle through available colors if we run out
                }
            }
            catch
            {
                MessageBox.Show("Make sure that the selected files have detected features.");
            }
        }

        private void Partition()
        {
            var massStepSize = (massAxis.ActualMaximum - massAxis.ActualMinimum) / numSections;
            var netStepSize = (netAxis.ActualMaximum - netAxis.ActualMinimum) / numSections;

            ////var featureList = features.OrderByDescending(feat => feat.Abundance).ToList();

            for (int i = 0; i < numSections; i++)
            {
                var minMass = i * massStepSize;
                var maxMass = (i + 1) * massStepSize;

                for (int j = 0; j < numSections; j++)
                {
                    var minNet = i * netStepSize;
                    var maxNet = (i + 1) * netStepSize;

                    ////var sectionFeatures = featureList.Where(feat => feat.)
                }
            }

        }
    }
}
