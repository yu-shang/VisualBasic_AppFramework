﻿#Region "Microsoft.VisualBasic::632a68ae133c4260c92b3a5e993a9576, ..\VisualBasic_AppFramework\Datavisualization\Microsoft.VisualBasic.Imaging\Drawing2D\VectorElements\Circle.vb"

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

    Public Class Circle : Inherits LayoutsElement

        Dim _Size As Size
        Dim Brush As SolidBrush

        Public Property FillColor As Color
            Get
                Return Brush.Color
            End Get
            Set(value As Color)
                Brush = New SolidBrush(value)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="LeftTop">左上角</param>
        ''' <param name="D">圆的直径</param>
        ''' <remarks></remarks>
        Friend Sub New(LeftTop As Point, D As Integer, GDI As GDIPlusDeviceHandle, FillColor As Color)
            Call MyBase.New(GDI, LeftTop)
            _Size = New Size(D, D)
            Me.FillColor = FillColor
        End Sub

        Protected Overloads Overrides Sub InvokeDrawing()
            Call Me._GDIDevice.Graphics.FillPie(Me.Brush, Me.DrawingRegion, 0, 360)
        End Sub

        Public Overrides ReadOnly Property Size As Size
            Get
                Return _Size
            End Get
        End Property
    End Class
End Namespace
