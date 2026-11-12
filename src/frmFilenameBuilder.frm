VERSION 5.00
Begin VB.Form frmFilenameBuilder 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Filename Builder"
   ClientHeight    =   5445
   ClientLeft      =   150
   ClientTop       =   795
   ClientWidth     =   6885
   Icon            =   "frmFilenameBuilder.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   5445
   ScaleWidth      =   6885
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdDelete 
      Height          =   360
      Left            =   5760
      Picture         =   "frmFilenameBuilder.frx":030A
      Style           =   1  'Graphical
      TabIndex        =   265
      ToolTipText     =   "Delete Separator"
      Top             =   120
      Width           =   360
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
      Left            =   5040
      Picture         =   "frmFilenameBuilder.frx":036A
      Style           =   1  'Graphical
      TabIndex        =   264
      ToolTipText     =   "Move Separator Up"
      Top             =   120
      Width           =   360
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
      Left            =   5400
      Picture         =   "frmFilenameBuilder.frx":03D1
      Style           =   1  'Graphical
      TabIndex        =   263
      ToolTipText     =   "Move Separator Down"
      Top             =   120
      Width           =   345
   End
   Begin VB.CommandButton cmdAddSeparator 
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   360
      Left            =   4320
      Picture         =   "frmFilenameBuilder.frx":043A
      Style           =   1  'Graphical
      TabIndex        =   262
      ToolTipText     =   "Add Separator to the List"
      Top             =   120
      Width           =   360
   End
   Begin VB.CommandButton cmdSaveSeparatorsToFile 
      Height          =   360
      Left            =   4680
      Picture         =   "frmFilenameBuilder.frx":05AA
      Style           =   1  'Graphical
      TabIndex        =   261
      ToolTipText     =   "Save Separators"
      Top             =   120
      Width           =   360
   End
   Begin VB.ListBox listSeparators 
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
      Height          =   4740
      ItemData        =   "frmFilenameBuilder.frx":062A
      Left            =   4320
      List            =   "frmFilenameBuilder.frx":062C
      MultiSelect     =   2  'Extended
      TabIndex        =   260
      Top             =   600
      Width           =   2295
   End
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
      Height          =   615
      Left            =   2160
      TabIndex        =   1
      Top             =   3720
      Visible         =   0   'False
      Width           =   1455
   End
   Begin VB.CommandButton cmdApplyChanges 
      Caption         =   "Apply Changes"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   615
      Left            =   480
      TabIndex        =   0
      Top             =   3720
      Visible         =   0   'False
      Width           =   1575
   End
   Begin VB.TextBox txtNameBuilder 
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
      Left            =   1920
      MaxLength       =   16
      TabIndex        =   2
      Top             =   240
      Width           =   2055
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   8
      X1              =   1560
      X2              =   3960
      Y1              =   2280
      Y2              =   2280
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   7
      X1              =   1560
      X2              =   1560
      Y1              =   2520
      Y2              =   2280
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   6
      X1              =   1080
      X2              =   1560
      Y1              =   2520
      Y2              =   2520
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   5
      X1              =   1080
      X2              =   1080
      Y1              =   2760
      Y2              =   2520
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   4
      X1              =   600
      X2              =   1080
      Y1              =   2760
      Y2              =   2760
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   3
      X1              =   600
      X2              =   600
      Y1              =   2760
      Y2              =   3000
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   2
      X1              =   360
      X2              =   600
      Y1              =   3000
      Y2              =   3000
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   1
      X1              =   360
      X2              =   360
      Y1              =   3480
      Y2              =   3000
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   95
      Left            =   3240
      TabIndex        =   98
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   94
      Left            =   3000
      TabIndex        =   97
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   93
      Left            =   2760
      TabIndex        =   96
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   92
      Left            =   2520
      TabIndex        =   95
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   91
      Left            =   2280
      TabIndex        =   94
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   90
      Left            =   2040
      TabIndex        =   93
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   89
      Left            =   1800
      TabIndex        =   92
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   88
      Left            =   1560
      TabIndex        =   91
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   87
      Left            =   1320
      TabIndex        =   90
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   86
      Left            =   1080
      TabIndex        =   89
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   85
      Left            =   840
      TabIndex        =   88
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   84
      Left            =   600
      TabIndex        =   87
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   83
      Left            =   360
      TabIndex        =   86
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   82
      Left            =   3720
      TabIndex        =   85
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   81
      Left            =   3480
      TabIndex        =   84
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   80
      Left            =   3240
      TabIndex        =   83
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   79
      Left            =   3000
      TabIndex        =   82
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   78
      Left            =   2760
      TabIndex        =   81
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   77
      Left            =   2520
      TabIndex        =   80
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   76
      Left            =   2280
      TabIndex        =   79
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   75
      Left            =   2040
      TabIndex        =   78
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   74
      Left            =   1800
      TabIndex        =   77
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   73
      Left            =   1560
      TabIndex        =   76
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   72
      Left            =   1320
      TabIndex        =   75
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   71
      Left            =   1080
      TabIndex        =   74
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   70
      Left            =   840
      TabIndex        =   73
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   69
      Left            =   600
      TabIndex        =   72
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   68
      Left            =   360
      TabIndex        =   71
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   67
      Left            =   3720
      TabIndex        =   70
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   66
      Left            =   3480
      TabIndex        =   69
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   65
      Left            =   3240
      TabIndex        =   68
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   64
      Left            =   3000
      TabIndex        =   67
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   63
      Left            =   2760
      TabIndex        =   66
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   62
      Left            =   2520
      TabIndex        =   65
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   61
      Left            =   2280
      TabIndex        =   64
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   60
      Left            =   2040
      TabIndex        =   63
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   59
      Left            =   1800
      TabIndex        =   62
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   58
      Left            =   1560
      TabIndex        =   61
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   57
      Left            =   1320
      TabIndex        =   60
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   56
      Left            =   1080
      TabIndex        =   59
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   55
      Left            =   840
      TabIndex        =   58
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   54
      Left            =   600
      TabIndex        =   57
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   53
      Left            =   3720
      TabIndex        =   56
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   52
      Left            =   3480
      TabIndex        =   55
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   51
      Left            =   3240
      TabIndex        =   54
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   50
      Left            =   3000
      TabIndex        =   53
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   49
      Left            =   2760
      TabIndex        =   52
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   48
      Left            =   2520
      TabIndex        =   51
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   47
      Left            =   2280
      TabIndex        =   50
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   46
      Left            =   2040
      TabIndex        =   49
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   45
      Left            =   1800
      TabIndex        =   48
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   44
      Left            =   1560
      TabIndex        =   47
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   43
      Left            =   1320
      TabIndex        =   46
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   42
      Left            =   1080
      TabIndex        =   45
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   41
      Left            =   3720
      TabIndex        =   44
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   40
      Left            =   3480
      TabIndex        =   43
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   39
      Left            =   3240
      TabIndex        =   42
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   38
      Left            =   3000
      TabIndex        =   41
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   37
      Left            =   2760
      TabIndex        =   40
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   36
      Left            =   2520
      TabIndex        =   39
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   35
      Left            =   2280
      TabIndex        =   38
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   34
      Left            =   2040
      TabIndex        =   37
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   33
      Left            =   1800
      TabIndex        =   36
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   32
      Left            =   1560
      TabIndex        =   35
      Top             =   2280
      Width           =   255
   End
   Begin VB.Line Line2 
      BorderColor     =   &H000000FF&
      Index           =   9
      X1              =   120
      X2              =   3960
      Y1              =   1080
      Y2              =   1080
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   170
      Left            =   120
      TabIndex        =   173
      Top             =   3240
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   167
      Left            =   120
      TabIndex        =   170
      Top             =   3000
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   153
      Left            =   360
      TabIndex        =   156
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   121
      Left            =   120
      TabIndex        =   124
      Top             =   2760
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   187
      Left            =   840
      TabIndex        =   190
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   172
      Left            =   600
      TabIndex        =   175
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   136
      Left            =   360
      TabIndex        =   139
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   104
      Left            =   120
      TabIndex        =   107
      Top             =   2520
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   155
      Left            =   1320
      TabIndex        =   158
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   191
      Left            =   1080
      TabIndex        =   194
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   190
      Left            =   840
      TabIndex        =   193
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   188
      Left            =   600
      TabIndex        =   191
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   130
      Left            =   360
      TabIndex        =   133
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   98
      Left            =   120
      TabIndex        =   101
      Top             =   2280
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   181
      Left            =   3720
      TabIndex        =   184
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   185
      Left            =   3480
      TabIndex        =   188
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   162
      Left            =   3240
      TabIndex        =   165
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   184
      Left            =   3000
      TabIndex        =   187
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   110
      Left            =   2760
      TabIndex        =   113
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   118
      Left            =   2520
      TabIndex        =   121
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   109
      Left            =   2280
      TabIndex        =   112
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   159
      Left            =   2040
      TabIndex        =   162
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   169
      Left            =   1800
      TabIndex        =   172
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   127
      Left            =   1560
      TabIndex        =   130
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   150
      Left            =   1320
      TabIndex        =   153
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   189
      Left            =   1080
      TabIndex        =   192
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   177
      Left            =   840
      TabIndex        =   180
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   173
      Left            =   600
      TabIndex        =   176
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   157
      Left            =   360
      TabIndex        =   160
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   103
      Left            =   120
      TabIndex        =   106
      Top             =   2040
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   161
      Left            =   3720
      TabIndex        =   164
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   168
      Left            =   3480
      TabIndex        =   171
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   124
      Left            =   3240
      TabIndex        =   127
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   156
      Left            =   3000
      TabIndex        =   159
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   166
      Left            =   2760
      TabIndex        =   169
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   186
      Left            =   2520
      TabIndex        =   189
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   108
      Left            =   2280
      TabIndex        =   111
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   152
      Left            =   2040
      TabIndex        =   155
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   147
      Left            =   1800
      TabIndex        =   150
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   120
      Left            =   1560
      TabIndex        =   123
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   115
      Left            =   1320
      TabIndex        =   118
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   179
      Left            =   1080
      TabIndex        =   182
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   123
      Left            =   840
      TabIndex        =   126
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   171
      Left            =   600
      TabIndex        =   174
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   135
      Left            =   360
      TabIndex        =   138
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   116
      Left            =   120
      TabIndex        =   119
      Top             =   1800
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   182
      Left            =   3720
      TabIndex        =   185
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   160
      Left            =   3480
      TabIndex        =   163
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   143
      Left            =   3240
      TabIndex        =   146
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   144
      Left            =   3000
      TabIndex        =   147
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   140
      Left            =   2760
      TabIndex        =   143
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   112
      Left            =   2520
      TabIndex        =   115
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   111
      Left            =   2280
      TabIndex        =   114
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   154
      Left            =   2040
      TabIndex        =   157
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   129
      Left            =   1800
      TabIndex        =   132
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   122
      Left            =   1560
      TabIndex        =   125
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   97
      Left            =   1320
      TabIndex        =   100
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   174
      Left            =   1080
      TabIndex        =   177
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   178
      Left            =   840
      TabIndex        =   181
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   176
      Left            =   600
      TabIndex        =   179
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   148
      Left            =   360
      TabIndex        =   151
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   165
      Left            =   120
      TabIndex        =   168
      Top             =   1560
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   151
      Left            =   3720
      TabIndex        =   154
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   145
      Left            =   3480
      TabIndex        =   148
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   139
      Left            =   3240
      TabIndex        =   142
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   138
      Left            =   3000
      TabIndex        =   141
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   107
      Left            =   2760
      TabIndex        =   110
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   106
      Left            =   2520
      TabIndex        =   109
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   158
      Left            =   2280
      TabIndex        =   161
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   126
      Left            =   2040
      TabIndex        =   129
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   133
      Left            =   1800
      TabIndex        =   136
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   132
      Left            =   1560
      TabIndex        =   135
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   131
      Left            =   1320
      TabIndex        =   134
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   128
      Left            =   1080
      TabIndex        =   131
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   134
      Left            =   840
      TabIndex        =   137
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   146
      Left            =   600
      TabIndex        =   149
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   142
      Left            =   360
      TabIndex        =   145
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   180
      Left            =   120
      TabIndex        =   183
      Top             =   1320
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   119
      Left            =   3720
      TabIndex        =   122
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   113
      Left            =   3480
      TabIndex        =   116
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   137
      Left            =   3240
      TabIndex        =   140
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   149
      Left            =   3000
      TabIndex        =   152
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   105
      Left            =   2760
      TabIndex        =   108
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   117
      Left            =   2520
      TabIndex        =   120
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   163
      Left            =   2280
      TabIndex        =   166
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   183
      Left            =   2040
      TabIndex        =   186
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   101
      Left            =   1800
      TabIndex        =   104
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   100
      Left            =   1560
      TabIndex        =   103
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   99
      Left            =   1320
      TabIndex        =   102
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   102
      Left            =   1080
      TabIndex        =   105
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   114
      Left            =   840
      TabIndex        =   117
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   175
      Left            =   600
      TabIndex        =   178
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   164
      Left            =   360
      TabIndex        =   167
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   141
      Left            =   120
      TabIndex        =   144
      Top             =   1080
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   31
      Left            =   3720
      TabIndex        =   34
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   30
      Left            =   3480
      TabIndex        =   33
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   29
      Left            =   3240
      TabIndex        =   32
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   28
      Left            =   3000
      TabIndex        =   31
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   27
      Left            =   2760
      TabIndex        =   30
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   26
      Left            =   2520
      TabIndex        =   29
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   25
      Left            =   2280
      TabIndex        =   28
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   24
      Left            =   2040
      TabIndex        =   27
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   23
      Left            =   1800
      TabIndex        =   26
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   22
      Left            =   1560
      TabIndex        =   25
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   21
      Left            =   1320
      TabIndex        =   24
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   20
      Left            =   1080
      TabIndex        =   23
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   19
      Left            =   840
      TabIndex        =   22
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   18
      Left            =   600
      TabIndex        =   21
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   17
      Left            =   360
      TabIndex        =   20
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   16
      Left            =   120
      TabIndex        =   19
      Top             =   840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   15
      Left            =   3720
      TabIndex        =   3
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   14
      Left            =   3480
      TabIndex        =   4
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   13
      Left            =   3240
      TabIndex        =   5
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   12
      Left            =   3000
      TabIndex        =   6
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   11
      Left            =   2760
      TabIndex        =   7
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   10
      Left            =   2520
      TabIndex        =   8
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   9
      Left            =   2280
      TabIndex        =   9
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   8
      Left            =   2040
      TabIndex        =   10
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   7
      Left            =   1800
      TabIndex        =   11
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   6
      Left            =   1560
      TabIndex        =   12
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   5
      Left            =   1320
      TabIndex        =   13
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   4
      Left            =   1080
      TabIndex        =   14
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   3
      Left            =   840
      TabIndex        =   15
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   2
      Left            =   600
      TabIndex        =   16
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   1
      Left            =   360
      TabIndex        =   17
      Top             =   600
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   0
      Left            =   120
      TabIndex        =   18
      Top             =   600
      Width           =   255
   End
   Begin VB.Label Label1 
      Caption         =   "Filename:"
      BeginProperty Font 
         Name            =   "CBM64"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   135
      Left            =   720
      TabIndex        =   259
      Top             =   285
      Width           =   1095
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   255
      Left            =   3840
      TabIndex        =   258
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   254
      Left            =   3600
      TabIndex        =   257
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   253
      Left            =   3360
      TabIndex        =   256
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   252
      Left            =   3120
      TabIndex        =   255
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   251
      Left            =   2880
      TabIndex        =   254
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   250
      Left            =   2640
      TabIndex        =   253
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   249
      Left            =   2400
      TabIndex        =   252
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   248
      Left            =   2160
      TabIndex        =   251
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   247
      Left            =   1920
      TabIndex        =   250
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   246
      Left            =   1680
      TabIndex        =   249
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   245
      Left            =   1440
      TabIndex        =   248
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   244
      Left            =   1200
      TabIndex        =   247
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   243
      Left            =   960
      TabIndex        =   246
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   242
      Left            =   720
      TabIndex        =   245
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   241
      Left            =   480
      TabIndex        =   244
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   240
      Left            =   240
      TabIndex        =   243
      Top             =   7920
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   239
      Left            =   3840
      TabIndex        =   242
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   238
      Left            =   3600
      TabIndex        =   241
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   237
      Left            =   3360
      TabIndex        =   240
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   236
      Left            =   3120
      TabIndex        =   239
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   235
      Left            =   2880
      TabIndex        =   238
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   234
      Left            =   2640
      TabIndex        =   237
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   233
      Left            =   2400
      TabIndex        =   236
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   232
      Left            =   2160
      TabIndex        =   235
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   231
      Left            =   1920
      TabIndex        =   234
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   230
      Left            =   1680
      TabIndex        =   233
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   229
      Left            =   1440
      TabIndex        =   232
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   228
      Left            =   1200
      TabIndex        =   231
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   227
      Left            =   960
      TabIndex        =   230
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   226
      Left            =   720
      TabIndex        =   229
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   225
      Left            =   480
      TabIndex        =   228
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   224
      Left            =   240
      TabIndex        =   227
      Top             =   7680
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   223
      Left            =   3840
      TabIndex        =   226
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   222
      Left            =   3600
      TabIndex        =   225
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   221
      Left            =   3360
      TabIndex        =   224
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   220
      Left            =   3120
      TabIndex        =   223
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   219
      Left            =   2880
      TabIndex        =   222
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   218
      Left            =   2640
      TabIndex        =   221
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   217
      Left            =   2400
      TabIndex        =   220
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   216
      Left            =   2160
      TabIndex        =   219
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   215
      Left            =   1920
      TabIndex        =   218
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   214
      Left            =   1680
      TabIndex        =   217
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   213
      Left            =   1440
      TabIndex        =   216
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   212
      Left            =   1200
      TabIndex        =   215
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   211
      Left            =   960
      TabIndex        =   214
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   210
      Left            =   720
      TabIndex        =   213
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   209
      Left            =   480
      TabIndex        =   212
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   208
      Left            =   240
      TabIndex        =   211
      Top             =   7440
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   207
      Left            =   3840
      TabIndex        =   210
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   206
      Left            =   3600
      TabIndex        =   209
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   205
      Left            =   3360
      TabIndex        =   208
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   204
      Left            =   3120
      TabIndex        =   207
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   203
      Left            =   2880
      TabIndex        =   206
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   202
      Left            =   2640
      TabIndex        =   205
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   201
      Left            =   2400
      TabIndex        =   204
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   200
      Left            =   2160
      TabIndex        =   203
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   199
      Left            =   1920
      TabIndex        =   202
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   198
      Left            =   1680
      TabIndex        =   201
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   197
      Left            =   1440
      TabIndex        =   200
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   196
      Left            =   1200
      TabIndex        =   199
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   195
      Left            =   960
      TabIndex        =   198
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   194
      Left            =   720
      TabIndex        =   197
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   193
      Left            =   480
      TabIndex        =   196
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   192
      Left            =   240
      TabIndex        =   195
      Top             =   7200
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   125
      Left            =   480
      TabIndex        =   128
      Top             =   6840
      Width           =   255
   End
   Begin VB.Label DirChar 
      Alignment       =   2  'Center
      BackColor       =   &H00C00000&
      BorderStyle     =   1  'Fixed Single
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   255
      Index           =   96
      Left            =   960
      TabIndex        =   99
      Top             =   6840
      Width           =   255
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuSaveSeparatorstoFile 
         Caption         =   "Save Separators to File"
         Visible         =   0   'False
      End
      Begin VB.Menu mnuExit 
         Caption         =   "Exit"
      End
   End
   Begin VB.Menu mnuSeparators 
      Caption         =   "&Separators"
      Visible         =   0   'False
      Begin VB.Menu mnuCopySeparatorsintoD64 
         Caption         =   "Copy Separators into D64"
      End
      Begin VB.Menu mnuModifySeparator 
         Caption         =   "Modify Separator"
      End
      Begin VB.Menu mnuDeleteSeparator 
         Caption         =   "Delete Separator"
      End
      Begin VB.Menu mnuSelectAllFiles 
         Caption         =   "Select All Files"
      End
      Begin VB.Menu mnuUnSelectAllFiles 
         Caption         =   "UnSelect All Files"
      End
      Begin VB.Menu mnuInvertSelection 
         Caption         =   "Invert Selection"
      End
   End
End
Attribute VB_Name = "frmFilenameBuilder"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmFilenameBuilder.frm
'
'  Written by
'  Felix Vazquez <al@p2pchat.net>
'  Forrest Mook <forrest@almighty.c64.org>
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




Private Sub cmdAddSeparator_Click()
    
    If txtNameBuilder.Text <> "" Then
      listSeparators.AddItem txtNameBuilder.Text
      txtNameBuilder.Text = ""
    End If
End Sub

Private Sub cmdApplyChanges_Click()


    If FileNameBuilderSTR = "FILEPROPERTIES" Then
       frmFileProperties.txtFilename.Text = txtNameBuilder.Text
       frmFileProperties.Enabled = True
       FileNameBuilderSTR = ""
    End If
    Unload Me
End Sub

Private Sub cmdDelete_Click()
Call DeleteSeparator
End Sub

Private Sub cmdDiscardChanges_Click()
    If FileNameBuilderSTR = "FILEPROPERTIES" Then
        frmFileProperties.Enabled = True
        FileNameBuilderSTR = ""
    End If
  Unload Me
End Sub


Private Sub cmdSaveSeparatorsToFile_Click()

  Call WriteSeparatorsToFile

  MsgBox "The separators have been saved.", vbInformation, "Separators Saved"

End Sub



Function DeleteSeparator()
  Dim listholder As Integer
  
    listholder = listSeparators.ListCount - 1
 
   For i = 0 To listholder
   
      If i > listSeparators.ListCount - 1 Then
         Exit Function
      End If

      If listSeparators.Selected(i) = True Then
          listSeparators.RemoveItem (i)
          If FileNotWiped = 0 Then
            listholder = listholder - 1
            i = i - 1
          End If
          
          FileNotWiped = 0
        
      End If
  
   Next i

End Function



Private Sub cmdSortDown_Click()

      Dim mcounter As Integer
      
      For i = 0 To listSeparators.ListCount - 1
          If listSeparators.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only move one separator at a time.", vbInformation, "Select one separator"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To listSeparators.ListCount - 1
      If listSeparators.Selected(i) = True Then
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
MsgBox "Highlight the separator you want to move.", vbExclamation, "No separator selected"
  Exit Sub
End If

'D64Modified = True

t = listvar

tvar = listvar + 1
tstring = listSeparators.Text

If listvar = listSeparators.ListCount - 1 Then
  Exit Sub
End If
listSeparators.AddItem listSeparators.Text, listvar + 2
listSeparators.RemoveItem listvar
listSeparators.Selected(tvar) = True
listSeparators.Selected(tvar - 1) = False

End Sub

Private Sub cmdSortUp_Click()

      Dim mcounter As Integer
      
      For i = 0 To listSeparators.ListCount - 1
          If listSeparators.Selected(i) = True Then
              mcounter = mcounter + 1
          End If
          If mcounter > 1 Then
             MsgBox "You can only move one separator at a time.", vbInformation, "Select one separator"
             Exit Sub
          End If
          
      Next i
      
      For i = 0 To listSeparators.ListCount - 1
      If listSeparators.Selected(i) = True Then
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
MsgBox "Highlight the separator you want to move.", vbExclamation, "No separator selected"
  Exit Sub
End If

'D64Modified = True

t = listvar

tvar = listvar - 1
tstring = listSeparators.Text

If listvar = 0 Then
  Exit Sub
End If
listSeparators.AddItem listSeparators.Text, listvar - 1
listSeparators.RemoveItem listvar + 1
listSeparators.Selected(tvar) = True
listSeparators.Selected(tvar + 1) = False
  
End Sub


Private Sub DirChar_Click(Index As Integer)

    Dim CurPos As Integer

     CurPos = txtNameBuilder.SelStart

    If Len(txtNameBuilder.Text) < 16 Then
'       txtNameBuilder.Text = txtNameBuilder.Text & DirChar(Index).Caption
        txtNameBuilder.Text = Left(txtNameBuilder.Text, CurPos) & Left(DirChar(Index).Caption, 1) & Right(txtNameBuilder.Text, Len(txtNameBuilder.Text) - CurPos)
        txtNameBuilder.SelStart = CurPos + 1

    End If

End Sub


Private Sub Form_Load()

    frmFilenameBuilder.Left = frmFileNameBuilderLEFT
    frmFilenameBuilder.Top = frmFileNameBuilderTOP

    For i = 0 To 255
       DirChar(i).Caption = Chr(i)
     DirChar(i).ToolTipText = Str$(i)
    Next i
    ' fix to display character 38 correctly
    DirChar(38).Caption = "&&"
   readSeparators
           
End Sub



Private Sub Form_Unload(Cancel As Integer)
   
   frmFileNameBuilderTOP = frmFilenameBuilder.Top
   frmFileNameBuilderLEFT = frmFilenameBuilder.Left
 
       If FileNameBuilderSTR = "FILEPROPERTIES" Then
        frmFileProperties.Enabled = True
        FileNameBuilderSTR = ""
    End If
  Unload Me

End Sub


' Write ALL Separators into the separators file
Function WriteSeparatorsToFile()
 Dim F As Integer
 Dim x As Integer
 Dim numSeparators As Integer
 Dim cSeparator As String * 16
 Dim buf As String * 256
 Dim return_len As Long
 Dim tempSeparatorName As String
 
 F = FreeFile
 

 return_len = GetWindowsDirectory(buf, Len(buf))
 WindowsDirectoryPath = Left$(buf, return_len)
 fnfilename = Left$(buf, return_len) & "\d64editor-separators.cfg"
 
 Kill fnfilename
 
 Open fnfilename For Binary As #F
  
  numSeparators = listSeparators.ListCount
  
   For x = 0 To numSeparators - 1
    tempSeparatorName = listSeparators.List(x)
    
   ' pad the filename with $A0's
    For w = Len(tempSeparatorName) To 16
   ' there's an $A0 inside the quotes below
   tempSeparatorName = tempSeparatorName & " "
    Next w
    
    cSeparator = tempSeparatorName
    Put #F, , cSeparator
   Next x
 
 Close #F


End Function

' Read ALL Separators into the list box
Function readSeparators()
 Dim F As Integer
 Dim x As Integer
 Dim cSeparator As String * 16
 Dim numSeparators As Integer
 Dim buf As String * 256
 Dim return_len As Long
 Dim tempSeparator As String
 Dim hitanA0 As Integer
 
 F = FreeFile
 
 return_len = GetWindowsDirectory(buf, Len(buf))
 WindowsDirectoryPath = Left$(buf, return_len)
 fnfilename = Left$(buf, return_len) & "\d64editor-separators.cfg"
 
 Open fnfilename For Binary As #F
  
  numSeparators = LOF(F) / 16
  
  If (Int(numSeparators) - numSeparators = 0) Then
   For x = 1 To numSeparators
    Get #F, , cSeparator

      
      ' strip out all the $A0's
      For u = 1 To 16
        If Mid(cSeparator, u, 1) = " " Then
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

     
        tempSeparator = Left(cSeparator, hitanA0 - 1)


    listSeparators.AddItem tempSeparator
   Next x
  Else
   MsgBox "Separator file " & fnfilename & " is corrupt.", vbOKOnly, "Could not read Separator File"
  End If
 
 Close #F
End Function


Private Sub mnuDeleteSeparator_Click()
Call DeleteSeparator
End Sub

Private Sub mnuExit_Click()
   frmFileNameBuilderTOP = frmFilenameBuilder.Top
   frmFileNameBuilderLEFT = frmFilenameBuilder.Left
 
       If FileNameBuilderSTR = "FILEPROPERTIES" Then
        frmFileProperties.Enabled = True
        FileNameBuilderSTR = ""
    End If
  Unload Me
End Sub

Private Sub mnuInvertSelection_Click()
   For i = 0 To listSeparators.ListCount - 1
              
       If listSeparators.Selected(i) = True Then
         listSeparators.Selected(i) = False
       Else
         listSeparators.Selected(i) = True
       End If
   Next i
End Sub



Private Sub mnuModifySeparator_Click()

    Dim lstSelected As Integer

    lstSelected = 0

    For i = 0 To listSeparators.ListCount - 1
       If listSeparators.Selected(i) = True Then
          lstSelected = lstSelected + 1
          listPOS = i
       End If
    Next i
     
    If lstSelected = 0 Then
       Exit Sub
    End If
     
    If lstSelected = 1 Then
      txtNameBuilder.Text = listSeparators.List(listPOS)
      Call DeleteSeparator
    Else
      MsgBox "Please select only one separator line to modify.", vbCritical, "Too many separators selected"
    End If

End Sub

Private Sub mnuSaveSeparatorstoFile_Click()
 Call WriteSeparatorsToFile

  MsgBox "The separators have been saved.", vbInformation, "Separators Saved"
End Sub

Private Sub mnuSelectAllFiles_Click()
   For i = 0 To listSeparators.ListCount - 1
       listSeparators.Selected(i) = True
   Next i
End Sub

Private Sub mnuUnSelectAllFiles_Click()
   For i = 0 To listSeparators.ListCount - 1
       listSeparators.Selected(i) = False
   Next i
End Sub


Private Sub listSeparators_MouseDown(Button As Integer, Shift As Integer, x As Single, Y As Single)

 

 
 If Button = 2 Then
      Dim lParam As Long
        Dim curritem As Long
        Dim r As Long
        Dim pt As POINTAPI
        
        Call GetCursorPos(pt)
        Call ScreenToClient(listSeparators.hWnd, pt)
        
        lParam = MAKELPARAM(pt.x, pt.Y)
                                
        r = SendMessage(listSeparators.hWnd, LB_ITEMFROMPOINT, 0&, ByVal lParam)
        
        If r > -1 Then
           curritem = LoWord(r)
           
          'this line selects the item (if desired)
          'with the right-click
           If listSeparators.ListCount <> 0 Then
              listSeparators.Selected(curritem) = True
           End If
        End If
   
   frmFilenameBuilder.PopupMenu mnuSeparators, 2
 End If
 
End Sub


Private Sub mnuCopySeparatorsIntoD64_Click()

   For i = 0 To listSeparators.ListCount - 1
      If listSeparators.Selected(i) = True Then
          CopySeparatorsToD64 (i)
      End If
  
   Next i

End Sub

Private Sub CopySeparatorsToD64(listvar As Integer)
    Dim b As Integer
    Dim extraspaces As String
    Dim displayfilename As String
    Dim tempfilename As String

    checkForFullDir

    If cantFitDirOnDisk <> 0 Then
       MsgBox "Sorry, you can't add a file to the disk." & vbCrLf & "The directory on the disk is completely full.", vbInformation, "Directory Full"
       Exit Sub
    End If

   displayfilename = listSeparators.List(listvar)
   tempfilename = listSeparators.List(listvar)

' pad the filename with $A0's
For w = Len(tempfilename) To 16

' there's an $A0 inside the quotes below
tempfilename = tempfilename & " "

Next w


    b = frmMain.List1.ListCount
          
    sfiletype(b) = &H80
    sfilename(b) = tempfilename
    sstartingtrack(b) = 0
    sstartingsector(b) = 0
    sfilesize1(b) = 0
    sfilesize2(b) = 0
    
    D64Modified = True
    
extraspaces = ""

   For u = Len(displayfilename) To 16
       extraspaces = extraspaces & " "
   Next u
    
    frmMain.List1.AddItem Chr$(34) & displayfilename & Chr$(34) & extraspaces & vbTab & sstartingtrack(b) & vbTab & sstartingsector(b) & vbTab & sfilesize1(b) & vbTab & "DEL"

    Call updateFileCount

End Sub



