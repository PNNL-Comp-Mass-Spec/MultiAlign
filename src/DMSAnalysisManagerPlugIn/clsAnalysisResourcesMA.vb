Imports AnalysisManagerBase
Imports PRISM.Logging
Imports System.IO
Imports System

Public Class clsAnalysisResourcesMA
	Inherits clsAnalysisResources

#Region "Constants"
    Public Const LOG_LOCAL_ONLY As Boolean = True
    Public Const LOG_DATABASE As Boolean = False

    Protected Const SP_NAME_GET_MULTIALIGN_DATA_FOLDER_PATHS As String = "GetMultiAlignDataFolderPaths"

    Protected Const CSV_ISOS_FILE_SUFFIX As String = "isos.csv"
    Protected Const CSV_ISOS_IC_FILE_SUFFIX As String = "isos_ic.csv"
    Protected Const PEK_FILE_SUFFIX As String = ".pek"

#End Region

#Region "Structures"
    Structure udtFileInfoType
        Public Job As Integer
        Public FolderPath As String
        Public FolderPathLocal As String
        Public FileName As String
    End Structure
#End Region

    Public Shared Function GetFileExtensionPrefList() As String()
        Dim strFileExtensionsPrefList(2) As String

        strFileExtensionsPrefList(0) = CSV_ISOS_FILE_SUFFIX
        strFileExtensionsPrefList(1) = CSV_ISOS_IC_FILE_SUFFIX
        strFileExtensionsPrefList(2) = PEK_FILE_SUFFIX

        Return strFileExtensionsPrefList

    End Function

    Protected Function GetPeakListFiles(ByVal strWorkingDirectory As String, ByVal intJobNum As Integer) As Boolean

        Dim intIndex As Integer
        Dim intExtensionIndex As Integer
        Dim intFileIndex As Integer

        Dim blnSuccess As Boolean

        Dim udtFileInfo() As udtFileInfoType
        ReDim udtFileInfo(0)

        Dim strFileExtensionsPrefList() As String

        Dim strConnectionString As String
        Dim strFolderPath As String
        Dim strBaseName As String

        Dim objDirInfo As System.IO.DirectoryInfo

        Dim blnCopyFile As Boolean
        Dim objFileList() As System.IO.FileInfo
        Dim intSortKey() As Integer

        strConnectionString = m_mgrParams.GetParam("ConnectionString")

        blnSuccess = GetPeakListFilePaths(intJobNum, strConnectionString, udtFileInfo)
        If Not blnSuccess Then
            Return False
        End If

        ' Initialize strFileExtensionsPrefList
        strFileExtensionsPrefList = GetFileExtensionPrefList()

        ' Copy the _isos.csv or .pek file for each job returned by the reader

        For intIndex = 0 To udtFileInfo.Length - 1
            If System.IO.Directory.Exists(udtFileInfo(intIndex).FolderPath) Then
                strFolderPath = udtFileInfo(intIndex).FolderPath
            ElseIf System.IO.Directory.Exists(udtFileInfo(intIndex).FolderPathLocal) Then
                strFolderPath = udtFileInfo(intIndex).FolderPathLocal
            Else
                ' Folder not found

                Dim ErrMsg As String = "clsAnalysisResourcesMA.GetPeakListFiles(), folder path not found for job " & intJobNum.ToString
                m_logger.PostEntry(ErrMsg, ILogger.logMsgType.logError, True)
                Return False
            End If

            objDirInfo = New System.IO.DirectoryInfo(strFolderPath)
            ReDim objFileList(-1)

            blnCopyFile = False
            For intExtensionIndex = 0 To strFileExtensionsPrefList.Length - 1
                objFileList = objDirInfo.GetFiles("*" & strFileExtensionsPrefList(intExtensionIndex))

                If objFileList.Length > 0 Then
                    If objFileList.Length > 1 Then
                        ' Multiple files found; find the best match
                        ' For _isos.csv files, we just use the first file
                        ' For .Pek files, we preferentially copy .Pek files vs. _ic.pek or _s.pek files 

                        If strFileExtensionsPrefList(intExtensionIndex) = PEK_FILE_SUFFIX Then
                            ReDim intSortKey(objFileList.Length - 1)

                            For intFileIndex = 0 To objFileList.Length - 1
                                strBaseName = System.IO.Path.GetFileNameWithoutExtension(objFileList(intFileIndex).Name)
                                intSortKey(intFileIndex) = 0

                                If strBaseName.Length >= 3 AndAlso strBaseName.Chars(strBaseName.Length - 2) = "_"c Then
                                    intSortKey(intFileIndex) = 1
                                ElseIf strBaseName.Length >= 4 AndAlso strBaseName.Chars(strBaseName.Length - 3) = "_"c Then
                                    intSortKey(intFileIndex) = 1
                                End If
                            Next

                            ' Sort intSortKey() and sort objFileList() parallel to it
                            Array.Sort(intSortKey, objFileList)

                        End If
                    End If

                    blnCopyFile = True
                    Exit For
                End If

            Next intExtensionIndex

            If blnCopyFile Then
                objFileList(0).CopyTo(System.IO.Path.Combine(strWorkingDirectory, objFileList(0).Name), True)
            End If


        Next intIndex

        Return blnSuccess
    End Function

    Protected Function GetPeakListFilePaths(ByVal intJobNum As Integer, ByVal strConnectionString As String, ByRef udtFileInfo() As udtFileInfoType) As Boolean

        Const spName As String = SP_NAME_GET_MULTIALIGN_DATA_FOLDER_PATHS

        Dim objConnection As System.Data.SqlClient.SqlConnection
        Dim objSP As System.Data.SqlClient.SqlCommand
        Dim objReader As System.Data.SqlClient.SqlDataReader

        Dim intFileCount As Integer

        intFileCount = 0
        ReDim udtFileInfo(9)

        If m_DebugLevel > 4 Then
            Dim MyMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), connection string: " & strConnectionString
            m_logger.PostEntry(MyMsg, ILogger.logMsgType.logDebug, True)
        End If

        Try
            ' Initialize the connection
            objConnection = New System.Data.SqlClient.SqlConnection(strConnectionString)
            objConnection.Open()
        Catch ex As Exception
            ' Error obtaining data
            Dim ErrMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), error initializing the connection " & ex.Message
            m_logger.PostEntry(ErrMsg, ILogger.logMsgType.logError, True)
            Return False
        End Try

        If m_DebugLevel > 4 Then
            Dim MyMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), connection established; will now call " & spName
            m_logger.PostEntry(MyMsg, ILogger.logMsgType.logDebug, True)
        End If

        ' Obtain the paths for the jobs associated with this task
        objSP = New System.Data.SqlClient.SqlCommand()
        With objSP
            .CommandType = System.Data.CommandType.StoredProcedure
            .CommandText = spName
            .Connection = objConnection

            .Parameters.Add(New SqlClient.SqlParameter("@Return", SqlDbType.Int))
            .Parameters.Item("@Return").Direction = ParameterDirection.ReturnValue

            .Parameters.Add(New SqlClient.SqlParameter("@JobNum", SqlDbType.Int))
            .Parameters.Item("@JobNum").Direction = ParameterDirection.Input
            .Parameters.Item("@JobNum").Value = intJobNum

            .Parameters.Add(New SqlClient.SqlParameter("@message", SqlDbType.VarChar, 512))
            .Parameters.Item("@message").Direction = ParameterDirection.InputOutput
            .Parameters.Item("@message").Value = ""
        End With

        Try
            objReader = objSP.ExecuteReader()
        Catch ex As Exception
            ' Error obtaining data
            Dim ErrMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), error calling " & spName & " to retrieve job paths: " & ex.Message
            m_logger.PostEntry(ErrMsg, ILogger.logMsgType.logError, True)
            Return False
        End Try

        Try
            If Not objReader.HasRows Then
                Dim ErrMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), No data returned by " & spName & " for job " & intJobNum.ToString
                m_logger.PostEntry(ErrMsg, ILogger.logMsgType.logError, True)
                Return False
            End If

            ' Populate udtFileInfo with the data returned by the SP
            Do While objReader.Read
                If intFileCount >= udtFileInfo.Length Then
                    ReDim Preserve udtFileInfo(udtFileInfo.Length * 2 - 1)
                End If

                With udtFileInfo(intFileCount)
                    .Job = objReader.GetInt32(0)
                    .FolderPath = objReader.GetString(1)
                    .FolderPathLocal = objReader.GetString(2)
                    .FileName = String.Empty
                End With

                intFileCount += 1
            Loop

            objReader.Close()
            objConnection.Close()

        Catch ex As Exception
            ' Error obtaining data
            Dim ErrMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), error retrieving job paths: " & ex.Message
            m_logger.PostEntry(ErrMsg, ILogger.logMsgType.logError, True)
            Return False
        End Try

        If m_DebugLevel > 4 Then
            Dim MyMsg As String = "clsAnalysisResourcesMA.GetPeakListFilePaths(), obtained information on " & intFileCount.ToString & " DMS job(s) for MultiAlign job " & intJobNum.ToString
            m_logger.PostEntry(MyMsg, ILogger.logMsgType.logDebug, True)
        End If

        If intFileCount > 0 Then
            If udtFileInfo.Length <> intFileCount Then
                ReDim Preserve udtFileInfo(intFileCount - 1)
            End If
        Else
            ReDim udtFileInfo(-1)
        End If

        Return True

    End Function

    Public Overrides Function GetResources() As IJobParams.CloseOutType

        Dim ParamFile As String = m_jobParams.GetParam("ParmFileName")
        'Dim SettingsFile As String = m_jobParams.GetParam("settingsFileName")
        'Dim OrgDB As String = m_jobParams.GetParam("organismDBName")
        Dim strErrorMessage As String

        m_WorkingDir = m_mgrParams.GetParam("WorkDir")

        'Make log entry about starting resource retrieval
        m_logger.PostEntry(m_MachName & ": Retrieving files, job " & m_JobNum, _
        ILogger.logMsgType.logNormal, LOG_DATABASE)

        'Copy param file, if one is specified
        If ParamFile.ToLower <> "na" Then
            If Not RetrieveParamFile(ParamFile, m_jobParams.GetParam("ParmFileStoragePath"), m_WorkingDir) Then
                strErrorMessage = "Error copying param file, job " & m_JobNum
                m_logger.PostEntry(strErrorMessage, ILogger.logMsgType.logError, LOG_DATABASE)
                '				m_message = AppendToComment(m_jobParams.GetParam("comment"), "Error copying param file")
                m_message = "Error copying param file"
                Return IJobParams.CloseOutType.CLOSEOUT_FAILED
            End If
        End If

        ' Copy the ICR-2LS or Decon2LS results files
        ' The path to the files is obtained using a stored procedure that de-references the Job Numbers to folder paths

        If Not GetPeakListFiles(m_WorkingDir, CInt(m_JobNum)) Then
            strErrorMessage = "Error obtaining Peak List files, job " & m_JobNum
            m_logger.PostEntry(strErrorMessage, ILogger.logMsgType.logError, LOG_DATABASE)
            m_message = "Error copying peak list files"
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        'You got to here, everything must be cool!
        Return IJobParams.CloseOutType.CLOSEOUT_SUCCESS

    End Function

	Protected Overrides Function RetrieveParamFile(ByVal ParamFileName As String, _
																	ByVal ParamFilePath As String, ByVal WorkDir As String) As Boolean

		Dim result As Boolean = True

		' MultiAlign just copies its parameter file from the central repository

		m_logger.PostEntry("Getting param file", ILogger.logMsgType.logNormal, True)

        If CopyFileToWorkDir(ParamFileName, ParamFilePath, WorkDir) Then
            Return True
        Else
            Return False
        End If

		' set up run parameter file to reference spectra file, taxonomy file, and analysis parameter file
		result = result And MakeInputFile()

        Return result

	End Function

	Protected Function MakeInputFile() As Boolean
		Dim result As Boolean = True

        ' Set up input file to be provided to MultiAlign

		Dim WorkingDir As String = m_mgrParams.GetParam("WorkDir")
        Dim OrganismName As String = m_jobParams.GetParam("OrganismName")
        Dim ParamFilePath As String = Path.Combine(WorkingDir, m_jobParams.GetParam("ParmFileName"))
        'Dim SpectrumFilePath As String = Path.Combine(WorkingDir, m_jobParams.GetParam("datasetNum") & "_dta.txt")
        'Dim TaxonomyFilePath As String = Path.Combine(WorkingDir, "taxonomy.xml")
        'Dim OutputFilePath As String = Path.Combine(WorkingDir, m_jobParams.GetParam("datasetNum") & "_xt.xml")

		'make input file
		'start by adding the contents of the parameter file.
		'replace substitution tags in input_base.txt with proper file path references
		'and add to input file (in proper XML format)
'		Try
'			' Create an instance of StreamWriter to write to a file.
'			Dim inputFile As StreamWriter = New StreamWriter(Path.Combine(WorkingDir, "input.xml"))
'			' Create an instance of StreamReader to read from a file.
'			Dim inputBase As StreamReader = New StreamReader(Path.Combine(WorkingDir, "input_base.txt"))
'			Dim paramFile As StreamReader = New StreamReader(ParamFilePath)
'			Dim paramLine As String
'			Dim inpLine As String
'			Dim tmpFlag As Boolean
'			' Read and display the lines from the file until the end 
'			' of the file is reached.
'			Do
'				paramLine = paramFile.ReadLine()
'				If paramLine Is Nothing Then
'					Exit Do
'				End If
'				inputFile.WriteLine(paramLine)
'				If paramLine.IndexOf("<bioml>") <> -1 Then
'					Do
'						inpLine = inputBase.ReadLine()
'						If Not inpLine Is Nothing Then
'							inpLine = inpLine.Replace("ORGANISM_NAME", OrganismName)
'							inpLine = inpLine.Replace("TAXONOMY_FILE_PATH", TaxonomyFilePath)
'							inpLine = inpLine.Replace("SPECTRUM_FILE_PATH", SpectrumFilePath)
'							inpLine = inpLine.Replace("OUTPUT_FILE_PATH", OutputFilePath)
'							inputFile.WriteLine(inpLine)
'							tmpFlag = False
'						End If
'					Loop Until inpLine Is Nothing
'				End If
'			Loop Until paramLine Is Nothing
'			inputBase.Close()
'			inputFile.Close()
'			paramFile.Close()
'		Catch E As Exception
'			' Let the user know what went wrong.
'			m_logger.PostError("The file could not be read", E, True)
'			result = False
'		End Try

		'get rid of base file
'		File.Delete(Path.Combine(WorkingDir, "input_base.txt"))

		Return result
    End Function

End Class
