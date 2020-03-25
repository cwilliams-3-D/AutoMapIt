Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.IO
Imports Microsoft.Win32



Public Class Form1
    Inherits MetroFramework.Forms.MetroForm

    Public objectcount As Integer
    Public Markercount As Integer
    Public TileCountRow As Integer
    Public TileCount As Integer
    Public TileCountColumn As Integer
    Public TileStartY As Integer
    Public TileStartX As Integer
    Public CurrentIcon As String


    Public Sub FixBrowserVersion()
        Try
            Dim BrowserVer As Integer, RegVal As Integer

            ' get the installed IE version
            Using Wb As New WebBrowser()
                BrowserVer = Wb.Version.Major
            End Using

            ' set the appropriate IE version
            If BrowserVer >= 11 Then
                RegVal = 11001
            ElseIf BrowserVer = 10 Then
                RegVal = 10001
            ElseIf BrowserVer = 9 Then
                RegVal = 9999
            ElseIf BrowserVer = 8 Then
                RegVal = 8888
            Else
                RegVal = 7000
            End If

            ' set the actual key
            Using Key As RegistryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadWriteSubTree)
                If Key.GetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe") Is Nothing Then
                    Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", RegVal, RegistryValueKind.DWord)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FixBrowserVersion()
        MetroTabControl1.SelectTab(0)
        objectcount = 1
        Markercount = 1
        TileCount = 1
        TileCountRow = 1
        TileCountColumn = 1
        TileStartY = 1
        TileStartX = 6
        ComboBox1.SelectedIndex = 0
        GetFilesFromDir(My.Application.Info.DirectoryPath & "\images\")
        'LoadDataGridWithImages()
        ' WebBrowser1.ScriptErrorsSuppressed = True
        'WebBrowser1.Navigate(New Uri("http://www.google.com"))
    End Sub



    Public Sub OpenImage()
        'Set the Filter.
        OpenFileDialog1.Filter = " PNG |*.png| Bitmap |*.bmp| JPG | *.jpg | GIF | *.gif | All Files|*.*"

        'Clear the file name
        OpenFileDialog1.FileName = ""

        'Show it
        If OpenFileDialog1.ShowDialog(Me) = DialogResult.OK Then
            'Get the image name
            Dim img As String = OpenFileDialog1.FileName

            'Create a new Bitmap and resize it
            Panel1.BackgroundImage = ResizeImage(System.Drawing.Bitmap.FromFile(img), 1280, 720)
            Markercount = 1
            objectcount = 1
            Panel1.Controls.Clear()
            ListView1.Items.Clear()
            'PictureBox1.BackColor = Color.Transparent
            'SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        End If
    End Sub

    Private Sub Panel1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseDown
        'txtValuex.Text = e.X.ToString
        'txtValuey.Text = e.Y.ToString

        If CurrentIcon = "Marker.png" Then
            AddImage(e.X, e.Y, "New " & Markercount, "")
        Else
            Dim mapid As String = InputBox("Please type the appropriate MapID location:", "Set MapID", DefaultResponse:="")
            If mapid = "" Then Exit Sub
            AddCombinedImage(e.X, e.Y, "", "", mapid, False)
        End If

    End Sub
    Private Sub AddImage(x As Integer, y As Integer, mapid As String, loadimage As String)

        Dim pb As New PictureBox

        If loadimage = "" Then

            pb.Name = "Marker (" & mapid & ")"
            pb.Width = 20
            pb.Height = 20

            If CurrentIcon = "Marker.png" Then
                pb.Top = y - 10
                pb.Left = x - 10
            Else
                pb.Top = y
                pb.Left = x + 45
            End If

            'pb.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory & "Images\printer.png")
            pb.Image = Image.FromFile(My.Application.Info.DirectoryPath & "\images\Markers\" & ComboBox1.SelectedItem.ToString & ".png")
            pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Parent = Panel1
            pb.BackColor = Color.Transparent
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Panel1.Controls.Add(pb)
            AddHandler pb.MouseDown, AddressOf Pic_MouseDown
            AddHandler pb.MouseMove, AddressOf Pic_MouseMove
            AddHandler pb.MouseUp, AddressOf Pic_MouseUp
            pb.BringToFront()
            pb.Tag = "Marker (" & mapid & ")" & "*" & ComboBox1.SelectedItem.ToString & ".png" & "*" & pb.Location.X & "*" & pb.Location.Y
            ListView1.Items.Add("Marker (" & mapid & ")", mapid)

            Markercount = Markercount + 1

        Else
            pb.Name = mapid
            pb.Width = 20
            pb.Height = 20
            pb.Top = y
            pb.Left = x
            pb.Image = Image.FromFile(My.Application.Info.DirectoryPath & "\images\Markers\" & loadimage)
            pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Parent = Panel1
            pb.BackColor = Color.Transparent
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Panel1.Controls.Add(pb)
            AddHandler pb.MouseDown, AddressOf Pic_MouseDown
            AddHandler pb.MouseMove, AddressOf Pic_MouseMove
            AddHandler pb.MouseUp, AddressOf Pic_MouseUp
            pb.BringToFront()
            pb.Tag = mapid & "*" & loadimage & "*" & pb.Location.X & "*" & pb.Location.Y
            ListView1.Items.Add(mapid)

            Markercount = Markercount + 1

        End If

    End Sub

    Private Sub AddCombinedImage(x As Integer, y As Integer, image1 As String, image2 As String, mapid As String, loadimage As Boolean)

        If Not loadimage = True Then
            If Not CurrentIcon = Nothing Then

                'Create image object from existing image

                Dim pb As New PictureBox
                Dim w As Integer
                Dim h As Integer

                w = 55
                h = 70

                pb.Width = w
                pb.Height = h
                pb.Top = y - 32
                pb.Left = x - 32
                Dim img As Image = New Bitmap(w, h)
                Dim g As Graphics = Graphics.FromImage(img)
                Dim bg = Color.FromArgb(0, 0, 0)
                'set backrgound
                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                g.CompositingQuality = CompositingQuality.HighQuality
                g.TextRenderingHint = TextRenderingHint.AntiAlias
                g.SmoothingMode = SmoothingMode.HighQuality
                Using br As New SolidBrush(bg)
                    g.FillRectangle(br, 0, 30, w, h)
                End Using
                g.DrawImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\BG\" & ComboBox1.SelectedItem.ToString & ".png"), New Point(0, 0))

                'set foreground

                g.DrawImage(ResizeImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\" & CurrentIcon), 40, 40), New Point(7, 7))

                'g.DrawImage(Image.FromFile("b.jpg"), New Point(70, 10))
                Using fnt As New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel)
                    TextRenderer.DrawText(g, mapid, fnt, New Rectangle(0, 0, 55, 68),
                      Color.WhiteSmoke,
                      TextFormatFlags.HorizontalCenter Or TextFormatFlags.Bottom)
                End Using

                pb.Image = img
                ' pb.SizeMode = PictureBoxSizeMode.StretchImage
                pb.Parent = Panel1
                pb.Tag = ComboBox1.SelectedItem.ToString & ".png" & "*" & CurrentIcon & "*" & mapid & "*" & pb.Location.X & "*" & pb.Location.Y
                pb.BackColor = Color.Transparent
                SetStyle(ControlStyles.SupportsTransparentBackColor, True)
                pb.Name = "Icon (" & mapid & ")"
                Panel1.Controls.Add(pb)
                'AddHandler pb.Click, AddressOf Pic_Click
                AddHandler pb.MouseDown, AddressOf Pic_MouseDown
                AddHandler pb.MouseMove, AddressOf Pic_MouseMove
                AddHandler pb.MouseUp, AddressOf Pic_MouseUp
                pb.BringToFront()
                ListView1.Items.Add("Icon (" & mapid & ")", objectcount)
                objectcount = objectcount + 1

                g.Dispose()

                AddImage(x, y, mapid, "")

            End If
        Else

            'load image from ini
            Dim pb As New PictureBox
            Dim w As Integer
            Dim h As Integer

            w = 55
            h = 70

            pb.Width = w
            pb.Height = h
            pb.Top = y
            pb.Left = x
            Dim img As Image = New Bitmap(w, h)
            Dim g As Graphics = Graphics.FromImage(img)
            Dim bg = Color.FromArgb(0, 0, 0)
            'set backrgound
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.CompositingQuality = CompositingQuality.HighQuality
            g.TextRenderingHint = TextRenderingHint.AntiAlias
            g.SmoothingMode = SmoothingMode.HighQuality
            Using br As New SolidBrush(bg)
                g.FillRectangle(br, 0, 30, w, h)
            End Using
            g.DrawImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\BG\" & image1), New Point(0, 0))

            'set foreground

            g.DrawImage(ResizeImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\" & image2), 40, 40), New Point(7, 7))

            'g.DrawImage(Image.FromFile("b.jpg"), New Point(70, 10))
            Using fnt As New Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel)
                TextRenderer.DrawText(g, mapid, fnt, New Rectangle(0, 0, 55, 68),
                  Color.WhiteSmoke,
                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.Bottom)
            End Using

            pb.Image = img
            ' pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Parent = Panel1
            pb.Tag = image1 & "*" & image2 & "*" & mapid & "*" & pb.Location.X & "*" & pb.Location.Y
            pb.BackColor = Color.Transparent
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            pb.Name = "Icon (" & mapid & ")"
            Panel1.Controls.Add(pb)
            'AddHandler pb.Click, AddressOf Pic_Click
            AddHandler pb.MouseDown, AddressOf Pic_MouseDown
            AddHandler pb.MouseMove, AddressOf Pic_MouseMove
            AddHandler pb.MouseUp, AddressOf Pic_MouseUp
            pb.BringToFront()
            ListView1.Items.Add("Icon (" & mapid & ")", objectcount)
            objectcount = objectcount + 1

            g.Dispose()

        End If

    End Sub

    'Private Sub Pic_Click(sender As Object, e As EventArgs)
    'Dim thisPic As PictureBox = DirectCast(sender, PictureBox)
    'MsgBox(thisPic.Name)
    'thisPIC now is a reference to the box, you can use .Name, etc. to get it's properties.
    'End Sub

    Private _isMoved As Boolean
    Private _x As Integer
    Private _y As Integer

    Private Sub Pic_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim thisPic As PictureBox = DirectCast(sender, PictureBox)
        _isMoved = True
        _x = e.Location.X
        _y = e.Location.Y
    End Sub

    Private Sub Pic_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim thisPic As PictureBox = DirectCast(sender, PictureBox)
        If _isMoved Then
            thisPic.Location = New Point(thisPic.Location.X + (e.Location.X - _x), thisPic.Location.Y + (e.Location.Y - _y))
            If thisPic.Name.Contains("Marker") Then
                Dim words As String() = thisPic.Tag.Split(New Char() {"*"c})
                Dim part1 As String = words(0)
                Dim part2 As String = words(1)
                thisPic.Tag = part1 & "*" & part2 & "*" & thisPic.Location.X & "*" & thisPic.Location.Y

            Else
                If thisPic.Tag.contains("Element") Then
                    Dim words As String() = thisPic.Tag.Split(New Char() {"*"c})
                    Dim part1 As String = words(1)
                    thisPic.Tag = "Element" & "*" & part1 & "*" & thisPic.Location.X & "*" & thisPic.Location.Y
                Else
                    Dim words As String() = thisPic.Tag.Split(New Char() {"*"c})
                    Dim part1 As String = words(0)
                    Dim part2 As String = words(1)
                    Dim part3 As String = words(2)
                    thisPic.Tag = part1 & "*" & part2 & "*" & part3 & "*" & thisPic.Location.X & "*" & thisPic.Location.Y
                End If
            End If
        End If
    End Sub

    Private Sub Pic_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim thisPic As PictureBox = DirectCast(sender, PictureBox)
        _isMoved = False
    End Sub

    Public Shared Function ResizeImage(ByVal InputImage As Image, w As Integer, h As Integer) As Image
        Return New Bitmap(InputImage, New Size(w, h))
    End Function
    Private Sub ExportImage()

        Dim saveFileDialog1 As New SaveFileDialog
        saveFileDialog1.InitialDirectory = "C:\"
        saveFileDialog1.Title = "Save image Files"
        saveFileDialog1.CheckFileExists = False
        saveFileDialog1.CheckPathExists = True
        saveFileDialog1.DefaultExt = "png"
        saveFileDialog1.Filter = "PNG (*.png)|*.png|All files (*.*)|*.*"
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True

        If (saveFileDialog1.ShowDialog() = DialogResult.OK) Then

            'TakeScreenShot(Panel1).Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png)
            Dim bmp As New Bitmap(1280, 720)
            Panel1.DrawToBitmap(bmp, Panel1.ClientRectangle)
            bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png)

        End If

    End Sub

    Public Sub GetFilesFromDir(path As String)
        ' Make a reference to a directory.
        Dim di As New DirectoryInfo(path)
        ' Get a reference to each file in that directory.
        Dim fiArr As FileInfo() = di.GetFiles()
        ' Display the names of the files.
        Dim file As FileInfo
        Dim fileName As String = ""
        Dim img1 As Image
        Dim NewRow As Boolean
        For Each file In fiArr
            If file.Extension = ".png" Then
                'Console.WriteLine("------------- {0} -----------", file.Name)
                fileName = System.IO.Path.GetFileName(path & "\" & file.Name)
                img1 = Image.FromFile(path & file.Name)
                If TileCountColumn = 1 Then
                    TileStartX = 6
                    NewRow = False
                    TileCountColumn = 2
                Else
                    TileStartX = 78
                    NewRow = True
                    TileCountColumn = 1
                End If

                addMetroTile(img1, fileName)
                If NewRow = True Then
                    TileStartY = TileStartY + 72
                End If
                'DataGridView1.Rows.Add(img1, fileName)
            End If
        Next
    End Sub

    Public Sub addMetroTile(img2 As Image, filename As String)

        Dim mt As New MetroFramework.Controls.MetroTile
        mt.Name = "Tile " & TileCount
        mt.Width = 70
        mt.Height = 70

        mt.Top = TileStartY
        mt.Left = TileStartX

        mt.UseTileImage = True
        mt.UseCustomBackColor = True
        mt.BackColor = Color.GhostWhite
        mt.Theme = MetroFramework.MetroThemeStyle.Dark
        'mt.Style = MetroFramework.MetroColorStyle.Green
        mt.TileTextFontSize = MetroFramework.MetroTileTextSize.Small
        mt.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold

        Dim img As Image = New Bitmap(70, 70)
        Dim g As Graphics = Graphics.FromImage(img)
        g.DrawImage(ResizeImage(img2, 60, 60), New Point(5, 5))
        mt.TileImage = img
        mt.Tag = filename
        mt.TextAlign = ContentAlignment.BottomLeft
        mt.Parent = MetroPanel1
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        MetroPanel1.Controls.Add(mt)
        AddHandler mt.Click, AddressOf Tile_Click
        'AddHandler mt.MouseMove, AddressOf Pic_MouseMove
        'AddHandler mt.MouseUp, AddressOf Pic_MouseUp
        mt.BringToFront()
        'ListView1.Items.Add("Marker " & Markercount, Markercount)
        'Markercount = Markercount + 1

        TileCount = TileCount + 0.5

    End Sub
    Private Sub Tile_Click(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim thisTile As MetroFramework.Controls.MetroTile = DirectCast(sender, MetroFramework.Controls.MetroTile)
        CurrentIcon = thisTile.Tag
        MetroLabel4.Text = CurrentIcon
        SetCurrentImage()

    End Sub

    Public Sub SetCurrentImage()

        If Not CurrentIcon = Nothing Then
            Dim w As Integer
            Dim h As Integer
            w = 100
            h = 100
            Dim img As Image = New Bitmap(w, h)
            Dim g As Graphics = Graphics.FromImage(img)
            Dim bg = Color.FromArgb(0, 0, 0)
            'set backrgound
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.CompositingQuality = CompositingQuality.HighQuality
            g.TextRenderingHint = TextRenderingHint.AntiAlias
            g.SmoothingMode = SmoothingMode.HighQuality

            If Not CurrentIcon = "Marker.png" Then
                Using br As New SolidBrush(bg)
                    g.FillRectangle(br, 0, 47, w, h)
                End Using
                g.DrawImage(ResizeImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\BG\" & ComboBox1.SelectedItem.ToString & ".png"), w, h), New Point(0, 0))
                g.DrawImage(ResizeImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\" & CurrentIcon), 70, 70), New Point(15, 15))
            Else
                g.DrawImage(ResizeImage(Image.FromFile(My.Application.Info.DirectoryPath & "\images\BG\" & ComboBox1.SelectedItem.ToString & ".png"), w, h), New Point(0, 0))
            End If
            'set foreground


            CurrentImage.Image = img
            ' pb.SizeMode = PictureBoxSizeMode.StretchImage
        Else
        End If
    End Sub

    'Private Sub LoadDataGridWithImages()
    '    '  Create Datagridview image column
    '    Dim dgvImageColumn As New DataGridViewImageColumn
    '    Dim dgvTextColumn As New DataGridViewTextBoxColumn
    '    ' set header text to the colun
    '    dgvImageColumn.HeaderText = "Image"
    '    ' make the image layout stretch to display the entire image
    '    dgvImageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom
    '    ' add the column to the datagridview
    '    DataGridView1.Columns.Add(dgvImageColumn)
    '    DataGridView1.Columns.Add(dgvTextColumn)
    '    ' make the columns take all the datagridview width
    '    DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
    '    dgvImageColumn.Width = 120

    '    ' change the datagridview height
    '    DataGridView1.RowTemplate.Height = 141
    '    'DataGridView1.BackgroundColor = System.Drawing.Color.Black

    '    DataGridView1.AllowUserToAddRows = False

    '    ' create images and add the to the datagridview
    '    GetFilesFromDir(My.Application.Info.DirectoryPath & "\images\")

    'End Sub

    Private HighlightedPictureBox As PictureBox = Nothing
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count > 0 Then
            button4.Enabled = True
            Dim i As Integer = Panel1.Controls.IndexOfKey(ListView1.SelectedItems(0).Text)
            'Panel1.Controls(i).

            'Get the rectangle of the control and inflate it to represent the border area  
            Dim BorderBounds As Rectangle = DirectCast(Panel1.Controls.Item(i), PictureBox).ClientRectangle
            BorderBounds.Inflate(0, 0)

            'Use ControlPaint to draw the border.  
            'Change the Color.Red parameter to your own colour below.
            Dim borderwidth As Integer = 3
            Dim bordercolor As Color = Color.Red
            Dim borderstyle As ButtonBorderStyle = ButtonBorderStyle.Solid
            ControlPaint.DrawBorder(DirectCast(Panel1.Controls.Item(i), PictureBox).CreateGraphics,
                                    BorderBounds,
                                    bordercolor,
                                    borderwidth,
                                    borderstyle,
                                    bordercolor,
                                    borderwidth,
                                    borderstyle,
                                    bordercolor,
                                    borderwidth,
                                    borderstyle,
                                    bordercolor,
                                    borderwidth,
                                    borderstyle)

            ' Dim p As Point = PointToScreen(Panel1.Controls.Item(i).Location)

            ' Dim ctrlrect As Rectangle = New Rectangle(p.X - 8, p.Y - 8, Panel1.Controls.Item(i).Width + 16, Panel1.Controls.Item(i).Height + 16)
            'Dim newpoint As Point = New Point(Panel1.Location.X, Me.wbQuestion.Location.Y + Me.wbQuestion.Height + 10)

            'ControlPaint.FillReversibleRectangle(ctrlrect, Color.Red)


            If Not (HighlightedPictureBox Is Nothing) Then
                'Remove the border of the last PictureBox  
                HighlightedPictureBox.Invalidate()
            End If

            'Rememeber the last highlighted PictureBox  
            HighlightedPictureBox = CType(Panel1.Controls.Item(i), PictureBox)
        Else
            button4.Enabled = False
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        SetCurrentImage()
    End Sub

    Private Function BitmapToString(ByVal bImage As Bitmap) As String
        ' From an article here: http://www.planet-source-code.com/vb/scripts/ShowCode.asp?txtCodeId=2116&lngWId=10

        Try
            Dim data As String
            Dim ms As MemoryStream = New MemoryStream
            bImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            data = Convert.ToBase64String(ms.ToArray())
            Return data

        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Private Function StringToBitmap(ByVal sImageData As String) As Bitmap
        ' From an article here: http://www.planet-source-code.com/vb/scripts/ShowCode.asp?txtCodeId=2116&lngWId=10

        Try
            Dim ms As New MemoryStream(Convert.FromBase64String(sImageData))
            ' Dim bmp As Bitmap = Bitmap.FromStream(ms)
            ' Note that I've modified his original code above into this:
            Dim bmp As New Bitmap(ms)
            Return bmp

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Sections As System.Collections.ICollection
    Private m_sections As Hashtable
    Public Sub LoadConfig()
        Dim ini As New IniFile

        'Set the Filter.
        OpenFileDialog1.Filter = " INI |*.ini"

        'Clear the file name
        OpenFileDialog1.FileName = ""

        'Show it
        If OpenFileDialog1.ShowDialog(Me) = DialogResult.OK Then

            ini.Load(OpenFileDialog1.FileName)
            Markercount = 1
            objectcount = 1
            Panel1.Controls.Clear()
            ListView1.Items.Clear()

            Dim Section As IniFile.IniSection
            Dim s As IniFile.IniSection
            Dim icon As String
            Dim type As String
            Dim x As Integer
            Dim y As Integer
            Dim bg As String
            icon = ""
            bg = ""
            type = ""

            For Each Section In ini.Sections
                Select Case True
                    Case Section.GetName.ToString.Contains("Marker")
                        'add a marker
                        s = ini.GetSection(Section.GetName.ToString)
                        For Each k In s.Keys
                            Select Case k.Name
                                Case "Icon"
                                    icon = k.Value
                                Case "x"
                                    x = k.Value
                                Case "y"
                                    y = k.Value
                            End Select
                        Next
                        AddImage(x, y, Section.GetName.ToString, icon)

                    Case Section.GetName.ToString.Contains("Element")
                        'add a marker
                        s = ini.GetSection(Section.GetName.ToString)
                        For Each k In s.Keys
                            Select Case k.Name
                                Case "Type"
                                    type = k.Value
                                Case "x"
                                    x = k.Value
                                Case "y"
                                    y = k.Value
                            End Select
                        Next
                        AddElement(True, type, x, y)

                    Case Section.GetName.ToString.Contains("ImageCode")
                        'add background
                        Dim imagecode As String = ini.GetKeyValue("ImageCode", "Code")
                        Panel1.BackgroundImage = StringToBitmap(imagecode)

                    Case IsNumeric(Section.GetName.ToString)
                        'add an icon
                        s = ini.GetSection(Section.GetName.ToString)
                        For Each k In s.Keys
                            Select Case k.Name
                                Case "Icon"
                                    icon = k.Value
                                Case "x"
                                    x = k.Value
                                Case "y"
                                    y = k.Value
                                Case "Background"
                                    bg = k.value
                            End Select
                        Next
                        AddCombinedImage(x, y, bg, icon, Section.GetName.ToString, True)

                End Select
            Next

        End If

    End Sub

    Public Sub SaveConfig()

        Dim pb As PictureBox
        Dim ini As New IniFile

        ' choose a file

        Dim saveFileDialog2 As New SaveFileDialog
        saveFileDialog2.InitialDirectory = "C:\"
        saveFileDialog2.CheckFileExists = False
        saveFileDialog2.CheckPathExists = True
        saveFileDialog2.DefaultExt = "ini"
        saveFileDialog2.Filter = "INI File|*.ini"
        saveFileDialog2.Title = "Save a config File"
        saveFileDialog2.FilterIndex = 1
        saveFileDialog2.RestoreDirectory = True

        If (saveFileDialog2.ShowDialog() = DialogResult.OK) Then
            Dim filename_with_ext As String = Path.GetFileName(saveFileDialog2.FileName)
            If System.IO.File.Exists(saveFileDialog2.FileName) Then ini.RemoveAllSections()
            Dim sTempFileName As String = System.IO.Path.GetTempFileName()
            Dim fsTemp As New System.IO.FileStream(Path.ChangeExtension(sTempFileName, ".ini"), IO.FileMode.Create)
            Dim result As String = fsTemp.Name
            fsTemp.Close()
            fsTemp.Dispose()
            ini.Load(result)

            For Each pb In Panel1.Controls
                ' Part 1: we want to split this input string.
                Dim s As String = pb.Tag

                If pb.Tag.contains("Marker") Then
                    Dim words As String() = s.Split(New Char() {"*"c})
                    ini.AddSection(words(0)).AddKey("Icon").Value = words(1)
                    ini.AddSection(words(0)).AddKey("x").Value = words(2)
                    ini.AddSection(words(0)).AddKey("y").Value = words(3)
                Else
                    If pb.Tag.contains("Element") Then
                        'add elements
                        Dim words As String() = s.Split(New Char() {"*"c})
                        ini.AddSection("Element: " & words(1)).AddKey("Type").Value = words(1)
                        ini.AddSection("Element: " & words(1)).AddKey("x").Value = words(2)
                        ini.AddSection("Element: " & words(1)).AddKey("y").Value = words(3)
                    Else
                        Dim words As String() = s.Split(New Char() {"*"c})
                        ini.AddSection(words(2)).AddKey("Background").Value = words(0)
                        ini.AddSection(words(2)).AddKey("Icon").Value = words(1)
                        ini.AddSection(words(2)).AddKey("x").Value = words(3)
                        ini.AddSection(words(2)).AddKey("y").Value = words(4)
                    End If
                End If
            Next

            'Export Map

            Dim imagecode As String = BitmapToString(Panel1.BackgroundImage)
            ini.AddSection("ImageCode").AddKey("Code").Value = imagecode

            ini.Save(result)
            FileSystem.FileCopy(result, saveFileDialog2.FileName)
            My.Computer.FileSystem.DeleteFile(result)
        End If

    End Sub


    Private Sub MetroButton5_Click(sender As Object, e As EventArgs) Handles MetroButton5.Click
        MetroContextMenu1.Show(MetroButton5, -2, 23)
    End Sub

    Private Sub SaveConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveConfigToolStripMenuItem.Click
        SaveConfig()
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        LoadConfig()
    End Sub

    Private Sub MetroButton1_Click_1(sender As Object, e As EventArgs)
        Dim surface As Graphics = CreateGraphics()
        Dim pen1 As Pen = New Pen(Color.Black, 3)
        surface.DrawLine(pen1, 10, 10, 100, 10)
    End Sub

    Private Sub AddMapLayoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddMapLayoutToolStripMenuItem.Click
        OpenImage()
    End Sub

    Private Sub ExportMapToImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMapToImageToolStripMenuItem.Click
        ExportImage()
    End Sub

    Private Sub MetroButton2_Click_1(sender As Object, e As EventArgs) Handles MetroButton2.Click
        MetroContextMenu2.Show(MetroButton2, -2, 23)
    End Sub

    Private Sub MetroButton3_Click_2(sender As Object, e As EventArgs) Handles MetroButton3.Click
        MetroContextMenu3.Show(MetroButton3, -2, 23)
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        If ToolStripMenuItem1.Checked = True Then
            AddElement(True, "Legend", 30, 30)
        Else
            AddElement(False, "Legend", 0, 0)
        End If
    End Sub

    Private Sub AddElement(Add As Boolean, Type As String, x As Integer, y As Integer)
        If Add = True Then
            Dim pb As New PictureBox
            Dim w As Integer
            Dim h As Integer

            Select Case True
                Case Type = "Legend"
                    w = 295
                    h = 105
                    ToolStripMenuItem1.Checked = True
                Case Type = "Logo"
                    w = 159
                    h = 103
                    ToolStripMenuItem3.Checked = True
            End Select

            pb.Name = Type
            pb.Width = w
            pb.Height = h
            pb.Top = y
            pb.Left = x
            pb.Image = Image.FromFile(My.Application.Info.DirectoryPath & "\images\Elements\" & Type & ".png")
            pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Parent = Panel1
            pb.BackColor = Color.Transparent
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Panel1.Controls.Add(pb)
            AddHandler pb.MouseDown, AddressOf Pic_MouseDown
            AddHandler pb.MouseMove, AddressOf Pic_MouseMove
            AddHandler pb.MouseUp, AddressOf Pic_MouseUp
            pb.BringToFront()
            pb.Tag = "Element" & "*" & Type & "*" & pb.Location.X & "*" & pb.Location.Y
            ListView1.Items.Add(Type)

        Else
            Panel1.Controls.RemoveByKey(Type)
            ListView1.Items.Remove((From i In ListView1.Items.OfType(Of ListViewItem)()
                                    Where i.Text = Type).First)
        End If

    End Sub

    Private Sub button4_Click(sender As Object, e As EventArgs) Handles button4.Click
        If ListView1.SelectedItems.Count > 0 Then
            If ListView1.SelectedItems(0).Text = "Logo" Then ToolStripMenuItem3.Checked = False
            If ListView1.SelectedItems(0).Text = "Legend" Then ToolStripMenuItem1.Checked = False
            Panel1.Controls.RemoveByKey(ListView1.SelectedItems(0).Text)
            ListView1.Items.RemoveAt(ListView1.FocusedItem.Index)
        End If
    End Sub

    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        If ToolStripMenuItem3.Checked = True Then
            AddElement(True, "Logo", 30, 30)
        Else
            AddElement(False, "Logo", 0, 0)
        End If
    End Sub
End Class