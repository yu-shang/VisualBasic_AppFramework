﻿#Region "Microsoft.VisualBasic::7d177df3a5c0c424dc2b78812b6aa34c, ..\Microsoft.VisualBasic.Architecture.Framework\Tools\Network\Abstract.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection

Namespace Net.Abstract

    ''' <summary>
    ''' Object for handles the request <see cref="Protocol"/>.
    ''' </summary>
    Public MustInherit Class IProtocolHandler

        MustOverride ReadOnly Property ProtocolEntry As Long
        MustOverride Function HandleRequest(CA As Long, request As RequestStream, remoteDevcie As System.Net.IPEndPoint) As RequestStream
    End Class

#Region "Delegate Abstract Interface"

    Public Delegate Sub ForceCloseHandle(socket As StateObject)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CA"><see cref="SSL.Certificate.uid"/></param>
    ''' <param name="request"></param>
    ''' <param name="RemoteAddress"></param>
    ''' <returns></returns>
    Public Delegate Function DataRequestHandler(CA As Long, request As RequestStream, RemoteAddress As System.Net.IPEndPoint) As RequestStream

    ''' <summary>
    ''' 处理错误的工作逻辑的抽象接口
    ''' </summary>
    ''' <param name="ex">Socket的内部错误信息</param>
    ''' <remarks></remarks>
    Public Delegate Sub ExceptionHandler(ex As Exception)
#End Region

    ''' <summary>
    ''' Socket listening object which is running at the server side asynchronous able multiple threading.
    ''' (运行于服务器端上面的Socket监听对象，多线程模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IServicesSocket : Inherits IDisposable, IObjectModel_Driver, IDataRequestHandler

        ''' <summary>
        ''' The server services listening on this local port.(当前的这个服务器对象实例所监听的本地端口号)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property LocalPort As Integer
        ReadOnly Property IsShutdown As Boolean
        ReadOnly Property IsRunning As Boolean

        ''' <summary>
        ''' This server waits for a connection and then uses  asychronous operations to
        ''' accept the connection, get data from the connected client,
        ''' echo that data back to the connected client.
        ''' It then disconnects from the client and waits for another client.(请注意，当服务器的代码运行到这里之后，代码将被阻塞在这里)
        ''' </summary>
        ''' <remarks></remarks>
        Overloads Function Run() As Integer

        ''' <summary>
        ''' This server waits for a connection and then uses  asychronous operations to
        ''' accept the connection, get data from the connected client,
        ''' echo that data back to the connected client.
        ''' It then disconnects from the client and waits for another client.(请注意，当服务器的代码运行到这里之后，代码将被阻塞在这里)
        ''' </summary>
        ''' <remarks></remarks>
        Overloads Function Run(localEndPoint As System.Net.IPEndPoint) As Integer
    End Interface

    Public Interface IDataRequestHandler

        ''' <summary>
        ''' This function pointer using for the data request handling of the data request from the client socket.
        ''' (这个函数指针用于处理来自于客户端的请求)
        ''' </summary>
        ''' <remarks></remarks>
        Property Responsehandler As Net.Abstract.DataRequestHandler
    End Interface
End Namespace
