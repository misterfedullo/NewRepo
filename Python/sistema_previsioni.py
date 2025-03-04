import numpy as np
import pandas as pd
from sklearn.linear_model import LinearRegression
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_absolute_error, mean_squared_error, r2_score

class SistemaPrevisioniAdattivo:
    def __init__(self, dimensione_blocco=30, records_test=3, soglia_accuratezza=0.75):
        """
        Inizializza il sistema di previsioni con auto-correzione.
        
        Args:
            dimensione_blocco: Numero di record da utilizzare per l'addestramento
            records_test: Numero di record successivi da testare per valutare l'accuratezza
            soglia_accuratezza: Soglia minima di accuratezza (percentuale di successo)
        """
        self.dimensione_blocco = dimensione_blocco
        self.records_test = records_test
        self.soglia_accuratezza = soglia_accuratezza
        
        # Modelli disponibili
        self.modelli = {
            'regressione_lineare': LinearRegression(),
            'random_forest': RandomForestRegressor(n_estimators=100, random_state=42)
        }
        
        # Modello attualmente in uso
        self.modello_attuale = 'regressione_lineare'
        
        # Parametri di configurazione attuali
        self.configurazione = {
            'finestra_mobile': False,
            'peso_recenti': 1.0,
            'normalizzazione': False
        }
        
        # Storico delle prestazioni
        self.storico_prestazioni = []
        
    def prepara_dati(self, dati, colonne_x, colonna_y, indice_inizio, indice_fine):
        """Prepara i dati per l'addestramento e il test."""
        if indice_fine > len(dati):
            indice_fine = len(dati)
        
        if self.configurazione['normalizzazione']:
            # Normalizzazione dei dati nel blocco corrente
            scaler = dati.iloc[indice_inizio:indice_fine][colonne_x].copy()
            media = scaler.mean()
            std = scaler.std()
            dati_x = (dati[colonne_x].iloc[indice_inizio:indice_fine] - media) / std
        else:
            dati_x = dati[colonne_x].iloc[indice_inizio:indice_fine]
        
        dati_y = dati[colonna_y].iloc[indice_inizio:indice_fine]
        
        # Applica pesi maggiori ai dati più recenti se richiesto
        pesi = None
        if self.configurazione['peso_recenti'] > 1.0:
            pesi = np.ones(len(dati_x))
            for i in range(len(pesi)):
                pesi[i] = 1.0 + (i / len(pesi)) * (self.configurazione['peso_recenti'] - 1.0)
        
        return dati_x, dati_y, pesi
    
    def prepara_dati_test(self, dati, colonne_x, colonna_y, indice_inizio, indice_fine):
        """Prepara i dati di test per la valutazione."""
        if indice_inizio >= len(dati) or indice_inizio + self.records_test > len(dati):
            return None, None
        
        indice_fine = min(indice_inizio + self.records_test, len(dati))
        
        if self.configurazione['normalizzazione']:
            # Usa gli stessi parametri di normalizzazione del training
            scaler = dati.iloc[indice_inizio-self.dimensione_blocco:indice_inizio][colonne_x].copy()
            media = scaler.mean()
            std = scaler.std()
            dati_x_test = (dati[colonne_x].iloc[indice_inizio:indice_fine] - media) / std
        else:
            dati_x_test = dati[colonne_x].iloc[indice_inizio:indice_fine]
        
        dati_y_test = dati[colonna_y].iloc[indice_inizio:indice_fine]
        
        return dati_x_test, dati_y_test
        
    def addestra_modello(self, dati_x, dati_y, pesi=None):
        """Addestra il modello attuale sui dati forniti."""
        modello = self.modelli[self.modello_attuale]
        
        if self.modello_attuale == 'regressione_lineare' and pesi is not None:
            modello.fit(dati_x, dati_y, sample_weight=pesi)
        else:
            modello.fit(dati_x, dati_y)
            
        return modello
    
    def valuta_accuratezza(self, modello, dati_x_test, dati_y_test, tolleranza_percentuale=0.05):
        """
        Valuta l'accuratezza del modello sui dati di test.
        Considera una previsione corretta se è entro una certa tolleranza percentuale.
        """
        previsioni = modello.predict(dati_x_test)
        
        # Calcola quante previsioni sono entro la tolleranza
        corrette = 0
        for i, (reale, previsto) in enumerate(zip(dati_y_test, previsioni)):
            errore_percentuale = abs(reale - previsto) / max(abs(reale), 1e-10)
            if errore_percentuale <= tolleranza_percentuale:
                corrette += 1
        
        # Calcola la percentuale di successo
        percentuale_successo = corrette / len(dati_y_test) if len(dati_y_test) > 0 else 0
        
        return percentuale_successo, previsioni
    
    def ottimizza_configurazione(self):
        """
        Ottimizza la configurazione del sistema in base alle prestazioni recenti.
        """
        if len(self.storico_prestazioni) < 3:
            return
        
        # Se le ultime prestazioni sono sotto la soglia, prova configurazioni alternative
        ultime_prestazioni = [p['accuratezza'] for p in self.storico_prestazioni[-3:]]
        media_recente = sum(ultime_prestazioni) / len(ultime_prestazioni)
        
        if media_recente < self.soglia_accuratezza:
            # Cambia modello
            if self.modello_attuale == 'regressione_lineare':
                self.modello_attuale = 'random_forest'
            else:
                self.modello_attuale = 'regressione_lineare'
                
                # Se abbiamo già provato entrambi i modelli, modifica altri parametri
                if self.configurazione['finestra_mobile'] == False:
                    self.configurazione['finestra_mobile'] = True
                elif self.configurazione['peso_recenti'] == 1.0:
                    self.configurazione['peso_recenti'] = 1.5
                elif self.configurazione['normalizzazione'] == False:
                    self.configurazione['normalizzazione'] = True
                else:
                    # Resetta e prova una combinazione diversa
                    self.configurazione = {
                        'finestra_mobile': True,
                        'peso_recenti': 2.0,
                        'normalizzazione': True
                    }
    
    def genera_previsioni(self, dati, colonne_x, colonna_y):
        """
        Genera previsioni con auto-correzione per l'intero dataset.
        """
        risultati = []
        
        for i in range(0, len(dati) - self.records_test, self.dimensione_blocco):
            # Determina gli indici per il blocco corrente
            indice_inizio = i
            indice_fine = i + self.dimensione_blocco
            
            # Prepara i dati di addestramento
            dati_x, dati_y, pesi = self.prepara_dati(
                dati, colonne_x, colonna_y, indice_inizio, indice_fine
            )
            
            # Addestra il modello
            modello = self.addestra_modello(dati_x, dati_y, pesi)
            
            # Prepara i dati di test (i prossimi records_test record)
            dati_x_test, dati_y_test = self.prepara_dati_test(
                dati, colonne_x, colonna_y, indice_fine, indice_fine + self.records_test
            )
            
            if dati_x_test is None:
                break
                
            # Valuta accuratezza e genera previsioni
            accuratezza, previsioni = self.valuta_accuratezza(modello, dati_x_test, dati_y_test)
            
            # Registra le prestazioni
            self.storico_prestazioni.append({
                'blocco': i // self.dimensione_blocco,
                'indice_inizio': indice_inizio,
                'indice_fine': indice_fine,
                'modello': self.modello_attuale,
                'configurazione': self.configurazione.copy(),
                'accuratezza': accuratezza,
                'previsioni': previsioni,
                'valori_reali': dati_y_test.values
            })
            
            # Aggiungi i risultati
            risultati.append({
                'blocco': i // self.dimensione_blocco,
                'indice_inizio': indice_inizio,
                'indice_fine': indice_fine,
                'indice_previsione_inizio': indice_fine,
                'indice_previsione_fine': indice_fine + self.records_test,
                'modello': self.modello_attuale,
                'accuratezza': accuratezza,
                'previsioni': previsioni,
                'valori_reali': dati_y_test.values,
                'configurazione': self.configurazione.copy()
            })
            
            # Ottimizza configurazione se necessario
            if accuratezza < self.soglia_accuratezza:
                self.ottimizza_configurazione()
                print(f"Blocco {i//self.dimensione_blocco}: Accuratezza {accuratezza:.2f} sotto soglia {self.soglia_accuratezza}.")
                print(f"Nuova configurazione: Modello={self.modello_attuale}, {self.configurazione}")
            else:
                print(f"Blocco {i//self.dimensione_blocco}: Accuratezza {accuratezza:.2f} sopra soglia {self.soglia_accuratezza}.")
        
        return pd.DataFrame(risultati)

# Esempio di utilizzo
def esempio_utilizzo():
    # Crea dataset di esempio
    np.random.seed(42)
    n = 300
    x1 = np.arange(n)
    x2 = np.random.randn(n) * 5
    y = 0.5 * x1 + 2 * np.sin(x1/10) + x2
    
    # Aggiungi un cambio di pattern a metà
    y[150:] = y[150:] + 30 + 0.2 * x1[150:]
    
    df = pd.DataFrame({
        'x1': x1,
        'x2': x2,
        'y': y
    })
    
    # Inizializza il sistema di previsioni
    sistema = SistemaPrevisioniAdattivo(dimensione_blocco=30, records_test=3, soglia_accuratezza=0.75)
    
    # Genera previsioni
    risultati = sistema.genera_previsioni(df, ['x1', 'x2'], 'y')
    
    # Visualizza i risultati
    print("\nRisultati delle previsioni con auto-correzione:")
    for i, risultato in risultati.iterrows():
        print(f"\nBlocco {risultato['blocco']}:")
        print(f"  Modello: {risultato['modello']}")
        print(f"  Configurazione: {risultato['configurazione']}")
        print(f"  Accuratezza: {risultato['accuratezza']:.2f}")
        print("  Previsioni vs Reali:")
        for j, (prev, reale) in enumerate(zip(risultato['previsioni'], risultato['valori_reali'])):
            print(f"    Record {j+1}: Previsto={prev:.2f}, Reale={reale:.2f}, Errore={abs(prev-reale):.2f}")

if __name__ == "__main__":
    esempio_utilizzo()