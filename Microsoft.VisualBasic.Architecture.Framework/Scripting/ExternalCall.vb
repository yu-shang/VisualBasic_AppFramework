﻿#Region "Microsoft.VisualBasic::5d3570f19f426874a246e78420eefdb5, ..\Microsoft.VisualBasic.Architecture.Framework\Scripting\ExternalCall.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Scripting

    ''' <summary>
    ''' Shell object for the external script running.
    ''' </summary>
    Public Class ExternalCall

        ''' <summary>
        ''' 脚本宿主的可执行文件的路径
        ''' </summary>
        ReadOnly __host As String
        ReadOnly __ext As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="host">The program its file name to run the script</param>
        ''' <param name="ext">File extension name of this type of script</param>
        Sub New(host As String, Optional ext As String = ".txt")
            __host = FileIO.FileSystem.GetFileInfo(host).FullName
            __ext = ext
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="script">The script content</param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Function Run(script As String, Optional args As Specialized.NameValueCollection = Nothing) As ShellValue
            Dim tmp As String = App.GetAppSysTempFile(__ext)
            Call script.SaveTo(tmp, Encodings.ASCII.GetEncodings)
            Return Shell(path:=tmp, args:=args)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">The script file path</param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        ''' <remarks>Perl脚本测试通过！</remarks>
        Public Function Shell(path As String, Optional args As Specialized.NameValueCollection = Nothing) As ShellValue
            Dim param As String =
                If(args Is Nothing,
                   "",
                   String.Join(" ", args.AllKeys.ToArray(Function(s) $"{s} {args.Get(s).CliToken}")))
            Dim IO As New IORedirect(__host, path & " " & param)
            Dim code As Integer = IO.Start(WaitForExit:=True)
            Return New ShellValue(IO, code)
        End Function

        Public Overrides Function ToString() As String
            Return __host
        End Function
    End Class

    ''' <summary>
    ''' Script shell result.
    ''' </summary>
    Public Structure ShellValue
        ''' <summary>
        ''' Standard output on the console
        ''' </summary>
        Public STD_OUT As String
        ''' <summary>
        ''' Standard error
        ''' </summary>
        Public STD_ERR As String
        ''' <summary>
        ''' Process exit code
        ''' </summary>
        Public state As Integer

        Sub New(io As IORedirect, exitCode As Integer)
            state = exitCode
            STD_OUT = io.StandardOutput
            STD_ERR = io.GetError
        End Sub

        ''' <summary>
        ''' Parsing object from the standard output
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="parser">Object parser</param>
        ''' <returns></returns>
        Public Function GetObject(Of T)(parser As Func(Of String, T)) As T
            Return parser(STD_OUT)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
