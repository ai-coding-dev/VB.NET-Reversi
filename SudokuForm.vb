Imports System.Drawing
Imports System.Windows.Forms

Public Module Program
    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New SudokuForm())
    End Sub
End Module

Public Class SudokuForm
    Inherits Form

    Private cells(8, 8) As TextBox
    Private board(8, 8) As Integer
    Private random As New Random()

    Public Sub New()

        Me.Text = "Sudoku - 9x9"
        Me.Size = New Size(500, 620)
        Me.BackColor = Color.FromArgb(45, 45, 48) 
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.StartPosition = FormStartPosition.CenterScreen
        
        InitializeUI()
        StartNewGame()
    End Sub

    Private Sub InitializeUI()
        Dim mainContainer As New TableLayoutPanel()
        mainContainer.ColumnCount = 9
        mainContainer.RowCount = 9
        mainContainer.Dock = DockStyle.Top
        mainContainer.Height = 450
        mainContainer.Padding = New Padding(10)
        mainContainer.BackColor = Color.Black 

        For i As Integer = 0 To 8
            mainContainer.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 11.11F))
            mainContainer.RowStyles.Add(New RowStyle(SizeType.Percent, 11.11F))
        Next

        For row As Integer = 0 To 8
            For col As Integer = 0 To 8
                Dim txt As New TextBox()
                txt.Multiline = True 
                txt.Dock = DockStyle.Fill
                txt.TextAlign = HorizontalAlignment.Center
                txt.Font = New Font("Segoe UI", 18, FontStyle.Bold)
                txt.MaxLength = 1
                txt.BorderStyle = BorderStyle.None
                
                Dim marginTop = If(row Mod 3 = 0 And row <> 0, 3, 1)
                Dim marginLeft = If(col Mod 3 = 0 And col <> 0, 3, 1)
                txt.Margin = New Padding(marginLeft, marginTop, 0, 0)

                If (row \ 3 + col \ 3) Mod 2 = 0 Then
                    txt.BackColor = Color.White
                Else
                    txt.BackColor = Color.FromArgb(225, 230, 235) ' 淡蓝色
                End If

                AddHandler txt.KeyPress, AddressOf TextBox_KeyPress
                cells(row, col) = txt
                mainContainer.Controls.Add(txt, col, row)
            Next
        Next

        Me.Controls.Add(mainContainer)

        Dim panel As New FlowLayoutPanel()
        panel.Dock = DockStyle.Bottom
        panel.Height = 80
        panel.FlowDirection = FlowDirection.LeftToRight
        panel.Padding = New Padding(20, 10, 0, 0)

        Dim btnStart As New Button With {
            .Text = "New Game", .Size = New Size(120, 45), 
            .FlatStyle = FlatStyle.Flat, .BackColor = Color.MediumSeaGreen, 
            .ForeColor = Color.White, .Font = New Font("微软雅黑", 10)
        }
        AddHandler btnStart.Click, Sub() StartNewGame()

        Dim btnCheck As New Button With {
            .Text = "Check Solution", .Size = New Size(120, 45), 
            .FlatStyle = FlatStyle.Flat, .BackColor = Color.DodgerBlue, 
            .ForeColor = Color.White, .Font = New Font("微软雅黑", 10),
            .Margin = New Padding(20, 0, 0, 0)
        }
        AddHandler btnCheck.Click, Sub() CheckSolution()

        panel.Controls.Add(btnStart)
        panel.Controls.Add(btnCheck)
        Me.Controls.Add(panel)
    End Sub

    Private Sub TextBox_KeyPress(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsDigit(e.KeyChar) OrElse e.KeyChar = "0"c Then
            If Not Char.IsControl(e.KeyChar) Then e.Handled = True
        End If
    End Sub

    Private Sub StartNewGame()
        Array.Clear(board, 0, board.Length)
        GenerateFullSudoku(0, 0)

        For r As Integer = 0 To 8
            For c As Integer = 0 To 8
                cells(r, c).ForeColor = Color.Black
                If random.Next(100) < 35 Then
                    cells(r, c).Text = board(r, c).ToString()
                    cells(r, c).ReadOnly = True
                    cells(r, c).BackColor = Color.FromArgb(200, 200, 200)
                Else
                    cells(r, c).Text = ""
                    cells(r, c).ReadOnly = False
                    If (r \ 3 + c \ 3) Mod 2 = 0 Then
                        cells(r, c).BackColor = Color.White
                    Else
                        cells(r, c).BackColor = Color.FromArgb(225, 230, 235)
                    End If
                End If
            Next
        Next
    End Sub

    Private Function GenerateFullSudoku(row As Integer, col As Integer) As Boolean
        If col = 9 Then
            col = 0 : row += 1
            If row = 9 Then Return True
        End If
        Dim nums = Enumerable.Range(1, 9).OrderBy(Function(x) random.Next()).ToArray()
        For Each num In nums
            If IsSafe(row, col, num) Then
                board(row, col) = num
                If GenerateFullSudoku(row, col + 1) Then Return True
                board(row, col) = 0
            End If
        Next
        Return False
    End Function

    Private Function IsSafe(row As Integer, col As Integer, num As Integer) As Boolean
        For i As Integer = 0 To 8
            If board(row, i) = num OrElse board(i, col) = num Then Return False
        Next
        Dim sR As Integer = (row \ 3) * 3, sC As Integer = (col \ 3) * 3
        For i As Integer = 0 To 2
            For j As Integer = 0 To 2
                If board(sR + i, sC + j) = num Then Return False
            Next
        Next
        Return True
    End Function

    Private Sub CheckSolution()
        Dim win As Boolean = True
        Dim emptyFields As Boolean = False

        For r As Integer = 0 To 8
            For c As Integer = 0 To 8
                If String.IsNullOrEmpty(cells(r, c).Text) Then
                    emptyFields = True
                    win = False
                    Continue For
                End If

                If cells(r, c).Text <> board(r, c).ToString() Then
                    win = False
                    If Not cells(r, c).ReadOnly Then cells(r, c).ForeColor = Color.Red
                Else
                    If Not cells(r, c).ReadOnly Then cells(r, c).ForeColor = Color.Blue
                End If
            Next
        Next

        If emptyFields Then
            MessageBox.Show("Please fill all the cells first!", "Notice")
        ElseIf win Then
            MessageBox.Show("Congratulations! You've solved it perfectly!", "Success")
        Else
            MessageBox.Show("Some numbers are incorrect. Please check the red marked cells.", "Failure")
        End If
    End Sub
End Class