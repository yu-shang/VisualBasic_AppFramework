﻿#Region "Microsoft.VisualBasic::d3c0f7e59dc858a76753e67ef244f1c8, ..\VisualBasic_AppFramework\Datavisualization\Microsoft.VisualBasic.Imaging\Drawing2D\VectorElements\LayoutsElement.vb"

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
Imports Microsoft.VisualBasic.Imaging

Namespace Drawing2D.VectorElements

    Public MustInherit Class LayoutsElement

        Public Property Location As Point
        Public Property TooltipTag As String

        Public MustOverride ReadOnly Property Size As Size

        Public ReadOnly Property DrawingRegion As Rectangle
            Get
                Return New Rectangle(Location, Size)
            End Get
        End Property

        Protected _GDIDevice As GDIPlusDeviceHandle

        ''' <summary>
        ''' 默认是允许自动组织布局的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EnableAutoLayout As Boolean = True

        Sub New(GDI As GDIPlusDeviceHandle, InitLoci As Point)
            _GDIDevice = GDI
            Location = InitLoci
        End Sub

        Protected MustOverride Sub InvokeDrawing()

        Public Function MoveTo(pt As Point) As LayoutsElement
            Location = pt
            Return Me
        End Function

        Public Function MoveOffset(offset As Point) As LayoutsElement
            Location = New Point(Location.X + offset.X, Location.Y + offset.Y)
            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OverridesLoci">假若需要进行绘制到的时候复写当前的元素的位置，则请使用这个参数</param>
        ''' <returns>函数返回当前元素在绘制之后所占据的区域</returns>
        ''' <remarks></remarks>
        Public Function InvokeDrawing(Optional OverridesLoci As Point = Nothing) As Rectangle

            If Not OverridesLoci.IsEmpty Then
                Me.Location = OverridesLoci
            End If

            Call InvokeDrawing()

            Return New Rectangle(Me.Location, Me.Size)
        End Function

        Public Overrides Function ToString() As String
            Return DrawingRegion.ToString
        End Function
    End Class
End Namespace
