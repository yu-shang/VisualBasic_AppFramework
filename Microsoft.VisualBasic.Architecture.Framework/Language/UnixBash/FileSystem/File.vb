﻿#Region "Microsoft.VisualBasic::91585d4dfc509ed114614f7a23214fe3, ..\Microsoft.VisualBasic.Architecture.Framework\Language\UnixBash\FileSystem\File.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Language.UnixBash

    ''' <summary>
    ''' Asbtract file IO model
    ''' </summary>
    Public MustInherit Class File : Inherits ClassObject
        Implements ISaveHandle

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function

        Public MustOverride Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save

        Public Shared Operator >(file As File, path As String) As Boolean
            Return file.Save(path, Encodings.UTF8)
        End Operator

        Public Shared Operator <(file As File, path As String) As Boolean
            Throw New NotImplementedException
        End Operator

        Public Shared Operator >>(file As File, path As Integer) As Boolean
            Dim handle As FileHandle = __getHandle(path)
            Return file.Save(handle.FileName, handle.encoding)
        End Operator
    End Class

    ''' <summary>
    ''' 文件系统对象的句柄
    ''' </summary>
    Public Structure FileHandle
        Dim FileName As String
        Dim handle As Integer
        Dim encoding As Encoding

        ''' <summary>
        ''' Determined that is this filename is a network location.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsHTTP As Boolean
            Get
                Return FileName.isURL
            End Get
        End Property

        Public ReadOnly Property IsFile As Boolean
            Get
                Return FileName.FileExists
            End Get
        End Property

        Public ReadOnly Property IsDirectory As Boolean
            Get
                Return FileName.DirectoryExists
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Module FileHandles

        ReadOnly ___opendHandles As Dictionary(Of Integer, FileHandle)

        Sub New()
            ___opendHandles = New Dictionary(Of Integer, FileHandle)
        End Sub

        Friend Function __getHandle(path As Integer) As FileHandle
            If Not FileHandles.___opendHandles.ContainsKey(path) Then
                Throw New ObjectNotFoundException($"Path {path} pointer to a null file handle!")
            Else
                Return FileHandles.___opendHandles(path)
            End If
        End Function

        Public Sub Close(file As Integer)
            If ___opendHandles.ContainsKey(file) Then
                Call ___opendHandles.Remove(file)
            Else
                ' Do Nothing.
            End If
        End Sub

        Dim __handle As Value(Of Integer) = New Value(Of Integer)(Integer.MinValue)

        ''' <summary>
        ''' Open a file system handle
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        Public Function OpenHandle(file As String, Optional encoding As Encodings = Encodings.UTF8) As Integer
            If String.IsNullOrEmpty(file) Then
                Throw New NullReferenceException("File handle null pointer!")
            End If

            SyncLock ___opendHandles
                SyncLock __handle
                    __handle.Value += 1

                    Dim handle As New FileHandle With {
                        .encoding = encoding.GetEncodings,
                        .FileName = file,
                        .handle = __handle.Value
                    }

                    Call ___opendHandles.Add(__handle.Value, handle)
                    Call FileIO.FileSystem.CreateDirectory(file.ParentPath)

                    Return __handle.Value
                End SyncLock
            End SyncLock
        End Function

        Public Function OpenTemp() As Integer
            Return OpenHandle(App.GetAppSysTempFile(App.Process.Id))
        End Function

        ''' <summary>
        ''' Is this file opened
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        <Extension> Public Function FileOpened(filename As String) As Boolean
            If Not filename.FileExists Then
                Return False
            Else
                Try
                    Using file As New IO.FileStream(filename, IO.FileMode.OpenOrCreate)
                    End Using

                    Return False
                Catch ex As Exception
                    Return True
                Finally
                End Try
            End If
        End Function

        Public Function Wait(file As String, Optional timeout As Integer = 1000 * 100) As Boolean
            Dim sw As Stopwatch = Stopwatch.StartNew

            Do While file.FileOpened
                Call Thread.Sleep(1)
                If sw.ElapsedMilliseconds >= timeout Then
                    Return False
                End If
            Loop

            Return True
        End Function
    End Module
End Namespace
