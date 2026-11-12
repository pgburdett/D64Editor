VERSION 5.00
Begin VB.Form frmBlockViewer 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Block Editor"
   ClientHeight    =   5310
   ClientLeft      =   150
   ClientTop       =   795
   ClientWidth     =   10470
   Icon            =   "frmBlockViewer.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   5310
   ScaleWidth      =   10470
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdPreviousBlock 
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
      Left            =   720
      Picture         =   "frmBlockViewer.frx":030A
      Style           =   1  'Graphical
      TabIndex        =   540
      TabStop         =   0   'False
      ToolTipText     =   "Go Back a Block"
      Top             =   480
      Width           =   360
   End
   Begin VB.CommandButton cmdNextBlock 
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
      Left            =   1140
      Picture         =   "frmBlockViewer.frx":07FE
      Style           =   1  'Graphical
      TabIndex        =   539
      TabStop         =   0   'False
      ToolTipText     =   "Go Forward a Block"
      Top             =   480
      Width           =   345
   End
   Begin VB.CheckBox CheckViewASCII 
      Caption         =   "View ASCII"
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
      Left            =   1680
      TabIndex        =   538
      TabStop         =   0   'False
      Top             =   480
      Width           =   1695
   End
   Begin VB.Frame Frame1 
      Caption         =   "Byte to Change"
      BeginProperty Font 
         Name            =   "MS Serif"
         Size            =   6.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1695
      Left            =   12360
      TabIndex        =   275
      Top             =   1800
      Width           =   1935
      Begin VB.ComboBox cmbByteChange 
         BeginProperty Font 
            Name            =   "MS Serif"
            Size            =   6.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   285
         ItemData        =   "frmBlockViewer.frx":0CF2
         Left            =   720
         List            =   "frmBlockViewer.frx":0CF4
         TabIndex        =   276
         Text            =   "cmbByteChange"
         Top             =   960
         Width           =   615
      End
      Begin VB.Label Label17 
         Caption         =   "Changing Byte:"
         BeginProperty Font 
            Name            =   "MS Serif"
            Size            =   6.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   120
         TabIndex        =   278
         Top             =   360
         Width           =   1695
      End
      Begin VB.Label lblByte2Change 
         BeginProperty Font 
            Name            =   "MS Serif"
            Size            =   6.75
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   840
         TabIndex        =   277
         Top             =   600
         Width           =   255
      End
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   255
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   282
      TabStop         =   0   'False
      Top             =   4920
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   254
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   283
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   253
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   284
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   252
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   285
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   251
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   286
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   250
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   287
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   249
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   288
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   248
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   289
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   247
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   290
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   246
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   291
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   245
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   292
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   244
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   293
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   243
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   294
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   242
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   295
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   241
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   296
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   240
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   297
      TabStop         =   0   'False
      Top             =   4920
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   239
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   298
      TabStop         =   0   'False
      Top             =   4680
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   238
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   299
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   237
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   300
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   236
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   301
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   235
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   302
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   234
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   303
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   233
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   304
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   232
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   305
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   231
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   306
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   230
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   307
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   229
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   308
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   228
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   309
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   227
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   310
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   226
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   311
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   225
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   312
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   224
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   313
      TabStop         =   0   'False
      Top             =   4680
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   223
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   314
      TabStop         =   0   'False
      Top             =   4440
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   222
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   315
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   221
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   316
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   220
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   317
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   219
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   318
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   218
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   319
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   217
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   320
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   216
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   321
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   215
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   322
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   214
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   323
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   213
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   324
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   212
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   325
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   211
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   326
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   210
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   327
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   209
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   328
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   208
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   329
      TabStop         =   0   'False
      Top             =   4440
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   207
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   330
      TabStop         =   0   'False
      Top             =   4200
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   206
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   331
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   205
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   332
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   204
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   333
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   203
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   334
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   202
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   335
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   201
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   336
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   200
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   337
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   199
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   338
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   198
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   339
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   197
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   340
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   196
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   341
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   195
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   342
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   194
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   343
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   193
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   344
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   192
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   345
      TabStop         =   0   'False
      Top             =   4200
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   191
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   346
      TabStop         =   0   'False
      Top             =   3960
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   190
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   347
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   189
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   348
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   188
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   349
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   187
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   350
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   186
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   351
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   185
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   352
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   184
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   353
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   183
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   354
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   182
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   355
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   181
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   356
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   180
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   357
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   179
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   358
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   178
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   359
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   177
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   360
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   176
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   361
      TabStop         =   0   'False
      Top             =   3960
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   175
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   362
      TabStop         =   0   'False
      Top             =   3720
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   174
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   363
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   173
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   364
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   172
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   365
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   171
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   366
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   170
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   367
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   169
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   368
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   168
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   369
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   167
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   370
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   166
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   371
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   165
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   372
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   164
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   373
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   163
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   374
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   162
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   375
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   161
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   376
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   160
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   377
      TabStop         =   0   'False
      Top             =   3720
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   159
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   378
      TabStop         =   0   'False
      Top             =   3480
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   158
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   379
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   157
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   380
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   156
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   381
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   155
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   382
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   154
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   383
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   153
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   384
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   152
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   385
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   151
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   386
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   150
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   387
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   149
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   388
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   148
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   389
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   147
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   390
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   146
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   391
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   145
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   392
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   144
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   393
      TabStop         =   0   'False
      Top             =   3480
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   143
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   394
      TabStop         =   0   'False
      Top             =   3240
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   142
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   395
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   141
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   396
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   140
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   397
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   139
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   398
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   138
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   399
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   137
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   400
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   136
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   401
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   135
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   402
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   134
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   403
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   133
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   404
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   132
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   405
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   131
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   406
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   130
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   407
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   129
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   408
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   128
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   409
      TabStop         =   0   'False
      Top             =   3240
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   127
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   410
      TabStop         =   0   'False
      Top             =   3000
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   126
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   411
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   125
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   412
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   124
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   413
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   123
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   414
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   122
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   415
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   121
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   416
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   120
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   417
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   119
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   418
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   118
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   419
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   117
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   420
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   116
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   421
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   115
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   422
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   114
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   423
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   113
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   424
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   112
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   425
      TabStop         =   0   'False
      Top             =   3000
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   111
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   426
      TabStop         =   0   'False
      Top             =   2760
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   110
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   427
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   109
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   428
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   108
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   429
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   107
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   430
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   106
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   431
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   105
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   432
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   104
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   433
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   103
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   434
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   102
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   435
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   101
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   436
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   100
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   437
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   99
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   438
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   98
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   439
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   97
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   440
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   96
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   441
      TabStop         =   0   'False
      Top             =   2760
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   95
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   442
      TabStop         =   0   'False
      Top             =   2520
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   94
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   443
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   93
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   444
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   92
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   445
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   91
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   446
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   90
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   447
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   89
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   448
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   88
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   449
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   87
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   450
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   86
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   451
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   85
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   452
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   84
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   453
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   83
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   454
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   82
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   455
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   81
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   456
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   80
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   457
      TabStop         =   0   'False
      Top             =   2520
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   79
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   458
      TabStop         =   0   'False
      Top             =   2280
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   78
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   459
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   77
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   460
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   76
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   461
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   75
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   462
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   74
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   463
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   73
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   464
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   72
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   465
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   71
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   466
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   70
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   467
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   69
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   468
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   68
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   469
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   67
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   470
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   66
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   471
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   65
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   472
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   64
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   473
      TabStop         =   0   'False
      Top             =   2280
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   63
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   474
      TabStop         =   0   'False
      Top             =   2040
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   62
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   475
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   61
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   476
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   60
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   477
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   59
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   478
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   58
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   479
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   57
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   480
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   56
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   481
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   55
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   482
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   54
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   483
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   53
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   484
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   52
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   485
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   51
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   486
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   50
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   487
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   49
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   488
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   48
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   489
      TabStop         =   0   'False
      Top             =   2040
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   47
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   490
      TabStop         =   0   'False
      Top             =   1800
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   46
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   491
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   45
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   492
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   44
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   493
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   43
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   494
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   42
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   495
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   41
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   496
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   40
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   497
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   39
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   498
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   38
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   499
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   37
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   500
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   36
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   501
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   35
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   502
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   34
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   503
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   33
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   504
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   32
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   505
      TabStop         =   0   'False
      Top             =   1800
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   31
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   506
      TabStop         =   0   'False
      Top             =   1560
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   30
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   507
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   29
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   508
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   28
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   509
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   27
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   510
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   26
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   511
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   25
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   512
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   24
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   513
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   23
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   514
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   22
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   515
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   21
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   516
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   20
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   517
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   19
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   518
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   18
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   519
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   17
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   520
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   16
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   521
      TabStop         =   0   'False
      Top             =   1560
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   15
      Left            =   10080
      MaxLength       =   1
      TabIndex        =   522
      TabStop         =   0   'False
      Top             =   1320
      Width           =   255
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   14
      Left            =   9840
      MaxLength       =   1
      TabIndex        =   523
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   13
      Left            =   9600
      MaxLength       =   1
      TabIndex        =   524
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   12
      Left            =   9360
      MaxLength       =   1
      TabIndex        =   525
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   11
      Left            =   9120
      MaxLength       =   1
      TabIndex        =   526
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   10
      Left            =   8880
      MaxLength       =   1
      TabIndex        =   527
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   9
      Left            =   8640
      MaxLength       =   1
      TabIndex        =   528
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   8
      Left            =   8400
      MaxLength       =   1
      TabIndex        =   529
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   7
      Left            =   8160
      MaxLength       =   1
      TabIndex        =   530
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   6
      Left            =   7920
      MaxLength       =   1
      TabIndex        =   531
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   5
      Left            =   7680
      MaxLength       =   1
      TabIndex        =   532
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   4
      Left            =   7440
      MaxLength       =   1
      TabIndex        =   533
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   3
      Left            =   7200
      MaxLength       =   1
      TabIndex        =   534
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   2
      Left            =   6960
      MaxLength       =   1
      TabIndex        =   535
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   1
      Left            =   6720
      MaxLength       =   1
      TabIndex        =   536
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.TextBox txtHexEditor 
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
      Height          =   240
      Index           =   0
      Left            =   6480
      MaxLength       =   1
      TabIndex        =   537
      TabStop         =   0   'False
      Top             =   1320
      Width           =   315
   End
   Begin VB.Label lblBytesAcrossASCII2 
      Caption         =   "0 1 2 3 4 5 6 7 8 9 A B C D E F"
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
      Left            =   6555
      TabIndex        =   542
      Top             =   1080
      Width           =   3855
   End
   Begin VB.Label lblBytesAcrossASCII 
      Caption         =   "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
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
      Left            =   6555
      TabIndex        =   541
      Top             =   940
      Width           =   3855
   End
   Begin VB.Label lblBlockLocation 
      Caption         =   "Lblblocklocation"
      Height          =   375
      Left            =   120
      TabIndex        =   281
      Top             =   720
      Visible         =   0   'False
      Width           =   495
   End
   Begin VB.Label Label18 
      Caption         =   "BLOCK OWNER"
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
      Left            =   2520
      TabIndex        =   280
      Top             =   0
      Width           =   615
   End
   Begin VB.Label lblFileOwner 
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
      Height          =   135
      Left            =   3240
      TabIndex        =   279
      Top             =   120
      Width           =   3135
   End
   Begin VB.Label lblBlockNum 
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
      Left            =   1440
      TabIndex        =   274
      Top             =   120
      Width           =   735
   End
   Begin VB.Label lblBlocklabel 
      Caption         =   "BLOCK#"
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
      Left            =   600
      TabIndex        =   273
      Top             =   120
      Width           =   735
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   255
      Left            =   6000
      TabIndex        =   272
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   254
      Left            =   5640
      TabIndex        =   271
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   253
      Left            =   5280
      TabIndex        =   270
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   252
      Left            =   4920
      TabIndex        =   269
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   251
      Left            =   4560
      TabIndex        =   268
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   250
      Left            =   4200
      TabIndex        =   267
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   249
      Left            =   3840
      TabIndex        =   266
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   248
      Left            =   3480
      TabIndex        =   265
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   247
      Left            =   3120
      TabIndex        =   264
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   246
      Left            =   2760
      TabIndex        =   263
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   245
      Left            =   2400
      TabIndex        =   262
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   244
      Left            =   2040
      TabIndex        =   261
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   243
      Left            =   1680
      TabIndex        =   260
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   242
      Left            =   1320
      TabIndex        =   259
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   241
      Left            =   960
      TabIndex        =   258
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   240
      Left            =   600
      TabIndex        =   257
      Top             =   4920
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   239
      Left            =   6000
      TabIndex        =   256
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   238
      Left            =   5640
      TabIndex        =   255
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   237
      Left            =   5280
      TabIndex        =   254
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   236
      Left            =   4920
      TabIndex        =   253
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   235
      Left            =   4560
      TabIndex        =   252
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   234
      Left            =   4200
      TabIndex        =   251
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   233
      Left            =   3840
      TabIndex        =   250
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   232
      Left            =   3480
      TabIndex        =   249
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   231
      Left            =   3120
      TabIndex        =   248
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   230
      Left            =   2760
      TabIndex        =   247
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   229
      Left            =   2400
      TabIndex        =   246
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   228
      Left            =   2040
      TabIndex        =   245
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   227
      Left            =   1680
      TabIndex        =   244
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   226
      Left            =   1320
      TabIndex        =   243
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   225
      Left            =   960
      TabIndex        =   242
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   224
      Left            =   600
      TabIndex        =   241
      Top             =   4680
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   223
      Left            =   6000
      TabIndex        =   240
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   222
      Left            =   5640
      TabIndex        =   239
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   221
      Left            =   5280
      TabIndex        =   238
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   220
      Left            =   4920
      TabIndex        =   237
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   219
      Left            =   4560
      TabIndex        =   236
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   218
      Left            =   4200
      TabIndex        =   235
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   217
      Left            =   3840
      TabIndex        =   234
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   216
      Left            =   3480
      TabIndex        =   233
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   215
      Left            =   3120
      TabIndex        =   232
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   214
      Left            =   2760
      TabIndex        =   231
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   213
      Left            =   2400
      TabIndex        =   230
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   212
      Left            =   2040
      TabIndex        =   229
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   211
      Left            =   1680
      TabIndex        =   228
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   210
      Left            =   1320
      TabIndex        =   227
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   209
      Left            =   960
      TabIndex        =   226
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   208
      Left            =   600
      TabIndex        =   225
      Top             =   4440
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   207
      Left            =   6000
      TabIndex        =   224
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   206
      Left            =   5640
      TabIndex        =   223
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   205
      Left            =   5280
      TabIndex        =   222
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   204
      Left            =   4920
      TabIndex        =   221
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   203
      Left            =   4560
      TabIndex        =   220
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   202
      Left            =   4200
      TabIndex        =   219
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   201
      Left            =   3840
      TabIndex        =   218
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   200
      Left            =   3480
      TabIndex        =   217
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   199
      Left            =   3120
      TabIndex        =   216
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   198
      Left            =   2760
      TabIndex        =   215
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   197
      Left            =   2400
      TabIndex        =   214
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   196
      Left            =   2040
      TabIndex        =   213
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   195
      Left            =   1680
      TabIndex        =   212
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   194
      Left            =   1320
      TabIndex        =   211
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   193
      Left            =   960
      TabIndex        =   210
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   192
      Left            =   600
      TabIndex        =   209
      Top             =   4200
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   191
      Left            =   6000
      TabIndex        =   208
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   190
      Left            =   5640
      TabIndex        =   207
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   189
      Left            =   5280
      TabIndex        =   206
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   188
      Left            =   4920
      TabIndex        =   205
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   187
      Left            =   4560
      TabIndex        =   204
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   186
      Left            =   4200
      TabIndex        =   203
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   185
      Left            =   3840
      TabIndex        =   202
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   184
      Left            =   3480
      TabIndex        =   201
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   183
      Left            =   3120
      TabIndex        =   200
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   182
      Left            =   2760
      TabIndex        =   199
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   181
      Left            =   2400
      TabIndex        =   198
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   180
      Left            =   2040
      TabIndex        =   197
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   179
      Left            =   1680
      TabIndex        =   196
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   178
      Left            =   1320
      TabIndex        =   195
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   177
      Left            =   960
      TabIndex        =   194
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   176
      Left            =   600
      TabIndex        =   193
      Top             =   3960
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   175
      Left            =   6000
      TabIndex        =   192
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   174
      Left            =   5640
      TabIndex        =   191
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   173
      Left            =   5280
      TabIndex        =   190
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   172
      Left            =   4920
      TabIndex        =   189
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   171
      Left            =   4560
      TabIndex        =   188
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   170
      Left            =   4200
      TabIndex        =   187
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   169
      Left            =   3840
      TabIndex        =   186
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   168
      Left            =   3480
      TabIndex        =   185
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   167
      Left            =   3120
      TabIndex        =   184
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   166
      Left            =   2760
      TabIndex        =   183
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   165
      Left            =   2400
      TabIndex        =   182
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   164
      Left            =   2040
      TabIndex        =   181
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   163
      Left            =   1680
      TabIndex        =   180
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   162
      Left            =   1320
      TabIndex        =   179
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   161
      Left            =   960
      TabIndex        =   178
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   160
      Left            =   600
      TabIndex        =   177
      Top             =   3720
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   159
      Left            =   6000
      TabIndex        =   176
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   158
      Left            =   5640
      TabIndex        =   175
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   157
      Left            =   5280
      TabIndex        =   174
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   156
      Left            =   4920
      TabIndex        =   173
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   155
      Left            =   4560
      TabIndex        =   172
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   154
      Left            =   4200
      TabIndex        =   171
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   153
      Left            =   3840
      TabIndex        =   170
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   152
      Left            =   3480
      TabIndex        =   169
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   151
      Left            =   3120
      TabIndex        =   168
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   150
      Left            =   2760
      TabIndex        =   167
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   149
      Left            =   2400
      TabIndex        =   166
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   148
      Left            =   2040
      TabIndex        =   165
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   147
      Left            =   1680
      TabIndex        =   164
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   146
      Left            =   1320
      TabIndex        =   163
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   145
      Left            =   960
      TabIndex        =   162
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   144
      Left            =   600
      TabIndex        =   161
      Top             =   3480
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   143
      Left            =   6000
      TabIndex        =   160
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   142
      Left            =   5640
      TabIndex        =   159
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   141
      Left            =   5280
      TabIndex        =   158
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   140
      Left            =   4920
      TabIndex        =   157
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   139
      Left            =   4560
      TabIndex        =   156
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   138
      Left            =   4200
      TabIndex        =   155
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   137
      Left            =   3840
      TabIndex        =   154
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   136
      Left            =   3480
      TabIndex        =   153
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   135
      Left            =   3120
      TabIndex        =   152
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   134
      Left            =   2760
      TabIndex        =   151
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   133
      Left            =   2400
      TabIndex        =   150
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   132
      Left            =   2040
      TabIndex        =   149
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   131
      Left            =   1680
      TabIndex        =   148
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   130
      Left            =   1320
      TabIndex        =   147
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   129
      Left            =   960
      TabIndex        =   146
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   128
      Left            =   600
      TabIndex        =   145
      Top             =   3240
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   127
      Left            =   6000
      TabIndex        =   144
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   126
      Left            =   5640
      TabIndex        =   143
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   125
      Left            =   5280
      TabIndex        =   142
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   124
      Left            =   4920
      TabIndex        =   141
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   123
      Left            =   4560
      TabIndex        =   140
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   122
      Left            =   4200
      TabIndex        =   139
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   121
      Left            =   3840
      TabIndex        =   138
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   120
      Left            =   3480
      TabIndex        =   137
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   119
      Left            =   3120
      TabIndex        =   136
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   118
      Left            =   2760
      TabIndex        =   135
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   117
      Left            =   2400
      TabIndex        =   134
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   116
      Left            =   2040
      TabIndex        =   133
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   115
      Left            =   1680
      TabIndex        =   132
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   114
      Left            =   1320
      TabIndex        =   131
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   113
      Left            =   960
      TabIndex        =   130
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   112
      Left            =   600
      TabIndex        =   129
      Top             =   3000
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   111
      Left            =   6000
      TabIndex        =   128
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   110
      Left            =   5640
      TabIndex        =   127
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   109
      Left            =   5280
      TabIndex        =   126
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   108
      Left            =   4920
      TabIndex        =   125
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   107
      Left            =   4560
      TabIndex        =   124
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   106
      Left            =   4200
      TabIndex        =   123
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   105
      Left            =   3840
      TabIndex        =   122
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   104
      Left            =   3480
      TabIndex        =   121
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   103
      Left            =   3120
      TabIndex        =   120
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   102
      Left            =   2760
      TabIndex        =   119
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   101
      Left            =   2400
      TabIndex        =   118
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   100
      Left            =   2040
      TabIndex        =   117
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   99
      Left            =   1680
      TabIndex        =   116
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   98
      Left            =   1320
      TabIndex        =   115
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   97
      Left            =   960
      TabIndex        =   114
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   96
      Left            =   600
      TabIndex        =   113
      Top             =   2760
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   95
      Left            =   6000
      TabIndex        =   112
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   94
      Left            =   5640
      TabIndex        =   111
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   93
      Left            =   5280
      TabIndex        =   110
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   92
      Left            =   4920
      TabIndex        =   109
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   91
      Left            =   4560
      TabIndex        =   108
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   90
      Left            =   4200
      TabIndex        =   107
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   89
      Left            =   3840
      TabIndex        =   106
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   88
      Left            =   3480
      TabIndex        =   105
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   87
      Left            =   3120
      TabIndex        =   104
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   86
      Left            =   2760
      TabIndex        =   103
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   85
      Left            =   2400
      TabIndex        =   102
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   84
      Left            =   2040
      TabIndex        =   101
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   83
      Left            =   1680
      TabIndex        =   100
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   82
      Left            =   1320
      TabIndex        =   99
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   81
      Left            =   960
      TabIndex        =   98
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   80
      Left            =   600
      TabIndex        =   97
      Top             =   2520
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   79
      Left            =   6000
      TabIndex        =   96
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   78
      Left            =   5640
      TabIndex        =   95
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   77
      Left            =   5280
      TabIndex        =   94
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   76
      Left            =   4920
      TabIndex        =   93
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   75
      Left            =   4560
      TabIndex        =   92
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   74
      Left            =   4200
      TabIndex        =   91
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   73
      Left            =   3840
      TabIndex        =   90
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   72
      Left            =   3480
      TabIndex        =   89
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   71
      Left            =   3120
      TabIndex        =   88
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   70
      Left            =   2760
      TabIndex        =   87
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   69
      Left            =   2400
      TabIndex        =   86
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   68
      Left            =   2040
      TabIndex        =   85
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   67
      Left            =   1680
      TabIndex        =   84
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   66
      Left            =   1320
      TabIndex        =   83
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   65
      Left            =   960
      TabIndex        =   82
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   64
      Left            =   600
      TabIndex        =   81
      Top             =   2280
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   63
      Left            =   6000
      TabIndex        =   80
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   62
      Left            =   5640
      TabIndex        =   79
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   61
      Left            =   5280
      TabIndex        =   78
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   60
      Left            =   4920
      TabIndex        =   77
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   59
      Left            =   4560
      TabIndex        =   76
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   58
      Left            =   4200
      TabIndex        =   75
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   57
      Left            =   3840
      TabIndex        =   74
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   56
      Left            =   3480
      TabIndex        =   73
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   55
      Left            =   3120
      TabIndex        =   72
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   54
      Left            =   2760
      TabIndex        =   71
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   53
      Left            =   2400
      TabIndex        =   70
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   52
      Left            =   2040
      TabIndex        =   69
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   51
      Left            =   1680
      TabIndex        =   68
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   50
      Left            =   1320
      TabIndex        =   67
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   49
      Left            =   960
      TabIndex        =   66
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   48
      Left            =   600
      TabIndex        =   65
      Top             =   2040
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   47
      Left            =   6000
      TabIndex        =   64
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   46
      Left            =   5640
      TabIndex        =   63
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   45
      Left            =   5280
      TabIndex        =   62
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   44
      Left            =   4920
      TabIndex        =   61
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   43
      Left            =   4560
      TabIndex        =   60
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   42
      Left            =   4200
      TabIndex        =   59
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   41
      Left            =   3840
      TabIndex        =   58
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   40
      Left            =   3480
      TabIndex        =   57
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   39
      Left            =   3120
      TabIndex        =   56
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   38
      Left            =   2760
      TabIndex        =   55
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   37
      Left            =   2400
      TabIndex        =   54
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   36
      Left            =   2040
      TabIndex        =   53
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   35
      Left            =   1680
      TabIndex        =   52
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   34
      Left            =   1320
      TabIndex        =   51
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   33
      Left            =   960
      TabIndex        =   50
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   32
      Left            =   600
      TabIndex        =   49
      Top             =   1800
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   31
      Left            =   6000
      TabIndex        =   48
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   30
      Left            =   5640
      TabIndex        =   47
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   29
      Left            =   5280
      TabIndex        =   46
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   28
      Left            =   4920
      TabIndex        =   45
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   27
      Left            =   4560
      TabIndex        =   44
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   26
      Left            =   4200
      TabIndex        =   43
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   25
      Left            =   3840
      TabIndex        =   42
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   24
      Left            =   3480
      TabIndex        =   41
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   23
      Left            =   3120
      TabIndex        =   40
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   22
      Left            =   2760
      TabIndex        =   39
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   21
      Left            =   2400
      TabIndex        =   38
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   20
      Left            =   2040
      TabIndex        =   37
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   19
      Left            =   1680
      TabIndex        =   36
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   18
      Left            =   1320
      TabIndex        =   35
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   17
      Left            =   960
      TabIndex        =   34
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   16
      Left            =   600
      TabIndex        =   33
      Top             =   1560
      Width           =   300
   End
   Begin VB.Label Label16 
      Caption         =   "F0"
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
      TabIndex        =   32
      Top             =   4980
      Width           =   255
   End
   Begin VB.Label Label15 
      Caption         =   "E0"
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
      TabIndex        =   31
      Top             =   4740
      Width           =   255
   End
   Begin VB.Label Label14 
      Caption         =   "D0"
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
      TabIndex        =   30
      Top             =   4500
      Width           =   255
   End
   Begin VB.Label Label13 
      Caption         =   "C0"
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
      TabIndex        =   29
      Top             =   4260
      Width           =   255
   End
   Begin VB.Label Label12 
      Caption         =   "B0"
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
      TabIndex        =   28
      Top             =   4020
      Width           =   255
   End
   Begin VB.Label Label11 
      Caption         =   "A0"
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
      TabIndex        =   27
      Top             =   3780
      Width           =   255
   End
   Begin VB.Label Label10 
      Caption         =   "90"
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
      TabIndex        =   26
      Top             =   3540
      Width           =   255
   End
   Begin VB.Label Label9 
      Caption         =   "80"
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
      TabIndex        =   25
      Top             =   3300
      Width           =   255
   End
   Begin VB.Label Label8 
      Caption         =   "70"
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
      TabIndex        =   24
      Top             =   3060
      Width           =   255
   End
   Begin VB.Label Label7 
      Caption         =   "60"
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
      TabIndex        =   23
      Top             =   2820
      Width           =   255
   End
   Begin VB.Label Label6 
      Caption         =   "50"
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
      TabIndex        =   22
      Top             =   2580
      Width           =   255
   End
   Begin VB.Label Label5 
      Caption         =   "40"
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
      TabIndex        =   21
      Top             =   2340
      Width           =   255
   End
   Begin VB.Label Label4 
      Caption         =   "30"
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
      TabIndex        =   20
      Top             =   2085
      Width           =   255
   End
   Begin VB.Label Label3 
      Caption         =   "20"
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
      TabIndex        =   19
      Top             =   1845
      Width           =   255
   End
   Begin VB.Label Label2 
      Caption         =   "10"
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
      TabIndex        =   18
      Top             =   1620
      Width           =   255
   End
   Begin VB.Label Label1 
      Caption         =   "00"
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
      TabIndex        =   17
      Top             =   1395
      Width           =   255
   End
   Begin VB.Label lblBytesAcross 
      Caption         =   "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F"
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
      Left            =   640
      TabIndex        =   16
      Top             =   1110
      Width           =   5655
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   15
      Left            =   6000
      TabIndex        =   15
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   14
      Left            =   5640
      TabIndex        =   14
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   13
      Left            =   5280
      TabIndex        =   13
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   12
      Left            =   4920
      TabIndex        =   12
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   11
      Left            =   4560
      TabIndex        =   11
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   10
      Left            =   4200
      TabIndex        =   10
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   9
      Left            =   3840
      TabIndex        =   9
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   8
      Left            =   3480
      TabIndex        =   8
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   7
      Left            =   3120
      TabIndex        =   7
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   6
      Left            =   2760
      TabIndex        =   6
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   5
      Left            =   2400
      TabIndex        =   5
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   4
      Left            =   2040
      TabIndex        =   4
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   3
      Left            =   1680
      TabIndex        =   3
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   2
      Left            =   1320
      TabIndex        =   2
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   1
      Left            =   960
      TabIndex        =   1
      Top             =   1320
      Width           =   300
   End
   Begin VB.Label lblBYTE 
      BackColor       =   &H80000014&
      BorderStyle     =   1  'Fixed Single
      Caption         =   "AA"
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
      Height          =   225
      Index           =   0
      Left            =   600
      TabIndex        =   0
      Top             =   1320
      Width           =   300
   End
   Begin VB.Menu mnuFile 
      Caption         =   "File"
      Begin VB.Menu mnuWipeBlock 
         Caption         =   "Wipe Block"
      End
      Begin VB.Menu mnuSaveChangesToBlock 
         Caption         =   "Save Changes to Block"
      End
      Begin VB.Menu mnuDiscardChanges 
         Caption         =   "Discard Changes and Close"
      End
   End
End
Attribute VB_Name = "frmBlockViewer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
' frmBlockViewer.frm
'
'  Written by
'  Felix Vazquez <al@p2pchat.net>
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



Private Sub CheckViewASCII_Click()

    If CheckViewASCII.Value = 1 Then
       Call BlockViewerHexToAscii
       frmBlockViewer.Width = 10575
       HexEnabled = True
    End If
    
    If CheckViewASCII.Value = 0 Then
       frmBlockViewer.Width = 6540
       HexEnabled = False
    End If
    
End Sub

Private Sub cmbByteChange_KeyDown(KeyCode As Integer, Shift As Integer)
       
       If KeyCode = vbKeyRight Then
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
           If varForBlockViewer + 1 < 256 Then
              lblBYTE_Click (varForBlockViewer + 1)
           Else
              lblBYTE_Click (0)
           End If
           Exit Sub
       End If
       
       If KeyCode = vbKeyLeft Then
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
           If varForBlockViewer > 1 Then
              lblBYTE_Click (varForBlockViewer - 1)
           Else
              lblBYTE_Click (0)
           End If
           Exit Sub
       End If
       
       
        If KeyCode = vbKeyDown Then
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
           If varForBlockViewer + 1 < 241 Then
              lblBYTE_Click (varForBlockViewer + 16)
           Else
           End If
           Exit Sub
       End If
      
         If KeyCode = vbKeyUp Then
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
           If varForBlockViewer + 1 > 16 Then
              lblBYTE_Click (varForBlockViewer - 16)
           Else
           End If
           Exit Sub
       End If
     
      
End Sub

Private Sub cmbByteChange_Keypress(Keyascii As Integer)

       ' If user hits TAB, move to next BYTE
       If Keyascii = 9 Then
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
           If varForBlockViewer + 1 < 256 Then
              lblBYTE_Click (varForBlockViewer + 1)
           Else
              lblBYTE_Click (0)
           End If
           Exit Sub
       End If
       

       
     If Keyascii <> 65 And Keyascii <> 66 And Keyascii <> 67 And Keyascii <> 68 And _
     Keyascii <> 69 And Keyascii <> 70 And Keyascii <> 48 And Keyascii <> 49 And _
     Keyascii <> 50 And Keyascii <> 51 And Keyascii <> 52 And Keyascii <> 53 And _
     Keyascii <> 54 And Keyascii <> 55 And Keyascii <> 56 And Keyascii <> 57 And _
     Keyascii <> 97 And Keyascii <> 98 And Keyascii <> 99 And Keyascii <> 100 And _
     Keyascii <> 101 And Keyascii <> 102 Then
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
        Exit Sub
     End If
 
     If firstKeyPressToggle = 0 Then
       firstKeyPressValue = Keyascii
       firstKeyPressToggle = 1
       Exit Sub
     End If
  
     If firstKeyPressToggle = 1 Then
         cmbByteChange.Text = UCase(Chr(firstKeyPressValue) & Chr(Keyascii))
         firstKeyPressToggle = 0
         firstKeyPressValue = 0
         
            lblBYTE(varForBlockViewer).Caption = cmbByteChange.Text
            
            ' Update the 'HexEditor'
            If HexEnabled = True Then Call BlockViewerHexToAscii
           
           If varForBlockViewer + 1 < 256 Then
              lblBYTE_Click (varForBlockViewer + 1)
           Else
              lblBYTE_Click (0)
           End If
         
         Exit Sub
     End If


End Sub



Private Sub cmdNextBlock_Click()
   Dim z As Integer
   Dim startblock As Long
   Dim p As Long
   Dim BITE As Byte
   Dim x As Long
   Dim tstring As String
   
   If lblBlockLocation.Caption = 682 Then
     z = 0
   Else
     z = lblBlockLocation.Caption + 1
   End If
   
   If blockOwner(z) = "" Then
     frmBlockViewer.lblFileOwner.Caption = "<UNCLAIMED>"
   Else
     frmBlockViewer.lblFileOwner.Caption = blockOwner(z)
   End If
   x = Val(z)
   startblock = x * 256
   startblock = startblock + 1
   
   HexEnabled = False
   
   Open frmMain.dialogopen.Filename For Binary As #1
   
          For p = startblock To (startblock + 255)
             
             Get #1, p, BITE
             
             tstring = Hex(BITE)
             
             If Len(tstring) < 2 Then
                
                tstring = "0" & tstring
             End If
             frmBlockViewer.lblBYTE(p - startblock).Caption = tstring
             
            ' Yes I know I can make this to a function but it will be quicker this way.
             If frmBlockViewer.lblBYTE(p - startblock).Caption = "00" Or frmBlockViewer.lblBYTE(p - startblock).Caption = "09" Or frmBlockViewer.lblBYTE(p - startblock).Caption = "1A" Then
             ' 00 is not displayed so lets use 'A0' until we get someone
             ' to add in a block we can use to the font. (something not used on c64)
              frmBlockViewer.txtHexEditor(p - startblock).Text = Chr$(frmBlockViewer.HexToDecimal("A0"))
             Else
              frmBlockViewer.txtHexEditor(p - startblock).Text = Chr$(frmBlockViewer.HexToDecimal(tstring))
             End If
             
          Next p
   
 
   Close #1
   
   If frmBlockViewer.CheckViewASCII.Value = 1 Then HexEnabled = True
   
   Call blockNum2TrackandSector(z)
   frmBlockViewer.lblBlockNum.Caption = convertedTrack & ", " & convertedSector
   frmBlockViewer.lblBlockLocation.Caption = z
          
   Call detect128AutoBoot
   
End Sub

Private Sub cmdPreviousBlock_Click()
   Dim z As Integer
   Dim startblock As Long
   Dim p As Long
   Dim BITE As Byte
   Dim x As Long
   Dim tstring As String
   
    If lblBlockLocation.Caption = 0 Then
     z = 682
   Else
     z = lblBlockLocation.Caption - 1
   End If
   
   If blockOwner(z) = "" Then
     frmBlockViewer.lblFileOwner.Caption = "<UNCLAIMED>"
   Else
     frmBlockViewer.lblFileOwner.Caption = blockOwner(z)
   End If
   x = Val(z)
   startblock = x * 256
   startblock = startblock + 1
   
   HexEnabled = False
   
   Open frmMain.dialogopen.Filename For Binary As #1
   
          For p = startblock To (startblock + 255)
             
             Get #1, p, BITE
             
             tstring = Hex(BITE)
             
             If Len(tstring) < 2 Then
                
                tstring = "0" & tstring
             End If
             frmBlockViewer.lblBYTE(p - startblock).Caption = tstring
             
            ' Yes I know I can make this to a function but it will be quicker this way.
             If frmBlockViewer.lblBYTE(p - startblock).Caption = "00" Or frmBlockViewer.lblBYTE(p - startblock).Caption = "09" Or frmBlockViewer.lblBYTE(p - startblock).Caption = "1A" Then
             ' 00 is not displayed so lets use 'A0' until we get someone
             ' to add in a block we can use to the font. (something not used on c64)
              frmBlockViewer.txtHexEditor(p - startblock).Text = Chr$(frmBlockViewer.HexToDecimal("A0"))
             Else
              frmBlockViewer.txtHexEditor(p - startblock).Text = Chr$(frmBlockViewer.HexToDecimal(tstring))
             End If
             
          Next p
   
 
   Close #1
   
   If frmBlockViewer.CheckViewASCII.Value = 1 Then HexEnabled = True
   
   Call blockNum2TrackandSector(z)
   frmBlockViewer.lblBlockNum.Caption = convertedTrack & ", " & convertedSector
   frmBlockViewer.lblBlockLocation.Caption = z
   
   Call detect128AutoBoot

End Sub

Private Sub mnuWipeBlock_Click()
  For i = 0 To 255
     frmBlockViewer.lblBYTE(i) = "00"
  Next i
  If HexEnabled = True Then Call BlockViewerHexToAscii

End Sub

Private Sub Form_Load()
frmBlockViewer.Top = frmBlockViewerTOP
frmBlockViewer.Left = frmBlockViewer.Left

  For x = 0 To 255
   tstring = Hex(x)
   
   If Len(tstring) < 2 Then
      tstring = "0" & tstring
   End If
   
   cmbByteChange.AddItem tstring
  Next x

  If BlockViewerASCIIMode = 0 Then
    HexEnabled = False
    frmBlockViewer.Width = 6540
    CheckViewASCII.Value = 0
  Else
    CheckViewASCII.Value = 1
    Call BlockViewerHexToAscii
    frmBlockViewer.Width = 10575
    HexEnabled = True
  End If

   Call detect128AutoBoot

End Sub

Private Sub Form_Unload(Cancel As Integer)
frmBlockViewerTOP = frmBlockViewer.Top
frmBlockViewerLEFT = frmBlockViewer.Left
BlockViewerASCIIMode = CheckViewASCII.Value
   frmBAM.Enabled = True

End Sub



Private Sub lblBYTE_Click(Index As Integer)
  Dim x As Integer
  Dim tstring As String
  
  HexEnabled = False
  
  If CheckViewASCII.Value = 1 Then Call BlockViewerHexToAscii
  
  lblBYTE(varForBlockViewer).ForeColor = &HFF0000
  txtHexEditor(varForBlockViewer).ForeColor = &HFF0000
  
  varForBlockViewer = lblBYTE(Index).Index


   cmbByteChange.Text = lblBYTE(Index).Caption
   lblBYTE(Index).ForeColor = &HFF
   txtHexEditor(varForBlockViewer).ForeColor = &HFF
   
   tstring = Hex(varForBlockViewer)
   
   If Len(tstring) < 2 Then
       tstring = "0" & tstring
   End If
   
   lblByte2Change.Caption = tstring
   cmbByteChange.SetFocus
   HexEnabled = True

End Sub



Private Sub mnuDiscardChanges_Click()
    Unload Me
End Sub

Private Sub mnuSaveChangesToBlock_Click()
   Dim blocklength As Long
   Dim blockstart As Long
   Dim z As Long
   Dim tempBITE As Byte
   

   blocklength = 256
   blockstart = Val(lblBlockLocation.Caption) + 1
   

   z = 0
   
   If warningsDisabled = 1 Then
   
   Open frmMain.dialogopen.Filename For Binary As #1
   
          For p = (((blocklength * blockstart) + 1) - 256) To (((blocklength * blockstart) + 256) - 256)
 
             tempBITE = CLng("&H" & lblBYTE(z).Caption)
             Put #1, p, tempBITE
             z = z + 1
          Next p
          
   Close #1

   MsgBox "Changes applied to block.", vbInformation, "Changes applied"
   D64Modified = True
   Exit Sub
   End If
   
   
If MsgBox("Changes will not be saved until you Save the D64." & vbCrLf & "", vbYesNo, "Apply Changes?") = vbYes Then

   Open frmMain.dialogopen.Filename For Binary As #1
   
          For p = (((blocklength * blockstart) + 1) - 256) To (((blocklength * blockstart) + 256) - 256)
 
             tempBITE = CLng("&H" & lblBYTE(z).Caption)
             Put #1, p, tempBITE
             z = z + 1
          Next p
          
   Close #1

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

   D64Modified = True
   MsgBox "Changes applied to block.", vbInformation, "Changes applied"
End If

End Sub


' Makes the frmBlockViewer Look like a Hex Editor
Function BlockViewerHexToAscii()
 Dim x As Integer
 
 If HexEnabled = False Then Exit Function
 
 HexEnabled = False
 
 For x = 0 To 255
  If lblBYTE(x).Caption = "00" Or lblBYTE(x).Caption = "09" Or lblBYTE(x).Caption = "1A" Then
   ' 00 is not displayed so lets use 'A0' until we get someone
   ' to add in a block we can use to the font. (something not used on c64)
   txtHexEditor(x).Text = Chr$(HexToDecimal("A0"))
  Else
   txtHexEditor(x).Text = Chr$(HexToDecimal(lblBYTE(x).Caption))
  End If
 Next x

  HexEnabled = True
End Function

' Needed by function BlockViewerHexToAscii
' Function found on Google. Author unknown.
Public Function HexToDecimal(sSource As String)
 Dim i As Integer, iVal As Byte
 If (Trim$(sSource) = "") Then
  HexToDecimal = 0
  Exit Function
 End If
 
 HexToDecimal = 0
 
 For i = 1 To Len(sSource)
  iVal = CByte("&h" & Mid$(sSource, i, 1))
  HexToDecimal = CDec(HexToDecimal + iVal * 16 ^ (Len(sSource) - i))
 Next i
End Function



Private Sub txtHexEditor_Change(Index As Integer)
 On Error GoTo exitSub
  If (HexEnabled = True) Then lblBYTE(Index).Caption = Hex(Asc(txtHexEditor(Index).Text))
exitSub:
End Sub

Function detect128AutoBoot()

  detect128AutoBoot = False

  ' Detect CBM [C128 Auto Boot Sector] 43 42 4D 00
  If (frmBlockViewer.lblBYTE(0).Caption = "43" And frmBlockViewer.lblBYTE(1).Caption = "42" And frmBlockViewer.lblBYTE(2).Caption = "4D" And frmBlockViewer.lblBYTE(3).Caption = "00") Then
    frmBlockViewer.lblFileOwner.Caption = "<C128 AUTO BOOT SECTOR>"
  End If
  
End Function


