﻿#Region "Microsoft.VisualBasic::ee027621fc8a4ed6ee864059abb64dec, ..\VisualBasic_AppFramework\Scripting\Math\Math\Helpers\MetaExpression.vb"

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

''' <summary>
''' 
''' </summary>
''' <typeparam name="T">Token type</typeparam>
''' <typeparam name="O">Operator type</typeparam>
Public Class MetaExpression(Of T, O)

    Public Property [Operator] As O

    ''' <summary>
    ''' 自动根据类型来计算出结果
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Property LEFT As T

    Sub New()
    End Sub

    Sub New(x As T)
        LEFT = x
    End Sub
End Class
