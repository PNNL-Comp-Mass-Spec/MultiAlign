using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Parameters;
using NUnit.Framework;

namespace MultiAlignTestSuite.IO.Parameters
{
    [TestFixture]
    public class XMLParameterFileTests
    {
        private delegate object DelegateConverter(object o);
        private object ChangeDouble(object o)
        {
            double d = Convert.ToDouble(o);
            return d + 1;
        }
        private object ChangeBool(object o)
        {
            bool d = Convert.ToBoolean(o);
            return d == false;
        }
        private object ChangeInt(object o)
        {
            int d = Convert.ToInt32(o);
            return d + 1;
        }
        private object ChangeLong(object o)
        {
            long d = Convert.ToInt64(o);
            return d + 1;
        }
        private object ChangeSingle(object o)
        {
            float d = Convert.ToSingle(o);
            return d + 1.0F;
        }
        private object ChangeString(object o)
        {
            string d = Convert.ToString(o);
            return d += "-test";
        }
        private object ChangeEnum(object o)
        {
            Array values = Enum.GetValues(o.GetType());
            object value = null;

            // Hack to change the value.
            for (int i = 0; i < values.Length; i++)
            {
                object tempValue = values.GetValue(i);
                if (tempValue.ToString() == o.ToString())
                {
                    // Make sure
                    if (i == 0)
                    {
                        i = i + 1;
                    }
                    else if (i < (values.Length - 1))
                    {
                        i = i + 1;
                    }
                    else
                    {
                        i = i - 1;
                    }

                    if (i < 0)
                    {
                        i = 0;
                    }
                    else if (i == values.Length)
                    {
                        i = values.Length - 1;
                    }

                    value = values.GetValue(i);
                    break;
                }
            }
            return value;
        }
        private void Compare(object o, object p)
        {
            Dictionary<Type, DelegateConverter> converters = new Dictionary<Type, DelegateConverter>();
            converters.Add(typeof(int), ChangeInt);
            converters.Add(typeof(double), ChangeDouble);
            converters.Add(typeof(float), ChangeSingle);
            converters.Add(typeof(long), ChangeLong);
            converters.Add(typeof(bool), ChangeBool);
            converters.Add(typeof(string), ChangeString);

            Dictionary<string, object> valueMap = new Dictionary<string, object>();
            if (o != null)
            {
                foreach (PropertyInfo prop in o.GetType().GetProperties())
                {
                    // Recurse to get parameters.
                    if (prop.CanRead)
                    {
                        object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                        object potential = null;
                        if (customAttributes.Length > 0)
                        {
                            potential = prop.GetValue(o,
                                                    BindingFlags.GetProperty,
                                                    null,
                                                    null,
                                                    null);
                        }
                        for (int i = 0; i < customAttributes.Length; i++)
                        {

                            ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;

                            if (potential != null && attr != null && attr.Name != "")
                            {
                                valueMap.Add(attr.Name, potential);
                            }
                            else
                            {
                                throw new NullReferenceException("The parameter value cannot be null.");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new NullReferenceException("The object o should not be null.");
            }
            if (p != null)
            {
                foreach (PropertyInfo prop in p.GetType().GetProperties())
                {
                    // Recurse to get parameters.
                    if (prop.CanRead)
                    {
                        object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                        object potential = null;
                        if (customAttributes.Length > 0)
                        {
                            potential = prop.GetValue(p,
                                                    BindingFlags.GetProperty,
                                                    null,
                                                    null,
                                                    null);
                        }
                        for (int i = 0; i < customAttributes.Length; i++)
                        {
                            ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                            if (potential != null && attr != null && attr.Name != "")
                            {
                                object ovalue = valueMap[attr.Name];

                                bool isEqual = ovalue.ToString() == potential.ToString();
                                if (!isEqual)
                                {
                                    throw new Exception("the two values should be equal.  " + ovalue.ToString() + " " + potential.ToString());
                                }
                            }
                            else
                            {
                                throw new NullReferenceException("The parameter value cannot be null.");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new NullReferenceException("The object p should not be null.");
            }
        }

        private object ChangeObjectValues(object o)
        {
            Dictionary<Type, DelegateConverter> converters = new Dictionary<Type, DelegateConverter>();
            converters.Add(typeof(int),     ChangeInt);
            converters.Add(typeof(double),  ChangeDouble);
            converters.Add(typeof(float), ChangeSingle);
            converters.Add(typeof(long), ChangeLong);
            converters.Add(typeof(bool), ChangeBool);
            converters.Add(typeof(string),  ChangeString);

            if (o != null)
            {
                foreach (PropertyInfo prop in o.GetType().GetProperties())
                {
                    // Recurse to get parameters.
                    if (prop.CanRead)
                    {
                        object[] customAttributes = prop.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                        object potential = null;
                        if (customAttributes.Length > 0)
                        {
                            potential = prop.GetValue(o,
                                                    BindingFlags.GetProperty,
                                                    null,
                                                    null,
                                                    null);
                        }
                        for (int i = 0; i < customAttributes.Length; i++)
                        {
                            ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                            if (potential != null && attr != null && attr.Name != "")
                            {
                                Type data = potential.GetType();
                                // We want the key not found exception to be thrown!
                                if (data.IsEnum)
                                {
                                    potential = ChangeEnum(potential);
                                }
                                else
                                {
                                    potential = converters[data](potential);
                                }
                                prop.SetValue(o, potential, BindingFlags.SetProperty, null, null, null);
                            }
                            else
                            {
                                throw new NullReferenceException("The parameter value cannot be null.");
                            }
                        }
                    }
                }
                foreach (FieldInfo field in o.GetType().GetFields())
                {
                    object[] customAttributes = field.GetCustomAttributes(typeof(ParameterFileAttribute), true);
                    object objectValue = null;
                    if (customAttributes.Length > 0)
                    {
                        objectValue = field.GetValue(o);
                    }
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        ParameterFileAttribute attr = customAttributes[i] as ParameterFileAttribute;
                        if (objectValue != null && attr != null)
                        {
                            throw new Exception("There should be no fields");
                        }
                    }
                }
            }

            return o;
        }


        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterCluster(string path)
        {
            try
            {
                XMLParameterFileWriter writer   = new XMLParameterFileWriter();
                XMLParamterFileReader reader    = new XMLParamterFileReader();
                MultiAlignAnalysis analysis     = new MultiAlignAnalysis();
                analysis.Options.ClusterOptions = ChangeObjectValues(analysis.Options.ClusterOptions) as MultiAlignCore.Algorithms.Clustering.LCMSFeatureClusteringOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.ClusterOptions, newAnalysis.Options.ClusterOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }

        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterDriftTimeAlignment(string path)
        {
            try
            {
                XMLParameterFileWriter writer   = new XMLParameterFileWriter();
                XMLParamterFileReader reader    = new XMLParamterFileReader();
                MultiAlignAnalysis analysis     = new MultiAlignAnalysis();
                analysis.Options.DriftTimeAlignmentOptions = ChangeObjectValues(analysis.Options.DriftTimeAlignmentOptions) as MultiAlignCore.Algorithms.Alignment.DriftTimeAlignmentOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.DriftTimeAlignmentOptions, newAnalysis.Options.DriftTimeAlignmentOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }

        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterFilter(string path)
        {
            try
            {
                XMLParameterFileWriter writer = new XMLParameterFileWriter();
                XMLParamterFileReader reader = new XMLParamterFileReader();
                MultiAlignAnalysis analysis = new MultiAlignAnalysis();
                analysis.Options.FeatureFilterOptions = ChangeObjectValues(analysis.Options.FeatureFilterOptions) as MultiAlignCore.Data.Features.FeatureFilterOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.FeatureFilterOptions, newAnalysis.Options.FeatureFilterOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }
        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterMTDB(string path)
        {
            try
            {
                XMLParameterFileWriter writer           = new XMLParameterFileWriter();
                XMLParamterFileReader reader            = new XMLParamterFileReader();
                MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
                analysis.Options.MassTagDatabaseOptions = ChangeObjectValues(analysis.Options.MassTagDatabaseOptions) as MultiAlignCore.IO.MTDB.MassTagDatabaseOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis          = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.MassTagDatabaseOptions, newAnalysis.Options.MassTagDatabaseOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }
        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterMSLinker(string path)
        {
            try
            {
                XMLParameterFileWriter writer           = new XMLParameterFileWriter();
                XMLParamterFileReader reader            = new XMLParamterFileReader();
                MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
                analysis.Options.MSLinkerOptions = ChangeObjectValues(analysis.Options.MSLinkerOptions) as MultiAlignCore.Algorithms.MSLinker.MSLinkerOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis          = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.MSLinkerOptions, newAnalysis.Options.MSLinkerOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }

        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterSTAC(string path)
        {
            try
            {
                XMLParameterFileWriter writer = new XMLParameterFileWriter();
                XMLParamterFileReader reader = new XMLParamterFileReader();
                MultiAlignAnalysis analysis = new MultiAlignAnalysis();
                analysis.Options.STACOptions = ChangeObjectValues(analysis.Options.STACOptions) as MultiAlignCore.Algorithms.FeatureMatcher.STACOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.STACOptions, newAnalysis.Options.STACOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }

        [Test]
        [TestCase("testParameter2.xml")]
        public void TestXMLParameterWriterFeatureFinderOptions(string path)
        {
            try
            {

                XMLParameterFileWriter writer           = new XMLParameterFileWriter();
                XMLParamterFileReader reader            = new XMLParamterFileReader();
                MultiAlignAnalysis analysis             = new MultiAlignAnalysis();
                analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = true;

                analysis.Options.FeatureFindingOptions  = ChangeObjectValues(analysis.Options.FeatureFindingOptions) as MultiAlignCore.Algorithms.FeatureFinding.LCMSFeatureFindingOptions;
                writer.WriteParameterFile(path, analysis);
                MultiAlignAnalysis newAnalysis = new MultiAlignAnalysis();
                reader.ReadParameterFile(path, ref newAnalysis);
                Compare(analysis.Options.FeatureFindingOptions, newAnalysis.Options.FeatureFindingOptions);

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    Console.WriteLine("The file was not deleted.");
                }
            }
        }
    }
}
