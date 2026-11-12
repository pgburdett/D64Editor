VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Begin VB.Form frmMain 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "D64 Editor 0.035  http://www.d64editor.com"
   ClientHeight    =   6285
   ClientLeft      =   150
   ClientTop       =   795
   ClientWidth     =   6990
   Icon            =   "frmMain.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   6285
   ScaleWidth      =   6990
   StartUpPosition =   3  'Windows Default
   Begin MSComDlg.CommonDialog dialogDirectoryExport 
      Left            =   7920
      Top             =   480
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.TextBox txtDirOutput 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   3615
      Left            =   9840
      MultiLine       =   -1  'True
      TabIndex        =   28
      Top             =   960
      Width           =   3615
   End
   Begin MSComDlg.CommonDialog dialog4 
      Left            =   6840
      Top             =   480
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.CommandButton cmdFileChain 
      Height          =   360
      Left            =   1800
      Picture         =   "frmMain.frx":030A
      Style           =   1  'Graphical
      TabIndex        =   9
      ToolTipText     =   "View File Chain"
      Top             =   510
      Width           =   360
   End
   Begin VB.CommandButton cmdSeqView 
      Height          =   360
      Left            =   1440
      Picture         =   "frmMain.frx":0374
      Style           =   1  'Graphical
      TabIndex        =   8
      ToolTipText     =   "View a SEQ File"
      Top             =   510
      Width           =   360
   End
   Begin VB.CommandButton cmdBAM 
      Height          =   360
      Left            =   1080
      Picture         =   "frmMain.frx":03F7
      Style           =   1  'Graphical
      TabIndex        =   7
      ToolTipText     =   "View the BAM"
      Top             =   510
      Width           =   360
   End
   Begin VB.CommandButton cmdSeperatorBuilder 
      Height          =   360
      Left            =   1800
      Picture         =   "frmMain.frx":04FE
      Style           =   1  'Graphical
      TabIndex        =   4
      ToolTipText     =   "Add Seperator Line"
      Top             =   150
      Width           =   360
   End
   Begin VB.CommandButton cmdScratch 
      Height          =   360
      Left            =   360
      Picture         =   "frmMain.frx":0548
      Style           =   1  'Graphical
      TabIndex        =   5
      ToolTipText     =   "Scratch File"
      Top             =   510
      Width           =   360
   End
   Begin VB.CommandButton cmdWipe 
      Height          =   360
      Left            =   720
      Picture         =   "frmMain.frx":05A2
      Style           =   1  'Graphical
      TabIndex        =   6
      ToolTipText     =   "Wipe File"
      Top             =   510
      Width           =   360
   End
   Begin VB.CommandButton cmdSave 
      Height          =   360
      Left            =   720
      Picture         =   "frmMain.frx":0602
      Style           =   1  'Graphical
      TabIndex        =   1
      ToolTipText     =   "Save D64"
      Top             =   150
      Width           =   360
   End
   Begin VB.CommandButton cmdOpen 
      Height          =   360
      Left            =   360
      Picture         =   "frmMain.frx":0682
      Style           =   1  'Graphical
      TabIndex        =   0
      ToolTipText     =   "Open File"
      Top             =   150
      Width           =   360
   End
   Begin VB.ListBox List2 
      Height          =   2010
      Left            =   6600
      TabIndex        =   25
      TabStop         =   0   'False
      Top             =   2160
      Visible         =   0   'False
      Width           =   2295
   End
   Begin MSComDlg.CommonDialog dialog3 
      Left            =   6840
      Top             =   960
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.TextBox txtDiskID 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FF0000&
      Height          =   270
      Left            =   5040
      MaxLength       =   5
      TabIndex        =   11
      Top             =   360
      Width           =   735
   End
   Begin VB.TextBox txtDiskName 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FF0000&
      Height          =   270
      Left            =   2760
      MaxLength       =   16
      TabIndex        =   10
      Top             =   360
      Width           =   2055
   End
   Begin VB.CommandButton cmdSortDown 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   360
      Left            =   1440
      Picture         =   "frmMain.frx":0703
      Style           =   1  'Graphical
      TabIndex        =   3
      ToolTipText     =   "Move File Down"
      Top             =   150
      Width           =   345
   End
   Begin VB.CommandButton cmdSortUp 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   360
      Left            =   1080
      Picture         =   "frmMain.frx":076C
      Style           =   1  'Graphical
      TabIndex        =   2
      ToolTipText     =   "Move File Up"
      Top             =   150
      Width           =   360
   End
   Begin MSComDlg.CommonDialog dialogopen 
      Left            =   6840
      Top             =   1440
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.ListBox List1 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FF0000&
      Height          =   3900
      Left            =   120
      MultiSelect     =   2  'Extended
      OLEDropMode     =   1  'Manual
      TabIndex        =   12
      Top             =   1320
      Width           =   6735
   End
   Begin VB.Label lblFileCount 
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FF0000&
      Height          =   255
      Left            =   120
      TabIndex        =   27
      Top             =   5520
      Width           =   6735
   End
   Begin VB.Label lblDiskType 
      Alignment       =   1  'Right Justify
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   2760
      TabIndex        =   26
      Top             =   5330
      Width           =   4095
   End
   Begin VB.Label Label4 
      Caption         =   "Label4"
      Height          =   495
      Left            =   6840
      TabIndex        =   24
      Top             =   1920
      Visible         =   0   'False
      Width           =   1455
   End
   Begin VB.Label lblBlocksFree 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FF0000&
      Height          =   255
      Left            =   120
      TabIndex        =   23
      Top             =   5330
      Width           =   1935
   End
   Begin VB.Label lblD64File 
      Caption         =   "NO D64 FILE LOADED"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   495
      Left            =   120
      TabIndex        =   22
      Top             =   5760
      Width           =   6735
   End
   Begin VB.Label Label10 
      Caption         =   "DISK ID"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   5025
      TabIndex        =   21
      Top             =   120
      Width           =   855
   End
   Begin VB.Label Label9 
      Caption         =   "DISK NAME"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   3240
      TabIndex        =   20
      Top             =   120
      Width           =   1335
   End
   Begin VB.Label Label8 
      Caption         =   "BLOCKS"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   4800
      TabIndex        =   19
      Top             =   1080
      Width           =   735
   End
   Begin VB.Label Label7 
      Caption         =   "STARTING "
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   135
      Left            =   3600
      TabIndex        =   18
      Top             =   840
      Width           =   975
   End
   Begin VB.Label Label6 
      Caption         =   "STARTING "
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   135
      Left            =   2520
      TabIndex        =   17
      Top             =   840
      Width           =   975
   End
   Begin VB.Label Label5 
      Caption         =   "TYPE"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   135
      Left            =   5880
      TabIndex        =   16
      Top             =   1080
      Width           =   615
   End
   Begin VB.Label Label3 
      Caption         =   "SECTOR"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   3720
      TabIndex        =   15
      Top             =   1080
      Width           =   855
   End
   Begin VB.Label Label2 
      Caption         =   "FILENAME"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   720
      TabIndex        =   14
      Top             =   1080
      Width           =   1095
   End
   Begin VB.Label Label1 
      Caption         =   "TRACK"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   2760
      TabIndex        =   13
      Top             =   1080
      Width           =   735
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuLoadFile 
         Caption         =   "&Load D64 File"
         Shortcut        =   ^L
      End
      Begin VB.Menu mnuSaveFile 
         Caption         =   "&Save D64 File"
         Shortcut        =   ^S
      End
      Begin VB.Menu mnuTrimFile 
         Caption         =   "&Trim File"
      End
      Begin VB.Menu mnuRevert 
         Caption         =   "&Revert "
         Shortcut        =   ^R
      End
      Begin VB.Menu seperator1 
         Caption         =   "-"
      End
      Begin VB.Menu mnuCreateBlankD64 
         Caption         =   "&Create Blank D64"
         Shortcut        =   ^N
      End
      Begin VB.Menu mnuConfig 
         Caption         =   "Configuration &Menu"
         Shortcut        =   ^M
      End
      Begin VB.Menu mnuQuit 
         Caption         =   "&Quit"
         Shortcut        =   ^Q
      End
   End
   Begin VB.Menu mnuEdit 
      Caption         =   "&Edit"
      Begin VB.Menu mnuFileExport 
         Caption         =   "File &Export"
         Shortcut        =   ^E
      End
      Begin VB.Menu mnuFileImport 
         Caption         =   "File &Import"
         Shortcut        =   ^I
      End
      Begin VB.Menu mnuCopyFilesToMemory 
         Caption         =   "Copy Files to &Memory"
         Shortcut        =   ^C
      End
      Begin VB.Menu mnuExportDirectoryToText 
         Caption         =   "Export &DIR to TXT"
         Shortcut        =   ^D
      End
      Begin VB.Menu seperator4 
         Caption         =   "-"
      End
      Begin VB.Menu mnuSelectAllFiles 
         Caption         =   "Select All Files"
      End
      Begin VB.Menu mnuUnselectAllFiles 
         Caption         =   "UnSelect All Files"
      End
      Begin VB.Menu mnuInvertSelection 
         Caption         =   "Invert Selection"
      End
      Begin VB.Menu seperator 
         Caption         =   "-"
      End
      Begin VB.Menu mnuModifyFileProperties 
         Caption         =   "Modify File &Properties"
         Shortcut        =   ^P
      End
      Begin VB.Menu mnuScratchFile 
         Caption         =   "&Scratch File"
         Shortcut        =   {DEL}
      End
      Begin VB.Menu mnuUnscratchFile 
         Caption         =   "&Unscratch File"
         Shortcut        =   +{DEL}
      End
      Begin VB.Menu mnuWipeFile 
         Caption         =   "&Wipe File"
         Shortcut        =   ^X
      End
      Begin VB.Menu mnuRemoveDirectoryEntry 
         Caption         =   "&Remove Directory Entry"
      End
      Begin VB.Menu mnuMoveFileToTop 
         Caption         =   "Move File To The Top"
      End
      Begin VB.Menu mnuMoveFileToBottom 
         Caption         =   "Move File To The Bottom"
      End
      Begin VB.Menu sep2 
         Caption         =   "-"
      End
      Begin VB.Menu mnuAddSeperatorLine 
         Caption         =   "&Add Separator Line"
         Shortcut        =   ^A
      End
      Begin VB.Menu mnuCopySelectedintoSeparatorBuilder 
         Caption         =   "Copy Selected into Separator Builder"
      End
      Begin VB.Menu mnuEditBAM 
         Caption         =   "Edit &BAM"
         Shortcut        =   ^B
      End
      Begin VB.Menu mnuBlockExport 
         Caption         =   "Block Export"
      End
   End
   Begin VB.Menu mnuView 
      Caption         =   "&View"
      Begin VB.Menu mnuViewFileChain 
         Caption         =   "&View File Chain"
         Shortcut        =   ^F
      End
      Begin VB.Menu mnuViewFile 
         Caption         =   "&SEQ File Viewer"
         Shortcut        =   ^V
      End
      Begin VB.Menu mnuViewErrorBlocks 
         Caption         =   "View &Error Blocks"
         Enabled         =   0   'False
         Shortcut        =   ^Z
      End
   End
   Begin VB.Menu mnuHelp 
      Caption         =   "&Help"
      Begin VB.Menu mnuVersion 
         Caption         =   "&Version"
         Enabled         =   0   'False
         Visible         =   0   'False
      End
      Begin VB.Menu mnuAbout 
         Caption         =   "About the D64 Editor"
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
' D64 editor
' Hope you're hungry, because it's spaghetti time!
' Started 11/20/99 by Forrest Mook
' You may use any of this source code in your own programs providing you
' follow the GNU License
' Enjoy!
'
' frmMain.frm
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


Private Sub List1_OLEDragDrop(Data As DataObject, Effect As Long, Button As Integer, Shift As Integer, x As Single, Y As Single)
Dim fname As Variant, txt As String
For Each fname In Data.Files
    frmMain.ProcDroppedFile (fname)   '  added 2/27/2014
 ''txt = txt + fname   ' commented out 2/27/2014
Next fname

''frmMain.ProcDroppedFile (txt)  ' commented out 2/27/2014
End Sub


Private Sub cmdBAM_Click()
mnuEditBAM_Click
End Sub

Private Sub cmdFileChain_Click()
mnuViewFileChain_Click
End Sub

Private Sub cmdOpen_Click()
mnuLoadFile_Click
End Sub

Private Sub cmdSave_Click()
If frmMain.List1.Enabled = True Then
mnuSaveFile_Click
End If
End Sub

Private Sub cmdScratch_Click()
mnuScratchFile_Click
End Sub

Private Sub cmdSeperator_Click()
mnuAddSeperatorLine_Click
End Sub

Private Sub cmdSeperatorBuilder_Click()
   If dialogopen.Filename = "" Then
     Exit Sub
   End If
  
  frmFilenameBuilder.Show
  frmFilenameBuilder.mnuSeparators.Visible = True
'  frmFilenameBuilder.Height = 4260
End Sub

Private Sub cmdSeqView_Click()
mnuViewFile_Click
End Sub

Private Sub cmdWipe_Click()
mnuWipeFile_Click
End Sub





Private Sub Form_Load()
  
  On Error GoTo errorhandler
  Call GetWinDir
  
  Open frmMain.Label4.Caption For Input As #1
    Input #1, lastDirPath
    Input #1, frmBAMTOP
    Input #1, garbage
    Input #1, frmBAMLEFT
    Input #1, garbage
    Input #1, frmBlockViewerTOP
    Input #1, garbage
    Input #1, frmBlockViewerLEFT
    Input #1, garbage
    Input #1, frmFilePropertiesTOP
    Input #1, garbage
    Input #1, frmFilePropertiesLEFT
    Input #1, garbage
    Input #1, frmMainTOP
    Input #1, garbage
    Input #1, frmMainLEFT
    Input #1, garbage
    Input #1, frmSectorDumpTOP
    Input #1, garbage
    Input #1, frmSectorDumpLEFT
    Input #1, garbage
    Input #1, frmCrossLinksLEFT
    Input #1, garbage
    Input #1, frmCrossLinksTOP
    Input #1, garbage
    Input #1, frmFileViewerTOP
    Input #1, garbage
    Input #1, frmFileViewerLEFT
    Input #1, garbage
    Input #1, FileExportPath
    Input #1, warningsDisabled
    Input #1, garbage
    Input #1, safeClean
    Input #1, garbage
    Input #1, disableCrosslinkCheck
    Input #1, garbage
    Input #1, frmConfigTOP
    Input #1, garbage
    Input #1, frmConfigLEFT
    Input #1, garbage
    Input #1, frmMemoryTOP
    Input #1, garbage
    Input #1, frmMemoryLEFT
    Input #1, garbage
    Input #1, FileViewerColumnMode
    Input #1, garbage
    Input #1, convertToASCII
    Input #1, garbage
    Input #1, seperatorLineText
    Input #1, BlockViewerASCIIMode
    Input #1, garbage
    Input #1, frmFileNameBuilderTOP
    Input #1, garbage
    Input #1, frmFileNameBuilderLEFT
    Input #1, garbage
    Input #1, frmMainCreateNewD64TOP
    Input #1, garbage
    Input #1, frmMainCreateNewD64LEFT
    Input #1, garbage
    Input #1, SelectNextFileUponSaveVar
    Input #1, garbage
    Input #1, treatTrack18FilesAsEmpty
    Input #1, garbage
    Input #1, InterLeave
    Input #1, garbage
    Input #1, UserSpecifiedFirstTrack
    Input #1, garbage
    Input #1, UserSpecifiedFirstSector
  Close #1
     frmMain.Top = frmMainTOP
     frmMain.Left = frmMainLEFT
  
     Do Until Right(lastDirPath, 1) = "\"
       lastDirPath = Left(lastDirPath, Len(lastDirPath) - 1)
     Loop


    
Exit Sub
errorhandler:
   
  Close #1
  
  Open frmMain.Label4.Caption For Output As #1
    Print #1, ".\"
    Print #1, "0 frmBAMTOP"
    Print #1, "0 frmBAMLEFT"
    Print #1, "0 frmBlockViewerTOP"
    Print #1, "0 frmBlockViewerLEFT"
    Print #1, "0 frmFilePropertiesTOP"
    Print #1, "0 frmFilePropertiesLEFT"
    Print #1, "0 frmMainTOP"
    Print #1, "0 frmMainLEFT"
    Print #1, "0 frmSectorDumpTOP"
    Print #1, "0 frmSectorDumpLEFT"
    Print #1, "0 frmCrossLinksLEFT"
    Print #1, "0 frmCrossLinksTOP"
    Print #1, "0 frmFileViewerTOP"
    Print #1, "0 frmFileViewerLEFT"
    Print #1, "C:"
    Print #1, "0 warningsDisabled"
    Print #1, "0 safeClean"
    Print #1, "0 disableCrosslinkCheck"
    Print #1, "0 frmConfigTOP"
    Print #1, "0 frmConfigLEFT"
    Print #1, "0 frmMemoryTOP"
    Print #1, "0 frmMemoryLEFT"
    Print #1, "0 FileViewerColumnMode"
    Print #1, "================"
    Print #1, "0 BlockViewerASCIIMode"
    Print #1, "0 frmFilenameBuilderTOP"
    Print #1, "0 frmFilenameBuilderLEFT"
    Print #1, "0 frmMainCreateNewD64TOP"
    Print #1, "0 frmMainCreateNewD64LEFT"
    Print #1, "0 SelectNextFileUponSaveVar"
    Print #1, "0 TreatTrack18FilesAsEmpty"
    Print #1, "10 InterLeave"
    Print #1, "17 ImportFirstTrack"
    Print #1, "0 ImportFirstSector"
    InterLeave = 10
    UserSpecifiedFirstTrack = 17
    UserSpecifiedFirstSector = 0
  Close #1
  seperatorLineText = "================"
End Sub


Private Sub Form_Unload(Cancel As Integer)

   If D64Modified = True Then
     If MsgBox("Changes to the D64 in memory have not been saved yet." & vbCrLf & "Are you sure you want to exit the program?", vbYesNo + vbDefaultButton2, "Changes not saved") = vbNo Then
            Cancel = 1
            Exit Sub
     End If
   End If
  
  
  frmMainTOP = frmMain.Top
  frmMainLEFT = frmMain.Left
  
  Call saveWindowLocations
  
  ' Clean up leftover temp files
  If TempFileName1 <> "" Then
     Kill TempFileName1
  End If
  
  End
End Sub



Private Sub List1_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       List1_dblClick
    End If
    
End Sub

Private Sub List1_MouseDown(Button As Integer, Shift As Integer, x As Single, Y As Single)
 

 
 If Button = 2 Then
      Dim lParam As Long
        Dim curritem As Long
        Dim r As Long
        Dim pt As POINTAPI
        
        Call GetCursorPos(pt)
        Call ScreenToClient(List1.hWnd, pt)
        
        lParam = MAKELPARAM(pt.x, pt.Y)
                                
        r = SendMessage(List1.hWnd, LB_ITEMFROMPOINT, 0&, ByVal lParam)
        
        If r > -1 Then
           curritem = LoWord(r)
           
          'this line selects the item (if desired)
          'with the right-click
           If List1.ListCount <> 0 Then
              List1.Selected(curritem) = True
           End If
        End If
   
   frmMain.PopupMenu mnuEdit, 2
 End If
 
End Sub

Private Sub mnuAbout_Click()
    frmAbout.Show
End Sub

Private Sub mnuAddSeperatorLine_Click()
   If dialogopen.Filename = "" Then
     Exit Sub
   End If
  
  frmFilenameBuilder.Show
  frmFilenameBuilder.mnuSeparators.Visible = True
  frmFilenameBuilder.mnuSaveSeparatorstoFile.Visible = True
 ' frmFilenameBuilder.Height = 4360
 
End Sub



Private Sub mnuBlockExport_Click()

If dialogopen.Filename = "" Then
      MsgBox "Please load a file first.", vbExclamation, "No file loaded"
  Exit Sub
End If
    
    frmSectorDump.optDecimal = True

   frmMain.Enabled = False
   frmSectorDump.Show
End Sub

Private Sub mnuConfig_Click()
  
   frmConfig.Show
   frmConfig.Left = frmConfigLEFT
   frmConfig.Top = frmConfigTOP
   
   If disableCrosslinkCheck = 1 Then
     frmConfig.checkDisableCrosslinkCheck.Value = Checked
   Else
     frmConfig.checkDisableCrosslinkCheck.Value = Unchecked
   End If
   
   If safeClean = 1 Then
     frmConfig.checkEnableSafeDiskClean.Value = Checked
   Else
     frmConfig.checkEnableSafeDiskClean.Value = Unchecked
   End If
   
   If warningsDisabled = 1 Then
     frmConfig.checkWarningMessages.Value = Checked
   Else
     frmConfig.checkWarningMessages.Value = Unchecked
   End If
   
   If convertToASCII = 1 Then
     frmConfig.checkConvertToASCII.Value = Checked
   Else
     frmConfig.checkConvertToASCII.Value = Unchecked
   End If
   
   If treatTrack18FilesAsEmpty = 1 Then
     frmConfig.checkTreatTrack18FilesAsEmpty.Value = Checked
   Else
     frmConfig.checkTreatTrack18FilesAsEmpty.Value = Unchecked
   End If
   
   frmConfig.txtSeperatorLine.Text = seperatorLineText
   
   If InterLeave = 0 Or InterLeave > 15 Then
        frmConfig.txtInterleave.Text = 10
        InterLeave = 10
   Else
        frmConfig.txtInterleave.Text = InterLeave
   End If

    If UserSpecifiedFirstTrack = 0 Then
        UserSpecifiedFirstTrack = 17
    End If

    frmConfig.txtImportStartingTrack.Text = UserSpecifiedFirstTrack
    frmConfig.txtImportStartingSector.Text = UserSpecifiedFirstSector

    
End Sub

Private Sub mnuCopyFilesToMemory_Click()
   
   Load frmMemory
   frmMemory.Top = frmMemoryTOP
   frmMemory.Left = frmMemoryLEFT

   For i = 0 To List1.ListCount - 1
      
      If (frmMemory.listMemory.ListCount) + 1 = 201 Then
          MsgBox "There are 200 files already in memory.", vbCritical, "Out of Memory"
          Exit For
      End If
      
      If List1.Selected(i) = True Then
          CopyFilesToMemory (i)
      End If
  
   Next i
   
   frmMemory.Show


End Sub

Private Sub CopyFilesToMemory(listvar As Integer)

   Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim g As Long
   Dim BBTT(0 To 257) As Byte
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim varString As String
   Dim tempInt As Integer
   Dim extraspaces As String
   Dim totBlocks As Integer
   
   bitespersector = 256
      
      frmMain.MousePointer = 11



    
    

       Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)
      
      ' If file starts at 18/0 or 18/1 then treat it as a seperator line (only if option enabled)
    If treatTrack18FilesAsEmpty = 1 Then
        If sstartingtrack(listvar) = 18 Then
             If sstartingsector(listvar) = 0 Or sstartingsector(listvar) = 1 Then
                 nxtTrack = 0
                 nxtSector = 0
             End If
        End If
    End If
      
      
  '  BEGIN FIX FOR SEPARATOR LINES 12/16/03 - FM
      If nxtTrack = 0 And nxtSector = 0 Then
         memorydisplayFilename((frmMemory.listMemory.ListCount) + 1) = sfilename(listvar)
         memoryFilename((frmMemory.listMemory.ListCount) + 1) = sfilename(listvar)
         memoryFileType((frmMemory.listMemory.ListCount) + 1) = sfiletype(listvar)
         memoryCache((frmMemory.listMemory.ListCount) + 1) = ""
         memoryFileLength((frmMemory.listMemory.ListCount) + 1) = 0
     For u = 1 To 16
        If Mid(memorydisplayFilename((frmMemory.listMemory.ListCount) + 1), u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If

        memorydisplayFilename((frmMemory.listMemory.ListCount) + 1) = Left(memorydisplayFilename((frmMemory.listMemory.ListCount) + 1), hitanA0 - 1)

        For w = Len(memorydisplayFilename((frmMemory.listMemory.ListCount) + 1)) To 16
           extraspaces = extraspaces & " "
        Next w
           
          frmMemory.listMemory.AddItem Chr$(34) & memorydisplayFilename((frmMemory.listMemory.ListCount) + 1) & Chr$(34) & extraspaces & totBlocks & " BLOCKS"
         
         Close #1
         Exit Sub
      End If
      ' END FIX FOR SEPARATOR LINES  12/16/03 - FM

      
      For p = 1 To 700
      
      If nxtTrack = 0 Then
       
        If nxtSector > 0 Then
          totBlocks = totBlocks + 1
          varString = String(nxtSector - 1, " ")
          Get #1, Val(BITE2START + 2), varString
          memoryCache((frmMemory.listMemory.ListCount) + 1) = memoryCache((frmMemory.listMemory.ListCount) + 1) & varString
          memoryFilename((frmMemory.listMemory.ListCount) + 1) = sfilename(listvar)
          memoryFileType((frmMemory.listMemory.ListCount) + 1) = sfiletype(listvar)
          memoryFileLength((frmMemory.listMemory.ListCount) + 1) = Len(memoryCache((frmMemory.listMemory.ListCount) + 1))

   memorydisplayFilename((frmMemory.listMemory.ListCount) + 1) = sfilename(listvar)
     For u = 1 To 16
        If Mid(memorydisplayFilename((frmMemory.listMemory.ListCount) + 1), u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If

        memorydisplayFilename((frmMemory.listMemory.ListCount) + 1) = Left(memorydisplayFilename((frmMemory.listMemory.ListCount) + 1), hitanA0 - 1)

        For w = Len(memorydisplayFilename((frmMemory.listMemory.ListCount) + 1)) To 16
           extraspaces = extraspaces & " "
        Next w
           
          frmMemory.listMemory.AddItem Chr$(34) & memorydisplayFilename((frmMemory.listMemory.ListCount) + 1) & Chr$(34) & extraspaces & totBlocks & " BLOCKS"
        End If
        Close #1
         frmMain.MousePointer = 0
         
         Exit Sub
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

    BITE2START = BITE2START + 1
      
    
         Get #1, Val(BITE2START), trackbite
         Get #1, Val(BITE2START + 1), sectorbite
         
         nxtTrack = trackbite
         nxtSector = sectorbite
   
                 If nxtTrack <> 0 Then
                 varString = String(254, " ")
                 Get #1, Val(BITE2START + 2), varString
          memoryCache((frmMemory.listMemory.ListCount) + 1) = memoryCache((frmMemory.listMemory.ListCount) + 1) & varString
                 totBlocks = totBlocks + 1
                 End If
   
           

 Next p
      Close #1
  '    Close #2
      frmMain.MousePointer = 0


End Sub

Private Sub mnuCopySelectedintoSeparatorBuilder_Click()
    
   Dim displayfilename As String
     
   If dialogopen.Filename = "" Then
     Exit Sub
   End If
  
  
  frmFilenameBuilder.Show
  frmFilenameBuilder.mnuSeparators.Visible = True
  frmFilenameBuilder.Height = 4360
  
   For i = 0 To List1.ListCount - 1
     If List1.Selected(i) = True Then
       displayfilename = sfilename(i)

     For u = 1 To 16
        If Mid(displayfilename, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If

        displayfilename = Left(displayfilename, hitanA0 - 1)

       frmFilenameBuilder.listSeparators.AddItem displayfilename
     End If
   Next i
  
End Sub

Private Sub mnuCreateBlankD64_Click()
 ' We can let user specify Default Disk Name and Default Disk ID/DOS
 frmMainCreateNewD64.Show
 frmMainCreateNewD64.opt35Track.Value = True
 frmMain.Enabled = False
End Sub


Function CreateBlankD64(diskName As String, diskID As String, c128BootSector As Boolean)
 Dim bitespersector As Long
 Dim z As Long
 Dim dirOffset As Long
 Dim bamString As String
 Dim F As Long
 Dim x As Integer

 Dim bootSectorCode As String

 z = 91393 ' first byte of 18/0
 bitespersector = 256
 dialog4.Filename = ""
 dialog4.Flags = &H2 Or &H4 Or &H800 ' Check to see if the file already exists
 dialog4.DefaultExt = "d64"
 dialog4.Filter = "C64 Disk Image (*.d64)|*.d64|All Files (*.*)|*.*"
 dialog4.ShowSave

 If dialog4.Filename = "" Then Exit Function

 Open dialog4.Filename For Binary As #1
  
    ' start with a normal D64 filesize
    varString$ = String$(174848, 0)
    
    If frmMainCreateNewD64.opt35TrackPlusErrors.Value = True Then
         varString$ = String$(175531, 0)
    End If
      
    If frmMainCreateNewD64.opt40Track.Value = True Then
         varString$ = String$(196608, 0)
    End If
    
    If frmMainCreateNewD64.opt40TrackPlusErrors.Value = True Then
         varString$ = String$(197376, 0)
    End If

  ' first fill the entire disk with zeros, starting at byte 1
    Put #1, 1, varString$

            ' write out the error bytes, filled with no error
      If frmMainCreateNewD64.opt35TrackPlusErrors.Value = True Then
           varString$ = String$(683, 1)
           Put #1, 174849, varString$
      End If
             ' write out the error bytes, filled with no error
      If frmMainCreateNewD64.opt40TrackPlusErrors.Value = True Then
           varString$ = String$(768, 1)
           Put #1, 196609, varString$
      End If
      
      ' probably not needed, more as a failsafe
      varString$ = String$(174848, 0)
  
  ' Set location of first dir sector, and dos version type
  Put #1, z, CLng(&H12)
  Put #1, z + 1, CLng(&H1)
  Put #1, z + 2, CLng(&H41)

  ' Set BAM to 664 blocks free (BAM Entries)
  ' Tracks 1 - 17 (21 Sectors per Track)
  For dirOffset = z + 4 To z + &H47 Step 4
    If (dirOffset = 91397 And frmMainCreateNewD64.chkProtectSector.Value = 1) Then
        ' Allocate BAM to protect T/S 1,0
        Put #1, dirOffset, CLng(&H14)
        Put #1, dirOffset + 1, CLng(&HFE)
        Put #1, dirOffset + 2, CLng(&HFF)
        Put #1, dirOffset + 3, CLng(&H1F)
    Else
        Put #1, dirOffset, CLng(&H15)
        Put #1, dirOffset + 1, CLng(&HFF)
        Put #1, dirOffset + 2, CLng(&HFF)
        Put #1, dirOffset + 3, CLng(&H1F)
    End If
  Next dirOffset

  ' Track 18? (Directory)
  Put #1, z + &H48, CLng(&H11)
  Put #1, z + &H49, CLng(&HFC)
  Put #1, z + &H4A, CLng(&HFF)
  Put #1, z + &H4B, CLng(&H7)

  ' Tracks 19 - 24 (19 Sectors per Track)
  For dirOffset = z + &H4C To z + &H63 Step 4
   Put #1, dirOffset, CLng(&H13)
   Put #1, dirOffset + 1, CLng(&HFF)
   Put #1, dirOffset + 2, CLng(&HFF)
   Put #1, dirOffset + 3, CLng(&H7)
  Next dirOffset

  ' Tracks 25 - 30 (18 Sectors per Track)
  For dirOffset = z + &H64 To z + &H7B Step 4
   Put #1, dirOffset, CLng(&H12)
   Put #1, dirOffset + 1, CLng(&HFF)
   Put #1, dirOffset + 2, CLng(&HFF)
   Put #1, dirOffset + 3, CLng(&H3)
  Next dirOffset

  ' Tracks 31 - 35 (17 Sectors per Track)
  For dirOffset = z + &H7C To z + &H8F Step 4
   Put #1, dirOffset, CLng(&H11)
   Put #1, dirOffset + 1, CLng(&HFF)
   Put #1, dirOffset + 2, CLng(&HFF)
   Put #1, dirOffset + 3, CLng(&H1)
  Next dirOffset

  ' Disk Name
   x = 0
  For dirOffset = z + &H90 To z + &H9F
   x = x + 1
   If (x > Len(diskName)) Then
    Put #1, dirOffset, CLng(&HA0)
   Else
     Put #1, dirOffset, CLng("&H" & Hex(Asc(Mid$(diskName, x, 1))))
   End If
  Next dirOffset
  
  ' Padding
  Put #1, z + &HA0, CLng(&HA0):  ' (PAD)
  Put #1, z + &HA1, CLng(&HA0):  ' (PAD)
  
  x = 0
  For dirOffset = z + &HA2 To z + &HA6
   x = x + 1
   If (x > Len(diskID)) Then
    Put #1, dirOffset, CLng(&HA0)
   Else
     Put #1, dirOffset, CLng("&H" & Hex(Asc(Mid$(diskID, x, 1))))
   End If
  Next dirOffset
  
  ' Padding
  For dirOffset = z + &HA7 To z + &HAA
   Put #1, dirOffset, CLng(&HA0)
  Next dirOffset

  ' prepare track 18/1
  Put #1, z + 257, CLng(&HFF)
   
  Close #1
   
 If (c128BootSector = True) Then
    
    Open dialog4.Filename For Binary As #1
    
    ' Patch C128 Auto-boot Sector
    Dim tFN As String
    tFN = frmMainCreateNewD64.txtAutobootFilename.Text

    Put #1, 1, Chr$(CDec("&H43")) ' C
    Put #1, , Chr$(CDec("&H42")) ' B
    Put #1, , Chr$(CDec("&H4d")) ' M
    Put #1, , Chr$(CDec("&H0")) '
    Put #1, , Chr$(CDec("&H0")) '
    Put #1, , Chr$(CDec("&H0")) '
    Put #1, , Chr$(CDec("&H0")) '

    ' Boot Message
    For x = 1 To Len(tFN)
        Put #1, , Chr$(Asc(Mid$(tFN, x, 1)))
    Next x

    Put #1, , Chr$(CDec("&H0")) '
    Put #1, , Chr$(CDec("&H0")) '
    Put #1, , Chr$(CDec("&H20")) '
    Put #1, , Chr$(CDec("&He4")) ' ä
    Put #1, , Chr$(CDec("&Hff")) ' ÿ
    Put #1, , Chr$(CDec("&Hc9")) ' É
    Put #1, , Chr$(CDec("&H3")) ' 
    Put #1, , Chr$(CDec("&Hd0")) ' Ð
    Put #1, , Chr$(CDec("&H1")) ' 
    Put #1, , Chr$(CDec("&H60")) ' `
    Put #1, , Chr$(CDec("&Ha2")) ' ¢
    
    ' Filename Length (23 + length)
    '    Put #1, , CLng(&H18) ' 
    Put #1, , Chr$(23 + Len(tFN))

    Put #1, , Chr$(CDec("&Ha0")) '  
    Put #1, , Chr$(CDec("&H0b")) ' 
    Put #1, , Chr$(CDec("&H4c")) ' L
    Put #1, , Chr$(CDec("&Ha5")) ' ¥
    Put #1, , Chr$(CDec("&Haf")) ' ¯
    Put #1, , Chr$(CDec("&H55")) ' U
    Put #1, , Chr$(CDec("&H3d")) ' =
    Put #1, , Chr$(CDec("&H50")) ' P
    Put #1, , Chr$(CDec("&H45")) ' E
    Put #1, , Chr$(CDec("&H45")) ' E
    Put #1, , Chr$(CDec("&H4b")) ' K
    Put #1, , Chr$(CDec("&H28")) ' (
    Put #1, , Chr$(CDec("&H31")) ' 1
    Put #1, , Chr$(CDec("&H38")) ' 8
    Put #1, , Chr$(CDec("&H36")) ' 6
    Put #1, , Chr$(CDec("&H29")) ' )
    Put #1, , Chr$(CDec("&H3a")) ' :
    Put #1, , Chr$(CDec("&H52")) ' R
    Put #1, , Chr$(CDec("&H55")) ' U
    Put #1, , Chr$(CDec("&H4e")) ' N
    Put #1, , Chr$(CDec("&H22")) ' "
    
    '   Boot Filename
    '    Put #1, , CLng(&H31) ' 1
    For x = 1 To Len(tFN)
        Put #1, , Chr$(Asc(Mid$(tFN, x, 1)))
    Next x

    Put #1, , Chr$(CDec("&H22")) ' "
    Put #1, , Chr$(CDec("&H2c")) ' ,
    Put #1, , Chr$(CDec("&H55")) ' U
    Put #1, , Chr$(CDec("&H28")) ' (
    Put #1, , Chr$(CDec("&H55")) ' U
    Put #1, , Chr$(CDec("&H29")) ' )
    Put #1, , Chr$(CDec("&H0")) '
    Put #1, , Chr$(CDec("&H0d")) '
    
    Close #1

    ' If sector is protected, put in dir entry to show it is autoboot
     If (c128BootSector = True) Then
        Open dialog4.Filename For Binary As #1
        
        Put #1, 91651, Chr$(CDec("&HC0"))
        Put #1, , Chr$(CDec("&H0"))
        Put #1, , Chr$(CDec("&H0"))
        Put #1, , Chr$(CDec("&HA2"))
        Put #1, , Chr$(CDec("&HB9"))
        Put #1, , Chr$(CDec("&H43"))
        Put #1, , Chr$(CDec("&H31"))
        Put #1, , Chr$(CDec("&H32"))
        Put #1, , Chr$(CDec("&H38"))
        Put #1, , Chr$(CDec("&H41"))
        Put #1, , Chr$(CDec("&H55"))
        Put #1, , Chr$(CDec("&H54"))
        Put #1, , Chr$(CDec("&H4F"))
        Put #1, , Chr$(CDec("&H42"))
        Put #1, , Chr$(CDec("&H4F"))
        Put #1, , Chr$(CDec("&H4F"))
        Put #1, , Chr$(CDec("&H54"))
        Put #1, , Chr$(CDec("&HB9"))
        Put #1, , Chr$(CDec("&HA2"))
        
        Close #1
    End If
    
    
    MsgBox "Blank D64 with C128 Auto-Boot Sector created.", vbInformation, "File creation complete"
 Else
    MsgBox "Blank D64 created.", vbInformation, "File creation complete"
 End If
   
 ' If no D64 loaded, then load the newly created one
 If dialogopen.Filename = "" Then
  LoadFile (dialog4.Filename)
 End If

End Function

Private Sub mnuEditBAM_Click()
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
   Dim tstring As String
   
If dialogopen.Filename = "" Then
  '    MsgBox "Please load a file first.", vbExclamation, "No file loaded"
  Exit Sub
End If

   For i = 0 To 682
   
      If blocksAllocated(i) = 1 Then
         frmBAM.lblBAM(i) = "*"
      Else
         frmBAM.lblBAM(i) = ""
      End If
   Next i
   
   frmBAM.optDecimal = True
   
   For n = 0 To 20
     frmBAM.SectorDecimal(n).Visible = True
     frmBAM.SectorHex(n).Visible = False
   Next n
   
   frmBAM.TrackBottomDecimal.Visible = True
   frmBAM.trackTopDecimal.Visible = True
   
   frmBAM.lblSector = "S" & vbCrLf & "E" & vbCrLf & "C" & vbCrLf & "T" & vbCrLf & "O" & vbCrLf & "R"
   
   frmMain.Enabled = False
   frmBAM.Show
   
   
   
   
End Sub



Private Sub mnuExportDirectoryToText_Click()
    Dim ExpFileName As String
    Dim YNResponse As Integer
   
    On Error GoTo ErrHandler
    
    If OriginalFileName = "" Then
       Exit Sub
    End If
   
    frmMain.dialogDirectoryExport.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*"
    frmMain.dialogDirectoryExport.Filename = OriginalFileName & ".txt"
    frmMain.dialogDirectoryExport.Flags = &H800 Or &H80000
    frmMain.dialogDirectoryExport.CancelError = True
    frmMain.dialogDirectoryExport.ShowSave

    
    ExpFileName = frmMain.dialogDirectoryExport.Filename
  
    If Dir(ExpFileName) <> "" Then
       YNResponse = MsgBox("That file already exists, overwrite?", vbYesNo, "Overwrite File")
    End If
    
    If YNResponse = vbNo Then
       Exit Sub
    End If
  
    ExportDirectory
    Exit Sub
    
ErrHandler:
    ' the user hit cancel on the save dialog box, so do nothing

End Sub

Private Sub ExportDirectory()

  Dim exportCount As Integer
  Dim exportList As Integer
  Dim ExportFilename As String
  Dim padFileNameCount As Integer
  Dim ExportOutputFilename As String
  Dim TempFileType As String
  Dim tmpstring As String
  Dim tmpDiskName As String
  Dim DiskNameLoopInt As Integer
  Dim d64FileNameLoopInt As Integer
  Dim outputSepLine As String
  Dim BlockSizeOutput As String
  Dim blocksizeLoopInt As Integer
  Dim FileTypeLoopInt As Integer
  
  On Error GoTo ErrHandler
  
   exportList = frmMain.List1.ListCount - 1
   If exportList <= 0 Then Exit Sub
    
    '  blank out the temporary text box on the form
   frmMain.txtDirOutput.Text = ""
    
   frmMain.txtDirOutput.Text = Date & " " & time & vbCrLf
    
    '  grab the full path filename
   frmMain.txtDirOutput.Text = frmMain.txtDirOutput.Text & frmMain.lblD64File.Caption & vbCrLf

      ' build a separator line the same length as the full path filename
   For d64FileNameLoopInt = 1 To Len(frmMain.lblD64File.Caption)
        outputSepLine = outputSepLine & "-"
   Next d64FileNameLoopInt

   frmMain.txtDirOutput.Text = frmMain.txtDirOutput.Text & outputSepLine & vbCrLf

    tmpDiskName = frmMain.txtDiskName.Text
    
     ' pad the diskname to 16 characters
    For DiskNameLoopInt = 0 To 16
       If Len(tmpDiskName) < 16 Then
          tmpDiskName = tmpDiskName & " "
       End If
    Next DiskNameLoopInt
    
    

    
    
    ' output the diskname/ID
    frmMain.txtDirOutput.Text = frmMain.txtDirOutput.Text & "0 " & Chr(34) & tmpDiskName & Chr(34) & " " & frmMain.txtDiskID.Text & vbCrLf
    
    
    
   For exportCount = 0 To exportList
        
        ' obtain the filetype to display
        Call convDecToBin(sfiletype(exportCount))
        tmpstring = strBin
        Call GETFILETYPE(tmpstring)
        sfiletypedisplay = FILECLOSED & FTYPE & FILELOCKED
        
        For FileTypeLoopInt = 0 To 4
            If Len(sfiletypedisplay) < 4 Then
               sfiletypedisplay = sfiletypedisplay & " "
            End If
        Next FileTypeLoopInt
        
            BlockSizeOutput = sfilesizeDisplay(exportCount)
         ' pad the block size to 5 characters
        For blocksizeLoopInt = 0 To 4
            If Len(BlockSizeOutput) < 4 Then
               BlockSizeOutput = BlockSizeOutput & " "
            End If
        Next blocksizeLoopInt
        
        
        frmMain.txtDirOutput.Text = frmMain.txtDirOutput.Text & BlockSizeOutput & " " & Chr(34) & sfilename(exportCount) & Chr(34) & " " & sfiletypedisplay & "   Track/Sector: " & sstartingtrack(exportCount) & "/" & sstartingsector(exportCount) & vbCrLf
   
   Next exportCount


   frmMain.txtDirOutput.Text = frmMain.txtDirOutput.Text & frmMain.lblBlocksFree.Caption
   ExportOutputFilename = frmMain.dialogDirectoryExport.Filename
   
   If Right(ExportOutputFilename, 4) <> ".txt" Then
     ExportOutputFilename = ExportOutputFilename & ".txt"
   End If

   '  Open ExportOutputFilename For Append As #1   '  use instead of below to append instead of overwrite
   Open ExportOutputFilename For Output As #1
   Print #1, frmMain.txtDirOutput.Text
   Close #1

Exit Sub

ErrHandler:
   MsgBox "An error has occurred.  Check to make sure the export path is writable.", vbExclamation

End Sub

Private Sub mnuFileExport_Click()
     Dim fcount As Integer
     Dim spath As String
     Dim text1 As String
     Dim Text2 As String
     
     Text2 = ""
       If FileExportPath <> "" Then
         text1 = FileExportPath
       Else
         text1 = "C:"
       End If
    'The call can not have a trailing slash, so
    'strip it from the path if present
     spath = UnqualifyPath((text1))
     
    'call the function, returning the path
    'selected (or blank if cancelled)
     Text2 = BrowseForFolderByPIDL(spath)
         
     If Text2 = "" Then
       Exit Sub
     End If
     
     FileExportPath = Text2
     
   For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          FileExport (i)
          fcount = fcount + 1
      End If
  
   Next i

   MsgBox fcount & " files exported to " & FileExportPath

End Sub


Private Sub FileExport(listvar As Integer)

   Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim g As Long
   Dim BBTT(0 To 257) As Byte
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim varString As String
   Dim ExportName As String
   Dim ExportPath As String
   Dim choppedFileName As String
   Dim tstring As String
   bitespersector = 256
   
If listvar = -1 Then
MsgBox "Highlight the files you want to export.", vbExclamation, "No Files Selected"
  Exit Sub
End If

      
         For p = 1 To Len(sfilename(listvar))
             If Mid(sfilename(listvar), p, 1) = "*" Or _
                Mid(sfilename(listvar), p, 1) = "/" Or _
                Mid(sfilename(listvar), p, 1) = "\" Or _
                Mid(sfilename(listvar), p, 1) = "?" Or _
                Mid(sfilename(listvar), p, 1) = "|" Or _
                Mid(sfilename(listvar), p, 1) = ":" Or _
                Mid(sfilename(listvar), p, 1) = "<" Or _
                Mid(sfilename(listvar), p, 1) = ">" Or _
                Mid(sfilename(listvar), p, 1) = Chr$(0) Or _
                Mid(sfilename(listvar), p, 1) = Chr$(34) Then
                  ExportName = ExportName & "_"
             Else
                  ExportName = ExportName & Mid(sfilename(listvar), p, 1)
             End If
         Next p
         
   choppedFileName = ExportName
     For u = 1 To 16
        If Mid(choppedFileName, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If

        choppedFileName = Left(choppedFileName, hitanA0 - 1)
         
Call convDecToBin(sfiletype(listvar))
tstring = strBin
Call GETFILETYPE(tstring)

      If FTYPE = "???" Then
         FTYPE = "___"
      End If
 
      Do
 
         ExportName = choppedFileName & "." & FTYPE
         
         If Right(FileExportPath, 1) = "\" Then
           ExportPath = FileExportPath & ExportName
         Else
           ExportPath = FileExportPath & "\" & ExportName
         End If
           
        FileExists (ExportPath)
        If FileExists(ExportPath) = True Then choppedFileName = choppedFileName & "_copy"

      Loop While FileExists(ExportPath) = True

      
     ' changed on 8/17/2010 per Ray Knight's suggestion so that exported files end up with all lowercase name
    '   Open ExportPath For Binary As #2
       Open LCase(ExportPath) For Binary As #2
    
       Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)
      
      
            ' If file starts at 18/0 or 18/1 then treat it as a seperator line (only if option enabled)
    If treatTrack18FilesAsEmpty = 1 Then
        If sstartingtrack(listvar) = 18 Then
             If sstartingsector(listvar) = 0 Or sstartingsector(listvar) = 1 Then
                 nxtTrack = 0
                 nxtSector = 0
             End If
        End If
    End If
      
      For p = 1 To 700
      
      If nxtTrack = 0 Then
       
        If nxtSector > 0 Then
          varString = String(nxtSector - 1, " ")
          Get #1, Val(BITE2START + 2), varString
          Put #2, , varString
        End If
        Close #1
         Close #2
         frmMain.MousePointer = 0
         Exit Sub
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

    BITE2START = BITE2START + 1
      
    
         Get #1, Val(BITE2START), trackbite
         Get #1, Val(BITE2START + 1), sectorbite
         
         nxtTrack = trackbite
         nxtSector = sectorbite
   
                 If nxtTrack <> 0 Then
                 varString = String(254, " ")
                 Get #1, Val(BITE2START + 2), varString
                   Put #2, , varString
                 End If
   
           

 Next p
      Close #1
      Close #2
      frmMain.MousePointer = 0

End Sub


Private Sub mnuFileImport_Click()
    Dim i As String
      i = ""

    FileImport (i)
End Sub



Private Sub FileImport(filenameImport As String)
    Dim fileImportLength As Long
    Dim fileImportByte As Byte
    Dim importBITE As Long
    Dim loopval As Integer
    Dim BITE2START As Long
    Dim currentBlock As Long
    Dim testvar As Byte
    Dim test2var As Byte
    Dim nextBlocktoUse As Integer
    Dim switchOffHighandLow As Integer
    Dim i As Long
    Dim firstTimeThrough As Integer
    Dim totBlocks As Integer
    Dim displayFName As String
    Dim p00Detector(1 To 7) As Byte
    Dim p00Detected As Integer
    Dim bites2go As Long
    Dim varString As String
    Dim FTYPEDISPLAY As String
    Dim TooBigToFitOnDisk As String
    Dim toolargeVar As Integer
    Dim tempvar As Double
    Dim tempString As String
    Dim filenameDragDrop As String
    Dim t64Detector(1 To 9) As Byte
    Dim t64Detected As Integer
    Dim t64DirLengthTotal As Long
    Dim t64DirLengthFirstByte As Byte
    Dim t64DirLengthSecondByte As Byte
    Dim T64StartAddressFirstByte As Byte
    Dim T64StartAddressSecondByte As Byte
    Dim T64EndAddressFirstByte As Byte
    Dim T64EndAddressSecondByte As Byte
    Dim t64FileLength As Long
    Dim t64FileOffsetFirstByte As Byte
    Dim t64FileOffsetSecondByte As Byte
    Dim T64FileOffset As Long
    Dim varStringTEMP As String
    Dim t64BITE1 As Byte
    Dim t64BITE2 As Byte
    Dim TTT As Long
    Dim UUU As Long

If dialogopen.Filename = "" Then
 '     MsgBox "Please load a file first.", vbExclamation, "No file loaded"
  Exit Sub
End If
    toolargeVar = 0
    checkForFullDir
    If cantFitDirOnDisk <> 0 Then
       MsgBox "Sorry, you can't add a file to the disk." & vbCrLf & "The directory on the disk is completely full.", vbInformation, "Directory Full"
       Exit Sub
    End If

    tempvar1 = 0

    If filenameImport <> "" Then
       List2.Clear
       filenames = filenameImport
       filenameDragDrop = filenames
       tempvar1 = 1
       List2.AddItem filenames
    Else
    dialog3.Filename = ""
    dialog3.MaxFileSize = 4 * 1024
    dialog3.Flags = &H1000 Or &H4 Or &H800 Or &H200 Or &H80000 Or &H100000
    List2.Clear
'    dialog3.Filter = "All Files (*.*)|*.*"
    dialog3.Filter = "C64 Program File (*.prg)|*.prg|PC64 Format (*.p00)|*.p00|Tape Image (*.t64)|*.t64|C64 Sequential File (*.seq)|*.seq|C64 Relative File (*.rel)|*.rel|C64 User File (*.usr)|*.usr|All Files (*.*)|*.*"
    dialog3.ShowOpen
    filenames = dialog3.Filename
    End If

    
    If Len(filenames) = 0 Then
        Exit Sub
    End If


'    tempvar1 = 0
    If dialog3.FileTitle <> "" Then
        List2.Clear
        List2.AddItem dialog3.FileTitle
        tempvar1 = 1
    End If

   If tempvar1 = 0 Then
    spPosition = InStr(filenames, Chr(0))
    pathName = Left(filenames, spPosition - 1)
    filenames = Mid(filenames, spPosition + 1)

    If Len(filenames) = 0 Then
        MsgBox "No files selected"
        Exit Sub
    Else
        spPosition = InStr(filenames, Chr(0))
        While spPosition > 0
            List2.AddItem Left(filenames, spPosition - 1)
            filenames = Mid(filenames, spPosition + 1)
            spPosition = InStr(filenames, Chr(0))
        Wend

        List2.AddItem filenames
    End If
   End If
       
      For k = 0 To List2.ListCount - 1
         List2.ListIndex = k
         If tempvar1 = 0 Then
           fullpath = pathName & "\" & List2.Text
         Else
           fullpath = filenames
         End If
         
       checkForFullDir
    If cantFitDirOnDisk <> 0 Then
       MsgBox "Sorry, you can't add any more files to the disk." & vbCrLf & "The directory on the disk is completely full.", vbInformation, "Directory Full"
       Exit Sub
    End If
         
      fileImportLength = FileLen(fullpath)
      tempvar2 = 0
      If fileImportLength / 254 > Val(frmMain.lblBlocksFree) Then
         toolargeVar = 1
         TooBigToFitOnDisk = TooBigToFitOnDisk & fullpath & vbCrLf
'         MsgBox fullpath & vbCrLf & "is too big to fit on the disk.", vbInformation, "No room on disk."
         tempvar2 = 1

      End If
  If tempvar2 = 0 Then
    Dim b As Integer

    b = frmMain.List1.ListCount
      If filenameImport <> "" Then
        fullpath = filenameDragDrop
      End If
      
      D64Modified = True
      
      Open fullpath For Binary As #2
      Open frmMain.dialogopen.Filename For Binary As #1


       ' code to detect for those fucking P00 files
       For x = 1 To 7
         Get #2, x, p00Detector(x)
       Next x

       If Hex(p00Detector(1)) = "43" And Hex(p00Detector(2)) = "36" And _
       Hex(p00Detector(3)) = "34" And Hex(p00Detector(4)) = "46" And _
       Hex(p00Detector(5)) = "69" And Hex(p00Detector(6)) = "6C" And _
       Hex(p00Detector(7)) = "65" Then
         p00Detected = 1
'         MsgBox "p00 detected"
       End If
       
          
       ' code to detect for T64 files
       For x = 1 To 9
          Get #2, x, t64Detector(x)
       Next x
          
       If Hex(t64Detector(1)) = "43" And Hex(t64Detector(2)) = "36" And _
       Hex(t64Detector(3)) = "34" And Hex(t64Detector(4)) = "53" And _
       Hex(t64Detector(5)) = "20" And Hex(t64Detector(6)) = "74" And _
       Hex(t64Detector(7)) = "61" And Hex(t64Detector(8)) = "70" And _
       Hex(t64Detector(9)) = "65" Or _
       Hex(t64Detector(1)) = "43" And Hex(t64Detector(2)) = "36" And _
       Hex(t64Detector(3)) = "34" And Hex(t64Detector(4)) = "20" And _
       Hex(t64Detector(5)) = "74" And Hex(t64Detector(6)) = "61" And _
       Hex(t64Detector(7)) = "70" And Hex(t64Detector(8)) = "65" Then
       
         t64Detected = 1
       End If
          
       If t64Detected = 1 Then
       ' Now that we've detected a T64, lets see how many DIR entries it has
       Get #2, 37, t64DirLengthFirstByte
       Get #2, 38, t64DirLengthSecondByte

       t64DirLengthTotal = (t64DirLengthSecondByte * 256) + t64DirLengthFirstByte
       
  '     For z = 1 To t64DirLengthTotal
   '    Next z
       
       ' byte 66 starts the first DIR entry
       
          
       End If
          
          
        If t64Detected = 1 Then
        
            Get #2, 67, T64StartAddressFirstByte
            Get #2, 68, T64StartAddressSecondByte
            Get #2, 69, T64EndAddressFirstByte
            Get #2, 70, T64EndAddressSecondByte
          
            Get #2, 73, t64FileOffsetFirstByte
            Get #2, 74, t64FileOffsetSecondByte
               
  
          TTT = T64EndAddressSecondByte
          UUU = T64StartAddressSecondByte

          t64FileLength = (TTT * 256) + T64EndAddressFirstByte - (UUU * 256) - T64StartAddressFirstByte
               
          T64FileOffset = (t64FileOffsetSecondByte * 256) + t64FileOffsetFirstByte
        
          If t64DirLengthTotal < 2 Then
             t64FileLength = FileLen(fullpath) - T64FileOffset
          Else
             MsgBox "This program currently does not support multi-file T64's.  Sorry.", vbInformation
             Close #1
             Close #2
             Exit Sub
          End If
        
          If t64FileLength + T64FileOffset > FileLen(fullpath) Then
             t64FileLength = FileLen(fullpath) - T64FileOffset
          End If
        
        End If
          
          importBITE = 1
          loopval = 254
          
          ' lets find a block that we can use
               currentBlock = 336   'lets start at 17/0
               bitespersector = 256
               bites2go = fileImportLength
               
               
     tfilename = fullpath
     tcountvar = Len(fullpath)
     sfilename(b) = ""
  '   frmMain.MousePointer = 11
          
     Do Until Left(sfilename(b), 1) = "\"
       
       sfilename(b) = Mid(tfilename, tcountvar, 1) & sfilename(b)
       tcountvar = tcountvar - 1
     Loop
    
    tcountvar = Len(sfilename(b))
    sfilename(b) = Right(sfilename(b), tcountvar - 1)
    
    sfiletype(b) = &H82
    FTYPEDISPLAY = "PRG"

    If Right(sfilename(b), 4) = ".PRG" Or Right(sfilename(b), 4) = ".prg" Then
         sfiletype(b) = &H82
         FTYPEDISPLAY = "PRG"
         sfilename(b) = Left(sfilename(b), Len(sfilename(b)) - 4)
     End If
        
    If Right(sfilename(b), 4) = ".SEQ" Or Right(sfilename(b), 4) = ".seq" Then
         sfiletype(b) = &H81
         FTYPEDISPLAY = "SEQ"
         sfilename(b) = Left(sfilename(b), Len(sfilename(b)) - 4)
     End If
        
    If Right(sfilename(b), 4) = ".DEL" Or Right(sfilename(b), 4) = ".del" Then
         sfiletype(b) = &H80
         FTYPEDISPLAY = "DEL"
         sfilename(b) = Left(sfilename(b), Len(sfilename(b)) - 4)
     End If
    
    If Right(sfilename(b), 4) = ".SCR" Or Right(sfilename(b), 4) = ".scr" Then
         sfiletype(b) = &H0
         FTYPEDISPLAY = "SCR"
         sfilename(b) = Left(sfilename(b), Len(sfilename(b)) - 4)
     End If
    
    If Right(sfilename(b), 4) = ".USR" Or Right(sfilename(b), 4) = ".usr" Then
         sfiletype(b) = &H83
         FTYPEDISPLAY = "USR"
         sfilename(b) = Left(sfilename(b), Len(sfilename(b)) - 4)
     End If

    If Right(sfilename(b), 4) = ".REL" Or Right(sfilename(b), 4) = ".rel" Then
         sfiletype(b) = &H84
         FTYPEDISPLAY = "REL"
         sfilename(b) = Left(sfilename(b), Len(sfilename(b)) - 4)
     End If



    If p00Detected = 1 Then
        sfilename(b) = ""
      For x = 9 To 24
        Get #2, x, p00Detector(1)
        
        If p00Detector(1) = 0 Then Exit For

        sfilename(b) = sfilename(b) & Chr(p00Detector(1))
        p00Detected = 0
      Next x
    
      importBITE = 27
      bites2go = bites2go - 26
    End If
    
    
    If t64Detected = 1 Then
        sfilename(b) = ""
      For x = 81 To 96
         Get #2, x, t64Detector(1)
         
         If t64Detector(1) = &H20 Then ppp = 1
    
         If ppp <> 1 Then
           sfilename(b) = sfilename(b) & Chr(t64Detector(1))
         End If
         ppp = 0
        
      Next x
    
      importBITE = T64FileOffset + 1
      bites2go = t64FileLength
     
        
    
    End If
    
    
    
    If Len(sfilename(b)) > 16 Then
      sfilename(b) = Left(sfilename(b), 16)
    End If
    sfilename(b) = UCase(sfilename(b))
    displayFName = sfilename(b)
     If Len(sfilename(b)) < 16 Then
        For u = Len(sfilename(b)) To 15
             sfilename(b) = sfilename(b) & " "  ' there's an $A0 in there
             extraspaces = extraspaces & " "
        Next u
     End If
               
                findFirstBlocktoUse
                If track = 0 And sector = 0 Then
             MsgBox fullpath & vbCrLf & "is too big to fit on the disk.", vbInformation, "No room on disk."
             Close #1
             Close #2
             Exit Sub
             End If
            Do Until bites2go < 1
                  


            
            currentBlock = block

   blocksAllocated(currentBlock) = 1
   blocksNeedingWiped(currentBlock) = 0

   If firstTimeThrough = 0 Then

     Call blockNum2TrackandSector(currentBlock)
     sstartingtrack(b) = convertedTrack
     sstartingsector(b) = convertedSector
     firstTimeThrough = 1
   End If

           
            ' If we've still got more than 254 bytes then we're not on the last block, so find the next one
            If bites2go > 254 Then
            
              findNextBlocktoUse
              nextBlocktoUse = block
            End If

 
            totBlocks = totBlocks + 1

            'if we made it out of the loop we must have found a block to use, woo hoo
            
            If bites2go < 255 Then
              bitesleft = bites2go
          
                 For i = (currentBlock * 256) + 3 To (currentBlock * 256) + bitesleft + 2
 
                   Get #2, importBITE, testvar
                   importBITE = importBITE + 1
                   bites2go = bites2go - 1
                   Put #1, i, testvar
  
                 Next i
                
            
                
                test2var = bitesleft + 1
                Put #1, (currentBlock * 256) + 1, 0
                Put #1, ((currentBlock * 256) + 2), test2var
                blocksAllocated(currentBlock) = 1
                blockOwner(currentBlock) = displayFName
            Else
               
                If t64Detected = 1 And firstpass = 0 Then
                   firstpass = 1
                   varString = String(252, " ")
             '      varString = varString & &H1
              '     varString = varString & &H8
                   Get #2, importBITE, varString
                   bites2go = bites2go - 252
                   importBITE = importBITE + 252
                   t64BITE1 = CLng(&H1)
                   t64BITE2 = CLng(&H8)
                   Put #1, (currentBlock * 256) + 3, t64BITE1
                   Put #1, (currentBlock * 256) + 4, t64BITE2
                   Put #1, (currentBlock * 256) + 5, varString
                Else
                   varString = String(254, " ")
                   Get #2, importBITE, varString
                   bites2go = bites2go - 254
                   importBITE = importBITE + 254
                   Put #1, (currentBlock * 256) + 3, varString
                End If
                
                Call blockNum2TrackandSector(nextBlocktoUse)
                test2var = (convertedTrack)
                 Put #1, ((currentBlock * 256) + 1), test2var
                test2var = (convertedSector)
                  Put #1, ((currentBlock * 256) + 2), test2var

                blocksAllocated(currentBlock) = 1
                blockOwner(currentBlock) = displayFName
            End If
            
            currentBlock = nextBlocktoUse
            
            If switchOffHighandLow = 0 Then
              switchOffHighandLow = 1
            Else
              switchOffHighandLow = 0
            End If
            
            Loop
                      
'    sfiletype(b) = &H82
     If totBlocks > 255 Then
      
      tempvar = totBlocks / 256
      tempString = tempvar
     
     For i = 1 To 20
        If Mid(tempString, i, 1) = "." Then
           tempString = Left(tempString, i - 1)
           Exit For
        End If
     Next i
             
      sfilesize2(b) = Val(tempString)
      sfilesize1(b) = totBlocks - (sfilesize2(b) * 256)

    Else
      sfilesize1(b) = totBlocks
      sfilesize2(b) = 0
    End If

    sfilesizeDisplay(b) = sfilesize1(b) + (sfilesize2(b) * 256)
    frmMain.List1.AddItem Chr$(34) & displayFName & Chr$(34) & extraspaces & vbTab & sstartingtrack(b) & vbTab & sstartingsector(b) & vbTab & sfilesizeDisplay(b) & vbTab & FTYPEDISPLAY
      frmMain.MousePointer = 0
      extraspaces = ""
    Close #1
    Close #2
    t64Detected = 0
    firstpass = 0
    Call writeOutDirectory
    Call BAMWRITE
    Call updateFileCount
    firstTimeThrough = 0
    totBlocks = 0
   End If
    Next k

 '   If Len(TooBigToFitOnDisk) > 1024 Then
  '      TooBigToFitOnDisk = Left(TooBigToFitOnDisk, 1024)
   ' End If
    
    If toolargeVar <> 0 Then
       MsgBox "The following files were too large to fit on the D64." & vbCrLf & vbCrLf & TooBigToFitOnDisk, vbInformation, "Too big"
    End If
filenameDragDrop = ""
filenameImport = ""
End Sub


Private Sub mnuInvertSelection_Click()
   For i = 0 To List1.ListCount - 1
              
       If List1.Selected(i) = True Then
         List1.Selected(i) = False
       Else
         List1.Selected(i) = True
       End If
   Next i
End Sub

Private Sub mnuModifyFileProperties_Click()
      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only modify one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          ModifyFileProperties (i)
          DirEntryBeingEdited = i
          Exit Sub
      End If
  
   Next i

End Sub

Private Sub ModifyFileProperties(listvar As Integer)
   Dim displayfilename As String
   Dim hitanA0 As Integer
   Dim firstTime As Integer
   Dim tempvar As Integer

If listvar = -1 Then
  MsgBox "Highlight the file you want to edit.", vbExclamation, "No File Selected"
  Exit Sub
End If


If sfiletype(listvar) = 0 Then
   MsgBox "That file is scratched.  Unscratch it first.", vbExclamation, "Scratched File"
   Exit Sub
End If

hitanA0 = 0
firstTime = 0
tempvar = 0
   displayfilename = sfilename(listvar)
     For u = 1 To 16
        If Mid(displayfilename, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If

        displayfilename = Left(displayfilename, hitanA0 - 1)

   frmFileProperties.txtFilename = displayfilename
   frmFileProperties.txtFileSize = sfilesizeDisplay(listvar)
   frmFileProperties.txtStartingSector = sstartingsector(listvar)
   frmFileProperties.txtStartingTrack = sstartingtrack(listvar)
   
   Call convDecToBin(BITE)
   
Call convDecToBin(sfiletype(listvar))
   
Call GETFILETYPE(strBin)
   
   frmFileProperties.optPRG = True
   
   If Left(strBin, 1) = "0" Then
      frmFileProperties.chkSplat.Value = Checked
   End If
   
   If Mid(strBin, 2, 1) = "1" Then
      frmFileProperties.chkLocked.Value = Checked
   End If
      
   If Right(strBin, 3) = "100" Then
      frmFileProperties.optRel = True
   End If
      
   If Right(strBin, 3) = "011" Then
      frmFileProperties.optUsr = True
   End If
   
   If Right(strBin, 3) = "010" Then
      frmFileProperties.optPRG = True
   End If
   
   If Right(strBin, 3) = "001" Then
      frmFileProperties.optSeq = True
   End If
   
   If Right(strBin, 3) = "000" Then
      frmFileProperties.optDel = True
    End If


frmFileProperties.Show
If SelectNextFileUponSaveVar = 1 Then
    frmFileProperties.mnuSelectNextFileUponSave.Checked = True
End If
frmMain.Enabled = False

End Sub


Private Sub mnuQuit_Click()

   If D64Modified = True Then
     If MsgBox("Changes to the D64 in memory have not been saved yet." & vbCrLf & "Are you sure you want to exit the program?", vbYesNo + vbDefaultButton2, "Changes not saved") = vbNo Then
            Exit Sub
     End If
   End If

frmMainTOP = frmMain.Top
frmMainLEFT = frmMain.Left
  Call saveWindowLocations
  End
End Sub

Private Sub mnuRemoveDirectoryEntry_Click()
  Dim listholder As Integer
  
    listholder = List1.ListCount - 1
 
   For i = 0 To listholder
   
      If i > List1.ListCount - 1 Then
         Exit Sub
      End If

      If List1.Selected(i) = True Then
          RemoveDirectoryEntry (i)
          listholder = listholder - 1
          i = i - 1
      End If
  
   Next i

End Sub

Private Sub RemoveDirectoryEntry(listvar As Integer)

Dim tstring As String
Dim tvar As Integer
Dim tempStr As String
Dim tempbyte As Byte
Dim tempInt As Integer


If listvar = -1 Then
MsgBox "Highlight the file you want to remove.", vbExclamation, "No file selected"
  Exit Sub
End If

D64Modified = True

For x = 0 To List1.ListCount

t = listvar

tvar = listvar + 1
tstring = List1.Text

If listvar = List1.ListCount - 1 Then
  Exit For
End If


List1.AddItem List1.Text, listvar + 2
List1.RemoveItem listvar
listvar = tvar

  tempStr = sfilename(t)
  sfilename(t) = sfilename(t + 1)
  sfilename(t + 1) = tempStr

  tempInt = sstartingtrack(t)
  sstartingtrack(t) = sstartingtrack(t + 1)
  sstartingtrack(t + 1) = tempInt
  
  tempInt = sstartingsector(t)
  sstartingsector(t) = sstartingsector(t + 1)
  sstartingsector(t + 1) = tempInt
  
  tempInt = sfilesize1(t)
  sfilesize1(t) = sfilesize1(t + 1)
  sfilesize1(t + 1) = tempInt
  
  tempInt = sfilesize2(t)
  sfilesize2(t) = sfilesize2(t + 1)
  sfilesize2(t + 1) = tempInt
  
  tempInt = sfilesizeDisplay(t)
  sfilesizeDisplay(t) = sfilesizeDisplay(t + 1)
  sfilesizeDisplay(t + 1) = tempInt
  
  tempbyte = sfiletype(t)
  sfiletype(t) = sfiletype(t + 1)
  sfiletype(t + 1) = tempbyte
  
  tempbyte = byte15(t)
  byte15(t) = byte15(t + 1)
  byte15(t + 1) = tempbyte
  
  tempbyte = byte16(t)
  byte16(t) = byte16(t + 1)
  byte16(t + 1) = tempbyte
  
  tempbyte = byte17(t)
  byte17(t) = byte17(t + 1)
  byte17(t + 1) = tempbyte
  
  tempbyte = byte18(t)
  byte18(t) = byte18(t + 1)
  byte18(t + 1) = tempbyte

  tempbyte = byte19(t)
  byte19(t) = byte19(t + 1)
  byte19(t + 1) = tempbyte
  
  tempbyte = byte1A(t)
  byte1A(t) = byte1A(t + 1)
  byte1A(t + 1) = tempbyte
  
  tempbyte = byte1B(t)
  byte1B(t) = byte1B(t + 1)
  byte1B(t + 1) = tempbyte

  tempbyte = byte1C(t)
  byte1C(t) = byte1C(t + 1)
  byte1C(t + 1) = tempbyte
  
  tempbyte = byte1D(t)
  byte1D(t) = byte1D(t + 1)
  byte1D(t + 1) = tempbyte

  Next x

   List1.RemoveItem List1.ListCount - 1
   sfilename(t) = ""
   Call updateFileCount
End Sub



Private Sub mnuMoveFileToBottom_Click()
      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only move one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          MoveFileToBottom (i)
          Exit Sub
      End If
  
   Next i
End Sub

Private Sub MoveFileToBottom(listvar As Integer)

    Do While listvar < List1.ListCount
        SortDown (listvar)
        listvar = listvar + 1
    Loop

End Sub

Private Sub mnuMoveFileToTop_Click()
      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only move one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          MoveFileToTop (i)
          Exit Sub
      End If
  
   Next i
End Sub

Private Sub MoveFileToTop(listvar As Integer)

    Do While listvar > 0
        SortUp (listvar)
        listvar = listvar - 1
    Loop

End Sub


Private Sub mnuRevert_Click()

If frmMain.dialogopen.Filename = "" Then
  Exit Sub
End If


If MsgBox("Revert to the previously saved version of" & vbCrLf & OriginalFileName & "?", vbYesNo, "Revert?") = vbYes Then

  frmMain.dialogopen.Filename = OriginalFileName
  LoadFile (frmMain.dialogopen.Filename)

End If

End Sub

Private Sub mnuSaveFile_Click()
      
   If dialogopen.Filename = "" Then
  '   MsgBox "There is no file to save.", vbExclamation, "No file"
     Exit Sub
   End If
    
'    checkForFullDir
'    If cantFitDirOnDisk <> 0 Then
'       MsgBox "Whoops, you can't fit the DIR back on the disk.", vbInformation, "Disk Full"
'       Exit Sub
'    End If

  Dim i As Long
  Dim BITE2START As Long
  Dim blocknumb As Integer

   
       ' zero out all the blocks from wiped files
       Open frmMain.dialogopen.Filename For Binary As #1
       For i = 0 To 682
          If blocksNeedingWiped(i) = 0 Then
             'do nothing
          Else
                BITE2START = (i * 256) + 1
                varString$ = String$(256, 0)
                Put #1, BITE2START, varString$
                blocksNeedingWiped(i) = 0
          End If
     
       Next i
    
       blocknumb = 0
    
       If frmMain.mnuViewErrorBlocks.Enabled = True Then
       
          For i = 174849 To 175531
              Put #1, i, ErrorCodeBlocks(blocknumb)
              blocknumb = blocknumb + 1
          Next i
       End If
    
       Close #1


   Call writeOutDirectory
   Call BAMWRITE
   
   
   ' copy the temp file over top of the real file
   Open frmMain.dialogopen.Filename For Binary As #1
      Dim tempfileLength As Long
      Dim varString1 As String
      
      tempfileLength = FileLen(frmMain.dialogopen.Filename)
      
      varString1 = String(tempfileLength, " ")
      Get #1, 1, varString1
      
Open OriginalFileName For Binary As #2
      Put #2, , varString1
Close #2

Close #1


   MsgBox "Changes to the D64 have been saved.", vbInformation, "D64 Saved"
   D64Modified = False

End Sub


Private Sub cmdSortDown_Click()

      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only move one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          SortDown (i)
          Exit Sub
      End If
  
   Next i
End Sub

Private Sub SortDown(listvar As Integer)
Dim tstring As String
Dim tvar As Integer
Dim tempStr As String
Dim tempbyte As Byte
Dim t As Integer
Dim tempInt As Integer


If listvar = -1 Then
MsgBox "Highlight the file you want to move.", vbExclamation, "No file selected"
  Exit Sub
End If

D64Modified = True

t = listvar

tvar = listvar + 1
tstring = List1.Text

If listvar = List1.ListCount - 1 Then
  Exit Sub
End If
List1.AddItem List1.Text, listvar + 2
List1.RemoveItem listvar
List1.Selected(tvar) = True
List1.Selected(tvar - 1) = False

  tempStr = sfilename(t)
  sfilename(t) = sfilename(t + 1)
  sfilename(t + 1) = tempStr

  tempInt = sstartingtrack(t)
  sstartingtrack(t) = sstartingtrack(t + 1)
  sstartingtrack(t + 1) = tempInt
  
  tempInt = sstartingsector(t)
  sstartingsector(t) = sstartingsector(t + 1)
  sstartingsector(t + 1) = tempInt
  
  tempInt = sfilesize1(t)
  sfilesize1(t) = sfilesize1(t + 1)
  sfilesize1(t + 1) = tempInt
  
  tempInt = sfilesize2(t)
  sfilesize2(t) = sfilesize2(t + 1)
  sfilesize2(t + 1) = tempInt
  
  tempInt = sfilesizeDisplay(t)
  sfilesizeDisplay(t) = sfilesizeDisplay(t + 1)
  sfilesizeDisplay(t + 1) = tempInt
  
  
  tempbyte = sfiletype(t)
  sfiletype(t) = sfiletype(t + 1)
  sfiletype(t + 1) = tempbyte
  
  tempbyte = byte15(t)
  byte15(t) = byte15(t + 1)
  byte15(t + 1) = tempbyte
  
  tempbyte = byte16(t)
  byte16(t) = byte16(t + 1)
  byte16(t + 1) = tempbyte
  
  tempbyte = byte17(t)
  byte17(t) = byte17(t + 1)
  byte17(t + 1) = tempbyte
  
  tempbyte = byte18(t)
  byte18(t) = byte18(t + 1)
  byte18(t + 1) = tempbyte

  tempbyte = byte19(t)
  byte19(t) = byte19(t + 1)
  byte19(t + 1) = tempbyte
  
  tempbyte = byte1A(t)
  byte1A(t) = byte1A(t + 1)
  byte1A(t + 1) = tempbyte
  
  tempbyte = byte1B(t)
  byte1B(t) = byte1B(t + 1)
  byte1B(t + 1) = tempbyte

  tempbyte = byte1C(t)
  byte1C(t) = byte1C(t + 1)
  byte1C(t + 1) = tempbyte
  
  tempbyte = byte1D(t)
  byte1D(t) = byte1D(t + 1)
  byte1D(t + 1) = tempbyte

End Sub

Private Sub cmdSortUp_Click()

      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only move one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          SortUp (i)
          Exit Sub
      End If
  
   Next i
End Sub

Private Sub SortUp(listvar As Integer)
Dim tstring As String
Dim tvar As Integer
Dim tempStr As String
Dim t As Integer
Dim tempInt As Integer
Dim tempbyte As Byte

If listvar = -1 Then
MsgBox "Highlight the file you want to move.", vbExclamation, "No file selected"
  Exit Sub
End If

D64Modified = True

t = listvar

tvar = listvar - 1
tstring = List1.Text

If listvar = 0 Then
  Exit Sub
End If
List1.AddItem List1.Text, listvar - 1
List1.RemoveItem listvar + 1
List1.Selected(tvar) = True
List1.Selected(tvar + 1) = False
    
  tempStr = sfilename(t)
  sfilename(t) = sfilename(t - 1)
  sfilename(t - 1) = tempStr
  
    tempInt = sstartingtrack(t)
  sstartingtrack(t) = sstartingtrack(t - 1)
  sstartingtrack(t - 1) = tempInt
  
  tempInt = sstartingsector(t)
  sstartingsector(t) = sstartingsector(t - 1)
  sstartingsector(t - 1) = tempInt
  
  tempInt = sfilesize1(t)
  sfilesize1(t) = sfilesize1(t - 1)
  sfilesize1(t - 1) = tempInt
  
  tempInt = sfilesize2(t)
  sfilesize2(t) = sfilesize2(t - 1)
  sfilesize2(t - 1) = tempInt
  
  tempInt = sfilesizeDisplay(t)
  sfilesizeDisplay(t) = sfilesizeDisplay(t - 1)
  sfilesizeDisplay(t - 1) = tempInt
  
  
  tempbyte = sfiletype(t)
  sfiletype(t) = sfiletype(t - 1)
  sfiletype(t - 1) = tempbyte
  
  tempbyte = byte15(t)
  byte15(t) = byte15(t - 1)
  byte15(t - 1) = tempbyte
  
  tempbyte = byte16(t)
  byte16(t) = byte16(t - 1)
  byte16(t - 1) = tempbyte
  
  tempbyte = byte17(t)
  byte17(t) = byte17(t - 1)
  byte17(t - 1) = tempbyte
  
  tempbyte = byte18(t)
  byte18(t) = byte18(t - 1)
  byte18(t - 1) = tempbyte

  tempbyte = byte19(t)
  byte19(t) = byte19(t - 1)
  byte19(t - 1) = tempbyte

  tempbyte = byte1A(t)
  byte1A(t) = byte1A(t - 1)
  byte1A(t - 1) = tempbyte

  tempbyte = byte1B(t)
  byte1B(t) = byte1B(t - 1)
  byte1B(t - 1) = tempbyte

  tempbyte = byte1C(t)
  byte1C(t) = byte1C(t - 1)
  byte1C(t - 1) = tempbyte

  tempbyte = byte1D(t)
  byte1D(t) = byte1D(t - 1)
  byte1D(t - 1) = tempbyte

End Sub

Private Sub mnuLoadFile_Click()
Dim FileNameHolder As String

   If D64Modified = True Then
     If MsgBox("Changes to the D64 in memory have not been saved yet." & vbCrLf & "Are you sure you want to load a different D64?", vbYesNo + vbDefaultButton2, "Changes not saved") = vbNo Then
            Exit Sub
     End If
   End If
   
   If InterLeave <> 10 And warnedAboutInterleave = 0 Then
        MsgBox "WARNING!!!!" & vbCrLf & "Import Interleave value is set to " & InterLeave & " instead of the default of 10!", vbInformation
        warnedAboutInterleave = 1
   End If

FileNameHolder = dialogopen.Filename
dialogopen.Filename = ""
dialogopen.InitDir = lastDirPath
dialogopen.Flags = &H1000 Or &H4 Or &H800
dialogopen.Filter = "C64 Disk Image (*.d64)|*.d64|C64 Sequential File (*.seq)|*.seq|All Files (*.*)|*.*"
dialogopen.ShowOpen
Dim Filename As String



If dialogopen.Filename = "" Then
  If FileNameHolder <> "" Then
      dialogopen.Filename = FileNameHolder
  End If
  Exit Sub
End If
           
Filename = dialogopen.Filename
           
  ' if the file is a SEQ file, lets open it with the SEQ viewer
  If UCase(Right(Filename, 3)) = "SEQ" Then

   Dim BITE As Byte
   Dim tempString As String
   
 
   Load frmFileViewer
   frmFileViewer.Left = frmFileViewerLEFT
   frmFileViewer.Top = frmFileViewerTOP
   frmFileViewer.lblFileName.Font = "CBM64"
   
  frmFileViewer.lblFileName = Filename
       Open frmMain.dialogopen.Filename For Binary As #1

            For i = 1 To FileLen(frmMain.dialogopen.Filename)

                Get #1, i, BITE

             If BITE = CLng(&HD) Then
               tempString = tempString & vbCrLf
             Else
               tempString = tempString & Chr(BITE)
             End If


            Next i

     lastDirPath = frmMain.dialogopen.Filename
     
     Do Until Right(lastDirPath, 1) = "\"
       lastDirPath = Left(lastDirPath, Len(lastDirPath) - 1)
     Loop


      frmFileViewer.text1.Text = tempString
      frmFileViewer.Show
      Close #1
      Exit Sub
  End If



If Right(Filename, 3) <> "d64" And Right(Filename, 3) <> "D64" Then
  MsgBox "That is not a D64 file"
  If FileNameHolder <> "" Then
      frmMain.dialogopen.Filename = FileNameHolder
  Else
     frmMain.dialogopen.Filename = ""
  End If
  Exit Sub

End If

If FileLen(Filename) <> 174848 And FileLen(Filename) <> 175531 And FileLen(Filename) <> 196608 And FileLen(Filename) <> 197376 Then
  MsgBox "That is not a D64 file"
  If FileNameHolder <> "" Then
      frmMain.dialogopen.Filename = FileNameHolder
  Else
     frmMain.dialogopen.Filename = ""
  End If

  Exit Sub
End If

lastDirPath = frmMain.dialogopen.Filename
     
     Do Until Right(lastDirPath, 1) = "\"
       lastDirPath = Left(lastDirPath, Len(lastDirPath) - 1)
     Loop


LoadFile (frmMain.dialogopen.Filename)

End Sub


Sub ProcDroppedFile(File As String) ' Called by DragDrop.bas upon drop
'    Dim X As Integer
 '   Dim c

'    c = Mid$(File, 1, 1)    'chop name at null-term
 '   While Asc(c) > 0
'        X = X + 1
 '       c = Mid$(File, X, 1)
  '  Wend
   ' If X > 1 Then
    '    File = Left$(File, X - 1)

        If UCase(Right(File, 3)) <> "D64" Then
             FileImport (File)
        Else
        
           If D64Modified = True Then
              If MsgBox("Changes to the D64 in memory have not been saved yet." & vbCrLf & "Are you sure you want to load a different D64?", vbYesNo + vbDefaultButton2, "Changes not saved") = vbNo Then
              Exit Sub
              End If
           End If
             LoadFile (File)
        End If
   ' End If
End Sub


Public Sub LoadFile(Filename As String)
Dim BITE As Byte
Dim tstring As String
Dim b As Integer
Dim z As Long
Dim Y As Integer
Dim i As Long
Dim nextDirSector As Byte
Dim nextDirTrack As Byte
Dim curDirTrack As Integer
Dim curDirSector As Integer
Dim sdiskname As String
Dim sdiskID As String
Dim displayfilename As String
Dim displayDiskName As String
Dim extraspaces As String
Dim d As Long
Dim e As Integer
Dim bq As Integer
Dim totalblocks As Integer
Dim hitanA0 As Integer
Dim firstTime As Integer
Dim tempvar As Integer
Dim pathVar As String
Dim FileNameHolder As String
Dim blocknum As Integer
Dim errorvari As Byte

 On Error GoTo errorhandler
curDirTrack = 18
curDirSector = 1
           
   D64Modified = False

   FileNameHolder = frmMain.dialogopen.Filename
   frmMain.dialogopen.Filename = Filename
           
'  strip out Quotes surrounding the filename if they exist
If Left(FileNameHolder, 1) = Chr$(34) And Right(FileNameHolder, 1) = Chr$(34) Then
     FileNameHolder = Left(FileNameHolder, Len(FileNameHolder) - 1)
     FileNameHolder = Right(FileNameHolder, Len(FileNameHolder) - 1)
End If
           
If dialogopen.Filename = "" Then
  If FileNameHolder <> "" Then
      dialogopen.Filename = FileNameHolder
  End If
  Exit Sub
End If
           
Filename = dialogopen.Filename
'  strip out Quotes surrounding the filename if they exist
If Left(Filename, 1) = Chr$(34) And Right(Filename, 1) = Chr$(34) Then
     Filename = Left(Filename, Len(Filename) - 1)
     Filename = Right(Filename, Len(Filename) - 1)
End If
           
frmMain.dialogopen.Filename = Filename
           
If Right(Filename, 3) <> "d64" And Right(Filename, 3) <> "D64" Then
  MsgBox "That is not a D64 file"
  If FileNameHolder <> "" Then
      frmMain.dialogopen.Filename = FileNameHolder
  Else
     frmMain.dialogopen.Filename = ""
  End If
  Exit Sub

End If

If FileLen(Filename) <> 174848 And FileLen(Filename) <> 175531 And FileLen(Filename) <> 196608 And FileLen(Filename) <> 197376 Then
  MsgBox "That is not a D64 file"
  If FileNameHolder <> "" Then
      frmMain.dialogopen.Filename = FileNameHolder
  Else
     frmMain.dialogopen.Filename = ""
  End If

  Exit Sub
End If

frmMain.lblDiskType = "35 TRACK D64 FILE (STANDARD)"
frmMain.lblDiskType.ForeColor = &H8000&

If FileLen(Filename) = 196608 Then
   If warningsDisabled = 0 Then
     MsgBox "That is a 40 track D64 File and is not" & vbCrLf & "fully supported by this program yet.", vbInformation
   End If
    frmMain.lblDiskType = "40 TRACK D64 FILE"
    frmMain.lblDiskType.ForeColor = &HC000C0
End If

    frmMain.mnuViewErrorBlocks.Enabled = False

If FileLen(Filename) = 197376 Then
   If warningsDisabled = 0 Then
     MsgBox "That is a 40 track D64 File and is not" & vbCrLf & "fully supported by this program yet.", vbInformation
   End If
    frmMain.lblDiskType = "40 TRACK D64 FILE + ERROR INFO"
    frmMain.lblDiskType.ForeColor = &H80FF&
    frmMain.mnuViewErrorBlocks.Enabled = True
End If

If FileLen(Filename) = 175531 Then
'   If warningsDisabled = 0 Then
 '    MsgBox "That D64 file has extended error information" & vbCrLf & "and is not fully supported by this program yet.", vbInformation
  ' End If
    frmMain.lblDiskType = "35 TRACK D64 FILE + ERROR INFO"
    frmMain.lblDiskType.ForeColor = &HFF&
    frmMain.mnuViewErrorBlocks.Enabled = True
End If
           
           
List1.Clear
b = 0

frmMain.List1.Enabled = True
frmMain.mnuSaveFile.Enabled = True
frmMain.cmdSave.Enabled = True
  frmMain.txtDiskID.Enabled = True
  frmMain.txtDiskName.Enabled = True

lblD64File.Caption = Filename


 If GetAttr(frmMain.dialogopen.Filename) And vbReadOnly Then
  If MsgBox("The D64 file is Read-Only.  Do you want to remove the Read-Only attribute?", vbYesNo + vbDefaultButton2, "Read Only File") = vbNo Then
     frmMain.mnuSaveFile.Enabled = False
     frmMain.cmdSave.Enabled = False
  Else
     SetAttr (frmMain.dialogopen.Filename), GetAttr(frmMain.dialogopen.Filename) - vbReadOnly
     
  End If
    
 End If

' NULL everything out
For bq = 0 To 145
   sfilename(bq) = ""
Next bq

For aq = 0 To 683
   blockOwner(aq) = ""
   blocksNeedingWiped(aq) = 0
   ErrorCodeBlocks(aq) = 0
Next aq

' If a tempfile already exists, lets whack it
If TempFileName1 <> "" Then
    Kill TempFileName1
End If

' Copy the D64 into a Randomly named Tempfile
TempFileName1 = GetTemporaryFilename("D64")

Open Filename For Binary As #1
      Dim tempfileLength As Long
      Dim varString1 As String
      
      tempfileLength = FileLen(Filename)
      
      varString1 = String(tempfileLength, " ")
      Get #1, 1, varString1
      
Open TempFileName1 For Binary As #2
      Put #2, , varString1
Close #2

Close #1


OriginalFileName = Filename

Filename = TempFileName1
frmMain.dialogopen.Filename = TempFileName1

' End Temp File Copy




Open Filename For Binary As #1

       If frmMain.mnuViewErrorBlocks.Enabled = True Then
        blocknumb = 0
    
         For w = 174849 To 175531
            Get #1, w, errorvari
            
            ErrorCodeBlocks(blocknumb) = errorvari
            
            blocknumb = blocknumb + 1
         Next w
        End If

z = 91649 'start of track 18/1

' get diskname
For i = (z + &H90 - 256) To (z + &H9F - 256)
  Get #1, i, BITE
  sdiskname = sdiskname & Chr(BITE)
Next i


hitanA0 = 0
firstTime = 0
tempvar = 0
   displayDiskName = sdiskname
     For u = 1 To 16
        If Mid(displayDiskName, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole diskname is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If
        
        displayDiskName = Left(displayDiskName, hitanA0 - 1)

  txtDiskName.Text = displayDiskName


' get disk ID
For i = (z + &HA2 - 256) To (z + &HA6 - 256)
  Get #1, i, BITE
  sdiskID = sdiskID & Chr(BITE)
  txtDiskID.Text = sdiskID
Next i

' get blocks free
  d = 91397 'first byte of BAM on track 18/0
 
  For e = 1 To 35
    Get #1, d, BITE   ' Amount of free sectors on each track
    d = d + 4
    
    ' add up the free blocks, but don't count the directory track
    If e <> 18 Then
      totalblocks = totalblocks + Val(BITE)
    End If
  Next e
 
 lblBlocksFree.Caption = totalblocks & " BLOCKS FREE."

' 18 possible directory sectors used
For t = 1 To 18
Y = 0
' get the next track/sector of the directory (nextdirtrack unused currently)
Get #1, z, nextDirTrack
Get #1, z + 1, nextDirSector

' loop through each directory sector 8 times
For c = 0 To 7

  sfilename(b) = ""

' grab the filename

For i = (z + &H5 + Y) To (z + &H14 + Y)
Get #1, i, BITE
sfilename(b) = sfilename(b) & Chr(BITE)
Next i

' grab the starting track
Get #1, z + &H3 + Y, BITE
sstartingtrack(b) = BITE

'grab the starting sector
Get #1, z + &H4 + Y, BITE
sstartingsector(b) = BITE

'grab the file size
Get #1, z + &H1E + Y, BITE
sfilesize1(b) = BITE
Get #1, z + &H1F + Y, BITE
sfilesize2(b) = BITE

sfilesizeDisplay(b) = Val(sfilesize1(b)) + Val((sfilesize2(b) * 256))

'get file type
Get #1, z + &H2 + Y, BITE
sfiletype(b) = BITE
Call convDecToBin(BITE)
tstring = strBin
Call GETFILETYPE(tstring)
sfiletypedisplay = FILECLOSED & FTYPE & FILELOCKED

' check for last file of the directory
'If sstartingtrack(b) = 0 And sstartingsector(b) = 0 And sfilesize1(b) = 0 And sfiletype(b) = 0 And Left(sfilename(b), 1) = Chr$(0) Then
 ' Close #1
  'Call READBAM

'  sfilename(b) = ""
 ' Call getBlockOwners
  'Exit Sub
'End If

' check for scratched files
If sfiletype(b) = 0 Then
   sfiletypedisplay = "SCR"
End If

tstring = ""

' grab all the GEOS and REL related stuff
Get #1, z + &H15 + Y, BITE
byte15(b) = BITE

Get #1, z + &H16 + Y, BITE
byte16(b) = BITE

Get #1, z + &H17 + Y, BITE
byte17(b) = BITE

Get #1, z + &H18 + Y, BITE
byte18(b) = BITE

Get #1, z + &H19 + Y, BITE
byte19(b) = BITE

Get #1, z + &H1A + Y, BITE
byte1A(b) = BITE

Get #1, z + &H1B + Y, BITE
byte1B(b) = BITE

Get #1, z + &H1C + Y, BITE
byte1C(b) = BITE

Get #1, z + &H1D + Y, BITE
byte1D(b) = BITE

Y = Y + &H20

extraspaces = ""

hitanA0 = 0
firstTime = 0
tempvar = 0
   displayfilename = sfilename(b)
     For u = 1 To 16
        If Mid(displayfilename, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If
        
        displayfilename = Left(displayfilename, hitanA0 - 1)
     
     If Len(displayfilename) < 16 Then
        For u = Len(displayfilename) To 15
             extraspaces = extraspaces & " "
        Next u
     End If

   If sstartingtrack(b) = 0 And sstartingsector(b) = 0 And sfilesize1(b) = 0 And sfiletype(b) = 0 And Left(sfilename(b), 1) = Chr$(0) Then
   ' do nothing
   Else
     List1.AddItem Chr$(34) & displayfilename & Chr$(34) & extraspaces & vbTab & sstartingtrack(b) & vbTab & sstartingsector(b) & vbTab & sfilesizeDisplay(b) & vbTab & sfiletypedisplay
     b = b + 1
   End If
    extraspaces = ""
  
Next c



 ' check to see if this is the last directory sector
If nextDirSector = "0" Or nextDirTrack = "0" Then
   Close #1
   Call READBAM
   Call getBlockOwners
   Call updateFileCount
   Exit Sub
End If

z = z + (256 * (nextDirSector - curDirSector))
curDirSector = nextDirSector

Next t
Close #1

Call READBAM
Call getBlockOwners
Call updateFileCount

errorhandler:
  MsgBox "Directory appears to be corrupted." & vbCrLf & "It is recommended to NOT use this program with this file.", vbInformation, "Corrupted Directory?"
  frmMain.mnuSaveFile.Enabled = False
  frmMain.cmdSave.Enabled = False
  frmMain.List1.Enabled = False
  frmMain.txtDiskID.Enabled = False
  frmMain.txtDiskName.Enabled = False
  Close #1
  Exit Sub

End Sub


Private Sub List1_dblClick()
  Dim tstring As String
  
  ' Check for SEQ file, if so, fire up the SEQ Viewer
  Call convDecToBin(sfiletype(List1.ListIndex))
  tstring = strBin
  Call GETFILETYPE(tstring)
   
  If FTYPE = "SEQ" Then
     mnuViewFile_Click
  Else
     mnuModifyFileProperties_Click
  End If

End Sub




Private Sub mnuScratchFile_Click()
   For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          ScratchFile (i)
      End If
  
   Next i

End Sub


Private Sub ScratchFile(listvar As Integer)
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
   Dim hitanA0 As Integer
Dim firstTime As Integer
Dim tempvar As Integer
Dim blksFreeCounter As Integer
   Dim t As Integer
   Dim b As Integer
   Dim blocksFreeCounter As Integer
   Dim tpart1ToBite As Byte
   Dim tpart2ToBite As Byte
   Dim tpart3ToBite As Byte
   Dim blocks2Bite As Byte
   Dim totalBlocksFreeCounter As Integer
   
   If listvar = -1 Then
    MsgBox "Highlight the files you want to scratch.", vbExclamation, "No Files Selected"
    Exit Sub
  End If
   
   
  Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim block2Mark As Integer
   Dim x As Integer
   
   D64Modified = True
   
   x = 1
   bitespersector = 256
   blksFreeCounter = Val(frmMain.lblBlocksFree.Caption)
   
        Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)
      
      If nxtTrack = 18 Then
         
         
        If MsgBox("WARNING!!!!!!!!" & vbCrLf & "Scratching this file is going to unallocate blocks on the directory track!" & vbCrLf & "Are you sure you want to do this?", vbYesNo, "Wipe File?") = vbNo Then
          Close #1
          Exit Sub
        End If
      End If
         

      
      For p = 1 To 700
      
      If nxtTrack = 0 Then
        Close #1
        Call updateFileCount
          frmMain.lblBlocksFree = blksFreeCounter & " BLOCKS FREE."
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
 
    If blocksAllocated(block2Mark) <> 0 Then
      blksFreeCounter = blksFreeCounter + 1
    End If
      
    blocksAllocated(block2Mark) = 0

      
      


 Next p
      Close #1


   sfiletype(listvar) = 0
   
   sfiletypedisplay = "SCR"
   b = listvar
   
extraspaces = ""

hitanA0 = 0
firstTime = 0
tempvar = 0
   displayfilename = sfilename(b)
     For u = 1 To 16
        If Mid(displayfilename, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If
        
        displayfilename = Left(displayfilename, hitanA0 - 1)
     
     If Len(displayfilename) < 16 Then
        For u = Len(displayfilename) To 15
             extraspaces = extraspaces & " "
        Next u
     End If
   
   
   List1.RemoveItem b
   List1.AddItem Chr$(34) & displayfilename & Chr$(34) & extraspaces & vbTab & sstartingtrack(b) & vbTab & sstartingsector(b) & vbTab & sfilesizeDisplay(b) & vbTab & sfiletypedisplay, b
   extraspaces = ""
   List1.ListIndex = b
   Call updateFileCount
          frmMain.lblBlocksFree = blksFreeCounter & " BLOCKS FREE."

End Sub


Private Sub mnuSelectAllFiles_Click()
   For i = 0 To List1.ListCount - 1
       List1.Selected(i) = True
   Next i
   
End Sub



Private Sub mnuTrimFile_Click()
   
Dim FileNameHolder As String
Dim newFileName As String
Dim Filename As String

   If dialogopen.Filename <> "" Then
      MsgBox "You cannot do this while a D64 is open", vbExclamation, "Close D64 First"
      Exit Sub
   End If

   MsgBox "!!!!!USE WITH CAUTION!!!!!" & vbCrLf & "This function is used to trim a D64 file of a non-standard size down to 174848 bytes."

FileNameHolder = dialogopen.Filename
dialogopen.Filename = ""
dialogopen.InitDir = lastDirPath
dialogopen.Flags = &H1000 Or &H4 Or &H800
dialogopen.Filter = "C64 Disk Image (*.d64)|*.d64|C64 Sequential File (*.seq)|*.seq|All Files (*.*)|*.*"
dialogopen.ShowOpen

If dialogopen.Filename = "" Then
  If FileNameHolder <> "" Then
      dialogopen.Filename = FileNameHolder
  End If
  Exit Sub
End If
          
Filename = dialogopen.Filename

If FileLen(Filename) < 174849 Or FileLen(Filename) > 175530 Then
    MsgBox "This file doesn't need to be trimmed"
    dialogopen.Filename = ""
    Exit Sub
End If


Dim tempString As String

newFileName = Left(Filename, Len(Filename) - 4) & "_trimmed.d64"

Open Filename For Binary As #1
      Dim newFileLength As Long
      Dim varString1 As String
      
      newFileLength = 174848
      
      varString1 = String(newFileLength, " ")
      Get #1, 1, varString1
      
Open newFileName For Binary As #2
      Put #2, , varString1
Close #2

Close #1

dialogopen.Filename = ""

MsgBox "File trimmed and saved as:" & vbCrLf & newFileName


End Sub

Private Sub mnuUnscratchFile_Click()
   For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          UnscratchFile (i)
      End If
  
   Next i
End Sub


Private Sub UnscratchFile(listvar As Integer)
   
   
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
   Dim hitanA0 As Integer
Dim firstTime As Integer
Dim tempvar As Integer
Dim blksFreeCounter As Integer

   Dim t As Integer
   Dim b As Integer
   Dim blocksFreeCounter As Integer
   Dim tpart1ToBite As Byte
   Dim tpart2ToBite As Byte
   Dim tpart3ToBite As Byte
   Dim blocks2Bite As Byte
   Dim totalBlocksFreeCounter As Integer
   
  If listvar = -1 Then
    MsgBox "Highlight the files you want to unscratch.", vbExclamation, "No Files Selected"
    Exit Sub
  End If

  If sfiletype(listvar) <> 0 Then
'    MsgBox "That file is not scratched!", vbExclamation
    Exit Sub
  End If
   
  D64Modified = True
   
  Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim block2Mark As Integer
   Dim x As Integer
   
   x = 1
   bitespersector = 256
   blksFreeCounter = Val(frmMain.lblBlocksFree.Caption)
   
   
        Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)
      
      For p = 1 To 700
      
      If nxtTrack = 0 Then
        Close #1
          Call updateFileCount
          frmMain.lblBlocksFree = blksFreeCounter & " BLOCKS FREE."
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
 
  
    If blocksAllocated(block2Mark) <> 1 Then
      blksFreeCounter = blksFreeCounter - 1
    End If
 
      blocksAllocated(block2Mark) = 1


 Next p
      Close #1


   sfiletype(listvar) = CLng(&H82)
   
   sfiletypedisplay = "PRG"
   b = listvar
   
extraspaces = ""

hitanA0 = 0
firstTime = 0
tempvar = 0
   displayfilename = sfilename(b)
     For u = 1 To 16
        If Mid(displayfilename, u, 1) = " " Then
             If firstTime = 0 Then
               hitanA0 = u
               firstTime = 1
             End If
             tempvar = 1
        End If
        
        If tempvar = 0 Then
          
          If firstTime = 1 Then
             hitanA0 = 0
             firstTime = 0
          End If
        End If
        
        tempvar = 0
     Next u
        ' if $A0 ends up being 0 then the whole filename is used
        If hitanA0 = 0 Then
          hitanA0 = 17
        End If
        
        displayfilename = Left(displayfilename, hitanA0 - 1)
     
     If Len(displayfilename) < 16 Then
        For u = Len(displayfilename) To 15
             extraspaces = extraspaces & " "
        Next u
     End If
   
   
   List1.RemoveItem b
   List1.AddItem Chr$(34) & displayfilename & Chr$(34) & extraspaces & vbTab & sstartingtrack(b) & vbTab & sstartingsector(b) & vbTab & sfilesizeDisplay(b) & vbTab & sfiletypedisplay, b
   extraspaces = ""
   List1.ListIndex = b
   frmMain.lblBlocksFree = blksFreeCounter & " BLOCKS FREE."
   Call updateFileCount

End Sub

Private Sub mnuUnSelectAllFiles_Click()
   For i = 0 To List1.ListCount - 1
       List1.Selected(i) = False
   Next i
End Sub

'Private Sub mnuVersion_Click()
'   MsgBox "Version:    0.025.7" & vbCrLf & "Released: January 25, 2004" & vbCrLf & vbCrLf & "http://almighty.c64.org/d64editor.html" & vbCrLf & "forrest@almighty.c64.org" & vbCrLf & vbCrLf & "If you appreciate the time I've put into this program" & vbCrLf & "and would like to thank me with a monetary reward" & vbCrLf & "or something similar, please do :)" & vbCrLf & "I can always use additional PC and C64 hardware" & vbCrLf & "as well.", vbInformation
'End Sub


Private Sub mnuViewErrorBlocks_Click()
    Dim stringvari As String

   mnuEditBAM_Click
   frmBAM.Hide
      
   frmBAM.mnuCleanDisk.Visible = False
   frmBAM.mnuFile.Visible = False
   frmBAM.optBAMeditMode.Visible = True
   frmBAM.optBAMeditMode.Caption = "ERROR EDIT MODE"
   frmBAM.optBlockEditMode.Visible = True
   frmBAM.Caption = "Error Blocks Viewer"

   For i = 0 To 682
   
       stringvari = Hex(ErrorCodeBlocks(i))
   
       If Len(stringvari) < 2 Then
          stringvari = "0" & stringvari
       End If
    
       frmBAM.lblBAM(i) = stringvari
       frmBAM.lblBAM(i).Font = "MS Sans Serif"
             
       If stringvari <> "01" Then
          frmBAM.lblBAM(i).ForeColor = &HFF00FF
       End If
   Next i
    frmBAM.optBAMeditMode.Value = True
    frmBAM.Show

End Sub

Public Sub mnuViewFile_Click()

      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only view one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          ViewSeqFile (i)
          Exit Sub
      End If
  
   Next i
End Sub


Private Sub ViewSeqFile(listvar As Integer)
   Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim g As Long
   Dim BBTT(0 To 257) As Byte
   Dim tempString As String
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim tester As Integer
   Dim tempInt As Integer
   
   bitespersector = 256
 
If listvar = -1 Then
MsgBox "Highlight the SEQ file you want to view.", vbExclamation, "No File Selected"
  Exit Sub
End If
   frmMain.MousePointer = 11
   Load frmFileViewer
   
   frmFileViewer.Left = frmFileViewerLEFT
   frmFileViewer.Top = frmFileViewerTOP
   
   If FileViewerColumnMode = 1 Then
       frmFileViewer.opt80.Value = True
   Else
       frmFileViewer.opt40.Value = True
   End If
   
  frmFileViewer.lblFileName = sfilename(listvar)
       Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)
      
      For p = 1 To 700
      
      If nxtTrack = 0 Then
      

          For g = (Val(BITE2START + 2)) To (nxtSector + 3)
            Get #1, g, BBTT(g - BITE2START)
         Next g
         
          For g = 2 To nxtSector
             tempInt = BBTT(g)
             If BBTT(g) = CLng(&HD) Or BBTT(g) = CLng(&H0) Then
               tempString = tempString & vbCrLf
             Else
   
            If convertToASCII = 1 Then
            
               If tempInt >= 65 And tempInt <= 90 Then
                  tempInt = tempInt + 32
                  tester = 1
               End If
                     
              If tempInt >= 97 And tempInt <= 122 And tester = 0 Then
                  tempInt = tempInt - 32
                  tester = 1
              End If
               
                If tempInt >= 193 And tempInt <= 218 And tester = 0 Then
                  tempInt = tempInt - 128
               End If
            End If
               tempString = tempString & Chr(tempInt)
               
               tester = 0
            End If
          
          Next g
        Close #1
         
       
         
         
         frmFileViewer.text1 = tempString
         frmFileViewer.Show
         frmMain.MousePointer = 0
         Exit Sub
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

    BITE2START = BITE2START + 1
      

         Get #1, Val(BITE2START), trackbite
         Get #1, Val(BITE2START + 1), sectorbite
         
         nxtTrack = trackbite
         nxtSector = sectorbite
   
         For g = (Val(BITE2START + 2)) To (Val(BITE2START) + 255)
            Get #1, g, BBTT(g - BITE2START)
           If nxtTrack <> 0 Then
           tester = 0
                If BBTT(g - BITE2START) = CLng(&HD) Or BBTT(g - BITE2START) = CLng(&H0) Then
                 tempString = tempString & vbCrLf
                Else
    
                tempInt = BBTT(g - BITE2START)
                
             If convertToASCII = 1 Then
                
                If tempInt >= 65 And tempInt <= 90 Then
                  tempInt = tempInt + 32
                  tester = 1
                End If
                    
                If tempInt >= 97 And tempInt <= 122 And tester = 0 Then
                  tempInt = tempInt - 32
                  tester = 1
                End If
                    
                If tempInt >= 193 And tempInt <= 218 And tester = 0 Then
                  tempInt = tempInt - 128
                End If
             End If
                 tester = 0
                 tempString = tempString & Chr(tempInt)
                End If
           End If
         Next g

 Next p
      Close #1
      frmFileViewer.text1 = tempString
      frmFileViewer.Show
      frmMain.MousePointer = 0
End Sub

Private Sub mnuViewFileChain_Click()

      Dim mcounter As Integer
      
      For i = 0 To List1.ListCount - 1
          If List1.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only view one file at a time.", vbInformation, "Select one file"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To List1.ListCount - 1
      If List1.Selected(i) = True Then
          ViewFileChain (i)
          Exit Sub
      End If
  
   Next i
End Sub



Private Sub ViewFileChain(listvar As Integer)
   Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim block2Mark As Integer
   Dim x As Integer
   
   x = 1
   bitespersector = 256
   
  If listvar = -1 Then
    MsgBox "Highlight the file you want to view.", vbExclamation, "No File Selected"
    Exit Sub
  End If
   
    Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)

   If nxtTrack < 1 Or nxtTrack > 35 Then
       MsgBox "Invalid Track/Sector at starting block.", vbExclamation, "Invalid Track/Sector"
       Close #1
       Exit Sub
   End If
      
   If nxtTrack < 18 And nxtSector > 20 Then
       MsgBox "Invalid Track/Sector at starting block.", vbExclamation, "Invalid Track/Sector"
       Close #1
       Exit Sub
   End If
   
   If nxtTrack < 25 And nxtTrack > 17 And nxtSector > 18 Then
       MsgBox "Invalid Track/Sector at starting block.", vbExclamation, "Invalid Track/Sector"
       Close #1
       Exit Sub
   End If
   
   If nxtTrack < 31 And nxtTrack > 24 And nxtSector > 17 Then
       MsgBox "Invalid Track/Sector at starting block.", vbExclamation, "Invalid Track/Sector"
       Close #1
       Exit Sub
   End If
      
   If nxtTrack < 36 And nxtTrack > 30 And nxtSector > 16 Then
       MsgBox "Invalid Track/Sector at starting block.", vbExclamation, "Invalid Track/Sector"
       Close #1
       Exit Sub
   End If
      
   mnuEditBAM_Click
   frmBAM.Hide
   frmBAM.Show
      
   frmBAM.mnuCleanDisk.Visible = False
   frmBAM.mnuFile.Visible = False
   frmBAM.optBAMeditMode.Visible = False
   frmBAM.optBlockEditMode.Visible = False
   frmBAM.lblBlocks1.Visible = True
   frmBAM.lblBlocks2.Visible = True
   frmBAM.lblBlocks3.Visible = True
   frmBAM.Label2.Visible = True
   frmBAM.Label3.Visible = True
   frmBAM.Label4.Visible = True
   frmBAM.Label5.Visible = True
   frmBAM.picLess100.Visible = True
   frmBAM.picMore100.Visible = True
   frmBAM.picMore200.Visible = True
   frmBAM.picMore300.Visible = True
   frmBAM.picMore400.Visible = True
   frmBAM.picMore500.Visible = True
   frmBAM.picMore600.Visible = True
   frmBAM.mnuSave.Enabled = False
   frmBAM.Caption = "File Chain Viewer"
   frmBAM.Width = 10200
      
      For p = 1 To 700
      
      If nxtTrack = 0 Then
        Close #1
        frmBAM.Show
         Exit Sub
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
 
   If x < 100 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x
   End If
   
   If x > 99 And x < 200 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x - 100
      frmBAM.lblBAM(block2Mark).ForeColor = &HFFFF81
   End If
   
   If x > 199 And x < 300 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x - 200
      frmBAM.lblBAM(block2Mark).ForeColor = &H11FF11
   End If
   
   If x > 299 And x < 400 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x - 300
      frmBAM.lblBAM(block2Mark).ForeColor = &HFF00FF
   End If

   If x > 399 And x < 500 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x - 400
      frmBAM.lblBAM(block2Mark).ForeColor = &H80FFFF
   End If

   If x > 499 And x < 600 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x - 500
      frmBAM.lblBAM(block2Mark).ForeColor = &H80C0FF
   End If
   
   If x > 599 Then
      frmBAM.lblBAM(block2Mark).Font = "MS Sans Serif"
      frmBAM.lblBAM(block2Mark) = x - 600
      frmBAM.lblBAM(block2Mark).ForeColor = &HFFC0C0
   End If


   x = x + 1

 Next p
      Close #1

frmBAM.Show
  
End Sub

Private Sub mnuWipeFile_Click()
  Dim listholder As Integer
  
    listholder = List1.ListCount - 1
 
   For i = 0 To listholder
   
      If i > List1.ListCount - 1 Then
         Exit Sub
      End If

      If List1.Selected(i) = True Then
          WipeFile (i)
          If FileNotWiped = 0 Then
            listholder = listholder - 1
            i = i - 1
          End If
          
          FileNotWiped = 0
        
      End If
  
   Next i

End Sub

Private Sub WipeFile(listvar As Integer)
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
   Dim hitanA0 As Integer
Dim firstTime As Integer
Dim tempvar As Integer
Dim blksFreeCounter As Integer


   Dim t As Integer
   Dim b As Integer
   Dim blocksFreeCounter As Integer
   Dim tpart1ToBite As Byte
   Dim tpart2ToBite As Byte
   Dim tpart3ToBite As Byte
   Dim blocks2Bite As Byte
   Dim totalBlocksFreeCounter As Integer
   
  If listvar = -1 Then
    MsgBox "Highlight the files you want to wipe.", vbExclamation, "No Files Selected"
    Exit Sub
  End If
   
  Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim block2Mark As Integer
   Dim x As Integer
   
   D64Modified = True
   
   x = 1
   bitespersector = 256
   blksFreeCounter = Val(frmMain.lblBlocksFree.Caption)
   
   
        Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(listvar)
nxtSector = sstartingsector(listvar)
      
      If nxtTrack = 18 Then
         
        If MsgBox("WARNING!!!!!!!!" & vbCrLf & "Wiping this file is going to erase blocks on the directory track!" & vbCrLf & "Are you sure you want to do this?", vbYesNo, "Wipe File?") = vbNo Then
          FileNotWiped = 1
          Close #1
          Exit Sub
        End If
      End If
     
     
      For p = 1 To 700
      
      If nxtTrack = 0 Then
        Close #1
          frmMain.lblBlocksFree = blksFreeCounter & " BLOCKS FREE."
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
 
 
    If blocksAllocated(block2Mark) <> 0 Then
      blksFreeCounter = blksFreeCounter + 1
    End If
 
      blocksAllocated(block2Mark) = 0
      blockOwner(block2Mark) = ""
      blocksNeedingWiped(block2Mark) = 1
      

 Next p
      Close #1


Dim tstring As String
Dim tvar As Integer
Dim tempStr As String
Dim tempbyte As Byte
Dim tempInt As Integer


If listvar = -1 Then
MsgBox "Highlight the file you want to move.", vbExclamation, "No file selected"
  Exit Sub
End If

For x = 0 To List1.ListCount

t = listvar

tvar = listvar + 1
tstring = List1.Text



If listvar = List1.ListCount - 1 Then
  Exit For
End If


List1.AddItem List1.Text, listvar + 2
List1.RemoveItem listvar
listvar = tvar

  tempStr = sfilename(t)
  sfilename(t) = sfilename(t + 1)
  sfilename(t + 1) = tempStr

  tempInt = sstartingtrack(t)
  sstartingtrack(t) = sstartingtrack(t + 1)
  sstartingtrack(t + 1) = tempInt
  
  tempInt = sstartingsector(t)
  sstartingsector(t) = sstartingsector(t + 1)
  sstartingsector(t + 1) = tempInt
  
  tempInt = sfilesize1(t)
  sfilesize1(t) = sfilesize1(t + 1)
  sfilesize1(t + 1) = tempInt
  
  tempInt = sfilesize2(t)
  sfilesize2(t) = sfilesize2(t + 1)
  sfilesize2(t + 1) = tempInt
  
  tempInt = sfilesizeDisplay(t)
  sfilesizeDisplay(t) = sfilesizeDisplay(t + 1)
  sfilesizeDisplay(t + 1) = tempInt
  
  
  tempbyte = sfiletype(t)
  sfiletype(t) = sfiletype(t + 1)
  sfiletype(t + 1) = tempbyte
  
  tempbyte = byte15(t)
  byte15(t) = byte15(t + 1)
  byte15(t + 1) = tempbyte
  
  tempbyte = byte16(t)
  byte16(t) = byte16(t + 1)
  byte16(t + 1) = tempbyte
  
  tempbyte = byte17(t)
  byte17(t) = byte17(t + 1)
  byte17(t + 1) = tempbyte
  
  tempbyte = byte18(t)
  byte18(t) = byte18(t + 1)
  byte18(t + 1) = tempbyte

  tempbyte = byte19(t)
  byte19(t) = byte19(t + 1)
  byte19(t + 1) = tempbyte
  
  tempbyte = byte1A(t)
  byte1A(t) = byte1A(t + 1)
  byte1A(t + 1) = tempbyte
  
  tempbyte = byte1B(t)
  byte1B(t) = byte1B(t + 1)
  byte1B(t + 1) = tempbyte

  tempbyte = byte1C(t)
  byte1C(t) = byte1C(t + 1)
  byte1C(t + 1) = tempbyte
  
  tempbyte = byte1D(t)
  byte1D(t) = byte1D(t + 1)
  byte1D(t + 1) = tempbyte

  Next x

   List1.RemoveItem List1.ListCount - 1
   sfilename(t) = ""
   
   frmMain.lblBlocksFree = blksFreeCounter & " BLOCKS FREE."
   Call updateFileCount
    
End Sub
