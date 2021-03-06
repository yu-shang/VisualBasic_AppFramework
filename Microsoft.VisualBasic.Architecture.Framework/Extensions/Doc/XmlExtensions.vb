﻿#Region "Microsoft.VisualBasic::08a027d02762a8d596e4fa51319dcba5, ..\Microsoft.VisualBasic.Architecture.Framework\Extensions\Doc\XmlExtensions.vb"

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

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml

<PackageNamespace("Doc.Xml", Description:="Tools for read and write sbml, KEGG document, etc, xml based documents...")>
Public Module XmlExtensions

    ''' <summary>
    ''' 这个函数主要是用作于Linq里面的Select语句拓展的，这个函数永远也不会报错，只会返回空值
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function SafeLoadXml(Of T)(xml As String,
                                      Optional encoding As Encodings = Encodings.Default,
                                      Optional preProcess As Func(Of String, String) = Nothing) As T
        Return xml.LoadXml(Of T)(encoding.GetEncodings, False, preProcess)
    End Function

    ''' <summary>
    ''' Load class object from the exists Xml document.(从文件之中加载XML之中的数据至一个对象类型之中)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="XmlFile">The path of the xml document.(XML文件的文件路径)</param>
    ''' <param name="ThrowEx">
    ''' If the deserialization operation have throw a exception, then this function should process this error automatically or just throw it?
    ''' (当反序列化出错的时候是否抛出错误？假若不抛出错误，则会返回空值)
    ''' </param>
    ''' <param name="preprocess">
    ''' The preprocessing on the xml document text, you can doing the text replacement or some trim operation from here.(Xml文件的预处理操作)
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function LoadXml(Of T)(XmlFile As String,
                                              Optional encoding As Encoding = Nothing,
                                              Optional ThrowEx As Boolean = True,
                                              Optional preprocess As Func(Of String, String) = Nothing) As T
        Dim type As Type = GetType(T)
        Dim obj As Object = XmlFile.LoadXml(type, encoding, ThrowEx, preprocess)
        If obj Is Nothing Then
            Return Nothing  ' 由于在底层函数之中已经将错误给处理掉了，所以这里直接返回
        Else
            Return DirectCast(obj, T)
        End If
    End Function


    ''' <summary>
    ''' 从文件之中加载XML之中的数据至一个对象类型之中
    ''' </summary>
    ''' <param name="XmlFile">XML文件的文件路径</param>
    ''' <param name="ThrowEx">当反序列化出错的时候是否抛出错误？假若不抛出错误，则会返回空值</param>
    ''' <param name="preprocess">Xml文件的预处理操作</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("LoadXml")>
    <Extension> Public Function LoadXml(XmlFile As String, type As Type,
                                        Optional encoding As Encoding = Nothing,
                                        Optional ThrowEx As Boolean = True,
                                        Optional preprocess As Func(Of String, String) = Nothing) As Object
        If encoding Is Nothing Then encoding = Encoding.Default

        If (Not XmlFile.FileExists) OrElse FileIO.FileSystem.GetFileInfo(XmlFile).Length = 0 Then
            Dim exMsg As String =
                $"{XmlFile.ToFileURL} is not exists on your file system or it is ZERO length content!"
            Dim ex As New Exception(exMsg)
            Call App.LogException(ex)
            If ThrowEx Then
                Throw ex
            Else
                Return Nothing
            End If
        End If

        Dim XmlDoc As String = IO.File.ReadAllText(XmlFile, encoding)

        If Not preprocess Is Nothing Then
            XmlDoc = preprocess(XmlDoc)
        End If

        Using Stream As New StringReader(s:=XmlDoc)
            Try
                Dim obj = New XmlSerializer(type).Deserialize(Stream)
                Return obj
            Catch ex As Exception
                ex = New Exception(type.FullName, ex)
                ex = New Exception(XmlFile.ToFileURL, ex)

                Call App.LogException(ex, MethodBase.GetCurrentMethod.GetFullName)
#If DEBUG Then
                Call ex.PrintException
#End If
                If ThrowEx Then
                    Throw ex
                Else
                    Return Nothing
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' Serialization the target object type into a XML document.(将一个类对象序列化为XML文档)
    ''' </summary>
    ''' <typeparam name="T">The type of the target object data should be a class object.(目标对象类型必须为一个Class)</typeparam>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetXml(Of T As Class)(obj As T,
                                                      Optional ThrowEx As Boolean = True,
                                                      Optional xmlEncoding As XmlEncodings = XmlEncodings.UTF16) As String
        Return GetXml(obj, GetType(T), ThrowEx, xmlEncoding)
    End Function

    Public Function GetXml(obj As Object, type As Type,
                           Optional throwEx As Boolean = True,
                           Optional xmlEncoding As XmlEncodings = XmlEncodings.UTF16) As String
        Try

            If xmlEncoding = XmlEncodings.UTF8 Then
                Dim serializer As New XmlSerializer(type)

                ' create a MemoryStream here, we are just working
                ' exclusively in memory
                Dim stream As New MemoryStream()

                ' The XmlTextWriter takes a stream And encoding
                ' as one of its constructors
                Dim xtWriter As New XmlTextWriter(stream, Encoding.UTF8)

                Call serializer.Serialize(xtWriter, obj)
                Call xtWriter.Flush()

                ' read back the contents of the stream And supply the encoding
                Dim result As String = Encoding.UTF8.GetString(stream.ToArray())
                Return result
            Else
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                Using StreamWriter As StringWriter = New StringWriter(sb:=sBuilder)
                    Call (New XmlSerializer(type)).Serialize(StreamWriter, obj)
                    Return sBuilder.ToString
                End Using
            End If

        Catch ex As Exception
            ex = New Exception(type.ToString, ex)
            Call App.LogException(ex)

#If DEBUG Then
            Call ex.PrintException
#End If

            If throwEx Then
                Throw ex
            Else
                Return Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' Save the object as the XML document.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="saveXml"></param>
    ''' <param name="throwEx"></param>
    ''' <param name="encoding"></param>
    ''' <returns></returns>
    <Extension> Public Function SaveAsXml(Of T As Class)(obj As T, saveXml As String,
                                                         Optional throwEx As Boolean = True,
                                                         Optional encoding As Encoding = Nothing,
                                                         <CallerMemberName> Optional caller As String = "") As Boolean
        Dim xmlDoc As String = obj.GetXml(throwEx)
        Try
            Return xmlDoc.SaveTo(saveXml, encoding)
        Catch ex As Exception
            ex = New Exception(caller, ex)
            If throwEx Then
                Throw ex
            Else
                Call App.LogException(ex)
                Call ex.PrintException
                Return False
            End If
        End Try
    End Function

    <ExportAPI("Xml.GetAttribute")>
    <Extension> Public Function GetXmlAttrValue(strData As String, Name As String) As String
        Dim m As Match = Regex.Match(strData, Name & "=(("".+?"")|[^ ]*)")
        If Not m.Success Then Return ""

        strData = m.Value.Replace(Name & "=", "")
        If strData.First = """"c AndAlso strData.Last = """"c Then
            strData = Mid(strData, 2, Len(strData) - 2)
        End If
        Return strData
    End Function

    ''' <summary>
    ''' Generate a specific type object from a xml document stream.(使用一个XML文本内容创建一个XML映射对象)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Xml">This parameter value is the document text of the xml file, not the file path of the xml file.(是Xml文件的文件内容而非文件路径)</param>
    ''' <param name="ThrowEx">Should this program throw the exception when the xml deserialization error happens?
    ''' if False then this function will returns a null value instead of throw exception.
    ''' (在进行Xml反序列化的时候是否抛出错误，默认抛出错误，否则返回一个空对象)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function CreateObjectFromXml(Of T As Class)(Xml As String, Optional ThrowEx As Boolean = True) As T
        Using Stream As New StringReader(s:=Xml)
            Try
                Dim type As Type = GetType(T)
                Dim o As Object = New XmlSerializer(type).Deserialize(Stream)
                Return DirectCast(o, T)
            Catch ex As Exception
                Dim curMethod As String = MethodBase.GetCurrentMethod.GetFullName
                ex = New Exception(Xml, ex)
                App.LogException(ex, curMethod)

                If ThrowEx Then
                    Throw ex
                Else
                    Return Nothing
                End If
            End Try
        End Using
    End Function

    <ExportAPI("Xml.CreateObject")>
    <Extension> Public Function CreateObjectFromXml(Xml As StringBuilder, typeInfo As Type) As Object
        Dim doc As String = Xml.ToString

        Using Stream As New StringReader(doc)
            Try
                Dim obj As Object = New XmlSerializer(typeInfo).Deserialize(Stream)
                Return obj
            Catch ex As Exception
                ex = New Exception(doc, ex)
                ex = New Exception(typeInfo.FullName, ex)

                Call App.LogException(ex)

                Throw ex
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 使用一个XML文本内容的一个片段创建一个XML映射对象
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Xml">是Xml文件的文件内容而非文件路径</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function CreateObjectFromXmlFragment(Of T As Class)(Xml As String) As T
        Using Stream As New StringReader(s:="<?xml version=""1.0""?>" & vbCrLf & Xml)
            Return DirectCast(New XmlSerializer(GetType(T)).Deserialize(Stream), T)
        End Using
    End Function

    <Extension>
    Public Function SetXmlEncoding(xml As String, encoding As XmlEncodings) As String
        Dim xmlEncoding As String = Text.Xml.XmlDeclaration.XmlEncodingString(encoding)
        Dim head As String = Regex.Match(xml, XmlDoc.XmlDeclares, RegexICSng).Value
        Dim enc As String = Regex.Match(head, "encoding=""\S+""", RegexICSng).Value

        If String.IsNullOrEmpty(enc) Then
            enc = head.Replace("?>", $" encoding=""{xmlEncoding}""?>")
        Else
            enc = head.Replace(enc, $"encoding=""{xmlEncoding}""")
        End If

        xml = xml.Replace(head, enc)

        Return xml
    End Function

    <Extension>
    Public Function SetXmlStandalone(xml As String, standalone As Boolean) As String
        Dim opt As String = Text.Xml.XmlDeclaration.XmlStandaloneString(standalone)
        Dim head As String = Regex.Match(xml, XmlDoc.XmlDeclares, RegexICSng).Value
        Dim enc As String = Regex.Match(head, "standalone=""\S+""", RegexICSng).Value

        If String.IsNullOrEmpty(enc) Then
            enc = head.Replace("?>", $" standalone=""{opt}""?>")
        Else
            enc = head.Replace(enc, $"standalone=""{opt}""")
        End If

        xml = xml.Replace(head, enc)

        Return xml
    End Function
End Module
