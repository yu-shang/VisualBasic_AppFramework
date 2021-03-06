﻿#Region "Microsoft.VisualBasic::cfb5214173c998d9880aaa457d82f81e, ..\Microsoft.VisualBasic.Architecture.Framework\Scripting\InputHandler.vb"

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

Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Imaging

Namespace Scripting

    ''' <summary>
    ''' 转换从终端或者脚本文件之中输入的字符串的类型的转换
    ''' </summary>
    Public Module InputHandler

        ''' <summary>
        ''' 在脚本编程之中将用户输入的字符串数据转换为目标类型的方法接口
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Delegate Function LoadObject(value As String) As Object
        Public Delegate Function LoadObject(Of T)(value As String) As T

        ''' <summary>
        ''' Object为字符串类型，这个字典可以讲字符串转为目标类型
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property CasterString As Dictionary(Of Type, Func(Of String, Object)) =
            New Dictionary(Of Type, Func(Of String, Object)) From {
                {GetType(String), AddressOf Casting.CastString},
                {GetType(Char), AddressOf Casting.CastChar},
                {GetType(Integer), AddressOf Casting.CastInteger},
                {GetType(Double), AddressOf Casting.CastDouble},
                {GetType(Long), AddressOf Casting.CastLong},
                {GetType(Boolean), AddressOf Casting.CastBoolean},
                {GetType(Char()), AddressOf Casting.CastCharArray},
                {GetType(Date), AddressOf Casting.CastDate},
                {GetType(StringBuilder), AddressOf Casting.CastStringBuilder},
                {GetType(CommandLine.CommandLine), AddressOf Casting.CastCommandLine},
                {GetType(Image), AddressOf Casting.CastImage},
                {GetType(FileInfo), AddressOf Casting.CastFileInfo},
                {GetType(GDIPlusDeviceHandle), AddressOf Casting.CastGDIPlusDeviceHandle},
                {GetType(Color), AddressOf Casting.CastColor},
                {GetType(Font), AddressOf Casting.CastFont},
                {GetType(System.Net.IPEndPoint), AddressOf Casting.CastIPEndPoint},
                {GetType(Logging.LogFile), AddressOf Casting.CastLogFile},
                {GetType(Process), AddressOf Casting.CastProcess},
                {GetType(RegexOptions), AddressOf Casting.CastRegexOptions}
        }

        ''' <summary>
        ''' Converts a string expression which was input from the console or script file to the specified type.
        ''' (请注意，函数只是转换最基本的数据类型，转换错误会返回空值)
        ''' </summary>
        ''' <param name="Expression">The string expression to convert.</param>
        ''' <param name="TargetType">The type to which to convert the object.</param>
        ''' <returns>An object whose type at run time is the requested target type.</returns>
        <Extension> Public Function CTypeDynamic(Expression As String, TargetType As Type) As Object
            If _CasterString.ContainsKey(TargetType) Then
                Dim Caster = _CasterString(TargetType)
                Return Caster(Expression)
            End If

            Try
                Return Conversion.CTypeDynamic(Expression, TargetType)
            Catch ex As Exception
                Call App.LogException(
                    New Exception($"{Expression}  ===> {TargetType.FullName}", ex),
                    MethodBase.GetCurrentMethod.GetFullName)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Converts a string expression which was input from the console or script file to the specified type.
        ''' (请注意，函数只是转换最基本的数据类型，转换错误会返回空值)
        ''' </summary>
        ''' <param name="Expression">The string expression to convert.</param>
        ''' <typeparam name="T">The type to which to convert the object.</typeparam>
        ''' <returns>An object whose type at run time is the requested target type.</returns>
        <Extension> Public Function CTypeDynamic(Of T)(Expression As String) As T
            Dim value As Object = CTypeDynamic(Expression, GetType(T))
            If value Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(value, T)
            End If
        End Function

        ''' <summary>
        ''' Does this type can be cast from the <see cref="String"/> type?(目标类型能否由字符串转换过来??)
        ''' </summary>
        ''' <param name="targetType"></param>
        ''' <returns></returns>
        Public Function IsPrimitive(targetType As Type) As Boolean
            Return CasterString.ContainsKey(targetType)
        End Function

        ''' <summary>
        ''' Dynamics updates the capability of function <see cref="InputHandler.CTypeDynamic(String, System.Type)"/>, 
        ''' <see cref="InputHandler.CTypeDynamic(Of T)(String)"/> and 
        ''' <see cref="InputHandler.IsPrimitive(System.Type)"/>
        ''' </summary>
        ''' <param name="briefName"></param>
        ''' <param name="stringConvertType"></param>
        ''' <param name="cast"></param>
        Public Sub CapabilityPromise(briefName As String, stringConvertType As Type, cast As Func(Of String, Object))
            If CasterString.ContainsKey(stringConvertType) Then
                Call CasterString.Remove(stringConvertType)
            End If
            Call CasterString.Add(stringConvertType, cast)
            If Types.ContainsKey(briefName.ToLower.ShadowCopy(briefName)) Then
                Call Types.Remove(briefName)
            End If
            Call Types.Add(briefName, stringConvertType)
        End Sub

        ''' <summary>
        ''' Enumerate all of the types that can be handled in this module. All of the key string is in lower case.(键值都是小写的)
        ''' </summary>
        Public ReadOnly Property Types As SortedDictionary(Of String, Type) =
            New SortedDictionary(Of String, Type) From {
 _
                {"string", GetType(String)},
                {"integer", GetType(Integer)},
                {"int32", GetType(Integer)},
                {"int64", GetType(Long)},
                {"long", GetType(Long)},
                {"double", GetType(Double)},
                {"byte", GetType(Byte)},
                {"date", GetType(Date)},
                {"logfile", GetType(Logging.LogFile)},
                {"color", GetType(Color)},
                {"process", GetType(Process)},
                {"font", GetType(Font)},
                {"image", GetType(Image)},
                {"fileinfo", GetType(IO.FileInfo)},
                {"ipaddress", GetType(System.Net.IPAddress)},
                {"commandline", GetType(CommandLine.CommandLine)},
                {"gdi+", GetType(GDIPlusDeviceHandle)},
                {"stringbuilder", GetType(StringBuilder)},
                {"boolean", GetType(Boolean)},
                {"char()", GetType(Char())},
                {"string()", GetType(String())},
                {"integer()", GetType(Integer())},
                {"double()", GetType(Double())},
                {"bitmap", GetType(Bitmap)},
                {"object", GetType(Object)},
                {"regexoptions", GetType(RegexOptions)}
        }

        ''' <summary>
        ''' 类型获取失败会返回空值，大小写不敏感
        ''' </summary>
        ''' <param name="name">类型的名称简写</param>
        ''' <param name="ObjectGeneric">是否出错的时候返回Object类型，默认返回Nothing</param>
        ''' <returns></returns>
        Public Function [GetType](name As String, Optional ObjectGeneric As Boolean = False) As Type
            name = name.ToLower

            If Types.ContainsKey(name) Then
                Return Types(name)
            Else
                Dim typeInfo As Type = System.Type.GetType(name, False, True)

                If typeInfo Is Nothing AndAlso ObjectGeneric Then
                    Return GetType(Object)
                Else
                    Return typeInfo
                End If
            End If
        End Function

        Public Function [GetType](obj As Object, Optional ObjectGeneric As Boolean = False) As Type
            If obj Is Nothing Then
                If ObjectGeneric Then
                    Return GetType(Object)
                Else
                    Return Nothing
                End If
            Else
                Return obj.GetType
            End If
        End Function

        ''' <summary>
        ''' <see cref="System.Type"/> information for <see cref="System.String"/> type from GetType operator
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [String] As Type = GetType(String)

        ''' <summary>
        ''' Does the <paramref name="inputtype"/> type can be cast to type <paramref name="DefType"/>.(主要为了方便减少脚本编程模块的代码)
        ''' </summary>
        ''' <param name="inputType"></param>
        ''' <param name="DefType"></param>
        ''' <returns></returns>
        Public Function Convertible(inputType As Type, DefType As Type) As Boolean
            Return inputType.Equals([String]) AndAlso CasterString.ContainsKey(DefType)
        End Function

        ''' <summary>
        ''' <seealso cref="ComponentModel.DataSourceModel.__toStringInternal"/>, 出现错误的时候总是会返回空字符串的
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function ToString(obj As Object) As String
            Return ComponentModel.DataSourceModel.__toStringInternal(obj)
        End Function

        ''' <summary>
        ''' The parameter <paramref name="obj"/> should implements a <see cref="System.Collections.IEnumerable"/> interface on the type. and then DirectCast object to target type.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function CastArray(Of T)(obj As Object) As T()
            Dim array = DirectCast(obj, IEnumerable)
            Dim data = (From val In array Select DirectCast(val, T)).ToArray
            Return data
        End Function

        Public Function CastArray(obj As Object, type As Type) As Object
            If Array.IndexOf(obj.GetType.GetInterfaces, GetType(IEnumerable)) = -1 Then
                Return obj
            End If

            Dim source As IEnumerable = DirectCast(obj, IEnumerable)
            Dim data = (From val As Object
                        In source
                        Select Conversion.CTypeDynamic(val, type)).ToArray
            Return [DirectCast](data, type)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="array"></param>
        ''' <param name="type">数组里面的元素的类型</param>
        ''' <returns></returns>
        Public Function [DirectCast](array As Object(), type As Type) As Object
            Dim typedArray = System.Array.CreateInstance(type, array.Length)
            'For i As Integer = 0 To array.Length - 1
            '    Call typedArray.SetValue(array(i), i)
            'Next
            Call System.Array.Copy(array, typedArray, array.Length) '直接复制不能够正常工作

            Return typedArray
        End Function

    End Module
End Namespace
