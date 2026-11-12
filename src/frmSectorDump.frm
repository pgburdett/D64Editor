VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Begin VB.Form frmSectorDump 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Sector Dumper"
   ClientHeight    =   2280
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   4680
   Icon            =   "frmSectorDump.frx":0000
   LinkTopic       =   "Form3"
   MaxButton       =   0   'False
   ScaleHeight     =   2280
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin MSComDlg.CommonDialog dialog2 
      Left            =   4080
      Top             =   2040
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.Frame Frame1 
      Height          =   1935
      Left            =   360
      TabIndex        =   5
      Top             =   120
      Width           =   3975
      Begin VB.CommandButton cmdCancel 
         Caption         =   "CANCEL"
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
         TabIndex        =   3
         Top             =   1320
         Width           =   975
      End
      Begin VB.CommandButton cmdDump 
         Caption         =   "DUMP"
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
         Left            =   720
         TabIndex        =   2
         Top             =   1320
         Width           =   1095
      End
      Begin VB.ComboBox cmbSector 
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   240
         Left            =   960
         Style           =   2  'Dropdown List
         TabIndex        =   1
         Top             =   720
         Width           =   855
      End
      Begin VB.ComboBox cmbTrack 
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   240
         Left            =   960
         Style           =   2  'Dropdown List
         TabIndex        =   0
         Top             =   240
         Width           =   855
      End
      Begin VB.OptionButton optHex 
         Caption         =   "HEX"
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
         Left            =   2520
         TabIndex        =   6
         Top             =   600
         Width           =   975
      End
      Begin VB.OptionButton optDecimal 
         Caption         =   "DECIMAL"
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
         Left            =   2520
         TabIndex        =   4
         Top             =   240
         Width           =   1215
      End
      Begin VB.Label Label2 
         Caption         =   "SECTOR:"
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
         Left            =   110
         TabIndex        =   8
         Top             =   780
         Width           =   855
      End
      Begin VB.Label Label1 
         Caption         =   "TRACK:"
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
         Left            =   220
         TabIndex        =   7
         Top             =   320
         Width           =   735
      End
   End
End
Attribute VB_Name = "frmSectorDump"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmSectorDump.frm
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

Private Sub cmbTrack_Click()

    If cmbTrack = "" Then
        cmbSector.Clear
        Exit Sub
    End If

    If optDecimal = True Then
           
       If cmbTrack < 18 Then
           cmbSector.Clear
           
           For w = 0 To 20
           
              If Len(w) < 2 Then
                cmbSector.AddItem "0" & w
              Else
                cmbSector.AddItem w
              End If
           Next w
       End If
       
       If cmbTrack > 17 And cmbTrack < 25 Then
           cmbSector.Clear
           
           For w = 0 To 18
              If Len(w) < 2 Then
                cmbSector.AddItem "0" & w
              Else
                cmbSector.AddItem w
              End If
           Next w
       End If
       
       If cmbTrack > 24 And cmbTrack < 31 Then
           cmbSector.Clear
       
          For w = 0 To 17
              If Len(w) < 2 Then
                cmbSector.AddItem "0" & w
              Else
                cmbSector.AddItem w
              End If
          Next w
       End If
        
       If cmbTrack > 30 And cmbTrack < 36 Then
           cmbSector.Clear
           
           For w = 0 To 16
              If Len(w) < 2 Then
                cmbSector.AddItem "0" & w
              Else
                cmbSector.AddItem w
              End If
           Next w
       End If
    
    End If
    
    
    If optHex = True Then
    
       If cmbTrack.Text < Hex(18) Then
           cmbSector.Clear
           
           For w = 0 To 20
              If Len(Hex(w)) < 2 Then
                cmbSector.AddItem "0" & Hex(w)
              Else
                cmbSector.AddItem Hex(w)
              End If
           Next w
       End If
       
       If cmbTrack.Text > Hex(17) And cmbTrack.Text < Hex(25) Then
           cmbSector.Clear
           
           For w = 0 To 18
              If Len(Hex(w)) < 2 Then
                cmbSector.AddItem "0" & Hex(w)
              Else
                cmbSector.AddItem Hex(w)
              End If
           Next w
       End If
       
       If cmbTrack.Text > Hex(24) And cmbTrack.Text < Hex(31) Then
           cmbSector.Clear
       
          For w = 0 To 17
              If Len(Hex(w)) < 2 Then
                cmbSector.AddItem "0" & Hex(w)
              Else
                cmbSector.AddItem Hex(w)
              End If
          Next w
       End If
        
       If cmbTrack.Text > Hex(30) And cmbTrack.Text < Hex(36) Then
           cmbSector.Clear
           
           For w = 0 To 16
              If Len(Hex(w)) < 2 Then
                cmbSector.AddItem "0" & Hex(w)
              Else
                cmbSector.AddItem Hex(w)
              End If

           Next w
       End If
    
    
    
    End If
    
End Sub

Private Sub cmdCancel_Click()
   frmMain.Enabled = True
  frmSectorDumpTOP = frmSectorDump.Top
  frmSectorDumpLEFT = frmSectorDump.Left
   Unload Me
End Sub


Private Sub cmdDump_Click()
    Dim bite2dump As Long
    Dim bitespersector As Long
    bitespersector = 256
    Dim g As Long
    Dim BBTT(0 To 256) As Byte

    If cmbTrack.Text = "" Or cmbSector.Text = "" Then
       MsgBox "Please select Track/Sector"
       Exit Sub
    End If


    If optDecimal = True Then

      If cmbTrack.Text < 18 Then
          bite2dump = ((Val(cmbTrack.Text - 1) * 21) * 256) + (Val(cmbSector.Text) * 256)
      End If

      If cmbTrack.Text > 17 And cmbTrack.Text < 25 Then
          bite2dump = (((Val(cmbTrack.Text) - 18) * 19) * 256) + (bitespersector * 17 * 21) + (Val(cmbSector.Text) * 256)
      End If

      If cmbTrack.Text > 24 And cmbTrack.Text < 31 Then
          bite2dump = (((Val(cmbTrack.Text) - 25) * 18) * 256) + (bitespersector * 17 * 21) + (bitespersector * 7 * 19) + (Val(cmbSector.Text) * 256)
      End If
     
      If cmbTrack.Text > 30 And cmbTrack.Text < 36 Then
          bite2dump = (((Val(cmbTrack.Text) - 31) * 17) * 256) + (bitespersector * 17 * 21) + (bitespersector * 7 * 19) + (bitespersector * 6 * 18) + (Val(cmbSector.Text) * 256)
      End If

    End If

    If optHex = True Then
    
      If cmbTrack.Text < Hex(18) Then
          bite2dump = ((CLng("&H" & (cmbTrack.Text - 1)) * 21) * 256) + (CLng("&H" & (cmbSector.Text)) * 256)
      End If

      If cmbTrack.Text > Hex(17) And cmbTrack.Text < Hex(25) Then
          bite2dump = (((CLng("&H" & (cmbTrack.Text)) - 18) * 19) * 256) + (bitespersector * 17 * 21) + (CLng("&H" & (cmbSector.Text)) * 256)
      End If
      If cmbTrack.Text > Hex(24) And cmbTrack.Text < Hex(31) Then
          bite2dump = (((CLng("&H" & (cmbTrack.Text)) - 25) * 18) * 256) + (bitespersector * 17 * 21) + (bitespersector * 7 * 19) + (CLng("&H" & (cmbSector.Text)) * 256)
      End If
     
      If cmbTrack.Text > Hex(30) And cmbTrack.Text < Hex(36) Then
          bite2dump = (((CLng("&H" & (cmbTrack.Text)) - 31) * 17) * 256) + (bitespersector * 17 * 21) + (bitespersector * 7 * 19) + (bitespersector * 6 * 18) + (CLng("&H" & (cmbSector.Text)) * 256)
      End If
       
    End If

      
    bite2dump = bite2dump + 1
      Open frmMain.dialogopen.Filename For Binary As #1
     
            
    
         For g = (Val(bite2dump)) To (Val(bite2dump) + 255)
            Get #1, g, BBTT(g - bite2dump)
         Next g
    
    Close #1
      frmSectorDump.dialog2.Filename = ""
      frmSectorDump.dialog2.Flags = &H2 Or &H800 Or &H4 ' Check to see if the file already exists
      frmSectorDump.dialog2.Filter = "All Files (*.*)|*.*"
      frmSectorDump.dialog2.ShowSave
      
      If dialog2.Filename = "" Then
         Exit Sub
      End If
      
      Open frmSectorDump.dialog2.Filename For Binary As #2
      
        For g = 0 To 255
            Put #2, , BBTT(g)
        Next g

      Close #2

MsgBox "Block dumped into " & dialog2.Filename
  frmSectorDumpTOP = frmSectorDump.Top
  frmSectorDumpLEFT = frmSectorDump.Left
    frmMain.Enabled = True
    Unload Me
End Sub


Private Sub Form_Load()
frmSectorDump.Top = frmSectorDumpTOP
frmSectorDump.Left = frmSectorDumpLEFT
End Sub

Private Sub Form_Unload(Cancel As Integer)
  frmSectorDumpTOP = frmSectorDump.Top
  frmSectorDumpLEFT = frmSectorDump.Left
  frmMain.Enabled = True
End Sub

Private Sub optDecimal_Click()

    cmbTrack.Clear
    cmbSector.Clear
    For w = 1 To 35
    
      If Len(w) < 2 Then
        frmSectorDump.cmbTrack.AddItem "0" & w
      Else
        frmSectorDump.cmbTrack.AddItem w
      End If
    Next w
End Sub

Private Sub optHex_Click()
    cmbTrack.Clear
    cmbSector.Clear
    For w = 1 To 35
    
      If Len(Hex(w)) < 2 Then
         frmSectorDump.cmbTrack.AddItem "0" & Hex(w)
      Else
         frmSectorDump.cmbTrack.AddItem Hex(w)
      End If
    Next w
    
End Sub
