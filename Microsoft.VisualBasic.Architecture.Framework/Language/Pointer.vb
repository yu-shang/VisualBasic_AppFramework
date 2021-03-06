﻿#Region "Microsoft.VisualBasic::82db232c2b98a7d9d007e145ff631756, ..\Microsoft.VisualBasic.Architecture.Framework\Language\Pointer.vb"

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

Namespace Language

    ''' <summary>
    ''' Type of <see cref="Int32"/> pointer class to the <see cref="Array"/> class.
    ''' (<see cref="Int32"/>类型，一般用来进行数组操作的)
    ''' </summary>
    Public Class Pointer

        ''' <summary>
        ''' Current read position
        ''' </summary>
        Protected __index As Integer
        ReadOnly __step As Integer

        ''' <summary>
        ''' Construct a pointer class and then assign a initial <see cref="Int32"/> value.(构造一个指针对象，并且赋值其初始值)
        ''' </summary>
        ''' <param name="n">The initial value.</param>
        Sub New(n As Integer)
            __index = n
            __step = 1
        End Sub

        Public Sub New(n As Integer, [step] As Integer)
            __index = n
            __step = [step]
        End Sub

        ''' <summary>
        ''' Creates a new <see cref="Integer"/> type pointer object in VisualBasic with its initial value is ZERO.(构造一个初始值为零的整形数指针对象)
        ''' </summary>
        Sub New()
            Call Me.New(Scan0)
        End Sub

        Public Overrides Function ToString() As String
            Return __index
        End Function

        Public Shared Widening Operator CType(n As Integer) As Pointer
            Return New Pointer(n)
        End Operator

        Public Shared Narrowing Operator CType(n As Pointer) As Integer
            Return n.__index
        End Operator

        Public Overloads Shared Operator +(n As Pointer, x As Integer) As Pointer
            n.__index += x
            Return n
        End Operator

        Public Overloads Shared Operator +(x As Integer, n As Pointer) As Pointer
            n.__index += x
            Return n
        End Operator

        Public Overloads Shared Operator +(x As Pointer, n As Pointer) As Pointer
            Return New Pointer(n.__index + x.__index)
        End Operator

        ''' <summary>
        ''' ``<see cref="__index"/> &lt; <paramref name="n"/>``
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Overloads Shared Operator <(x As Pointer, n As Integer) As Boolean
            Return x.__index < n
        End Operator

        Public Overloads Shared Operator >(x As Pointer, n As Integer) As Boolean
            Return x.__index > n
        End Operator

        Public Shared Operator -(x As Pointer, n As Integer) As Integer
            Dim p As Integer = x.__index
            x.__index -= n
            Return p
        End Operator

        ''' <summary>
        ''' 移动n，然后返回之前的数值
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Shared Operator <<(x As Pointer, n As Integer) As Integer
            Dim value As Integer = x.__index
            x.__index += n
            Return value
        End Operator

        ''' <summary>
        ''' Automatically increasing self +1 and then returns the previous value.(自增1，然后返回之前的数值)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Overloads Shared Operator +(x As Pointer) As Integer
            Dim p As Integer = x.__index
            x.__index += x.__step
            Return p
        End Operator

        ''' <summary>
        ''' 自减1，然后返回之前的数值
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Overloads Shared Operator -(x As Pointer) As Integer
            Dim p As Integer = x.__index
            x.__index -= x.__step
            Return p
        End Operator

        ''' <summary>
        ''' Less than or equals
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Shared Operator <=(x As Pointer, n As Integer) As Boolean
            Return x.__index <= n
        End Operator

        ''' <summary>
        ''' Greater than or equals
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Shared Operator >=(x As Pointer, n As Integer) As Boolean
            Return x.__index >= n
        End Operator
    End Class
End Namespace


