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
        private string mstring_connectionString;

		/// <summary>
		/// Default constructor for the Access Database stored query creator.  
		/// </summary>
		/// <param name="filename">Full path to the database location.</param>
        public clsMassTagDBPeptideStoredQueryCreator(AccessType type, string databasePath)
        {
            mstring_connectionString = clsAccessStoredQueryCreator.GetConnectionString(type, databasePath);
            mobj_storedQueryCreator  = new clsAccessStoredQueryCreator();
        }

		/// <summary>
		/// Creates the mass tag to protein name map stored query in the database associated with this
		/// class.  
		/// </summary>
		/// <param name="mainProteinNameMapFilename">Path of file that contains the stored query text.</param>
		/// <param name="viewIFCFileName">Path of file that has the internal VIEW DMS used to create the protein name map.</param>
		/// <returns>Boolean indicating failure (false) or success (true).  FileNotFoundException is not handled here.</returns>
		public void CreateGetMassTagToProteinNameMapFromFile(string mainProteinNameMapFilename, string viewIFCFileName)
		{   									
			mobj_storedQueryCreator.CreateStoredQueryFromFile(mstring_connectionString, viewIFCFileName);
            mobj_storedQueryCreator.CreateStoredQueryFromFile(mstring_connectionString, mainProteinNameMapFilename);			
		}

		/// <summary>
		/// Create the peptide prophet stats stored query given the file names that contain the stored query text.
		/// </summary>
		/// <param name="peptideProphetStatsMainFilename">Path to main peptide propher stats stored query file that contains stored query text.</param>
		/// <param name="peptideProphetViewFilename">Path to temporary prophet stored query that handles writting to the temp table.</param>
		/// <returns>Boolean indicating failure (false) or success (true).  FileNotFoundException is not handled here.</returns>
		public void CreateGetMassTagsPlusPepProphetStatsFromFile(string peptideProphetStatsMainFilename, string peptideProphetViewFilename)
		{   
             mobj_storedQueryCreator.CreateStoredQueryFromFile(mstring_connectionString, peptideProphetViewFilename);
             mobj_storedQueryCreator.CreateStoredQueryFromFile(mstring_connectionString, peptideProphetStatsMainFilename);			  
		}	
		
		/// <summary>
		/// Creates the mass tag to protein name map stored query strings given the query string to add to the database.		
		/// </summary>
		/// <param name="mainProteinNameMapFilename">Path of file that contains the stored query text.</param>
		/// <param name="viewIFCFileName">Path of file that has the internal VIEW DMS used to create the protein name map.</param>
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public void CreateGetMassTagToProteinNameMap(string mainProteinNameMap, string viewIFC)
		{   						
			mobj_storedQueryCreator.CreateStoredQuery(mstring_connectionString, viewIFC);
            mobj_storedQueryCreator.CreateStoredQuery(mstring_connectionString, mainProteinNameMap);			
		}

		/// <summary>
		/// Create the peptide prophet stats stored query given query strings.
		/// </summary>
		/// <param name="peptideProphetStatsMainFilename">Main peptide propher stats stored query that contains stored query text.</param>
		/// <param name="peptideProphetViewFilename">Temporary prophet stored query that handles writting to the temp table.</param>
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public void CreateGetMassTagsPlusPepProphetStats(string peptideProphetStatsMain, string peptideProphetView)
		{   
			mobj_storedQueryCreator.CreateStoredQuery(mstring_connectionString, peptideProphetView);
            mobj_storedQueryCreator.CreateStoredQuery(mstring_connectionString, peptideProphetStatsMain);			
		}		
		
		/// <summary>
		/// Create the get mass tag count stored query in the Access Database from the command text provided.
		/// </summary>
		/// <param name="getMassTagCountCommandString">Stored Query Command text.</param>		
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public void CreateGetMassTagsMatchCount(string getMassTagCountCommandString)
		{   
            mobj_storedQueryCreator.CreateStoredQuery(mstring_connectionString, getMassTagCountCommandString);			
		}		
		
		/// <summary>
		/// Create the get mass tag count stored query in the Access Database from the file path provided.
		/// </summary>
		/// <param name="getMassTagCountCommandFilePath">Path that contains GetMassTagMatchCount stored query text.</param>		
		/// <returns>Boolean indicating failure (false) or success (true).</returns>
		public void CreateGetMassTagsMatchCountFromFile(string getMassTagCountCommandFilePath)
		{   
            mobj_storedQueryCreator.CreateStoredQueryFromFile(mstring_connectionString, getMassTagCountCommandFilePath);			
		}		
    }
}
