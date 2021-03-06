﻿#Region "Microsoft.VisualBasic::6925a27aafe797b55648f1419558a5a3, ..\Microsoft.VisualBasic.Architecture.Framework\Tools\Network\Protocol\Abstract.vb"

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

Imports Microsoft.VisualBasic.Serialization

Namespace Net.Protocols

    ''' <summary>
    ''' 支持序列化的对象，则这个对象可以被应用于<see cref="RequestStream"/>数据载体的网络传输操作过程
    ''' </summary>
    Public Interface ISerializable
        ''' <summary>
        ''' Transform this .NET object into a raw stream object for the network data transfer. 
        ''' </summary>
        ''' <returns></returns>
        Function Serialize() As Byte()
    End Interface

    ''' <summary>
    ''' 原始串流的基本模型，这个流对象应该具备有两个基本的方法：
    ''' 1. 从原始的字节流之中反序列化构造出自身的构造函数
    ''' 2. 将自身序列化为字节流的<see cref="ISerializable.Serialize()"/>序列化方法
    ''' </summary>
    <Serializable> Public MustInherit Class RawStream : Implements ISerializable

        ''' <summary>
        ''' You should overrides this constructor to generate a stream object.(必须要有一个这个构造函数来执行反序列化)
        ''' </summary>
        ''' <param name="rawStream"></param>
        Sub New(rawStream As Byte())

        End Sub

        Public Sub New()
        End Sub

        ''' <summary>
        ''' <see cref="ISerializable.Serialize"/>序列化方法
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function Serialize() As Byte() Implements ISerializable.Serialize

        ''' <summary>
        ''' 按照类型的定义进行反序列化操作
        ''' </summary>
        ''' <typeparam name="TRawStream"></typeparam>
        ''' <param name="rawStream"></param>
        ''' <returns></returns>
        Public Shared Function GetRawStream(Of TRawStream As RawStream)(rawStream As Byte()) As TRawStream
            Dim value As Object = Activator.CreateInstance(GetType(TRawStream), {rawStream})
            Return DirectCast(value, TRawStream)
        End Function

        Protected Shared ReadOnly Property _rawStreamType As Type = GetType(Byte())

        Public Const INT64 As Integer = 8
        ''' <summary>
        ''' Single/Integer
        ''' </summary>
        Public Const INT32 As Integer = 4
        ''' <summary>
        ''' System.Double
        ''' </summary>
        Public Const DblFloat As Integer = 8
        Public Const ShortInt As Integer = 2
        Public Const SingleFloat As Integer = 4
        Public Const DecimalInt As Integer = 12

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="addr">IPEndPoint string value likes 127.0.0.1:8080</param>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        ''' <![CDATA[
        '''
        ''' Dim rep As RequestStream =
        '''    "127.0.0.1:80" <= New RequestStream With {
        '''           ...
        '''     }
        ''' ]]>
        Public Shared Operator <=(addr As String, raw As RawStream) As RequestStream
            Dim ep As New IPEndPoint(addr)
            Dim invoke As New AsynInvoke(ep)
            Dim rep As RequestStream = New RequestStream(invoke.SendMessage(raw.Serialize))
            Return rep
        End Operator

        Public Shared Operator >=(addr As String, raw As RawStream) As RequestStream
            Throw New NotSupportedException
        End Operator
    End Class

    Public Delegate Function LoadObject(Of T)(source As String) As T
End Namespace
