﻿#Region "Microsoft.VisualBasic::39f8d85e2b9e003e83601b9849d357a5, ..\Microsoft.VisualBasic.Architecture.Framework\Serialization\JSON\Formatter\Strategies\SkipWhileNotInStringStrategy.vb"

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

Namespace Serialization.JSON.Formatter.Internals.Strategies
    Friend NotInheritable Class SkipWhileNotInStringStrategy
        Implements ICharacterStrategy
        Private ReadOnly selectionCharacter As Char

        Public Sub New(selectionCharacter As Char)
            Me.selectionCharacter = selectionCharacter
        End Sub

        Public Sub Execute(context As JsonFormatterStrategyContext) Implements ICharacterStrategy.Execute
            If context.IsProcessingString Then
                context.AppendCurrentChar()
            End If
        End Sub

        Public ReadOnly Property ForWhichCharacter() As Char Implements ICharacterStrategy.ForWhichCharacter
            Get
                Return Me.selectionCharacter
            End Get
        End Property
    End Class
End Namespace
