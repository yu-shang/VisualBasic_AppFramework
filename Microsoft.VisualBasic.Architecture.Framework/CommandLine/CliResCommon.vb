﻿#Region "Microsoft.VisualBasic::a35ac6ec5094a662c893198d6059382d, ..\Microsoft.VisualBasic.Architecture.Framework\CommandLine\CliResCommon.vb"

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

Imports System.Reflection

Namespace CommandLine

    ''' <summary>
    ''' CLI resources manager
    ''' </summary>
    Public Class CliResCommon

        Private ReadOnly bufType As Type = GetType(Byte())
        Private ReadOnly Resource As Dictionary(Of String, Func(Of Byte()))

        ReadOnly EXPORT As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="EXPORT">资源文件的数据缓存文件夹</param>
        Sub New(EXPORT As String, ResourceManager As Type)
            Dim tag As BindingFlags = BindingFlags.NonPublic Or BindingFlags.Static
            Dim propBufs = From [Property] As PropertyInfo
                           In ResourceManager.GetProperties(bindingAttr:=tag)
                           Where [Property].PropertyType.Equals(bufType)
                           Select [Property]

            Me.EXPORT = EXPORT
            Me.Resource = propBufs.ToDictionary(Of String, Func(Of Byte()))(
                Function(x) x.Name,
                Function(x) New Func(Of Byte())(Function() DirectCast(x.GetValue(Nothing, Nothing), Byte())))
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">使用 NameOf 操作符来获取资源</param>
        ''' <returns></returns>
        Public Function TryRelease(Name As String, Optional ext As String = "exe") As String
            Dim path As String = $"{EXPORT}/{Name}.{ext}"

            If path.FileExists Then
                Return path
            End If

            If Not Resource.ContainsKey(Name) Then
                Return ""
            End If

            Dim buf As Byte() = Resource(Name)()
            Try
                If buf.FlushStream(path) Then
                    Call Console.WriteLine(resReleaseMsg, path.ToFileURL, buf.Length)
                    Return path
                Else
                    Return ""
                End If
            Catch ex As Exception
                ex = New Exception(path, ex)
                Call App.LogException(ex)
                Return ""
            End Try
        End Function

        Const resReleaseMsg As String = "Release resource to {0} // length={1} bytes"

        Public Overrides Function ToString() As String
            Return EXPORT
        End Function
    End Class
End Namespace
