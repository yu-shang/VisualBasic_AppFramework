﻿#Region "Microsoft.VisualBasic::5348c906a27b2ef98852b4e700349f55, ..\VisualBasic_AppFramework\DocumentFormats\VB_DataFrame\VB_DataFrame\DocumentStream\NetStream\StreamHelper.vb"

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

Imports Microsoft.VisualBasic.Text

Namespace DocumentStream.NetStream

    Module StreamHelper

        Public Function GetBytes(x As RowObject) As Byte()
            Return x.Serialize
        End Function

        Public Function LoadHelper(encoding As Encodings) As Func(Of Byte(), RowObject)
            Dim helper As New EncodingHelper(encoding)
            Return AddressOf New __load(helper).Load
        End Function

        Private Class __load

            ReadOnly __encoding As EncodingHelper

            Sub New(encoding As EncodingHelper)
                __encoding = encoding
            End Sub

            Public Function Load(byts As Byte()) As RowObject
                Return New RowObject(byts, __encoding)
            End Function
        End Class
    End Module
End Namespace
