Imports System.Drawing
Imports System.Windows.Forms

' 确保类继承自 Form
Public Class Form1
    Inherits Form

    ' 游戏常量
    Private Const BoardSize As Integer = 8
    Private Const CellSize As Integer = 60

    ' 游戏状态
    Private board(BoardSize - 1, BoardSize - 1) As Integer ' 0: 空, 1: 黑子, 2: 白子
    Private currentPlayer As Integer = 1
    Private buttons(BoardSize - 1, BoardSize - 1) As Button

    ' 显式定义构造函数
    Public Sub New()
        ' 设置窗体初始属性
        Me.DoubleBuffered = True
        Me.StartPosition = FormStartPosition.CenterScreen
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.Text = "VB.NET 黑白棋 (Reversi)"
        ' 设置窗口大小
        Me.ClientSize = New Size(BoardSize * CellSize, BoardSize * CellSize + 50)
        InitializeBoard()
    End Sub

    Private Sub InitializeBoard()
        Me.Controls.Clear()
        Array.Clear(board, 0, board.Length)

        For i As Integer = 0 To BoardSize - 1
            For j As Integer = 0 To BoardSize - 1
                Dim btn As New Button()
                btn.Size = New Size(CellSize, CellSize)
                btn.Location = New Point(j * CellSize, i * CellSize)
                btn.Tag = New Point(i, j)
                btn.FlatStyle = FlatStyle.Flat
                btn.BackColor = Color.ForestGreen
                AddHandler btn.Click, AddressOf Cell_Click
                buttons(i, j) = btn
                Me.Controls.Add(btn)
            Next
        Next

        board(3, 3) = 2 : board(3, 4) = 1
        board(4, 3) = 1 : board(4, 4) = 2

        UpdateBoardUI()
    End Sub

    Private Sub Cell_Click(sender As Object, e As EventArgs)
        Dim btn As Button = DirectCast(sender, Button)
        Dim pos As Point = DirectCast(btn.Tag, Point)
        Dim r As Integer = pos.X
        Dim c As Integer = pos.Y

        If IsValidMove(r, c, currentPlayer) Then
            MakeMove(r, c, currentPlayer)
            currentPlayer = If(currentPlayer = 1, 2, 1)

            If Not CanMove(currentPlayer) Then
                currentPlayer = If(currentPlayer = 1, 2, 1)
                If Not CanMove(currentPlayer) Then
                    UpdateBoardUI()
                    ShowResult()
                Else
                    MessageBox.Show("一方无棋可走，换手！")
                End If
            End If
            UpdateBoardUI()
        End If
    End Sub

    Private Sub UpdateBoardUI()
        Dim blackCount As Integer = 0
        Dim whiteCount As Integer = 0

        For i As Integer = 0 To BoardSize - 1
            For j As Integer = 0 To BoardSize - 1
                If board(i, j) = 1 Then
                    buttons(i, j).Text = "●"
                    buttons(i, j).ForeColor = Color.Black
                    blackCount += 1
                ElseIf board(i, j) = 2 Then
                    buttons(i, j).Text = "●"
                    buttons(i, j).ForeColor = Color.White
                    whiteCount += 1
                Else
                    buttons(i, j).Text = ""
                End If
            Next
        Next
        ' 使用全局限定名解决 vbCrLf 歧义，或者直接改用 ControlChars.CrLf
        Me.Text = String.Format("黑白棋 - 轮到: {0} (黑: {1} 白: {2})",
                                If(currentPlayer = 1, "黑", "白"), blackCount, whiteCount)
    End Sub

    Private Function IsValidMove(row As Integer, col As Integer, player As Integer) As Boolean
        If board(row, col) <> 0 Then Return False
        Return GetFlips(row, col, player).Count > 0
    End Function

    Private Function GetFlips(row As Integer, col As Integer, player As Integer) As List(Of Point)
        Dim flips As New List(Of Point)
        Dim opponent As Integer = If(player = 1, 2, 1)
        Dim directions() As Point = {
            New Point(-1, -1), New Point(-1, 0), New Point(-1, 1),
            New Point(0, -1), New Point(0, 1),
            New Point(1, -1), New Point(1, 0), New Point(1, 1)
        }

        For Each d In directions
            Dim tempFlips As New List(Of Point)
            Dim r As Integer = row + d.X
            Dim c As Integer = col + d.Y

            While r >= 0 AndAlso r < BoardSize AndAlso c >= 0 AndAlso c < BoardSize AndAlso board(r, c) = opponent
                tempFlips.Add(New Point(r, c))
                r += d.X
                c += d.Y
            End While

            If r >= 0 AndAlso r < BoardSize AndAlso c >= 0 AndAlso c < BoardSize AndAlso board(r, c) = player Then
                flips.AddRange(tempFlips)
            End If
        Next
        Return flips
    End Function

    Private Sub MakeMove(row As Integer, col As Integer, player As Integer)
        Dim flips = GetFlips(row, col, player)
        board(row, col) = player
        For Each p In flips
            board(p.X, p.Y) = player
        Next
    End Sub

    Private Function CanMove(player As Integer) As Boolean
        For i As Integer = 0 To BoardSize - 1
            For j As Integer = 0 To BoardSize - 1
                If IsValidMove(i, j, player) Then Return True
            Next
        Next
        Return False
    End Function

    Private Sub ShowResult()
        Dim black As Integer = 0
        Dim white As Integer = 0
        For Each cell In board
            If cell = 1 Then black += 1
            If cell = 2 Then white += 1
        Next

        Dim winner As String = If(black > white, "黑棋胜！", If(white > black, "白棋胜！", "平局！"))
        ' 使用 Environment.NewLine 替代 vbCrLf 避免歧义
        Dim msg As String = String.Format("游戏结束！{0}黑棋: {1}{0}白棋: {2}{0}{3}",
                                          Environment.NewLine, black, white, winner)
        MessageBox.Show(msg)
        InitializeBoard()
    End Sub
End Class