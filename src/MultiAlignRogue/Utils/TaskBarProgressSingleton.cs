using System;
using System.Windows;
using System.Windows.Shell;
using MultiAlign.Data;
using MultiAlignRogue.Alignment;
using MultiAlignRogue.Clustering;
using MultiAlignRogue.Feature_Finding;

namespace MultiAlignRogue.Utils
{
    public class TaskBarProgressSingleton
    {
        private static TaskBarProgressSingleton _singleton = null;

        private bool _disableStepProgress = false;
        private object _controllingObject;

        private FeatureFindingSettingsViewModel _featureModel;
        private AlignmentSettingsViewModel _alignmentModel;
        private ClusterSettingsViewModel _clusterModel;

        public TaskBarProgressSingleton(FeatureFindingSettingsViewModel featureModel,
            AlignmentSettingsViewModel alignmentModel, ClusterSettingsViewModel clusterModel)
        {
            if (_singleton == null)
            {
                _featureModel = featureModel;
                _alignmentModel = alignmentModel;
                _clusterModel = clusterModel;
                _singleton = this;
            }
        }

        public static void SetFeatureModel(object callingObj, FeatureFindingSettingsViewModel featureModel)
        {
            if (_singleton._controllingObject == null ||
                ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                _singleton._featureModel = featureModel;
            }
        }

        public static void SetAlignmentModel(object callingObj, AlignmentSettingsViewModel alignmentModel)
        {
            if (_singleton._controllingObject == null ||
                ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                _singleton._alignmentModel = alignmentModel;
            }
        }

        public static void SetClusterModel(object callingObj, ClusterSettingsViewModel clusterModel)
        {
            if (_singleton._controllingObject == null ||
                ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                _singleton._clusterModel = clusterModel;
            }
        }

        public static void TakeTaskbarControl(object callingObj)
        {
            if (_singleton._controllingObject == null)
            {
                _singleton._controllingObject = callingObj;
                _singleton._disableStepProgress = true;
            }
        }

        public static void ReleaseTaskbarControl(object callingObj)
        {
            if (ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                _singleton._controllingObject = null;
                _singleton._disableStepProgress = false;
            }
        }

        public static void SetTaskBarProgress(object callingObj, double pct)
        {
            if (!_singleton._disableStepProgress || ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                ThreadSafeDispatcher.Invoke((Action) (() =>
                {
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = pct / 100.0;
                }));
            }
            else
            {
                var ppct = -1D;
                if (ReferenceEquals(callingObj, _singleton._featureModel))
                {
                    ppct = (pct / 100.0) * 0.4;
                }
                else if (ReferenceEquals(callingObj, _singleton._alignmentModel))
                {
                    ppct = (pct / 100.0) * 0.4 + 0.4;
                }
                else if (ReferenceEquals(callingObj, _singleton._clusterModel))
                {
                    ppct = (pct / 100.0) * 0.2 + 0.8;
                }
                if (ppct > 0.0)
                {
                    ThreadSafeDispatcher.Invoke((Action)(() =>
                    {
                        Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = ppct;
                    }));
                }
            }
        }

        public static void ShowTaskBarProgress(object callingObj, bool doShow)
        {
            if (!_singleton._disableStepProgress || ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                if (doShow)
                {
                    ThreadSafeDispatcher.Invoke((Action)(() =>
                    {
                        Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    }));
                }
                else
                {
                    ThreadSafeDispatcher.Invoke((Action)(() =>
                    {
                        Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    }));

                }
            }
        }


    }
}
