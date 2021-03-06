﻿#Region "Microsoft.VisualBasic::9f91116fddd59255b38e1be4490364bb, ..\VisualBasic_AppFramework\Datavisualization\Datavisualization.Network\NetworkCanvas\SVG.vb"

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
Imports Microsoft.VisualBasic.DataVisualization.Network.Graph
Imports Microsoft.VisualBasic.DataVisualization.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.SVG
Imports Microsoft.VisualBasic.Imaging.SVG.CSS
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MarkupLanguage.HTML
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.DataVisualization.Network.Layouts.Interfaces

''' <summary>
''' <see cref="NetworkGraph"/> to svg doc
''' </summary>
Public Module SVGExtensions

    Public Function DefaultStyle() As CSS.DirectedForceGraph
        Return New DirectedForceGraph With {
            .link = New CssValue With {
                .stroke = "#CCC",
                .strokeOpacity = "0.85",
                .strokeWidth = "6"
            },
            .node = New CssValue With {
                .strokeWidth = "0.5px",
                .strokeOpacity = "0.8",
                .stroke = "#FFF",
                .opacity = "0.85"
            }
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <param name="style">Default value is <see cref="DefaultStyle"/></param>
    ''' <param name="size">The export canvas size</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToSVG(graph As NetworkGraph, size As Size, Optional style As CSS.DirectedForceGraph = Nothing) As SVGXml
        Dim rect As New Rectangle(New Point, size)
        Dim nodes As SVG.circle() =
            LinqAPI.Exec(Of SVG.circle) <= From n As Graph.Node
                                           In graph.nodes
                                           Let pos As Point = Renderer.GraphToScreen(TryCast(n.Data.initialPostion, FDGVector2), rect)
                                           Let c As Color = If(
                                               TypeOf n.Data.Color Is SolidBrush,
                                               DirectCast(n.Data.Color, SolidBrush).Color,
                                               Color.Black)
                                           Let r As Single = n.__getRadius
                                           Let pt = New Point(CInt(pos.X - r / 2), CInt(pos.Y - r / 2))
                                           Select New circle With {
                                               .class = "node",
                                               .cx = pt.X,
                                               .cy = pt.Y,
                                               .r = r,
                                               .style = $"fill: rgb({c.R}, {c.G}, {c.B});"
                                           }
        Dim links As line() =
            LinqAPI.Exec(Of line) <= From edge As Edge
                                     In graph.edges
                                     Let source As Graph.Node = edge.Source
                                     Let target As Graph.Node = edge.Target
                                     Let pts As Point = Renderer.GraphToScreen(TryCast(source.Data.initialPostion, FDGVector2), rect)
                                     Let ptt As Point = Renderer.GraphToScreen(TryCast(target.Data.initialPostion, FDGVector2), rect)
                                     Let rs As Single = source.__getRadius / 2,
                                         rt As Single = target.__getRadius / 2
                                     Select New line With {
                                         .class = "link",
                                         .x1 = pts.X - rs,
                                         .x2 = ptt.X - rt,
                                         .y1 = pts.Y - rs,
                                         .y2 = ptt.Y - rt
                                     }
        Dim svg As New SVGXml With {
            .defs = New CSSStyles With {
                .styles = {
                    New XmlMeta.CSS With {
                        .style = If(style Is Nothing, DefaultStyle(), style).ToString
                    }
                }
            },
            .width = size.Width & "px",
            .height = size.Height & "px",
            .lines = links,
            .circles = nodes,
            .fill = "#dbf3ff"
        }

        Return svg
    End Function

    <Extension>
    Private Function __getRadius(n As Graph.Node) As Single
        Dim r As Single = n.Data.radius
        Dim rd As Single = If(r = 0!, If(n.Data.Neighborhoods < 30, n.Data.Neighborhoods * 9, n.Data.Neighborhoods * 7), r)
        Dim r2 As Single = If(rd = 0, 10.0!, rd) / 2.5!

        Return r2
    End Function

    <Extension>
    Public Sub WriteLayouts(ByRef graph As NetworkGraph, engine As IForceDirected)
        For Each node As Graph.Node In graph.nodes
            node.Data.initialPostion =
                New FDGVector2(engine.GetPoint(node).position.Point2D)
        Next
    End Sub
End Module
