﻿#Region "Microsoft.VisualBasic::4f8834ed4afa21047c972b371c34d90a, ..\Microsoft.VisualBasic.Architecture.Framework\ConsoleDevices\STDIO__\STDIO.vb"

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

Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.STDIO__
Imports System.Runtime.CompilerServices
Imports System.IO

Namespace Terminal

    ''' <summary>
    ''' A standard input/output compatibility package that makes VisualBasic console
    ''' program easily running on the Linux server or mac osx operating system.
    ''' (一个用于让VisualBasic应用程序更加容易的运行于Linux服务器或者MAC系统之上的标准输入输出流的系统兼容包)
    ''' </summary>
    ''' <remarks></remarks>
    <PackageNamespace("STDIO", Description:="A standard input/output compatibility package that makes VisualBasic console program easily running on the Linux server or mac osx operating system.",
                      Publisher:="xie.guigang@live.com",
                      Revision:=569,
                      Url:="http://gcmodeller.org")>
    Public Module STDIO

        ''' <summary>
        ''' A dictionary list of the escape characters.(转义字符列表)
        ''' </summary>
        ''' <remarks></remarks>
        Dim Eschs As Dictionary(Of String, String) =
            New Dictionary(Of String, String) From
            {
                {"\o", String.Empty},
                {"\n", vbCrLf},
                {"\r", vbCr},
                {"\t", vbTab},
                {"\v", String.Empty},
                {"\b", vbBack},
                {"\f", vbFormFeed},
                {"\'", QUOT_CHAR},
                {"\" & QUOT_CHAR, QUOT_CHAR}}

        Public Const QUOT_CHAR As Char = Chr(34)

#Region "printf"

        ''' <summary>
        ''' Output the string to the console using a specific formation.(按照指定的格式将字符串输出到终端窗口之上，请注意，这个函数除了将数据流输出到标准终端之外，还会输出到调试终端)
        ''' </summary>
        ''' <param name="s">A string to print on the console window.(输出到终端窗口之上的字符串)</param>
        ''' <param name="args">Formation parameters.(格式化参数)</param>
        ''' <remarks></remarks>
        '''
        <ExportAPI("printf", Info:="Output the string to the console using a specific formation.")>
        Public Sub printf(s As String, ParamArray args As Object())
            s = sprintf(s, args)

            Console.Write(s)
            Call Trace.Write(s)
            Call Debug.Write(s)
        End Sub
#End Region

        <Extension>
        Public Sub fprintf(Destination As TextWriter, Format As String, ParamArray Parameters As Object())
            Destination.Write(sprintf(Format, Parameters))
        End Sub

        Public Sub print(s As String, Optional color As ConsoleColor = ConsoleColor.White)
            Dim cl As ConsoleColor = Console.ForegroundColor
            Console.ForegroundColor = color
            Console.Write(s)
            Console.ForegroundColor = cl
        End Sub

        ''' <summary>
        ''' 不换行
        ''' </summary>
        ''' <param name="out"></param>
        ''' <remarks></remarks>
        Public Sub cat(ParamArray out As String())
            Dim s As String = String.Join("", out)
            Call Console.Write(s)
        End Sub

        ''' <summary>
        ''' Read the string that user input on the console to the function paramenter.
        ''' (将用户在终端窗口之上输入的数据赋值给一个字符串变量)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function scanf(ByRef s As String, Optional color As ConsoleColor = ConsoleColor.White) As String
            Dim cl As ConsoleColor = Console.ForegroundColor
            Call Console.Write(s)
            Console.ForegroundColor = color
            s = Console.ReadLine
            Console.ForegroundColor = cl
            Return s
        End Function

        <ExportAPI("Pause")>
        Public Function Pause() As Integer
            Return Console.Read()
        End Function

        Const ____ZERO As String =
                "000000000000000000000000000000000000000000000000000000000000" &
                "000000000000000000000000000000000000000000000000000000000000" &
                "000000000000000000000000000000000000000000000000000000000000" &
                "000000000000000000000000000000000000000000000000000000000000" &
                "000000000000000000000000000000000000000000000000000000000000"

        ''' <summary>
        ''' Fill the number string with specific length of ZERO sequence to generates the fixed width string.
        ''' </summary>
        ''' <param name="sn"></param>
        ''' <param name="len"></param>
        ''' <returns></returns>
        <ExportAPI("ZeroFill", Info:="Fill the number string with specific length of ZERO sequence to generates the fixed width string.")>
        Public Function ZeroFill(sn As String, len As Integer) As String
            If sn.Length >= len Then
                Return sn
            Else
                Dim d As Integer = len - sn.Length
                Dim zr As String = ____ZERO.Substring(0, d)
                Return zr & sn
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="prompt"></param>
        ''' <param name="style">
        ''' Value just allow:
        ''' <see cref="MsgBoxStyle.AbortRetryIgnore"/>,
        ''' <see cref="MsgBoxStyle.OkCancel"/>,
        ''' <see cref="MsgBoxStyle.OkOnly"/>,
        ''' <see cref="MsgBoxStyle.RetryCancel"/>,
        ''' <see cref="MsgBoxStyle.YesNo"/>,
        ''' <see cref="MsgBoxStyle.YesNoCancel"/></param>
        ''' <returns></returns>
        Public Function MsgBox(prompt As String, Optional style As MsgBoxStyle = MsgBoxStyle.YesNo) As MsgBoxResult
            Dim [default] As String = ""

            Call Console.WriteLine(prompt)

            If style.HasFlag(MsgBoxStyle.AbortRetryIgnore) Then
                Call Console.Write("Abort/Retry/Ignore?(a/r/i) [R]")
                [default] = "R"
            ElseIf style.HasFlag(MsgBoxStyle.OkCancel) Then
                Call Console.Write("Ok/Cancel?(o/c) [O]")
                [default] = "O"
            ElseIf style.HasFlag(MsgBoxStyle.OkOnly) Then
                Call Console.WriteLine("Press any key to continute...")
                Call Console.ReadKey()
                Return MsgBoxResult.Ok
            ElseIf style.HasFlag(MsgBoxStyle.RetryCancel) Then
                Call Console.Write("Retry/Cancel?(r/c) [R]")
                [default] = "R"
            ElseIf style.HasFlag(MsgBoxStyle.YesNo) Then
                Call Console.Write("Yes/No?(y/n) [Y]")
                [default] = "Y"
            ElseIf style.HasFlag(MsgBoxStyle.YesNoCancel) Then
                Call Console.Write("Yes/No/Cancel?(y/n/c) [Y]")
                [default] = "Y"
            End If

            Call Console.Write("  ")

            Dim input As String = Console.ReadLine
            If String.IsNullOrEmpty(input) Then
                input = [default]
            Else
                input = input.ToUpper
            End If

            If style.HasFlag(MsgBoxStyle.AbortRetryIgnore) Then
                If __testEquals(input, "A"c) Then
                    Return MsgBoxResult.Abort
                ElseIf __testEquals(input, "R"c) Then
                    Return MsgBoxResult.Retry
                ElseIf __testEquals(input, "I"c) Then
                    Return MsgBoxResult.Ignore
                Else
                    Return MsgBoxResult.Retry
                End If
            ElseIf style.HasFlag(MsgBoxStyle.OkCancel) Then

                If __testEquals(input, "O"c) Then
                    Return MsgBoxResult.Ok
                ElseIf __testEquals(input, "C"c) Then
                    Return MsgBoxResult.Cancel
                Else
                    Return MsgBoxResult.Ok
                End If
            ElseIf style.HasFlag(MsgBoxStyle.OkOnly) Then
                Return MsgBoxResult.Ok
            ElseIf style.HasFlag(MsgBoxStyle.RetryCancel) Then

                If __testEquals(input, "R"c) Then
                    Return MsgBoxResult.Retry
                ElseIf __testEquals(input, "C"c) Then
                    Return MsgBoxResult.Cancel
                Else
                    Return MsgBoxResult.Retry
                End If
            ElseIf style.HasFlag(MsgBoxStyle.YesNo) Then

                If __testEquals(input, "Y"c) Then
                    Return MsgBoxResult.Yes
                ElseIf __testEquals(input, "N"c) Then
                    Return MsgBoxResult.No
                Else
                    Return MsgBoxResult.Yes
                End If
            ElseIf style.HasFlag(MsgBoxStyle.YesNoCancel) Then

                If __testEquals(input, "Y"c) Then
                    Return MsgBoxResult.Yes
                ElseIf __testEquals(input, "N"c) Then
                    Return MsgBoxResult.No
                ElseIf __testEquals(input, "C"c) Then
                    Return MsgBoxResult.Cancel
                Else
                    Return MsgBoxResult.Yes
                End If
            Else
                Return MsgBoxResult.Ok
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="compare">大写的</param>
        ''' <returns></returns>
        Private Function __testEquals(input As String, compare As Char) As Boolean
            If String.IsNullOrEmpty(input) Then
                Return False
            End If
            Return Asc(input.First) = Asc(compare)
        End Function

        Public Delegate Function TryParseDelegate(Of T)(str As String, ByRef val As T) As Boolean

        ''' <summary>
        ''' Read Method with Generics &amp; Delegate
        ''' 
        ''' In a console application there is often the need to ask (and validate) some data from users. 
        ''' For this reason I have created a function that make use of generics and delegates to 
        ''' speed up programming.
        ''' 
        ''' > http://www.codeproject.com/Tips/1108772/Read-Method-with-Generics-Delegate
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="msg"></param>
        ''' <param name="parser"></param>
        ''' <param name="_default"></param>
        ''' <returns></returns>
        Public Function Read(Of T)(msg As String, parser As TryParseDelegate(Of T), Optional _default As T = Nothing) As T
            Dim line As String
            Dim value As T
            Do
                Console.Write(msg & ": ")
                line = Console.ReadLine()
                If String.IsNullOrWhiteSpace(line) Then
                    line = _default?.ToString()
                End If
            Loop While Not parser(line, value)
            Return value
        End Function
    End Module
End Namespace
