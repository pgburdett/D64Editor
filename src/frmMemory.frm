VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "comdlg32.ocx"
Begin VB.Form frmMemory 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Memory"
   ClientHeight    =   4380
   ClientLeft      =   150
   ClientTop       =   795
   ClientWidth     =   3990
   Icon            =   "frmMemory.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   4380
   ScaleWidth      =   3990
   StartUpPosition =   3  'Windows Default
   Begin MSComDlg.CommonDialog memoryDialog 
      Left            =   3480
      Top             =   4080
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.ListBox listMemory 
      BeginProperty Font 
         Name            =   "CBM-Directory-Charset/ck!"
         Size            =   6
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FF0000&
      Height          =   4140
      ItemData        =   "frmMemory.frx":030A
      Left            =   120
      List            =   "frmMemory.frx":030C
      MultiSelect     =   2  'Extended
      TabIndex        =   0
      Top             =   120
      Width           =   3735
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuCopyFilesintoD64 
         Caption         =   "Copy Files Into D64"
      End
      Begin VB.Menu mnuExportToFile 
         Caption         =   "Export to a File"
         Enabled         =   0   'False
         Visible         =   0   'False
      End
      Begin VB.Menu mnuRemoveSelectedFilesFromMemory 
         Caption         =   "Remove selected files from memory"
      End
   End
   Begin VB.Menu mnuEdit 
      Caption         =   "&Edit"
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
Attribute VB_Name = "frmMemory"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
 '
' frmMemory.frm
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


Private Sub Form_Unload(Cancel As Integer)
     For i = 0 To 200
       memoryCache(i) = ""
     Next i
     
     frmMemoryTOP = frmMemory.Top
     frmMemoryLEFT = frmMemory.Left
     
End Sub




Private Sub listMemory_MouseDown(Button As Integer, Shift As Integer, x As Single, Y As Single)

 

 
 If Button = 2 Then
      Dim lParam As Long
        Dim curritem As Long
        Dim r As Long
        Dim pt As POINTAPI
        
        Call GetCursorPos(pt)
        Call ScreenToClient(listMemory.hWnd, pt)
        
        lParam = MAKELPARAM(pt.x, pt.Y)
                                
        r = SendMessage(listMemory.hWnd, LB_ITEMFROMPOINT, 0&, ByVal lParam)
        
        If r > -1 Then
           curritem = LoWord(r)
           
          'this line selects the item (if desired)
          'with the right-click
           If listMemory.ListCount <> 0 Then
              listMemory.Selected(curritem) = True
           End If
        End If
   
   frmMemory.PopupMenu mnuFile, 2
 End If
 
End Sub

Private Sub mnuCopyFilesintoD64_Click()

   For i = 0 To listMemory.ListCount - 1
      If listMemory.Selected(i) = True Then
          CopyFileToD64 (i)
      End If
  
   Next i

End Sub

Private Sub CopyFileToD64(listvar As Integer)
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
    Dim tempDouble As Double
    
       listvar = listvar + 1
       checkForFullDir
    If cantFitDirOnDisk <> 0 Then
    '   MsgBox "Sorry, you can't add any more files to the disk." & vbCrLf & "The directory on the disk is completely full.", vbInformation, "Directory Full"
       Exit Sub
    End If
         
      fileImportLength = memoryFileLength(listvar)
      tempvar2 = 0
      tempDouble = fileImportLength / 254
      If tempDouble > Val(frmMain.lblBlocksFree) Then
            Exit Sub
      End If
  If tempvar2 = 0 Then
    Dim b As Integer

    b = frmMain.List1.ListCount
      
      D64Modified = True

      Open frmMain.dialogopen.Filename For Binary As #1
       
          importBITE = 1
          loopval = 254
          
          ' lets find a block that we can use
               currentBlock = 0   'lets start at 1/0
               bitespersector = 256
               bites2go = fileImportLength
               
               
     tfilename = memoryFilename(listvar)
     tcountvar = memoryFileLength(listvar)
     sfilename(b) = memoryFilename(listvar)
    
    sfiletype(b) = memoryFileType(listvar)
    FTYPEDISPLAY = "PRG"

    If sfiletype(b) = &H82 Then
         FTYPEDISPLAY = "PRG"
     End If
        
    If sfiletype(b) = &H81 Then
         FTYPEDISPLAY = "SEQ"
     End If
        
    If sfiletype(b) = &H80 Then
         FTYPEDISPLAY = "DEL"
     End If
    
    If sfiletype(b) = &H0 Then
         FTYPEDISPLAY = "SCR"
     End If
    
    If sfiletype(b) = &H83 Then
         FTYPEDISPLAY = "USR"
     End If

    If sfiletype(b) = &H84 Then
         FTYPEDISPLAY = "REL"
     End If


    
    If Len(sfilename(b)) > 16 Then
      sfilename(b) = Left(sfilename(b), 16)
    End If
  'not needed, removed 5/13/2015, caused buggy filenames with symbols  sfilename(b) = UCase(sfilename(b))
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

                 bites2go = bites2go - 1
                 varString = String(1, " ")
                 varString = Mid(memoryCache(listvar), importBITE, 1)
                 Put #1, i, varString
                 importBITE = importBITE + 1
                 Next i
            
                test2var = bitesleft + 1
                Put #1, (currentBlock * 256) + 1, 0
                Put #1, ((currentBlock * 256) + 2), test2var
                blocksAllocated(currentBlock) = 1
                blockOwner(currentBlock) = memoryFilename(listvar)
            Else
               
 
                varString = String(254, " ")
                varString = Mid(memoryCache(listvar), importBITE, 254)
'                Get #2, importBITE, varString
                bites2go = bites2go - 254
                importBITE = importBITE + 254
                Put #1, (currentBlock * 256) + 3, varString

                Call blockNum2TrackandSector(nextBlocktoUse)
                test2var = (convertedTrack)
                 Put #1, ((currentBlock * 256) + 1), test2var
                test2var = (convertedSector)
                  Put #1, ((currentBlock * 256) + 2), test2var

                blocksAllocated(currentBlock) = 1
                blockOwner(currentBlock) = memoryFilename(listvar)
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
    
    For x = Len(memorydisplayFilename(listvar)) To 16
       extraspaces = extraspaces & " "
    Next x
    
    If memoryFileLength(listvar) = 0 Then
        sstartingsector(b) = 0
        sstartingtrack(b) = 0
    End If
    
    frmMain.List1.AddItem Chr$(34) & memorydisplayFilename(listvar) & Chr$(34) & extraspaces & vbTab & sstartingtrack(b) & vbTab & sstartingsector(b) & vbTab & sfilesizeDisplay(b) & vbTab & FTYPEDISPLAY
      frmMain.MousePointer = 0
      extraspaces = ""
    Close #1
'    Close #2
    Call writeOutDirectory
    Call BAMWRITE
    firstTimeThrough = 0
    totBlocks = 0
   End If
 '   Next k

 '   If Len(TooBigToFitOnDisk) > 1024 Then
  '      TooBigToFitOnDisk = Left(TooBigToFitOnDisk, 1024)
   ' End If
    
    If toolargeVar <> 0 Then
       MsgBox "The following files were too large to fit on the D64." & vbCrLf & vbCrLf & TooBigToFitOnDisk, vbInformation, "Too big"
    End If





End Sub

Private Sub mnuExportToFile_Click()

   Dim nxtTrack As Long
   Dim nxtSector As Long
   Dim BITE2START As Long
   Dim bitespersector As Long
   Dim g As Long
   Dim BBTT(0 To 257) As Byte
   Dim trackbite As Byte
   Dim sectorbite As Byte
   Dim varString As String
   
   bitespersector = 256
   
If listvar = -1 Then
MsgBox "Highlight the files you want to export.", vbExclamation, "No Files Selected"
  Exit Sub
End If
      memoryDialog.Filename = ""
      memoryDialog.DialogTitle = "Save " & sfilename(listvar) & " As"
      memoryDialog.Flags = &H2 Or &H800 Or &H4 ' Check to see if the file already exists
      memoryDialog.DefaultExt = "prg"
      memoryDialog.Filter = "C64 Program File (*.prg)|*.prg|All Files (*.*)|*.*"
      memoryDialog.ShowSave
      
      If memoryDialog.Filename = "" Then
         Exit Sub
      End If
      
      Open memoryDialog.Filename For Binary As #1

      Put #1, , memoryCache(listMemory.ListIndex + 1)

      Close #1



End Sub

Private Sub mnuInvertSelection_Click()
   For i = 0 To listMemory.ListCount - 1
              
       If listMemory.Selected(i) = True Then
         listMemory.Selected(i) = False
       Else
         listMemory.Selected(i) = True
       End If
   Next i
End Sub

Private Sub mnuRemoveSelectedFilesFromMemory_Click()
  Dim listholder As Integer
  
    listholder = listMemory.ListCount - 1
 
   For i = 0 To listholder
   
      If i > listMemory.ListCount - 1 Then
         Exit Sub
      End If

      If listMemory.Selected(i) = True Then
          RemoveMemoryEntry (i)
          listholder = listholder - 1
          i = i - 1

      End If
  
   Next i
   

   
End Sub

Private Sub RemoveMemoryEntry(memoryVar As Integer)

Dim tstring As String
Dim tvar As Integer
Dim tempStr As String
Dim tempbyte As Byte
Dim tempInt As Integer
Dim tempLong As Long
Dim t As Integer


If memoryVar = -1 Then
MsgBox "Highlight the file you want to remove.", vbExclamation, "No file selected"
  Exit Sub
End If

For x = 0 To listMemory.ListCount

t = memoryVar

tvar = memoryVar + 1
tstring = listMemory.Text

If memoryVar = listMemory.ListCount - 1 Then
  Exit For
End If
listMemory.AddItem listMemory.Text, memoryVar + 2
listMemory.RemoveItem memoryVar
memoryVar = tvar

  tempStr = memoryFilename(t + 1)
  memoryFilename(t + 1) = memoryFilename(t + 2)
  memoryFilename(t + 2) = tempStr

  tempStr = memoryCache(t + 1)
  memoryCache(t + 1) = memoryCache(t + 2)
  memoryCache(t + 2) = tempStr
  
  tempStr = memorydisplayFilename(t + 1)
  memorydisplayFilename(t + 1) = memorydisplayFilename(t + 2)
  memorydisplayFilename(t + 2) = tempStr
  
  tempbyte = memoryFileType(t + 1)
  memoryFileType(t + 1) = memoryFileType(t + 2)
  memoryFileType(t + 2) = tempbyte
  
  tempLong = memoryFileLength(t + 1)
  memoryFileLength(t + 1) = memoryFileLength(t + 2)
  memoryFileLength(t + 2) = tempLong
  


  Next x

   listMemory.RemoveItem listMemory.ListCount - 1
  ' sfilename(t) = ""
  memoryCache(t + 1) = ""
  memoryFilename(t + 1) = ""
  memorydisplayFilename(t + 1) = ""
  memoryFileType(t + 1) = 0
  memoryFileLength(t + 1) = 0
End Sub


Private Sub mnuSelectAllFiles_Click()
   For i = 0 To listMemory.ListCount - 1
       listMemory.Selected(i) = True
   Next i
End Sub

Private Sub mnuUnSelectAllFiles_Click()
   For i = 0 To listMemory.ListCount - 1
       listMemory.Selected(i) = False
   Next i
End Sub
