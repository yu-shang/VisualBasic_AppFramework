﻿#Region "Microsoft.VisualBasic::1f9eb9076bbe9c4e511b408ed8adde84, ..\Microsoft.VisualBasic.Architecture.Framework\Extensions\IO\PathMatch.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Structure PathMatch

    Dim Pair1 As String
    Dim Pair2 As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Iterator Function Pairs(paths1 As IEnumerable(Of String), path2 As IEnumerable(Of String), Optional process As Func(Of String, String) = Nothing) As IEnumerable(Of PathMatch)
        Dim pas1 As String() = paths1.ToArray
        Dim pas2 As String() = path2.ToArray

        If process Is Nothing Then
            process = Function(s) s
        End If

        If pas1.Length >= pas2.Length Then
            For Each x As PathMatch In __pairs(pas1, pas2, process)
                Yield x
            Next
        Else
            For Each x As PathMatch In __pairs(pas2, pas1, process)
                Yield x
            Next
        End If
    End Function

    ''' <summary>
    ''' <paramref name="paths1"/>的元素要比<paramref name="path2"/>多
    ''' </summary>
    ''' <param name="paths1"></param>
    ''' <param name="path2"></param>
    ''' <returns></returns>
    Private Shared Iterator Function __pairs(paths1 As String(), path2 As String(), process As Func(Of String, String)) As IEnumerable(Of PathMatch)
        Dim pls = (From p As String In path2 Select name = process(p.BaseName), p).ToArray

        For Each path As String In paths1
            Dim q As String = process(path.BaseName)

            For Each S In pls
                If InStr(q, S.name, CompareMethod.Text) = 1 OrElse
                    InStr(S.name, q, CompareMethod.Text) = 1 Then
                    Yield New PathMatch With {
                        .Pair1 = path,
                        .Pair2 = S.p
                    }
                    Exit For
                End If
            Next
        Next
    End Function
End Structure

Public Module PathMatches

    Public Iterator Function Pairs(ParamArray paths As NamedValue(Of String())()) As IEnumerable(Of Dictionary(Of String, String))
        Dim primary As NamedValue(Of String()) = paths(Scan0)
        Dim others = (From path As NamedValue(Of String())
                      In paths.Skip(1)
                      Select path.Name,
                          pls = (From p As String
                                 In path.x
                                 Select pName = p.BaseName,
                                     p).ToArray).ToArray

        For Each path As String In primary.x
            Dim q As String = path.BaseName
            Dim result As New Dictionary(Of String, String) From {{primary.Name, path}}

            For Each otherpath In others
                For Each S In otherpath.pls
                    If InStr(q, S.pName, CompareMethod.Text) = 1 OrElse
                        InStr(S.pName, q, CompareMethod.Text) = 1 Then

                        result.Add(otherpath.Name, S.p)
                        Exit For
                    End If
                Next
            Next

            If result.Count = paths.Length Then
                Yield result
            End If
        Next
    End Function
End Module
