﻿#Region "Microsoft.VisualBasic::93a6e7963846a458741849c9e50432d5, ..\VisualBasic_AppFramework\Datavisualization\Datavisualization.Network\Datavisualization.Network\LDM\FileStream\Node.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace FileStream

    Public MustInherit Class INetComponent : Inherits DynamicPropertyBase(Of String)

        <Meta(GetType(String))>
        Public Overrides Property Properties As Dictionary(Of String, String)
            Get
                Return MyBase.Properties
            End Get
            Set(value As Dictionary(Of String, String))
                MyBase.Properties = value
            End Set
        End Property

        Public Sub Add(key As String, value As String)
            Call Properties.Add(key, value)
        End Sub
    End Class

    ''' <summary>
    ''' An node entity in the target network.(这个对象里面包含了网络之中的节点的实体的最基本的定义：节点的标识符以及节点的类型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Node : Inherits INetComponent
        Implements sIdEnumerable
        Implements INode

        ''' <summary>
        ''' 这个节点的标识符
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property Identifier As String Implements sIdEnumerable.Identifier, INode.Identifier
        ''' <summary>
        ''' Node data groups identifier.(这个节点的分组类型的定义)
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property NodeType As String Implements INode.NodeType

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Sub New()
        End Sub

        Sub New(sId As String)
            Identifier = sId
        End Sub

        Sub New(sid As String, type As String)
            Call Me.New(sid)
            NodeType = type
        End Sub

        Public Function CopyTo(Of T As Node)() As T
            Dim NewNode As T = Activator.CreateInstance(Of T)()
            NewNode.Identifier = Identifier
            NewNode.NodeType = NodeType

            Return NewNode
        End Function
    End Class
End Namespace
