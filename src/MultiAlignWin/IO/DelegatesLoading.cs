/*////////////////////////////////////////////////////////////////////////
 *  File  : delegatesLoading.cs
 *  Author: Brian LaMarche
 *  Date  : 9/11/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Definitions for loading a dataset from disk, database, etc.
 * 
 *  Revisions:
 *      9-11-2008:
 *          - Created file and comment header.
 * 
 *////////////////////////////////////////////////////////////////////////
using MultiAlignEngine;

namespace MultiAlignWin.IO
{
    #region Delegate Definitions
    /// <summary>
    /// Delegate method definition for adding a dataset to the listview controls and the internal array lists.
    /// </summary>
    /// <param name="dataset"></param>
    public delegate void DelegateDataSetLoaded(clsDatasetInfo dataset);
    /// <summary>
    /// Delegate method for when a data set is loaded, to update the progress of the loading progress bar.
    /// </summary>
    /// <param name="progressValue"></param>
    public delegate void DelegateUpdateLoadingPercentLoaded(double percentLoaded);
    /// <summary>
    /// Delegate method definition for when dataset loading is complete.
    /// </summary>
    public delegate void DelegateUpdateLoadingComplete();
    /// <summary>
    /// Delegate method definition for how many datasets were found
    /// </summary>
    public delegate void DelegateTotalDatasetsFound(int found);

    public delegate void DelegateUpdateLoadingState(bool value);
    public delegate void DelegateUpdateDatasetsChecked(int numberChecked);

    #endregion
}