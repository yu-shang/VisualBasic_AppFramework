﻿#Region "Microsoft.VisualBasic::a11615f874cf8141f7ecef3bd079fab7, ..\VisualBasic_AppFramework\DocumentFormats\VB_HTML\VB_HTML\YAML\Syntax\YamlDocument.vb"

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

Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace YAML.Syntax

    Public Class YamlDocument

        Public Root As DataItem

        Public Directives As New List(Of Directive)()
        Public AnchoredItems As New Dictionary(Of String, DataItem)()


    End Class
End Namespace
