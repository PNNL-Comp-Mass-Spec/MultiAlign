/*/////////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 *	Author: Brian LaMarche
 *  Date: 3-4-2008
 *  File: 
 *  Notes: 
 *		Create a stored query in the access database specifically
 *	for the mass tag database creator.  
 * 
 *	Revisions:
 *		3-4-2008
 *			Cleaned up code.
 *		3-6-2008
 *			Modified class to throw file not found exceptions in read file.
 *		3-7-2008 
 *			Separated STORED QUERY Creation code from specific stored query creation functionality. 
/*/////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Data.Odbc;
using System.Data.OleDb;

namespace MassTagDatabaseStoredQueryCreator
{
	/// <summary>
	///  Class that creates stored queries in an access database.  The queries must already be sanitized before being entered or 
	///  creating the stored query will not work.
	/// </summary>
    public class clsMassTagDBPeptideStoredQueryCreator
    {				
		/// <summary>
		/// Reference to stored query creator.
		/// </summary>
		private clsAccessStoredQueryCreator mobj_storedQueryCreator;

		/// <summary>
		/// Default constructor for the Access Database stored query creator.  
		/// </summary>
		/// <param name="filename">Full path to the database location.</param>
        public clsMassTagDBPeptideStoredQueryCreator(string databasePath)
        {
            mobj_storedQueryCreator = new clsAccessStoredQueryCreator(databasePath);
        }

		/// <summary>
		/// Creates the mass tag to protein name map stored query in the database associated with this
		/// class.  
		/// </summary>
		/// <param name="mainProteinNameMapFilename">Path of file that contains the stored query text.</param>
		/// <param name="viewIFCFileName">Path of file that has the internal VIEW DMS used to create the protein name map.</param>
		/// <returns>Boolean indicating failure (false) or success (true).  FileNotFoundException is not handled here.</returns>
		public bool CreateGetMassTagToProteinNameMapFromFile(string mainProteinNameMapFilename, string viewIFCFileName)
		{   						
			bool val = false;			 
			val = mobj_storedQueryCreator.CreateStoredQueryFromFile(viewIFCFileName);
			val = val && mobj_storedQueryCreator.CreateStoredQueryFromFile(mainProteinNameMapFilename);
			return val;
		}

		/// <summary>
		/// Create the peptide prophet stats stored query given the file names that contain the stored query text.
		/// </summary>
		/// <param name="peptideProphetStatsMainFilename">Path to main peptide propher stats stored query file that contains stored query text.</param>
		/// <param name="peptideProphetViewFilename">Path to temporary prophet stored query that handles writting to the temp table.</param>
		/// <returns>Boolean indicating failure (false) or success (true).  FileNotFoundException is not handled here.</returns>
		public bool CreateGetMassTagsPlusPepProphetStatsFromFile(string peptideProphetStatsMainFilename, string peptideProphetViewFilename)
		{   
			 bool val = false;			 
  			 val = mobj_storedQueryCreator.CreateStoredQueryFromFile(peptideProphetViewFilename);
			 val = val && mobj_storedQueryCreator.CreateStoredQueryFromFile(peptideProphetStatsMainFilename);
			 return val;
		}	
		
		/// <summary>
		/// Creates the mass tag to protein name map stored query strings given the query string to add to the database.		
		/// </summary>
		/// <param name="mainProteinNameMapFilename">Path of file that contains the stored query text.</param>
		/// <param name="viewIFCFileName">Path of file that has the internal VIEW DMS used to create the protein name map.</param>
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public bool CreateGetMassTagToProteinNameMap(string mainProteinNameMap, string viewIFC)
		{   						
			bool val = false;			 
			val = mobj_storedQueryCreator.CreateStoredQuery(viewIFC);
			val = val && mobj_storedQueryCreator.CreateStoredQuery(mainProteinNameMap);
			return val;
		}

		/// <summary>
		/// Create the peptide prophet stats stored query given query strings.
		/// </summary>
		/// <param name="peptideProphetStatsMainFilename">Main peptide propher stats stored query that contains stored query text.</param>
		/// <param name="peptideProphetViewFilename">Temporary prophet stored query that handles writting to the temp table.</param>
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public bool CreateGetMassTagsPlusPepProphetStats(string peptideProphetStatsMain, string peptideProphetView)
		{   
			bool val = false;			 
			val = mobj_storedQueryCreator.CreateStoredQuery(peptideProphetView);
			val = val && mobj_storedQueryCreator.CreateStoredQuery(peptideProphetStatsMain);
			return val;
		}		
		
		/// <summary>
		/// Create the get mass tag count stored query in the Access Database from the command text provided.
		/// </summary>
		/// <param name="getMassTagCountCommandString">Stored Query Command text.</param>		
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public bool CreateGetMassTagsMatchCount(string getMassTagCountCommandString)
		{   
			bool val = false;			 
			val = mobj_storedQueryCreator.CreateStoredQuery(getMassTagCountCommandString);
			return val;
		}		
		
		/// <summary>
		/// Create the get mass tag count stored query in the Access Database from the file path provided.
		/// </summary>
		/// <param name="getMassTagCountCommandFilePath">Path that contains GetMassTagMatchCount stored query text.</param>		
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public bool CreateGetMassTagsMatchCountFromFile(string getMassTagCountCommandFilePath)
		{   
			bool val = false;			 
			val = mobj_storedQueryCreator.CreateStoredQueryFromFile(getMassTagCountCommandFilePath);
			return val;
		}		
    }
}
