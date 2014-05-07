namespace MultiAlignCore.Data.Features
{
    public class FeatureAnnotation
    {
        public int AnnotationID
        {
            get;
            set;
        }
        public int FeatureID
        {
            get;
            set;
        }
        public int GroupID
        {
            get;
            set;
        }
        public FeatureType FeatureType
        {
            get;
            set;
        }
        public int FeatureTypeID
        {
            get
            {
                return (int)FeatureType;
            }
            set
            {
                if (value > (int)FeatureType.MSSpectra)
                    return;
                if (value < (int)FeatureType.Cluster)
                    return;
                FeatureType = (FeatureType)value; 
            }
        }
        public string Annotation
        {
            get;
            set;
        }
    }

    public enum FeatureType    
    {
        Cluster = 0,
        Feature = 1,
        MSFeature = 2,
        MSMS = 3,
        MSMSSpectra = 4,
        MSSpectra =5
    }
}
