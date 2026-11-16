' ============================================================
' FOC-C10 (Prog0347-FOCC10\frmParametri.frm): stesso form di FOC6 con
' queste differenze, applicate qui: nel file parametri i campi SPCC/SPCW,
' AOS/EOS, wind/unstow stow e i settori SA hanno una cifra in meno
' (ParFile) e i settori SA/SL si scrivono con 2 decimali (ParSalva);
' a fine invio file l'esito va in LblFine (non MsgBox) e b_Ricezione
' torna False; timeout ACU indicizzato per canale (Port_Control(ACU)).
' ------------------------------------------------------------
' FrmParametri.vb — portato da BTS1\FrmParametri.vb (stessa origine
' VB6, funzionalità confermata identica per il nucleo di editing
' parametri ACU: ValoreParametri/ParSalva/ParFile/SalvaData/
' Presenta_Data e tutti i comandi CmdXX_Click).
' Tabelle di calibrazione per settore, specifiche di FOC6 (assenti
' in BTS1): offset correttivi che l'ACU applica in funzione del
' settore di puntamento sulle tre coordinate — TxtSA00-31 (comando
' AO, Azimuth), TxtSL00-07 (comando EO, Elevazione), TxtSP00-07/24-31
' (comando PO, Polarizzazione), più TxtMofa/TxtMofe (offset tensione
' motore Az/El), TxtPOS/TxtPli/TxtSynPol/TxtAltri. Nessuno di questi
' campi ha un handler _KeyUp nel VB6 originale (nessuna validazione
' dal vivo); le tabelle AO/EO/PO non sono persistite su database nel
' VB6 originale (solo nel file .PAR), gli altri 4 campi singoli sì
' (PLI/pos/POM/ALTRI).
' ============================================================

Public Class FrmParametri

    ' ── Module-level fields (from VB6 Dim at form level) ──────────────────────
    Private tmpSQL           As String
    Private LastBackupped_C  As Long
    Private m_Reader         As System.IO.StreamReader   ' replaces VB6 FileNum_C
    Private i_lun            As Integer
    Private Port             As Integer
    Private SEND1            As String
    Private sSegno           As String

    ' ── ValoreParametri ───────────────────────────────────────────────────────
    Public Function ValoreParametri(Messaggio As String) As Boolean
        Dim pos   As Integer
        Dim Campo As String
        Try

        ValoreParametri = True
        Campo = Mid$(Messaggio, 6, 1)

        If Campo = "?" Then
            ClearAllTxt(Me)
            ' K09: tracciato del messaggio parametri della versione comune
            ' (ComuniIntelsatImmersatnew\frmParametri.frm), diverso da C10/FOC6
            ' nella prima parte (fino alla posizione 72); da AOS (73) in poi è
            ' uguale. Niente SS/STEP in K09.
            pos = 9
            TxtIT.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtAT.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtET.Text = Mid(Messaggio, pos, 2)
            pos = 17
            TxtXT.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtYT.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtRT.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtCY.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtBM.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtATTS.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtATTA.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtATTE.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtAli.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtEli.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtPli.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtSynAz.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtSynEl.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtSynPol.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtMR.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtPT.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtFST.Text = Mid(Messaggio, pos, 1)
            pos = pos + 1
            TxtMT.Text = Mid(Messaggio, pos, 2)
            pos = pos + 2
            TxtSPCC.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSPCW.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSPDW.Text = Mid(Messaggio, pos, 2) & "." & Mid(Messaggio, pos + 2, 3)
            pos = pos + 5
            TxtSPUP.Text = Mid(Messaggio, pos, 2) & "." & Mid(Messaggio, pos + 2, 3)
            pos = pos + 5
            TxtSPPCC.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSPPCW.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)

            ' Tabelle di calibrazione per settore (AO/EO/PO) e campi singoli —
            ' porting diretto dal VB6 di FOC6 (Prog0357-FOC6\frmParametri.frm,
            ' stesso ramo "Campo=?"): offset correttivi che l'ACU applica in
            ' funzione del settore di puntamento su Azimuth/Elevazione/
            ' Polarizzazione. Assenti in BTS1. "pos" viene reimpostato in modo
            ' assoluto (73/325/425/458) esattamente come nel VB6 originale,
            ' quindi l'inserimento è indipendente da dove si trovasse "pos"
            ' subito prima.
            pos = 73
            TxtAOS.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA00.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA01.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA02.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA03.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA04.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA05.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA06.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA07.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA08.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA09.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA10.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA11.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA12.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA13.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA14.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA15.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA16.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA17.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA18.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA19.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA20.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA21.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA22.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA23.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA24.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA25.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA26.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA27.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA28.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA29.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA30.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSA31.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtEOS.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL00.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL01.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL02.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL03.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL04.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL05.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL06.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = pos + 6
            TxtSL07.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 3)
            pos = 325
            TxtPOS.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP00.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP01.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP02.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP03.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP04.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP05.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP06.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP07.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = 425
            TxtSP24.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP25.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP26.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP27.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP28.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP29.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP30.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = pos + 4
            TxtSP31.Text = Mid(Messaggio, pos, 3) & "." & Mid(Messaggio, pos + 3, 1)
            pos = 458
            TxtMofa.Text = Mid(Messaggio, pos, 3)
            pos = pos + 3
            TxtMofe.Text = Mid(Messaggio, pos, 3)

            frmRicezione.tmrTimeoutMSG(ACU).Enabled = False
            SEND1 = CheckSum("07#")
            s_Parametri = ""
            b_int(ACU) = True
            i_Cont = 2
            Attesa(0.2)
            b_Ricezione(ACU) = True
            InvioACU(SEND1)
            frmRicezione.tmrTimeoutMSG(ACU).Enabled = True

        ElseIf Campo = "#" Then
            Cursor.Current = Cursors.Default
            i_Inter = LeggiFileIni("Canale1", "Inter")
            TxtWSAZ.Text = Mid(Messaggio, 7, 3)  & "." & Mid(Messaggio, 10, 3)
            TxtWSEL.Text = Mid(Messaggio, 13, 2) & "." & Mid(Messaggio, 15, 3)
            TxtUSAZ.Text = Mid(Messaggio, 18, 3) & "." & Mid(Messaggio, 21, 3)
            TxtUSEL.Text = Mid(Messaggio, 24, 2) & "." & Mid(Messaggio, 26, 3)
            b_Parametri = False
            i_Cont = 0
        End If
        Catch ex As Exception
            WriteError(0, "frm Parametri - ValoreParametri " & ex.Message)
        End Try
    End Function

    ' ── ParSalva ──────────────────────────────────────────────────────────────
    Private Function ParSalva() As Boolean
        Dim Buffer As String = ""
        ParSalva = True

        If TxtSTEP.Text <> "" Then Buffer = Buffer & "STEP " & Format(Val(TxtSTEP.Text), "00") & vbCrLf
        If TxtIT.Text   <> "" Then Buffer = Buffer & "IT "   & Format(Val(TxtIT.Text),   "00") & vbCrLf
        If TxtAT.Text   <> "" Then Buffer = Buffer & "AT "   & Format(Val(TxtAT.Text),   "00") & vbCrLf
        If TxtET.Text   <> "" Then Buffer = Buffer & "ET "   & Format(Val(TxtET.Text),   "00") & vbCrLf
        If TxtSS.Text   <> "" Then Buffer = Buffer & "SS "   & Format(Val(TxtSS.Text),   "00") & vbCrLf
        If TxtXT.Text   <> "" Then Buffer = Buffer & "XT "   & Format(Val(TxtXT.Text),   "00") & vbCrLf
        If TxtYT.Text   <> "" Then Buffer = Buffer & "YT "   & Format(Val(TxtYT.Text),   "00") & vbCrLf
        If TxtRT.Text   <> "" Then Buffer = Buffer & "RT "   & Format(Val(TxtRT.Text),   "00") & vbCrLf
        If TxtBM.Text   <> "" Then Buffer = Buffer & "BM "   & TxtBM.Text  & vbCrLf
        If TxtATTS.Text <> "" Then Buffer = Buffer & "ATTS " & Format(Val(TxtATTS.Text), "00") & vbCrLf
        If TxtATTA.Text <> "" Then Buffer = Buffer & "ATTA " & Format(Val(TxtATTA.Text), "00") & vbCrLf
        If TxtATTE.Text <> "" Then Buffer = Buffer & "ATTE " & Format(Val(TxtATTE.Text), "00") & vbCrLf
        If TxtAli.Text  <> "" Then Buffer = Buffer & "ALI "  & Format(Val(TxtAli.Text),  "0")  & vbCrLf
        If TxtEli.Text  <> "" Then Buffer = Buffer & "ELI "  & Format(Val(TxtEli.Text),  "0")  & vbCrLf
        If TxtMR.Text   <> "" Then Buffer = Buffer & "MR"    & TxtMR.Text  & vbCrLf
        If TxtFST.Text  <> "" Then Buffer = Buffer & "FST"   & TxtFST.Text & vbCrLf
        If TxtMT.Text   <> "" Then Buffer = Buffer & "MT "   & TxtMT.Text  & vbCrLf
        If TxtSPCC.Text <> "" Then Buffer = Buffer & "SPCC " & TxtSPCC.Text & vbCrLf
        If TxtSPCW.Text <> "" Then Buffer = Buffer & "SPCW " & TxtSPCW.Text & vbCrLf
        If TxtSPDW.Text <> "" Then Buffer = Buffer & "SPDW " & TxtSPDW.Text & vbCrLf
        If TxtSPUP.Text <> "" Then Buffer = Buffer & "SPUP " & TxtSPUP.Text & vbCrLf
        If TxtAOS.Text  <> "" Then Buffer = Buffer & "AOS "  & TxtAOS.Text  & vbCrLf
        If TxtEOS.Text  <> "" Then Buffer = Buffer & "EOS "  & TxtEOS.Text  & vbCrLf
        If TxtWSAZ.Text <> "" And TxtWSEL.Text <> "" Then
            Buffer = Buffer & "WS " & TxtWSAZ.Text & " " & LTrim(TxtWSEL.Text) & vbCrLf
        End If
        If TxtUSAZ.Text <> "" And TxtUSEL.Text <> "" Then
            Buffer = Buffer & "US " & TxtUSAZ.Text & " " & LTrim(TxtUSEL.Text) & vbCrLf
        End If
        ' K09: posizione di back-up (VB6 comune: "BP az el")
        If TxtBKAZ.Text <> "" And TxtBKEL.Text <> "" Then
            Buffer = Buffer & "BP " & TxtBKAZ.Text & " " & LTrim(TxtBKEL.Text) & vbCrLf
        End If

        ' Tabelle di calibrazione per settore (AO/EO/PO) e campi singoli — scrittura nel Buffer
        If IsNumeric(TxtSA00.Text) Then
            If Val(TxtSA00.Text) < 0 Then
                Buffer = Buffer & "AO 00 M" & Format(Math.Abs(Val(TxtSA00.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 00 P" & Format(Val(TxtSA00.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA00.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA00.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA00.Text, 1) = "P") Then
            Buffer = Buffer & "AO 00 " & TxtSA00.Text & vbCrLf
        End If
        If IsNumeric(TxtSA01.Text) Then
            If Val(TxtSA01.Text) < 0 Then
                Buffer = Buffer & "AO 01 M" & Format(Math.Abs(Val(TxtSA01.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 01 P" & Format(Val(TxtSA01.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA01.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA01.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA01.Text, 1) = "P") Then
            Buffer = Buffer & "AO 01 " & TxtSA01.Text & vbCrLf
        End If
        If IsNumeric(TxtSA02.Text) Then
            If Val(TxtSA02.Text) < 0 Then
                Buffer = Buffer & "AO 02 M" & Format(Math.Abs(Val(TxtSA02.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 02 P" & Format(Val(TxtSA02.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA02.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA02.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA02.Text, 1) = "P") Then
            Buffer = Buffer & "AO 02 " & TxtSA02.Text & vbCrLf
        End If
        If IsNumeric(TxtSA03.Text) Then
            If Val(TxtSA03.Text) < 0 Then
                Buffer = Buffer & "AO 03 M" & Format(Math.Abs(Val(TxtSA03.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 03 P" & Format(Val(TxtSA03.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA03.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA03.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA03.Text, 1) = "P") Then
            Buffer = Buffer & "AO 03 " & TxtSA03.Text & vbCrLf
        End If
        If IsNumeric(TxtSA04.Text) Then
            If Val(TxtSA04.Text) < 0 Then
                Buffer = Buffer & "AO 04 M" & Format(Math.Abs(Val(TxtSA04.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 04 P" & Format(Val(TxtSA04.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA04.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA04.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA04.Text, 1) = "P") Then
            Buffer = Buffer & "AO 04 " & TxtSA04.Text & vbCrLf
        End If
        If IsNumeric(TxtSA05.Text) Then
            If Val(TxtSA05.Text) < 0 Then
                Buffer = Buffer & "AO 05 M" & Format(Math.Abs(Val(TxtSA05.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 05 P" & Format(Val(TxtSA05.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA05.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA05.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA05.Text, 1) = "P") Then
            Buffer = Buffer & "AO 05 " & TxtSA05.Text & vbCrLf
        End If
        If IsNumeric(TxtSA06.Text) Then
            If Val(TxtSA06.Text) < 0 Then
                Buffer = Buffer & "AO 06 M" & Format(Math.Abs(Val(TxtSA06.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 06 P" & Format(Val(TxtSA06.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA06.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA06.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA06.Text, 1) = "P") Then
            Buffer = Buffer & "AO 06 " & TxtSA06.Text & vbCrLf
        End If
        If IsNumeric(TxtSA07.Text) Then
            If Val(TxtSA07.Text) < 0 Then
                Buffer = Buffer & "AO 07 M" & Format(Math.Abs(Val(TxtSA07.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 07 P" & Format(Val(TxtSA07.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA07.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA07.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA07.Text, 1) = "P") Then
            Buffer = Buffer & "AO 07 " & TxtSA07.Text & vbCrLf
        End If
        If IsNumeric(TxtSA08.Text) Then
            If Val(TxtSA08.Text) < 0 Then
                Buffer = Buffer & "AO 08 M" & Format(Math.Abs(Val(TxtSA08.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 08 P" & Format(Val(TxtSA08.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA08.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA08.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA08.Text, 1) = "P") Then
            Buffer = Buffer & "AO 08 " & TxtSA08.Text & vbCrLf
        End If
        If IsNumeric(TxtSA09.Text) Then
            If Val(TxtSA09.Text) < 0 Then
                Buffer = Buffer & "AO 09 M" & Format(Math.Abs(Val(TxtSA09.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 09 P" & Format(Val(TxtSA09.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA09.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA09.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA09.Text, 1) = "P") Then
            Buffer = Buffer & "AO 09 " & TxtSA09.Text & vbCrLf
        End If
        If IsNumeric(TxtSA10.Text) Then
            If Val(TxtSA10.Text) < 0 Then
                Buffer = Buffer & "AO 10 M" & Format(Math.Abs(Val(TxtSA10.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 10 P" & Format(Val(TxtSA10.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA10.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA10.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA10.Text, 1) = "P") Then
            Buffer = Buffer & "AO 10 " & TxtSA10.Text & vbCrLf
        End If
        If IsNumeric(TxtSA11.Text) Then
            If Val(TxtSA11.Text) < 0 Then
                Buffer = Buffer & "AO 11 M" & Format(Math.Abs(Val(TxtSA11.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 11 P" & Format(Val(TxtSA11.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA11.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA11.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA11.Text, 1) = "P") Then
            Buffer = Buffer & "AO 11 " & TxtSA11.Text & vbCrLf
        End If
        If IsNumeric(TxtSA12.Text) Then
            If Val(TxtSA12.Text) < 0 Then
                Buffer = Buffer & "AO 12 M" & Format(Math.Abs(Val(TxtSA12.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 12 P" & Format(Val(TxtSA12.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA12.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA12.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA12.Text, 1) = "P") Then
            Buffer = Buffer & "AO 12 " & TxtSA12.Text & vbCrLf
        End If
        If IsNumeric(TxtSA13.Text) Then
            If Val(TxtSA13.Text) < 0 Then
                Buffer = Buffer & "AO 13 M" & Format(Math.Abs(Val(TxtSA13.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 13 P" & Format(Val(TxtSA13.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA13.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA13.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA13.Text, 1) = "P") Then
            Buffer = Buffer & "AO 13 " & TxtSA13.Text & vbCrLf
        End If
        If IsNumeric(TxtSA14.Text) Then
            If Val(TxtSA14.Text) < 0 Then
                Buffer = Buffer & "AO 14 M" & Format(Math.Abs(Val(TxtSA14.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 14 P" & Format(Val(TxtSA14.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA14.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA14.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA14.Text, 1) = "P") Then
            Buffer = Buffer & "AO 14 " & TxtSA14.Text & vbCrLf
        End If
        If IsNumeric(TxtSA15.Text) Then
            If Val(TxtSA15.Text) < 0 Then
                Buffer = Buffer & "AO 15 M" & Format(Math.Abs(Val(TxtSA15.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 15 P" & Format(Val(TxtSA15.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA15.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA15.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA15.Text, 1) = "P") Then
            Buffer = Buffer & "AO 15 " & TxtSA15.Text & vbCrLf
        End If
        If IsNumeric(TxtSA16.Text) Then
            If Val(TxtSA16.Text) < 0 Then
                Buffer = Buffer & "AO 16 M" & Format(Math.Abs(Val(TxtSA16.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 16 P" & Format(Val(TxtSA16.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA16.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA16.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA16.Text, 1) = "P") Then
            Buffer = Buffer & "AO 16 " & TxtSA16.Text & vbCrLf
        End If
        If IsNumeric(TxtSA17.Text) Then
            If Val(TxtSA17.Text) < 0 Then
                Buffer = Buffer & "AO 17 M" & Format(Math.Abs(Val(TxtSA17.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 17 P" & Format(Val(TxtSA17.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA17.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA17.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA17.Text, 1) = "P") Then
            Buffer = Buffer & "AO 17 " & TxtSA17.Text & vbCrLf
        End If
        If IsNumeric(TxtSA18.Text) Then
            If Val(TxtSA18.Text) < 0 Then
                Buffer = Buffer & "AO 18 M" & Format(Math.Abs(Val(TxtSA18.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 18 P" & Format(Val(TxtSA18.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA18.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA18.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA18.Text, 1) = "P") Then
            Buffer = Buffer & "AO 18 " & TxtSA18.Text & vbCrLf
        End If
        If IsNumeric(TxtSA19.Text) Then
            If Val(TxtSA19.Text) < 0 Then
                Buffer = Buffer & "AO 19 M" & Format(Math.Abs(Val(TxtSA19.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 19 P" & Format(Val(TxtSA19.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA19.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA19.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA19.Text, 1) = "P") Then
            Buffer = Buffer & "AO 19 " & TxtSA19.Text & vbCrLf
        End If
        If IsNumeric(TxtSA20.Text) Then
            If Val(TxtSA20.Text) < 0 Then
                Buffer = Buffer & "AO 20 M" & Format(Math.Abs(Val(TxtSA20.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 20 P" & Format(Val(TxtSA20.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA20.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA20.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA20.Text, 1) = "P") Then
            Buffer = Buffer & "AO 20 " & TxtSA20.Text & vbCrLf
        End If
        If IsNumeric(TxtSA21.Text) Then
            If Val(TxtSA21.Text) < 0 Then
                Buffer = Buffer & "AO 21 M" & Format(Math.Abs(Val(TxtSA21.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 21 P" & Format(Val(TxtSA21.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA21.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA21.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA21.Text, 1) = "P") Then
            Buffer = Buffer & "AO 21 " & TxtSA21.Text & vbCrLf
        End If
        If IsNumeric(TxtSA22.Text) Then
            If Val(TxtSA22.Text) < 0 Then
                Buffer = Buffer & "AO 22 M" & Format(Math.Abs(Val(TxtSA22.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 22 P" & Format(Val(TxtSA22.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA22.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA22.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA22.Text, 1) = "P") Then
            Buffer = Buffer & "AO 22 " & TxtSA22.Text & vbCrLf
        End If
        If IsNumeric(TxtSA23.Text) Then
            If Val(TxtSA23.Text) < 0 Then
                Buffer = Buffer & "AO 23 M" & Format(Math.Abs(Val(TxtSA23.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 23 P" & Format(Val(TxtSA23.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA23.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA23.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA23.Text, 1) = "P") Then
            Buffer = Buffer & "AO 23 " & TxtSA23.Text & vbCrLf
        End If
        If IsNumeric(TxtSA24.Text) Then
            If Val(TxtSA24.Text) < 0 Then
                Buffer = Buffer & "AO 24 M" & Format(Math.Abs(Val(TxtSA24.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 24 P" & Format(Val(TxtSA24.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA24.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA24.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA24.Text, 1) = "P") Then
            Buffer = Buffer & "AO 24 " & TxtSA24.Text & vbCrLf
        End If
        If IsNumeric(TxtSA25.Text) Then
            If Val(TxtSA25.Text) < 0 Then
                Buffer = Buffer & "AO 25 M" & Format(Math.Abs(Val(TxtSA25.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 25 P" & Format(Val(TxtSA25.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA25.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA25.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA25.Text, 1) = "P") Then
            Buffer = Buffer & "AO 25 " & TxtSA25.Text & vbCrLf
        End If
        If IsNumeric(TxtSA26.Text) Then
            If Val(TxtSA26.Text) < 0 Then
                Buffer = Buffer & "AO 26 M" & Format(Math.Abs(Val(TxtSA26.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 26 P" & Format(Val(TxtSA26.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA26.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA26.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA26.Text, 1) = "P") Then
            Buffer = Buffer & "AO 26 " & TxtSA26.Text & vbCrLf
        End If
        If IsNumeric(TxtSA27.Text) Then
            If Val(TxtSA27.Text) < 0 Then
                Buffer = Buffer & "AO 27 M" & Format(Math.Abs(Val(TxtSA27.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 27 P" & Format(Val(TxtSA27.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA27.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA27.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA27.Text, 1) = "P") Then
            Buffer = Buffer & "AO 27 " & TxtSA27.Text & vbCrLf
        End If
        If IsNumeric(TxtSA28.Text) Then
            If Val(TxtSA28.Text) < 0 Then
                Buffer = Buffer & "AO 28 M" & Format(Math.Abs(Val(TxtSA28.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 28 P" & Format(Val(TxtSA28.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA28.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA28.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA28.Text, 1) = "P") Then
            Buffer = Buffer & "AO 28 " & TxtSA28.Text & vbCrLf
        End If
        If IsNumeric(TxtSA29.Text) Then
            If Val(TxtSA29.Text) < 0 Then
                Buffer = Buffer & "AO 29 M" & Format(Math.Abs(Val(TxtSA29.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 29 P" & Format(Val(TxtSA29.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA29.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA29.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA29.Text, 1) = "P") Then
            Buffer = Buffer & "AO 29 " & TxtSA29.Text & vbCrLf
        End If
        If IsNumeric(TxtSA30.Text) Then
            If Val(TxtSA30.Text) < 0 Then
                Buffer = Buffer & "AO 30 M" & Format(Math.Abs(Val(TxtSA30.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 30 P" & Format(Val(TxtSA30.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA30.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA30.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA30.Text, 1) = "P") Then
            Buffer = Buffer & "AO 30 " & TxtSA30.Text & vbCrLf
        End If
        If IsNumeric(TxtSA31.Text) Then
            If Val(TxtSA31.Text) < 0 Then
                Buffer = Buffer & "AO 31 M" & Format(Math.Abs(Val(TxtSA31.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "AO 31 P" & Format(Val(TxtSA31.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSA31.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSA31.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSA31.Text, 1) = "P") Then
            Buffer = Buffer & "AO 31 " & TxtSA31.Text & vbCrLf
        End If
        If IsNumeric(TxtSL00.Text) Then
            If Val(TxtSL00.Text) < 0 Then
                Buffer = Buffer & "EO 00 M" & Format(Math.Abs(Val(TxtSL00.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 00 P" & Format(Val(TxtSL00.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL00.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL00.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL00.Text, 1) = "P") Then
            Buffer = Buffer & "EO 00 " & TxtSL00.Text & vbCrLf
        End If
        If IsNumeric(TxtSL01.Text) Then
            If Val(TxtSL01.Text) < 0 Then
                Buffer = Buffer & "EO 01 M" & Format(Math.Abs(Val(TxtSL01.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 01 P" & Format(Val(TxtSL01.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL01.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL01.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL01.Text, 1) = "P") Then
            Buffer = Buffer & "EO 01 " & TxtSL01.Text & vbCrLf
        End If
        If IsNumeric(TxtSL02.Text) Then
            If Val(TxtSL02.Text) < 0 Then
                Buffer = Buffer & "EO 02 M" & Format(Math.Abs(Val(TxtSL02.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 02 P" & Format(Val(TxtSL02.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL02.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL02.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL02.Text, 1) = "P") Then
            Buffer = Buffer & "EO 02 " & TxtSL02.Text & vbCrLf
        End If
        If IsNumeric(TxtSL03.Text) Then
            If Val(TxtSL03.Text) < 0 Then
                Buffer = Buffer & "EO 03 M" & Format(Math.Abs(Val(TxtSL03.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 03 P" & Format(Val(TxtSL03.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL03.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL03.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL03.Text, 1) = "P") Then
            Buffer = Buffer & "EO 03 " & TxtSL03.Text & vbCrLf
        End If
        If IsNumeric(TxtSL04.Text) Then
            If Val(TxtSL04.Text) < 0 Then
                Buffer = Buffer & "EO 04 M" & Format(Math.Abs(Val(TxtSL04.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 04 P" & Format(Val(TxtSL04.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL04.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL04.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL04.Text, 1) = "P") Then
            Buffer = Buffer & "EO 04 " & TxtSL04.Text & vbCrLf
        End If
        If IsNumeric(TxtSL05.Text) Then
            If Val(TxtSL05.Text) < 0 Then
                Buffer = Buffer & "EO 05 M" & Format(Math.Abs(Val(TxtSL05.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 05 P" & Format(Val(TxtSL05.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL05.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL05.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL05.Text, 1) = "P") Then
            Buffer = Buffer & "EO 05 " & TxtSL05.Text & vbCrLf
        End If
        If IsNumeric(TxtSL06.Text) Then
            If Val(TxtSL06.Text) < 0 Then
                Buffer = Buffer & "EO 06 M" & Format(Math.Abs(Val(TxtSL06.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 06 P" & Format(Val(TxtSL06.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL06.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL06.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL06.Text, 1) = "P") Then
            Buffer = Buffer & "EO 06 " & TxtSL06.Text & vbCrLf
        End If
        If IsNumeric(TxtSL07.Text) Then
            If Val(TxtSL07.Text) < 0 Then
                Buffer = Buffer & "EO 07 M" & Format(Math.Abs(Val(TxtSL07.Text)), "00.00") & vbCrLf
            Else
                Buffer = Buffer & "EO 07 P" & Format(Val(TxtSL07.Text), "00.00") & vbCrLf
            End If
        ElseIf TxtSL07.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSL07.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSL07.Text, 1) = "P") Then
            Buffer = Buffer & "EO 07 " & TxtSL07.Text & vbCrLf
        End If
        If IsNumeric(TxtSP00.Text) Then
            If Val(TxtSP00.Text) < 0 Then
                Buffer = Buffer & "PO 00 M" & Format(Math.Abs(Val(TxtSP00.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 00 P" & Format(Val(TxtSP00.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP00.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP00.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP00.Text, 1) = "P") Then
            Buffer = Buffer & "PO 00 " & TxtSP00.Text & vbCrLf
        End If
        If IsNumeric(TxtSP01.Text) Then
            If Val(TxtSP01.Text) < 0 Then
                Buffer = Buffer & "PO 01 M" & Format(Math.Abs(Val(TxtSP01.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 01 P" & Format(Val(TxtSP01.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP01.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP01.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP01.Text, 1) = "P") Then
            Buffer = Buffer & "PO 01 " & TxtSP01.Text & vbCrLf
        End If
        If IsNumeric(TxtSP02.Text) Then
            If Val(TxtSP02.Text) < 0 Then
                Buffer = Buffer & "PO 02 M" & Format(Math.Abs(Val(TxtSP02.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 02 P" & Format(Val(TxtSP02.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP02.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP02.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP02.Text, 1) = "P") Then
            Buffer = Buffer & "PO 02 " & TxtSP02.Text & vbCrLf
        End If
        If IsNumeric(TxtSP03.Text) Then
            If Val(TxtSP03.Text) < 0 Then
                Buffer = Buffer & "PO 03 M" & Format(Math.Abs(Val(TxtSP03.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 03 P" & Format(Val(TxtSP03.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP03.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP03.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP03.Text, 1) = "P") Then
            Buffer = Buffer & "PO 03 " & TxtSP03.Text & vbCrLf
        End If
        If IsNumeric(TxtSP04.Text) Then
            If Val(TxtSP04.Text) < 0 Then
                Buffer = Buffer & "PO 04 M" & Format(Math.Abs(Val(TxtSP04.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 04 P" & Format(Val(TxtSP04.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP04.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP04.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP04.Text, 1) = "P") Then
            Buffer = Buffer & "PO 04 " & TxtSP04.Text & vbCrLf
        End If
        If IsNumeric(TxtSP05.Text) Then
            If Val(TxtSP05.Text) < 0 Then
                Buffer = Buffer & "PO 05 M" & Format(Math.Abs(Val(TxtSP05.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 05 P" & Format(Val(TxtSP05.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP05.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP05.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP05.Text, 1) = "P") Then
            Buffer = Buffer & "PO 05 " & TxtSP05.Text & vbCrLf
        End If
        If IsNumeric(TxtSP06.Text) Then
            If Val(TxtSP06.Text) < 0 Then
                Buffer = Buffer & "PO 06 M" & Format(Math.Abs(Val(TxtSP06.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 06 P" & Format(Val(TxtSP06.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP06.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP06.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP06.Text, 1) = "P") Then
            Buffer = Buffer & "PO 06 " & TxtSP06.Text & vbCrLf
        End If
        If IsNumeric(TxtSP07.Text) Then
            If Val(TxtSP07.Text) < 0 Then
                Buffer = Buffer & "PO 07 M" & Format(Math.Abs(Val(TxtSP07.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 07 P" & Format(Val(TxtSP07.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP07.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP07.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP07.Text, 1) = "P") Then
            Buffer = Buffer & "PO 07 " & TxtSP07.Text & vbCrLf
        End If
        If IsNumeric(TxtSP24.Text) Then
            If Val(TxtSP24.Text) < 0 Then
                Buffer = Buffer & "PO 24 M" & Format(Math.Abs(Val(TxtSP24.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 24 P" & Format(Val(TxtSP24.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP24.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP24.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP24.Text, 1) = "P") Then
            Buffer = Buffer & "PO 24 " & TxtSP24.Text & vbCrLf
        End If
        If IsNumeric(TxtSP25.Text) Then
            If Val(TxtSP25.Text) < 0 Then
                Buffer = Buffer & "PO 25 M" & Format(Math.Abs(Val(TxtSP25.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 25 P" & Format(Val(TxtSP25.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP25.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP25.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP25.Text, 1) = "P") Then
            Buffer = Buffer & "PO 25 " & TxtSP25.Text & vbCrLf
        End If
        If IsNumeric(TxtSP26.Text) Then
            If Val(TxtSP26.Text) < 0 Then
                Buffer = Buffer & "PO 26 M" & Format(Math.Abs(Val(TxtSP26.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 26 P" & Format(Val(TxtSP26.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP26.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP26.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP26.Text, 1) = "P") Then
            Buffer = Buffer & "PO 26 " & TxtSP26.Text & vbCrLf
        End If
        If IsNumeric(TxtSP27.Text) Then
            If Val(TxtSP27.Text) < 0 Then
                Buffer = Buffer & "PO 27 M" & Format(Math.Abs(Val(TxtSP27.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 27 P" & Format(Val(TxtSP27.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP27.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP27.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP27.Text, 1) = "P") Then
            Buffer = Buffer & "PO 27 " & TxtSP27.Text & vbCrLf
        End If
        If IsNumeric(TxtSP28.Text) Then
            If Val(TxtSP28.Text) < 0 Then
                Buffer = Buffer & "PO 28 M" & Format(Math.Abs(Val(TxtSP28.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 28 P" & Format(Val(TxtSP28.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP28.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP28.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP28.Text, 1) = "P") Then
            Buffer = Buffer & "PO 28 " & TxtSP28.Text & vbCrLf
        End If
        If IsNumeric(TxtSP29.Text) Then
            If Val(TxtSP29.Text) < 0 Then
                Buffer = Buffer & "PO 29 M" & Format(Math.Abs(Val(TxtSP29.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 29 P" & Format(Val(TxtSP29.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP29.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP29.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP29.Text, 1) = "P") Then
            Buffer = Buffer & "PO 29 " & TxtSP29.Text & vbCrLf
        End If
        If IsNumeric(TxtSP30.Text) Then
            If Val(TxtSP30.Text) < 0 Then
                Buffer = Buffer & "PO 30 M" & Format(Math.Abs(Val(TxtSP30.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 30 P" & Format(Val(TxtSP30.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP30.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP30.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP30.Text, 1) = "P") Then
            Buffer = Buffer & "PO 30 " & TxtSP30.Text & vbCrLf
        End If
        If IsNumeric(TxtSP31.Text) Then
            If Val(TxtSP31.Text) < 0 Then
                Buffer = Buffer & "PO 31 M" & Format(Math.Abs(Val(TxtSP31.Text)), "00.0") & vbCrLf
            Else
                Buffer = Buffer & "PO 31 P" & Format(Val(TxtSP31.Text), "00.0") & vbCrLf
            End If
        ElseIf TxtSP31.Text <> "" AndAlso (Microsoft.VisualBasic.Left(TxtSP31.Text, 1) = "M" OrElse Microsoft.VisualBasic.Left(TxtSP31.Text, 1) = "P") Then
            Buffer = Buffer & "PO 31 " & TxtSP31.Text & vbCrLf
        End If
        If TxtMofa.Text <> "" Then Buffer = Buffer & "MOFA " & TxtMofa.Text & vbCrLf
        If TxtMofe.Text <> "" Then Buffer = Buffer & "MOFE " & TxtMofe.Text & vbCrLf
        If TxtPOS.Text  <> "" Then Buffer = Buffer & "POS "  & TxtPOS.Text  & vbCrLf
        If TxtPli.Text  <> "" Then Buffer = Buffer & "PLI "  & Format(Val(TxtPli.Text), "00") & vbCrLf
        If TxtAltri.Text <> "" Then Buffer = Buffer & TxtAltri.Text.TrimEnd() & vbCrLf
        Try
            System.IO.File.WriteAllText(g_Parametri, Buffer)
        Catch ex As Exception
            MsgBox("Unable to save the file", MsgBoxStyle.OkOnly)
        End Try
    End Function

    ' ── Form events ───────────────────────────────────────────────────────────
    Private Sub FrmParametri_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load
        ClearAllTxt(Me)
    End Sub

    Private Sub FrmParametri_Activated(sender As Object, e As System.EventArgs) Handles MyBase.Activated
        ParACU()
        g_AggPar = False
    End Sub

    ' ── Menu handlers — mnuPar ────────────────────────────────────────────────
    Private Sub mnuParExit_Click(sender As Object, e As System.EventArgs) Handles mnuParExit.Click
        Cursor.Current = Cursors.Default
        If Port_Control(ACU).Socket <> "S" Then
            Port = Canali(ACU).Port_IN
            frmRicezione.tmrTimeoutMSG(ACU).Interval = If(Port_Control(ACU).Time_out * 1000 > 0, Port_Control(ACU).Time_out * 1000, 30000)
        Else
            frmRicezione.tmrTimeoutMSG(ACU).Interval = If(Port_Control(ACU).Time_out * 1000 > 0, Port_Control(ACU).Time_out * 1000, 30000)
        End If
        i_Cont = 1
        b_Parametri = False
        Me.Close()
    End Sub

    Private Sub mnuParFromAcu_Click(sender As Object, e As System.EventArgs) Handles mnuParFromAcu.Click
        Port = Canali(ACU).Port_IN
        If Port_Control(ACU).Socket <> "S" Then
            i_Cont = 2
            Attesa(0.5)
            ParACU()
        Else
            ParACU()
        End If
    End Sub

    Private Sub mnuParFromFile_Click(sender As Object, e As System.EventArgs) Handles mnuParFromFile.Click
        ParFile()
    End Sub

    Private Sub mnuParToAcu_Click(sender As Object, e As System.EventArgs) Handles mnuParToAcu.Click
        If b_Parametri Then Exit Sub
        g_Parametri = Application.StartupPath & "\Appoggio.par"
        If b_Origine Then
            ParSalva()
        Else
            System.IO.File.WriteAllText(g_Parametri, par_Buffer)
        End If
        Cursor.Current = Cursors.WaitCursor
        If m_Reader IsNot Nothing Then
            m_Reader.Close()
            m_Reader.Dispose()
            m_Reader = Nothing
        End If
        m_Reader = New System.IO.StreamReader(g_Parametri)
        b_Parametri = True
        frmRicezione.tmrTimeoutMSG(ACU).Enabled = False
        i_Cont = 2
        tmrParametri.Enabled = True
        LblFine.Text    = ""
        LblFine.Visible = False
    End Sub

    ' ── Menu handlers — mnuSalva (submenu of Save on File) ───────────────────
    Private Sub mnuSalvaNew_Click(sender As Object, e As System.EventArgs) Handles mnuSalvaNew.Click
        Dim s_App As String = InputBox("Enter the file name only")
        g_Parametri = g_DirPar & "\" & s_App & ".PAR"
        ParSalva()
    End Sub

    Private Sub mnuSalvaSelection_Click(sender As Object, e As System.EventArgs) Handles mnuSalvaSelection.Click
        frmBrowse.ShowDialog()
        ParSalva()
    End Sub

    ' ── ParACU ────────────────────────────────────────────────────────────────
    Private Sub ParACU()
        Cursor.Current = Cursors.WaitCursor
        b_int(ACU)      = True
        b_Ricezione(ACU) = True
        s_Parametri     = ""
        InvioACU(Chr(STX) & "07?3A" & Chr(ETX))
        frmRicezione.tmrTimeoutMSG(ACU).Enabled  = False
        frmRicezione.tmrTimeoutMSG(ACU).Interval = 10000
        frmRicezione.tmrTimeoutMSG(ACU).Enabled  = True
    End Sub

    ' ── ParFile ───────────────────────────────────────────────────────────────
    Private Function ParFile() As Boolean
        Dim sBuffer        As String
        Dim sBuffer1       As String
        Dim BufferLenght   As Integer
        Dim ev1            As String
        Dim evTemp         As String
        Dim Gr_file        As Integer
        Dim b_i            As Boolean

        On Error Resume Next
        ParFile = True
        ClearAllTxt(Me)
        frmBrowse.ShowDialog()

        sBuffer      = System.IO.File.ReadAllText(g_Parametri)
        sBuffer1     = sBuffer
        BufferLenght = Len(sBuffer)

        ' replace CR+LF pairs with a space for simpler parsing
        b_i = True
        Do While b_i
            Gr_file = InStr(1, sBuffer, Chr(13), 1)
            If Gr_file > 0 Then
                sBuffer = Mid(sBuffer, 1, Gr_file - 1) & " " & Mid(sBuffer, Gr_file + 2, BufferLenght - Gr_file + 1)
            Else
                b_i = False
            End If
        Loop
        BufferLenght = Len(sBuffer)

        ev1 = ExtractValue("STEP", sBuffer)
        If ev1 <> "---" Then
            TxtSTEP.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "STEP", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 9, Len(sBuffer1) - 9)
        End If
        ev1 = ExtractValue("IT", sBuffer)
        If ev1 <> "---" Then
            TxtIT.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "IT", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("AT", sBuffer)
        If ev1 <> "---" Then
            TxtAT.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "AT", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("ET", sBuffer)
        If ev1 <> "---" Then
            TxtET.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "ET", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("SS", sBuffer)
        If ev1 <> "---" Then
            TxtSS.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "SS", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("XT", sBuffer)
        If ev1 <> "---" Then
            TxtXT.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "XT", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("YT", sBuffer)
        If ev1 <> "---" Then
            TxtYT.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "YT", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("RT", sBuffer)
        If ev1 <> "---" Then
            TxtRT.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "RT", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("BM", sBuffer)
        If ev1 <> "---" Then
            TxtBM.Text = Microsoft.VisualBasic.Left(ev1, 1)
            Gr_file  = InStr(1, sBuffer1, "BM", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 6, Len(sBuffer1) - 6)
        End If
        ev1 = ExtractValue("ATTS", sBuffer)
        If ev1 <> "---" Then
            TxtATTS.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "ATTS", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 9, Len(sBuffer1) - 9)
        End If
        ev1 = ExtractValue("ATTA", sBuffer)
        If ev1 <> "---" Then
            TxtATTA.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "ATTA", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 9, Len(sBuffer1) - 9)
        End If
        ev1 = ExtractValue("ATTE", sBuffer)
        If ev1 <> "---" Then
            TxtATTE.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "ATTE", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 9, Len(sBuffer1) - 9)
        End If
        Gr_file = InStr(1, sBuffer1, "ALI", 1)
        If Gr_file > 0 Then
            TxtAli.Text = Mid$(sBuffer1, Gr_file + 4, 1)
            sBuffer1    = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        Gr_file = InStr(1, sBuffer1, "ELI", 1)
        If Gr_file > 0 Then
            TxtEli.Text = Mid$(sBuffer1, Gr_file + 4, 1)
            sBuffer1    = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        Gr_file = InStr(1, sBuffer1, "MR", 1)
        If Gr_file > 0 Then
            TxtMR.Text = Mid$(sBuffer1, Gr_file + 2, 1)
            sBuffer1   = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 5, Len(sBuffer1) - 5)
        End If
        Gr_file = InStr(1, sBuffer1, "FST", 1)
        If Gr_file > 0 Then
            TxtFST.Text = Mid$(sBuffer1, Gr_file + 3, 1)
            sBuffer1    = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 6, Len(sBuffer1) - 6)
        End If
        ev1 = ExtractValue("MT", sBuffer)
        If ev1 <> "---" Then
            TxtMT.Text = Microsoft.VisualBasic.Left(ev1, 2)
            Gr_file  = InStr(1, sBuffer1, "MT", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        ev1 = ExtractValue("SPCC", sBuffer)
        If ev1 <> "---" Then
            TxtSPCC.Text = Microsoft.VisualBasic.Left(ev1, 6)
            Gr_file  = InStr(1, sBuffer1, "SPCC", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
        End If
        ev1 = ExtractValue("SPCW", sBuffer)
        If ev1 <> "---" Then
            TxtSPCW.Text = Microsoft.VisualBasic.Left(ev1, 6)
            Gr_file  = InStr(1, sBuffer1, "SPCW", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
        End If
        ev1 = ExtractValue("SPDW", sBuffer)
        If ev1 <> "---" Then
            TxtSPDW.Text = Microsoft.VisualBasic.Left(ev1, 5)
            Gr_file  = InStr(1, sBuffer1, "SPDW", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 12, Len(sBuffer1) - 12)
        End If
        ev1 = ExtractValue("SPUP", sBuffer)
        If ev1 <> "---" Then
            TxtSPUP.Text = Microsoft.VisualBasic.Left(ev1, 5)
            Gr_file  = InStr(1, sBuffer1, "SPUP", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 12, Len(sBuffer1) - 12)
        End If
        ev1 = ExtractValue("AOS", sBuffer)
        If ev1 <> "---" Then
            TxtAOS.Text = Microsoft.VisualBasic.Left(ev1, 6)
            Gr_file  = InStr(1, sBuffer1, "AOS", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 12, Len(sBuffer1) - 12)
        End If
        ev1 = ExtractValue("EOS", sBuffer)
        If ev1 <> "---" Then
            TxtEOS.Text = Microsoft.VisualBasic.Left(ev1, 6)
            Gr_file  = InStr(1, sBuffer1, "EOS", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 12, Len(sBuffer1) - 12)
        End If
        Gr_file = InStr(1, sBuffer, "WS", 1)
        If Gr_file > 0 Then
            TxtWSAZ.Text = Mid(sBuffer, Gr_file + 3, 6)
            TxtWSEL.Text = Mid(sBuffer, Gr_file + 9, 6)
            Gr_file  = InStr(1, sBuffer1, "WS", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 17, Len(sBuffer1) - 17)
        End If
        Gr_file = InStr(1, sBuffer, "US", 1)
        If Gr_file > 0 Then
            TxtUSAZ.Text = Mid(sBuffer, Gr_file + 3, 6)
            TxtUSEL.Text = Mid(sBuffer, Gr_file + 9, 6)
            Gr_file  = InStr(1, sBuffer1, "US", 1)
            sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 17, Len(sBuffer1) - 17)
        End If
        ' K09: posizione di back-up (VB6 comune, campi da 7 caratteri)
        Gr_file = InStr(1, sBuffer, "BP", 1)
        If Gr_file > 0 Then
            TxtBKAZ.Text = Mid(sBuffer, Gr_file + 3, 7).Trim()
            TxtBKEL.Text = Mid(sBuffer, Gr_file + 10, 7).Trim()
            Gr_file = InStr(1, sBuffer1, "BP", 1)
            If Gr_file > 0 Then sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 19, Len(sBuffer1) - 19)
        End If

        ' Tabelle di calibrazione per settore (AO/EO/PO) e campi singoli — lettura dal file .PAR salvato
        Gr_file = InStr(1, sBuffer, "AO 00", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA00.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA00.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 00", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 01", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA01.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA01.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 01", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 02", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA02.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA02.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 02", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 03", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA03.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA03.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 03", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 04", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA04.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA04.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 04", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 05", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA05.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA05.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 05", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 06", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA06.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA06.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 06", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 07", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA07.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA07.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 07", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 08", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA08.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA08.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 08", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 09", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA09.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA09.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 09", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 10", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA10.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA10.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 10", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 11", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA11.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA11.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 11", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 12", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA12.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA12.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 12", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 13", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA13.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA13.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 13", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 14", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA14.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA14.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 14", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 15", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA15.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA15.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 15", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 16", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA16.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA16.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 16", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 17", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA17.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA17.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 17", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 18", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA18.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA18.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 18", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 19", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA19.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA19.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 19", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 20", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA20.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA20.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 20", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 21", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA21.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA21.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 21", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 22", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA22.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA22.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 22", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 23", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA23.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA23.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 23", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 24", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA24.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA24.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 24", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 25", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA25.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA25.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 25", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 26", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA26.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA26.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 26", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 27", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA27.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA27.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 27", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 28", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA28.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA28.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 28", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 29", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA29.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA29.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 29", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 30", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA30.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA30.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 30", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "AO 31", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSA31.Text = "-" & Mid(sBuffer, Gr_file + 7, 5)
            Else
                TxtSA31.Text = "+" & Mid(sBuffer, Gr_file + 7, 5)
            End If
            Gr_file = InStr(1, sBuffer1, "AO 31", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 14, Len(sBuffer1) - 14)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 00", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL00.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL00.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 00", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 01", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL01.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL01.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 01", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 02", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL02.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL02.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 02", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 03", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL03.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL03.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 03", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 04", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL04.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL04.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 04", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 05", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL05.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL05.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 05", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 06", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL06.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL06.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 06", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "EO 07", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSL07.Text = "-" & Mid(sBuffer, Gr_file + 7, 6)
            Else
                TxtSL07.Text = "+" & Mid(sBuffer, Gr_file + 7, 6)
            End If
            Gr_file = InStr(1, sBuffer1, "EO 07", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 15, Len(sBuffer1) - 15)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 00", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP00.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP00.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 00", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 01", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP01.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP01.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 01", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 02", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP02.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP02.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 02", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 03", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP03.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP03.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 03", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 04", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP04.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP04.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 04", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 05", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP05.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP05.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 05", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 06", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP06.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP06.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 06", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 07", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP07.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP07.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 07", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 24", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP24.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP24.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 24", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 25", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP25.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP25.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 25", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 26", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP26.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP26.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 26", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 27", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP27.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP27.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 27", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 28", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP28.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP28.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 28", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 29", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP29.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP29.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 29", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 30", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP30.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP30.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 30", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        Gr_file = InStr(1, sBuffer, "PO 31", 1)
        If Gr_file > 0 Then
            sSegno = Mid(sBuffer, Gr_file + 6, 1)
            If sSegno = "M" Then
                TxtSP31.Text = "-" & Mid(sBuffer, Gr_file + 7, 4)
            Else
                TxtSP31.Text = "+" & Mid(sBuffer, Gr_file + 7, 4)
            End If
            Gr_file = InStr(1, sBuffer1, "PO 31", 1)
            If Gr_file > 0 Then
                sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 13, Len(sBuffer1) - 13)
            End If
        End If
        ev1 = ExtractValue("MOFA", sBuffer)
        If ev1 <> "---" Then
            TxtMofa.Text = Microsoft.VisualBasic.Left(ev1, 3)
            Gr_file  = InStr(1, sBuffer1, "MOFA", 1)
            If Gr_file > 0 Then sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 9, Len(sBuffer1) - 9)
        End If
        ev1 = ExtractValue("MOFE", sBuffer)
        If ev1 <> "---" Then
            TxtMofe.Text = Microsoft.VisualBasic.Left(ev1, 3)
            Gr_file  = InStr(1, sBuffer1, "MOFE", 1)
            If Gr_file > 0 Then sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 9, Len(sBuffer1) - 9)
        End If
        ev1 = ExtractValue("POS", sBuffer)
        If ev1 <> "---" Then
            TxtPOS.Text = Microsoft.VisualBasic.Left(ev1, 4)
            Gr_file  = InStr(1, sBuffer1, "POS", 1)
            If Gr_file > 0 Then sBuffer1 = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 8, Len(sBuffer1) - 8)
        End If
        Gr_file = InStr(1, sBuffer1, "PLI", 1)
        If Gr_file > 0 Then
            TxtPli.Text = Mid$(sBuffer1, Gr_file + 4, 2)
            sBuffer1    = Mid$(sBuffer1, 1, Gr_file - 1) & Mid$(sBuffer1, Gr_file + 7, Len(sBuffer1) - 7)
        End If
        b_Origine = True
    End Function

    ' ── SalvaData ─────────────────────────────────────────────────────────────
    Private Function SalvaData() As Boolean
        SalvaData = True
        Try
        rs_App = dbOrbite.OpenRecordset("Parametri", dbOpenDynaset)
        Dim Ind As Integer
        If rs_App.BOF Then
            Ind = 1
        Else
            rs_App.MoveLast()
            Ind = rs_App!NrProg + 1
        End If
        rs_App.FindFirst("Antenna = '" & Antenna & "' and Satellite ='" & s_Satellite & "'")
        If Not rs_App.NoMatch Then
            rs_App.Edit()
        Else
            rs_App.AddNew()
            rs_App!Antenna  = Antenna
            rs_App!Satellite = s_Satellite
            rs_App!ACU      = s_ACUType
            rs_App!NrProg   = Ind
        End If

        If TxtSTEP.Text <> "" Then rs_App!Step = TxtSTEP.Text
        If TxtIT.Text   <> "" Then rs_App!IT   = TxtIT.Text
        If TxtAT.Text   <> "" Then rs_App!AT   = TxtAT.Text
        If TxtET.Text   <> "" Then rs_App!ET   = TxtET.Text
        If TxtSS.Text   <> "" Then rs_App!SS   = TxtSS.Text
        If TxtXT.Text   <> "" Then rs_App!XT   = TxtXT.Text
        If TxtYT.Text   <> "" Then rs_App!YT   = TxtYT.Text
        If TxtRT.Text   <> "" Then rs_App!RT   = TxtRT.Text
        If TxtBM.Text   <> "" Then rs_App!BM   = TxtBM.Text
        If TxtATTS.Text <> "" Then rs_App!ATTS = TxtATTS.Text
        If TxtATTA.Text <> "" Then rs_App!ATTA = TxtATTA.Text
        If TxtATTE.Text <> "" Then rs_App!ATTE = TxtATTE.Text
        If TxtAli.Text  <> "" Then rs_App!ALI  = TxtAli.Text
        If TxtEli.Text  <> "" Then rs_App!ELI  = TxtEli.Text
        If TxtSynAz.Text <> "" Then rs_App!ARS = TxtSynAz.Text
        If TxtSynEl.Text <> "" Then rs_App!ERS = TxtSynEl.Text
        If TxtMR.Text   <> "" Then rs_App!MR   = TxtMR.Text
        If TxtPT.Text   <> "" Then rs_App!PT   = TxtPT.Text
        If TxtFST.Text  <> "" Then rs_App!FST  = TxtFST.Text
        If TxtMT.Text   <> "" Then rs_App!MT   = TxtMT.Text
        If TxtSPCC.Text <> "" Then rs_App!SPCC = TxtSPCC.Text
        If TxtSPCW.Text <> "" Then rs_App!SPCW = TxtSPCW.Text
        If TxtSPDW.Text <> "" Then rs_App!SPDW = TxtSPDW.Text
        If TxtSPUP.Text <> "" Then rs_App!SPUP = TxtSPUP.Text
        If TxtAOS.Text  <> "" Then rs_App!AOS  = TxtAOS.Text
        If TxtEOS.Text  <> "" Then rs_App!EOS  = TxtEOS.Text
        If TxtWSAZ.Text <> "" And TxtWSEL.Text <> "" Then
            rs_App!WS = TxtWSAZ.Text & " " & LTrim(TxtWSEL.Text)
        End If
        If TxtUSAZ.Text <> "" And TxtUSEL.Text <> "" Then
            rs_App!US = TxtUSAZ.Text & " " & LTrim(TxtUSEL.Text)
        End If
        ' Le tabelle AO/EO/PO non sono persistite su database nel VB6
        ' originale di FOC6 (nessun rs_Parametri!SA00 ecc. — solo file
        ' .PAR), solo questi 4 campi singoli lo sono.
        If TxtPli.Text    <> "" Then rs_App!PLI   = TxtPli.Text
        If TxtPOS.Text    <> "" Then rs_App!pos   = TxtPOS.Text
        If TxtSynPol.Text <> "" Then rs_App!POM   = TxtSynPol.Text
        If TxtAltri.Text  <> "" Then rs_App!ALTRI = RTrim(TxtAltri.Text)

        rs_App.Update()
        rs_App.Close()
        Catch ex As Exception
            WriteError(0, "SalvaData: " & ex.Message)
        End Try
    End Function

    ' ── Presenta_Data ─────────────────────────────────────────────────────────
    Private Function Presenta_Data() As Boolean
        Presenta_Data = True
        Try
        If Not g_AggPar Then Exit Function
        ClearAllTxt(Me)
        rs_App = dbOrbite.OpenRecordset("Parametri", dbOpenDynaset)
        rs_App.FindFirst("Antenna = '" & Antenna & "' and Satellite ='" & s_Satellite & "'")
        If rs_App.NoMatch Then
            ' C10: messaggio in barra di stato invece di MsgBox
            FrmMain.stpMessaggio.Text = "There is not parameter"
            FrmMain.TmrMsg.Enabled = True
            rs_App.Close()
            Exit Function
        End If

        If Not IsDBNull(rs_App!IT)   And rs_App!IT   <> "" Then TxtIT.Text   = CStr(rs_App!IT)
        If Not IsDBNull(rs_App!AT)   And rs_App!AT   <> "" Then TxtAT.Text   = CStr(rs_App!AT)
        If Not IsDBNull(rs_App!ET)   And rs_App!ET   <> "" Then TxtET.Text   = CStr(rs_App!ET)
        If Not IsDBNull(rs_App!XT)   And rs_App!XT   <> "" Then TxtXT.Text   = CStr(rs_App!XT)
        If Not IsDBNull(rs_App!YT)   And rs_App!YT   <> "" Then TxtYT.Text   = CStr(rs_App!YT)
        If Not IsDBNull(rs_App!RT)   And rs_App!RT   <> "" Then TxtRT.Text   = CStr(rs_App!RT)
        If Not IsDBNull(rs_App!BM)   And rs_App!BM   <> "" Then TxtBM.Text   = CStr(rs_App!BM)
        If Not IsDBNull(rs_App!AOM)  And rs_App!AOM  <> "" Then TxtSynAz.Text = CStr(rs_App!AOM)
        If Not IsDBNull(rs_App!EOM)  And rs_App!EOM  <> "" Then TxtSynEl.Text = CStr(rs_App!EOM)
        If Not IsDBNull(rs_App!MR)   And rs_App!MR   <> "" Then TxtMR.Text   = CStr(rs_App!MR)
        If Not IsDBNull(rs_App!PT)   And rs_App!PT   <> "" Then TxtPT.Text   = CStr(rs_App!PT)
        If Not IsDBNull(rs_App!FST)  And rs_App!FST  <> "" Then TxtFST.Text  = CStr(rs_App!FST)
        If Not IsDBNull(rs_App!SPCC) And rs_App!SPCC <> "" Then TxtSPCC.Text = CStr(rs_App!SPCC)
        If Not IsDBNull(rs_App!SPCW) And rs_App!SPCW <> "" Then TxtSPCW.Text = CStr(rs_App!SPCW)
        If Not IsDBNull(rs_App!SPDW) And rs_App!SPDW <> "" Then TxtSPDW.Text = CStr(rs_App!SPDW)
        If Not IsDBNull(rs_App!SPUP) And rs_App!SPUP <> "" Then TxtSPUP.Text = CStr(rs_App!SPUP)
        If Not IsDBNull(rs_App!AOS)  And rs_App!AOS  <> "" Then TxtAOS.Text  = CStr(rs_App!AOS)
        If Not IsDBNull(rs_App!EOS)  And rs_App!EOS  <> "" Then TxtEOS.Text  = CStr(rs_App!EOS)
        If Not IsDBNull(rs_App!WS)   And rs_App!WS   <> "" Then
            TxtWSAZ.Text = Mid$(CStr(rs_App!WS), 1, 6)
            TxtWSEL.Text = Mid$(CStr(rs_App!WS), 8, 5)
        End If
        If Not IsDBNull(rs_App!US)   And rs_App!US   <> "" Then
            TxtUSAZ.Text = Mid$(CStr(rs_App!US), 1, 6)
            TxtUSEL.Text = Mid$(CStr(rs_App!US), 8, 5)
        End If
        If Not IsDBNull(rs_App!PLI)   And rs_App!PLI   <> "" Then TxtPli.Text    = CStr(rs_App!PLI)
        If Not IsDBNull(rs_App!pos)   And rs_App!pos   <> "" Then TxtPOS.Text    = CStr(rs_App!pos)
        If Not IsDBNull(rs_App!POM)   And rs_App!POM   <> "" Then TxtSynPol.Text = CStr(rs_App!POM)
        If Not IsDBNull(rs_App!ALTRI) And rs_App!ALTRI <> "" Then TxtAltri.Text  = CStr(rs_App!ALTRI)

        rs_App.Close()
        Catch ex As Exception
            WriteError(0, "Presenta_Data: " & ex.Message)
        End Try
    End Function

    ' ── Timer ─────────────────────────────────────────────────────────────────
    Private Sub tmrParametri_Tick(sender As Object, e As System.EventArgs) Handles tmrParametri.Tick
        Dim Port As Integer
        b_errore = False

        If m_Reader Is Nothing OrElse m_Reader.EndOfStream Then
            Cursor.Current = Cursors.Default
            ' C10: etichetta lblFine invece di MsgBox; b_Ricezione a False
            LblFine.Text = "Order sending parameters"
            LblFine.Visible = True
            If m_Reader IsNot Nothing Then
                m_Reader.Close()
                m_Reader.Dispose()
                m_Reader = Nothing
            End If
            b_Ricezione(ACU) = False
            b_Parametri      = False
            i_Cont           = 0
            If Port_Control(ACU).Socket <> "S" Then
                Port = Canali(ACU).Port_IN
                frmRicezione.tmrTimeoutMSG(ACU).Interval = If(Port_Control(ACU).Time_out * 1000 > 0, Port_Control(ACU).Time_out * 1000, 30000)
            Else
                frmRicezione.tmrTimeoutMSG(ACU).Interval = If(Port_Control(ACU).Time_out * 1000 > 0, Port_Control(ACU).Time_out * 1000, 30000)
            End If
            tmrParametri.Enabled = False
            ParACU()
            Exit Sub
        End If

        LastBackupped_C  = LastBackupped_C + 1
        Cursor.Current   = Cursors.WaitCursor
        SEND1 = m_Reader.ReadLine()

        b_Ricezione(ACU) = False
        i_lun = Len(SEND1)
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_Ricezione(ACU) = False
            b_int(ACU)       = True
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            InvioACU(SEND1)
        End If
        tmrParametri.Enabled = True
    End Sub

    ' ── Button Click handlers ─────────────────────────────────────────────────
    Private Sub CmdAT_Click(sender As Object, e As System.EventArgs) Handles CmdAT.Click
        If TxtAT.Text <> "" Then
            SEND1 = "AT " & Format(Val(TxtAT.Text), "00")
        Else
            Exit Sub
        End If
        b_Ricezione(ACU) = False
        i_lun = Len(SEND1)
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_Ricezione(ACU) = False
            b_int(ACU)       = True
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    Private Sub CmdET_Click(sender As Object, e As System.EventArgs) Handles CmdET.Click
        If TxtET.Text <> "" Then
            SEND1 = "ET " & Format(Val(TxtET.Text), "00")
        Else
            Exit Sub
        End If
        b_Ricezione(ACU) = False
        i_lun = Len(SEND1)
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_int(ACU)       = True
            b_Ricezione(ACU) = False
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    Private Sub CmdXT_Click(sender As Object, e As System.EventArgs) Handles CmdXT.Click
        If TxtXT.Text <> "" Then
            SEND1 = "XT " & Format(Val(TxtXT.Text), "00")
        Else
            Exit Sub
        End If
        b_Ricezione(ACU) = False
        i_lun = Len(SEND1)
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_Ricezione(ACU) = False
            b_int(ACU)       = True
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    Private Sub CmdYT_Click(sender As Object, e As System.EventArgs) Handles CmdYT.Click
        If TxtYT.Text <> "" Then
            SEND1 = "YT " & Format(Val(TxtYT.Text), "00")
        Else
            Exit Sub
        End If
        b_Ricezione(ACU) = False
        i_lun = Len(SEND1)
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_Ricezione(ACU) = False
            b_int(ACU)       = True
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    Private Sub CmdATTS_Click(sender As Object, e As System.EventArgs) Handles CmdATTS.Click
        If TxtATTS.Text <> "" Then
            SEND1 = "ATTS " & Format(Val(TxtATTS.Text), "00")
        Else
            Exit Sub
        End If
        i_lun = Len(SEND1)
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_int(ACU)       = True
            b_Ricezione(ACU) = False
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    Private Sub CmdATTA_Click(sender As Object, e As System.EventArgs) Handles CmdATTA.Click
        If TxtATTA.Text <> "" Then
            SEND1 = "ATTA " & Format(Val(TxtATTA.Text), "00")
        Else
            Exit Sub
        End If
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_int(ACU)       = True
            b_Ricezione(ACU) = False
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    Private Sub CmdATTE_Click(sender As Object, e As System.EventArgs) Handles CmdATTE.Click
        If TxtATTE.Text <> "" Then
            SEND1 = "ATTE " & Format(Val(TxtATTE.Text), "00")
        Else
            Exit Sub
        End If
        If i_lun > 0 Then
            i_lun = Len(SEND1) + 6
            b_Ricezione(ACU) = False
            SEND1 = Format(i_lun, "00") & Mid$(SEND1, 1, Len(SEND1))
            SEND1 = CheckSum(SEND1)
            Invio(SEND1, ACU)
        End If
    End Sub

    ' ── KeyUp handlers ────────────────────────────────────────────────────────
    Private Sub TxtAli_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtAli.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtAli.Text) Then
                If Val(TxtAli.Text) >= 0 And Val(TxtAli.Text) <= 6 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "ALI " & Format(Val(TxtAli.Text), "0") & vbCrLf
                Else
                    TxtAli.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtEli_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtEli.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtEli.Text) Then
                If Val(TxtEli.Text) >= 0 And Val(TxtEli.Text) <= 6 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "ELI " & Format(Val(TxtEli.Text), "0") & vbCrLf
                Else
                    TxtEli.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtATTS_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtATTS.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtATTS.Text) Then
                If Val(TxtATTS.Text) >= 0 And Val(TxtATTS.Text) <= 70 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "ATTS " & Format(Val(TxtATTS.Text), "00") & vbCrLf
                Else
                    TxtATTS.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtATTA_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtATTA.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtATTA.Text) Then
                If Val(TxtATTA.Text) >= 0 And Val(TxtATTA.Text) <= 99 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "ATTA " & Format(Val(TxtATTA.Text), "00") & vbCrLf
                Else
                    TxtATTA.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtATTE_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtATTE.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtATTE.Text) Then
                If Val(TxtATTE.Text) >= 0 And Val(TxtATTE.Text) <= 99 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "ATTE " & Format(Val(TxtATTE.Text), "00") & vbCrLf
                Else
                    TxtATTE.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtBM_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtBM.KeyUp
        ' VB6 TxtBM handler actually validated TxtXT (copy-paste bug in original — preserved)
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtXT.Text) Then
                If Val(TxtXT.Text) >= 5 And Val(TxtXT.Text) <= 35 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "XT " & Format(Val(TxtXT.Text), "00") & vbCrLf
                Else
                    TxtXT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtFST_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtFST.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtFST.Text = "Y" Or TxtFST.Text = "N" Then
                If Not b_Origine Then par_Buffer = par_Buffer & "FST" & TxtFST.Text & vbCrLf
            Else
                TxtFST.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtMR_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtMR.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtMR.Text = "Y" Or TxtMR.Text = "N" Then
                If Not b_Origine Then par_Buffer = par_Buffer & "MR" & TxtMR.Text & vbCrLf
            Else
                TxtMR.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtMT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtMT.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtMT.Text) Then
                If Val(TxtMT.Text) >= 0 And Val(TxtMT.Text) <= 99 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "MT " & Format(Val(TxtMT.Text), "00") & vbCrLf
                Else
                    TxtMT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtAOS_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtAOS.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtAOS.Text) Then
                If Val(TxtAOS.Text) >= -91.999 And Val(TxtAOS.Text) <= 91.999 Then
                    If Not b_Origine Then
                        If Val(TxtAOS.Text) < 0 Then
                            par_Buffer = par_Buffer & "AOS " & "-" & Format(Math.Abs(Val(TxtAOS.Text)), "00.000") & vbCrLf
                        Else
                            par_Buffer = par_Buffer & "AOS " & "+" & Format(Math.Abs(Val(TxtAOS.Text)), "00.000") & vbCrLf
                        End If
                    End If
                Else
                    TxtAOS.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtEOS_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtEOS.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtEOS.Text) Then
                If Val(TxtEOS.Text) >= -91.999 And Val(TxtEOS.Text) <= 91.99 Then
                    If Not b_Origine Then
                        If Val(TxtEOS.Text) < 0 Then
                            par_Buffer = par_Buffer & "EOS " & "-" & Format(Math.Abs(Val(TxtEOS.Text)), "00.000") & vbCrLf
                        Else
                            par_Buffer = par_Buffer & "EOS " & "+" & Format(Math.Abs(Val(TxtEOS.Text)), "00.000") & vbCrLf
                        End If
                    End If
                Else
                    TxtEOS.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtAT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtAT.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtAT.Text) Then
                If Val(TxtAT.Text) >= 1 And Val(TxtAT.Text) <= 99 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "AT " & Format(Val(TxtAT.Text), "00") & vbCrLf
                Else
                    TxtAT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtET_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtET.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtET.Text) Then
                If Val(TxtET.Text) >= 1 And Val(TxtET.Text) <= 99 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "ET " & TxtET.Text & vbCrLf
                Else
                    TxtET.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtIT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtIT.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtIT.Text) Then
                If Val(TxtIT.Text) >= 5 And Val(TxtIT.Text) <= 15 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "IT " & TxtIT.Text & vbCrLf
                Else
                    TxtIT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtPT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtPT.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtPT.Text = "Y" Or TxtPT.Text <= "N" Then
                If Not b_Origine Then par_Buffer = par_Buffer & "PT" & TxtPT.Text & vbCrLf
            Else
                TxtPT.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtSPCC_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtSPCC.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtSPCC.Text) Then
                If Val(TxtSPCC.Text) >= 0 And Val(TxtSPCC.Text) <= 359.999 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "SPCC " & Format(Val(TxtSPCC.Text), "000.000") & vbCrLf
                Else
                    TxtSPCC.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtSPCW_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtSPCW.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtSPCW.Text) Then
                If Val(TxtSPCW.Text) >= 0 And Val(TxtSPCW.Text) <= 359.999 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "SPCW " & Format(Val(TxtSPCW.Text), "000.000") & vbCrLf
                Else
                    TxtSPCW.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtSPDW_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtSPDW.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtSPDW.Text) Then
                If Val(TxtSPDW.Text) >= 0 And Val(TxtSPDW.Text) <= 91.999 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "SPDW " & Format(Val(TxtSPDW.Text), "00.000") & vbCrLf
                Else
                    TxtSPDW.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtSPUP_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtSPUP.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtSPUP.Text) Then
                If Val(TxtSPUP.Text) >= 0 And Val(TxtSPUP.Text) <= 91.999 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "SPUP " & Format(Val(TxtSPUP.Text), "00.000") & vbCrLf
                Else
                    TxtSPUP.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtSynAz_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtSynAz.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtSynAz.Text = "F" Or TxtSynAz.Text = "R" Then
                If Not b_Origine Then par_Buffer = par_Buffer & "A" & TxtSynAz.Text & vbCrLf
            Else
                TxtSynAz.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtSynEl_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtSynEl.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtSynEl.Text = "F" Or TxtSynEl.Text = "R" Then
                If Not b_Origine Then par_Buffer = par_Buffer & "E" & TxtSynEl.Text & vbCrLf
            Else
                TxtSynEl.Text = ""
            End If
        End If
    End Sub

    Private Sub TxtUSAZ_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtUSAZ.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtUSAZ.Text <> "" And TxtUSEL.Text <> "" Then
                par_Buffer = par_Buffer & "US " & Format(Val(TxtUSAZ.Text), "000.000") & " " & Format(Val(LTrim(TxtUSEL.Text)), "00.000") & vbCrLf
            End If
        End If
    End Sub

    Private Sub TxtUSEL_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtUSEL.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtUSAZ.Text <> "" And TxtUSEL.Text <> "" Then
                par_Buffer = par_Buffer & "US " & Format(Val(TxtUSAZ.Text), "000.000") & " " & Format(Val(LTrim(TxtUSEL.Text)), "00.000") & vbCrLf
            End If
        End If
    End Sub

    Private Sub TxtWSAZ_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtWSAZ.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtWSAZ.Text <> "" And TxtWSEL.Text <> "" Then
                par_Buffer = par_Buffer & "WS " & Format(Val(TxtWSAZ.Text), "000.000") & " " & Format(Val(LTrim(TxtWSEL.Text)), "00.000") & vbCrLf
            End If
        End If
    End Sub

    Private Sub TxtWSEL_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtWSEL.KeyUp
        If e.KeyCode = Keys.Return Then
            If TxtWSAZ.Text <> "" And TxtWSEL.Text <> "" Then
                par_Buffer = par_Buffer & "WS " & Format(Val(TxtWSAZ.Text), "000.000") & " " & Format(Val(LTrim(TxtWSEL.Text)), "00.000") & vbCrLf
            End If
        End If
    End Sub

    Private Sub TxtXT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtXT.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtXT.Text) Then
                If Val(TxtXT.Text) >= 5 And Val(TxtXT.Text) <= 35 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "XT " & Format(Val(TxtXT.Text), "00") & vbCrLf
                Else
                    TxtXT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtYT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtYT.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtYT.Text) Then
                If Val(TxtYT.Text) >= 16 And Val(TxtYT.Text) <= 85 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "YT " & Format(Val(TxtYT.Text), "00") & vbCrLf
                Else
                    TxtYT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtRT_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtRT.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtRT.Text) Then
                If Val(TxtRT.Text) >= 1 And Val(TxtRT.Text) <= 99 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "RT " & Format(Val(TxtRT.Text), "00") & vbCrLf
                Else
                    TxtRT.Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub TxtCY_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtCY.KeyUp
        If e.KeyCode = Keys.Return Then
            If IsNumeric(TxtCY.Text) Then
                If Val(TxtCY.Text) >= 1 And Val(TxtCY.Text) <= 5 Then
                    If Not b_Origine Then par_Buffer = par_Buffer & "CY " & Format(Val(TxtCY.Text), "00") & vbCrLf
                Else
                    TxtCY.Text = ""
                End If
            End If
        End If
    End Sub

End Class
