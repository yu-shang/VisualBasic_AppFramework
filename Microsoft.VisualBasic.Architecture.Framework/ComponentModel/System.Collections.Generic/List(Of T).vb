﻿Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures

''' <summary>
''' Represents a strongly typed list of objects that can be accessed by index. Provides
''' methods to search, sort, and manipulate lists.To browse the .NET Framework source
''' code for this type, see the Reference Source.
''' </summary>
''' <typeparam name="T">The type of elements in the list.</typeparam>
Public Class List(Of T) : Inherits Generic.List(Of T)

    Dim __index As Pointer

    ''' <summary>
    ''' Initializes a new instance of the List`1 class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="source">The collection whose elements are copied to the new list.</param>
    Sub New(source As IEnumerable(Of T))
        Call MyBase.New(If(source Is Nothing, {}, source.ToArray))
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the List`1 class that
    ''' contains elements copied from the specified collection and has sufficient capacity
    ''' to accommodate the number of elements copied.
    ''' </summary>
    ''' <param name="x">The collection whose elements are copied to the new list.</param>
    Sub New(ParamArray x As T())
        Call MyBase.New(x)
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the List`1 class that
    ''' is empty and has the default initial capacity.
    ''' </summary>
    Public Sub New()
        Call MyBase.New
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the List`1 class that
    ''' is empty and has the specified initial capacity.
    ''' </summary>
    ''' <param name="capacity">The number of elements that the new list can initially store.</param>
    Public Sub New(capacity As Integer)
        Call MyBase.New(capacity)
    End Sub

    ''' <summary>
    ''' Move Next
    ''' </summary>
    ''' <param name="list"></param>
    ''' <returns></returns>
    Public Overloads Shared Operator +(list As List(Of T)) As T
        Return list(+list.__index)
    End Operator

    ''' <summary>
    ''' Adds an object to the end of the List`1.
    ''' </summary>
    ''' <param name="list"></param>
    ''' <param name="x">The object to be added to the end of the List`1. The
    ''' value can be null for reference types.</param>
    ''' <returns></returns>
    Public Shared Operator +(list As List(Of T), x As T) As List(Of T)
        If list Is Nothing Then
            Return New List(Of T) From {x}
        Else
            Call list.Add(x)
            Return list
        End If
    End Operator

    ''' <summary>
    ''' Adds an object to the end of the List`1.
    ''' </summary>
    ''' <param name="list"></param>
    ''' <param name="x">The object to be added to the end of the List`1. The
    ''' value can be null for reference types.</param>
    ''' <returns></returns>
    Public Shared Operator +(x As T, list As List(Of T)) As List(Of T)
        If list Is Nothing Then
            Return New List(Of T) From {x}
        Else
            Call list.Insert(Scan0, x)
            Return list
        End If
    End Operator

    ''' <summary>
    ''' Removes the first occurrence of a specific object from the List`1.
    ''' </summary>
    ''' <param name="list"></param>
    ''' <param name="x">The object to remove from the List`1. The value can
    ''' be null for reference types.</param>
    ''' <returns></returns>
    Public Shared Operator -(list As List(Of T), x As T) As List(Of T)
        Call list.Remove(x)
        Return list
    End Operator

    ''' <summary>
    ''' Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.
    ''' </summary>
    ''' <param name="list"></param>
    ''' <param name="vals"></param>
    ''' <returns></returns>
    Public Shared Operator +(list As List(Of T), vals As IEnumerable(Of T)) As List(Of T)
        Call list.AddRange(vals.ToArray)
        Return list
    End Operator

    ''' <summary>
    ''' Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.
    ''' </summary>
    ''' <param name="vals"></param>
    ''' <param name="list"></param>
    ''' <returns></returns>
    Public Shared Operator +(vals As IEnumerable(Of T), list As List(Of T)) As List(Of T)
        Dim all As List(Of T) = vals.ToList
        Call all.AddRange(list)
        Return all
    End Operator

    Public Shared Operator -(list As List(Of T), vals As IEnumerable(Of T)) As List(Of T)
        If Not vals Is Nothing Then
            For Each x As T In vals
                Call list.Remove(x)
            Next
        End If
        Return list
    End Operator

    Public Shared Operator -(list As List(Of T), index As Integer) As List(Of T)
        Call list.RemoveAt(index)
        Return list
    End Operator

    Public Shared Narrowing Operator CType(list As List(Of T)) As T()
        Return list.ToArray
    End Operator
End Class