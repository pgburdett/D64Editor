VERSION 5.00
Begin VB.Form frmErrorNumbers 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Error Numbers"
   ClientHeight    =   6540
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   8415
   Icon            =   "frmErrorNumbers.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   6540
   ScaleWidth      =   8415
   StartUpPosition =   3  'Windows Default
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
      Left            =   3240
      TabIndex        =   14
      Top             =   6000
      Width           =   1695
   End
   Begin VB.Frame Frame1 
      BorderStyle     =   0  'None
      Height          =   5535
      Left            =   240
      TabIndex        =   0
      Top             =   120
      Width           =   7935
      Begin VB.OptionButton errorCode 
         Caption         =   "0F    74    READ   Drive Not Ready"
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
         Index           =   14
         Left            =   2040
         TabIndex        =   19
         Top             =   7080
         Visible         =   0   'False
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "0F    74    READ   Drive Not Ready"
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
         Index           =   13
         Left            =   1200
         TabIndex        =   18
         Top             =   6720
         Visible         =   0   'False
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "0F    74    READ   Drive Not Ready"
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
         Index           =   12
         Left            =   1080
         TabIndex        =   17
         Top             =   6480
         Visible         =   0   'False
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "0F    74    READ   Drive Not Ready"
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
         Index           =   0
         Left            =   1320
         TabIndex        =   16
         Top             =   6240
         Visible         =   0   'False
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "0F    74    READ   Drive Not Ready"
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
         Index           =   15
         Left            =   600
         TabIndex        =   13
         Top             =   5160
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "0B    29    SEEK   Disk sector ID mismatch"
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
         Index           =   11
         Left            =   600
         TabIndex        =   12
         Top             =   4800
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "0A    28    WRITE  Write error"
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
         Index           =   10
         Left            =   600
         TabIndex        =   11
         Top             =   4440
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "09    27    SEEK   Checksum error in header block"
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
         Index           =   9
         Left            =   600
         TabIndex        =   10
         Top             =   4080
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "08    26    WRITE  Write protect on"
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
         Index           =   8
         Left            =   600
         TabIndex        =   9
         Top             =   3720
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "07    25    WRITE  Write verify error"
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
         Index           =   7
         Left            =   600
         TabIndex        =   8
         Top             =   3360
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "06    24    READ   Byte decoding error"
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
         Index           =   6
         Left            =   600
         TabIndex        =   7
         Top             =   3000
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "05    23    READ   Checksum error in data block"
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
         Index           =   5
         Left            =   600
         TabIndex        =   6
         Top             =   2640
         Width           =   7215
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "04    22    READ   Data descriptor byte not found ($07)"
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
         Index           =   4
         Left            =   600
         TabIndex        =   5
         Top             =   2280
         Width           =   7335
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "03    21    R/W    No sync sequence found"
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
         Index           =   3
         Left            =   600
         TabIndex        =   4
         Top             =   1920
         Width           =   6375
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "02    20    READ   Header descriptor byte not found ($08)"
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
         Index           =   2
         Left            =   600
         TabIndex        =   3
         Top             =   1560
         Width           =   7335
      End
      Begin VB.OptionButton errorCode 
         Caption         =   "01    00    N/A    No Error"
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
         Index           =   1
         Left            =   600
         TabIndex        =   1
         Top             =   1200
         Width           =   5295
      End
      Begin VB.Label Label1 
         Caption         =   "Code  Error  Type   1541 error description"
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
         Left            =   720
         TabIndex        =   2
         Top             =   720
         Width           =   5415
      End
   End
   Begin VB.Label lblBlockNumber 
      Caption         =   "Label2"
      Height          =   495
      Left            =   8400
      TabIndex        =   15
      Top             =   480
      Visible         =   0   'False
      Width           =   495
   End
End
Attribute VB_Name = "frmErrorNumbers"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmErrorNumbers.frm
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

Private Sub cmdSaveChanges_Click()

    Dim v As Byte
    Dim i As Integer

    For i = 1 To 15
    
          If frmErrorNumbers.errorCode(i) = True Then
            frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).Caption = Hex(i)
            v = i
            
            If i = 10 Then
               v = &HA
            End If
            
            If i = 11 Then
               v = &HB
            End If
            
            If i = 15 Then
               v = &HF
            End If
            
            ErrorCodeBlocks(frmErrorNumbers.lblBlockNumber) = v
              If Len(frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).Caption) < 2 Then
                 frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).Caption = "0" & frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).Caption
              
                 If frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).Caption <> "01" Then
                     frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).ForeColor = &H11FF11

'                     frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).ForeColor = &H0
                 Else
                     frmBAM.lblBAM(frmErrorNumbers.lblBlockNumber).ForeColor = &HFFFFFF
                 End If
              
              End If
          End If

       
    Next i
  
   D64Modified = True

   Unload Me

End Sub
