﻿#Region "Microsoft.VisualBasic::3cf9d150f78cbd3951f1c7721e1cf654, ..\Microsoft.VisualBasic.Architecture.Framework\ComponentModel\DataStructures\HandleList\HashHandle.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel


    Public Class DefaultHashHandle(Of T As sIdEnumerable) : Inherits HashHandle(Of IHashValue(Of T))

        Sub New(Optional capacity As Integer = 2048)
            Call MyBase.New(capacity)
        End Sub

        Sub New(source As IEnumerable(Of T), Optional capacity As Integer = 2048)
            Call Me.New(capacity)
            Call Me.Add(source)
        End Sub

        Public Overloads Sub Add(x As T)
            Call MyBase.Add(New IHashValue(Of T) With {.obj = x})
        End Sub

        Public Overloads Sub Add(source As IEnumerable(Of T))
            For Each x In source
                Call Add(x)
            Next
        End Sub

        Public Shared Operator +(list As DefaultHashHandle(Of T), x As T) As DefaultHashHandle(Of T)
            Call list.Add(x)
            Return list
        End Operator

        Public Shared Operator +(list As DefaultHashHandle(Of T), x As IEnumerable(Of T)) As DefaultHashHandle(Of T)
            Call list.Add(x)
            Return list
        End Operator
    End Class

    Public Class LinkNode(Of T As IHashHandle)

        Private list As HashHandle(Of T)

        ''' <summary>
        ''' Current node in the chain list
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property node As T

        Friend Sub New(x As String, source As HashHandle(Of T))
            list = source
            node = source(x)
        End Sub

        Friend Sub New(x As T, source As HashHandle(Of T))
            list = source
            node = x
        End Sub

        ''' <summary>
        ''' The next element in the chain after this element
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [Next] As LinkNode(Of T)
            Get
                Return New LinkNode(Of T)(list.Next(node.Identifier), list)
            End Get
        End Property

        ''' <summary>
        ''' The previous element in the chain before this element
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Previous As LinkNode(Of T)
            Get
                Return New LinkNode(Of T)(list.Previous(node.Identifier), list)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return node.GetJson
        End Function
    End Class

    Public Class HashHandle(Of T As IHashHandle) : Implements IEnumerable(Of T)

        Protected __innerHash As New Dictionary(Of T)
        Protected __innerList As List(Of T)
        Dim __emptys As Queue(Of Integer)
        Dim delta As Integer

        Default Public ReadOnly Property Item(id As String) As T
            Get
                Return __innerHash(id)
            End Get
        End Property

        Sub New(Optional capacity As Integer = 2048)
            __innerList = New List(Of T)(capacity)
            __emptys = New Queue(Of Integer)(capacity)
            delta = capacity

            Call Me.__allocate()
        End Sub

        Sub New(source As IEnumerable(Of T), Optional capacity As Integer = 2048)
            Call Me.New(capacity)
            Call Me.Add(source)
        End Sub

        Public Function HasElement(x As String) As Boolean
            Return __innerHash.ContainsKey(x)
        End Function

        Public Function IsNull(x As Integer) As Boolean
            Return __emptys.Contains(x)
        End Function

        Public Function [Next](x As T) As T
            Return [Next](x.Identifier)
        End Function

        Public Function [Next](x As String) As T
            Dim pos As Integer = __innerHash(x).Address
            Dim n As T = __innerList(pos + 1)
            Return n
        End Function

        Public Function [Next](i As Integer) As T
            Return __innerList(i + 1)
        End Function

        Public Function Previous(x As T) As T
            Dim pos As Integer = __innerHash(x.Identifier).Address
            Return __innerList(pos - 1)
        End Function

        Public Function Previous(x As String) As T
            Dim pos As Integer = __innerHash(x).Address
            Return __innerList(pos - 1)
        End Function

        Public Function Previous(x As Integer) As T
            Return __innerList(x - 1)
        End Function

        Public Function Current(x As String) As LinkNode(Of T)
            Return New LinkNode(Of T)(x, Me)
        End Function

        Public Function Current(i As Integer) As LinkNode(Of T)
            Dim name As String = __innerList(i).Identifier
            Return New LinkNode(Of T)(name, Me)
        End Function

        Public Sub Remove(x As String)
            Dim n As T = Current(x).node
            __innerList(n.Address) = Nothing
            __innerHash.Remove(n.Identifier)
            __emptys.Enqueue(n.Address)
        End Sub

        Public Sub Remove(i As Integer)
            Dim n As T = __innerList(i)
            __innerList(n.Address) = Nothing
            __innerHash.Remove(n.Identifier)
            __emptys.Enqueue(n.Address)
        End Sub

        Public Sub Add(x As T)
            If __emptys.Count = 0 Then
                Call __allocate()
            End If

            Dim i As Integer = __emptys.Dequeue
            x.Address = i
            __innerList(i) = x
            __innerHash(x.Identifier) = x
        End Sub

        Public Sub Add(source As IEnumerable(Of T))
            For Each x In source
                Call Add(x)
            Next
        End Sub

        ''' <summary>
        ''' Allocate memory
        ''' </summary>
        Private Sub __allocate()
            Dim top As Integer = __innerList.Count

            For i As Integer = 0 To delta - 1
                Call __emptys.Enqueue(top + i)
            Next

            __innerList += New T(__emptys.Count - 1) {}
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x In __innerList
                Yield x
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
