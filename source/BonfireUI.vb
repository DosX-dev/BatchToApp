Imports System.Drawing.Drawing2D
Imports System.ComponentModel

Module Drawing

    Public Function RoundRect(ByVal rect As Rectangle, ByVal slope As Integer) As GraphicsPath
        Dim gp As GraphicsPath = New GraphicsPath()
        Dim arcWidth As Integer = slope * 2
        gp.AddArc(New Rectangle(rect.X, rect.Y, arcWidth, arcWidth), -180, 90)
        gp.AddArc(New Rectangle(rect.Width - arcWidth + rect.X, rect.Y, arcWidth, arcWidth), -90, 90)
        gp.AddArc(New Rectangle(rect.Width - arcWidth + rect.X, rect.Height - arcWidth + rect.Y, arcWidth, arcWidth), 0, 90)
        gp.AddArc(New Rectangle(rect.X, rect.Height - arcWidth + rect.Y, arcWidth, arcWidth), 90, 90)
        gp.CloseAllFigures()
        Return gp
    End Function

End Module

Class BonfireButton
    Inherits Control

    Enum Style
        Blue
        Dark
        Light
    End Enum

    Private _style As Style
    Public Property ButtonStyle As Style
        Get
            Return _style
        End Get
        Set(ByVal value As Style)
            _style = value
            Invalidate()
        End Set
    End Property

    Private _image As Image
    Public Property Image As Image
        Get
            Return _image
        End Get
        Set(ByVal value As Image)
            _image = value
            Invalidate()
        End Set
    End Property

    Private _rounded As Boolean
    Public Property RoundedCorners As Boolean
        Get
            Return _rounded
        End Get
        Set(ByVal value As Boolean)
            _rounded = value
            Invalidate()
        End Set
    End Property

    Enum State
        None
        Over
        Down
    End Enum

    Private MouseState As State

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        MouseState = State.None
        Size = New Size(65, 26)
        Font = New Font("Verdana", 8)
        Cursor = Cursors.Hand
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim G As Graphics = e.Graphics

        G.Clear(Parent.BackColor)
        G.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

        Dim slope As Integer = 3

        Dim shadowRect As New Rectangle(0, 0, Width - 1, Height - 1)
        Dim shadowPath As GraphicsPath = RoundRect(shadowRect, slope)
        Dim mainRect As New Rectangle(0, 0, Width - 2, Height - 2)
        Select Case ButtonStyle
            Case Style.Blue
                If _rounded Then
                    G.FillPath(New SolidBrush(Color.FromArgb(20, 135, 195)), shadowPath)
                    G.FillPath(New SolidBrush(Color.FromArgb(20, 160, 230)), RoundRect(mainRect, slope))
                Else
                    G.FillRectangle(New SolidBrush(Color.FromArgb(20, 135, 195)), shadowRect)
                    G.FillRectangle(New SolidBrush(Color.FromArgb(20, 160, 230)), mainRect)
                End If
            Case Style.Dark
                If _rounded Then
                    G.FillPath(New SolidBrush(Color.FromArgb(45, 45, 45)), shadowPath)
                    G.FillPath(New SolidBrush(Color.FromArgb(75, 75, 75)), RoundRect(mainRect, slope))
                Else
                    G.FillRectangle(New SolidBrush(Color.FromArgb(45, 45, 45)), shadowRect)
                    G.FillRectangle(New SolidBrush(Color.FromArgb(75, 75, 75)), mainRect)
                End If
            Case Style.Light
                If _rounded Then
                    G.FillPath(New SolidBrush(Color.FromArgb(145, 145, 145)), shadowPath)
                    G.FillPath(New SolidBrush(Color.FromArgb(170, 170, 170)), RoundRect(mainRect, slope))
                Else
                    G.FillRectangle(New SolidBrush(Color.FromArgb(145, 145, 145)), shadowRect)
                    G.FillRectangle(New SolidBrush(Color.FromArgb(170, 170, 170)), mainRect)
                End If
        End Select

        If _image Is Nothing Then
            Dim textX As Integer = ((Me.Width - 1) / 2) - (G.MeasureString(Text, Font).Width / 2)
            Dim textY As Integer = ((Me.Height - 1) / 2) - (G.MeasureString(Text, Font).Height / 2)
            G.DrawString(Text, Font, Brushes.White, textX, textY)
        Else
            Dim textSize As Size = New Size(G.MeasureString(Text, Font).Width, G.MeasureString(Text, Font).Height)
            Dim imageWidth As Integer = Me.Height - 19, imageHeight As Integer = Me.Height - 19
            Dim imageX As Integer = ((Me.Width - 1) / 2) - ((imageWidth + 4 + textSize.Width) / 2)
            Dim imageY As Integer = ((Me.Height - 1) / 2) - (imageHeight / 2)
            G.DrawImage(_image, imageX, imageY, imageWidth, imageHeight)
            G.DrawString(Text, Font, Brushes.White, New Point(imageX + imageWidth + 4, ((Me.Height - 1) / 2) - textSize.Height / 2))
        End If

        If MouseState = State.Over Then
            G.FillPath(New SolidBrush(Color.FromArgb(25, Color.White)), shadowPath)
        ElseIf MouseState = State.Down Then
            G.FillPath(New SolidBrush(Color.FromArgb(40, Color.White)), shadowPath)
        End If

    End Sub

    Protected Overrides Sub OnMouseEnter(ByVal e As System.EventArgs)
        MyBase.OnMouseEnter(e)
        MouseState = State.Over
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
        MyBase.OnMouseLeave(e)
        MouseState = State.None
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseUp(e)
        MouseState = State.Over
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)
        MouseState = State.Down
        Invalidate()
    End Sub
End Class

Class BonfireTabControl
    Inherits TabControl

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or _
                 ControlStyles.UserPaint Or ControlStyles.DoubleBuffer, True)
        ItemSize = New Size(0, 30)
        Font = New Font("Verdana", 8)
    End Sub

    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        Alignment = TabAlignment.Top
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

        Dim G As Graphics = e.Graphics

        Dim borderPen As New Pen(Color.FromArgb(225, 225, 225))

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim fillRect As New Rectangle(2, ItemSize.Height + 2, Width - 6, Height - ItemSize.Height - 3)
        G.FillRectangle(Brushes.White, fillRect)
        G.DrawRectangle(borderPen, fillRect)

        Dim FontColor As New Color

        For i = 0 To TabCount - 1

            Dim mainRect As Rectangle = GetTabRect(i)

            If i = SelectedIndex Then

                G.FillRectangle(New SolidBrush(Color.White), mainRect)
                G.DrawRectangle(borderPen, mainRect)
                G.DrawLine(New Pen(Color.FromArgb(20, 160, 230)), New Point(mainRect.X + 1, mainRect.Y), New Point(mainRect.X + mainRect.Width - 1, mainRect.Y))
                G.DrawLine(Pens.White, New Point(mainRect.X + 1, mainRect.Y + mainRect.Height), New Point(mainRect.X + mainRect.Width - 1, mainRect.Y + mainRect.Height))
                FontColor = Color.FromArgb(20, 160, 230)

            Else

                G.FillRectangle(New SolidBrush(Color.FromArgb(245, 245, 245)), mainRect)
                G.DrawRectangle(borderPen, mainRect)
                FontColor = Color.FromArgb(160, 160, 160)

            End If

            Dim titleX As Integer = (mainRect.Location.X + mainRect.Width / 2) - (G.MeasureString(TabPages(i).Text, Font).Width / 2)
            Dim titleY As Integer = (mainRect.Location.Y + mainRect.Height / 2) - (G.MeasureString(TabPages(i).Text, Font).Height / 2)
            G.DrawString(TabPages(i).Text, Font, New SolidBrush(FontColor), New Point(titleX, titleY))

            Try : TabPages(i).BackColor = Color.White : Catch : End Try

        Next

    End Sub

End Class

Class BonfireGroupBox
    Inherits ContainerControl

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        BackColor = Color.FromArgb(250, 250, 250)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim G As Graphics = e.Graphics

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim mainRect As New Rectangle(0, 0, Width - 1, Height - 1)
        G.FillRectangle(New SolidBrush(Color.FromArgb(250, 250, 250)), mainRect)
        G.DrawRectangle(New Pen(Color.FromArgb(225, 225, 225)), mainRect)

    End Sub

End Class

Class BonfireLabelHeader
    Inherits Label

    Sub New()
        Font = New Font("Verdana", 10, FontStyle.Bold)
    End Sub

End Class

Class BonfireLabel
    Inherits Label

    Sub New()
        Font = New Font("Verdana", 8)
        ForeColor = Color.FromArgb(135, 135, 135)
    End Sub

End Class

Class BonfireProgressBar
    Inherits Control

    Private _Maximum As Integer = 100
    Public Property Maximum As Integer
        Get
            Return _Maximum
        End Get
        Set(ByVal v As Integer)
            If v < 1 Then v = 1
            If v < _Value Then _Value = v
            _Maximum = v
            Invalidate()
        End Set
    End Property

    Private _Value As Integer
    Public Property Value As Integer
        Get
            Return _Value
        End Get
        Set(ByVal v As Integer)
            If v > _Maximum Then v = Maximum
            _Value = v
            Invalidate()
        End Set
    End Property

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        Size = New Size(100, 40)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim G As Graphics = e.Graphics

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim mainRect As New Rectangle(0, 0, Width - 1, Height - 1)
        G.FillRectangle(New SolidBrush(Color.FromArgb(240, 240, 240)), mainRect)
        G.DrawLine(New Pen(Color.FromArgb(230, 230, 230)), New Point(mainRect.X, mainRect.Height), New Point(mainRect.Width, mainRect.Height))

        Dim barRect As New Rectangle(0, 0, CInt(((Width / _Maximum) * _Value) - 1), Height - 1)
        G.FillRectangle(New SolidBrush(Color.FromArgb(20, 160, 230)), barRect)
        If _Value > 1 Then G.DrawLine(New Pen(Color.FromArgb(20, 140, 200)), New Point(barRect.X, barRect.Height), New Point(barRect.Width, barRect.Height))

        Dim textX As Integer = 12
        Dim textY As Integer = ((Me.Height - 1) / 2) - (G.MeasureString(Text, Font).Height / 2)
        Dim percent As Single = (_Value / _Maximum) * 100
        Dim txt As String = Text & " " & CStr(percent) & "%"
        G.DrawString(txt, Font, Brushes.White, New Point(textX, textY))

    End Sub

End Class

Class BonfireAlertBox
    Inherits Control

    Private exitLocation As Point
    Private overExit As Boolean

    Enum Style
        _Error
        _Success
        _Warning
        _Notice
    End Enum

    Private _alertStyle As Style
    Public Property AlertStyle As Style
        Get
            Return _alertStyle
        End Get
        Set(ByVal value As Style)
            _alertStyle = value
            Invalidate()
        End Set
    End Property

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        Font = New Font("Verdana", 8)
        Size = New Size(200, 40)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim G As Graphics = e.Graphics

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim borderColor, innerColor, textColor As Color
        Select Case _alertStyle
            Case Style._Error
                borderColor = Color.FromArgb(250, 195, 195)
                innerColor = Color.FromArgb(255, 235, 235)
                textColor = Color.FromArgb(220, 90, 90)
            Case Style._Notice
                borderColor = Color.FromArgb(180, 215, 230)
                innerColor = Color.FromArgb(235, 245, 255)
                textColor = Color.FromArgb(80, 145, 180)
            Case Style._Success
                borderColor = Color.FromArgb(180, 220, 130)
                innerColor = Color.FromArgb(235, 245, 225)
                textColor = Color.FromArgb(95, 145, 45)
            Case Style._Warning
                borderColor = Color.FromArgb(220, 215, 140)
                innerColor = Color.FromArgb(250, 250, 220)
                textColor = Color.FromArgb(145, 135, 110)
        End Select

        Dim mainRect As New Rectangle(0, 0, Width - 1, Height - 1)
        G.FillRectangle(New SolidBrush(innerColor), mainRect)
        G.DrawRectangle(New Pen(borderColor), mainRect)

        Dim styleText As String = Nothing
        Select Case _alertStyle
            Case Style._Error
                styleText = "Error!"
            Case Style._Notice
                styleText = "Notice!"
            Case Style._Success
                styleText = "Success!"
            Case Style._Warning
                styleText = "Warning!"
        End Select

        Dim styleFont As New Font(Font.FontFamily, Font.Size, FontStyle.Bold)
        Dim textY As Integer = ((Me.Height - 1) / 2) - (G.MeasureString(Text, Font).Height / 2)
        G.DrawString(styleText, styleFont, New SolidBrush(textColor), New Point(10, textY))
        G.DrawString(Text, Font, New SolidBrush(textColor), New Point(10 + G.MeasureString(styleText, styleFont).Width + 4, textY))

        Dim exitFont As New Font("Marlett", 6)
        Dim exitY As Integer = ((Me.Height - 1) / 2) - (G.MeasureString("r", exitFont).Height / 2) + 1
        exitLocation = New Point(Width - 26, exitY - 3)
        G.DrawString("r", exitFont, New SolidBrush(textColor), New Point(Width - 23, exitY))

    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)

        If e.X >= Width - 26 AndAlso e.X <= Width - 12 AndAlso e.Y > exitLocation.Y AndAlso e.Y < exitLocation.Y + 12 Then
            If Cursor <> Cursors.Hand Then Cursor = Cursors.Hand
            overExit = True
        Else
            If Cursor <> Cursors.Arrow Then Cursor = Cursors.Arrow
            overExit = False
        End If

        Invalidate()

    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)

        If overExit Then Me.Visible = False

    End Sub

End Class

Class BonfireCombo
    Inherits ComboBox

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw Or _
                 ControlStyles.SupportsTransparentBackColor, True)
        Font = New Font("Verdana", 8)
    End Sub

    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()

        DrawMode = Windows.Forms.DrawMode.OwnerDrawFixed
        DropDownStyle = ComboBoxStyle.DropDownList
        DoubleBuffered = True
        ItemHeight = 20

    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim G As Graphics = e.Graphics

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim mainRect As New Rectangle(0, 0, Width - 1, Height - 1)
        G.FillRectangle(Brushes.White, mainRect)
        G.DrawRectangle(New Pen(Color.FromArgb(225, 225, 225)), mainRect)

        Dim triangle As Point() = New Point() {New Point(Width - 14, 16), New Point(Width - 18, 10), New Point(Width - 10, 10)}
        G.FillPolygon(Brushes.DarkGray, triangle)
        G.DrawLine(New Pen(Color.FromArgb(225, 225, 225)), New Point(Width - 27, 0), New Point(Width - 27, Height - 1))

        Try
            If Items.Count > 0 Then
                If Not SelectedIndex = -1 Then
                    Dim textX As Integer = 6
                    Dim textY As Integer = ((Me.Height - 1) / 2) - (G.MeasureString(Items(SelectedIndex), Font).Height / 2) + 1
                    G.DrawString(Items(SelectedIndex), Font, Brushes.Gray, New Point(textX, textY))
                Else
                    Dim textX As Integer = 6
                    Dim textY As Integer = ((Me.Height - 1) / 2) - (G.MeasureString(Items(0), Font).Height / 2) + 1
                    G.DrawString(Items(0), Font, Brushes.Gray, New Point(textX, textY))
                End If
            End If
        Catch : End Try

    End Sub

    Sub replaceItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles Me.DrawItem
        e.DrawBackground()

        Dim G As Graphics = e.Graphics
        G.SmoothingMode = SmoothingMode.HighQuality

        Dim rect As New Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, e.Bounds.Width + 1, e.Bounds.Height + 1)

        Try
            If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
                G.FillRectangle(New SolidBrush(Color.FromArgb(20, 160, 230)), rect)
                G.DrawString(MyBase.GetItemText(MyBase.Items(e.Index)), Font, Brushes.White, New Rectangle(e.Bounds.X + 6, e.Bounds.Y + 3, e.Bounds.Width, e.Bounds.Height))
                G.DrawRectangle(New Pen(Color.FromArgb(20, 160, 230)), rect)
            Else
                G.FillRectangle(Brushes.White, rect)
                G.DrawString(MyBase.GetItemText(MyBase.Items(e.Index)), Font, Brushes.DarkGray, New Rectangle(e.Bounds.X + 6, e.Bounds.Y + 3, e.Bounds.Width, e.Bounds.Height))
                G.DrawRectangle(New Pen(Color.FromArgb(20, 160, 230)), rect)
            End If

        Catch : End Try

    End Sub

    Protected Overrides Sub OnSelectedItemChanged(ByVal e As System.EventArgs)
        MyBase.OnSelectedItemChanged(e)
        Invalidate()
    End Sub

End Class

<DefaultEvent("CheckedChanged")> Class BonfireCheckbox
    Inherits Control

    Event CheckedChanged(ByVal sender As Object)

    Private _checked As Boolean
    Public Property Checked() As Boolean
        Get
            Return _checked
        End Get
        Set(ByVal value As Boolean)
            _checked = value
            Invalidate()
        End Set
    End Property

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        Size = New Size(140, 20)
        Font = New Font("Verdana", 8)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim G As Graphics = e.Graphics

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim box As New Rectangle(0, 0, Height, Height - 1)
        G.FillRectangle(Brushes.White, box)
        G.DrawRectangle(New Pen(Color.FromArgb(225, 225, 225)), box)

        Dim markPen As New Pen(Color.FromArgb(150, 155, 160))
        Dim lightMarkPen As New Pen(Color.FromArgb(170, 175, 180))
        If _checked Then G.DrawString("a", New Font("Marlett", 14), Brushes.Gray, New Point(0, 0))

        Dim textY As Integer = (Height / 2) - (G.MeasureString(Text, Font).Height / 2)
        G.DrawString(Text, Font, Brushes.Gray, New Point(24, textY))

    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)

        If _checked Then
            _checked = False
        Else
            _checked = True
        End If

        RaiseEvent CheckedChanged(Me)
        Invalidate()

    End Sub

End Class

<DefaultEvent("CheckedChanged")> Class BonfireRadioButton
    Inherits Control

    Event CheckedChanged(ByVal sender As Object)

    Private _checked As Boolean
    Public Property Checked() As Boolean
        Get
            Return _checked
        End Get
        Set(ByVal value As Boolean)
            _checked = value
            Invalidate()
        End Set
    End Property

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or _
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        Size = New Size(140, 20)
        Font = New Font("Verdana", 8)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)

        Dim G As Graphics = e.Graphics

        G.SmoothingMode = SmoothingMode.HighQuality
        G.Clear(Parent.BackColor)

        Dim box As New Rectangle(0, 0, Height - 1, Height - 1)
        G.FillEllipse(Brushes.White, box)
        G.DrawEllipse(New Pen(Color.FromArgb(225, 225, 225)), box)

        If _checked Then
            Dim innerMark As New Rectangle(6, 6, Height - 13, Height - 13)
            G.FillEllipse(New SolidBrush(Color.FromArgb(20, 160, 230)), innerMark)
        End If

        Dim textY As Integer = (Height / 2) - (G.MeasureString(Text, Font).Height / 2)
        G.DrawString(Text, Font, Brushes.Gray, New Point(24, textY))

    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)

        For Each C As Control In Parent.Controls
            If C IsNot Me AndAlso TypeOf C Is BonfireRadioButton Then
                DirectCast(C, BonfireRadioButton).Checked = False
            End If
        Next

        If _checked Then
            _checked = False
        Else
            _checked = True
        End If

        RaiseEvent CheckedChanged(Me)
        Invalidate()

    End Sub

End Class