﻿#Region "Microsoft.VisualBasic::9e5e92b3df10402c9e30fc179e68e169, ..\Microsoft.VisualBasic.Architecture.Framework\ComponentModel\System.Collections.Generic\CollectionIO.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Collection IO extensions
''' </summary>
Public Module CollectionIO

    Public Delegate Function ISave(obj As IEnumerable, path As String, encoding As Encoding) As Boolean
    Public Delegate Function IRead(type As Type, path As String, encoding As Encoding) As IEnumerable

    Public ReadOnly Property DefaultHandle As ISave = AddressOf SaveJSON
    Public ReadOnly Property DefaultLoadHandle As IRead = AddressOf ReadJSON

    Public Sub SetHandle(handle As ISave)
        _DefaultHandle = handle
    End Sub

    Public Function ReadJSON(type As Type, path As String, encoding As Encoding) As IEnumerable
        Dim text As String = path.ReadAllText(encoding)
        type = type.MakeArrayType
        Return DirectCast(JsonContract.LoadObject(text, type), IEnumerable)
    End Function

    Public Function SaveJSON(obj As IEnumerable, path As String, encoding As Encoding) As Boolean
        Return GetJson(obj, obj.GetType).SaveTo(path, encoding)
    End Function

    Public Function SaveXml(obj As IEnumerable, path As String, encoding As Encoding) As Boolean
        Return GetXml(obj, obj.GetType).SaveTo(path, encoding)
    End Function

    Public Function [TypeOf](Of T)() As [Class](Of T)
        Dim cls As New [Class](Of T)
        Return cls
    End Function
End Module
