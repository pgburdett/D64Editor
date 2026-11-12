VERSION 5.00
Begin VB.Form frmConfig 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Configuration"
   ClientHeight    =   6660
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   7725
   Icon            =   "frmConfig.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   6660
   ScaleWidth      =   7725
   StartUpPosition =   3  'Windows Default
   Begin VB.TextBox txtImportStartingSector 
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   285
      Left            =   360
      TabIndex        =   11
      Top             =   4560
      Width           =   495
   End
   Begin VB.TextBox txtImportStartingTrack 
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   285
      Left            =   360
      TabIndex        =   10
      Top             =   4200
      Width           =   495
   End
   Begin VB.TextBox txtInterleave 
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   285
      Left            =   360
      TabIndex        =   9
      Top             =   3840
      Width           =   495
   End
   Begin VB.CheckBox checkTreatTrack18FilesAsEmpty 
      Caption         =   "Treat Track 18/0-1 starting files as empty when copying"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   360
      TabIndex        =   7
      Top             =   2280
      Width           =   6975
   End
   Begin VB.CheckBox checkConvertToASCII 
      Caption         =   "Convert SEQ file from PETASCII->ASCII"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   360
      TabIndex        =   6
      Top             =   1800
      Width           =   5415
   End
   Begin VB.TextBox txtSeperatorLine 
      Alignment       =   2  'Center
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   3720
      MaxLength       =   16
      TabIndex        =   3
      Text            =   "================"
      Top             =   120
      Visible         =   0   'False
      Width           =   2055
   End
   Begin VB.CommandButton cmdEnableChanges 
      Caption         =   "Enable Changes"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   855
      Left            =   2640
      TabIndex        =   4
      Top             =   5280
      Width           =   2415
   End
   Begin VB.CheckBox checkWarningMessages 
      Caption         =   "Disable Most Warning Messages"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   360
      TabIndex        =   2
      Top             =   1320
      Width           =   5415
   End
   Begin VB.CheckBox checkEnableSafeDiskClean 
      Caption         =   "Enable Safe Disk Clean (Skips Track 18)"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   360
      TabIndex        =   1
      Top             =   860
      Width           =   5295
   End
   Begin VB.CheckBox checkDisableCrosslinkCheck 
      Caption         =   "Disable Crosslinked Files Check"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   360
      TabIndex        =   0
      Top             =   360
      Width           =   5295
   End
   Begin VB.Label Label3 
      Caption         =   "Advanced Settings"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H000000FF&
      Height          =   255
      Left            =   2760
      TabIndex        =   15
      Top             =   3120
      Width           =   2055
   End
   Begin VB.Label Label2 
      Caption         =   "Do not change unless you have a specific reason to do so!"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H000000FF&
      Height          =   255
      Left            =   480
      TabIndex        =   14
      Top             =   3360
      Width           =   6975
   End
   Begin VB.Label lblStartingSector 
      Caption         =   "Import Starting Sector (Default 0)"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   960
      TabIndex        =   13
      Top             =   4605
      Width           =   4455
   End
   Begin VB.Label lblStartingTrack 
      Caption         =   "Import Starting Track (Default 17)"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   960
      TabIndex        =   12
      Top             =   4245
      Width           =   4335
   End
   Begin VB.Label lblInterleave 
      Caption         =   "Interleave (Default is 10, set to 15 or less)"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   960
      TabIndex        =   8
      Top             =   3900
      Width           =   6015
   End
   Begin VB.Label Label1 
      Caption         =   "Seperator Line Text"
      Height          =   255
      Left            =   5880
      TabIndex        =   5
      Top             =   165
      Visible         =   0   'False
      Width           =   1575
   End
End
Attribute VB_Name = "frmConfig"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmConfig.frm
'
'  Written by
'  Forrest Mook <forrest@almighty.c64.org>
'  Copyright 2000-2016
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


Private Sub cmdEnableChanges_Click()
    
    On Error GoTo errorhandler
    Dim startingtracktemp As Integer
    Dim startingsectortemp As Integer
    Dim tempinterleave As Integer
    
    ' use these to error out the subroutine if a non-number is entered
    startingtracktemp = txtImportStartingTrack.Text
    startingsectortemp = txtImportStartingSector.Text
    tempinterleave = txtInterleave.Text
    
    If startingtracktemp < 1 Or startingtracktemp > 35 Then
        MsgBox "Please choose a whole number between 1 and 35 for the starting track.", vbCritical
        Exit Sub
    End If
    
    If startingsectortemp < 0 Or startingsectortemp > 21 Then
        MsgBox "Please choose a whole number between 0 and 20 for a starting sector.", vbCritical
        Exit Sub
    End If
    
    If startingtracktemp >= 19 And startingtracktemp <= 24 And startingsectortemp > 18 Then
        MsgBox "Invalid sector for that starting track.", vbCritical
        Exit Sub
    End If
    
    If startingtracktemp >= 25 And startingtracktemp <= 30 And startingsectortemp > 17 Then
        MsgBox "Invalid sector for that starting track.", vbCritical
        Exit Sub
    End If
    
    If startingtracktemp >= 31 And startingtracktemp <= 35 And startingsectortemp > 16 Then
        MsgBox "Invalid sector for that starting track.", vbCritical
        Exit Sub
    End If
    
    If Len(txtInterleave.Text) > 2 Then
        MsgBox "Please choose a whole number less than 15 for an interleave.", vbCritical
        Exit Sub
    Else
        InterLeave = txtInterleave.Text
    End If
 
    If Len(txtImportStartingSector.Text) > 2 Then
        MsgBox "Please choose a whole number between 0 and 20 for a starting sector.", vbCritical
        Exit Sub
    Else
        UserSpecifiedFirstSector = txtImportStartingSector.Text
    End If
 
     If Len(txtImportStartingTrack.Text) > 2 Then
        MsgBox "Please choose a whole number between 1 and 35 for a starting track.", vbCritical
        Exit Sub
    Else
        UserSpecifiedFirstTrack = txtImportStartingTrack.Text
    End If
 
    If InterLeave = 0 Or InterLeave > 15 Then
        MsgBox "Invalid value for Interleave, setting to default 10.", vbInformation
        InterLeave = 10
    End If
    

    
    If frmConfig.checkDisableCrosslinkCheck.Value = Checked Then
        disableCrosslinkCheck = 1
    Else
        disableCrosslinkCheck = 0
    End If
    
    If frmConfig.checkEnableSafeDiskClean.Value = Checked Then
        safeClean = 1
    Else
        safeClean = 0
    End If
    
    If frmConfig.checkWarningMessages.Value = Checked Then
        warningsDisabled = 1
    Else
        warningsDisabled = 0
    End If
    
    If frmConfig.checkConvertToASCII.Value = Checked Then
        convertToASCII = 1
    Else
        convertToASCII = 0
    End If
    
    If frmConfig.checkTreatTrack18FilesAsEmpty.Value = Checked Then
        treatTrack18FilesAsEmpty = 1
    Else
        treatTrack18FilesAsEmpty = 0
    End If
 

    
    
 
    seperatorLineText = frmConfig.txtSeperatorLine.Text
    
    Unload Me
   
Exit Sub

errorhandler:
    Unload Me
   
End Sub



Private Sub Form_Unload(Cancel As Integer)
  frmConfigTOP = frmConfig.Top
  frmConfigLEFT = frmConfig.Left
End Sub


