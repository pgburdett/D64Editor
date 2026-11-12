Attribute VB_Name = "Module1"
'
' Module1.bas
'
'  Written by
'  Forrest Mook <forrest@almighty.c64.org>
'  Copyright 1999-2016
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA
' 02111-1307  USA.

Public FTYPE As String
Public strBin As String
Public FILECLOSED As String
Public FILELOCKED As String
Public sfilename(0 To 145) As String
Public sstartingtrack(0 To 145) As Integer
Public sstartingsector(0 To 145) As Integer
Public sfilesize1(0 To 145) As Long
Public sfilesize2(0 To 145) As Long
Public sfilesizeDisplay(0 To 145) As Long
Public sfiletype(0 To 145) As Byte
'crap needed for REL and geos type files
Public byte15(0 To 145) As Byte
Public byte16(0 To 145) As Byte
Public byte17(0 To 145) As Byte
Public byte18(0 To 145) As Byte
Public byte19(0 To 145) As Byte
Public byte1A(0 To 145) As Byte
Public byte1B(0 To 145) As Byte
Public byte1C(0 To 145) As Byte
Public byte1D(0 To 145) As Byte
Public bambite(0 To 683) As Integer
Public varForBlockViewer As Integer
Public firstKeyPressValue As Integer
Public firstKeyPressToggle As Integer
Public blocksAllocated(0 To 683) As Integer
Public blockOwner(0 To 683) As String
Public blocksNeedingWiped(0 To 683) As Integer
Public ErrorCodeBlocks(0 To 683) As Byte
Public cantFitDirOnDisk As Integer
Public operatingSystemVersion As Integer
Public ImportFilenames(0 To 100) As String
Public WindowsDirectoryPath As String
Public seperatorLineText As String
Public FileNotWiped As Integer
Public D64Modified As Boolean
Public convertToASCII As Integer
Public HexEnabled As Boolean:  ' So HexEditor doesnt go into infinite loop
Public warnedAboutInterleave As Integer

Public ltemp As Long
Public convertedSector As Integer
Public convertedTrack As Integer

'variables needed for saving window states
Public frmBAMLEFT As Integer
Public frmBAMTOP As Integer
Public frmBlockViewerLEFT As Integer
Public frmBlockViewerTOP As Integer
Public frmFilePropertiesLEFT As Integer
Public frmFilePropertiesTOP As Integer
Public frmMainLEFT As Integer
Public frmMainTOP As Integer
Public frmSectorDumpLEFT As Integer
Public frmSectorDumpTOP As Integer
Public frmCrossLinksLEFT As Integer
Public frmCrossLinksTOP As Integer
Public frmFileViewerTOP As Integer
Public frmFileViewerLEFT As Integer
Public frmConfigLEFT As Integer
Public frmConfigTOP As Integer
Public frmMemoryLEFT As Integer
Public frmMemoryTOP As Integer
Public frmFileNameBuilderLEFT As Integer
Public frmFileNameBuilderTOP As Integer
Public frmMainCreateNewD64LEFT As Integer
Public frmMainCreateNewD64TOP As Integer
Public lastDirPath As String
Public FileExportPath As String
Public DirEntryBeingEdited As Integer
Public warningsDisabled As Integer
Public safeClean As Integer
Public disableCrosslinkCheck As Integer
Public FileViewerColumnMode As Integer
Public OriginalFileName As String
Public TempFileName1 As String
Public BlockViewerASCIIMode As Integer
Public FileNameBuilderSTR As String
Public FirstBlockToUse As Integer
Public CurrentBlockBeingWritten As Integer
Public nextBlocktoUse As Integer
Public block As Integer
Public track As Integer
Public sector As Integer
Public distance As Integer
Public direction As Integer ' 1 = upward , 2 = downward
Public SelectNextFileUponSaveVar As Integer
Public treatTrack18FilesAsEmpty As Integer
Public InterLeave As Integer
Public UserSpecifiedFirstTrack  As Integer
Public UserSpecifiedFirstSector As Integer
' Memory Cache variables
Public memoryCache(0 To 200) As String
Public memoryFilename(0 To 200) As String
Public memoryFileType(0 To 200) As Byte
Public memoryFileLength(0 To 200) As Long
Public memorydisplayFilename(0 To 200) As String

Public Declare Function ShellExecute Lib "shell32.dll" Alias _
"ShellExecuteA" (ByVal hWnd As Long, ByVal lpOperation As String, ByVal lpFile As String, _
ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long


Public Declare Function GetTempFilename Lib "kernel32" Alias _
   "GetTempFileNameA" (ByVal lpszPath As String, ByVal lpPrefixString _
   As String, ByVal wUnique As Long, ByVal lpTempFileName As String) As Long

   Public Declare Function GetTempPath Lib "kernel32" Alias _
   "GetTempPathA" (ByVal nBufferLength As Long, ByVal lpBuffer _
   As String) As Long



  ' All of this crap before form_load and most of list1_mousedown used for highlighting
  ' the file when right clicking on it.
  ' Taken from the following web address.
  ' http://www.mvps.org/vbnet/code/listapi/rightmouseclick.htm
  Public Const LB_ITEMFROMPOINT As Long = &H1A9

  Public Declare Function SendMessage Lib "user32" _
     Alias "SendMessageA" _
   (ByVal hWnd As Long, _
     ByVal wMsg As Long, _
     ByVal wParam As Long, _
     lParam As Long) As Long
     
  Public Declare Sub CopyMemory Lib "kernel32" _
     Alias "RtlMoveMemory" _
    (Destination As Any, Source As Any, _
     ByVal Length As Long)


  Public Declare Function GetCursorPos Lib "user32" _
    (lpPoint As POINTAPI) As Long

  Public Declare Function ScreenToClient Lib "user32" _
    (ByVal hWnd As Long, lpPoint As POINTAPI) As Long


Public Type BROWSEINFO
    hOwner As Long
    pidlRoot As Long
    pszDisplayName As String
    lpszTitle As String
    ulFlags As Long
    lpfn As Long
    lParam As Long
    iImage As Long
  End Type

  Public Declare Function SHBrowseForFolder Lib _
     "shell32.dll" Alias "SHBrowseForFolderA" _
     (lpBrowseInfo As BROWSEINFO) As Long

  Public Declare Function SHGetPathFromIDList Lib _
     "shell32.dll" Alias "SHGetPathFromIDListA" _
     (ByVal pidl As Long, _
     ByVal pszPath As String) As Long

  Public Declare Sub CoTaskMemFree Lib "ole32.dll" (ByVal pv As Long)


      
  Public Const MAX_PATH = 260
  Public Const WM_USER = &H400
  Public Const BFFM_INITIALIZED = 1

  'Constants ending in 'A' are for Win95 ANSI
  'calls; those ending in 'W' are the wide Unicode
  'calls for NT.

  'Sets the status text to the null-terminated
  'string specified by the lParam parameter.
  'wParam is ignored and should be set to 0.
  Public Const BFFM_SETSTATUSTEXTA As Long = (WM_USER + 100)
  Public Const BFFM_SETSTATUSTEXTW As Long = (WM_USER + 104)

  'If the lParam  parameter is non-zero, enables the
  'OK button, or disables it if lParam is zero.
  '(docs erroneously said wParam!)
  'wParam is ignored and should be set to 0.
  Public Const BFFM_ENABLEOK As Long = (WM_USER + 101)

  'Selects the specified folder. If the wParam
  'parameter is FALSE, the lParam parameter is the
  'PIDL of the folder to select , or it is the path
  'of the folder if wParam is the C value TRUE (or 1).
  'Note that after this message is sent, the browse
  'dialog receives a subsequent BFFM_SELECTIONCHANGED
  'message.
  Public Const BFFM_SETSELECTIONA As Long = (WM_USER + 102)
  Public Const BFFM_SETSELECTIONW As Long = (WM_USER + 103)


Public Declare Function GetWindowsDirectory Lib "kernel32" Alias "GetWindowsDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Long) As Long

' code to get the Windows version taken from http://vbapi.com/ref/g/getversionex.html
Public Declare Function GetVersionEx Lib "kernel32.dll" Alias "GetVersionExA" (lpVersionInformation As OSVERSIONINFO) As Long

Type OSVERSIONINFO
  dwOSVersionInfoSize As Long
  dwMajorVersion As Long
  dwMinorVersion As Long
  dwBuildNumber As Long
  dwPlatformId As Long
  szCSDVersion As String * 128
End Type


'specific to the PIDL method
  'Undocumented call for the example. IShellFolder's
  'ParseDisplayName member function should be used instead.
  Public Declare Function SHSimpleIDListFromPath Lib _
     "shell32" Alias "#162" _
     (ByVal szPath As String) As Long
     
     
 Public Function BrowseCallbackProc(ByVal hWnd As Long, _
                                     ByVal uMsg As Long, _
                                     ByVal lParam As Long, _
                                     ByVal lpData As Long) As Long
    'Callback for the Browse PIDL method.
    'On initialization, set the dialog's
    'pre-selected folder using the pidl
    'set as the bi.lParam, and passed back
    'to the callback as lpData param.
   
     Select Case uMsg
        Case BFFM_INITIALIZED
        
           Call SendMessage(hWnd, BFFM_SETSELECTIONA, _
                            False, ByVal lpData)
                            
           Case Else:
           
     End Select

  End Function


  Public Function FARPROC(pfn As Long) As Long
    
    'A dummy procedure that receives and returns
    'the value of the AddressOf operator.
   
    'Obtain and set the address of the callback
    'This workaround is needed as you can't assign
    'AddressOf directly to a member of a user-
    'defined type, but you can assign it to another
    'long and use that (as returned here)
   
    FARPROC = pfn

  End Function
   

Sub getOSVersion()
  Dim os As OSVERSIONINFO  ' receives version information
  Dim retval As Long  ' return value

  os.dwOSVersionInfoSize = Len(os)  ' set the size of the structure
  retval = GetVersionEx(os)

    'detect if running Win95/98
  If os.dwPlatformId = 1 Then
     frmMain.OLEDropMode = 1
     operatingSystemVersion = 1
  End If

    'detect if running WinNT/2000
  If os.dwPlatformId = 2 Then
     frmMain.OLEDropMode = 1
     operatingSystemVersion = 2
  End If


End Sub

    
Public Function LoWord(dwValue As Long) As Integer

    CopyMemory LoWord, dwValue, 2
    
  End Function


  Public Function MAKELONG(wLow As Long, wHigh As Long) As Long

    MAKELONG = LoWord(wLow) Or (&H10000 * LoWord(wHigh))
    
  End Function


  Public Function MAKELPARAM(wLow As Long, wHigh As Long) As Long

   'Combines two integers into a long integer
    MAKELPARAM = MAKELONG(wLow, wHigh)
    
  End Function

Public Function BrowseForFolderByPIDL(sSelPath As String) As String

     Dim BI As BROWSEINFO
     Dim pidl As Long
     Dim spath As String * MAX_PATH
    
     With BI
        .hOwner = frmMain.hWnd
        .pidlRoot = 0
        .lpszTitle = "Select a folder to Export Files into."
        .lpfn = FARPROC(AddressOf BrowseCallbackProc)
        .lParam = GetPIDLFromPath(sSelPath)
     End With
    
     pidl = SHBrowseForFolder(BI)
    
     If pidl Then
        If SHGetPathFromIDList(pidl, spath) Then
           BrowseForFolderByPIDL = Left$(spath, InStr(spath, vbNullChar) - 1)
        End If
       
       'free the pidl returned by call to SHBrowseForFolder
        Call CoTaskMemFree(pidl)
    End If
    
   'free the pidl set in call to GetPIDLFromPath
    Call CoTaskMemFree(BI.lParam)
    
  End Function


  Public Function GetPIDLFromPath(spath As String) As Long

    'return the pidl to the path supplied by calling the
    'undocumented API #162 (our name SHSimpleIDListFromPath).
    'This function is necessary as, unlike documented APIs,
    'the API is not implemented in 'A' or 'W' versions.

      ' If OS is NT then do this, otherwise it's win95
    If operatingSystemVersion = 2 Then
      GetPIDLFromPath = SHSimpleIDListFromPath(StrConv(spath, vbUnicode))
    Else
      GetPIDLFromPath = SHSimpleIDListFromPath(spath)
    End If

  End Function


Public Function UnqualifyPath(spath As String) As String

    'qualifying a path usually involves assuring
    'that its format is valid, including a trailing slash
    'ready for a filename. Since the SHBrowseForFolder API
    'will pre-select the path if it contains the trailing
    'slash, I call stripping it 'unqualifying the path'.
     If Len(spath) > 0 Then
     
        If Right$(spath, 1) = "\" Then
        
           UnqualifyPath = Left$(spath, Len(spath) - 1)
           Exit Function
        
        End If
     
     End If
     
     UnqualifyPath = spath
     
  End Function

Sub GetWinDir()
  Dim buf As String * 256
  Dim return_len As Long
  return_len = GetWindowsDirectory(buf, Len(buf))
  WindowsDirectoryPath = Left$(buf, return_len)
  frmMain.Label4.Caption = Left$(buf, return_len) & "\d64editor.cfg"
End Sub
               
               
               
            
               
               
               
Sub saveWindowLocations()
  Call GetWinDir
'  cfgFilePath = windir & "\d64editor.cfg"

  If Right(FileExportPath, 1) = "\" Then
     FileExportPath = Left(FileExportPath, Len(FileExportPath) - 1)
  End If
  Open frmMain.Label4.Caption For Output As #1
      Print #1, lastDirPath
      Print #1, frmBAMTOP, "frmBAMTOP"
      Print #1, frmBAMLEFT, "frmBAMLEFT"
      Print #1, frmBlockViewerTOP, "frmBlockViewerTOP"
      Print #1, frmBlockViewerLEFT, "frmBlockViewerLEFT"
      Print #1, frmFilePropertiesTOP, "frmFilePropertiesTOP"
      Print #1, frmFilePropertiesLEFT, "frmFilePropertiesLEFT"
      Print #1, frmMainTOP, "frmMainTOP"
      Print #1, frmMainLEFT, "frmMainLEFT"
      Print #1, frmSectorDumpTOP, "frmSectorDumpTOP"
      Print #1, frmSectorDumpLEFT, "frmSectorDumpLEFT"
      Print #1, frmCrossLinksTOP, "frmCrossLinksTOP"
      Print #1, frmCrossLinksLEFT, "frmCrossLinksLEFT"
      Print #1, frmFileViewerTOP, "frmFileViewerTOP"
      Print #1, frmFileViewerLEFT, "frmFileViewerLEFT"
      Print #1, FileExportPath
      Print #1, warningsDisabled, "warningsDisabled"
      Print #1, safeClean, "safeClean"
      Print #1, disableCrosslinkCheck, "disableCrosslinkCheck"
      Print #1, frmConfigTOP, "frmConfigTOP"
      Print #1, frmConfigLEFT, "frmConfigLEFT"
      Print #1, frmMemoryTOP, "frmMemoryTOP"
      Print #1, frmMemoryLEFT, "frmMemoryLEFT"
      Print #1, FileViewerColumnMode, "FileViewerColumnMode"
      Print #1, convertToASCII, "ConvertToASCII"
      Print #1, seperatorLineText
      Print #1, BlockViewerASCIIMode, "BlockViewerASCIIMode"
      Print #1, frmFileNameBuilderTOP, "frmFileNameBuilderTOP"
      Print #1, frmFileNameBuilderLEFT, "frmFileNameBuilderLEFT"
      Print #1, frmMainCreateNewD64TOP, "frmMainCreateNewD64TOP"
      Print #1, frmMainCreateNewD64LEFT, "frmMainCreateNewD64LEFT"
      Print #1, SelectNextFileUponSaveVar, "SelectNextFileUponSaveVar"
      Print #1, treatTrack18FilesAsEmpty, "TreatTrack18FilesAsEmpty"
      Print #1, InterLeave, "InterLeave"
      Print #1, UserSpecifiedFirstTrack, "ImportFirstTrack"
      Print #1, UserSpecifiedFirstSector, "ImportFirstSector"
  Close #1
End Sub


Sub GETFILETYPE(strBin As String)
   FILECLOSED = ""
   FILELOCKED = ""
   FTYPE = "???"
   
   If Left(strBin, 1) = "0" Then
      FILECLOSED = "*"
   End If
   
   If Mid(strBin, 2, 1) = "1" Then
      FILELOCKED = "<"
   End If
      
   If Right(strBin, 3) = "100" Then
      FTYPE = "REL"
   End If
      
   If Right(strBin, 3) = "011" Then
      FTYPE = "USR"
   End If
   
   If Right(strBin, 3) = "010" Then
      FTYPE = "PRG"
   End If
   
   If Right(strBin, 3) = "001" Then
      FTYPE = "SEQ"
   End If
   
   If Right(strBin, 3) = "000" Then
      FTYPE = "DEL"
    End If
    
   
End Sub


Sub unAllocateDirSectors()
Dim z As Long
Dim nextDirSector As Byte
Dim nextDirTrack As Byte
Dim curDirTrack As Integer
Dim curDirSector As Integer


Open frmMain.dialogopen.Filename For Binary As #1
curDirTrack = 18
curDirSector = 1

z = 91649

' unallocate 18/1
blocksAllocated(358) = 0


For t = 1 To 18
Y = 0

Get #1, z, nextDirTrack
Get #1, z + 1, nextDirSector

 ' check to see if this is the last directory sector
   If nextDirSector = "0" Or nextDirTrack = "0" Then
     Close #1
     Exit Sub
   End If
   
   If nextDirTrack = &H12 Then
      blocksAllocated(nextDirSector + 357) = 0
   Else
      MsgBox "The Directory is not located completely on track 18!" & vbCrLf & "Do not edit this D64 with this program!", vbCritical
   End If

z = z + (256 * (nextDirSector - curDirSector))
curDirSector = nextDirSector

Next t
Close #1
'  ---- END UNALLOCATING DIRECTORY SECTORS


End Sub

Sub checkForFullDir()

Dim z As Long
Dim nextDirSector As Byte
Dim nextDirTrack As Byte
Dim curDirTrack As Integer
Dim curDirSector As Integer
Dim dirBlocks(0 To 18) As Integer
Dim countString As String
Dim countVar As Double

For i = 0 To 18
    dirBlocks(i) = blocksAllocated(i + 357)
Next i

cantFitDirOnDisk = 0
Open frmMain.dialogopen.Filename For Binary As #1
curDirTrack = 18
curDirSector = 1

z = 91649

dirBlocks(1) = 0



For t = 1 To 18
Y = 0

Get #1, z, nextDirTrack
Get #1, z + 1, nextDirSector

 ' check to see if this is the last directory sector
   If nextDirSector = "0" Or nextDirTrack = "0" Then
     Close #1
           
        ' don't check 18/0, it should never be used for a dir sector
       For i = 1 To 18
         If dirBlocks(i) = 0 Then
            blockfree = blockfree + 1
         End If
       Next i
    
       ' now that we know how many total DIR sectors we can use,
       ' lets see if the new directory will fit into that.
         countVar = (frmMain.List1.ListCount + 1) / 8
         countString = countVar
         
         If countVar < 10 Then
           If Len(countString) > 1 Then  'not a whole number, lets chop off and round up
              countVar = Val(Left(countString, 1)) + 1
              ' now we should know how much DIR sector's we'll need to write out the DIR
            End If
         End If
         
         If countVar > 9 Then
           If Len(countString) > 2 Then
              countVar = Val(Left(countString, 2)) + 1
           End If
           
         End If
         
         
        If countVar > blockfree Then  ' uh oh, directory is too big to fit on the disk
            cantFitDirOnDisk = 1
            Exit Sub
        End If
        

     Exit Sub
   End If
   
   If nextDirTrack = &H12 Then
      dirBlocks(nextDirSector) = 0
'      blocksAllocated(nextDirSector + 357) = 0
   Else
      MsgBox "The Directory is not located completely on track 18!" & vbCrLf & "Do not edit this D64 with this program!", vbCritical
   End If

z = z + (256 * (nextDirSector - curDirSector))
curDirSector = nextDirSector

Next t
Close #1



End Sub

Sub writeOutDirectory()
Dim BITE As Byte
Dim tstring As String
Dim b As Integer
Dim z As Long
Dim Y As Integer
Dim nextDirSector As Byte
Dim nextDirTrack As Byte
Dim curDirTrack As Integer
Dim curDirSector As Integer
Dim displayDiskName As String
Dim firstDirSector As Integer
Dim totalfiles As Integer

    ' first unallocate the existing directory sectors
  Call unAllocateDirSectors
  D64Modified = True
  
curDirTrack = 18
curDirSector = 1

totalfiles = frmMain.List1.ListCount

' track 18/1
firstDirSector = 358

b = 0
Open frmMain.dialogopen.Filename For Binary As #1
z = 91649 'start of track 18/1

' Allocate 18/1 (first dir sector)
blocksAllocated(firstDirSector) = 1

' set next dir sector
firstDirSector = firstDirSector + 3

  displayDiskName = frmMain.txtDiskName.Text

  If Len(frmMain.txtDiskName.Text) < 16 Then
     For m = Len(frmMain.txtDiskName.Text) To 15
       displayDiskName = displayDiskName & " "
     Next m
  End If
  
  If Len(frmMain.txtDiskID.Text) < 5 Then
     For m = Len(frmMain.txtDiskID.Text) To 4
       frmMain.txtDiskID.Text = frmMain.txtDiskID.Text & " "
     Next m
  End If


  Put #1, (z + &H90 - 256), displayDiskName
  Put #1, (z + &HA2 - 256), frmMain.txtDiskID.Text

' 18 possible directory sectors used
For t = 1 To 18
Y = 0

' If there are more than 8 files, set the next dir track/sector
If totalfiles > 8 Then

    ' fix implemented in ver 0.030 to fix accidental allocation of 19/0
    ' and failure to write out proper directory if 19/0 was not already
    ' allocated.
    If firstDirSector > 375 Then
        firstDirSector = 359
    End If
    
   Do Until blocksAllocated(firstDirSector) = 0
     If blocksAllocated(firstDirSector) = 1 Then
        firstDirSector = firstDirSector + 1
        If firstDirSector > 375 Then
          firstDirSector = 359
        End If
     End If
   Loop
   
' After finding a free directory sector, allocate it
blocksAllocated(firstDirSector) = 1
  
Call blockNum2TrackandSector(firstDirSector)
firstDirSector = firstDirSector + 3
Else

  convertedTrack = &H0
  convertedSector = &HFF
End If

      ' fill the block with 0's first
      varString$ = String$(256, 0)
      Put #1, z, varString$

Put #1, z, convertedTrack
Put #1, z + 1, convertedSector

' loop through each directory sector 8 times
For c = 0 To 7
If sfilename(b) = "" Then
  Close #1
  Exit Sub
End If

Put #1, (z + &H5 + Y), sfilename(b)


BITE = sstartingtrack(b)
Put #1, z + &H3 + Y, BITE

BITE = sstartingsector(b)
Put #1, z + &H4 + Y, BITE

BITE = sfilesize1(b)
Put #1, z + &H1E + Y, BITE

BITE = sfilesize2(b)
Put #1, z + &H1F + Y, BITE

BITE = sfiletype(b)
Put #1, z + &H2 + Y, BITE

BITE = byte15(b)
Put #1, z + &H15 + Y, BITE

BITE = byte16(b)
Put #1, z + &H16 + Y, BITE

BITE = byte17(b)
Put #1, z + &H17 + Y, BITE

BITE = byte18(b)
Put #1, z + &H18 + Y, BITE

BITE = byte19(b)
Put #1, z + &H19 + Y, BITE

BITE = byte1A(b)
Put #1, z + &H1A + Y, BITE

BITE = byte1B(b)
Put #1, z + &H1B + Y, BITE

BITE = byte1C(b)
Put #1, z + &H1C + Y, BITE

BITE = byte1D(b)
Put #1, z + &H1D + Y, BITE

totalfiles = totalfiles - 1
Y = Y + &H20
   b = b + 1
Next c



 ' check to see if this is the last directory sector
If convertedTrack = 0 Or convertedSector = &HFF Then
   Close #1
   Exit Sub
End If


z = z + (256 * (convertedSector - curDirSector))
curDirSector = convertedSector

Next t
Close #1


End Sub


Sub READBAM()
   Dim z As Long
   Dim i As Long
   Dim o As Integer
   Dim r As Integer
   Dim BBITE1 As Byte
   Dim BBITE2 As Byte
   Dim BBITE3 As Byte
   Dim BBITE4 As Byte
   Dim trackPart1 As String
   Dim trackPart2 As String
   Dim trackPart3 As String
   Dim trackCombined As String
   Dim bamsectors As String
   '  ------- START BAM READ, READ INTO blocksAllocated Array  --------
   Open frmMain.dialogopen.Filename For Binary As #1
   
  i = 91397 'first byte of BAM on track 18/0
  r = 0


  For z = 1 To 35
    Get #1, i, BBITE1   ' Amount of free sectors on each track
    i = i + 1
    
    Get #1, i, BBITE2
    i = i + 1
    
    Call convDecToBin(BBITE2)
      trackPart1 = strBin
    
    Get #1, i, BBITE3
    i = i + 1
    
    Call convDecToBin(BBITE3)
      trackPart2 = strBin
    
    Get #1, i, BBITE4
    i = i + 1
    
    Call convDecToBin(BBITE4)
      trackPart3 = strBin

   trackCombined = trackPart3 & trackPart2 & trackPart1
  
  If z < 18 Then
      For o = 24 To 4 Step -1
      
        If Mid(trackCombined, o, 1) = 0 Then
           blocksAllocated(r) = 1
        Else
           blocksAllocated(r) = 0
        End If
      
        r = r + 1
      Next o
  
  End If
  
  
  If z > 17 And z < 25 Then
      For o = 24 To 6 Step -1
      
        If Mid(trackCombined, o, 1) = 0 Then
           blocksAllocated(r) = 1
        Else
           blocksAllocated(r) = 0
        End If
      
        r = r + 1
      Next o
  
  End If
  
  
  If z > 24 And z < 31 Then
      For o = 24 To 7 Step -1
      
        If Mid(trackCombined, o, 1) = 0 Then
           blocksAllocated(r) = 1
        Else
           blocksAllocated(r) = 0
        End If
      
        r = r + 1
      Next o
  
  End If
  
  
  If z > 30 And z < 36 Then
      For o = 24 To 8 Step -1
      
        If Mid(trackCombined, o, 1) = 0 Then
           blocksAllocated(r) = 1
        Else
           blocksAllocated(r) = 0
        End If
      
        r = r + 1
      Next o
  
  End If
  trackCombined = ""
  Next z
  
  trackPart1 = ""
  trackPart2 = ""
  trackPart3 = ""
     
   Close #1
   '  ------- END BAM READ  --------

End Sub

Sub BAMWRITE()
   Dim z As Long
   Dim i As Long
   Dim o As Integer
   Dim r As Integer
   Dim BBITE1 As Byte
   Dim BBITE2 As Byte
   Dim BBITE3 As Byte
   Dim BBITE4 As Byte
   Dim trackPart1 As String
   Dim trackPart2 As String
   Dim trackPart3 As String
   Dim trackCombined As String
   Dim bamsectors As String


   Dim t As Integer
   Dim b As Integer
   Dim blocksFreeCounter As Integer
   Dim tpart1ToBite As Byte
   Dim tpart2ToBite As Byte
   Dim tpart3ToBite As Byte
   Dim blocks2Bite As Byte
   Dim totalBlocksFreeCounter As Integer
   
   ' -------- START BAM SAVE   -------
    
    D64Modified = True
    
    Open frmMain.dialogopen.Filename For Binary As #1
    
  i = 91397 'first byte of BAM on track 18/0
  t = 0
  blocksUsedCounter = 0
  
  For z = 1 To 35
  
     If z < 18 Then
        b = 5
     End If
  
     If z > 17 And z < 25 Then
        b = 3
     End If
    
     If z > 24 And z < 31 Then
        b = 2
     End If
      
     If z > 30 And z < 36 Then
        b = 1
     End If
  
     For r = 1 To 8
       If blocksAllocated(t) = 1 Then
         trackPart1 = trackPart1 & "0"
       Else
         trackPart1 = trackPart1 & "1"
         blocksFreeCounter = blocksFreeCounter + 1
       End If
       t = t + 1
     Next r
  
     For r = 1 To 8
       If blocksAllocated(t) = 1 Then
         trackPart2 = trackPart2 & "0"
       Else
         trackPart2 = trackPart2 & "1"
         blocksFreeCounter = blocksFreeCounter + 1
       End If
       t = t + 1
     Next r
     
     For r = 1 To b
       If blocksAllocated(t) = 1 Then
         trackPart3 = trackPart3 & "0"
       Else
         trackPart3 = trackPart3 & "1"
         blocksFreeCounter = blocksFreeCounter + 1
       End If
       t = t + 1
     Next r
     
      If z < 18 Then
        trackPart3 = trackPart3 & "000"
     End If
  
     If z > 17 And z < 25 Then
        trackPart3 = trackPart3 & "00000"
     End If
    
     If z > 24 And z < 31 Then
        trackPart3 = trackPart3 & "000000"
     End If
      
     If z > 30 And z < 36 Then
        trackPart3 = trackPart3 & "0000000"
     End If
        
     trackPart1 = ReverseString(trackPart1)
     trackPart2 = ReverseString(trackPart2)
     trackPart3 = ReverseString(trackPart3)
     ' dont count directory toward blocks free total
     If z <> 18 Then
       totalBlocksFreeCounter = totalBlocksFreeCounter + blocksFreeCounter
     End If
     blocks2Bite = blocksFreeCounter
     
     '  ltemp is the binary value back to long
   Call convBinToLong(trackPart1)
      tpart1ToBite = ltemp
   Call convBinToLong(trackPart2)
      tpart2ToBite = ltemp
   Call convBinToLong(trackPart3)
      tpart3ToBite = ltemp
      
   Put #1, i, blocks2Bite
      i = i + 1
   Put #1, i, tpart1ToBite
      i = i + 1
   Put #1, i, tpart2ToBite
      i = i + 1
   Put #1, i, tpart3ToBite
      i = i + 1
      
   trackPart1 = ""
   trackPart2 = ""
   trackPart3 = ""
   blocksFreeCounter = 0
  
  Next z
    frmMain.lblBlocksFree = totalBlocksFreeCounter & " BLOCKS FREE."
    
    Close #1
'  --- END BAM SAVE  ----

End Sub

Sub getBlockOwners()
   Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim block2Mark As Integer
   Dim x As Integer
   Dim showcrosslinks As Integer
   Dim qq As Integer
   Dim pp As Integer
   
   showcrosslinks = 0
   pp = 0
   qq = 0
   x = 1
   block2Mark = 0
   nxtTrack = 0
   nxtSector = 0
   BITE2START = 0
   
   
   On Error GoTo crashhandler
   
   bitespersector = 256
   Load frmCrossLinks
   frmCrossLinks.Top = frmCrossLinksTOP
   frmCrossLinks.Left = frmCrossLinksLEFT
   
     Open frmMain.dialogopen.Filename For Binary As #1

     For qq = 0 To frmMain.List1.ListCount

     nxtTrack = sstartingtrack(qq)
     nxtSector = sstartingsector(qq)

      
      For pp = 1 To 256
      
      If nxtTrack = 0 Then
         Exit For
      End If
      
      
      If nxtTrack < 18 And nxtTrack <> 0 Then
          BITE2START = ((Val(nxtTrack - 1) * 21) * 256) + (Val(nxtSector) * 256)
      End If

      If nxtTrack > 17 And nxtTrack < 25 Then
          BITE2START = (((Val(nxtTrack) - 18) * 19) * 256) + (bitespersector * 17 * 21) + (Val(nxtSector) * 256)
      End If

      If nxtTrack > 24 And nxtTrack < 31 Then
          BITE2START = (((Val(nxtTrack) - 25) * 18) * 256) + (bitespersector * 17 * 21) + (bitespersector * 7 * 19) + (Val(nxtSector) * 256)
      End If
     
      If nxtTrack > 30 And nxtTrack < 36 Then
          BITE2START = (((Val(nxtTrack) - 31) * 17) * 256) + (bitespersector * 17 * 21) + (bitespersector * 7 * 19) + (bitespersector * 6 * 18) + (Val(nxtSector) * 256)
      End If

    block2Mark = BITE2START / 256
    BITE2START = BITE2START + 1
    
      
    
         Get #1, Val(BITE2START), trackbite
         Get #1, Val(BITE2START + 1), sectorbite
         
         nxtTrack = trackbite
         nxtSector = sectorbite
     
  If block2Mark > -1 And block2Mark < 683 Then
     ' check for crosslinked blocks and then set the blockowner
   If blockOwner(block2Mark) = "" Then
      blockOwner(block2Mark) = sfilename(qq)
   Else
      Call blockNum2TrackandSector(block2Mark)
      
      If convertedTrack <> 18 Then
          frmCrossLinks.List1.AddItem Chr$(34) & blockOwner(block2Mark) & Chr$(34) & " IS CROSSLINKED WITH " & Chr$(34) & sfilename(qq) & Chr$(34) & " AT BLOCK #" & convertedTrack & ", " & convertedSector
          showcrosslinks = 1
      End If
   End If
   End If

 Next pp
 
 
   ' no idea why this is necessary, but it seems to be
  If qq = frmMain.List1.ListCount - 1 Then
     Exit For
  End If
  
 Next qq
 
 ' GET DIR BLOCKS NEXT
 
Dim z As Long
Dim nextDirSector As Byte
Dim nextDirTrack As Byte
Dim curDirTrack As Integer
Dim curDirSector As Integer

curDirSector = 0
nextDirSector = 0
 curDirTrack = 18
curDirSector = 1

z = 91649

blockOwner(357) = "<BAM BLOCK>"
blockOwner(358) = "<DIR BLOCK>"


For t = 1 To 18
Y = 0

Get #1, z, nextDirTrack
Get #1, z + 1, nextDirSector

 ' check to see if this is the last directory sector
   If nextDirSector = "0" Or nextDirTrack = "0" Then
     Close #1
  If showcrosslinks = 1 And disableCrosslinkCheck = 0 Then
    frmCrossLinks.Show
  Else
    Unload frmCrossLinks
  End If
     Exit Sub
   End If
   
   If nextDirTrack = &H12 Then
      blockOwner(nextDirSector + 357) = "<DIR BLOCK>"
'   Else
'      MsgBox "The Directory is not located completely on track 18!" & vbCrLf & "Do not edit this D64 with this program!", vbCritical
   End If

z = z + (256 * (nextDirSector - curDirSector))
curDirSector = nextDirSector

Next t
 
      Close #1


  If showcrosslinks = 1 And disableCrosslinkCheck = 0 Then
    frmCrossLinks.Show
  Else
    Unload frmCrossLinks
  End If
  
  
crashhandler:
  MsgBox "Directory appears to be corrupted." & vbCrLf & "It is recommended to NOT use this program with this file.", vbInformation, "Corrupted Directory?"
  frmMain.mnuSaveFile.Enabled = False
  frmMain.List1.Enabled = False
  frmMain.txtDiskID.Enabled = False
  frmMain.txtDiskName.Enabled = False
  Close #1
  Exit Sub

End Sub


Sub findFirstBlocktoUse()

    Dim tries As Integer
    Dim giveUp As Integer
    Dim DoNothing As Integer
    'lets start looking at track/sector 17/0
'    track = 17
'    sector = 0
    ' lets start looking at the user specified track/sector
    track = UserSpecifiedFirstTrack
    sector = UserSpecifiedFirstSector
    
    If track = 0 Or track = 18 Then
        track = 17
    End If
    
    
    distance = 1
    tries = 0
    DoNothing = 0
    convertTrackSectorToBlock
    

    If blocksAllocated(block) = 1 Then 'block is allocated, find another
               
           Do Until blocksAllocated(block) = 0
              
                If (track < 18 And track <> 0) And DoNothing = 0 Then
                    If sector < 20 Then
                      sector = sector + 1
                    Else
                      track = 18 + distance
                      sector = 0
                      tries = tries + 1
                      DoNothing = 1
                    End If
                End If
                If (track > 17 And track < 25) And DoNothing = 0 Then
                    If sector < 18 Then
                      sector = sector + 1
                    Else
                      track = 18 - distance
                      sector = 0
                      tries = tries + 1
                      DoNothing = 1
                    End If
                End If
                
                If (track > 24 And track < 31) And DoNothing = 0 Then
                    If sector < 17 Then
                      sector = sector + 1
                    Else
                      track = 18 - distance
                      sector = 0
                      tries = tries + 1
                      DoNothing = 1
                    End If
                End If
                
                If (track > 30 And track < 36) And DoNothing = 0 Then
                    If sector < 16 Then
                      sector = sector + 1
                    Else
                      track = 18 - distance
                      sector = 0
                      tries = tries + 1
                      DoNothing = 1
                    End If
                End If
                
            convertTrackSectorToBlock
    
            giveUp = giveUp + 1
            If tries = 2 Then
              distance = distance + 1
              tries = 0
            End If
            
           If giveUp = 1500 Then
             track = 0
             sector = 0
             Exit Do
           End If
           DoNothing = 0
           Loop
End If
End Sub

Sub findNextBlocktoUse()
    Dim previousSector As Integer
    Dim tries As Integer
    
  ' previousSector = sector
  
    'sanity check for invalid interleaves
        If InterLeave = 0 Or InterLeave > 15 Then
            InterLeave = 10
        End If
  
    ' step upwards by user selected sector interleave
    sector = sector + InterLeave
previousSector = sector
        If track < 18 And track > 0 Then
            direction = 2
            If sector > 20 Then
              sector = sector - 21
            End If
         End If
         
         If track > 17 And track < 25 Then
            direction = 1
            If sector > 18 Then
              sector = sector - 19
            End If
         End If
         
         If track > 24 And track < 31 Then
            direction = 1
            If sector > 17 Then
              sector = sector - 18
            End If
         End If

         If track > 30 And track < 36 Then
            direction = 1
            If sector > 16 Then
              sector = sector - 17
            End If
         End If

    convertTrackSectorToBlock

    Do Until blocksAllocated(block) = 0

          If track < 18 And track > 0 Then
            direction = 2
            If sector > 20 Then
              sector = sector - 21
            End If
         End If
         
         If track > 17 And track < 25 Then
            direction = 1
            If sector > 18 Then
              sector = sector - 19
            End If
         End If
         
         If track > 24 And track < 31 Then
            direction = 1
            If sector > 17 Then
              sector = sector - 18
            End If
         End If

         If track > 30 And track < 36 Then
            direction = 1
            If sector > 16 Then
              sector = sector - 17
            End If
         End If
        If tries > 60 Then
          If direction = 1 Then
           track = track + 1
           sector = previousSector
           tries = 0
          End If
          If direction = 2 Then
           track = track - 1
           sector = previousSector
           tries = 0
        End If
          
        End If
          
         If track < 18 And track > 0 Then
            direction = 2
            If sector > 20 Then
              sector = sector - 21
            End If
         End If
         
         If track > 17 And track < 25 Then
            direction = 1
            If sector > 18 Then
              sector = sector - 19
            End If
         End If
         
         If track > 24 And track < 31 Then
            direction = 1
            If sector > 17 Then
              sector = sector - 18
            End If
         End If

         If track > 30 And track < 36 Then
            direction = 1
            If sector > 16 Then
              sector = sector - 17
            End If
         End If
          
        If track < 1 Then
          direction = 1
          track = 19
          sector = 0
        End If
        
        If track > 35 Then
          direction = 2
          track = 17
          sector = 0
        End If
         
 
       convertTrackSectorToBlock
 '      MsgBox track & " " & sector & " " & block
     If blocksAllocated(block) <> 0 Then
      sector = sector + 1
     End If
       tries = tries + 1
       
       If tries > 10000 Then
          track = 0
          sector = 0
          Exit Do
       End If
       
    Loop
End Sub

Sub convertTrackSectorToBlock()

      If track < 18 And track <> 0 Then
          block = ((Val(track - 1) * 21)) + (Val(sector))
      End If

      If track > 17 And track < 25 Then
          block = (((Val(track) - 18) * 19)) + 357 + (Val(sector))
      End If

      If track > 24 And track < 31 Then
          block = (((Val(track) - 25) * 18)) + 490 + (Val(sector))
      End If
     
      If track > 30 And track < 36 Then
          block = (((Val(track) - 31) * 17)) + 598 + (Val(sector))
      End If

End Sub

Sub blockNum2TrackandSector(ByVal bnum As Integer)
  Dim trackPlusRemainder As Double
  Dim tempString As String

    ' Track/Sector is lower than track 18
  If bnum < 357 Then
     trackPlusRemainder = (bnum / 21) + 1
     tempString = trackPlusRemainder
     
     For i = 1 To 20
        If Mid(tempString, i, 1) = "." Then
           tempString = Left(tempString, i - 1)
           Exit For
        End If
     Next i
     convertedTrack = Val(tempString)
     trackPlusRemainder = trackPlusRemainder - convertedTrack
     convertedSector = trackPlusRemainder * 21
  End If


  If bnum > 356 And bnum < 490 Then
     trackPlusRemainder = ((bnum - 357) / 19) + 1
     tempString = trackPlusRemainder
     
     For i = 1 To 20
        If Mid(tempString, i, 1) = "." Then
           tempString = Left(tempString, i - 1)
           Exit For
        End If
     Next i
     convertedTrack = Val(tempString)
     trackPlusRemainder = trackPlusRemainder - convertedTrack
     convertedSector = trackPlusRemainder * 19
     convertedTrack = convertedTrack + 17
  End If
  
  
  If bnum > 489 And bnum < 598 Then
     trackPlusRemainder = ((bnum - 490) / 18) + 1
     tempString = trackPlusRemainder
     
     For i = 1 To 20
        If Mid(tempString, i, 1) = "." Then
           tempString = Left(tempString, i - 1)
           Exit For
        End If
     Next i
     convertedTrack = Val(tempString)
     trackPlusRemainder = trackPlusRemainder - convertedTrack
     convertedSector = trackPlusRemainder * 18
     convertedTrack = convertedTrack + 24
  End If
  
  If bnum > 597 Then
     trackPlusRemainder = ((bnum - 598) / 17) + 1
     tempString = trackPlusRemainder
     
     For i = 1 To 20
        If Mid(tempString, i, 1) = "." Then
           tempString = Left(tempString, i - 1)
           Exit For
        End If
     Next i
     convertedTrack = Val(tempString)
     trackPlusRemainder = trackPlusRemainder - convertedTrack
     convertedSector = trackPlusRemainder * 17
     convertedTrack = convertedTrack + 30
  End If

End Sub


 Sub convDecToBin(ByVal curNumber As Integer)
   Dim j As Long

   strBin = ""
   
   For j = 64 To 0 Step -1
      
     If Int(curNumber / (2 ^ j)) = 1 Then

       strBin = strBin & "1"
       curNumber = curNumber - (2 ^ j)

     Else

         strBin = strBin & "0"

     End If

   Next
   
   strBin = Right(strBin, 8)
   
 End Sub
 
 
Sub updateFileCount()
    Dim fcount1 As Integer
    Dim Y As Integer
    Dim BITE1 As Byte
    Dim numPRG As Integer
    Dim numSEQ As Integer
    Dim numREL As Integer
    Dim numDEL As Integer
    Dim numUSR As Integer
    Dim numSCR As Integer
    Dim numUnknown As Integer
    Dim fileCountString As String
    Dim tstring As String
    Dim fstring As String
    
    fcount1 = frmMain.List1.ListCount
    
For Y = 0 To fcount1 - 1
    
    BITE1 = sfiletype(Y)
    Call convDecToBin(BITE1)
    tstring = strBin
    Call GETFILETYPE(tstring)
    
    If FTYPE = "PRG" Then
       numPRG = numPRG + 1
    End If
    
    If FTYPE = "SEQ" Then
       numSEQ = numSEQ + 1
    End If
    
    If FTYPE = "DEL" Then
       numDEL = numDEL + 1
    End If
    
    If FTYPE = "REL" Then
       numREL = numREL + 1
    End If
    
    If FTYPE = "USR" Then
       numUSR = numUSR + 1
    End If
    
 '   If FTYPE = "SCR" Then
  '     numSCR = numSCR + 1
   ' End If
    
    If FTYPE = "???" Then
       numUnknown = numUnknown + 1
    End If
    
    If sfiletype(Y) = 0 Then
       numSCR = numSCR + 1
       numDEL = numDEL - 1
    End If
    
    
Next Y
    
    ' fix for proper english when D64 only contains one file
    fstring = " files "
    If fcount1 = 1 Then
       fstring = " file "
    End If
    
    fileCountString = "D64 contains " & fcount1 & fstring & "(" & numPRG & " PRG"
    
    
    If numSEQ > 0 Then
       fileCountString = fileCountString & "," & numSEQ & " SEQ"
    End If

    If numDEL > 0 Then
       fileCountString = fileCountString & "," & numDEL & " DEL"
    End If

    If numREL > 0 Then
       fileCountString = fileCountString & "," & numREL & " REL"
    End If
    
    If numUSR > 0 Then
       fileCountString = fileCountString & "," & numUSR & " USR"
    End If
    
    If numSCR > 0 Then
       fileCountString = fileCountString & "," & numSCR & " SCR"
    End If
    
    If numUnknown > 0 Then
       fileCountString = fileCountString & "," & numUnknown & " ???"
    End If
    
    fileCountString = fileCountString & ")"
    
    frmMain.lblFileCount.Caption = fileCountString
End Sub
 
Sub reloadSEQfile()
    frmMain.mnuViewFile_Click

End Sub

 
 Function convBinToLong(strbin2 As String) As Long
   Dim i As Long
   ltemp = 0
   For i = Len(strbin2) To 1 Step -1
     If Mid(strbin2, i, 1) = "1" Then
       ltemp = ltemp + 2 ^ (Len(strbin2) - i)
     End If
   Next i
 End Function

Function ReverseString(sString As String) As String
        Dim sNewString As String
        Dim iCount As Integer
   
        sNewString = ""
        
        For iCount = Len(sString) To 1 Step -1
          sNewString = sNewString & Mid(sString, iCount, 1)
        Next iCount
   
        ReverseString = sNewString
End Function

' Code from VBWORLD.COM to create a temp file
' http://www.vbworld.com/files/tip477.html

Function GetTemporaryFilename(Optional Prefix _
   As String = "") As String

   On Error GoTo TempNameErr

   Dim lngReturnVal As Long

   Dim strTempPath As String * 255
   Dim strTempFilename As String * 255

   lngReturnVal = GetTempPath(254, strTempPath)
   lngReturnVal = GetTempFilename(strTempPath _
   & "\", Prefix, 0, strTempFilename)

   GetTemporaryFilename = strTempFilename

   Exit Function

TempNameErr:

   'Enter any error handling here.
   MsgBox "Cannot retrieve temporary filename - " & _
   Err.Description

   End Function

               
' END CODE FROM VBWORLD.COM
               

' Code to see if a file exists already in the filesystem
' http://216.26.168.92/vbsquare/files/tip360.html

Function FileExists(strPath As String) As Integer
                 Dim lngRetVal As Long
                ' On Error Resume Next
                 lngRetVal = Len(Dir$(strPath))
                 If Err Or lngRetVal = 0 Then
                 FileExists = False
                 Else
                 FileExists = True
                 End If
End Function


' End code from VBSQUARE
