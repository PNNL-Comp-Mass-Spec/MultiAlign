Option Strict On

Imports AnalysisManagerBase
Imports PRISM.Logging
Imports AnalysisManagerBase.clsGlobal

Public Class clsAnalysisToolRunnerMA
	Inherits clsAnalysisToolRunnerBase


#Region "Module Variables"

    Protected Const PROGRESS_PCT_MULTIALIGN_RUNNING As Single = 5

    Protected Const XML_PARAM_FILE_NAME As String = "input.xml"

    Protected WithEvents CmdRunner As clsRunDosProgram

    ' Note: This array contains file names and not full paths
    ' These files will be in m_WorkDir
    Protected mDataFileNames() As String

#End Region

    Protected Function CreateCustomParamFile(ByVal strTemplateParamFileName As String, ByVal strCustomParamFileName As String, ByVal strDataFileNames() As String) As Boolean

        ' Create the parameter file that specifies the input data filenames

        Dim strCustomParamFilePath As String
        Dim objXMLFile As System.Xml.XmlTextWriter
        Dim intIndex As Integer
        Dim blnSuccess As Boolean = False

        strCustomParamFilePath = System.IO.Path.Combine(m_WorkDir, strCustomParamFileName)

        Try
            If System.IO.File.Exists(strCustomParamFilePath) Then
                System.IO.File.Delete(strCustomParamFilePath)
            End If

            objXMLFile = New System.Xml.XmlTextWriter(strCustomParamFilePath, System.Text.Encoding.ASCII)
            objXMLFile.Formatting = Xml.Formatting.Indented

            objXMLFile.WriteStartDocument()
            objXMLFile.WriteStartElement("MultiAlignOptions")
            For intIndex = 0 To strDataFileNames.Length - 1
                objXMLFile.WriteStartElement("DataFileName")
                objXMLFile.WriteAttributeString("ID", intIndex.ToString)
                objXMLFile.WriteString(strDataFileNames(intIndex))
                objXMLFile.WriteEndElement()
            Next
            objXMLFile.WriteEndElement()
            objXMLFile.WriteEndDocument()
            objXMLFile.Flush()
            objXMLFile.Close()

            blnSuccess = True

        Catch ex As Exception
            m_logger.PostError("Error in CreateCustomParamFile for job " & m_JobNum, ex, LOG_DATABASE)
            blnSuccess = False
        End Try

        Return blnSuccess

    End Function

    Protected Function DeleteDataFiles(ByVal strDataFileNames() As String) As IJobParams.CloseOutType

        'Deletes the input files from the working directory

        Dim FoundFiles() As String
        Dim MyFile As String
        Dim intIndex As Integer
        Dim eSuccess As IJobParams.CloseOutType = IJobParams.CloseOutType.CLOSEOUT_FAILED

        ' Delete the input files
        Try
            If strDataFileNames Is Nothing OrElse strDataFileNames.Length = 0 Then
                ' Look for the files to delete

                FoundFiles = System.IO.Directory.GetFiles(m_WorkDir, "*.pek")
                For Each MyFile In FoundFiles
                    DeleteFileWithRetries(MyFile)
                Next MyFile

                FoundFiles = System.IO.Directory.GetFiles(m_WorkDir, "*_isos.csv")
                For Each MyFile In FoundFiles
                    DeleteFileWithRetries(MyFile)
                Next MyFile

            Else
                ' Delete the files in strDataFileNames

                For intIndex = 0 To strDataFileNames.Length - 1
                    DeleteFileWithRetries(System.IO.Path.Combine(m_WorkDir, strDataFileNames(intIndex)))
                Next intIndex
            End If

            eSuccess = IJobParams.CloseOutType.CLOSEOUT_SUCCESS

        Catch Err As Exception
            m_logger.PostError("Error finding input data files to delete, job " & m_JobNum, Err, LOG_DATABASE)
            eSuccess = IJobParams.CloseOutType.CLOSEOUT_FAILED
        End Try

        Return eSuccess

    End Function

    Protected Function DeleteDuplicateFilesInTarget(ByVal strSourceFolderPath As String, ByVal strTargetFolderPath As String) As Boolean

        Dim objSourceDir As System.IO.DirectoryInfo
        Dim objFile As System.IO.FileInfo
        Dim objFileCheck As System.IO.FileInfo

        Dim blnSuccess As Boolean = False

        Try
            objSourceDir = New System.IO.DirectoryInfo(strSourceFolderPath)

            For Each objFile In objSourceDir.GetFiles()
                objFileCheck = New System.IO.FileInfo(System.IO.Path.Combine(strTargetFolderPath, objFile.Name))
                If objFileCheck.Exists Then
                    objFileCheck.Delete()
                End If
            Next

            blnSuccess = True
        Catch ex As Exception
            m_logger.PostError("Error in DeleteDuplicateFilesInTarget for job " & m_JobNum, ex, LOG_DATABASE)
            blnSuccess = False
        End Try

        Return blnSuccess

    End Function

    Protected Function FindDataFiles(ByVal strWorkDir As String, ByRef strDataFiles() As String) As Boolean
        ' Look for files named _isos.csv and .Pek in strWorkDir

        Dim intExtensionIndex As Integer
        Dim strFileExtensionList() As String

        Dim objDirInfo As System.IO.DirectoryInfo
        Dim objFile As System.IO.FileInfo

        Dim intDataFileCount As Integer
        Dim blnSuccess As Boolean = False

        Try
            ' Initialize the file extension list
            strFileExtensionList = clsAnalysisResourcesMA.GetFileExtensionPrefList

            objDirInfo = New System.IO.DirectoryInfo(strWorkDir)

            intDataFileCount = 0
            ReDim strDataFiles(9)

            For intExtensionIndex = 0 To strFileExtensionList.Length - 1
                For Each objFile In objDirInfo.GetFiles("*" & strFileExtensionList(intExtensionIndex))
                    If intDataFileCount >= strDataFiles.Length Then
                        ReDim Preserve strDataFiles(strDataFiles.Length * 2 - 1)
                    End If

                    strDataFiles(intDataFileCount) = objFile.Name
                    intDataFileCount += 1
                Next objFile
            Next intExtensionIndex

            If intDataFileCount <> strDataFiles.Length Then
                ReDim Preserve strDataFiles(intDataFileCount - 1)
            End If

            If strDataFiles.Length > 0 Then
                blnSuccess = True
            Else
                m_logger.PostEntry("Could not find any matching data files in the working directory for job " & m_JobNum, ILogger.logMsgType.logError, LOG_DATABASE)
                blnSuccess = False
            End If

        Catch ex As Exception
            m_logger.PostError("Error in CreateCustomParamFile for job " & m_JobNum, ex, LOG_DATABASE)
            blnSuccess = False
        End Try

        Return blnSuccess

    End Function

    Protected Overrides Function MakeResultsFolder(ByVal AnalysisType As String) As IJobParams.CloseOutType

        Dim strResFolderNameNew As String
        Dim eSuccess As IJobParams.CloseOutType = IJobParams.CloseOutType.CLOSEOUT_FAILED

        Try
            MyBase.MakeResultsFolder("MA")
        Catch ex As Exception
            m_logger.PostEntry("Error making results folder for job " & m_JobNum, ILogger.logMsgType.logError, LOG_DATABASE)
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End Try


        ' Rename the results folder from m_ResFolderName to the name specified by m_jobParams.GetParam("ResultsFolderName"))
        ' When doing this, must update m_ResFolderName since the analysis manager will use that name when delivering (copying) the results to the transfer folder
        Try
            Dim strOldPath As String = System.IO.Path.Combine(m_WorkDir, m_ResFolderName)
            strResFolderNameNew = m_jobParams.GetParam("ResultsFolderName")

            Dim strNewPath As String = System.IO.Path.Combine(m_WorkDir, strResFolderNameNew)

            System.IO.Directory.Move(strOldPath, strNewPath)

            m_ResFolderName = String.Copy(strResFolderNameNew)

            eSuccess = IJobParams.CloseOutType.CLOSEOUT_SUCCESS
        Catch ex As Exception
            m_logger.PostEntry("Error renaming results folder for job " & m_JobNum, ILogger.logMsgType.logError, LOG_DATABASE)
            eSuccess = IJobParams.CloseOutType.CLOSEOUT_FAILED
        End Try

        Return eSuccess

    End Function

    Public Function OperateAnalysisTool() As IJobParams.CloseOutType
        Dim CmdStr As String

        CmdRunner = New clsRunDosProgram(m_logger, m_WorkDir)

        If m_DebugLevel > 4 Then
            m_logger.PostEntry("clsAnalysisToolRunnerMA.OperateAnalysisTool(): Enter", ILogger.logMsgType.logDebug, True)
        End If

        ' verify that program file exists
        Dim progLoc As String = m_mgrParams.GetParam("MultiAlignprogloc")
        If Not System.IO.File.Exists(progLoc) Then
            m_logger.PostEntry("Cannot find MultiAlign program file", ILogger.logMsgType.logError, True)
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        'Set up and execute a program runner to run MultiAlign
        CmdStr = XML_PARAM_FILE_NAME
        If Not CmdRunner.RunProgram(progLoc, CmdStr, "MultiAlign", True) Then
            m_logger.PostEntry("Error running MultiAlign, job " & m_JobNum, ILogger.logMsgType.logError, LOG_DATABASE)
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        'Zip the output file
        Dim ZipResult As IJobParams.CloseOutType = ZipMainOutputFile()

        Return ZipResult

    End Function

    Protected Overridable Function PerfPostAnalysisTasks(ByVal ResType As String) As IJobParams.CloseOutType

        Dim StepResult As IJobParams.CloseOutType

        'Stop the job timer
        m_StopTime = Now()

        'Get rid of the raw data files
        StepResult = DeleteDataFiles(mDataFileNames)
        If StepResult <> IJobParams.CloseOutType.CLOSEOUT_SUCCESS Then
            Return StepResult
        End If

        'Update the job summary file
        If Not UpdateSummaryFile() Then
            m_logger.PostEntry("Error creating summary file, job " & m_JobNum, _
             ILogger.logMsgType.logWarning, LOG_DATABASE)
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        StepResult = MakeResultsFolder(ResType)
        If StepResult <> IJobParams.CloseOutType.CLOSEOUT_SUCCESS Then
            Return StepResult
        End If

        Return IJobParams.CloseOutType.CLOSEOUT_SUCCESS

    End Function

    Protected Function RunMultiAlign() As IJobParams.CloseOutType

        ' ToDo: Call OperateAnalysisTool() or comparable function

        Return IJobParams.CloseOutType.CLOSEOUT_SUCCESS

    End Function

    Public Overrides Function RunTool() As IJobParams.CloseOutType

        Dim StepResult As IJobParams.CloseOutType
        Dim blnSuccess As Boolean

        ' Verify that the Transfer Folder exists on the server; create it if missing
        If Not VerifyXferFolder() Then
            m_message = AppendToComment(m_message, "Error performing post analysis tasks")
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        ' Get the settings file info via the base class
        ' At present, MultiAlign doesn't have a settings file, so the .RunTool() command simply records the start of the job
        If Not MyBase.RunTool() = IJobParams.CloseOutType.CLOSEOUT_SUCCESS Then
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        ' Determine the input file names by examining the files in the working folder
        blnSuccess = FindDataFiles(m_WorkDir, mDataFileNames)
        If Not blnSuccess Then
            CleanupFailedJob("Error finding input files")
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        blnSuccess = CreateCustomParamFile(m_jobParams.GetParam("ParmFileName"), XML_PARAM_FILE_NAME, mDataFileNames)
        If Not blnSuccess Then
            CleanupFailedJob("Error creating parameter file")
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        ' Start MultiAlign
        m_logger.PostEntry("Calling MultiAlign to process the data files, job " & m_JobNum, ILogger.logMsgType.logNormal, LOG_LOCAL_ONLY)
        Try
            StepResult = RunMultiAlign()
            If StepResult <> IJobParams.CloseOutType.CLOSEOUT_SUCCESS Then
                CleanupFailedJob("Error")
                Return StepResult
            End If
        Catch Err As Exception
            m_logger.PostEntry("clsAnalysisToolRunnerMA.RunTool(), Exception calling MultiAlign, " & _
             Err.Message, ILogger.logMsgType.logError, True)
            CleanupFailedJob("Exception calling MultiAlign, " & Err.Message)
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End Try

        ' Update progress to 100%
        m_progress = 100

        m_StatusTools.UpdateAndWrite(IStatusFile.JobStatus.STATUS_RUNNING, m_progress)

        ' Define the results folder name; this name is provided by the RequestTaskParams stored procedure called by clsAnalysisResourcesMA
        m_ResFolderName = m_jobParams.GetParam("ResultsFolderName")

        ' Run the cleanup routine from the base class
        If PerfPostAnalysisTasks("MA") <> IJobParams.CloseOutType.CLOSEOUT_SUCCESS Then
            CleanupFailedJob("Error performing post analysis tasks")
            Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        End If

        '
        ' ToDo: Remove the call to DeleteDuplicateFilesInTarget if DeliverResults gets updated to overwrite existing files
        '

        ' The DeliverResults function will not overwrite existing files in the target directory
        ' Therefore, this function will look for files that exist in both places and delete the files in the target
        blnSuccess = DeleteDuplicateFilesInTarget(System.IO.Path.Combine(m_WorkDir, m_ResFolderName), System.IO.Path.Combine(m_jobParams.GetParam("transferFolderPath"), m_ResFolderName))

        Return IJobParams.CloseOutType.CLOSEOUT_SUCCESS

    End Function

    Protected Overrides Function UpdateSummaryFile() As Boolean

        'Add a separator
        clsSummaryFile.Add(vbCrLf & "=====================================================================================" & vbCrLf)

        'Add the data
        clsSummaryFile.Add("Job Number" & vbTab & m_JobNum)
        clsSummaryFile.Add("Date" & vbTab & Now())
        clsSummaryFile.Add("Processor" & vbTab & m_MachName)
        clsSummaryFile.Add("Tool" & vbTab & m_jobParams.GetParam("toolname"))
        clsSummaryFile.Add("Priority" & vbTab & m_jobParams.GetParam("priority"))
        clsSummaryFile.Add("Dataset Name" & vbTab & m_jobParams.GetParam("datasetNum"))
        'clsSummaryFile.Add("Dataset Folder Name" & vbTab & m_jobParams.GetParam("datasetFolderName"))
        'clsSummaryFile.Add("Dataset Folder Path" & vbTab & m_jobParams.GetParam("DatasetStoragePath"))
        clsSummaryFile.Add("Results Folder" & vbTab & m_jobParams.GetParam("TransferFolderPath"))
        clsSummaryFile.Add("Param File Name" & vbTab & m_jobParams.GetParam("ParmFileName"))
        clsSummaryFile.Add("Param File Path" & vbTab & m_jobParams.GetParam("ParmFileStoragePath"))
        'clsSummaryFile.Add("Settings File Name" & vbTab & m_jobParams.GetParam("settingsFileName"))
        'clsSummaryFile.Add("Settings File Path" & vbTab & m_jobParams.GetParam("settingsFileStoragePath"))
        clsSummaryFile.Add("Analysis Time (hh:mm)" & vbTab & CalcElapsedTime(m_StartTime, m_StopTime))

        'Add another separator
        clsSummaryFile.Add(vbCrLf & "=====================================================================================" & vbCrLf)

        Return True

    End Function

    Private Function VerifyXferFolder() As Boolean

        Dim XferDir As String = m_jobParams.GetParam("TransferFolderPath")

        ' Verify xfer System.IO.Directory exists
        If Not System.IO.Directory.Exists(XferDir) Then
            Try
                ' Note that the .CreateDirectory command will create multiple layers of folders, if necessary
                System.IO.Directory.CreateDirectory(XferDir)
            Catch err As Exception
                m_logger.PostError("Unable to create server results folder, job " & m_JobNum, err, LOG_DATABASE)
                m_message = "Unable to create server results folder"
                Return False
            End Try
        End If

        Return True

    End Function

    Private Function ZipMainOutputFile() As IJobParams.CloseOutType
        '		Dim TmpFile As String
        '		Dim FileList() As String
        '		Dim ZipFileName As String
        '
        '		'Zip concatenated XML output files (should only be one)
        '		Dim Zipper As New clsSharpZipWrapper
        '		FileList = System.IO.Directory.GetFiles(m_workdir, "*_xt.xml")
        '		For Each TmpFile In FileList
        '			ZipFileName = Path.Combine(m_workdir, Path.GetFileNameWithoutExtension(TmpFile)) & ".zip"
        '			If Not Zipper.ZipFilesInFolder(ZipFileName, m_workdir, False, Path.GetFileName(TmpFile)) Then
        '				Dim Msg As String = "Error zipping output files, job " & m_jobnum & ": " & Zipper.ErrMsg
        '				m_logger.PostEntry(Msg, ILogger.logMsgType.logError, LOG_DATABASE)
        '				m_message = AppendToComment(m_message, "Error zipping output files")
        '				Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        '			End If
        '		Next
        '
        '		'Delete the XML output files
        '		Try
        '			FileList = System.IO.Directory.GetFiles(m_workdir, "*_xt.xml")
        '			For Each TmpFile In FileList
        '				File.SetAttributes(TmpFile, File.GetAttributes(TmpFile) And (Not FileAttributes.ReadOnly))
        '				File.Delete(TmpFile)
        '			Next
        '		Catch Err As Exception
        '			m_logger.PostError("Error deleting _xt.xml file, job " & m_JobNum, Err, LOG_DATABASE)
        '			Return IJobParams.CloseOutType.CLOSEOUT_FAILED
        '		End Try

        Return IJobParams.CloseOutType.CLOSEOUT_SUCCESS

    End Function


    Private Sub CmdRunner_LoopWaiting() Handles CmdRunner.LoopWaiting

        'Update the status file
        m_StatusTools.UpdateAndWrite(PROGRESS_PCT_MULTIALIGN_RUNNING)

    End Sub

End Class
