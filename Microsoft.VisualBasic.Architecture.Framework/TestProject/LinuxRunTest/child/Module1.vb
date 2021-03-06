﻿#Region "Microsoft.VisualBasic::cad64572a1181bca413434f0d9aa6f3b, ..\Microsoft.VisualBasic.Architecture.Framework\TestProject\LinuxRunTest\child\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.Language.UnixBash

Module Module1

    Sub Main()

        Call App.Command.__DEBUG_ECHO

        Dim ps1 As New PS1("[\u@\h \W \A #\#]\$ ")

        For i As Integer = 0 To 100
            Call $"{ps1.ToString}  ---> {i}%".__DEBUG_ECHO
            Call Threading.Thread.Sleep(300)
        Next
    End Sub

End Module

