VERSION 5.00
Begin VB.Form frmAbout 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "About the D64 Editor"
   ClientHeight    =   5025
   ClientLeft      =   -15
   ClientTop       =   270
   ClientWidth     =   5640
   Icon            =   "frmAbout.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   MousePointer    =   3  'I-Beam
   Picture         =   "frmAbout.frx":030A
   ScaleHeight     =   5025
   ScaleWidth      =   5640
   StartUpPosition =   3  'Windows Default
   Begin VB.PictureBox Picture1 
      Height          =   5145
      Left            =   0
      Picture         =   "frmAbout.frx":8A25
      ScaleHeight     =   5027.468
      ScaleMode       =   0  'User
      ScaleWidth      =   5713.906
      TabIndex        =   0
      Top             =   0
      Width           =   5715
   End
End
Attribute VB_Name = "frmAbout"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmAbout.frm
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





Private Sub Form_Load()
 ' Text1.ForeColor = RGB(62, 49, 162)
  'Text1.BackColor = RGB(124, 112, 218)
 ' frmAbout.BackColor = RGB(124, 112, 218)

End Sub

Private Sub Picture1_Click()
ShellExecute Me.hWnd, "open", "http://www.d64editor.com/", vbNullString, "", 0
 
End Sub
