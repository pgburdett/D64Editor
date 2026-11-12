VERSION 5.00
Begin VB.Form frmMainCreateNewD64 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "frmMainCreateNewD64.Caption"
   ClientHeight    =   3915
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   3795
   Icon            =   "frmMainCreateNewD64.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3915
   ScaleWidth      =   3795
   StartUpPosition =   1  'CenterOwner
   Begin VB.Frame frameCreateD64 
      Height          =   3735
      Left            =   120
      TabIndex        =   0
      Top             =   0
      Width           =   3495
      Begin VB.OptionButton opt40TrackPlusErrors 
         Caption         =   "40 Track + Errors D64"
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
         Height          =   315
         Left            =   120
         TabIndex        =   13
         Top             =   3240
         Width           =   3135
      End
      Begin VB.OptionButton opt35TrackPlusErrors 
         Caption         =   "35 Track + Errors D64"
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
         Height          =   315
         Left            =   120
         TabIndex        =   12
         Top             =   2520
         Width           =   3015
      End
      Begin VB.OptionButton opt40Track 
         Caption         =   "40 Track D64 (extended)"
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
         Height          =   315
         Left            =   120
         TabIndex        =   11
         Top             =   2880
         Width           =   3135
      End
      Begin VB.OptionButton opt35Track 
         Caption         =   "35 Track D64 (normal)"
         BeginProperty Font 
            Name            =   "CBM64"
            Size            =   6.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H0000C000&
         Height          =   315
         Left            =   120
         TabIndex        =   10
         Top             =   2160
         Width           =   3135
      End
      Begin VB.CheckBox chkProtectSector 
         Caption         =   "Protect Boot Sector"
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   480
         TabIndex        =   9
         Top             =   1680
         Visible         =   0   'False
         Width           =   2175
      End
      Begin VB.TextBox txtAutobootFilename 
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "CBM-Directory-Charset/ck!"
            Size            =   6
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   285
         Left            =   480
         MaxLength       =   16
         TabIndex        =   7
         Text            =   "MENU"
         Top             =   1320
         Width           =   2055
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
         Height          =   285
         Left            =   2160
         MaxLength       =   5
         TabIndex        =   4
         Text            =   "00 2A"
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
         Height          =   285
         Left            =   120
         MaxLength       =   16
         TabIndex        =   3
         Text            =   "NEW DISK"
         Top             =   360
         Width           =   2055
      End
      Begin VB.CommandButton cmdCreateNewD64 
         Height          =   360
         Left            =   2520
         Style           =   1  'Graphical
         TabIndex        =   2
         Top             =   720
         Width           =   360
      End
      Begin VB.CheckBox chkC128Boot 
         Caption         =   "C128 Auto-boot Sector"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   120
         TabIndex        =   1
         Top             =   720
         Width           =   2295
      End
      Begin VB.Label lblAbootFN 
         Caption         =   "Auto-boot Filename"
         Enabled         =   0   'False
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   480
         TabIndex        =   8
         Top             =   1080
         Width           =   1695
      End
      Begin VB.Label lblDiskID 
         Caption         =   "Disk ID"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   2160
         TabIndex        =   6
         Top             =   120
         Width           =   735
      End
      Begin VB.Label lblDiskName 
         Caption         =   "Disk Name"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   120
         TabIndex        =   5
         Top             =   120
         Width           =   1215
      End
   End
   Begin VB.Line lineGray 
      BorderColor     =   &H00808080&
      X1              =   4080
      X2              =   4080
      Y1              =   360
      Y2              =   840
   End
   Begin VB.Line lineBlack 
      BorderColor     =   &H00000000&
      X1              =   3960
      X2              =   3960
      Y1              =   360
      Y2              =   840
   End
End
Attribute VB_Name = "frmMainCreateNewD64"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmMainCreateNewD64.frm
'
'  Written by
'  Felix Vazquez <al@p2pchat.net>
'  Copyright 2003-2016
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
'
'  Changelog
'       15 Dec 06 - Completed Code for C128 Auto Boot Sector Generation
'           - Modified frmMain, frmMainCreateNewD64, frmBlockViewer and frmBAM
'           - Auto Boot Code generated from manual disassembly of code
'             created by aboot128.prg [Unknown Author]
'           - Not tested on REAL C128.. Just WinVICE


Private Sub chkC128Boot_Click()

    Dim x As Integer

    If (chkC128Boot.Value = 1) Then
        Me.lblAbootFN.Enabled = True
        Me.txtAutobootFilename.Enabled = True
        Me.chkProtectSector.Enabled = True
        Me.chkProtectSector.Value = Checked
        For x = 0 To 2
        '    Me.lineBoot(x).BorderColor = Me.lineBlack.BorderColor
        Next x
    Else
        Me.lblAbootFN.Enabled = False
        Me.txtAutobootFilename.Enabled = False
        Me.chkProtectSector.Enabled = False
        Me.chkProtectSector.Value = Unchecked
        For x = 0 To 2
         '   Me.lineBoot(x).BorderColor = Me.lineGray.BorderColor
        Next x
    End If

End Sub

Private Sub cmdCreateNewD64_Click()

    ' 15 Dec 06 - C128 Auto-Boot Code Check

    If (chkC128Boot.Value = 1) Then
        Call frmMain.CreateBlankD64(txtDiskName.Text, txtDiskID.Text, True)
    Else
        Call frmMain.CreateBlankD64(txtDiskName.Text, txtDiskID.Text, False)
    End If
  
    Unload Me
End Sub

Private Sub Form_Load()
 frmMainCreateNewD64.Icon = frmMain.Icon
 frmMainCreateNewD64.Caption = "Create New D64"
 frmMainCreateNewD64.Top = frmMainCreateNewD64TOP
 frmMainCreateNewD64.Left = frmMainCreateNewD64LEFT
 cmdCreateNewD64.Picture = frmMain.cmdSave.Picture
End Sub

Private Sub Form_Unload(Cancel As Integer)
 
 frmMainCreateNewD64TOP = frmMainCreateNewD64.Top
 frmMainCreateNewD64LEFT = frmMainCreateNewD64.Left
 frmMain.Enabled = True
 frmMain.Show

End Sub

Private Sub opt35TrackPlusErrors_Click()
    MsgBox "This is not a normal disk image.  Do not create this unless you have a specific need!", vbExclamation
End Sub

Private Sub opt40Track_Click()
    MsgBox "This is not a normal disk image.  Do not create this unless you have a specific need!", vbExclamation
End Sub

Private Sub opt40TrackPlusErrors_Click()
    MsgBox "This is not a normal disk image.  Do not create this unless you have a specific need!", vbExclamation
End Sub
