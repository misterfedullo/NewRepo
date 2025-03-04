<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        btnGenerate = New Button()
        TextBox1 = New TextBox()
        txtPrediction = New Button()
        btnSave = New Button()
        btnLoad = New Button()
        MenuStrip2 = New MenuStrip()
        MenuStrip3 = New MenuStrip()
        CaricaEstrazioniToolStripMenuItem = New ToolStripMenuItem()
        SalvaLArchivioToolStripMenuItem = New ToolStripMenuItem()
        OpenFileDialog1 = New OpenFileDialog()
        SaveFileDialog1 = New SaveFileDialog()
        DataGridView1 = New DataGridView()
        Column1 = New DataGridViewTextBoxColumn()
        Column2 = New DataGridViewTextBoxColumn()
        Column3 = New DataGridViewTextBoxColumn()
        Column4 = New DataGridViewTextBoxColumn()
        Column5 = New DataGridViewTextBoxColumn()
        Column6 = New DataGridViewTextBoxColumn()
        Column7 = New DataGridViewTextBoxColumn()
        Column8 = New DataGridViewTextBoxColumn()
        Column9 = New DataGridViewTextBoxColumn()
        Column10 = New DataGridViewTextBoxColumn()
        Column11 = New DataGridViewTextBoxColumn()
        TextBox2 = New TextBox()
        TextBox3 = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        DataGridStat = New DataGridView()
        Column12 = New DataGridViewTextBoxColumn()
        Column13 = New DataGridViewTextBoxColumn()
        Column14 = New DataGridViewTextBoxColumn()
        Column15 = New DataGridViewTextBoxColumn()
        Column16 = New DataGridViewTextBoxColumn()
        Button29 = New Button()
        ProgressBar1 = New ProgressBar()
        Label5 = New Label()
        Label6 = New Label()
        Button1 = New Button()
        DebugLog = New RichTextBox()
        Label7 = New Label()
        Label8 = New Label()
        CronoStat = New RichTextBox()
        Label9 = New Label()
        btnImportaTesto = New Button()
        MenuStrip3.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridStat, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnGenerate
        ' 
        btnGenerate.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        btnGenerate.Location = New Point(444, 39)
        btnGenerate.Name = "btnGenerate"
        btnGenerate.Size = New Size(201, 66)
        btnGenerate.TabIndex = 6
        btnGenerate.Text = "GENERA ANALISI" & vbCrLf & "A BLOCCHI" & vbCrLf
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(330, 129)
        TextBox1.Multiline = True
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(63, 27)
        TextBox1.TabIndex = 2
        ' 
        ' txtPrediction
        ' 
        txtPrediction.Location = New Point(37, 96)
        txtPrediction.Name = "txtPrediction"
        txtPrediction.Size = New Size(217, 45)
        txtPrediction.TabIndex = 3
        txtPrediction.Text = "PREDICTION( bunon privo di codice)"
        txtPrediction.UseVisualStyleBackColor = True
        ' 
        ' btnSave
        ' 
        btnSave.Location = New Point(1469, 165)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(115, 26)
        btnSave.TabIndex = 4
        btnSave.Text = "SALVA lo stato"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' btnLoad
        ' 
        btnLoad.Location = New Point(1469, 210)
        btnLoad.Name = "btnLoad"
        btnLoad.Size = New Size(86, 26)
        btnLoad.TabIndex = 5
        btnLoad.Text = "LOAD"
        btnLoad.UseVisualStyleBackColor = True
        ' 
        ' MenuStrip2
        ' 
        MenuStrip2.Location = New Point(0, 40)
        MenuStrip2.Name = "MenuStrip2"
        MenuStrip2.Padding = New Padding(7, 2, 0, 2)
        MenuStrip2.Size = New Size(1584, 24)
        MenuStrip2.TabIndex = 7
        MenuStrip2.Text = "MenuStrip2"
        ' 
        ' MenuStrip3
        ' 
        MenuStrip3.Items.AddRange(New ToolStripItem() {CaricaEstrazioniToolStripMenuItem, SalvaLArchivioToolStripMenuItem})
        MenuStrip3.Location = New Point(0, 0)
        MenuStrip3.Name = "MenuStrip3"
        MenuStrip3.Padding = New Padding(7, 2, 0, 2)
        MenuStrip3.Size = New Size(1584, 40)
        MenuStrip3.TabIndex = 8
        MenuStrip3.Text = "MenuStrip3"
        ' 
        ' CaricaEstrazioniToolStripMenuItem
        ' 
        CaricaEstrazioniToolStripMenuItem.Font = New Font("Segoe UI", 18.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        CaricaEstrazioniToolStripMenuItem.Name = "CaricaEstrazioniToolStripMenuItem"
        CaricaEstrazioniToolStripMenuItem.Size = New Size(215, 36)
        CaricaEstrazioniToolStripMenuItem.Text = "Carica Estrazioni"
        ' 
        ' SalvaLArchivioToolStripMenuItem
        ' 
        SalvaLArchivioToolStripMenuItem.Font = New Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        SalvaLArchivioToolStripMenuItem.Name = "SalvaLArchivioToolStripMenuItem"
        SalvaLArchivioToolStripMenuItem.Size = New Size(182, 36)
        SalvaLArchivioToolStripMenuItem.Text = "Salva l'Archivio"
        ' 
        ' OpenFileDialog1
        ' 
        OpenFileDialog1.FileName = "OpenFileDialog1"
        ' 
        ' DataGridView1
        ' 
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Columns.AddRange(New DataGridViewColumn() {Column1, Column2, Column3, Column4, Column5, Column6, Column7, Column8, Column9, Column10, Column11})
        DataGridView1.Location = New Point(307, 163)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.Size = New Size(709, 401)
        DataGridView1.TabIndex = 9
        ' 
        ' Column1
        ' 
        Column1.HeaderText = "Id"
        Column1.Name = "Column1"
        Column1.Width = 45
        ' 
        ' Column2
        ' 
        Column2.HeaderText = "Data"
        Column2.Name = "Column2"
        Column2.Width = 62
        ' 
        ' Column3
        ' 
        Column3.HeaderText = "Conc"
        Column3.Name = "Column3"
        Column3.Width = 63
        ' 
        ' Column4
        ' 
        Column4.HeaderText = "E1"
        Column4.Name = "Column4"
        Column4.Width = 47
        ' 
        ' Column5
        ' 
        Column5.HeaderText = "E2"
        Column5.Name = "Column5"
        Column5.Width = 47
        ' 
        ' Column6
        ' 
        Column6.HeaderText = "E3"
        Column6.Name = "Column6"
        Column6.Width = 47
        ' 
        ' Column7
        ' 
        Column7.HeaderText = "E4"
        Column7.Name = "Column7"
        Column7.Width = 47
        ' 
        ' Column8
        ' 
        Column8.HeaderText = "E5"
        Column8.Name = "Column8"
        Column8.Width = 47
        ' 
        ' Column9
        ' 
        Column9.HeaderText = "E6"
        Column9.Name = "Column9"
        Column9.Width = 47
        ' 
        ' Column10
        ' 
        Column10.HeaderText = "J"
        Column10.Name = "Column10"
        Column10.Width = 39
        ' 
        ' Column11
        ' 
        Column11.HeaderText = "Ss"
        Column11.Name = "Column11"
        Column11.Width = 46
        ' 
        ' TextBox2
        ' 
        TextBox2.Location = New Point(415, 129)
        TextBox2.Name = "TextBox2"
        TextBox2.Size = New Size(63, 25)
        TextBox2.TabIndex = 10
        ' 
        ' TextBox3
        ' 
        TextBox3.Location = New Point(37, 165)
        TextBox3.Multiline = True
        TextBox3.Name = "TextBox3"
        TextBox3.Size = New Size(231, 392)
        TextBox3.TabIndex = 11
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(330, 109)
        Label1.Name = "Label1"
        Label1.Size = New Size(25, 17)
        Label1.TabIndex = 12
        Label1.Text = "Da"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(415, 109)
        Label2.Name = "Label2"
        Label2.Size = New Size(15, 17)
        Label2.TabIndex = 13
        Label2.Text = "a"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(500, 135)
        Label3.Name = "Label3"
        Label3.Size = New Size(53, 17)
        Label3.TabIndex = 14
        Label3.Text = "records"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(559, 137)
        Label4.Name = "Label4"
        Label4.Size = New Size(22, 17)
        Label4.TabIndex = 15
        Label4.Text = "00"
        ' 
        ' DataGridStat
        ' 
        DataGridStat.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        DataGridStat.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridStat.Columns.AddRange(New DataGridViewColumn() {Column12, Column13, Column14, Column15, Column16})
        DataGridStat.Location = New Point(1042, 165)
        DataGridStat.Name = "DataGridStat"
        DataGridStat.Size = New Size(419, 399)
        DataGridStat.TabIndex = 16
        ' 
        ' Column12
        ' 
        Column12.HeaderText = "Id"
        Column12.Name = "Column12"
        Column12.Width = 45
        ' 
        ' Column13
        ' 
        Column13.HeaderText = "Usc"
        Column13.Name = "Column13"
        Column13.Width = 54
        ' 
        ' Column14
        ' 
        Column14.HeaderText = "Ra"
        Column14.Name = "Column14"
        Column14.Width = 48
        ' 
        ' Column15
        ' 
        Column15.HeaderText = "Rm"
        Column15.Name = "Column15"
        Column15.Width = 53
        ' 
        ' Column16
        ' 
        Column16.HeaderText = "Rapp Perc Ra/Rm"
        Column16.Name = "Column16"
        Column16.Width = 127
        ' 
        ' Button29
        ' 
        Button29.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Button29.Location = New Point(307, 43)
        Button29.Name = "Button29"
        Button29.Size = New Size(131, 62)
        Button29.TabIndex = 17
        Button29.Text = "Statistica normale"
        Button29.UseVisualStyleBackColor = True
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(1042, 148)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(181, 16)
        ProgressBar1.TabIndex = 18
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label5.Location = New Point(637, 135)
        Label5.Name = "Label5"
        Label5.Size = New Size(114, 17)
        Label5.TabIndex = 19
        Label5.Text = "lunghezza blocco"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(757, 135)
        Label6.Name = "Label6"
        Label6.Size = New Size(22, 17)
        Label6.TabIndex = 20
        Label6.Text = "00"
        ' 
        ' Button1
        ' 
        Button1.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Button1.Location = New Point(883, 119)
        Button1.Name = "Button1"
        Button1.Size = New Size(133, 36)
        Button1.TabIndex = 21
        Button1.Text = "Clear risultanze"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' DebugLog
        ' 
        DebugLog.Location = New Point(320, 587)
        DebugLog.Name = "DebugLog"
        DebugLog.Size = New Size(484, 244)
        DebugLog.TabIndex = 22
        DebugLog.Text = ""
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(321, 567)
        Label7.Name = "Label7"
        Label7.Size = New Size(72, 17)
        Label7.TabIndex = 23
        Label7.Text = "DebugLog"
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Location = New Point(1042, 119)
        Label8.Name = "Label8"
        Label8.Size = New Size(109, 17)
        Label8.TabIndex = 24
        Label8.Text = "Analisi statistica"
        ' 
        ' CronoStat
        ' 
        CronoStat.Font = New Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        CronoStat.Location = New Point(1042, 587)
        CronoStat.Name = "CronoStat"
        CronoStat.Size = New Size(419, 244)
        CronoStat.TabIndex = 25
        CronoStat.Text = ""
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Location = New Point(1053, 567)
        Label9.Name = "Label9"
        Label9.Size = New Size(69, 17)
        Label9.TabIndex = 26
        Label9.Text = "CronoStat"
        ' 
        ' btnImportaTesto
        ' 
        btnImportaTesto.Location = New Point(651, 67)
        btnImportaTesto.Name = "btnImportaTesto"
        btnImportaTesto.Size = New Size(237, 23)
        btnImportaTesto.TabIndex = 27
        btnImportaTesto.Text = "Importa testo per aggiornamento"
        btnImportaTesto.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8.0F, 17.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1584, 843)
        Controls.Add(btnImportaTesto)
        Controls.Add(Label9)
        Controls.Add(CronoStat)
        Controls.Add(Label8)
        Controls.Add(Label7)
        Controls.Add(DebugLog)
        Controls.Add(Button1)
        Controls.Add(Label6)
        Controls.Add(Label5)
        Controls.Add(ProgressBar1)
        Controls.Add(Button29)
        Controls.Add(DataGridStat)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(TextBox3)
        Controls.Add(TextBox2)
        Controls.Add(DataGridView1)
        Controls.Add(btnLoad)
        Controls.Add(btnSave)
        Controls.Add(txtPrediction)
        Controls.Add(TextBox1)
        Controls.Add(btnGenerate)
        Controls.Add(MenuStrip2)
        Controls.Add(MenuStrip3)
        Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        MainMenuStrip = MenuStrip2
        Name = "Form1"
        Text = "Form1"
        MenuStrip3.ResumeLayout(False)
        MenuStrip3.PerformLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridStat, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub


    Friend WithEvents btnGenerate As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents txtPrediction As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnLoad As Button
    Friend WithEvents MenuStrip2 As MenuStrip
    Friend WithEvents MenuStrip3 As MenuStrip
    Friend WithEvents CaricaEstrazioniToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents Column7 As DataGridViewTextBoxColumn
    Friend WithEvents Column8 As DataGridViewTextBoxColumn
    Friend WithEvents Column9 As DataGridViewTextBoxColumn
    Friend WithEvents Column10 As DataGridViewTextBoxColumn
    Friend WithEvents Column11 As DataGridViewTextBoxColumn
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents DataGridStat As DataGridView
    Friend WithEvents Button29 As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Column12 As DataGridViewTextBoxColumn
    Friend WithEvents Column13 As DataGridViewTextBoxColumn
    Friend WithEvents Column14 As DataGridViewTextBoxColumn
    Friend WithEvents Column15 As DataGridViewTextBoxColumn
    Friend WithEvents Column16 As DataGridViewTextBoxColumn
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents DebugLog As RichTextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents CronoStat As RichTextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents SalvaLArchivioToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnImportaTesto As Button

End Class
