namespace MultiAlignCore.Algorithms.Clustering
{
    public interface IClusterWriter<T>
    {
        void WriteCluster(T cluster);
        void Close();
    }
}
