This directory stores DLLs not available from NuGet:

alglibnet2.dll
* ALGLIB is a cross-platform numerical analysis and data processing library. 
* http://www.alglib.net/download.php

PNNLOmics.dll
* Data analysis, file I/O, and visualization routines used by several of the Omics-related software developed at PNNL
* https://github.com/PNNL-Comp-Mass-Spec/PNNL-Omics

QuadTreeLib.dll
* Implements a spatial partitioning strategy used to make queries on relationships between 2D spatial data
  * see http://bluetoque.ca/products/quadtree/
* Compiled from source code at https://bitbucket.org/bluetoque/bluetoque.quadtree
  * Modified to use WindowsBase objects (Point and Rect) instead of System.Drawing objects (PointF and RectangleF)
* https://github.com/PNNL-Comp-Mass-Spec/LCMS-Spectator/tree/master/Library/QuadTreeLib

== InformedProteomics DLLs

| DLL                                         | Source                                                        | Comments         |
|---------------------------------------------|---------------------------------------------------------------|------------------|
| InformedProteomics.Backend.dll              | https://github.com/PNNL-Comp-Mass-Spec/Informed-Proteomics    |                  |
| InformedProteomics.Backend.MassSpecData.dll | https://github.com/PNNL-Comp-Mass-Spec/Informed-Proteomics    |                  |
| InformedProteomics.FeatureFinding.dll       | https://github.com/PNNL-Comp-Mass-Spec/Informed-Proteomics    |                  |
| ProteinFileReader.dll                       | https://www.nuget.org/packages/ProteinFileReader              | NuGet package    |
| ThermoFisher.CommonCore.RawFileReader.dll   | Thermo                                                        | See license in RawFileReaderLicense.doc |
| ThermoRawFileReader.dll                     | https://github.com/PNNL-Comp-Mass-Spec/Thermo-Raw-File-Reader | Wrapper to ThermoFisher.CommonCore      |

== Mage DLLs

| DLL                                         | Source                                                          | Comments                            |
|---------------------------------------------|-----------------------------------------------------------------|-------------------------------------|
| ICSharpCode.SharpZipLib.dll                 | https://www.nuget.org/packages/SharpZipLib/                     | NuGet package; used by MyEMSLReader |
| Jayrock.Json.dll                            | https://www.nuget.org/packages/jayrock-json/                    | NuGet package; used by MyEMSLReader |
| Mage.dll                                    | https://github.com/PNNL-Comp-Mass-Spec/Mage                     |                                     |
| MyEMSLReader.dll                            | https://github.com/PNNL-Comp-Mass-Spec/MyEMSL-Reader            |                                     |
| Pacifica.Core.dll                           | https://github.com/PNNL-Comp-Mass-Spec/DMS-Capture-Task-Manager | Used by MyEMSLReader                |
| PRISM.dll                                   | https://www.nuget.org/packages/PRISM-Library/                   | NuGet package                       |
| System.Data.SQLite.dll                      | https://www.nuget.org/packages/System.Data.SQLite.Core/         | NuGet package                       |

== MTDBFrameworkBase DLLs

| DLL                             | Source                                                       | Comments                                    |
|---------------------------------|--------------------------------------------------------------|---------------------------------------------|
| Antlr3.Runtime.dll              | https://www.nuget.org/packages/Antlr3.Runtime/               | NuGet package, auto-pulled via NHibernate   |
| FeatureAlignment.dll            | https://github.com/PNNL-Comp-Mass-Spec/MultiAlign            | src/Library/FeatureAlignment                |
| FluentNHibernate.dll            | https://www.nuget.org/packages/FluentNHibernate/             | NuGet package                               |
| Iesi.Collections.dll            | https://www.nuget.org/packages/Iesi.Collections/             | NuGet package, auto-pulled via NHibernate   |
| MathNet.Numerics.dll            | https://www.nuget.org/packages/MathNet.Numerics/             |                                             |
| MTDBFrameworkBase.dll           | https://github.com/PNNL-Comp-Mass-Spec/MTDB-Creator          |                                             |
| NHibernate.dll                  | https://www.nuget.org/packages/NHibernate/                   | NuGet package, auto-pulled via FluentNHibernate    |
| PHRPReader.dll                  | https://github.com/PNNL-Comp-Mass-Spec/PHRP                  |                                             |
| PRISM.dll                       | https://www.nuget.org/packages/PRISM-Library/                | NuGet package                               |
| Remotion.Linq.dll               | https://www.nuget.org/packages/Remotion.Linq/                | NuGet package, auto-pulled via NHibernate   |
| Remotion.Linq.EagerFetching.dll | https://www.nuget.org/packages/Remotion.Linq.EagerFetching/  | NuGet package, auto-pulled via NHibernate   |
| System.Data.SQLite.dll          | https://www.nuget.org/packages/System.Data.SQLite.Core/      | NuGet package                               |
