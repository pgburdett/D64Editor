VERSION 5.00
Begin VB.Form frmFileProperties 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "File Properties"
   ClientHeight    =   3405
   ClientLeft      =   150
   ClientTop       =   795
   ClientWidth     =   7245
   Icon            =   "frmFileProperties.frx":0000
   LinkTopic       =   "Form2"
   MaxButton       =   0   'False
   ScaleHeight     =   3405
   ScaleWidth      =   7245
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdDiscardChanges 
      Caption         =   "Discard Changes"
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
      Left            =   4920
      TabIndex        =   12
      Top             =   2880
      Width           =   2055
   End
   Begin VB.CommandButton cmdSaveChanges 
      Caption         =   "Save Changes"
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
      Left            =   2640
      TabIndex        =   11
      Top             =   2880
      Width           =   2055
   End
   Begin VB.Frame Frame1 
      Caption         =   "FILE PROPERTIES"
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2055
      Left            =   240
      TabIndex        =   13
      Top             =   120
      Width           =   6735
      Begin VB.CheckBox chkSplat 
         Caption         =   "SPLAT (*)"
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
         Left            =   3000
         TabIndex        =   19
         Top             =   1200
         Width           =   1455
      End
      Begin VB.CommandButton cmdShowFilenameBuilder 
         Caption         =   "Filename Builder"
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
         Left            =   5400
         TabIndex        =   10
         Top             =   240
         Width           =   1215
      End
      Begin VB.TextBox txtStartingTrack 
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
         Height          =   285
         Left            =   2235
         MaxLength       =   2
         TabIndex        =   2
         Top             =   870
         Width           =   495
      End
      Begin VB.TextBox txtStartingSector 
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
         Height          =   285
         Left            =   2235
         MaxLength       =   2
         TabIndex        =   3
         Top             =   1190
         Width           =   495
      End
      Begin VB.TextBox txtFileSize 
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
         Height          =   285
         Left            =   2235
         MaxLength       =   5
         TabIndex        =   1
         Top             =   560
         Width           =   735
      End
      Begin VB.TextBox txtFilename 
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
         Height          =   285
         Left            =   2235
         MaxLength       =   16
         TabIndex        =   0
         Top             =   240
         Width           =   2055
      End
      Begin VB.CheckBox chkLocked 
         Caption         =   "LOCKED (<)"
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
         Left            =   3000
         TabIndex        =   4
         Top             =   840
         Width           =   1815
      End
      Begin VB.OptionButton optDel 
         Caption         =   "DEL"
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   5640
         TabIndex        =   9
         Top             =   1515
         Width           =   735
      End
      Begin VB.OptionButton optPRG 
         Caption         =   "PRG"
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   2280
         TabIndex        =   5
         Top             =   1515
         Width           =   735
      End
      Begin VB.OptionButton optSeq 
         Caption         =   "SEQ"
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   3120
         TabIndex        =   6
         Top             =   1515
         Width           =   735
      End
      Begin VB.OptionButton optUsr 
         Caption         =   "USR"
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   4800
         TabIndex        =   8
         Top             =   1515
         Width           =   735
      End
      Begin VB.OptionButton optRel 
         Caption         =   "REL"
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   3960
         TabIndex        =   7
         Top             =   1515
         Width           =   735
      End
      Begin VB.Label Label5 
         Caption         =   "FILE TYPE:"
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
         Left            =   960
         TabIndex        =   18
         Top             =   1640
         Width           =   1215
      End
      Begin VB.Label Label4 
         Caption         =   "STARTING TRACK:"
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
         Left            =   360
         TabIndex        =   17
         Top             =   960
         Width           =   1815
      End
      Begin VB.Label Label3 
         Caption         =   "STARTING SECTOR:"
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
         Left            =   240
         TabIndex        =   16
         Top             =   1280
         Width           =   1935
      End
      Begin VB.Label Label2 
         Caption         =   "SIZE:"
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
         Left            =   1560
         TabIndex        =   15
         Top             =   645
         Width           =   615
      End
      Begin VB.Label Label1 
         Caption         =   "FILENAME:"
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
         Left            =   1080
         TabIndex        =   14
         Top             =   315
         Width           =   1095
      End
   End
   Begin VB.Label Label6 
      Caption         =   "    Note:  Setting File Type to *DEL is the same             designation as a ""scratched"" file (SCR)."
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
      Left            =   360
      TabIndex        =   20
      Top             =   2280
      Width           =   6495
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuSaveChanges 
         Caption         =   "Save Changes"
      End
      Begin VB.Menu mnuSelectNextFileUponSave 
         Caption         =   "Select Next File Upon Saving Changes"
      End
      Begin VB.Menu mnuDiscardChanges 
         Caption         =   "Discard Changes"
      End
   End
End
Attribute VB_Name = "frmFileProperties"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmFileProperties.frm
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





Private Sub cmdDiscardChanges_Click()
    mnuDiscardChanges_Click
End Sub

Private Sub cmdSaveChanges_Click()
    mnuSaveChanges_Click
End Sub

Private Sub cmdShowFilenameBuilder_Click()
    Load frmFilenameBuilder
    frmFilenameBuilder.txtNameBuilder.Text = frmFileProperties.txtFilename.Text
    frmFileProperties.Enabled = False
    frmFilenameBuilder.cmdDiscardChanges.Visible = True
    frmFilenameBuilder.cmdApplyChanges.Visible = True
    frmFilenameBuilder.Width = 4170
    FileNameBuilderSTR = "FILEPROPERTIES"
    frmFilenameBuilder.Show
End Sub

Private Sub Form_Load()
frmFileProperties.Top = frmFilePropertiesTOP
frmFileProperties.Left = frmFilePropertiesLEFT
End Sub

Private Sub Form_Unload(Cancel As Integer)
  frmFilePropertiesTOP = frmFileProperties.Top
  frmFilePropertiesLEFT = frmFileProperties.Left
  frmMain.Enabled = True
  frmMain.List1.SetFocus
End Sub

Private Sub mnuDiscardChanges_Click()
  frmFilePropertiesTOP = frmFileProperties.Top
  frmFilePropertiesLEFT = frmFileProperties.Left
  frmMain.Enabled = True
  frmMain.List1.SetFocus
  Unload Me
End Sub

Private Sub mnuSelectNextFileUponSave_Click()
    If mnuSelectNextFileUponSave.Checked = True Then
         mnuSelectNextFileUponSave.Checked = False
         SelectNextFileUponSaveVar = 0
    Else
         mnuSelectNextFileUponSave.Checked = True
         SelectNextFileUponSaveVar = 1
    End If

End Sub

Private Sub mnuSaveChanges_Click()
   Dim tempfilet As String
   Dim tstring As String
   Dim nstring As String
   Dim vint As Integer
   Dim displayfilename As String
   Dim extraspaces As String
   Dim tempInt As Double
   
    If Val(txtStartingTrack) > 35 Or Val(txtStartingTrack) < 0 _
    Or Val(txtStartingSector) > 20 Or Val(txtStartingSector) < 0 Then
       MsgBox "Invalid Starting Track or Starting Sector"
       Exit Sub
    End If
   
   If Val(txtStartingTrack) < 18 And Val(txtStartingSector) > 20 Then
       MsgBox "Invalid Track/Sector selection"
       Exit Sub
   End If
   
   If Val(txtStartingTrack) > 17 And Val(txtStartingTrack) < 25 And Val(txtStartingSector) > 18 Then
       MsgBox "Invalid Track/Sector selection"
       Exit Sub
   End If
   
   If Val(txtStartingTrack) > 24 And Val(txtStartingTrack) < 31 And Val(txtStartingSector) > 17 Then
       MsgBox "Invalid Track/Sector selection"
       Exit Sub
   End If
   
   If Val(txtStartingTrack) > 30 And Val(txtStartingTrack) < 36 And Val(txtStartingSector) > 16 Then
       MsgBox "Invalid Track/Sector selection"
       Exit Sub
   End If

   If Val(txtFileSize) > 65535 Then
       MsgBox "Filesize cannot be larger than 65535."
       Exit Sub
   End If
   
   If Val(txtFileSize) < 0 Then
       MsgBox "Filesize cannot be less than 0."
       Exit Sub
   End If
    
      tempfilet = "1"
    
   If frmFileProperties.chkSplat.Value = Checked Then
      tempfilet = "0"
   End If
      
   D64Modified = True

' pad the filename with $A0's
For w = Len(txtFilename.Text) To 16

' there's an $A0 inside the quotes below
txtFilename.Text = txtFilename.Text & " "

Next w
   
   If chkLocked.Value = Checked Then
      tempfilet = tempfilet & "1000"
   Else
      tempfilet = tempfilet & "0000"
   End If
   
   If optPRG = True Then
        tempfilet = tempfilet & "010"
   End If
   
   If optRel = True Then
        tempfilet = tempfilet & "100"
   End If
   
   If optDel = True Then
        tempfilet = tempfilet & "000"
   End If
   
   If optUsr = True Then
        tempfilet = tempfilet & "011"
   End If
    
   If optSeq = True Then
        tempfilet = tempfilet & "001"
    End If

   Call convBinToLong(tempfilet)
     '  ltemp is the binary value back to long
    
    sfiletype(DirEntryBeingEdited) = Str(ltemp)
    
    sfilename(DirEntryBeingEdited) = frmFileProperties.txtFilename
      
Call convDecToBin(ltemp)
tstring = strBin
Call GETFILETYPE(tstring)
sfiletypedisplay = FILECLOSED & FTYPE & FILELOCKED

If sfiletype(DirEntryBeingEdited) = 0 Then
   sfiletypedisplay = "SCR"
End If

extraspaces = ""

   displayfilename = sfilename(DirEntryBeingEdited)

     For u = 1 To 16
        If Mid(displayfilename, u, 1) = " " Then
             displayfilename = Left(displayfilename, u - 1)
             Exit For
        End If
     Next u
     
     If Len(displayfilename) < 16 Then
        For u = Len(displayfilename) To 15
             extraspaces = extraspaces & " "
        Next u
     End If
        
    sstartingtrack(DirEntryBeingEdited) = Val(txtStartingTrack.Text)
    sstartingsector(DirEntryBeingEdited) = Val(txtStartingSector.Text)
    sfilesizeDisplay(DirEntryBeingEdited) = Val(txtFileSize.Text)




      tempInt = Val(txtFileSize.Text) / 256
      
      If tempInt < 10 Then
         tempInt = Val(Left(tempInt, 1))
      End If
      
      If tempInt > 9 And tempInt < 100 Then
         tempInt = Val(Left(tempInt, 2))
      End If
      
      If tempInt > 100 Then
         tempInt = Val(Left(tempInt, 3))
      End If
      
      
         sfilesize2(DirEntryBeingEdited) = tempInt
         sfilesize1(DirEntryBeingEdited) = Val(txtFileSize.Text) - (sfilesize2(DirEntryBeingEdited) * 256)
     
    nstring = Chr$(34) & displayfilename & Chr$(34) & extraspaces & vbTab & sstartingtrack(DirEntryBeingEdited) & vbTab & sstartingsector(DirEntryBeingEdited) & vbTab & sfilesizeDisplay(DirEntryBeingEdited) & vbTab & sfiletypedisplay
    extraspaces = ""
    
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
   
   
        Open frmMain.dialogopen.Filename For Binary As #1

nxtTrack = sstartingtrack(DirEntryBeingEdited)
nxtSector = sstartingsector(DirEntryBeingEdited)
      
      For p = 1 To 256
      
      If nxtTrack = 0 Then
        Close #1
vint = DirEntryBeingEdited
frmMain.List1.RemoveItem vint
frmMain.List1.AddItem nstring, vint
    
  If SelectNextFileUponSaveVar = 1 And frmMain.List1.ListIndex + 1 < frmMain.List1.ListCount - 1 Then
     frmMain.List1.Selected(vint + 1) = True
  Else
     frmMain.List1.Selected(vint) = True
  End If
    
    
 tempfilet = ""
   frmFilePropertiesTOP = frmFileProperties.Top
  frmFilePropertiesLEFT = frmFileProperties.Left
 frmMain.Enabled = True
 
 Call updateFileCount
 Unload Me
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
 
      blockOwner(block2Mark) = frmFileProperties.txtFilename
 Next p
      Close #1
 
    
    
    
vint = DirEntryBeingEdited
frmMain.List1.RemoveItem vint
frmMain.List1.AddItem nstring, vint
    
  If SelectNextFileUponSaveVar = 1 And frmMain.List1.ListIndex + 1 < frmMain.List1.ListCount - 1 Then
     frmMain.List1.Selected(vint + 1) = True
  Else
     frmMain.List1.Selected(vint) = True
  End If
  
 tempfilet = ""
   frmFilePropertiesTOP = frmFileProperties.Top
  frmFilePropertiesLEFT = frmFileProperties.Left
 frmMain.Enabled = True
 
  
 Call updateFileCount
 Unload Me
    
End Sub



Private Sub optDel_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub

Private Sub optPRG_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If

    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub


Private Sub optRel_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If

    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub

Private Sub optSeq_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub


Private Sub optUsr_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub

Private Sub txtFilename_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If

    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub


Private Sub txtFileSize_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub


Private Sub txtStartingSector_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub

Private Sub txtStartingTrack_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub

Private Sub chkLocked_KeyPress(Keyascii As Integer)
    If Keyascii = vbKeyReturn Then
       mnuSaveChanges_Click
    End If
    
    If Keyascii = vbKeyEscape Then
       mnuDiscardChanges_Click
    End If
End Sub
