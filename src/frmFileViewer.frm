VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Object = "{3B7C8863-D78F-101B-B9B5-04021C009402}#1.2#0"; "Richtx32.ocx"
Begin VB.Form frmFileViewer 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "SEQ File Viewer"
   ClientHeight    =   6660
   ClientLeft      =   150
   ClientTop       =   795
   ClientWidth     =   10200
   Icon            =   "frmFileViewer.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   6660
   ScaleWidth      =   10200
   StartUpPosition =   3  'Windows Default
   Begin VB.Frame Frame2 
      BorderStyle     =   0  'None
      Height          =   375
      Left            =   120
      TabIndex        =   7
      Top             =   720
      Width           =   3255
      Begin VB.OptionButton OptASCII 
         Caption         =   "ASCII"
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
         Left            =   1680
         TabIndex        =   9
         Top             =   120
         Width           =   1335
      End
      Begin VB.OptionButton OptPETASCII 
         Caption         =   "PETASCII"
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
         Left            =   0
         TabIndex        =   8
         Top             =   120
         Width           =   1455
      End
   End
   Begin MSComDlg.CommonDialog commonDialogPrint 
      Left            =   8760
      Top             =   360
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin RichTextLib.RichTextBox text1 
      Height          =   5295
      Left            =   120
      TabIndex        =   6
      Top             =   1200
      Width           =   9975
      _ExtentX        =   17595
      _ExtentY        =   9340
      _Version        =   393217
      ScrollBars      =   2
      MaxLength       =   3000000
      TextRTF         =   $"frmFileViewer.frx":030A
   End
   Begin MSComDlg.CommonDialog CommonDialog1 
      Left            =   9600
      Top             =   360
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.Frame Frame1 
      BorderStyle     =   0  'None
      Height          =   375
      Left            =   0
      TabIndex        =   2
      Top             =   360
      Width           =   3375
      Begin VB.OptionButton opt80 
         Caption         =   "80 Column"
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
         Left            =   1800
         TabIndex        =   3
         Top             =   120
         Width           =   1455
      End
      Begin VB.OptionButton opt40 
         Caption         =   "40 Column"
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
         Left            =   120
         TabIndex        =   0
         Top             =   120
         Value           =   -1  'True
         Width           =   1455
      End
   End
   Begin VB.Label Label2 
      Caption         =   "Character Set -------------"
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
      Left            =   3480
      TabIndex        =   5
      Top             =   120
      Width           =   2415
   End
   Begin VB.Label Label1 
      Caption         =   "Label1"
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
      Height          =   240
      Left            =   3600
      TabIndex        =   4
      Top             =   480
      Width           =   1335
   End
   Begin VB.Label lblFileName 
      Caption         =   "Label1"
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
      Left            =   120
      TabIndex        =   1
      Top             =   120
      Width           =   9975
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuSaveToText 
         Caption         =   "&Save To Text"
         Shortcut        =   ^S
      End
      Begin VB.Menu mnuPrint 
         Caption         =   "&Print"
         Shortcut        =   ^P
         Visible         =   0   'False
      End
   End
End
Attribute VB_Name = "frmFileViewer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmFileViewer.frm
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




Private Sub mnuPrint_Click()
On Error GoTo errorhandler
    
    commonDialogPrint.CancelError = True
    commonDialogPrint.ShowPrinter
    Printer.Print frmFileViewer.text1.Text
    Printer.EndDoc
    Exit Sub
errorhandler:
    Exit Sub
End Sub

Private Sub mnuSaveToText_Click()
      CommonDialog1.Filename = ""
      CommonDialog1.Flags = &H2 Or &H4 Or &H800 ' Check to see if the file already exists
      CommonDialog1.DefaultExt = "txt"
      CommonDialog1.Filter = "Text Document (*.txt)|*.txt|All Files (*.*)|*.*"
      CommonDialog1.ShowSave
      
      If CommonDialog1.Filename = "" Then
         Exit Sub
      End If
      
      Open CommonDialog1.Filename For Output As #1
         Print #1, frmFileViewer.text1.Text
      Close #1


End Sub

Private Sub Form_Load()

   If convertToASCII = 1 Then
       frmFileViewer.text1.Font = "CBM64"
       frmFileViewer.Label1.Caption = "ASCII"
       frmFileViewer.OptASCII = True
   Else
       frmFileViewer.text1.Font = "SEQ VIEWER (C64)"
       frmFileViewer.Label1.Caption = "PETASCII"
       frmFileViewer.OptPETASCII = True
   End If


   If opt40.Value = True Then
    With frmFileViewer
        .text1.Height = 5295
        .text1.Width = 5175
        .Height = 7020
        .Width = 5505
    End With
   End If

   If opt80.Value = True Then
    With frmFileViewer
        .text1.Height = 5295
        .text1.Width = 9975
        .Height = 7020
        .Width = 10335
    End With
   End If
End Sub

Private Sub Form_Unload(Cancel As Integer)
   frmFileViewerLEFT = frmFileViewer.Left
   frmFileViewerTOP = frmFileViewer.Top
   
     ' 0 if in 40 column mode
   If opt40.Value = True Then
       FileViewerColumnMode = 0
   End If
   
     ' 1 if in 80 column mode
   If opt80.Value = True Then
       FileViewerColumnMode = 1
   End If
   
End Sub



Private Sub opt40_Click()
    With frmFileViewer
        .text1.Height = 5295
        .text1.Width = 5175
        .Height = 7020
        .Width = 5505
    End With
    FileViewerColumnMode = 0
End Sub

Private Sub opt80_Click()
    With frmFileViewer
        .text1.Height = 5295
        .text1.Width = 9975
        .Height = 7020
        .Width = 10335
    End With
    FileViewerColumnMode = 1
End Sub


Private Sub OptASCII_Click()
       ' if ASCII mode is already selected, do nothing
    If convertToASCII = 1 Then
      Exit Sub
    End If
    
    ' Switch to ASCII and reload the SEQ file
    convertToASCII = 1
    reloadSEQfile
    frmFileViewer.text1.Font = "CBM64"
    frmFileViewer.Label1.Caption = "ASCII"
End Sub

Private Sub OptPETASCII_Click()
       ' if PETASCII mode is already selected, do nothing
    If convertToASCII = 0 Then
      Exit Sub
    End If
    
    ' Switch to PETASCII and reload the SEQ file
    convertToASCII = 0
    reloadSEQfile
    frmFileViewer.text1.Font = "SEQ VIEWER (C64)"
    frmFileViewer.Label1.Caption = "PETASCII"
End Sub
