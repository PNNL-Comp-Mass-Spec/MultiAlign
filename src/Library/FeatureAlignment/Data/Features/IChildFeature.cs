namespace FeatureAlignment.Data.Features
{

    /// <summary>
    /// An object that realizes this interface can be part of another object's aggregation.
    /// </summary>
    /// <typeparam name="T">Parent Type</typeparam>
    public interface IChildFeature<T>
    {
        /// <summary>
        /// Sets the parent feature of the object.
        /// </summary>
        /// <param name="parentFeature">Parent feature to reference.</param>
        void SetParentFeature(T parentFeature);

        /// <summary>
        /// Gets the parent feature for this object.
        /// </summary>
        T GetParentFeature();
    }
}
