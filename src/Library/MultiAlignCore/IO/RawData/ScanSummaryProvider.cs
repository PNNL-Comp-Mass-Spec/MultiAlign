using System;
using System.Collections.Generic;
using FluentNHibernate.Utils;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.TextFiles;

namespace MultiAlignCore.IO.RawData
{
    public class ScanSummaryProvider: IScanSummaryProvider
    {
        private readonly Dictionary<int, DatasetSummary> _summaries = new Dictionary<int, DatasetSummary>();
        private readonly Dictionary<int, int> _minScans = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _maxScans = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _scanCounts = new Dictionary<int, int>();

        public Dictionary<int, ScanSummary> GetScanData(int groupId)
        {
            if (!_summaries.ContainsKey(groupId))
            {
                throw new ArgumentOutOfRangeException("The group-dataset ID provided was not found.");
            }
            //var summaries = new Dictionary<int, ScanSummary>();
            //for (int i = 0; i < _maxScans[groupId]; i++)
            //{
            //    summaries.Add(i, GetScanSummary(i, groupId));
            //}
            //return summaries;
            return _summaries[groupId].ScanMetaData;
        }

        public ScanSummary GetScanSummary(int scan, int group)
        {
            if (!_summaries.ContainsKey(group))
            {
                throw new ArgumentOutOfRangeException("The group-dataset ID provided was not found.");
            }
            if (!_summaries[group].ScanMetaData.ContainsKey(scan))
            {
                throw new ScanOutOfRangeException("The requested scan is out of range.");
            }
            return _summaries[group].ScanMetaData[scan];
        }

        public void AddDataFile(string path, int groupId)
        {
            if (path.ToLower().EndsWith("_scans.csv"))
            {
                LoadScansFile(path, groupId);
            }
            else if (path.ToLower().EndsWith("analysis.db3"))
            {
                LoadFromDatabase();
            }
            else
            {
                throw new ArgumentException("Cannot handle input file", "path");
            }
        }

        public int GetTotalScans(int group)
        {
            if (!_summaries.ContainsKey(group))
            {
                throw new ArgumentOutOfRangeException("The group-dataset ID provided was not found.");
            }
            return _scanCounts[group];
        }

        public int GetMinScan(int group)
        {
            if (!_summaries.ContainsKey(group))
            {
                throw new ArgumentOutOfRangeException("The group-dataset ID provided was not found.");
            }
            return _minScans[group];
        }

        public int GetMaxScan(int group)
        {
            if (!_summaries.ContainsKey(group))
            {
                throw new ArgumentOutOfRangeException("The group-dataset ID provided was not found.");
            }
            return _maxScans[group];
        }

        public void Dispose()
        {
            _summaries.Clear();
            _maxScans.Clear();
            _minScans.Clear();
            _scanCounts.Clear();
        }

        public void LoadScansFile(string scansFilePath, int groupId)
        {
            var reader = new ScansFileReader { Delimiter = ',' };
            var scans = reader.ReadFile(scansFilePath);
            if (!_summaries.ContainsKey(groupId))
            {
                _summaries.Add(groupId, new DatasetSummary
                {
                    ScanMetaData = new Dictionary<int, ScanSummary>()
                });
                _minScans.Add(groupId, 0);
                _maxScans.Add(groupId, 0);
                _scanCounts.Add(groupId, 0);
            }
            var dict = _summaries[groupId].ScanMetaData;
            dict.Clear();
            var minScan = int.MaxValue;
            var maxScan = int.MinValue;
            foreach (var scan in scans)
            {
                scan.DatasetId = groupId;
                dict.Add(scan.Scan, scan);
                minScan = Math.Min(minScan, scan.Scan);
                maxScan = Math.Max(maxScan, scan.Scan);
            }
            _minScans[groupId] = minScan;
            _maxScans[groupId] = maxScan;
            _scanCounts[groupId] = maxScan - minScan; // Not truly the total number present; Scans file contains only MS1 scans
            SetNets(groupId);
        }

        public void LoadFromDatabase()
        {
            var dao = new ScanSummaryDAOHibernate();
            LoadFromDatabase(dao);
        }

        public void LoadFromDatabase(ScanSummaryDAOHibernate dao)
        {
            var data = dao.FindAll();
            var idsSeen = new Dictionary<int, bool>();
            foreach (var summary in data)
            {
                var groupId = summary.DatasetId;
                if (!_summaries.ContainsKey(groupId))
                {
                    _summaries.Add(groupId, new DatasetSummary
                    {
                        ScanMetaData = new Dictionary<int, ScanSummary>()
                    });
                    _minScans.Add(groupId, int.MaxValue);
                    _maxScans.Add(groupId, int.MinValue);
                    _scanCounts.Add(groupId, 0);
                }
                if (!idsSeen.ContainsKey(groupId))
                {
                    _summaries[groupId].ScanMetaData.Clear();
                    idsSeen.Add(groupId, true);
                }
                
                _summaries[groupId].ScanMetaData.Add(summary.Scan, summary);

                _minScans[groupId] = Math.Min(_minScans[groupId], summary.Scan);
                _maxScans[groupId] = Math.Max(_maxScans[groupId], summary.Scan);
            }
            foreach (var groupId in idsSeen.Keys)
            {
                _scanCounts[groupId] = _maxScans[groupId] - _minScans[groupId];
            }
        }

        private void SetNets(int group)
        {
            var datasetSummary = this._summaries[group];
            var minScanSum = this.GetScanSummary(this.GetMinScan(group), group);
            var maxScanSum = this.GetScanSummary(this.GetMaxScan(group), group);
            var timeDiff = maxScanSum.Time - minScanSum.Time;

            foreach (var summary in datasetSummary.ScanMetaData.Values)
            {
                summary.Net = (summary.Time - minScanSum.Time) / timeDiff;
            }
        }
    }
}
