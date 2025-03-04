
Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports System.Reflection.Emit
Imports System.Text
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Xml.Serialization
Imports System.Linq


Public Class Form1
    Inherits Form

    Private menuStrip1 As MenuStrip
    Private fileToolStripMenuItem As ToolStripMenuItem
    Private openToolStripMenuItem As ToolStripMenuItem
    Private saveToolStripMenuItem As ToolStripMenuItem
    Private exitToolStripMenuItem As ToolStripMenuItem
    Public Shared DalRecord, AlRecord As Integer
    Public UscDivisoRa(91)
    Public UscDivisoRmax(91)
    Public ind As Integer : Public _I90num(91) As Integer
    Public Messaggio As String = ""
    Public NumBlocco As String = ""
    Public Frequenza(91) As Double : Public ColonnaIndice8(91) As Integer : Public ArryTestIndex(91) As Integer : Public ColonnaFrequenze(91) As Double
    Public dato
    Public Volte_(91) As Integer
    Public counterRipetizioniBlocchi As Integer = 0
    Public FineArchivio As Integer
    Public InizioRecord, FineRecords As Integer
    Public ProssimoInizioRecord As Integer
    Public ProssimoFineRecord As Integer
    Public RecordSuccessivoAlFineBlocco As Integer

    Public EccedenzaBlocco As Integer
    Public SegnalazionePerInterruzioneLavoro As Boolean = False
    Public percSviluppo As Integer = 0
    Public NumeroRecDataGridView1 As Integer
    Private blocco As Integer
    Private Uscite(90) As Integer      ' Modificato per indici 1-90
    Private Ritardo(90) As Integer     ' Modificato per indici 1-90
    Private RitardoMax(90) As Integer  ' Modificato per indici 1-90
    Private CurrentWeights As PredictionWeights
    'Public 
    Public Sub New()
        InitializeComponent()
        Me.fileToolStripMenuItem = New ToolStripMenuItem()
        Me.openToolStripMenuItem = New ToolStripMenuItem()
        Me.saveToolStripMenuItem = New ToolStripMenuItem()
        Me.exitToolStripMenuItem = New ToolStripMenuItem()


    End Sub

    Private Structure PredictionWeights
        Public FrequencyWeight As Double
        Public DelayWeight As Double
        Public TrendWeight As Double
        Public SequenceWeight As Double
    End Structure

    ' Aggiungi queste variabili alla classe Form1
    Private Structure PredictionResult
        Public PredictedNumbers() As Integer
        Public ActualNumbers() As Integer
        Public MatchCount As Integer
        Public Accuracy As Double
        Public WeightFactors As PredictionWeights
    End Structure
    Private PredictionHistory As New List(Of PredictionResult)
    Private Sub InitializePredictionWeights()
        CurrentWeights = New PredictionWeights With {
        .FrequencyWeight = 0.6,
        .DelayWeight = 0.4
    }
    End Sub
    Private Function GeneratePrediction() As PredictionResult
        Dim predictedNumbers = GenerateNextPrediction()
        Return New PredictionResult With {
        .PredictedNumbers = predictedNumbers,
        .Accuracy = CalculatePredictionConfidence(predictedNumbers)
    }
    End Function

    Private Function GenerateNextPrediction() As Integer()
        ' Array per memorizzare i punteggi di ogni numero
        Dim scores(90) As Double    ' 1-90 per i numeri
        Dim predictions(5) As Integer  ' Per 6 numeri

        ' Calcola i punteggi per ogni numero
        For num As Integer = 1 To 90
            ' Calcola il punteggio combinando frequenza e ritardo
            Dim frequencyFactor = Uscite(num) / blocco
            Dim delayFactor = If(RitardoMax(num) > 0, CDbl(Ritardo(num)) / RitardoMax(num), 0)
            scores(num) = (frequencyFactor * CurrentWeights.FrequencyWeight) +
                     (delayFactor * CurrentWeights.DelayWeight)
        Next

        ' Seleziona i 6 numeri con i punteggi più alti
        Dim selectedCount As Integer = 0
        While selectedCount < 6
            ' Trova il numero con il punteggio più alto non ancora selezionato
            Dim maxScore As Double = -1
            Dim selectedNumber As Integer = -1

            For i As Integer = 1 To 90
                If scores(i) > maxScore AndAlso Not predictions.Contains(i) Then
                    maxScore = scores(i)
                    selectedNumber = i
                End If
            Next

            If selectedNumber <> -1 Then
                predictions(selectedCount) = selectedNumber
                selectedCount += 1
            End If
        End While

        ' Ordina i numeri in ordine crescente
        Array.Sort(predictions)
        Return predictions
    End Function
    Private Sub ShowPredictionDialog(prediction As PredictionResult)
        Dim message = "Previsione per la prossima estrazione:" & vbCrLf &
                 String.Join(" - ", prediction.PredictedNumbers)

        '  MessageBox.Show(message, "Previsione Numeri",
        'MessageBoxButtons.OK, MessageBoxIcon.Information)
        DebugLog.AppendText(Messaggio + " : " + Environment.NewLine)
        DebugLog.AppendText(String.Join(" - ", prediction.PredictedNumbers) + Environment.NewLine)
    End Sub


    Private Sub UpdateStatisticsGrid(prediction As PredictionResult)
        Dim rowIndex As Integer = DataGridStat.Rows.Add()
        With DataGridStat.Rows(rowIndex)
            .Cells(0).Value = "Previsione"
            .Cells(1).Value = String.Join(", ", prediction.PredictedNumbers)
        End With
    End Sub
    Private Function CalculatePredictionScore(number As Integer, weights As PredictionWeights) As Double
        Dim score As Double = 0

        ' Frequenza relativa
        Dim frequencyScore As Double = Uscite(number - 1) / blocco
        score += frequencyScore * weights.FrequencyWeight

        ' Ritardo
        Dim delayScore As Double = If(RitardoMax(number) > 0,
                                 CDbl(Ritardo(number)) / RitardoMax(number),
                                 0)
        score += delayScore * weights.DelayWeight

        Return score
    End Function
    Private Function CalculateSequenceScore(number As Integer) As Double
        ' Analizza i pattern di sequenza nelle ultime estrazioni
        Dim sequenceScore As Double = 0
        Dim lastOccurrence As Integer = -1

        ' Cerca l'ultima occorrenza del numero
        For i As Integer = FineRecords - 1 To InizioRecord Step -1
            For j As Integer = 3 To 8
                If CInt(DataGridView1.Rows(i).Cells(j).Value) = number Then
                    lastOccurrence = i
                    Exit For
                End If
            Next
            If lastOccurrence <> -1 Then Exit For
        Next

        If lastOccurrence <> -1 Then
            ' Calcola il pattern basato sulla distanza dall'ultima occorrenza
            Dim distance As Integer = FineRecords - lastOccurrence
            sequenceScore = 1.0 / (1 + distance)
        End If

        Return sequenceScore
    End Function
    Private Sub AdjustWeights(result As PredictionResult)
        ' Aggiusta i pesi in base all'accuratezza della previsione
        Const AdjustmentFactor As Double = 0.05

        If result.Accuracy < 0.2 Then  ' Meno di 2 numeri indovinati
            ' Aumenta i pesi che hanno dato i migliori risultati storici
            Dim bestWeight As String = FindBestHistoricalWeight()
            Select Case bestWeight
                Case "Frequency"
                    CurrentWeights.FrequencyWeight += AdjustmentFactor
                Case "Delay"
                    CurrentWeights.DelayWeight += AdjustmentFactor
                Case "Trend"
                    CurrentWeights.TrendWeight += AdjustmentFactor
                Case "Sequence"
                    CurrentWeights.SequenceWeight += AdjustmentFactor
            End Select

            ' Normalizza i pesi
            NormalizeWeights()
        End If
    End Sub
    Private Function FindBestHistoricalWeight() As String
        ' Analizza lo storico per trovare quale peso ha dato i migliori risultati
        If PredictionHistory.Count < 2 Then Return "Frequency"

        Dim lastResults = PredictionHistory.TakeLast(5).ToList()
        Dim avgAccuracy As New Dictionary(Of String, Double) From {
        {"Frequency", 0}, {"Delay", 0}, {"Trend", 0}, {"Sequence", 0}
    }

        For Each result In lastResults
            With result.WeightFactors
                avgAccuracy("Frequency") += .FrequencyWeight * result.Accuracy
                avgAccuracy("Delay") += .DelayWeight * result.Accuracy
                avgAccuracy("Trend") += .TrendWeight * result.Accuracy
                avgAccuracy("Sequence") += .SequenceWeight * result.Accuracy
            End With
        Next

        Return avgAccuracy.OrderByDescending(Function(x) x.Value).First().Key
    End Function
    Private Sub NormalizeWeights()
        ' Normalizza i pesi in modo che la loro somma sia 1
        Dim total As Double = CurrentWeights.FrequencyWeight +
                        CurrentWeights.DelayWeight +
                        CurrentWeights.TrendWeight +
                        CurrentWeights.SequenceWeight

        CurrentWeights.FrequencyWeight /= total
        CurrentWeights.DelayWeight /= total
        CurrentWeights.TrendWeight /= total
        CurrentWeights.SequenceWeight /= total
    End Sub
    Private Sub DisplayPredictionResults(prediction As PredictionResult)
        ' Aggiunge una riga alla DataGridStat per visualizzare la previsione
        Dim rowIndex As Integer = DataGridStat.Rows.Add()

        With DataGridStat.Rows(rowIndex)
            .Cells(0).Value = "Previsione"
            .Cells(1).Value = String.Join(", ", prediction.PredictedNumbers)
            .Cells(2).Value = String.Join(", ", prediction.ActualNumbers)
            .Cells(3).Value = prediction.MatchCount
            .Cells(4).Value = $"{prediction.Accuracy:P2}"
        End With

        ' Aggiorna la UI
        DataGridStat.Update()
        Application.DoEvents()
    End Sub


    Public Class ControlState
        Public Property Name As String
        Public Property Value As Object
    End Class

    Public Class FormState
        Public Property Width As Integer
        Public Property Height As Integer
        Public Property LocationX As Integer
        Public Property LocationY As Integer
        Public Property Controls As List(Of ControlState)
    End Class
    ' Struttura per le previsioni
    Private Structure PredictionMetrics
        Public PredictedNumbers() As Integer    ' Numeri previsti
        Public Confidence() As Double           ' Livello di confidenza per ogni numero
        Public ActualNumbers() As Integer       ' Numeri effettivamente estratti
        Public Accuracy As Double              ' Precisione della previsione
    End Structure

    ' Array per tenere traccia delle previsioni per ogni blocco
    Private BlockPredictions As New List(Of PredictionMetrics)

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
        ' Calcola la dimensione del blocco dai valori di TextBox1 e TextBox2
        Dim startValue, endValue As Integer
        If Not Integer.TryParse(TextBox1.Text, startValue) OrElse
   Not Integer.TryParse(TextBox2.Text, endValue) Then
            MessageBox.Show("Valori non validi nei campi Da/a", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Calcola la dimensione del blocco
        blocco = endValue - startValue
        If blocco <= 0 Then
            MessageBox.Show("Intervallo non valido", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Aggiorna l'etichetta con la dimensione del blocco
        Label6.Text = "" & blocco.ToString("00")
        Label6.Update()

        InitializePredictionWeights()
        MarioStatisticaPerBlocchi()

        ' NON chiamare queste funzioni qui, sono già chiamate in MarioStatisticaPerBlocchi()
        ' UpdateStatistics()
        ' Dim prediction = GeneratePrediction()
        ' ShowPredictionDialog(prediction)
        ' UpdateStatisticsGrid(prediction)
    End Sub

    Private Sub UpdateBlockSizeLabel()
        Label6.Text = "lunghezza blocco " & blocco.ToString("00")
    End Sub
    Private Function CalculatePredictionConfidence(predictedNumbers As Integer()) As Double
        Dim confidence As Double = 0
        For Each num In predictedNumbers
            Dim frequencyFactor = Uscite(num) / blocco
            Dim delayFactor = If(RitardoMax(num) > 0, CDbl(Ritardo(num)) / RitardoMax(num), 0)
            confidence += (frequencyFactor * 0.6 + delayFactor * 0.4)
        Next
        Return confidence / predictedNumbers.Length
    End Function

    Private Sub ShowFinalStats()
        If PredictionHistory.Count > 0 Then
            Dim avgAccuracy As Double = PredictionHistory.Average(Function(p) p.Accuracy)
            Dim bestPrediction = PredictionHistory.OrderByDescending(Function(p) p.MatchCount).First()

            MessageBox.Show(
                $"Analisi completata{Environment.NewLine}" &
                $"Numero totale previsioni: {PredictionHistory.Count}{Environment.NewLine}" &
                $"Accuratezza media: {avgAccuracy:P2}{Environment.NewLine}" &
                $"Miglior previsione: {bestPrediction.MatchCount} numeri corretti{Environment.NewLine}" &
                $"Pesi finali algoritmo:{Environment.NewLine}" &
                $"Frequenza: {CurrentWeights.FrequencyWeight:P2}{Environment.NewLine}" &
                $"Ritardo: {CurrentWeights.DelayWeight:P2}{Environment.NewLine}" &
                $"Trend: {CurrentWeights.TrendWeight:P2}{Environment.NewLine}" &
                $"Sequenza: {CurrentWeights.SequenceWeight:P2}"
            )
        End If
    End Sub


    Private Function AnalyzeBlockAndPredict() As PredictionMetrics
        Dim prediction As New PredictionMetrics
        ReDim prediction.PredictedNumbers(5)    ' 6 numeri da predire
        ReDim prediction.Confidence(5)
        ReDim prediction.ActualNumbers(5)

        ' Esegue analisi statistica del blocco
        GeneraRipBlocc()

        ' Analizza pattern statistici per la previsione
        For i As Integer = 0 To 5
            Dim bestNumber As Integer = PredictNextNumber()
            prediction.PredictedNumbers(i) = bestNumber
            prediction.Confidence(i) = CalculateConfidence(bestNumber)
        Next

        Return prediction
    End Function

    Private Function PredictNextNumber() As Integer
        ' Logica per predire il prossimo numero basata su:
        ' 1. Frequenza di uscita nel blocco corrente
        ' 2. Ritardo attuale
        ' 3. Pattern di uscita
        Dim bestNumber As Integer = 1
        Dim maxScore As Double = 0

        For num As Integer = 1 To 90
            Dim score As Double = CalculatePredictionScore(num)
            If score > maxScore Then
                maxScore = score
                bestNumber = num
            End If
        Next

        Return bestNumber
    End Function

    Private Function CalculatePredictionScore(number As Integer) As Double
        ' Calcola un punteggio predittivo basato su multiple metriche
        Dim score As Double = 0

        ' Peso della frequenza relativa
        score += (Uscite(number) / blocco) * 0.3

        ' Peso del ritardo (maggiore è il ritardo, maggiore è la probabilità di uscita)
        If RitardoMax(number) > 0 Then
            score += (Ritardo(number) / RitardoMax(number)) * 0.4
        End If

        ' Trend delle ultime estrazioni
        score += CalculateTrendScore(number) * 0.3

        Return score
    End Function

    Private Function CalculateConfidence(number As Integer) As Double
        ' Calcola il livello di confidenza della previsione
        Return CalculatePredictionScore(number) * 100
    End Function

    Private Sub VerifyPrediction(prediction As PredictionMetrics, nextRecordIndex As Integer)
        ' Verifica la previsione contro l'estrazione reale
        Dim matches As Integer = 0

        ' Ottiene i numeri effettivamente estratti
        For j As Integer = 3 To 8
            prediction.ActualNumbers(j - 3) = CInt(DataGridView1.Rows(nextRecordIndex).Cells(j).Value)
        Next

        ' Conta le corrispondenze
        For i As Integer = 0 To 5
            If prediction.ActualNumbers.Contains(prediction.PredictedNumbers(i)) Then
                matches += 1
            End If
        Next

        prediction.Accuracy = matches / 6.0 * 100
    End Sub

    Private Sub ShowPredictionResults()
        ' Aggiunge colonne al DataGridStat per i risultati predittivi
        If Not DataGridStat.Columns.Contains("PredAccuracy") Then
            DataGridStat.Columns.Add("PredAccuracy", "Accuratezza Predizione (%)")
        End If

        ' Visualizza statistiche delle previsioni
        Dim totalAccuracy As Double = BlockPredictions.Average(Function(p) p.Accuracy)
        MessageBox.Show($"Accuratezza media delle previsioni: {totalAccuracy:F2}%")
    End Sub


    Private Sub openToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Codice per aprire un file
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "File di testo|*.txt"
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                ' Logica per gestire l'apertura del file
                Dim filePath As String = openFileDialog.FileName
                ' Ad esempio, qui puoi leggere i dati del file
            End If
        End Using
    End Sub

    Private Sub saveToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Codice per salvare un file
        Using saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "File di testo|*.txt"
            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Dim filePath As String = saveFileDialog.FileName
                ' Logica per salvare i dati nel file
            End If
        End Using
    End Sub

    Private Sub exitToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Codice per uscire dall'applicazione
        Me.Close()
    End Sub



    Private Sub CaricaEstrazioniToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CaricaEstrazioniToolStripMenuItem.Click
        ' Carica dati da file nel DataGridView
        Dim filePath As String = "C:\BkArchivio\DatagridArchivio.txt"

        ' Verifica se il file esiste prima di procedere
        If Not System.IO.File.Exists(filePath) Then
            MessageBox.Show("Il file specificato non esiste.")
            Return
        End If

        ' Pulisci il DataGridView prima di ricaricare i dati
        DataGridView1.Rows.Clear()
        ' TextBox4.Text = ""
        ' Leggi il file e aggiungi i dati nel DataGridView
        Using sr As New StreamReader(filePath)
            While Not sr.EndOfStream
                Dim line As String = sr.ReadLine()
                If Not String.IsNullOrWhiteSpace(line) Then
                    ' Dividi la EstrattoArchivioStorico per tabulazioni
                    Dim cells() As String = line.Split(vbTab)

                    ' Controlla che ci siano abbastanza celle
                    If cells.Length >= 11 Then
                        ' Aggiungi una nuova EstrattoArchivioStorico nel DataGridView
                        DataGridView1.Rows.Add(Convert.ToInt32(cells(0)),               ' Id
                                           DateTime.Parse(cells(1)),  ' DataConc
                                           Convert.ToInt32(cells(2)), ' NumConc
                                           Convert.ToInt32(cells(3)), ' E1
                                           Convert.ToInt32(cells(4)), ' E2
                                           Convert.ToInt32(cells(5)), ' E3
                                           Convert.ToInt32(cells(6)), ' E4
                                           Convert.ToInt32(cells(7)), ' E5
                                           Convert.ToInt32(cells(8)), ' E6
                                           Convert.ToInt32(cells(9)), ' Jolly
                                           Convert.ToInt32(cells(10)) ' Superstar
                                           )
                    End If
                End If
            End While
        End Using

        Dim successiva_ad_a_ = DataGridView1.Rows.Count - 2
        DalRecord = 1 : AlRecord = DataGridView1.Rows.Count - 1
        TextBox1.Text = Convert.ToString(DalRecord)
        TextBox2.Text = Convert.ToString(AlRecord)
        For fd As Integer = 3 To 8
            ' TextBox4.Text += DataGridView1.Rows(successiva_ad_a_).Cells(fd).Value.ToString() + ","
        Next

        ' Scorri automaticamente alla fine del DataGridView
        If DataGridView1.Rows.Count > 0 Then
            DataGridView1.FirstDisplayedScrollingRowIndex = DataGridView1.Rows.Count - 1
        End If
        Label4.Text = Convert.ToString(DataGridView1.Rows.Count - 1)
        Label4.Update()
        blocco = CInt(TextBox2.Text) - CInt(TextBox1.Text)
        Label6.Text = blocco
        Label6.Update()
        '   MessageBox.Show("Dati caricati con successo!")

    End Sub

    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        'statistiche senza routine ripetizione di blocchi di statistica
        LeggiTextBox()
        Statistica()
    End Sub
    Public Sub Statistica()
        ' Reset dei contatori
        For x As Integer = 1 To 90
            Ritardo(x) = 0
            RitardoMax(x) = 0
            Uscite(x) = 0
        Next
        'Clear DebugLog
        DebugLog.Clear()
        Dim riga As Integer
        Label4.Text = Convert.ToString(DataGridView1.Rows.Count - 1)
        Label4.Update()
        blocco = CInt((TextBox2.Text) - CInt(TextBox1.Text)) + 1
        Label6.Text = blocco
        Label6.Update()
        Dim arry3_8(6) As Integer
        Dim FineRecords As Integer = TextBox2.Text
        FineRecords = FineRecords - 1
        Dim InizioRecord As Integer = 0
        For z = 1 To 90
            For i As Integer = InizioRecord To FineRecords ' parte da indice 0
                riga = i ' cioè da 0 a 89
                For j = 3 To 8
                    dato = CInt(DataGridView1.Rows(i).Cells(j).Value)
                    arry3_8(j - 3) = dato
                Next j
                If arry3_8.Contains(z) Then
                    Uscite(z) += 1
                    Ritardo(z) = 0
                Else
                    Ritardo(z) += 1
                End If
                '   DebugLog.AppendText("Num " + z.ToString + " Riga " + (riga + 1).ToString + " Rmax = " + RitardoMax(z).ToString + " Ra = " + Ritardo(z).ToString + Environment.NewLine)
                If RitardoMax(z) < Ritardo(z) Then ' z si riferisce ai numeri 1-90

                    RitardoMax(z) = Ritardo(z)
                    '    DebugLog.AppendText(" Ritardo massimo aggiornato a " + RitardoMax(z).ToString + Environment.NewLine)
                End If
            Next

        Next z

        ' Aggiornamento DataGridStat
        DataGridStat.Rows.Clear()
        Dim counter As Integer = 0
        For num As Integer = 0 To 89
            counter += 1
            Dim row = DataGridStat.Rows.Add()
            DataGridStat.Rows(row).Cells(0).Value = counter
            DataGridStat.Rows(row).Cells(1).Value = Uscite(counter)
            DataGridStat.Rows(row).Cells(2).Value = Ritardo(counter)
            DataGridStat.Rows(row).Cells(3).Value = RitardoMax(counter)

            ' Calcolo percentuale ritardo
            If RitardoMax(counter) > 0 Then
                Dim percRitardo As Double = (Ritardo(counter) * 100.0) / RitardoMax(counter)
                DataGridStat.Rows(row).Cells(4).Value = Math.Round(percRitardo, 2)
            End If
        Next
    End Sub

    Public Sub MarioStatisticaPerBlocchi()
        ' Pulisci log e cronologia all'inizio
        DebugLog.Clear()
        CronoStat.Clear()

        ' Calcola il numero totale di record disponibili
        Dim TotaleRecords As Integer = DataGridView1.Rows.Count - 1
        Label4.Text = Convert.ToString(TotaleRecords)

        ' Leggi la dimensione del blocco dai TextBox
        Dim startValue As Integer = CInt(TextBox1.Text)  ' Sempre 1
        Dim endValue As Integer = CInt(TextBox2.Text)    ' Dimensione specificata dall'utente
        blocco = endValue - startValue + 1
        Label6.Text = blocco.ToString()

        ' Determina quanti blocchi completi possiamo fare
        Dim numeroBloccoPossibili As Integer = (TotaleRecords / blocco)
        If numeroBloccoPossibili < 1 Then numeroBloccoPossibili = 1

        ' Arrotonda a intero
        Dim numeroBlocchiInteri As Integer = CInt(Math.Floor(numeroBloccoPossibili))

        ' Arrays e variabili di lavoro
        Dim arry3_8(6) As Integer

        ' Processa ogni blocco
        For bloccoCorrente As Integer = 1 To numeroBlocchiInteri
            ' Reset contatori per il nuovo blocco
            For x As Integer = 1 To 90
                Ritardo(x) = 0
                RitardoMax(x) = 0
                Uscite(x) = 0
            Next

            ' Determina l'ultimo record per questo blocco
            Dim ultimoRecord As Integer = Math.Min(blocco * bloccoCorrente, TotaleRecords)

            ' Elaborazione per ogni numero da 1 a 90
            For z = 1 To 90
                ' Per ogni record nel blocco corrente (sempre partendo da 0 = record 1)
                For i As Integer = 0 To ultimoRecord - 1
                    ' Leggi i 6 numeri estratti in questo record
                    For j = 3 To 8
                        dato = CInt(DataGridView1.Rows(i).Cells(j).Value)
                        arry3_8(j - 3) = dato
                    Next j

                    ' Verifica se il numero z è presente in questa estrazione
                    If arry3_8.Contains(z) Then
                        Uscite(z) += 1
                        Ritardo(z) = 0
                    Else
                        Ritardo(z) += 1
                    End If

                    ' Aggiorna il ritardo massimo se necessario
                    If RitardoMax(z) < Ritardo(z) Then
                        RitardoMax(z) = Ritardo(z)
                    End If
                Next i
            Next z

            ' Aggiornamento DataGridStat
            DataGridStat.Rows.Clear()
            For num As Integer = 1 To 90
                Dim row = DataGridStat.Rows.Add()
                DataGridStat.Rows(row).Cells(0).Value = num
                DataGridStat.Rows(row).Cells(1).Value = Uscite(num)
                DataGridStat.Rows(row).Cells(2).Value = Ritardo(num)
                DataGridStat.Rows(row).Cells(3).Value = RitardoMax(num)

                ' Calcolo percentuale ritardo
                If RitardoMax(num) > 0 Then
                    Dim percRitardo As Double = (Ritardo(num) * 100.0) / RitardoMax(num)
                    DataGridStat.Rows(row).Cells(4).Value = Math.Round(percRitardo, 2)
                Else
                    DataGridStat.Rows(row).Cells(4).Value = 0
                End If
            Next

            ' Aggiorna informazioni sul blocco
            NumBlocco = bloccoCorrente.ToString()
            Messaggio = "Blocco " + NumBlocco + " Records 1-" + ultimoRecord.ToString()

            ' Aggiorna statistiche e genera previsione
            UpdateStatistics()
            Dim prediction = GeneratePrediction()
            ShowPredictionDialog(prediction)
            UpdateStatisticsGrid(prediction)

            ' Aggiorna i TextBox per riflettere il blocco corrente
            TextBox1.Text = "1"
            TextBox2.Text = ultimoRecord.ToString()
            TextBox1.Update()
            TextBox2.Update()
        Next
    End Sub

    Private Function NumeroPresente(numero As Integer, riga As Integer) As Boolean
        ' Controlla se il numero è presente nell'estrazione
        For colonna As Integer = 3 To 8 ' Colonne E1-E6
            If CInt(DataGridView1.Rows(riga).Cells(colonna).Value) = numero Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub GeneraRipBlocc()
        writeLog(" Procedura GeneraRipBlocc()" + Environment.NewLine)
        If SegnalazionePerInterruzioneLavoro = True Then
            writeLog("Procedura GeneraRipBlocc()" + " : " + "SegnalazionePerInterruzioneLavoro = True  ")
            writeLog(" linea n. 266: se true Exit Sub")
            Exit Sub
        End If
        DataGridStat.Rows.Clear()
        FineArchivio = DataGridView1.Rows.Count - 1
        InizioRecord = CInt(TextBox1.Text)
        FineRecords = CInt(TextBox2.Text)
        DataGridStat.Update()
        blocco = (FineRecords - InizioRecord) + 1
        Label6.Text = blocco.ToString
        writeLog("linea 278 " + "  InizioRecord e FineRecords = " + InizioRecord.ToString + " - " + FineRecords.ToString + Environment.NewLine)
        writeLog("  blocco = " + blocco.ToString + Environment.NewLine)
        NumeroRecDataGridView1 = DataGridView1.Rows.Count - 1
        Dim q As Integer = NumeroRecDataGridView1 / blocco
        percSviluppo = counterRipetizioniBlocchi * 100 / blocco
        If percSviluppo > 0 Then
            ProgressBar1.Value = percSviluppo
            ProgressBar1.Update()

        End If
        writeLog("Lancia ControllaDatiTextBox a linea289 " + Environment.NewLine)
        ControllaDatiTextBox()
        blocco = (FineRecords - InizioRecord) + 1
        Label6.Text = blocco.ToString
        writeLog(" linea 289 - blocco = " + blocco.ToString + Environment.NewLine)
        writeLog("Linea 295-313  For i As Integer = InizioRecord To FineRecords - 1 " + Environment.NewLine)

        For i As Integer = InizioRecord To FineRecords - 1
            If FineRecords > DataGridView1.Rows.Count - 1 Then
                writeLog(" linea 296 - FineRecords > " + DataGridView1.Rows.Count - 1)
                writeLog("FineRecords =  " + FineRecords)
                writeLog(" linea 299 - Exit Sub " + Environment.NewLine)
                Exit Sub
            End If
            For x As Integer = 1 To 90
                Ritardo(x) += 1
            Next
            For j As Integer = 3 To 8
                dato = CInt(DataGridView1.Rows(i).Cells(j).Value)
                Uscite(dato) += 1
                If RitardoMax(dato) < Ritardo(dato) Then
                    DebugLog.AppendText("Riga " + i.ToString + " Ra = " + Ritardo(dato).ToString + " Rm = " + RitardoMax(dato).ToString + Environment.NewLine)
                    RitardoMax(dato) = Ritardo(dato)
                End If
                Ritardo(dato) = 0
            Next
        Next

        If InizioRecord <= FineRecords Then
            writeLog(" linea 315 - 331 If InizioRecord <= FineRecords :   ' compiliamo DataGridStat " + Environment.NewLine)

            ' compiliamo DataGridStat
            Dim calcolato As Double = 0
            Dim estratto As Integer
            For g As Integer = 0 To 89
                DataGridStat.Rows.Add()
                estratto = g + 1
                DataGridStat.Rows(g).Cells(0).Value = estratto
                DataGridStat.Rows(g).Cells(1).Value = Uscite(estratto)
                DataGridStat.Rows(g).Cells(2).Value = Ritardo(estratto)
                DataGridStat.Rows(g).Cells(3).Value = RitardoMax(estratto)
                calcolato = Ritardo(estratto) * 100 / RitardoMax(estratto)
                DataGridStat.Rows(g).Cells(4).Value = calcolato
            Next
        End If
        DataGridStat.Update()
        ProgressBar1.Update()

        ProssimoInizioRecord = FineRecords + 1
        ProssimoFineRecord = (ProssimoInizioRecord + blocco) - 1
        RecordSuccessivoAlFineBlocco = ProssimoFineRecord + 1
        writeLog("linee 335-337 =  " + Environment.NewLine)
        writeLog(" ProssimoInizioRecord = " + ProssimoInizioRecord.ToString + Environment.NewLine)
        writeLog(" ProssimoFineRecord = " + ProssimoFineRecord.ToString + Environment.NewLine)
        writeLog(" RecordSuccessivoAlFineBlocco = " + RecordSuccessivoAlFineBlocco.ToString + Environment.NewLine)


UscitaSub:
        If SegnalazionePerInterruzioneLavoro = True Then
            writeLog(" linea 345-349 : If SegnalazionePerInterruzioneLavoro = True " + Environment.NewLine)
            writeLog("  SegnalazionePerInterruzioneLavoro =  " + SegnalazionePerInterruzioneLavoro.ToString + Environment.NewLine)
            writeLog(" linea 350 - se true esegue  Statistica() " + Environment.NewLine)

            Statistica()
        End If
    End Sub
    Public Shared Sub ControllaDatiTextBox()
        Dim valore As Integer = 0
        Dim valore1 As Integer = 0
        Dim valore2 As Integer = 0
        Dim verifica1 As Integer = 0
        Dim verifica2 As Integer = 0
        Form1.writeLog(" For k As Integer = 1 To 90
            Form1.Uscite(k) = 0
        Next  - a linea 349 " + Environment.NewLine)

        For k As Integer = 1 To 90
            Form1.Uscite(k) = 0
        Next
        Dim inputString1 As String = Form1.TextBox1.Text
        Dim inputString2 As String = Form1.TextBox2.Text
        Dim result1, result2 As Double
        Dim isNumeric1, isNumeric2 As Boolean
        isNumeric1 = Double.TryParse(inputString1, result1)
        isNumeric2 = Double.TryParse(inputString2, result2)
        Form1.writeLog("inizio e fine records = " + inputString1 + " - " + inputString2)
        If isNumeric1 And isNumeric2 Then
            ' MessageBox.Show("Entrambi i campi contengono solo valori numerici validi.")
            Dim numrecTextbox2 As Integer = CDbl(Form1.TextBox2.Text)

            If numrecTextbox2 >= Form1.DataGridView1.Rows.Count - 1 Then
                Form1.writeLog("Se TextBox2 >= Form1.DataGridView1.Rows.Count - 1  a linea 363 " + Environment.NewLine)
                Form1.writeLog(" TextBox2 = " + CDbl(Form1.TextBox2.Text).ToString + " e " + " Form1.DataGridView1.Rows.Count - 1  = " + (Form1.DataGridView1.Rows.Count - 1).ToString + Environment.NewLine)

                Form1.SegnalazionePerInterruzioneLavoro = True
                Form1.writeLog(" Segnalazione errore = " + Form1.SegnalazionePerInterruzioneLavoro.ToString + " a linea 367 e dopo -> Exit Sub" + Environment.NewLine)
                Exit Sub
            End If
        ElseIf Not isNumeric1 And Not isNumeric2 Then
            Form1.writeLog(" Entrambi i campi contengono valori non numerici. " + Environment.NewLine)
            MessageBox.Show("Entrambi i campi contengono valori non numerici.")
        ElseIf Not isNumeric1 Then
            Form1.writeLog(" Il campo TextBox1 contiene valori non numerici. " + Environment.NewLine)
            MessageBox.Show("Il campo TextBox1 contiene valori non numerici.")

        ElseIf Not isNumeric2 Then
            Form1.writeLog(" Il campo TextBox2 contiene valori non numerici. " + Environment.NewLine)
            MessageBox.Show("Il campo TextBox2 contiene valori non numerici.")
        End If
        If Form1.counterRipetizioniBlocchi = 0 Then
            Form1.writeLog(" GoTo salta_la_prima_volta " + Environment.NewLine)
            GoTo salta_la_prima_volta
        End If
        If Form1.ProssimoInizioRecord > 0 And Form1.ProssimoFineRecord > 0 Then
            Form1.TextBox1.Text = Form1.ProssimoInizioRecord
            Form1.TextBox2.Text = Form1.ProssimoFineRecord
            Form1.TextBox1.Update()
            Form1.TextBox2.Update()
            Form1.writeLog(" ProssimoInizioRecord e ProssimoFineRecord = " + Form1.ProssimoInizioRecord.ToString + " - " + Form1.ProssimoFineRecord.ToString + Environment.NewLine)
            If Form1.SegnalazionePerInterruzioneLavoro = True Then
                Form1.writeLog(" SegnalazionePerInterruzioneLavoro = True  " + Environment.NewLine)
                Exit Sub
            End If
        Else
            Form1.writeLog("Entrambi i Textbox non contengono valori maggiori di zero : procedura ControllaDatiTextBox()" + Environment.NewLine)
            MessageBox.Show("Entrambi i Textbox non contengono valori maggiori di zero : procedura ControllaDatiTextBox()")

        End If

salta_la_prima_volta:
        Form1.writeLog("Dopo l'etichetta salta_la_prima_volta: quando CDbl(Form1.TextBox2.Text) = " + CDbl(Form1.TextBox2.Text).ToString + " " + Environment.NewLine)
        Form1.writeLog("Contatore ripetizioni blocchi = " + Form1.counterRipetizioniBlocchi.ToString)
    End Sub
    Public Sub LeggiTextBox()
        Dim isNumeric1 As Boolean = IsNumeric(TextBox1.Text)
        Dim isNumeric2 As Boolean = IsNumeric(TextBox2.Text)
        If isNumeric1 And isNumeric2 Then
            ' MessageBox.Show("Entrambi i campi contengono solo valori numerici.")
        ElseIf Not isNumeric1 And Not isNumeric2 Then
            MessageBox.Show("Entrambi i campi contengono valori non numerici: Ora correggo")
            TextBox1.Text = 1
            TextBox2.Text = DataGridView1.Rows.Count - 1

        ElseIf Not isNumeric1 Then
            MessageBox.Show("Il campo TextBox1 contiene valori non numerici.Ora correggo con valori sicuri")
            TextBox1.Text = 1
            TextBox2.Text = DataGridView1.Rows.Count - 1
        ElseIf Not isNumeric2 Then
            MessageBox.Show("Il campo TextBox2 contiene valori non numerici.Ora correggo con valori sicuri")
            TextBox1.Text = 1
            TextBox2.Text = DataGridView1.Rows.Count - 1
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Clear risultanze
        TextBox1.Text = "" : TextBox2.Text = "" : Label4.Text = "00" : Label6.Text = "00"
        DataGridStat.Rows.Clear()

    End Sub
    Private Sub writeLog(testo As String)
        DebugLog.AppendText(testo)
    End Sub
    Public Function ShowInputBox(prompt As String) As String
        Dim inputForm As New InputBoxForm(prompt)
        If inputForm.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Return inputForm.InputText
        Else
            Return String.Empty
        End If
    End Function

    '****************************SALVARE E CARICARE CON UN NOME SPECIFICO ************************
    Private Sub SaveFormState(stateName As String)
        Dim state As New FormState With {
        .Width = Me.Width,
        .Height = Me.Height,
        .LocationX = Me.Location.X,
        .LocationY = Me.Location.Y,
        .Controls = New List(Of ControlState)()
    }

        For Each ctrl As System.Windows.Forms.Control In Me.Controls
            Dim ctrlState As New ControlState With {
            .Name = ctrl.Name,
            .Value = GetControlValue(ctrl)
        }
            state.Controls.Add(ctrlState)
        Next

        Using fs As New System.IO.FileStream(stateName & ".xml", System.IO.FileMode.Create)
            Dim serializer As New System.Xml.Serialization.XmlSerializer(GetType(FormState))
            serializer.Serialize(fs, state)
        End Using
    End Sub



    Private Function GetControlValue(ctrl As System.Windows.Forms.Control) As Object
        If TypeOf ctrl Is System.Windows.Forms.TextBox Then
            Return DirectCast(ctrl, System.Windows.Forms.TextBox).Text
        ElseIf TypeOf ctrl Is System.Windows.Forms.CheckBox Then
            Return DirectCast(ctrl, System.Windows.Forms.CheckBox).Checked
        ElseIf TypeOf ctrl Is System.Windows.Forms.RadioButton Then
            Return DirectCast(ctrl, System.Windows.Forms.RadioButton).Checked
            ' Aggiungi qui altri tipi di controllo se necessario
        Else
            Return Nothing
        End If
    End Function
    Private Sub RestoreFormState(stateName As String)
        If System.IO.File.Exists(stateName & ".xml") Then
            Using fs As New System.IO.FileStream(stateName & ".xml", System.IO.FileMode.Open)
                Dim serializer As New System.Xml.Serialization.XmlSerializer(GetType(FormState))
                Dim state As FormState = DirectCast(serializer.Deserialize(fs), FormState)

                Me.Width = state.Width
                Me.Height = state.Height
                Me.Location = New System.Drawing.Point(state.LocationX, state.LocationY)

                For Each ctrlState As ControlState In state.Controls
                    Dim ctrl As System.Windows.Forms.Control = Me.Controls.Find(ctrlState.Name, True).FirstOrDefault()
                    If ctrl IsNot Nothing Then
                        SetControlValue(ctrl, ctrlState.Value)
                    End If
                Next
            End Using
        End If
    End Sub

    Private Sub SetControlValue(ctrl As System.Windows.Forms.Control, value As Object)
        If TypeOf ctrl Is System.Windows.Forms.TextBox Then
            DirectCast(ctrl, System.Windows.Forms.TextBox).Text = value.ToString()
        ElseIf TypeOf ctrl Is System.Windows.Forms.CheckBox Then
            DirectCast(ctrl, System.Windows.Forms.CheckBox).Checked = Convert.ToBoolean(value)
        ElseIf TypeOf ctrl Is System.Windows.Forms.RadioButton Then
            DirectCast(ctrl, System.Windows.Forms.RadioButton).Checked = Convert.ToBoolean(value)
            ' Aggiungi qui altri tipi di controllo se necessario
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim stateName As String = InputBox("Inserisci il nome per lo stato da salvare:")
        SaveFormState(stateName)
    End Sub

    Private Sub UpdateBlockState()
        ' Aggiorna i contatori e lo stato per il prossimo blocco
        counterRipetizioniBlocchi += 1

        ' Calcola la percentuale di sviluppo
        percSviluppo = (counterRipetizioniBlocchi * 100) / (NumeroRecDataGridView1 / blocco)
        If percSviluppo > 0 AndAlso percSviluppo <= 100 Then
            ProgressBar1.Value = percSviluppo
            ProgressBar1.Update()
        End If

        ' Aggiorna gli indici per il prossimo blocco
        InizioRecord = ProssimoInizioRecord
        FineRecords = ProssimoFineRecord

        ' Calcola i nuovi indici per il prossimo ciclo
        ProssimoInizioRecord = FineRecords + 1
        ProssimoFineRecord = (ProssimoInizioRecord + blocco) - 1

        ' Aggiorna l'eccedenza del blocco
        EccedenzaBlocco = NumeroRecDataGridView1 - ProssimoFineRecord

        ' Aggiorna i TextBox con i nuovi valori
        TextBox1.Text = InizioRecord.ToString()
        TextBox2.Text = FineRecords.ToString()
        TextBox1.Update()
        TextBox2.Update()
    End Sub

    Private Sub CalculateStatistics()
        ' aggiunta variabili in data 27-2-2025-ore 1 :44 circa
        Dim flagUscite_Ritardi(90) As Boolean

        'fine aggiunta

        ' Resetta gli array - partiamo da indice 1
        ReDim Uscite(90)    ' 1-90 per numeri 1-90
        ReDim Ritardo(90)
        ReDim RitardoMax(90)
        ReDim flagUscite_Ritardi(90)

        ' Calcola il numero totale di record
        Dim totalRecords As Integer = DataGridView1.Rows.Count - 1

        ' Calcola il numero di blocchi
        Dim numeroBlocchi As Integer = Math.Ceiling(CDbl(totalRecords) / blocco)

        ' Per ogni blocco progressivo
        For bloccoCorrente As Integer = 1 To numeroBlocchi
            ' Reset degli array per ogni blocco
            For x As Integer = 1 To 90
                Ritardo(x) = 0
                RitardoMax(x) = 0
                Uscite(x) = 0
                flagUscite_Ritardi(x) = False
            Next

            ' Calcola l'indice finale del blocco corrente
            Dim endIndex As Integer = Math.Min(bloccoCorrente * blocco, totalRecords)

            ' CORREZIONE CRUCIALE: Inizializza i ritardi prima del ciclo
            For j As Integer = 1 To 90
                Ritardo(j) = 0
                RitardoMax(j) = 0
            Next

            ' Analizza le estrazioni da 1 fino all'indice finale
            For i As Integer = 1 To endIndex
                If i <= totalRecords AndAlso Not DataGridView1.Rows(i).IsNewRow Then

                    ' Invertito il seguente processo -->' Poi processa i numeri estratti
                    'Prima processa i numeri estratti
                    For b As Integer = 1 To 90
                        If flagUscite_Ritardi(b) = False Then ' numeri il cui ritardo non è stato azzerato
                            Ritardo(b) += 1
                        End If
                    Next
                    For j As Integer = 3 To 8  ' Colonne E1-E6
                        Dim cellValue = DataGridView1.Rows(i).Cells(j).Value
                        If cellValue IsNot Nothing Then
                            Dim num As Integer
                            If Integer.TryParse(cellValue.ToString(), num) AndAlso num >= 1 AndAlso num <= 90 Then
                                Uscite(num) += 1   ' Incrementa frequenza
                                If Ritardo(num) > RitardoMax(num) Then
                                    RitardoMax(num) = Ritardo(num)
                                End If
                                Ritardo(num) = 0    ' Resetta ritardo
                                flagUscite_Ritardi(num) = True ' indica che il ritardo di questi numeri è stato azzerato
                            End If
                        End If
                    Next
                End If

            Next

            ' Aggiorna il numero del blocco e il messaggio
            NumBlocco = bloccoCorrente.ToString()
            Messaggio = "Blocco " + NumBlocco + " Records 1-" + endIndex.ToString()
            ' prima di aggiornare le statistiche

            ' Aggiorna le statistiche e genera la previsione
            UpdateStatistics()
            Dim prediction = GeneratePrediction()
            ShowPredictionDialog(prediction)
            UpdateStatisticsGrid(prediction)

            ' Se abbiamo raggiunto la fine dei record, usciamo
            If endIndex >= totalRecords Then
                Exit For
            End If
        Next
    End Sub
    Private Sub UpdateStatistics()
        ' Pulisci la griglia
        DataGridStat.Rows.Clear()

        ' Prepara il testo per la cronologia
        Dim statText As New StringBuilder()
        statText.AppendLine($"=== {Messaggio} ===")
        statText.AppendLine($"Data: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}")
        statText.AppendLine("ID  | Freq | Rit | RMax | Rapp%")
        statText.AppendLine("--------------------------------")

        ' Aggiorna la griglia e la cronologia per ogni numero
        For num As Integer = 1 To 90
            Dim rowIndex As Integer = DataGridStat.Rows.Add()
            With DataGridStat.Rows(rowIndex)
                .Cells(0).Value = num                         ' Id
                .Cells(1).Value = Uscite(num)                ' Usc (frequenza)
                .Cells(2).Value = Ritardo(num)               ' Ra (ritardo attuale)
                .Cells(3).Value = RitardoMax(num)            ' Rm (ritardo massimo)

                Dim rapporto As String = "0.00"
                If RitardoMax(num) > 0 Then
                    rapporto = (CDbl(Ritardo(num)) / RitardoMax(num)).ToString("F2")
                    .Cells(4).Value = rapporto ' Rapp Perc
                Else
                    .Cells(4).Value = rapporto
                End If

                ' Aggiungi tutti i numeri alla cronologia
                statText.AppendLine($"{num,2} | {Uscite(num),4} | {Ritardo(num),3} | {RitardoMax(num),4} | {rapporto}")
            End With
        Next

        statText.AppendLine("--------------------------------")
        statText.AppendLine()

        ' Aggiungi al RichTextBox CronoStat se esiste
        If CronoStat IsNot Nothing Then
            CronoStat.AppendText(statText.ToString())

            ' Scorri alla fine del RichTextBox
            CronoStat.SelectionStart = CronoStat.Text.Length
            CronoStat.ScrollToCaret()
        End If
    End Sub
    Private Function CalculateTrendScore(number As Integer) As Double
        ' Calcola un punteggio basato sul trend delle ultime estrazioni
        Dim trendScore As Double = 0
        Dim recentCount As Integer = 0

        ' Consideriamo le ultime 10 estrazioni
        For i As Integer = FineRecords - 10 To FineRecords
            If i >= InizioRecord Then
                For j As Integer = 3 To 8
                    If CInt(DataGridView1.Rows(i).Cells(j).Value) = number Then
                        ' Diamo più peso alle estrazioni più recenti
                        recentCount += 1 + (i - (FineRecords - 10)) / 10
                    End If
                Next
            End If
        Next

        ' Normalizza il punteggio
        trendScore = recentCount / 10
        Return trendScore
    End Function


    ' Modifica della classe InputBoxForm per aggiungere la proprietà InputText

    Private Sub OkButton_Click(sender As Object, e As EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub SalvaLArchivioToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SalvaLArchivioToolStripMenuItem.Click
        ' SALVATAGGIO ARCHIVIO
        Dim cnt As Integer
        Dim i As Integer
        Dim j As Integer

        cnt = DataGridView1.RowCount - 1

        If cnt > 0 Then
            Dim ilfile As String = "C:/BkArchivio/Archivio.txt"
            Dim fi As New FileInfo(ilfile)

            Using scrivi As New StreamWriter(ilfile)
                scrivi.WriteLine(cnt)
                For i = 0 To cnt - 1

                    For j = 1 To 10
                        scrivi.WriteLine(DataGridView1.Rows(i).Cells(j).Value)
                    Next
                Next
            End Using

            MessageBox.Show("Records salvati = " & cnt)

        Else
            MessageBox.Show("Archivio Vuoto o non trovato")
        End If
    End Sub

    Private Sub btnImportaTesto_Click(sender As Object, e As EventArgs) Handles btnImportaTesto.Click
        ' importa testo per aggiornamento automatico dell'Archivio
        ImportaEstrazioni()
    End Sub
    Private Sub ImportaEstrazioni()
        ' Inizializza OpenFileDialog
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "File di testo|*.txt"
        openFileDialog.Title = "Seleziona file estrazioni"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            ' Leggi il file
            Dim linee As String() = File.ReadAllLines(openFileDialog.FileName)

            ' Lista per raccogliere le estrazioni
            Dim nuoveEstrazioni As New List(Of EstrazioneDati)()

            ' Variabili per il parsing
            Dim dataCorrente As DateTime = Nothing
            Dim numeriCorrente As New List(Of Integer)()

            ' Elabora le linee
            For i As Integer = 0 To linee.Length - 1
                Dim linea As String = linee(i).Trim()

                ' Salta le linee vuote
                If String.IsNullOrEmpty(linea) Then Continue For

                ' Controlla se è una riga di data (contiene sia numeri che caratteri alfabetici)
                If System.Text.RegularExpressions.Regex.IsMatch(linea, "\d+\s+[a-zA-Z]+\s+\d{4}") Then
                    ' Se abbiamo già una data e dei numeri, salviamo l'estrazione precedente
                    If dataCorrente <> Nothing AndAlso numeriCorrente.Count >= 8 Then
                        nuoveEstrazioni.Add(New EstrazioneDati With {
                        .Data = dataCorrente,
                        .Numeri = numeriCorrente.ToArray()
                    })
                    End If

                    ' Parse della nuova data
                    dataCorrente = ParseData(linea)
                    numeriCorrente = New List(Of Integer)()
                Else
                    ' Prova a fare il parsing dei numeri
                    Dim numeri As List(Of Integer) = ParseNumeri(linea)
                    If numeri.Count > 0 Then
                        numeriCorrente.AddRange(numeri)
                    End If
                End If
            Next

            ' Aggiungi l'ultima estrazione se presente
            If dataCorrente <> Nothing AndAlso numeriCorrente.Count >= 8 Then
                nuoveEstrazioni.Add(New EstrazioneDati With {
                .Data = dataCorrente,
                .Numeri = numeriCorrente.ToArray()
            })
            End If

            ' Inverti l'ordine (da decrescente a crescente)
            nuoveEstrazioni.Reverse()

            ' Aggiungi al DataGridView
            AggiungiEstrazioniAlGrid(nuoveEstrazioni)
        End If
    End Sub
    Private Function ParseData(lineaData As String) As DateTime
        Try
            ' Esempio: "25 febbraio 2025"
            Dim parti As String() = lineaData.Split(" "c)
            Dim giorno As Integer = Integer.Parse(parti(0))
            Dim mese As Integer = GetMeseNumero(parti(1))
            Dim anno As Integer = Integer.Parse(parti(2))

            Return New DateTime(anno, mese, giorno)
        Catch ex As Exception
            MessageBox.Show($"Errore nel parsing della data: {lineaData}" & vbCrLf & ex.Message)
            Return Nothing
        End Try
    End Function

    Private Function GetMeseNumero(nomeMese As String) As Integer
        ' Mappa completa per tutti i mesi dell'anno
        Dim mesiMap As New Dictionary(Of String, Integer) From {
            {"gennaio", 1}, {"febbraio", 2}, {"marzo", 3}, {"aprile", 4},
            {"maggio", 5}, {"giugno", 6}, {"luglio", 7}, {"agosto", 8},
            {"settembre", 9}, {"ottobre", 10}, {"novembre", 11}, {"dicembre", 12}
        }

        Dim meseLower As String = nomeMese.ToLower()
        If mesiMap.ContainsKey(meseLower) Then
            Return mesiMap(meseLower)
        Else
            Throw New Exception($"Mese non riconosciuto: {nomeMese}")
        End If
    End Function

    Private Function ParseNumeri(lineaNumeri As String) As List(Of Integer)
        Dim numeri As New List(Of Integer)()

        ' Divide la linea in possibili numeri usando spazi come separatori
        Dim parti As String() = lineaNumeri.Split(New Char() {" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)

        For Each parte As String In parti
            Dim numero As Integer
            If Integer.TryParse(parte, numero) Then
                numeri.Add(numero)
            End If
        Next

        Return numeri
    End Function

    Private Sub AggiungiEstrazioniAlGrid(estrazioni As List(Of EstrazioneDati))
        ' Trova l'ultimo ID, concorso e data nel DataGridView
        Dim ultimoID As Integer = 0
        Dim ultimoConcorso As Integer = 0
        Dim ultimaData As DateTime = DateTime.MinValue

        If DataGridView1.Rows.Count > 1 Then  ' Assicurati che ci sia almeno una riga (esclusa quella vuota finale)
            Dim ultimaRiga As Integer = DataGridView1.Rows.Count - 2  ' -2 per escludere la riga vuota finale
            ultimoID = CInt(DataGridView1.Rows(ultimaRiga).Cells(0).Value)
            ultimoConcorso = CInt(DataGridView1.Rows(ultimaRiga).Cells(2).Value)  ' Colonna 2 è Conc
            ultimaData = CDate(DataGridView1.Rows(ultimaRiga).Cells(1).Value)
        End If

        ' Filtro le estrazioni per includere solo quelle con data maggiore dell'ultima data
        ' Versione senza LINQ
        Dim nuoveEstrazioniValide As New List(Of EstrazioneDati)
        For Each e In estrazioni
            If e.Data > ultimaData Then
                nuoveEstrazioniValide.Add(e)
            End If
        Next

        ' Messaggio se non ci sono nuove estrazioni
        If nuoveEstrazioniValide.Count = 0 Then
            MessageBox.Show("Non ci sono nuove estrazioni da importare.")
            Return
        End If

        ' Aggiungi le nuove estrazioni
        For Each estrazione In nuoveEstrazioniValide
            ultimoID += 1
            ultimoConcorso += 1
            Dim numeri As Integer() = estrazione.Numeri

            DataGridView1.Rows.Add(
                ultimoID,                              ' ID
                estrazione.Data,                       ' Data
                ultimoConcorso,                        ' Concorso
                numeri(0),                             ' E1
                numeri(1),                             ' E2
                numeri(2),                             ' E3
                numeri(3),                             ' E4
                numeri(4),                             ' E5
                numeri(5),                             ' E6
                numeri(6),                             ' Jolly
                numeri(7)                              ' SuperStar
            )
        Next

        ' Ordina il DataGridView per data crescente
        DataGridView1.Sort(DataGridView1.Columns(1), System.ComponentModel.ListSortDirection.Ascending)

        MessageBox.Show($"Importate {nuoveEstrazioniValide.Count} nuove estrazioni.")
    End Sub

End Class
Public Class InputBoxForm
    Inherits System.Windows.Forms.Form

    Private label As New System.Windows.Forms.Label()
    Private textBox As New System.Windows.Forms.TextBox()
    Private buttonOk As New System.Windows.Forms.Button()
    Private buttonCancel As New System.Windows.Forms.Button()

    Public ReadOnly Property InputText As String
        Get
            Return textBox.Text
        End Get
    End Property

    Public Sub New(prompt As String)
        Me.Text = "Input"
        Me.Size = New System.Drawing.Size(300, 150)

        label.Text = prompt
        label.Location = New System.Drawing.Point(10, 10)
        label.Size = New System.Drawing.Size(260, 20)
        Me.Controls.Add(label)

        textBox.Location = New System.Drawing.Point(10, 40)
        textBox.Size = New System.Drawing.Size(260, 20)
        Me.Controls.Add(textBox)

        buttonOk.Text = "OK"
        buttonOk.Location = New System.Drawing.Point(10, 70)
        buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK
        AddHandler buttonOk.Click, AddressOf Me.OkButton_Click
        Me.Controls.Add(buttonOk)

        buttonCancel.Text = "Cancel"
        buttonCancel.Location = New System.Drawing.Point(100, 70)
        buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Controls.Add(buttonCancel)
    End Sub

    Private Sub OkButton_Click(sender As Object, e As EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub
End Class
' Classe per i risultati della previsione
Public Class PredictionResult
    Public Property PredictedNumbers As Integer()
    Public Property ActualNumbers As Integer()
    Public Property Accuracy As Double
End Class
' Classe per i pesi della previsione
Public Class PredictionWeights
    Public Property FrequencyWeight As Double = 0.6
    Public Property DelayWeight As Double = 0.4
End Class