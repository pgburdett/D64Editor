Attribute VB_Name = "DragDrop"
'
' DragDrop.bas
'
'  Written by
'  the Webulator <balzano50@hotmail.com>
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


'Drag-Drop
Public Const WM_CLOSE = &H10
Public Const WM_DESTROY = &H2
Public Const WM_DROPFILES = &H233
Public Const WM_QUIT = &H12
Public Const PM_NOREMOVE = &H0
Public Const PM_NOYIELD = &H2
Public Const PM_REMOVE = &H1
Public Const NullVar = 0

'GetFileAttributes
Public Const FILE_ATTRIBUTE_ARCHIVE = &H20
Public Const FILE_ATTRIBUTE_COMPRESSED = &H800
Public Const FILE_ATTRIBUTE_DIRECTORY = &H10
Public Const FILE_ATTRIBUTE_HIDDEN = &H2
Public Const FILE_ATTRIBUTE_NORMAL = &H80
Public Const FILE_ATTRIBUTE_READONLY = &H1
Public Const FILE_ATTRIBUTE_SYSTEM = &H4
Public Const FILE_ATTRIBUTE_TEMPORARY = &H100

Type POINTAPI
        x As Long
        Y As Long
End Type

Type MSG
    hWnd As Long
    message As Long
    wParam As Long
    lParam As Long
    time As Long
    pt As POINTAPI
End Type

Declare Function GetMessage Lib "user32" Alias "GetMessageA" (lpMsg As MSG, ByVal hWnd As Long, ByVal wMsgFilterMin As Long, ByVal wMsgFilterMax As Long) As Long
Declare Function PeekMessage Lib "user32" Alias "PeekMessageA" (lpMsg As MSG, ByVal hWnd As Long, ByVal wMsgFilterMin As Long, ByVal wMsgFilterMax As Long, ByVal wRemoveMsg As Long) As Boolean
Declare Sub DragAcceptFiles Lib "shell32.dll" (ByVal hWnd As Long, ByVal fAccept As Long)
Declare Function DragQueryFile Lib "shell32.dll" Alias "DragQueryFileA" (ByVal hDrop As Long, ByVal UINT As Long, ByVal lpStr As String, ByVal ch As Long) As Long
Declare Sub DragFinish Lib "shell32.dll" (ByVal hDrop As Long)
Declare Function GetLastError Lib "kernel32" () As Long
Declare Function WaitMessage Lib "user32" () As Long
Declare Function SetForegroundWindow Lib "user32" (ByVal hWnd As Long) As Long
Declare Function GetFileAttributes Lib "kernel32" Alias "GetFileAttributesA" (ByVal lpFileName As String) As Long
'Declare Function SetFocus Lib "user32" (ByVal hwnd As Long) As Long  'Only use if detecting a mouseover event to recieve and then return focus

Sub Main()
    
    frmMain.Show
    On Error GoTo fontmissing
        fontcheck = FileLen(WindowsDirectoryPath & "\fonts\seq-viewer.fon")
        fontcheck = FileLen(WindowsDirectoryPath & "\fonts\cbm-directory-charset.fon")
        fontcheck = FileLen(WindowsDirectoryPath & "\fonts\cbm64.fon")

   ' Call getOSVersion
    If Command = "" Then
      ' no nothing
    Else
      frmMain.dialogopen.Filename = Command
      frmMain.LoadFile (Command)
    End If
    
'    DragAcceptFiles frmMain.hWnd, True
 ''   WaitforDrop
  '  Unload frmMain
Exit Sub

fontmissing:
    MsgBox "One or more fonts is not properly installed." & vbCrLf & "Please copy the 3 fonts to your " & WindowsDirectoryPath & "\fonts\ directory", vbCritical
'    Call getOSVersion
 '   DragAcceptFiles frmMain.hWnd, True
  '  WaitforDrop
   ' Unload frmMain

End Sub

' Look in frmmain.frm at Sub ProcDroppedFile for the remainder of the code.

'Sub WaitforDrop()
'    Dim msgWin As MSG
 '   Dim ret As Long
  '  Dim msgRet As VbMsgBoxResult
'    Dim hDrop As Long
 '   Dim Filename As String * 128
  '  Dim numOfDroppedFiles As Long
   ' Dim curFile As Long
    
 '   Do
  '      WaitMessage
   '     ret = PeekMessage(msgWin, frmMain.hWnd, 0, 0, PM_NOREMOVE)
    '    If ret Then
     '       'Debug.Print msgWin.message
      '      Select Case msgWin.message
       '         Case WM_DROPFILES
        '            hDrop = msgWin.wParam
         '           numOfDroppedFiles = DragQueryFile(hDrop, True, Filename, 127) 'get the toal number of files dropped
          '          For curFile = 1 To numOfDroppedFiles
           '             ret = DragQueryFile(hDrop, curFile - 1, Filename, 127)    'get the file name
            '            'the filename will be a full qualified path
             '           frmMain.ProcDroppedFile (Filename)
              '      Next curFile
               '     DragFinish (hDrop) 'we are now done with the structure, tell windows to discard it
                '    SetForegroundWindow (frmMain.hWnd)
                 '   'frmMain.DropFinish
                'Case Whatever - need to get and set focus properly on dragover event
 '               Case WM_CLOSE, WM_QUIT
  '                  msgRet = MsgBox("DONE!", vbOKOnly)
   '                 Exit Sub
    '        End Select
     '   End If
      '
       ' ret = GetLastError
'        DoEvents
 '
  '      If ret Then
   '         Debug.Print "Error: " & ret
    '    End If
  '  Loop
'End Sub

